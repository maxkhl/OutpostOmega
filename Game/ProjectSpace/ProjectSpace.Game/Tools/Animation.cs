using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OutpostOmega.Game.Tools
{
    /// <summary>
    /// Takes care of animating a given value
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Field, this animation is accessing
        /// </summary>
        public PropertyInfo Field { private set; get; }

        /// <summary>
        /// Instance, this animation is accessing
        /// </summary>
        public object Instance { private set; get; }

        /// <summary>
        /// Target Value the animation is moving towards
        /// </summary>
        public object Value { private set; get; }

        /// <summary>
        /// Duration of the animation in MS
        /// </summary>
        public float Duration { private set; get; }

        /// <summary>
        /// Easing function the animation is using
        /// </summary>
        public Easing.EaseFunction Function { private set; get; }

        /// <summary>
        /// Time the animation started (first update run after activation)
        /// </summary>
        public float AnimationStart { private set; get; }

        /// <summary>
        /// Time the animation will end or ended already
        /// </summary>
        public float AnimationEnd { private set; get; }

        /// <summary>
        /// Target Value at the start of the animation
        /// </summary>
        public object ValueStart { private set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Animation"/> is active.
        /// </summary>
        public bool Active { get; set; }

        public Animation(PropertyInfo Field, object Instance)
        {

            this.Instance = Instance;
            this.Field = Field;
            this.Active = false;
        }

        public void Animate(object Target, int Duration, Easing.EaseFunction Function)
        {
            this.Reset();
            this.Value = Target;
            this.Duration = Duration;
            this.Function = Function;
            this.Active = true;
        }

        private bool WasActive = false;
        private float _CurrentPosition;
        public void Update(float ElapsedTime)
        {
            if (!WasActive && Active)
            {
                this.AnimationStart = _CurrentPosition;
                this.AnimationEnd = _CurrentPosition + Duration;
                this.ValueStart = this.Field.GetValue(this.Instance);
            }

            if (this.Active)
            {
                _CurrentPosition += ElapsedTime;

                if (_CurrentPosition > this.AnimationEnd)
                {
                    this.Field.SetValue(this.Instance, this.Value);
                    Reset();

                    if (OnAnimationDone != null)
                    {
                        OnAnimationDone(this);
                    }
                }
                else
                {
                    WasActive = true;
                    var newValue = Animate(_CurrentPosition - AnimationStart, this.ValueStart, this.Value, (float)Duration);
                    this.Field.SetValue(this.Instance, newValue);
                }
            }
            else
                WasActive = false;
        }

        public void Reset()
        {
            WasActive = false;
            Active = false;
            this.AnimationStart = 0;
            this.AnimationEnd = 0;
            this.ValueStart = 0;
            this.Duration = 0;
            this.Value = 0;
        }

        public object Add(object o1, object o2)
        {
            if (o1.GetType() != o2.GetType())
                throw new Exception("Types of values in animation do not match");

            if (typeof(float).IsAssignableFrom(o1.GetType()))
                return (float)o1 + (float)o2;

            if (typeof(int).IsAssignableFrom(o1.GetType()))
                return (int)o1 + (int)o2;

            if (typeof(System.Drawing.Color).IsAssignableFrom(o1.GetType()))
                return
                    System.Drawing.Color.FromArgb(((System.Drawing.Color)o1).ToArgb()                    
                     + ((System.Drawing.Color)o2).ToArgb());

            // JITTER
            if (typeof(Jitter.LinearMath.JVector2).IsAssignableFrom(o1.GetType()))
                return (Jitter.LinearMath.JVector2)o1 + (Jitter.LinearMath.JVector2)o2;

            if (typeof(Jitter.LinearMath.JVector).IsAssignableFrom(o1.GetType()))
                return (Jitter.LinearMath.JVector)o1 + (Jitter.LinearMath.JVector)o2;

            // OPENTK
            if (typeof(OpenTK.Vector2).IsAssignableFrom(o1.GetType()))
                return (OpenTK.Vector2)o1 + (OpenTK.Vector2)o2;

            if (typeof(OpenTK.Vector3).IsAssignableFrom(o1.GetType()))
                return (OpenTK.Vector3)o1 + (OpenTK.Vector3)o2;

            throw new Exception(String.Format("The type {0} is not compatible with animations", o1.GetType().ToString()));
        }

        private object Animate(float Time, object ValueStart, object Value, float Duration)
        {
            if (Value.GetType() != ValueStart.GetType())
                throw new Exception("Types of values in animation do not match");

            if (typeof(int).IsAssignableFrom(Value.GetType()))
                return (int)Easing.Ease(this.Function, Time, (float)(int)ValueStart, (float)(int)Value, (float)Duration);

            if (typeof(float).IsAssignableFrom(Value.GetType()))
                return Easing.Ease(this.Function, Time, (float)ValueStart, (float)Value, (float)Duration);

            if (typeof(System.Drawing.Color).IsAssignableFrom(Value.GetType()))
                return System.Drawing.Color.FromArgb(
                        (int)Easing.Ease(this.Function, Time, (float)((System.Drawing.Color)ValueStart).A, (float)((System.Drawing.Color)Value).A, (float)Duration),
                        (int)Easing.Ease(this.Function, Time, (float)((System.Drawing.Color)ValueStart).R, (float)((System.Drawing.Color)Value).R, (float)Duration),
                        (int)Easing.Ease(this.Function, Time, (float)((System.Drawing.Color)ValueStart).G, (float)((System.Drawing.Color)Value).G, (float)Duration),
                        (int)Easing.Ease(this.Function, Time, (float)((System.Drawing.Color)ValueStart).B, (float)((System.Drawing.Color)Value).B, (float)Duration)
                    );

            // JITTER

            if (typeof(Jitter.LinearMath.JVector2).IsAssignableFrom(Value.GetType()))
                return new Jitter.LinearMath.JVector2(
                        Easing.Ease(this.Function, Time, ((Jitter.LinearMath.JVector2)ValueStart).X, ((Jitter.LinearMath.JVector2)Value).X, (float)Duration),
                        Easing.Ease(this.Function, Time, ((Jitter.LinearMath.JVector2)ValueStart).Y, ((Jitter.LinearMath.JVector2)Value).Y, (float)Duration)
                    );

            if (typeof(Jitter.LinearMath.JVector).IsAssignableFrom(Value.GetType()))
                return new Jitter.LinearMath.JVector(
                        Easing.Ease(this.Function, Time, ((Jitter.LinearMath.JVector)ValueStart).X, ((Jitter.LinearMath.JVector)Value).X, (float)Duration),
                        Easing.Ease(this.Function, Time, ((Jitter.LinearMath.JVector)ValueStart).Y, ((Jitter.LinearMath.JVector)Value).Y, (float)Duration),
                        Easing.Ease(this.Function, Time, ((Jitter.LinearMath.JVector)ValueStart).Z, ((Jitter.LinearMath.JVector)Value).Z, (float)Duration)
                    );

            // OPENTK

            if (typeof(OpenTK.Vector2).IsAssignableFrom(Value.GetType()))
                return new OpenTK.Vector2(
                        Easing.Ease(this.Function, Time, ((OpenTK.Vector2)ValueStart).X, ((OpenTK.Vector2)Value).X, (float)Duration),
                        Easing.Ease(this.Function, Time, ((OpenTK.Vector2)ValueStart).Y, ((OpenTK.Vector2)Value).Y, (float)Duration)
                    );

            if (typeof(OpenTK.Vector3).IsAssignableFrom(Value.GetType()))
                return new OpenTK.Vector3(
                        Easing.Ease(this.Function, Time, ((OpenTK.Vector3)ValueStart).X, ((OpenTK.Vector3)Value).X, (float)Duration),
                        Easing.Ease(this.Function, Time, ((OpenTK.Vector3)ValueStart).Y, ((OpenTK.Vector3)Value).Y, (float)Duration),
                        Easing.Ease(this.Function, Time, ((OpenTK.Vector3)ValueStart).Z, ((OpenTK.Vector3)Value).Z, (float)Duration)
                    );

            throw new Exception(String.Format("The type {0} is not compatible with animations", Value.GetType().ToString()));
        }

        public delegate void OnAnimationDoneHandler(Animation sender);
        public event OnAnimationDoneHandler OnAnimationDone;
    }
}
