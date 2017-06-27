using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutpostOmega.Game;
using OpenTK;
using Jitter.LinearMath;

namespace OutpostOmega.View
{
    /// <summary>
    /// Camera that follows a specified GameObject
    /// </summary>
    class GOCamera : Camera
    {
        /// <summary>
        /// Referenced gameObject this camera is synchronized with
        /// </summary>
        public GameObject Reference
        { 
            get
            {
                return _Reference;
            }
            set
            {
                if (_Reference != value)
                {
                    //if (_Reference != null)
                    //    _Reference.PropertyChanged -= Reference_PropertyChanged;

                    _Reference = value;
                }
            }
        }
        private GameObject _Reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="GOCamera"/> class.
        /// </summary>
        /// <param name="Reference">The reference.</param>
        /// <param name="Screen">The assigned screen.</param>
        public GOCamera(GameObject Reference, Drawing.Screen Screen)
            : base(Screen)
        {
            this.Reference = Reference;
        }

        /// <summary>
        /// Generates a new View/Projection Matrix
        /// </summary>
        public override Matrix4 ViewProjectionMatrix
        {
            get
            {
                var LookAt = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Reference.Forward);
                this.Position = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Reference.Position);

                var LookAtMatrix = Matrix4.LookAt(Position, Position + LookAt, Vector3.UnitY);
                var VPM = LookAtMatrix * Matrix4.CreatePerspectiveFieldOfView(FieldOfView, (float)this.Width / (float)this.Height, 0.01f, 4000.0f);

                return VPM;
            }
        }
    }
}
