using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Gwen.Control;

namespace OutpostOmega.Tools
{
    static class Other
    {
        /// <summary>
        /// Show a not implemented error message ingame
        /// </summary>
        /// <param name="parent">Parent for the messagebox</param>
        public static void NotImplemented(Base parent)
        {
            new MessageBox(parent, "This function is not implemented", "Sry bruh").Show();
        }
        
        public static Vector2 GetPointOnScreen(Vector3 Position, Drawing.Screen Screen, out bool InsideScreen)
        {
            var retPos = new Vector2();
            Vector4 Projected = Vector4.Zero;
            Vector4 PositionV4 = new Vector4(Position - Screen.Camera.Position);
            var ViewProj = Screen.Camera.ViewProjectionMatrix;
            Vector4.Transform(ref PositionV4, ref ViewProj, out Projected);

            InsideScreen = true;
            if(Projected.X < -Projected.W)
            {
                Projected.X = -Projected.W;
                InsideScreen = false;
            }

            if(Projected.X > Projected.W)
            {
                Projected.X = Projected.W;
                InsideScreen = false;
            }

            if(Projected.Y < -Projected.W)
            {
                Projected.Y = -Projected.W;
                InsideScreen = false;
            }

            if(Projected.Y > Projected.W)
            {
                Projected.Y = Projected.W;
                InsideScreen = false;
            }

            Projected.X = Projected.X / Projected.W;
            Projected.Y = Projected.Y / Projected.W;
            Projected.Z = Projected.Z / Projected.W;

            Projected.X = ((float)(((Projected.X + 1) * 0.5) * Screen.Width)) + Screen.X;
            Projected.Y = ((float)(((1.0 - Projected.Y) * 0.5) * Screen.Height)) + Screen.Y;
            //Projected.Z
            return new Vector2(Projected.X, Projected.Y);
            
            /*double inputX = Position.X - Screen.Camera.Position.X,
                   inputY = Position.Y - Screen.Camera.Position.Y,
                   inputZ = Position.Z - Screen.Camera.Position.Z;

            double AspectRatio = Screen.Width / Screen.Height;

            retPos.X = (float)(inputX / (-inputZ * Math.Tan(Screen.Camera.FieldOfView / 2)));
            retPos.Y = (float)((inputY * AspectRatio) / (-inputZ * Math.Tan(Screen.Camera.FieldOfView / 2)));

            retPos.X *= Screen.Width;
            retPos.Y *= Screen.Height;*/

            //return retPos;
        }
    }
}
