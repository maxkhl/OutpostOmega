using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OutpostOmega.Game;
using System.Diagnostics;


namespace OutpostOmega
{
    /// <summary>
    /// Combination of objects, that build a complete scene
    /// </summary>
    partial class Scene : IDisposable
    {

        /// <summary>
        /// Gwen Renderer
        /// </summary>
        public Gwen.Renderer.OpenTK renderer { get; protected set; }

        /// <summary>
        /// Default Skin
        /// </summary>
        public Gwen.Skin.Base Skin { get; protected set; }

        /// <summary>
        /// Assigned Game Object
        /// </summary>
        public MainGame Game { get; protected set; }

        /// <summary>
        /// Last Scene Update duration
        /// </summary>
        public Stopwatch elapsedUpdateTime { get; protected set; }

        /// <summary>
        /// Last Scene Draw duration
        /// </summary>
        public Stopwatch elapsedDrawTime { get; protected set; }

        /// <summary>
        /// Object disposing
        /// </summary>
        public bool Disposing { get; protected set; }

        /// <summary>
        /// Main GWEN Container for this scene
        /// </summary>
        public Gwen.Control.Canvas Canvas { get; protected set; }
        
        /// <summary>
        /// Stops processing this scene (except UI)
        /// </summary>
        public bool Stop { get; set; }

        private Gwen.Input.OpenTK _input;
        private Matrix4 gwenIdentity;

        public Scene(MainGame Game)
        {
            this.Game = Game;
            elapsedUpdateTime = new Stopwatch();
            elapsedDrawTime = new Stopwatch();

            // Loading the default key sets - this needs to be dynamic in the future to allow custome key layouts
            Tools.Input.LoadDefaultSet();

            #region Gwen Stuff
            renderer = new Gwen.Renderer.OpenTK();
            //Skin = new Gwen.Skin.TexturedBase(renderer, @"Content\UI\DefaultSkin.png");
            var lSkin = Skin;
            CreateDefaultSkin(ref lSkin, renderer);
            Skin = lSkin;
            Canvas = new Gwen.Control.Canvas(Skin);
            _input = new Gwen.Input.OpenTK(Game);
            _input.Initialize(Canvas);
            Canvas.SetSize(Game.Width, Game.Height);
            //canvas.ShouldDrawBackground = true;
            Canvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);
            #endregion

            //Initialize FPS-Counter
            InitFPS();
        }

        public static void CreateDefaultSkin(ref Gwen.Skin.Base Skin, Gwen.Renderer.OpenTK Renderer)
        {
            Skin = new Gwen.Skin.TexturedBase(Renderer, @"Content\UI\MenuSkin.png");
            Skin.Colors.Label.Default = Color.White;
            Skin.Colors.Label.Bright = Color.FromArgb(233, 255, 167); // Bright Lime
            Skin.Colors.Label.Dark = Color.FromArgb(96, 128, 0); // Dark Lime

            Skin.Colors.Button.Normal = Color.Silver;
            Skin.Colors.Button.Hover = Color.FromArgb(193, 254, 9); // OO-Lime

            Skin.Colors.Tree.Normal = Color.White;
            Skin.Colors.Tree.Hover = Color.FromArgb(193, 254, 9); // OO-Lime
            Skin.Colors.Tree.Lines = Color.FromArgb(193, 254, 9); // OO-Lime

            Skin.Colors.Tab.Active.Normal = Color.FromArgb(193, 254, 9); // OO-Lime
            Skin.Colors.Tab.Active.Down = Color.FromArgb(193, 254, 9); // OO-Lime
            Skin.Colors.Tab.Active.Hover = Color.FromArgb(193, 254, 9); // OO-Lime
            Skin.Colors.Tab.Active.Disabled = Color.FromArgb(193, 254, 9); // OO-Lime

            Skin.Colors.Tab.Inactive.Normal = Color.White;
            Skin.Colors.Tab.Inactive.Hover = Color.FromArgb(193, 254, 9); // OO-Lime
        }

