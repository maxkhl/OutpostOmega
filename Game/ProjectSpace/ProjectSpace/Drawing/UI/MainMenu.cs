using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using OutpostOmega.Game;
using System.IO;

namespace OutpostOmega.Drawing.UI
{
    class MainMenu : Base
    {
        Scene Scene;
        List<AnimatedLabel> animLabels = new List<AnimatedLabel>();
        AnimatedTexture2D animation;
        AnimatedTexture2D animationChund;
        List<Button> MenuButtons;

        List<OutpostOmega.Game.Tools.Animation> MenuButtonAnimations;

        public MainMenu(Scene Scene, Base parent)
            : base(parent)
        {
            this.Scene = Scene;

            this.SetSize(50, Scene.Game.Height);
            this.SetPosition(0, 0);
            //this.IsClosable = false;
            this.Dock = Pos.Fill;

            int bWidth = 200;
            int bHeight = 25;

            int vPosition = 200; //game.Height / 4
            int vSpace = 40;

            this.SetSkin(Scene.Skin, true);


            var contRight = new Gwen.Control.Base(this)
            {
                Dock = Pos.Right,
                Width = 100,
            };

            var contLogo = new Base(this)
            {
                Dock = Pos.Top,
                Height = 200
            };


            var contMain = new Base(this)
            {
                Dock = Pos.Bottom,
                Height = vPosition + vSpace * 7
            };



            ImagePanel logo = new ImagePanel(contLogo);
            logo.SetPosition(30, 10);
            animation = new AnimatedTexture2D(new FileInfo(@"Content\Image\MenuAnimation.gif"));
            animation.Play = true;
            //logo.ImageName = @"Content\Image\LogoSmall.png";
            logo.ImageHandle = animation.Handle;
            logo.Clicked += delegate(Base sender, ClickedEventArgs arguments)
            {
                Spawn = !Spawn;

                if (animationChund == null)
                {
                    ImagePanel imgpanel = new ImagePanel(this);
                    imgpanel.SetBounds(120 - 80, 200 - 80, 80, 80);
                    animationChund = new AnimatedTexture2D(new FileInfo(@"Content\Image\SadSmiley.gif"));
                    animationChund.Play = true;
                    animationChund.Repeat = true;
                    animationChund.TargetFPS = 1;

                    imgpanel.ImageHandle = animationChund.Handle;
                }
            };
            //logo.Dock = Pos.Right;
            logo.Width = 391;
            logo.Height = 125;
            //logo.Hide();

            MenuButtons = new List<Button>();

            Button continueWorld = new Button(contMain);
            MenuButtons.Add(continueWorld);
            continueWorld.Width = bWidth;
            continueWorld.Height = bHeight;
            continueWorld.Text = "Continue";
            continueWorld.SetPosition(-500, vPosition);
            continueWorld.IsDisabled = !File.Exists("Save/Autosave.sav");
            continueWorld.Clicked += continueWorld_Clicked;
            continueWorld.TextColor = System.Drawing.Color.White;
            vPosition += vSpace;

            Button newWorld = new Button(contMain);
            MenuButtons.Add(newWorld);
            newWorld.Width = bWidth;
            newWorld.Height = bHeight;
            newWorld.Text = "New Testworld";
            newWorld.SetPosition(-500, vPosition);
            newWorld.Clicked += newWorld_Clicked;
            newWorld.TextColor = System.Drawing.Color.White;
            vPosition += vSpace;

            Button load = new Button(contMain);
            MenuButtons.Add(load);
            load.Width = bWidth;
            load.Height = bHeight;
            load.Text = "Load Game";
            load.SetPosition(-500, vPosition);
            load.Clicked += load_Clicked;
            load.TextColor = System.Drawing.Color.White;
            vPosition += vSpace;

            Button multiplayer = new Button(contMain);
            MenuButtons.Add(multiplayer);
            multiplayer.Width = bWidth;
            multiplayer.Height = bHeight;
            multiplayer.Text = "Multiplayer";
            multiplayer.SetPosition(-500, vPosition);
            multiplayer.Clicked += multiplayer_Clicked;
            multiplayer.TextColor = System.Drawing.Color.White;
            vPosition += vSpace;

            Button options = new Button(contMain);
            MenuButtons.Add(options);
            options.Width = bWidth;
            options.Height = bHeight;
            options.Text = "Settings";
            options.SetPosition(-500, vPosition);
            options.Clicked += options_Clicked;
            options.TextColor = System.Drawing.Color.White;
            vPosition += vSpace;

            Button exit = new Button(contMain);
            MenuButtons.Add(exit);
            exit.Width = bWidth;
            exit.Height = bHeight;
            exit.Text = "Exit Game";
            exit.SetPosition(-500, vPosition);
            exit.Pressed += exit_Pressed;
            exit.TextColor = System.Drawing.Color.White;
            vPosition += vSpace;






            Button test = new Button(contRight);
            test.Width = 100;
            test.Height = bHeight;
            test.Text = "Test";
            test.SetPosition(0, 50);
            test.Position(Pos.Right);
            test.Pressed += delegate(Base sender, EventArgs arguments)
            {
                var tstwindow = new Test(Scene, this.Parent);
                tstwindow.Show();
            };
            test.TextColor = System.Drawing.Color.White;

            Button test2 = new Button(contRight);
            test2.Width = 100;
            test2.Height = bHeight;
            test2.Text = "Video";
            test2.SetPosition(0, 90);
            test2.Position(Pos.Right);
            test2.Pressed += delegate(Base sender, EventArgs arguments)
            {
                var vPlaer = new VideoPlayer(Scene, this.Parent);
                vPlaer.Show();
            };
            test2.TextColor = System.Drawing.Color.White;

            Button test3 = new Button(contRight);
            test3.Width = 100;
            test3.Height = bHeight;
            test3.Text = "Game of Life";
            test3.SetPosition(0, 120);
            test3.Position(Pos.Right);
            test3.Pressed += delegate(Base sender, EventArgs arguments)
            {
                var cw = new Conway(Scene, this);
                cw.Show();
            };
            test3.TextColor = System.Drawing.Color.White;


            Scene.Game.UnlockCursor();


            MenuButtonAnimations = new List<OutpostOmega.Game.Tools.Animation>();
            for (int i = 0; i < MenuButtons.Count; i++ )
            {
                MenuButtons[i].Animate("X", 25 + (i * 5), 2000 + (i * 100), OutpostOmega.Game.Tools.Easing.EaseFunction.BackEaseOut);
            }

            //newWorld_Clicked(null, null);
            //QuickLoad();
        }

