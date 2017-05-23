using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ProjectSpace
{
    class Camera
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Orientation = new Vector3((float)Math.PI, 0f, 0f);
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;

        public Matrix4 GetViewMatrix()
        {
            Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
            lookat.Y = (float)Math.Sin((float)Orientation.Y);
            lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
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

        public Matrix4 ViewProjectionMatrix;
        Vector2 lastMousePos = new Vector2();
        public void Update(GameWindow gameWindow)
        {
            this.ViewProjectionMatrix = GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, gameWindow.ClientSize.Width / (float)gameWindow.ClientSize.Height, 0.01f, 40.0f);
            if (gameWindow.Focused)
            {
                Vector2 delta = lastMousePos - new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);

                AddRotation(delta.X, delta.Y);
                ResetCursor(gameWindow);
            }
        }

        public void ResetCursor(GameWindow gameWindow)
        {
            OpenTK.Input.Mouse.SetPosition(gameWindow.Bounds.Left + gameWindow.Bounds.Width / 2, gameWindow.Bounds.Top + gameWindow.Bounds.Height / 2);
            lastMousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
        }

        public void KeyPress(KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'w':
                    Move(0f, 0.1f, 0f);
                    break;
                case 'a':
                    Move(-0.1f, 0f, 0f);
                    break;
                case 's':
                    Move(0f, -0.1f, 0f);
                    break;
                case 'd':
                    Move(0.1f, 0f, 0f);
                    break;
                case 'q':
                    Move(0f, 0f, 0.1f);
                    break;
                case 'e':
                    Move(0f, 0f, -0.1f);
                    break;
            }
        }
    }
}
