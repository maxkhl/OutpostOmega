using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using Jitter.LinearMath;
using System.Diagnostics;

namespace OutpostOmega.Game
{
	/// <summary>
	/// Interaction-part of a gameobject
	/// </summary>
	public partial class GameObject
    {
        /// <summary>
        /// Movement vector. Will be applied to the position every frame
        /// </summary>
        public JVector Move
        { 
            get
            {
                return _Move;
            }
            set
            {
                _Move = value;
                NotifyPropertyChanged();
            }
        }
        private JVector _Move;

        /// <summary>
        /// Contains the currently active animations
        /// </summary>
        public List<Animation> Animations 
        { 
            get
            {
                return _Animations;
            }
        }
        private List<Animation> _Animations = new List<Animation>();

        public Animation Animate(string Property, object TargetValue, float Duration, string EasingFunction)
        {
            Animation anim = null;
            foreach(var property in this.GetType().GetProperties())
            {
                if(property.Name.ToLower() == Property.ToLower())
                {
                    Animation.EaseFunction eFunc = Animation.EaseFunction.Linear;
                    Enum.TryParse<Animation.EaseFunction>(EasingFunction, true, out eFunc);


                    anim = new Animation(property, this, TargetValue, Duration, eFunc);
                    Animations.Add(anim);
                    anim.AnimationDone += anim_AnimationDone;
                }
            }
            return anim;
        }

        public Animation Animate(string Property, object TargetValue, float Duration, Animation.EaseFunction EasingFunction)
        {
            if (Animations == null)
                _Animations = new List<Animation>();

            Animation anim = null;
            if (Animations == null)
                _Animations = new List<Animation>();

            foreach (var property in this.GetType().GetProperties())
            {
                if (property.Name.ToLower() == Property.ToLower())
                {
                    anim = new Animation(property, this, TargetValue, Duration, EasingFunction);
                    Animations.Add(anim);
                    anim.AnimationDone += anim_AnimationDone;
                }
            }
            return anim;
        }

        void anim_AnimationDone(Animation sender)
        {
            Animations.Remove(sender);
            sender.Dispose();
        }

        /// <summary>
        /// Used to process a single animation of a gameobject
        /// </summary>
        public class Animation : IDisposable
        {
            private PropertyInfo _Property;
            private GameObject _Instance;
            private object _Start;
            private object _Target;

            public delegate void AnimationDoneHandler(Animation sender);
            public event AnimationDoneHandler AnimationDone;

            public float Duration { get; set; }

            public bool Playing { get; set; }

            public EaseFunction eFunction { get; protected set;}

            public Animation(PropertyInfo Property, GameObject Instance, object Target, float Duration, EaseFunction EaseFunction = EaseFunction.Linear)
            {
                if (!Supports(Property.PropertyType))
                    throw new Exception("Animation of type '" + Property.PropertyType.ToString() + "' not supported");

                _Property = Property;
                _Instance = Instance;
                _Target = Target;
                _Start = Property.GetValue(Instance);
                this.Duration = Duration / 1000;
                this.eFunction = EaseFunction;
                Playing = true;
            }

            public object GetNewValue(object CurrentValue, float ElapsedTime)
            {
                object retStep = null;
                if (_Property.PropertyType == typeof(float))
                    retStep = Ease((float)CurrentValue, (float)_Start, (float)_Target, ElapsedTime) - (float)CurrentValue;
                if (_Property.PropertyType == typeof(decimal))
                    retStep = Ease((float)CurrentValue, (float)_Start, (float)_Target, ElapsedTime) - (float)CurrentValue;
                if (_Property.PropertyType == typeof(JVector))
                    retStep = new JVector(
                        Ease(((JVector)CurrentValue).X, ((JVector)_Start).X, ((JVector)_Target).X, ElapsedTime) - ((JVector)CurrentValue).X,
                        Ease(((JVector)CurrentValue).Y, ((JVector)_Start).Y, ((JVector)_Target).Y, ElapsedTime) - ((JVector)CurrentValue).Y,
                        Ease(((JVector)CurrentValue).Z, ((JVector)_Start).Z, ((JVector)_Target).Z, ElapsedTime) - ((JVector)CurrentValue).Z);
                if (_Property.PropertyType == typeof(JVector2))
                    retStep = new JVector2(
                        Ease(((JVector2)CurrentValue).X, ((JVector2)_Start).X, ((JVector2)_Target).X, ElapsedTime) - ((JVector2)CurrentValue).X,
                        Ease(((JVector2)CurrentValue).Y, ((JVector2)_Start).Y, ((JVector2)_Target).Y, ElapsedTime) - ((JVector2)CurrentValue).Y);