        class AnimatedLabel : IDisposable
        {
            Label label;
            OpenTK.Vector2 Position;
            OpenTK.Vector2 Target;

            public AnimatedLabel(Base Parent, Scene scene)
            {
                label = new Label(Parent);
                //subtitle.Dock = Pos.Bottom;
                label.Margin = new Gwen.Margin(25, 25, 25, 25);
                var rand = new Random();
                Position = new OpenTK.Vector2(
                    rand.Next(0, scene.Game.Width - 50), 
                    -20);

                Target = new OpenTK.Vector2(
                    rand.Next(0, scene.Game.Width - 50),
                    scene.Game.Height + 20);

                Speed = rand.Next(1, 10);

                var titles = new List<string>()
                {
                    "EichelsaftLP",
                    "50 Winkel of Jay",
                    "Alle inkontinent hier",
                    "Kotzt mich schon wieder an",
                    "Sgt. Lutschbandi",
                    "Agentmess",
                    "Spähtkiller",
                    "Licky Guy",
                    "Toxyn die alde Dreckschlambe",
                    "Niggerlas Gelb",
                    "Maikehrozopft",
                    "Jaylums Schahhhtz",
                    "Richtiger Pfisch",
                    "Labba ned",
                    "Pfehlix",
                    "Paul is voll der Ranzbruder",
                    "Asoziales PACK",
                    "Kotzt mich alles an hier",
                    "Voll keen bock uf meine Mudder",
                    "Schuldest mir noch 10 Eurooo hhhhhh",
                    "Des bleibt alles so wie's hier is",
                    "Gesundheit",
                    "Meh",
                    "Elmo is im Gebäudeee",
                    "Pah",
                    "Ischeer",
                    "Selbst im Angesicht der Niederlage an der Ostfront wird sich das TS-Regime dem Bolschewismus nicht beugen!",
                    "Junge Junge",
                    "Bissle Stange verbiegen und McFit Mitarbeiter bespucken",
                    "HattErNichGesaaaaaagt",
                    "Eeeeeeeeeeeeee",
                }.ToArray();

                label.Text = "-" + titles[rand.Next(0, titles.Length)] + "-";
                label.Font = new Font(scene.renderer, "Verdana", 12);
                label.TextColor = System.Drawing.Color.FromArgb(rand.Next(100, 255), rand.Next(100, 255), rand.Next(100, 255));
                label.X = (int)Position.X;
                label.Y = (int)Position.Y;
            }
            float Speed = 2;
            public bool Update()
            {
                if (Disposing)
                    return false;

                var norm = (Target - Position);
                norm.Normalize();
                Position += norm * Speed;
                label.X = (int)Position.X;
                label.Y = (int)Position.Y;
                return false;

                if((Target-Position).Length < 4)
                {
                    this.Dispose();
                    return true;
                }
            }

