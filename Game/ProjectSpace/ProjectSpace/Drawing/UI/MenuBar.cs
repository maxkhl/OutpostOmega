using System;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using System.IO;
using System.Threading;

namespace OutpostOmega.Drawing.UI
{
    class MenuBar : DockBase
    {
        Scenes.Game GameScene;
        Label buttonLabel;
        public MenuBar(Scenes.Game GameScene, Base parent)
            : base(parent)
        {
            this.GameScene = GameScene;
            this.SetSize(500, 0);
            this.Dock = Pos.Right;
            //this.IsClosable = false;

            int posy = 2;

            ImagePanel reset = new ImagePanel(this);
            reset.Name = "Reset";
            reset.ImageName = @"Content\Image\Buttons\Back.png";
            reset.Width = 22;
            reset.Height = 22;
            reset.SetPosition(this.Width - reset.Width, posy);
            reset.Clicked += reset_Clicked;
            reset.HoverEnter += HoverEnter;
            reset.HoverLeave += HoverLeave;
            posy += 34;

            /*ImagePanel inventory = new ImagePanel(this);
            inventory.Name = "Inventory";
            inventory.ImageName = @"Content\Image\Buttons\Info.png";
            inventory.Width = 22;
            inventory.Height = 22;
            inventory.SetPosition(this.Width - inventory.Width, posy);
            inventory.Clicked += inventory_Clicked;
            inventory.HoverEnter += HoverEnter;
            inventory.HoverLeave += HoverLeave;
            posy += 34;*/

            ImagePanel settings = new ImagePanel(this);
            settings.Name = "Settings";
            settings.ImageName = @"Content\Image\Buttons\Gear.png";
            settings.Width = 22;
            settings.Height = 22;
            settings.SetPosition(this.Width - settings.Width, posy);
            settings.Clicked += options_Clicked;
            settings.HoverEnter += HoverEnter;
            settings.HoverLeave += HoverLeave;
            posy += 34;

            ImagePanel fullScreen = new ImagePanel(this);
            fullScreen.Name = "Fullscreen";
            fullScreen.ImageName = @"Content\Image\Buttons\Fullscreen.png";
            fullScreen.Width = 22;
            fullScreen.Height = 22;
            fullScreen.SetPosition(this.Width - fullScreen.Width, posy);
            fullScreen.Clicked += fullScreen_Clicked;
            fullScreen.HoverEnter += HoverEnter;
            fullScreen.HoverLeave += HoverLeave;
            posy += 34;

            ImagePanel performance = new ImagePanel(this);
            performance.Name = "Performance";
            performance.ImageName = @"Content\Image\Buttons\Stats.png";
            performance.Width = 22;
            performance.Height = 22;
            performance.SetPosition(this.Width - performance.Width, posy);
            performance.Clicked += performance_Clicked;
            performance.HoverEnter += HoverEnter;
            performance.HoverLeave += HoverLeave;
            posy += 34;

            ImagePanel console = new ImagePanel(this);
            console.Name = "Console";
            console.ImageName = @"Content\Image\Buttons\Hash.png";
            console.Width = 22;
            console.Height = 22;
            console.SetPosition(this.Width - console.Width, posy);
            console.Clicked += console_Clicked;
            console.HoverEnter += HoverEnter;
            console.HoverLeave += HoverLeave;
            posy += 34;

            /*ImagePanel SpawnMenu = new ImagePanel(this);
            SpawnMenu.Name = "Build";
            SpawnMenu.ImageName = @"Content\Image\Buttons\Plus.png";
            SpawnMenu.Width = 22;
            SpawnMenu.Height = 22;
            SpawnMenu.SetPosition(this.Width - SpawnMenu.Width, posy);
            SpawnMenu.Clicked += SpawnMenu_Clicked;
            SpawnMenu.HoverEnter += HoverEnter;
            SpawnMenu.HoverLeave += HoverLeave;
            posy += 34;*/

            ImagePanel videoPlayer = new ImagePanel(this);
            videoPlayer.Name = "Video Player";
            videoPlayer.ImageName = @"Content\Image\Buttons\Media.png";
            videoPlayer.Width = 22;
            videoPlayer.Height = 22;
            videoPlayer.SetPosition(this.Width - videoPlayer.Width, posy);
            videoPlayer.Clicked += videoPlayer_Clicked;
            videoPlayer.HoverEnter += HoverEnter;
            videoPlayer.HoverLeave += HoverLeave;
            posy += 34;

            ImagePanel debug = new ImagePanel(this);
            debug.Name = "Debug";
            debug.ImageName = @"Content\Image\Buttons\Bug.png";
            debug.Width = 22;
            debug.Height = 22;
            debug.SetPosition(this.Width - videoPlayer.Width, posy);
            debug.Clicked += debug_Clicked;
            debug.HoverEnter += HoverEnter;
            debug.HoverLeave += HoverLeave;
            posy += 34;

            ImagePanel save = new ImagePanel(this);
            save.Name = "Save";
            save.ImageName = @"Content\Image\Buttons\Save.png";
            save.Width = 22;
            save.Height = 22;
            save.SetPosition(this.Width - save.Width, posy);
            save.Clicked += save_Clicked;
            save.HoverEnter += HoverEnter;
            save.HoverLeave += HoverLeave;
            posy += 34;

            ImagePanel exit = new ImagePanel(this);
            exit.Name = "Exit";
            exit.ImageName = @"Content\Image\Buttons\Standby.png";
            exit.Width = 22;
            exit.Height = 22;
            exit.SetPosition(this.Width - exit.Width, posy);
            exit.Clicked += exit_Pressed;
            exit.HoverEnter += HoverEnter;
            exit.HoverLeave += HoverLeave;
            posy += 34;

            buttonLabel = new Label(this);
            //buttonLabel.Width = 200;

            Label tipp = new Label(this.Parent);
            tipp.Text = "Press TAB to switch between camera and cursor mode";
            tipp.Y = 0;
            tipp.X = (GameScene.Game.Width / 2) - (tipp.Width / 2);

            //exit_Pressed(null, null);
        }

