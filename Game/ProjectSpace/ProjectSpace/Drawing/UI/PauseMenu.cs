using System;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using System.IO;

namespace OutpostOmega.Drawing.UI
{
    class PauseMenu : Menu
    {
        public PauseMenu(Scenes.Game GameScene, Base parent, int Width, int Height)
            : base(GameScene, parent, "Game paused")
        {
            this.SetSize(Width, Height);
            this.SetPosition(100, 20);
            //this.IsClosable = false;


            Button reset = new Button(this);
            reset.Text = "Reset Position";
            reset.SetPosition(20, 20);
            reset.Clicked += reset_Clicked;

            Button options = new Button(this);
            options.Text = "Settings";
            options.SetPosition(20, 40);
            options.Clicked += options_Clicked;

            Button save = new Button(this);
            save.Text = "Save";
            save.SetPosition(20, 60);
            save.Clicked += save_Clicked;

            Button exit = new Button(this);
            exit.Text = "Exit Game";
            exit.SetPosition(20, 80);
            exit.Pressed += exit_Pressed;
        }

        void save_Clicked(Base sender, ClickedEventArgs arguments)
        {
            SaveWorld();
        }

        public void SaveWorld()
        {
            if (((Scenes.Game)Scene).World != null)
            {
                FileInfo save = new FileInfo("Autosave.sav");
                OutpostOmega.Data.DataHandler.SaveToFile(((Scenes.Game)Scene).World, save, false);
            }
        }

        void reset_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ((Scenes.Game)Scene).World.Player.Mob.SetPosition(new Jitter.LinearMath.JVector(0, 20, 0));
            ((Scenes.Game)Scene).World.Player.Mob.charController.Position = ((Scenes.Game)Scene).World.Player.Mob.Position;
            //game.dWorld.World.Player.Mob._RigidBody.LinearVelocity = Jitter.LinearMath.JVector.Zero;
        }

        void exit_Pressed(Base sender, EventArgs arguments)
        {
            if (AppSettings.Default.Autosave)
                SaveWorld();

            Scene.Dispose();
            this.Parent.Hide();
        }

        void options_Clicked(Base sender, ClickedEventArgs arguments)
        {
            //Inventory settings = new Inventory(Scene, this.Parent);
            //settings.Show();
        }
    }
}