            public bool Disposing { get; set; }
            public void Dispose()
            {
                label.Dispose();
            }
        }

        float SpawnTime = 10;
        bool Spawn = false;
        public override void Think()
        {


            animation.Update();
            if(animationChund != null)
                animationChund.Update();

            for (int i = 0; i < animLabels.Count; i++)
                if (animLabels[i].Update())
                    animLabels.Remove(animLabels[i]);

            if (Spawn)
            {
                SpawnTime--;
                if (SpawnTime <= 0)
                {
                    SpawnTime = new Random().Next(10, 50);
                    animLabels.Add(new AnimatedLabel(this.Parent, Scene));
                }
            }

            if (newWorld != null)
            {
                int handler = Scene.Game.SceneManager.AddScene(new Scenes.Game(Scene.Game, newWorld));
                Scene.Game.SceneManager.ScheduleSceneChange(handler);
                //Scene.Game.SceneManager.MakeSceneActive(handler);
                this.Hide();
            }
            base.Think();
        }

        void continueWorld_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Tools.Other.NotImplemented(this);
            //return;

            //lScreen = new LoadingScreen(this.Scene, this.Parent, "Loading World");

            GameStarter.Load(this.Scene.Game.SceneManager, this.Scene.Canvas, new FileInfo("Save/Autosave.sav"));

        }

        void multiplayer_Clicked(Base sender, ClickedEventArgs arguments)
        {
            /*Tools.Other.NotImplemented(this);
            return;*/

            var multiplayer = new Multiplayer(Scene, this);
            multiplayer.Show();
        }

        void load_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Tools.Other.NotImplemented(this);
            //return;

            var load = new Load(Scene, this);
            load.Show();
        }

        LoadingScreen lScreen;
        void newWorld_Clicked(Base sender, ClickedEventArgs arguments)
        {
            lScreen = new LoadingScreen(this.Scene, this.Parent, "Loading World");
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(CreateTestWorld));
        }

        World newWorld = null;
        public void CreateTestWorld(object state)
        {
            World world = null;
            System.Threading.Thread.Sleep(2000);

            FileInfo autoload = new FileInfo(@"Content\DefaultWorld.sav");
            /*if(autoload.Exists)
                world = OutpostOmega.Data.DataHandler.LoadWorldFromFile(autoload);
            else
            {*/
                world = World.CreateTest();
                world.MakePlayer();
            //}

            newWorld = world;

            //Scene.LoadWorld(World);

            //this.Close();

        }

        void exit_Pressed(Base sender, EventArgs arguments)
        {
            Scene.Game.Exit();
        }

        void options_Clicked(Base sender, ClickedEventArgs arguments)
        {
            Settings settings = new Settings(Scene, this.Parent);
            settings.Show();
        }
    }
}
