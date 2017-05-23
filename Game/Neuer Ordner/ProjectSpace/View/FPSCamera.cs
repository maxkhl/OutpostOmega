using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OutpostOmega.View
{
    class FPSCamera : Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Orientation = new Vector3((float)Math.PI, 0f, 0f);
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;

        public FPSCamera(Drawing.Screen Screen)
            : base(Screen)
        {
            LockCursor = false;
            Position = Vector3.Zero;
        }

        public Matrix4 GetViewMatrix(Vector3 Lookat)
        {
            /*Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
            lookat.Y = (float)Math.Sin((float)Orientation.Y);
            lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));*/

            return Matrix4.LookAt(Position, Position + Lookat, Vector3.UnitY);
        }
        public void Move(float x, float y, float z)
        {
            Vector3 offset = new Vector3();

            Vector3 forward = new Vector3((float)Math.Sin((float)Orientation.X), 0, (float)Math.Cos((float)Orientation.X));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += x * right;
            offset += y * forward;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            Position += offset;
        }
        public void AddRotation(float x, float y)
        {
            x = x * MouseSensitivity;
            y = y * MouseSensitivity;

            Orientation.X = (Orientation.X + x) % ((float)Math.PI * 2.0f);
            Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
        }

        public Matrix4 ViewProjectionMatrix { get; set; }
        public bool LockCursor { get; set; }
        public void Update(Scene Scene, Vector3 Lookat)
        {
            this.ViewProjectionMatrix = GetViewMatrix(Lookat) * Matrix4.CreatePerspectiveFieldOfView(FieldOfView, Scene.Game.ClientSize.Width / (float)Scene.Game.ClientSize.Height, 0.01f, 4000.0f);
            /*if (Scene.Game.Focused && LockCursor)
            {
                Vector2 delta = Scene.Game.MouseData.LastPosition - new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
                
                AddRotation(delta.X, delta.Y);
            }*/
        }
    }
}