        /// <summary>
        /// Initialize Command (happens during update tick and only when this scene got activated)
        /// </summary>
        public virtual void Initialize()
        {


            // Close the loading screen
            Drawing.UI.LoadingScreen.Stop();
        }

        /// <summary>
        /// Draw Call for this Scene
        /// </summary>
        public void Draw()
        {
            if (Disposing)
                return;

            //Update Framecounter
            UpdateFPS(elapsedDrawTime.Elapsed.Milliseconds);

            if (!elapsedDrawTime.IsRunning)
            {
                elapsedDrawTime.Reset();
                elapsedDrawTime.Start();
            }

            GL.GetFloat(GetPName.ProjectionMatrix, out gwenIdentity);

            // render graphics
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            //GL.Enable(EnableCap.CullFace);


            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Tools.Performance.Start("Draw Scene");
            if (!Stop)
            {
#if DEBUG
                DrawSceneFree();
#else
                try
                {
                    DrawSceneFree();
                }
                catch (Exception e) { ThrowCrash(e); }
#endif
            }
            Tools.Performance.Stop("Draw Scene");

            /*if (DebugVoxel)
            {
                Tools.Draw.Line(
                    Tools.Convert.Vector.Jitter_To_OpenGL(dWorld.World.Player.Mob.View.TargetHit),
                    Tools.Convert.Vector.Jitter_To_OpenGL(dWorld.World.Player.Mob.View.TargetHit + Jitter.LinearMath.JVector.Up),
                    Color4.Red);


                for (int i = 0; i < dWorld.World.Structures[0].chunks.Count; i++)
                    Tools.Draw.Line(
                        Tools.Convert.Vector.Jitter_To_OpenGL(dWorld.World.Structures[0].chunks[i].bounds.Min),
                        Tools.Convert.Vector.Jitter_To_OpenGL(dWorld.World.Structures[0].chunks[i].bounds.Max),
                        Color4.Red);

                Tools.Draw.Line(
                    Tools.Convert.Vector.Jitter_To_OpenGL(dWorld.World.Structures[0].chunks[0].Position),
                    Vector3.Zero,
                    Color4.Red);

                Tools.Draw.Line(
                    Tools.Convert.Vector.Jitter_To_OpenGL(dWorld.World.Player.Position),
                    Vector3.Zero,
                    Color4.Red);
            }*/


            // Switch to 2D mode

            GL.Disable(EnableCap.CullFace);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref gwenIdentity);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 ModelReset = Matrix4.Identity;
            GL.LoadMatrix(ref ModelReset);

            Tools.Performance.Start("Draw Orthographic");
            if (!Stop)
            {
#if DEBUG
                DrawSceneOrtho();
#else
                try
                {
                    DrawSceneOrtho();
                }
                catch (Exception e) { ThrowCrash(e); }
#endif
            }
            Tools.Performance.Stop("Draw Orthographic");


            Tools.Performance.Start("Draw GWEN");
            Canvas.RenderCanvas();
            Tools.Performance.Stop("Draw GWEN");

            Game.SwapBuffers();

            Tools.OpenGL.CheckError();
            
