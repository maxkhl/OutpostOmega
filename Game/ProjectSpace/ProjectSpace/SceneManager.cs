using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OutpostOmega.Game;
using OpenTK.Input;

namespace OutpostOmega
{
    /// <summary>
    /// Used to manage game scenes
    /// </summary>
    class SceneManager : IDisposable
    {
        /// <summary>
        /// Stored Scenes (if any)
        /// </summary>
        public List<Scene> Scenebuffer { get; protected set; }

        /// <summary>
        /// Currently active Scene
        /// </summary>
        public Scene Active { get; protected set; }
        private int ActiveHandle { get; set; }

        /// <summary>
        /// Assigned Game
        /// </summary>
        public MainGame Game { get; protected set; }

        /// <summary>
        /// Objekt is disposing
        /// </summary>
        public bool Disposing { get; set; }

        /// <summary>
        /// Did the scene change in the last frame?
        /// </summary>
        private bool _SceneChanged = false;

        public SceneManager(MainGame Game)
        {
            this.Game = Game;
            this.Scenebuffer = new List<Scene>();

            // Register Render Event
            this.Game.RenderFrame += RenderFrame;

            // Register Update Event
            this.Game.UpdateFrame += UpdateFrame;

            // Register all the Input Events
            this.Game.KeyDown += Game_KeyDown;
            this.Game.KeyUp += Game_KeyUp;
            this.Game.MouseDown += Game_MouseDown;
            this.Game.MouseUp += Game_MouseUp;
            this.Game.MouseWheel += Game_MouseWheel;
            this.Game.MouseMove += Game_MouseMove;

            // Register Viewport Changes
            this.Game.WindowStateChanged += ResizeViewport;
            this.Game.Resize += ResizeViewport;
        }

        /// <summary>
        /// Adds a new scene to the buffer
        /// </summary>
        /// <param name="Scene">New Scene</param>
        /// <returns>Handle</returns>
        public int AddScene(Scene Scene)
        {
            Scenebuffer.Insert(Scenebuffer.Count, Scene);
            return Scenebuffer.Count-1;
        }

        /// <summary>
        /// Brings a scene from the scenebuffer to the front and activates it
        /// </summary>
        /// <param name="Handle">Scene Handle</param>
        /// <returns>True if successful</returns>
        public bool MakeSceneActive(int Handle)
        {
            if (Handle >= 0 && Handle < Scenebuffer.Count)
            {
                var newScene = Scenebuffer[Handle];

                if (Active != null)
                {
                    Scenebuffer[ActiveHandle] = null;
                    Active.Dispose();
                }

                // Start the loading screen up
                //Drawing.UI.LoadingScreen.Start(newScene, newScene.Canvas);

                _SceneChanged = true;
                Active = Scenebuffer[Handle];
                ActiveHandle = Handle;

                Active?.RefreshView();

                // Close the loading screen
                //Drawing.UI.LoadingScreen.Stop();

                return true;
            }
            else
                return false;
        }

        private int NextSceneHandle = -1;
        /// <summary>
        /// Schedules a threadsafe scenechange.
        /// </summary>
        public void ScheduleSceneChange(int Handle)
        {
            NextSceneHandle = Handle;
        }

        /// <summary>
        /// Draw Call for the active scene
        /// </summary>
        void RenderFrame(object sender, FrameEventArgs e)
        {
            Active?.Draw();
        }

        /// <summary>
        /// Update Call for the active scene
        /// </summary>
        void UpdateFrame(object sender, FrameEventArgs e)
        {
            // Initialize new scene
            if(_SceneChanged)
            {
                _SceneChanged = false;
                if (Active != null)
                    Active.Initialize();
            }

            Active?.Update();

            if (NextSceneHandle != ActiveHandle)
                MakeSceneActive(NextSceneHandle);
        }

        /// <summary>
        /// Disposes all scenes in this manager and releases the Game to be available for a new manager
        /// </summary>
        public void Dispose()
        {
            this.Disposing = true;

            foreach (Scene scene in Scenebuffer)
                if(scene != null)
                    scene.Dispose();

            //Release all Events
            this.Game.RenderFrame -= RenderFrame;
            this.Game.UpdateFrame -= UpdateFrame;
            this.Game.KeyDown -= Game_KeyDown;
            this.Game.KeyUp -= Game_KeyUp;
            this.Game.MouseDown -= Game_MouseDown;
            this.Game.MouseUp -= Game_MouseUp;
            this.Game.MouseWheel -= Game_MouseWheel;
            this.Game.MouseMove -= Game_MouseMove;
            this.Game.WindowStateChanged -= ResizeViewport;
            this.Game.Resize -= ResizeViewport;
        }

        /// <summary>
        /// Triggers a View reset inside the active scene
        /// </summary>
        void ResizeViewport(object sender, EventArgs e)
        {
            Active?.RefreshView();
        }

        #region Input Stuff
        void Game_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            Active?.OnKeyDown(e);
        }
        void Game_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            Active?.OnKeyUp(e);
        }
        void Game_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Active?.OnMouseDown(e);
        }
        void Game_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Active?.OnMouseUp(e);
        }
        void Game_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Active?.OnMouseWheel(e);
        }
        void Game_MouseMove(object sender, MouseMoveEventArgs e)
        {
            Active?.OnMouseMove(e);
        }
        #endregion
    }
}
