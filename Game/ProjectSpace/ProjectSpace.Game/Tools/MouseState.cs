using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace OutpostOmega.Game.Tools
{
    /// <summary>
    /// I made dis to build a bridge between the game and opengls shitty mouse state class
    /// </summary>
    public class MouseState
    {
        /// <summary>
        /// Contains button states of the mouse
        /// </summary>
        public Dictionary<MouseButton, bool> MouseButtonStates { get; set; }

        /// <summary>
        /// Current state of the left mouse key
        /// </summary>
        public bool LeftKey
        {
            get
            {
                if (MouseButtonStates.ContainsKey(MouseButton.Left))
                    return MouseButtonStates[MouseButton.Left];
                else
                    return false;
            }
            set
            {
                if (MouseButtonStates.ContainsKey(MouseButton.Left))
                    MouseButtonStates[MouseButton.Left] = value;
                else
                    MouseButtonStates.Add(MouseButton.Left, value);
            }
        }

        /// <summary>
        /// Current state of the middle mouse key
        /// </summary>
        public bool MiddleKey
        {
            get
            {
                if (MouseButtonStates.ContainsKey(MouseButton.Middle))
                    return MouseButtonStates[MouseButton.Middle];
                else
                    return false;
            }
            set
            {
                if (MouseButtonStates.ContainsKey(MouseButton.Middle))
                    MouseButtonStates[MouseButton.Middle] = value;
                else
                    MouseButtonStates.Add(MouseButton.Middle, value);
            }
        }

        /// <summary>
        /// Current state of the right mouse key
        /// </summary>
        public bool RightKey
        {
            get
            {
                if (MouseButtonStates.ContainsKey(MouseButton.Right))
                    return MouseButtonStates[MouseButton.Right];
                else
                    return false;
            }
            set
            {
                if (MouseButtonStates.ContainsKey(MouseButton.Right))
                    MouseButtonStates[MouseButton.Right] = value;
                else
                    MouseButtonStates.Add(MouseButton.Right, value);
            }
        }

        public int X = 0;
        public int Y = 0;

        public int ScrollWheel = 0;

        public int ScrollWheelAbs = 0;
        public float ScrollWheelPrec = 0;

        public MouseState()
        {
            MouseButtonStates = new Dictionary<MouseButton, bool>();
        }

        public MouseState(MouseState mState)
        {
            MouseButtonStates = mState.MouseButtonStates;

            X = mState.X;
            Y = mState.Y;

            ScrollWheel = mState.ScrollWheel;
            ScrollWheelAbs = mState.ScrollWheelAbs;
            ScrollWheelPrec = mState.ScrollWheelPrec;
        }

        public MouseState(OpenTK.Input.MouseState mState)
        {
            MouseButtonStates = new Dictionary<MouseButton, bool>();

            foreach(var mbName in Enum.GetNames(typeof(MouseButton)))
            {
                var mButton = (MouseButton)Enum.Parse(typeof(MouseButton), mbName);
                MouseButtonStates.Add(mButton, mState.IsButtonDown(mButton));
            }

            X = mState.X;
            Y = mState.Y;

            ScrollWheel = mState.ScrollWheelValue;
            ScrollWheelAbs = mState.Wheel;
            ScrollWheelPrec = mState.WheelPrecise;
        }

        /// <summary>
        /// Reads this mouse states button states and returns if this button got pressed
        /// </summary>
        /// <param name="button">Button that should be checked</param>
        /// <returns>True, if button is pressed</returns>
        public bool IsButtonDown(MouseButton button)
        {
            if (MouseButtonStates.ContainsKey(button))
                return MouseButtonStates[button];
            else
                return false;
        }
    }
}