                return retStep;
            }

            private float _CurrentPosition;
            public void Update(float ElapsedTime)
            {
                if (!Playing || Disposing) return;


                _CurrentPosition += ElapsedTime;

                if (_CurrentPosition > Duration)
                {
                    if (AnimationDone != null)
                        AnimationDone(this);

                    Playing = false;
                    return;
                }

                var actValue = _Property.GetValue(_Instance);

                object newValue = null;
                object Step = GetNewValue(actValue, ElapsedTime);

                if (_Property.PropertyType == typeof(float))
                    newValue = (float)actValue + (float)Step;
                if (_Property.PropertyType == typeof(decimal))
                    newValue = (decimal)actValue + (decimal)Step;
                if (_Property.PropertyType == typeof(JVector))
                    newValue = (JVector)actValue + (JVector)Step;
                if (_Property.PropertyType == typeof(JVector2))
                    newValue = (JVector2)actValue + (JVector2)Step;
                /*if (_Property.PropertyType == typeof(JMatrix))
                {
                    var RotationStep = JQuaternion.Diff(JQuaternion.CreateFromMatrix((JMatrix)_Target), JQuaternion.CreateFromMatrix((JMatrix)_Start)) / Duration;
                    

                    JMatrix.CreateFromQuaternion(JQuaternion.CreateFromMatrix((JMatrix)actValue)) * RotationStep);

                    var Position = JVector.TranslationFromMatrix((JMatrix)_Target);
                }*/
                
                _Property.SetValue(_Instance, newValue);
            }

            private bool Supports(Type tType)
            {
                if (tType != typeof(float) &&
                    tType != typeof(decimal) &&
                    tType != typeof(JVector) &&
                    tType != typeof(JVector2) &&
                    tType != typeof(JMatrix))
                    return false;
                else
                    return true;
            }

            private MethodInfo method = null;
            private float Ease(float CurrentValue, float StartValue, float TargetValue, float ElapsedTime)
            {
                if (StartValue == TargetValue) return CurrentValue;

                if (method == null || method.Name != eFunction.ToString())
                    method = this.GetType().GetMethod(eFunction.ToString(), BindingFlags.Public | BindingFlags.Static);

                if(method != null)
                    return (float)(double)method.Invoke(null, new object[4] { (double)_CurrentPosition, (double)StartValue, (double)(TargetValue - StartValue), (double)Duration });

                // Linear is backup and default
                return (((TargetValue - StartValue) * ElapsedTime) / Duration) + CurrentValue;
            }

            public bool Disposing { get; set; }
            public void Dispose()
            {
                Disposing = true;
            }


            /// <summary>
            /// http://wpf-animation.googlecode.com/svn/trunk/src/WPF/Animation/PennerDoubleAnimation.cs
            /// </summary>
            public enum EaseFunction
            {
                Linear,
                QuadEaseOut, QuadEaseIn, QuadEaseInOut, QuadEaseOutIn,
                ExpoEaseOut, ExpoEaseIn, ExpoEaseInOut, ExpoEaseOutIn,
                CubicEaseOut, CubicEaseIn, CubicEaseInOut, CubicEaseOutIn,
                QuartEaseOut, QuartEaseIn, QuartEaseInOut, QuartEaseOutIn,
                QuintEaseOut, QuintEaseIn, QuintEaseInOut, QuintEaseOutIn,
                CircEaseOut, CircEaseIn, CircEaseInOut, CircEaseOutIn,
                SineEaseOut, SineEaseIn, SineEaseInOut, SineEaseOutIn,
                ElasticEaseOut, ElasticEaseIn, ElasticEaseInOut, ElasticEaseOutIn,
                BounceEaseOut, BounceEaseIn, BounceEaseInOut, BounceEaseOutIn,
                BackEaseOut, BackEaseIn, BackEaseInOut, BackEaseOutIn
            }

            #region Linear

            /// <summary>
            /// Easing equation function for a simple linear tweening, with no easing.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double Linear(double t, double b, double c, double d)
            {
                return c * t / d + b;
            }

            #endregion

