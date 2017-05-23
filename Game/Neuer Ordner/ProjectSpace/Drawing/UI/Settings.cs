using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using OpenTK;

namespace OutpostOmega.Drawing.UI
{
    class Settings : Menu
    {
        bool IsGameScene = false;

        public Settings(Scene Scene, Base parent)
            : base(Scene, parent, "Game Settings")
        {
            this.SetSize(220, 300);

            IsGameScene = typeof(Scenes.Game) == Scene.GetType() && ((Scenes.Game)Scene).World != null;
                

            //Center meh
            this.SetPosition(Scene.Game.Width / 2 - this.Width / 2, Scene.Game.Height / 2 - this.Height / 2);

            int posY = 0;

            CheckBox flymode = new CheckBox(this);
            posY += 20;
            flymode.SetPosition(5, posY);
            if (IsGameScene)
                flymode.IsChecked = ((Scenes.Game)Scene).World.Player.Mob.FlyMode;
            else
                flymode.IsDisabled = true;
            flymode.CheckChanged += flymode_CheckChanged;

            Label flymode_l = new Label(this);
            flymode_l.Text = "Flymode";
            flymode_l.SetPosition(25, posY);


            CheckBox physicdebug = new CheckBox(this);
            posY += 20;
            physicdebug.SetPosition(5, posY);
            if (IsGameScene)
                physicdebug.IsChecked = ((Scenes.Game)Scene).World.Debug;
            else
                physicdebug.IsDisabled = true;
            physicdebug.CheckChanged += physicdebug_CheckChanged;

            Label physicdebug_l = new Label(this);
            physicdebug_l.Text = "Debug Physics";
            physicdebug_l.SetPosition(25, posY);


            CheckBox voxeldebug = new CheckBox(this);
            posY += 20;
            voxeldebug.SetPosition(5, posY);
            if (IsGameScene)
                voxeldebug.IsChecked = ((Scenes.Game)Scene).World.Debug;
            else
                voxeldebug.IsDisabled = true;
            voxeldebug.CheckChanged += voxeldebug_CheckChanged;

            Label voxeldebug_l = new Label(this);
            voxeldebug_l.Text = "Debug Voxel";
            voxeldebug_l.SetPosition(25, posY);


            CheckBox fullscreen = new CheckBox(this);
            posY += 20;
            fullscreen.SetPosition(5, posY);
            fullscreen.IsChecked = Scene.Game.WindowState == WindowState.Fullscreen;
            fullscreen.CheckChanged += fullscreen_CheckChanged;

            Label fullscreen_l = new Label(this);
            fullscreen_l.Text = "Fullscreen";
            fullscreen_l.SetPosition(25, posY);

            CheckBox vsync = new CheckBox(this);
            posY += 20;
            vsync.SetPosition(5, posY);
            vsync.IsChecked = Scene.Game.VSync == VSyncMode.On;
            vsync.CheckChanged += vsync_CheckChanged;

            Label vsync_l = new Label(this);
            vsync_l.Text = "VSync";
            vsync_l.SetPosition(25, posY);


            CheckBox lRenderer = new CheckBox(this);
            posY += 20;
            vsync.SetPosition(5, posY);
            vsync.IsChecked = AppSettings.Default.LegacyRendering;
            vsync.CheckChanged += lRenderer_CheckChanged;   

            Label lRenderer_l = new Label(this);
            vsync_l.Text = "Use Legacy Renderer";
            vsync_l.SetPosition(25, posY);


            ComboBox resolution = new ComboBox(this);
            posY += 20;
            resolution.SetPosition(5, posY);
            resolution.Width = 200;

            foreach (DisplayResolution res in DisplayDevice.Default.AvailableResolutions)
                resolution.AddItem(res.Width + "x" + res.Height + "x" + res.BitsPerPixel + " " + res.RefreshRate, "", res);

            resolution.Text =
                AppSettings.Default.Width + "x" +
                AppSettings.Default.Height + "x" +
                AppSettings.Default.BitsPerPixel + " " +
                AppSettings.Default.RefreshRate;

            resolution.ItemSelected += resolution_ItemSelected;

            Label speed_l = new Label(this);
            posY += 20;
            speed_l.Text = "Simulation Speed (100%)";
            speed_l.SetPosition(25, posY);

            HorizontalSlider speed = new HorizontalSlider(this);
            posY += 20;
            speed.SetPosition(5, posY);
            speed.Width = 200;
            speed.Height = 15;
            speed.Max = 200;
            speed.Min = 0;
            speed.Value = 100;
            speed.ValueChanged += speed_ValueChanged;
            speed.UserData = speed_l;

            if (IsGameScene)
                speed.IsDisabled = true;
            
            CheckBox autosave = new CheckBox(this);
            posY += 20;
            autosave.SetPosition(5, posY);
            autosave.IsChecked = AppSettings.Default.Autosave;
            autosave.CheckChanged += autosave_CheckChanged;

            Label autosave_l = new Label(this);
            autosave_l.Text = "Autosave/Autoload";
            autosave_l.SetPosition(25, posY);
            posY += 20;

            Button screenmanager = new Button(this);
            screenmanager.SetPosition(5, posY);
            screenmanager.Text = "Screenmanager";
            screenmanager.Clicked += screenmanager_Clicked;
            posY += 20;

            Button perfmon = new Button(this);
            perfmon.SetPosition(5, posY);
            perfmon.Text = "Performance Monitor";
            perfmon.Clicked += perfmon_Clicked;
            posY += 20;

            CheckBox skipIntro = new CheckBox(this);
            skipIntro.SetPosition(5, posY);
            skipIntro.IsChecked = AppSettings.Default.SkipIntro;
            skipIntro.CheckChanged += skipIntro_CheckChanged;

            Label skipIntro_l = new Label(this);
            skipIntro_l.Text = "Skip Intro";
            skipIntro_l.SetPosition(25, posY);
            posY += 20;

        }