            elapsedDrawTime.Stop();
        }

        /// <summary>
        /// Can be overridden to define the drawing in the moment of the draw call, that has no defined matrix
        /// </summary>
        protected virtual void DrawSceneFree()
        { }

        /// <summary>
        /// Can be overridden to define the drawing in an orthogonal space (2D stuffz)
        /// </summary>
        protected virtual void DrawSceneOrtho()
        { }

        /// <summary>
        /// Update Call for this Scene
        /// </summary>
        public void Update()
        {
            if (Disposing)
                return;

            Tools.Performance.Start("Update");
            if (!elapsedUpdateTime.IsRunning)
            {
                elapsedUpdateTime.Reset();
                elapsedUpdateTime.Start();
            }

            // Checks for input updates and fires InputChanged event
            UpdateInput();

            if (!Stop)
            {
#if DEBUG
                UpdateScene();
#else
                try
                {
                    UpdateScene();
                }
                catch (Exception e) { ThrowCrash(e); }
#endif
            }

            elapsedUpdateTime.Restart();
            Tools.Performance.Stop("Update");
        }

        /// <summary>
        /// Can be overridden to define update logic for this scene
        /// </summary>
        protected virtual void UpdateScene()
        {
            this.Canvas.Update(elapsedUpdateTime.Elapsed.Milliseconds);
        }

        /// <summary>
        /// Refresh the Bounds if screen/window-size is changing
        /// </summary>
        public void RefreshView()
        {
            if (Disposing)
                return;

            GL.Viewport(0, 0, Game.Width, Game.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Game.Width, Game.Height, 0, -1, 1);

            renderer.FlushTextCache();
            Canvas.SetSize(Game.Width, Game.Height);

            RefreshSceneView();
        }

        /// <summary>
        /// Gets triggered when the screen bounds (and probably other stuff) changes
        /// </summary>
        protected virtual void RefreshSceneView()
        { }

        /// <summary>
        /// Dispose EVERYTHING related to this scene
        /// </summary>
        public virtual void Dispose()
        {
            this.Disposing = true;

            //Dispose this GWEN instance
            if (renderer != null)
                renderer.Dispose();
            if (Skin != null)
                Skin.Dispose();
            if (Canvas != null)
                Canvas.Dispose();

            //Give it some time to dispose properly
            System.Threading.Thread.Sleep(200);

            Tools.OpenGL.CheckError();
        }


        /// <summary>
        /// Last known crash message (to stop message spamming from crashes in gameloops)
        /// </summary>
        private string LastCrashMessage { get; set; }

        /// <summary>
        /// Shows a messagebox with the given exception and logs the crash (TODO)
        /// </summary>
        protected void ThrowCrash(Exception e)
        {
#if DEBUG
            throw e;
#else
            if (e.Message != LastCrashMessage)
            {
                new Gwen.Control.MessageBox(Canvas, "A minor crash occoured. This is not that bad." + Environment.NewLine + "The game will still run but keep in mind that" + Environment.NewLine + "something might not work properly." + Environment.NewLine + Environment.NewLine + "This is the crash message:" + Environment.NewLine + e.Message, "Minor Crash");
                LastCrashMessage = e.Message;
            }
#endif
        }

        #region Input Methods
        public virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (Disposing)
                return;

#if DEBUG
            _input.ProcessKeyDown(e);
#else
            try
            {
                _input.ProcessKeyDown(e);
            }
            catch (Exception exc) { ThrowCrash(exc); }
#endif
        }
        public virtual void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (Disposing)
                return;

#if DEBUG
            _input.ProcessKeyUp(e);
#else
            try
            {
                _input.ProcessKeyUp(e);
            }
            catch (Exception exc) { ThrowCrash(exc); }
#endif
        }
        public virtual void OnMouseDown(MouseButtonEventArgs e)
        {
            if (Disposing)
                return;

#if DEBUG
            _input.ProcessMouseMessage(e);
#else
            try
            {
                _input.ProcessMouseMessage(e);
            }
            catch (Exception exc) { ThrowCrash(exc); }
#endif
        }
        public virtual void OnMouseUp(MouseButtonEventArgs e)
        {
            if (Disposing)
                return;
            
#if DEBUG
            _input.ProcessMouseMessage(e);
#else
            try
            {
                _input.ProcessMouseMessage(e);
            }
            catch (Exception exc) { ThrowCrash(exc); }
#endif
        }
        public virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (Disposing)
                return;
            
#if DEBUG
            _input.ProcessMouseMessage(e);
#else
            try
            {
                _input.ProcessMouseMessage(e);
            }
            catch (Exception exc) { ThrowCrash(exc); }
#endif
        }
        public virtual void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Disposing)
                return;

#if DEBUG
            _input.ProcessMouseMessage(e);
#else
            try
            {
                _input.ProcessMouseMessage(e);
            }
            catch (Exception exc) { ThrowCrash(exc); }
#endif
        }
        #endregion
    }
}
