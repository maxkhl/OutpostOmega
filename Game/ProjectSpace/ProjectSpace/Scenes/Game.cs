using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OutpostOmega.Game;
using System.Diagnostics;
using OpenTK.Input;
using System.Drawing;

namespace OutpostOmega.Scenes
{
    /// <summary>
    /// Ingame Scene
    /// </summary>
    class Game : Scene
    {
        /// <summary>
        /// World for this scene
        /// </summary>
        public World World { get; protected set; }

        /// <summary>
        /// World drawer for this scene
        /// </summary>
        public Drawing.Game.WorldDrawer Drawer { get; protected set; }

        /// <summary>
        /// Screens drawn in this scene
        /// </summary>
        public List<Drawing.Screen> Screens { get; protected set; }

        /// <summary>
        /// Screens that are processed in the background
        /// </summary>
        public List<Drawing.Screen> BackgroundScreens { get; protected set; }

        /// <summary>
        /// Last World Update duration
        /// </summary>
        public Stopwatch ElapsedWorldUpdateTime { get; protected set; }

        /// <summary>
        /// A list of all loaded mods
        /// </summary>
        public List<OutpostOmega.Game.Lua.ModPack> Mods { get; protected set; }

        /// <summary>
        /// HUD Skin
        /// </summary>
        public Gwen.Skin.Base HUDSkin { get; protected set; }

        /// <summary>
        /// True if mouse is active, false if camera is controlled
        /// </summary>
        public bool MouseMode
        {
            get
            {
                return _mouseMode;
            }
            set
            {
                _mouseMode = value;

                if (value)
                    Game.UnlockCursor();
                else
                    Game.LockCursor();
            }
        }
        private bool _mouseMode = true;

        /// <summary>
        /// The ingame console
        /// </summary>
        public Drawing.UI.Console Console { get; set; }

        Gwen.Control.Label FPSLabel;
        World tmpWrld;
        public Game(MainGame game, World World)
            : base(game)
        {
            tmpWrld = World;
            //this.World = World;
        }


        Gwen.Control.Label UILabel;
        Gwen.Control.ImagePanel Crosshair;
        public override void Initialize()
        {
            // Make sure this happens first as it initializes gwen
            base.Initialize();


            tmpWrld.UICall += World_UICall;
            this.KeyStateChanged += Game_InputChanged;
            this.MouseMoved += Game_MouseMoved;
            HUDSkin = new Gwen.Skin.TexturedBase(renderer, @"Content\UI\HUD.png");
            HUDSkin.Colors.Label.Default = Color.White;
            HUDSkin.Colors.Button.Normal = Color.Silver;

            Console = new Drawing.UI.Console(this, Canvas);

            Tools.Input.LoadDefaultSet();

            FPSLabel = new Gwen.Control.Label(Canvas);
            //FPSLabel.Text = "Project Space - Dev Build";
            FPSLabel.SetPosition(5, 1);
            FPSLabel.TextColor = Color.White;

            var modfolder = new System.IO.DirectoryInfo(AppSettings.Default.ModFolder);
            var modfiles = new List<System.IO.FileInfo>();
            Mods = new List<OutpostOmega.Game.Lua.ModPack>();
            if (modfolder.Exists)
            {
                modfiles = OutpostOmega.Game.Lua.ModPack.SearchFolder(modfolder);
                foreach (var modfile in modfiles)
                    Mods.Add(tmpWrld.LoadMod(modfile));
            }
            else
                throw new Exception("Modfolder not found at '" + modfolder.FullName + "'");

            foreach (var Mod in Mods)
            {
                Console.Message(string.Format("{0} loaded", Mod.ID));
            }

            Screens = new List<Drawing.Screen>();
            BackgroundScreens = new List<Drawing.Screen>();

            ElapsedWorldUpdateTime = new Stopwatch();

            this.LoadWorld(tmpWrld);

            Drawing.Screen defaultScreen = null;
            if (AppSettings.Default.LegacyRendering)
                defaultScreen = new Drawing.Screens.Default(this);
            else
                defaultScreen = new Drawing.Screens.Default_Deferred(this);

            defaultScreen.Fullscreen = true;
            this.Screens.Add(defaultScreen);

            var cyberScreen = new Drawing.Screens.Cybernet(this)
            {
                Width = 150,
                Height = 100,
                X = 20,
                Y = 90,
                amount = 0.3f
            };
            this.Screens.Add(cyberScreen);

            //new Drawing.UI.Chat(this, this.Canvas);
            var toolbar = new Drawing.UI.ToolBar(this, this.Canvas);
            new Drawing.UI.MenuBar(this, this.Canvas);

            var logoPanel = new Gwen.Control.ImagePanel(Canvas)
            {
                ImageName = @"Content\Image\IngameLogo.png"
            };
            logoPanel.SetPosition(0, 0);
            logoPanel.SetBounds(0, 0, 391, 125);

            Crosshair = new Gwen.Control.ImagePanel(Canvas)
            {
                ImageName = @"Content\Image\Crosshair.png"
            };
            Crosshair.SetBounds(Game.Width / 2 - 25, Game.Height / 2 - 25, 50, 50);

            MouseMode = Game.CursorVisible;

            // We need to wipe the addon assembly before executing lua code
            OutpostOmega.Game.Lua.ModPack.WipeAddonAssembly();

            bool Error = false;
            Console.Message("Calling startup hook");
            Console.Message("### Mod output starting here ###");
            foreach (var Mod in Mods)
            {
                var messages = Mod.Execute(OutpostOmega.Game.Lua.ModPack.ScriptHook.startup, this.World);
                foreach (var msg in messages)
                {
                    Console.Message(string.Format("{0} {1}@{2}: {3}", msg.TimeStamp.ToShortTimeString(), msg.Sender, Mod.Name, msg.Text));
                    Error = msg.Error ? true : Error;
                }
                //Mod.Assembly.HookException += Assembly_HookException;
            }
            Console.Message("### Mod output ending here ###");

            if (Error)
                new Gwen.Control.MessageBox(Canvas, "One or multiple mods are causing errors. Check the console (tilde)", "Minor crash");

            // Try to resolve all objects that probably came with a mod (that should be loaded now)
            Data.DataHandler.ProcessUnloadedObjects();

            // Test
            UILabel = new Gwen.Control.Label(Canvas)
            {
                TextColorOverride = Color.FromArgb(193, 254, 9) // OO-Lime
            };

            //base.Initialize();
        }