        void debug_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (this.Parent.CheckWindowOpen(typeof(Debugger)))
                return;

            var debugger = new Debugger(GameScene, this.Parent);
            debugger.Show();
        }

        void console_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (this.Parent.CheckWindowOpen(typeof(LuaConsole)))
                return;

            var console = new LuaConsole(GameScene, this.Parent);
            console.Show();
        }

        void performance_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (this.Parent.CheckWindowOpen(typeof(Performance)))
                return;

            var perfmon = new Performance(GameScene, this.Parent);
            perfmon.Show();
        }

        void fullScreen_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (GameScene.Game.WindowState == OpenTK.WindowState.Normal)
                GameScene.Game.WindowState = OpenTK.WindowState.Fullscreen;
            else
                GameScene.Game.WindowState = OpenTK.WindowState.Normal;

            AppSettings.Default.FullScreen = GameScene.Game.WindowState == OpenTK.WindowState.Fullscreen;
            AppSettings.Default.Save();
        }

        void videoPlayer_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (this.Parent.CheckWindowOpen(typeof(VideoPlayer)))
                return;

            VideoPlayer vplayer = new VideoPlayer(GameScene, this.Parent);
            vplayer.Show();
        }

        void SpawnMenu_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (this.Parent.CheckWindowOpen(typeof(SpawnMenu)))
                return;

            SpawnMenu smenu = new SpawnMenu(GameScene, this.Parent);
            smenu.Show();
        }

        /*void inventory_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (this.Parent.CheckWindowOpen(typeof(Inventory)))
                return;

            var inventorydialog = new Inventory(GameScene, this.Parent);
            inventorydialog.Show();
        }*/

        void options_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (this.Parent.CheckWindowOpen(typeof(Settings)))
                return;

            Settings settings = new Settings(GameScene, this.Parent);
            settings.Show();
        }

        int hovamount = 10;
        void HoverLeave(Base sender, EventArgs arguments)
        {
            var imgPanel = (ImagePanel)sender;
            imgPanel.Width = imgPanel.Width - hovamount;
            imgPanel.X = imgPanel.X + hovamount;

            imgPanel.Height = imgPanel.Height - hovamount;
            imgPanel.Y = imgPanel.Y + hovamount / 2;

            buttonLabel.Hide();
            //imgPanel.Height = imgPanel.Height - 5;
            //imgPanel.PaddingOutlineColor = System.Drawing.Color.Transparent;
            //imgPanel.UpdateColors();
        }

        void HoverEnter(Base sender, EventArgs arguments)
        {
            var imgPanel = (ImagePanel)sender;
            imgPanel.Width = imgPanel.Width + hovamount;
            imgPanel.X = imgPanel.X - hovamount;

            imgPanel.Height = imgPanel.Height + hovamount;
            imgPanel.Y = imgPanel.Y - hovamount / 2;

            buttonLabel.Show();
            buttonLabel.Text = imgPanel.Name;
            buttonLabel.SetPosition(imgPanel.X - buttonLabel.Width - 10, imgPanel.Y + (imgPanel.Height / 2) - (buttonLabel.Height / 2));
            //imgPanel.PaddingOutlineColor = System.Drawing.Color.Red;
            //imgPanel.UpdateColors();
        }

        void save_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameStarter.Save(GameScene.Game.SceneManager, GameScene.Canvas, GameScene.World, new FileInfo("Save/Autosave.sav"));
            
            //EnqueueSaving();
            //new MessageBox(this.Parent, "Game saved!", "Save").Show();
        }

        bool WorldSaved = false;
        public void EnqueueSaving()
        {
            WorldSaved = false;
            //this.Hide();
            for (int i = 0; i < this.Parent.Children.Count; i++)
                this.Parent.Children[i].Hide();

            if (loadScreen == null)
                loadScreen = new LoadingScreen(this.GameScene, this.Parent);
            loadScreen.Message = "Saving current world";
            loadScreen.Show();
            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveWorld));
        }
        LoadingScreen loadScreen;
        public void SaveWorld(object state)
        {
            if (GameScene.World != null)
            {
                GameScene.Stop = true;
                FileInfo save = new FileInfo("Save/Autosave.sav");
                OutpostOmega.Data.DataHandler.SaveToFile(GameScene.World, save, false);
                //loadScreen.Hide();
                WorldSaved = true;
                GameScene.Stop = false;

                this.SetSize(0, 0);
                this.Show();
            }
        }

        void reset_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameScene.World.Player.Mob.SetPosition(new Jitter.LinearMath.JVector(0, 20, 0));
            GameScene.World.Player.Mob.charController.Position = GameScene.World.Player.Mob.Position;
            //game.dWorld.World.Player.Mob._RigidBody.LinearVelocity = Jitter.LinearMath.JVector.Zero;
        }

        bool Exit = false;
        void exit_Pressed(Base sender, EventArgs arguments)
        {
            if (GameScene.GetType() == typeof(Scenes.NetworkGame))
            {
                WorldSaved = true;
            }
            else
            {
                if (AppSettings.Default.Autosave)
                    GameStarter.Save(GameScene.Game.SceneManager, GameScene.Canvas, GameScene.World, new FileInfo("Save/Autosave.sav"));
                    //EnqueueSaving();
            }

            Exit = true;
            //GameScene.Dispose();
            //this.Parent.Hide();
        }

        public override void Think()
        {
            if (Exit)
            {
                GameScene.Game.SceneManager.ScheduleSceneChange(GameScene.Game.SceneManager.AddScene(new Scenes.Menu(GameScene.Game)));
            }
            else if (WorldSaved)
                WorldSaved = false;
            base.Think();
        }
    }
}
