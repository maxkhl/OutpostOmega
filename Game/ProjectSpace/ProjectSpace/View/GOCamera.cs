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
    /// This camera has the big advantage, that it does not need any additional information.
    /// Construct it and forget about it. Every trigger comes from referenced objects to keep the matrix up to date.
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

                    // Force a camera refresh with new parameters
                    this.LookAt = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(JMatrix.ViewMatrixForward(_Reference.Orientation));
                    this.Position = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(_Reference.Position);
                    Refresh();

                    // Register event to observe orientation and position
                    //_Reference.PropertyChanged += Reference_PropertyChanged;
                }
            }
        }
        private GameObject _Reference;

        /// <summary>
        /// LookAt Vector
        /// </summary>
        private Vector3 LookAt;

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
        /// Handles the PropertyChanged-event of the Reference gameObject.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void Reference_PropertyChanged(GameObject sender, string PropertyName, bool MinorChange)
        {
            GameObject GOSender = sender;

            bool PropChanged = false;
            switch(PropertyName)
            {
                case "Orientation":
                    LookAt = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(GOSender.Forward);
                    PropChanged = true;
                    break;
                case "Position":
                    this.Position = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(GOSender.Position);
                    PropChanged = true;
                    break;
            }

            //if (PropChanged)
            //    Refresh();
        }

        /// <summary>
        /// Triggers a refresh and recalculation of the view and projection matrix
        /// </summary>
        public override void Refresh()
        {
            //this.ViewProjectionMatrix = ;
            base.Refresh();
        }

        public override Matrix4 ViewProjectionMatrix => GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(FieldOfView, (float)this.Width / (float)this.Height, 0.01f, 4000.0f);

        /// <summary>
        /// Returns the view matrix.
        /// </summary>
        public Matrix4 GetViewMatrix()
        {
            /*Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
            lookat.Y = (float)Math.Sin((float)Orientation.Y);
            lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));*/

            LookAt = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Reference.Forward);
            this.Position = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Reference.Position);

            return Matrix4.LookAt(Position, Position + LookAt, Vector3.UnitY);
        }
    }
}