        /// <summary>
        /// Used for turning the player
        /// </summary>
        private void Game_MouseMoved(int X, int Y)
        {
            if (!this.MouseMode && this.Game.Focused) // Only pass if mousemode is disabled
                this.World.Player.ApplyMouseDelta(X, Y);
        }

        /// <summary>
        /// Passes triggered actions to the current player
        /// </summary>
        private void Game_InputChanged(OutpostOmega.Game.Tools.Action action, OutpostOmega.Game.Tools.ActionState actionState)
        {
            if (!this.Game.Focused) return;

            // Special case for mouse mode. This needs to be handled in the scene
            if (action == OutpostOmega.Game.Tools.Action.ToggleMouseMode &&
                actionState == OutpostOmega.Game.Tools.ActionState.Activate)
            {
                this.MouseMode = !this.MouseMode;
                return;
            }

            if(!this.MouseMode) // Only pass if mousemode is disabled
                this.World.Player.InjectAction(action, actionState);
        }

        /// <summary>
        /// UI calls from within the game
        /// </summary>
        void World_UICall(GameObject sender, UICommand command, object data)
        {
            switch(command)
            {
                case UICommand.Open:
                    if(sender.GetType() == typeof(OutpostOmega.Game.GameObjects.Items.Devices.Spawner))
                    {
                        var sMenu = new Drawing.UI.SpawnMenu(this, this.Canvas);
                        sMenu.Show();
                    }
                    break;
                case UICommand.Highlight:
                    if(data.GetType() == typeof(Jitter.LinearMath.JBBox))
                    {
                        var box = (Jitter.LinearMath.JBBox)data;
                        Drawer.Highlight.SetBounds(Tools.Convert.Vector.Jitter_To_OpenGL(box.Min), Tools.Convert.Vector.Jitter_To_OpenGL(box.Max));
                    }
                    break;
                case UICommand.HighlightStop:
                    Drawer.Highlight.Visible = false;
                    break;

            }
        }

        /*void Assembly_HookException(object sender, LuaInterface.HookExceptionEventArgs e)
        {
            Console.Message(string.Format("Exception: {0}", e.Exception.ToString()));

            new Gwen.Control.MessageBox(Canvas, "A mod is causing errors. Check the console (tilde)", "Minor crash");
        }*/


        protected override void RefreshSceneView()
        {
            if (Crosshair != null) Crosshair.SetBounds(Game.Width / 2 - 25, Game.Height / 2 - 25, 50, 50);
            base.RefreshSceneView();
        }

        protected override void DrawSceneFree()
        {
#if DEBUG
            for (int i = 0; i < Screens.Count; i++)
                Screens[i].Render();

            for (int i = 0; i < BackgroundScreens.Count; i++)
                BackgroundScreens[i].Render();
#else
            try
            {
                for (int i = 0; i < Screens.Count; i++)
                    Screens[i].Render();

                for (int i = 0; i < BackgroundScreens.Count; i++)
                    BackgroundScreens[i].Render();
            }
            catch (Exception e) { ThrowCrash(e); }
            
            base.DrawSceneFree();
#endif
        }

        protected override void DrawSceneOrtho()
        {
            for (int i = 0; i < Screens.Count; i++)
                Screens[i].DrawScreen();

            base.DrawSceneOrtho();
        }

