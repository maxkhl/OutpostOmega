using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing
{
    public delegate void ScreenBoundsChanged(object sender, int Width, int Height);

    /// <summary>
    /// 
    /// </summary>
    abstract class Screen : iDrawable, iUpdateable
    {
        public static PPShader DefaultShader { get; set; }

        public RenderTarget RenderTarget { get; protected set; }

        /// <summary>
        /// Post Processing Shader
        /// </summary>
        public PPShader Shader
        { 
            get
            {
                if (_Shader == null)
                {
                    if (DefaultShader == null)
                        DefaultShader = new PPShader(new System.IO.FileInfo(@"Content\Shader\Default\Default_PPS.glsl"));

                    return DefaultShader;
                }
                else
                    return _Shader;
            }
            set
            {
                _Shader = value;
            }
        }
        private PPShader _Shader;

        public View.Camera Camera { get; set; }

        public Scenes.Game GameScene { get; protected set; }

        /// <summary>
        /// X-screen-coordinate where the screen should be drawn to
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// X-screen-coordinate where the screen should be drawn to
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Width onscreen of this screen (careful with changing this value too often)
        /// </summary>
        public float Width
        {
            get
            {
                return RenderTarget.Width;
            }
            set
            {
                RenderTarget.Width = (int)value;
                if (BoundsChanged != null)
                    BoundsChanged(this, RenderTarget.Width, RenderTarget.Height);
            }
        }

        /// <summary>
        /// Height onscreen of this screen (careful with changing this value too often)
        /// </summary>
        public float Height
        {
            get
            {
                return RenderTarget.Height;
            }
            set
            {
                RenderTarget.Height = (int)value;
                if (BoundsChanged != null)
                    BoundsChanged(this, RenderTarget.Width, RenderTarget.Height);
            }
        }

        /// <summary>
        /// Decides if this Screen should be scaled (and synchronized) to the full window size or not
        /// </summary>
        public bool Fullscreen 
        { 
            get
            {
                return _Fullscreen;
            }
            set
            {
                _Fullscreen = value;
                if (value)
                    ViewChanged(null, new EventArgs());
            }
        }
        private bool _Fullscreen = false;

        /// <summary>
        /// Fires every time the bounding of this screen has changed (Widht and Height)
        /// </summary>
        public event ScreenBoundsChanged BoundsChanged;

        /// <summary>
        /// Overall width this screen can operate in (window size usualy)
        /// </summary>
        private int MasterWidth { get; set; }

        /// <summary>
        /// Overall height this screen can operate in (window size usualy)
        /// </summary>
        private int MasterHeight { get; set; }

        /// <summary>
        /// Forces the resolution of this screen in any case
        /// </summary>
        public bool ForceResolution { get; set; }


        public Screen(Scenes.Game GameScene, int Width, int Height, RenderTarget renderTarget = null)
        {
            this.GameScene = GameScene;
            this.GameScene.Game.WindowStateChanged += ViewChanged;
            this.GameScene.Game.Resize += ViewChanged;
            if (renderTarget == null)
                this.RenderTarget = new RenderTargets.SimpleRenderTarget(Width, Height);
            else
                this.RenderTarget = renderTarget;

            // Fire it once to make sure we got the newest bounds
            ViewChanged(null, new EventArgs());
        }

        protected virtual void ViewChanged(object sender, EventArgs e)
        {
            if (ForceResolution) return;

            if (Fullscreen) // Only in fullscreen mode
            {
                //Re-scale our screen to the new solution
                if (this.Width != GameScene.Game.Width)
                    this.Width = GameScene.Game.Width;

                if (this.Height != GameScene.Game.Height)
                    this.Height = GameScene.Game.Height;
            }
            else
            {
                // When no fullscreen we need to scale our screen to the new resolution

                // Move Width relative to old width
                this.Width += Width - ((float)MasterWidth * (Width / (float)GameScene.Game.Width));

                // Move Height relative to old height
                this.Height += Height - ((float)MasterHeight * (Height / (float)GameScene.Game.Height));
            }

            // Move X-Coordinate relative to old width
            X += X - ((float)MasterWidth * (X / (float)GameScene.Game.Width));

            // Move Y-Coordinate relative to old height
            Y += Y - ((float)MasterHeight * (Y / (float)GameScene.Game.Height));


            // Pin the size this screen has been refreshed with
            MasterWidth = GameScene.Game.Width;
            MasterHeight = GameScene.Game.Height;
        }

        /// <summary>
        /// Renders the screen to the FBO without drawing it
        /// </summary>
        /// <returns>Texture ID</returns>
        public virtual int Render()
        {
            Tools.OpenGL.CheckError();
            //Set camera
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 viewProj = Camera.ViewProjectionMatrix;
            GL.LoadMatrix(ref viewProj);

            //Draw the scene to the rendertarget
            RenderTarget.Start();
            DrawScene();
            RenderTarget.End();

            //Return the texture id
            return (int)RenderTarget.OutTexture;
        }

        /// <summary>
        /// Draws the screen without rendering the FBO
        /// </summary>
        public virtual void DrawScreen()
        {
            //Draw the rendertarget to the screen
            if (Shader != null)
            {
                Shader.Bind();
                for (int i = 0; i < Shader.PassCount; i++)
                {
                    SetUniform(i);
                    RenderTarget.Draw(this.X, this.Y);
                }
                Shader.UnBind();
            }
            else
                RenderTarget.Draw(this.X, this.Y);
        }

        /// <summary>
        /// Draws the screen without rendering the FBO (scaleable overload)
        /// </summary>
        public void DrawScreen(int Widht, int Height)
        {
            //Draw the rendertarget to the screen
            if (Shader != null)
            {
                Shader.Bind();
                for (int i = 0; i < Shader.PassCount; i++)
                {
                    SetUniform(i);
                    RenderTarget.Draw(this.X, this.Y, Widht, Height);
                }
                Shader.UnBind();
            }
            else
                RenderTarget.Draw(this.X, this.Y, Widht, Height);
        }
        
        /// <summary>
        /// Renders and draws the screen to the specified screen-coordinates
        /// </summary>
        public virtual void Draw()
        {
            Render();
            DrawScreen();
        }

        /// <summary>
        /// Renders and draws the screen to the specified screen-coordinates
        /// </summary>
        public virtual void Draw(RenderOptions options)
        {
            Draw();
        }

        public virtual void Update(double ElapsedTime)
        {

        }

        protected virtual void SetUniform(int Pass)
        {
            GL.Uniform1(Shader.GetUniformLocation("uHeight"), this.Height);
            GL.Uniform1(Shader.GetUniformLocation("Width"), this.Width);
        }

        protected abstract void DrawScene();
    }
}
