using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

namespace OutpostOmega
{
    /// <summary>
    /// Main Game
    /// </summary>
    partial class MainGame : GameWindow
    {
        /// <summary>
        /// Assigned ScreenManager
        /// </summary>
        public SceneManager SceneManager { get; protected set; }

        public MainGame()
            : base(AppSettings.Default.Width, AppSettings.Default.Height, new GraphicsMode(32, 24, 0, 4), "Outpost Omega")
        {

            if (AppSettings.Default.FullScreen)
                this.WindowState = OpenTK.WindowState.Fullscreen;
            else
                this.WindowState = OpenTK.WindowState.Normal;

            if (AppSettings.Default.VSync)
                this.VSync = VSyncMode.On;
            else
                this.VSync = VSyncMode.Off;

            this.Icon = new Icon(@"Content\Image\favicon.ico");

            this.SceneManager = new SceneManager(this);

            this.Closing += MainGame_Closing;

            // Run the game at 60 updates per second
            this.Run(60, 60);
        }

        void MainGame_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Cursor should be visible on startup
            this.CursorVisible = true;

            var Extensions = GL.GetString(StringName.Extensions);
            var Renderer = GL.GetString(StringName.Renderer);

            // Version check
            string Version = GL.GetString(StringName.Version);
            int v01 = int.Parse(Version.Split('.')[0]),
                v02 = int.Parse(Version.Split('.')[1]);
            if ((v01 == 1 && v02 < 5) ||
                v01 < 1) //Everything above version 1.5 is cool
            {
                System.Windows.Forms.MessageBox.Show("At least OpenGL 1.5 needed to run this game (You use " + Version + "). Aborting. Sorry mate.", "OpenGL Version not supported",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.Exit();
                return;
            }
            
            // Enable some shit
            GL.Enable(EnableCap.DepthTest);
            //GL.ShadeModel(ShadingModel.Smooth);
            
            GL.Enable(EnableCap.Texture2D);
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);


            // Load Intro Scene

            if (AppSettings.Default.SkipIntro)
            {
                int Handle = SceneManager.AddScene(new Scenes.Menu(this));
                SceneManager.MakeSceneActive(Handle);
            }
            else
            {
                int Handle = SceneManager.AddScene(new Scenes.Intro(this));
                SceneManager.MakeSceneActive(Handle);
            }
        }

        public override void Dispose()
        {
            // Releases pretty much every ressource the game has
            SceneManager.Dispose();
            
            base.Dispose();
        }

        /*protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if(e.KeyChar == 'p')
                new Drawing.UI.Performance(SceneManager.Active, SceneManager.Active.Canvas).Show();

            base.OnKeyPress(e);
        }*/
    }
}
