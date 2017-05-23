using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OutpostOmega.View
{
    class OrbitCamera : Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Orientation = new Vector3((float)Math.PI, 0f, 0f);
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;
        public bool LockCursor { get; set; }
        public float radius = 50;

        private float _theta = (float)Math.PI;
        private float _phi = (float)Math.PI * 2;

        public OrbitCamera(Drawing.Screen Screen)
            : base(Screen)
        {
            Position = Vector3.Zero;
        }

        public Drawing.RenderTarget RenderTarget { get; set; }
        public OrbitCamera(Drawing.RenderTarget renderTarget)
            : base()
        {
            Position = Vector3.Zero;
            this.RenderTarget = renderTarget;
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 lookat = new Vector3();

            lookat.X = Position.X + radius * (float)Math.Cos(_phi) * (float)Math.Sin(_theta);
            lookat.Y = Position.Y + radius * (float)Math.Sin(_phi) * (float)Math.Sin(_theta);
            lookat.Z = Position.Z + radius * (float)Math.Cos(_theta);

            return Matrix4.LookAt(lookat, Position, Vector3.UnitY);
        }
        public void AddRotation(float x, float y)
        {
            _theta += x;
            _phi += y;

            /*x = x * MouseSensitivity;
            y = y * MouseSensitivity;

            Orientation.X = (Orientation.X + x) % ((float)Math.PI * 2.0f);
            Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);*/
        }

        public override void Refresh()
        {
            if (RenderTarget != null)
                this.ViewProjectionMatrix = GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(FieldOfView, this.RenderTarget.Width / (float)this.RenderTarget.Height, 0.01f, 4000.0f);
            else
                this.ViewProjectionMatrix = GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(FieldOfView, this.Screen.Width / (float)this.Screen.Height, 0.01f, 4000.0f);
        }

        public Matrix4 ViewProjectionMatrix { get; set; }
        public void Update(Scene Scene, Vector3 Lookat)
        {

        }
    }
}
