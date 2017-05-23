using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OutpostOmega.View
{
    abstract class Camera
    {
        /// <summary>
        /// Optional screen that will trigger the Refresh-Call
        /// </summary>
        protected Drawing.Screen Screen 
        { 
            get
            {
              return _Screen;
            }
            set
            {
                if(_Screen != value)
                {
                    if(_Screen != null)
                        _Screen.BoundsChanged -=_Screen_BoundsChanged;

                    _Screen = value;
                    _Screen.BoundsChanged += _Screen_BoundsChanged;
                }
            }
        }
        private Drawing.Screen _Screen;

        public float FieldOfView = 1.3f;

        void _Screen_BoundsChanged(object sender, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            this.Refresh();
        }

        /// <summary>
        /// Position of the camera
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Width of this Viewport
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of this Viewport
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The View and Projection Matrix of this camera
        /// </summary>
        public Matrix4 ViewProjectionMatrix { get; set; }


        public bool LockCursor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        public Camera()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="Screen">A Screen, this camera will be synced with</param>
        public Camera(Drawing.Screen Screen)
        {
            this.Screen = Screen;
        }

        /// <summary>
        /// Triggers a refresh and recalculation of the view and projection matrix
        /// </summary>
        public virtual void Refresh()
        { }
    }
}