        void skipIntro_CheckChanged(Base sender, EventArgs arguments)
        {
            var ischecked = ((CheckBox)sender).IsChecked;
            AppSettings.Default.SkipIntro = ischecked;
            AppSettings.Default.Save();
        }

        void perfmon_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (IsGameScene)
            {
                var perfmon = new Performance(Scene, this.Parent);
                perfmon.Show();
            }
        }

        void screenmanager_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (IsGameScene)
            {
                var smanager = new Screenmanager((Scenes.Game)Scene, this.Parent);
                smanager.Show();
            }
        }

        void vsync_CheckChanged(Base sender, EventArgs arguments)
        {
            var ischecked = ((CheckBox)sender).IsChecked;
            if (ischecked)
                Scene.Game.VSync = VSyncMode.On;
            else
                Scene.Game.VSync = VSyncMode.Off;
            AppSettings.Default.VSync = ischecked;
            AppSettings.Default.Save();
        }

        void lRenderer_CheckChanged(Base sender, EventArgs arguments)
        {
            var ischecked = ((CheckBox)sender).IsChecked;
            AppSettings.Default.LegacyRendering = ischecked;
            AppSettings.Default.Save();
            new MessageBox(this.Parent, "This change requires a restart");
        }

        void autosave_CheckChanged(Base sender, EventArgs arguments)
        {
            AppSettings.Default.Autosave = ((CheckBox)sender).IsChecked;
            AppSettings.Default.Save();
        }

        void speed_ValueChanged(Base sender, EventArgs arguments)
        {
            var slider = (HorizontalSlider)sender;
            var label = (Label)slider.UserData;
            if (IsGameScene)
            {
                ((Scenes.Game)Scene).Drawer.SimulationSpeedFactor = slider.Value / 100;
                label.Text = "Simulation Speed (" + Math.Round(slider.Value).ToString() + "%)";
            }
        }

        void voxeldebug_CheckChanged(Base sender, EventArgs arguments)
        {
            var checkbox = (CheckBox)sender;
            //game.DebugVoxel = checkbox.IsChecked; TODO
        }

        void physicdebug_CheckChanged(Base sender, EventArgs arguments)
        {
            var checkbox = (CheckBox)sender;
            if (IsGameScene)
                ((Scenes.Game)Scene).World.Debug = checkbox.IsChecked;
        }

        void fullscreen_CheckChanged(Base sender, EventArgs arguments)
        {
            var checkbox = (CheckBox)sender;
            if (checkbox.IsChecked)
                Scene.Game.WindowState = WindowState.Fullscreen;
            else
                Scene.Game.WindowState = WindowState.Normal;

            AppSettings.Default.FullScreen = checkbox.IsChecked;
            AppSettings.Default.Save();
        }

        void resolution_ItemSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var combobox = (ComboBox)sender;
            var resolution = (DisplayResolution)combobox.SelectedItem.UserData;

            Scene.Game.Size = new System.Drawing.Size(resolution.Width, resolution.Height);
            Scene.Game.TargetRenderFrequency = resolution.RefreshRate;
            //DisplayDevice.Default.ChangeResolution(resolution.Width, resolution.Height, resolution.BitsPerPixel, resolution.RefreshRate);

            AppSettings.Default.BitsPerPixel = resolution.BitsPerPixel;
            AppSettings.Default.Width = resolution.Width;
            AppSettings.Default.Height = resolution.Height;
            AppSettings.Default.RefreshRate = resolution.RefreshRate;
            AppSettings.Default.Save();
        }

        void flymode_CheckChanged(Base sender, EventArgs arguments)
        {
            var ischecked = ((CheckBox)sender).IsChecked;

            if (IsGameScene)
                ((Scenes.Game)Scene).World.Player.Mob.FlyMode = ischecked;
        }
    }
}
