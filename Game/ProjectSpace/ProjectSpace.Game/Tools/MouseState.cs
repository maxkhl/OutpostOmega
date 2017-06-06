using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace OutpostOmega.Game.Tools
{
    public class MouseState
    {
        public double ElapsedTime = -1;

        public bool LeftKey = false;
        public bool MiddleKey = false;
        public bool RightKey = false;

        public int X = 0;
        public int Y = 0;

        public int ScrollWheel = 0;

        public int ScrollWheelAbs = 0;
        public float ScrollWheelPrec = 0;

        public MouseState()
        {

        }

        public MouseState(MouseState mState)
        {
            LeftKey = mState.LeftKey;
            MiddleKey = mState.MiddleKey;
            RightKey = mState.RightKey;

            X = mState.X;
            Y = mState.Y;

            ScrollWheel = mState.ScrollWheel;
            ScrollWheelAbs = mState.ScrollWheelAbs;
            ScrollWheelPrec = mState.ScrollWheelPrec;
        }

        public MouseState(OpenTK.Input.MouseState mState)
        {
            LeftKey = mState.LeftButton == ButtonState.Pressed;
            MiddleKey = mState.MiddleButton == ButtonState.Pressed;
            RightKey = mState.RightButton == ButtonState.Pressed;

            X = mState.X;
            Y = mState.Y;

            ScrollWheel = mState.ScrollWheelValue;
            ScrollWheelAbs = mState.Wheel;
            ScrollWheelPrec = mState.WheelPrecise;
        }
    }
}