            #region Expo

            /// <summary>
            /// Easing equation function for an exponential (2^t) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double ExpoEaseOut(double t, double b, double c, double d)
            {
                return (t == d) ? b + c : c * (-Math.Pow(2, -10 * t / d) + 1) + b;
            }

            /// <summary>
            /// Easing equation function for an exponential (2^t) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double ExpoEaseIn(double t, double b, double c, double d)
            {
                return (t == 0) ? b : c * Math.Pow(2, 10 * (t / d - 1)) + b;
            }

            /// <summary>
            /// Easing equation function for an exponential (2^t) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double ExpoEaseInOut(double t, double b, double c, double d)
            {
                if (t == 0)
                    return b;

                if (t == d)
                    return b + c;

                if ((t /= d / 2) < 1)
                    return c / 2 * Math.Pow(2, 10 * (t - 1)) + b;

                return c / 2 * (-Math.Pow(2, -10 * --t) + 2) + b;
            }

            /// <summary>
            /// Easing equation function for an exponential (2^t) easing out/in: 
            /// deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double ExpoEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return ExpoEaseOut(t * 2, b, c / 2, d);

                return ExpoEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Circular

            /// <summary>
            /// Easing equation function for a circular (sqrt(1-t^2)) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double CircEaseOut(double t, double b, double c, double d)
            {
                return c * Math.Sqrt(1 - (t = t / d - 1) * t) + b;
            }

            /// <summary>
            /// Easing equation function for a circular (sqrt(1-t^2)) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double CircEaseIn(double t, double b, double c, double d)
            {
                return -c * (Math.Sqrt(1 - (t /= d) * t) - 1) + b;
            }

            /// <summary>
            /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double CircEaseInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1)
                    return -c / 2 * (Math.Sqrt(1 - t * t) - 1) + b;

                return c / 2 * (Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
            }

            /// <summary>
            /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double CircEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return CircEaseOut(t * 2, b, c / 2, d);

                return CircEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Quad

            /// <summary>
            /// Easing equation function for a quadratic (t^2) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuadEaseOut(double t, double b, double c, double d)
            {
                return -c * (t /= d) * (t - 2) + b;
            }

            /// <summary>
            /// Easing equation function for a quadratic (t^2) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuadEaseIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t + b;
            }

            /// <summary>
            /// Easing equation function for a quadratic (t^2) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuadEaseInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1)
                    return c / 2 * t * t + b;

