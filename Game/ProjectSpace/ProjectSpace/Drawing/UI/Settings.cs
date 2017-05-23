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

        TabControl tabControl;

        public Settings(Scene Scene, Base parent)
            : base(Scene, parent, "Game Settings")
        {
            this.SetSize(500, 400);
            this.Position(Pos.Center);

            tabControl = new Gwen.Control.TabControl(this)
                {
                    Dock = Pos.Fill,
                };
            

            var tabButtonGame = new TabButton(tabControl) { Text = "Game", Page = new Base(tabControl), };
            tabControl.AddPage(tabButtonGame);

            var tabButtonGraphics = new TabButton(tabControl) { Text = "Graphics", Page = new Base(tabControl), };
            tabControl.AddPage(tabButtonGraphics);

            var tabButtonAudio = new TabButton(tabControl) { Text = "Audio", Page = new Base(tabControl), };
            tabControl.AddPage(tabButtonAudio);


            IsGameScene = typeof(Scenes.Game) == Scene.GetType() && ((Scenes.Game)Scene).World != null;

            var tabButtonIngame = new TabButton(tabControl)
            {
                Text = "Ingame Tools",
                Page = new Base(tabControl),
                IsHidden = !IsGameScene,
                IsDisabled = !IsGameScene
            };

            tabControl.AddPage(tabButtonIngame);

            int lineSpacing = 40;
            int posY = 0;

            Base page = null;

            ////////////////// GAME TAB ////////////////////////////
            page = tabButtonGame.Page;

            CheckBox skipIntro = new CheckBox(page);
            skipIntro.SetPosition(5, posY);
            skipIntro.IsChecked = AppSettings.Default.SkipIntro;
            skipIntro.CheckChanged += skipIntro_CheckChanged;

            Label skipIntro_l = new Label(page);
            skipIntro_l.Text = "Skip Intro";
            skipIntro_l.SetPosition(25, posY);

            posY += lineSpacing; ////// NEW LINE

            CheckBox autosave = new CheckBox(page);
            posY += 20;
            autosave.SetPosition(5, posY);
            autosave.IsChecked = AppSettings.Default.Autosave;
            autosave.CheckChanged += autosave_CheckChanged;

            Label autosave_l = new Label(page);
            autosave_l.Text = "Autosave/Autoload";
            autosave_l.SetPosition(25, posY);

            ////////////////// GRAPHICS TAB ////////////////////////////

            page = tabButtonGraphics.Page;
            posY = 0;
            CheckBox fullscreen = new CheckBox(page);
            fullscreen.SetPosition(5, posY);
            fullscreen.IsChecked = Scene.Game.WindowState == WindowState.Fullscreen;
            fullscreen.CheckChanged += fullscreen_CheckChanged;

            Label fullscreen_l = new Label(page);
            fullscreen_l.Text = "Fullscreen";
            fullscreen_l.SetPosition(25, posY);

            posY += lineSpacing; ////// NEW LINE

            CheckBox vsync = new CheckBox(page);
            vsync.SetPosition(5, posY);
            vsync.IsChecked = Scene.Game.VSync == VSyncMode.On;
            vsync.CheckChanged += vsync_CheckChanged;

            Label vsync_l = new Label(page);
            vsync_l.Text = "VSync";
            vsync_l.SetPosition(25, posY);

            posY += lineSpacing; ////// NEW LINE

            CheckBox lRenderer = new CheckBox(page);
            lRenderer.SetPosition(5, posY);
            lRenderer.IsChecked = AppSettings.Default.LegacyRendering;
            lRenderer.CheckChanged += lRenderer_CheckChanged;

            Label lRenderer_l = new Label(page);
            lRenderer_l.Text = "Use Legacy Renderer";
            lRenderer_l.SetPosition(25, posY);

            posY += lineSpacing; ////// NEW LINE

            ComboBox resolution = new ComboBox(page);
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

            ////////////////// INGAME TAB ////////////////////////////

            page = tabButtonIngame.Page;
            posY = 0;

            Label speed_l = new Label(page);
            speed_l.Text = "Simulation Speed (100%)";
            speed_l.SetPosition(25, posY);

            HorizontalSlider speed = new HorizontalSlider(page);
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

            posY += lineSpacing; ////// NEW LINE

            CheckBox flymode = new CheckBox(page);
            flymode.SetPosition(5, posY);
            if (IsGameScene)
                flymode.IsChecked = ((Scenes.Game)Scene).World.Player.Mob.FlyMode;
            else
                flymode.IsDisabled = true;
            flymode.CheckChanged += flymode_CheckChanged;

            Label flymode_l = new Label(page);
            flymode_l.Text = "Flymode";
            flymode_l.SetPosition(25, posY);

            posY += lineSpacing; ////// NEW LINE

            CheckBox physicdebug = new CheckBox(page);
            physicdebug.SetPosition(5, posY);
            if (IsGameScene)
                physicdebug.IsChecked = ((Scenes.Game)Scene).World.Debug;
            else
                physicdebug.IsDisabled = true;
            physicdebug.CheckChanged += physicdebug_CheckChanged;

            Label physicdebug_l = new Label(page);
            physicdebug_l.Text = "Debug Physics";
            physicdebug_l.SetPosition(25, posY);

            posY += lineSpacing; ////// NEW LINE

            CheckBox voxeldebug = new CheckBox(page);
            voxeldebug.SetPosition(5, posY);
            if (IsGameScene)
                voxeldebug.IsChecked = ((Scenes.Game)Scene).World.Debug;
            else
                voxeldebug.IsDisabled = true;
            voxeldebug.CheckChanged += voxeldebug_CheckChanged;

            Label voxeldebug_l = new Label(page);
            voxeldebug_l.Text = "Debug Voxel";
            voxeldebug_l.SetPosition(25, posY);

            posY += lineSpacing; ////// NEW LINE

            Button screenmanager = new Button(page);
            screenmanager.SetPosition(5, posY);
            screenmanager.Width = 200;
            screenmanager.Text = "Screenmanager";
            screenmanager.Clicked += screenmanager_Clicked;

            posY += lineSpacing; ////// NEW LINE

            Button perfmon = new Button(page);
            perfmon.SetPosition(5, posY);
            perfmon.Width = 200;
            perfmon.Text = "Performance Monitor";
            perfmon.Clicked += perfmon_Clicked;

            posY += lineSpacing; ////// NEW LINE

            Button conwaygame = new Button(page);
            conwaygame.SetPosition(5, posY);
            conwaygame.Text = "Bleh";
            conwaygame.Clicked += conwaygame_Clicked;

        }

        void conwaygame_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var cway = new Conway(this.Scene, this.Parent);
            cway.Show();
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