        protected override void UpdateScene()
        {

            var kstate = OpenTK.Input.Keyboard.GetState();
            var mouseState = new OutpostOmega.Game.Tools.MouseState(OpenTK.Input.Mouse.GetState());

            if (MouseMode || !Game.Focused)
            {
                kstate = new KeyboardState();
                mouseState = new OutpostOmega.Game.Tools.MouseState() { X = (int)Game.MouseData.LastPosition.X, Y = (int)Game.MouseData.LastPosition.Y }; // Do not send mouse input either to prevent accidential clicking ingame
            }


            if (World != null)
            {
                foreach (var Mod in Mods)
                {
                    var messages = Mod.Execute(OutpostOmega.Game.Lua.ModPack.ScriptHook.update, this.World);
                    foreach (var msg in messages)
                    {
                        Console.Message(string.Format("{0} {1}@{2}: {3}", msg.TimeStamp.ToShortTimeString(), msg.Sender, Mod.Name, msg.Text));
                    }

                    if (Mod.Assembly.Output.Count > 0)
                    {
                        var message = Mod.Assembly.Output.Dequeue();
                        Console.Message(string.Format("{0} {1}@{2}: {3}", message.TimeStamp.ToShortTimeString(), message.Sender, Mod.Name, message.Text));
                    }
                    
                    while(LuaInterface.Lua.ExceptionQueue.Count > 0)
                    {
                        var exception = LuaInterface.Lua.ExceptionQueue.Dequeue();
                        Console.Message(string.Format("{0} @{1}: {2} {3}", DateTime.Now.ToShortTimeString(), Mod.Name, exception.ToString(), exception.InnerException != null ? exception.InnerException.ToString() : ""));
                    }
                }

                var ElapsedTime = ElapsedWorldUpdateTime.ElapsedMilliseconds / (double)1000;

                for (int i = 0; i < Screens.Count; i++)
                    Screens[i].Update(ElapsedTime);

                for (int i = 0; i < BackgroundScreens.Count; i++)
                    BackgroundScreens[i].Update(ElapsedTime);

                ElapsedWorldUpdateTime.Stop();
                Tools.Performance.Start("Update World");
                Drawer.Update(mouseState, kstate, ElapsedTime);
                Tools.Performance.Stop("Update World");
                //World.Update(kstate, elapsedWorldUpdateTime.ElapsedMilliseconds / (double)1000);
                if (ElapsedTime > 0)
                    FPSLabel.Text = string.Format("{0} FPS", Math.Round(1 / ElapsedTime).ToString());
                ElapsedWorldUpdateTime.Reset();
                ElapsedWorldUpdateTime.Start();

                var gObj = World.Player.Mob.View.TargetGameObject;
                if (gObj != null)
                {
                    var screenPos = Tools.Other.GetPointOnScreen(Tools.Convert.Vector.Jitter_To_OpenGL(gObj.Position), this.Screens[0], out bool onScreen);

                    if (!onScreen && !UILabel.IsHidden)
                        UILabel.Hide();

                    if (onScreen && UILabel.IsHidden)
                        UILabel.Show();

                    if (UILabel.Text != gObj.ID)
                        UILabel.Text = gObj.ID;

                    if (UILabel.X != screenPos.X || UILabel.Y != screenPos.Y)
                        UILabel.SetPosition(screenPos.X, screenPos.Y);
                }
                else
                {
                    if (!UILabel.IsHidden)
                        UILabel.Hide();
                }
            }
            base.UpdateScene();
        }


        /*public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            Tools.Performance.Start("User Interaction");
            if (World != null)
            {
                if (e.Key == Key.Tilde && !e.IsRepeat)
                    Console.Toggle();

                if (e.Key == Key.Tab && !e.IsRepeat)
                {
                    if (Game.CursorVisible)
                    {
                        MouseMode = false;
                        Game.LockCursor();

                        //World.Player.OldMouseState = new OutpostOmega.Game.Tools.MouseState() { X = (int)Game.MouseData.LastPosition.X, Y = (int)Game.MouseData.LastPosition.Y };
                    }
                    else
                    {
                        MouseMode = true;
                        Game.UnlockCursor();
                    }
                }

                if (!MouseMode)
                    World.KeyPress(e.Key, e.IsRepeat);
            }
            base.OnKeyDown(e);
            Tools.Performance.Stop("User Interaction");
        }*/

        public override void Dispose()
        {
            this.Disposing = true;

            //Dispose World drawer
            if (Drawer != null)
                Drawer.Dispose();

            //Dispose World
            if (World != null)
                World.Dispose();

            base.Dispose();
        }

        /// <summary>
        /// Loads a World into the scene. World must be initialized and working
        /// </summary>
        /// <param name="World">Running World</param>
        public void LoadWorld(World World)
        {
            //Remove old Drawer
            if (this.Drawer != null)
            {
                this.Drawer.Dispose();
                this.Drawer = null;
            }

            //Remove old World
            if (this.World != null)
            {
                this.World.Dispose();
                this.World = null;
            }


            this.World = World;
            this.Drawer = new Drawing.Game.WorldDrawer(this.World, this);
        }
    }
}