                return -c / 2 * ((--t) * (t - 2) - 1) + b;
            }

            /// <summary>
            /// Easing equation function for a quadratic (t^2) easing out/in: 
            /// deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuadEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return QuadEaseOut(t * 2, b, c / 2, d);

                return QuadEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Sine

            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double SineEaseOut(double t, double b, double c, double d)
            {
                return c * Math.Sin(t / d * (Math.PI / 2)) + b;
            }

            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double SineEaseIn(double t, double b, double c, double d)
            {
                return -c * Math.Cos(t / d * (Math.PI / 2)) + c + b;
            }

            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double SineEaseInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1)
                    return c / 2 * (Math.Sin(Math.PI * t / 2)) + b;

                return -c / 2 * (Math.Cos(Math.PI * --t / 2) - 2) + b;
            }

            /// <summary>
            /// Easing equation function for a sinusoidal (sin(t)) easing in/out: 
            /// deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double SineEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return SineEaseOut(t * 2, b, c / 2, d);

                return SineEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Cubic

            /// <summary>
            /// Easing equation function for a cubic (t^3) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double CubicEaseOut(double t, double b, double c, double d)
            {
                return c * ((t = t / d - 1) * t * t + 1) + b;
            }

            /// <summary>
            /// Easing equation function for a cubic (t^3) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double CubicEaseIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t * t + b;
            }

            /// <summary>
            /// Easing equation function for a cubic (t^3) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double CubicEaseInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1)
                    return c / 2 * t * t * t + b;

                return c / 2 * ((t -= 2) * t * t + 2) + b;
            }

            /// <summary>
            /// Easing equation function for a cubic (t^3) easing out/in: 
            /// deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double CubicEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return CubicEaseOut(t * 2, b, c / 2, d);

                return CubicEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Quartic

            /// <summary>
            /// Easing equation function for a quartic (t^4) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuartEaseOut(double t, double b, double c, double d)
            {
                return -c * ((t = t / d - 1) * t * t * t - 1) + b;
            }

            /// <summary>
            /// Easing equation function for a quartic (t^4) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuartEaseIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t * t * t + b;
            }

            /// <summary>
            /// Easing equation function for a quartic (t^4) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuartEaseInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1)
                    return c / 2 * t * t * t * t + b;

                return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
            }

            /// <summary>
            /// Easing equation function for a quartic (t^4) easing out/in: 
            /// deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuartEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return QuartEaseOut(t * 2, b, c / 2, d);

                return QuartEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Quintic

            /// <summary>
            /// Easing equation function for a quintic (t^5) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuintEaseOut(double t, double b, double c, double d)
            {
                return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
            }

            /// <summary>
            /// Easing equation function for a quintic (t^5) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuintEaseIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t * t * t * t + b;
            }

            /// <summary>
            /// Easing equation function for a quintic (t^5) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuintEaseInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) < 1)
                    return c / 2 * t * t * t * t * t + b;
                return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
            }

            /// <summary>
            /// Easing equation function for a quintic (t^5) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double QuintEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return QuintEaseOut(t * 2, b, c / 2, d);
                return QuintEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Elastic

            /// <summary>
            /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double ElasticEaseOut(double t, double b, double c, double d)
            {
                if ((t /= d) == 1)
                    return b + c;

                double p = d * .3;
                double s = p / 4;

                return (c * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b);
            }

            /// <summary>
            /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double ElasticEaseIn(double t, double b, double c, double d)
            {
                if ((t /= d) == 1)
                    return b + c;

                double p = d * .3;
                double s = p / 4;

                return -(c * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
            }

            /// <summary>
            /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double ElasticEaseInOut(double t, double b, double c, double d)
            {
                if ((t /= d / 2) == 2)
                    return b + c;

                double p = d * (.3 * 1.5);
                double s = p / 4;

                if (t < 1)
                    return -.5 * (c * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
                return c * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
            }

            /// <summary>
            /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
            /// deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double ElasticEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return ElasticEaseOut(t * 2, b, c / 2, d);
                return ElasticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Bounce

            /// <summary>
            /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double BounceEaseOut(double t, double b, double c, double d)
            {
                if ((t /= d) < (1 / 2.75))
                    return c * (7.5625 * t * t) + b;
                else if (t < (2 / 2.75))
                    return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
                else if (t < (2.5 / 2.75))
                    return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
                else
                    return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
            }

            /// <summary>
            /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double BounceEaseIn(double t, double b, double c, double d)
            {
                return c - BounceEaseOut(d - t, 0, c, d) + b;
            }

            /// <summary>
            /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double BounceEaseInOut(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return BounceEaseIn(t * 2, 0, c, d) * .5 + b;
                else
                    return BounceEaseOut(t * 2 - d, 0, c, d) * .5 + c * .5 + b;
            }

            /// <summary>
            /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out/in: 
            /// deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double BounceEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return BounceEaseOut(t * 2, b, c / 2, d);
                return BounceEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion

            #region Back

            /// <summary>
            /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: 
            /// decelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double BackEaseOut(double t, double b, double c, double d)
            {
                return c * ((t = t / d - 1) * t * ((1.70158 + 1) * t + 1.70158) + 1) + b;
            }

            /// <summary>
            /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: 
            /// accelerating from zero velocity.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double BackEaseIn(double t, double b, double c, double d)
            {
                return c * (t /= d) * t * ((1.70158 + 1) * t - 1.70158) + b;
            }

            /// <summary>
            /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: 
            /// acceleration until halfway, then deceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double BackEaseInOut(double t, double b, double c, double d)
            {
                double s = 1.70158;
                if ((t /= d / 2) < 1)
                    return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
                return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
            }

            /// <summary>
            /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: 
            /// deceleration until halfway, then acceleration.
            /// </summary>
            /// <param name="t">Current time in seconds.</param>
            /// <param name="b">Starting value.</param>
            /// <param name="c">Final value.</param>
            /// <param name="d">Duration of animation.</param>
            /// <returns>The correct value.</returns>
            public static double BackEaseOutIn(double t, double b, double c, double d)
            {
                if (t < d / 2)
                    return BackEaseOut(t * 2, b, c / 2, d);
                return BackEaseIn((t * 2) - d, b + c / 2, c / 2, d);
            }

            #endregion
        }
	}
}

