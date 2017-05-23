using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using OpenTK;
using System.IO;
using OutpostOmega.Drawing.Screens;

namespace OutpostOmega.Drawing.UI
{
    class Screenmanager : Menu
    {
        ListBox screenlist;
        HorizontalSlider width;
        HorizontalSlider height;
        HorizontalSlider positX;
        HorizontalSlider positY;
        ImagePanel preview;
        CheckBox fullscreen;
        public Screenmanager(Scenes.Game GameScene, Base parent)
            : base(GameScene, parent, "Screenmanager")
        {
            //this.SetSkin(game.MenuSkin, true);

            this.SetSize(530, 500);

            int posY = 20;

            VerticalSplitter vsplitter = new VerticalSplitter(this);
            vsplitter.Dock = Pos.Fill;

            screenlist = new ListBox(this);
            screenlist.SetPosition(5, posY);
            screenlist.Width = 200;
            screenlist.Height = 225;
            screenlist.RowSelected += screenlist_RowSelected;

            vsplitter.SetPanel(0, screenlist);



            refresh_Clicked(this, null);

            GroupBox groupb = new GroupBox(this);
            groupb.SetPosition(210, posY);
            groupb.Dock = Pos.Fill;
            groupb.Text = "Screen Settings";
            vsplitter.SetPanel(1, groupb);

            int groupb_Y = 20;

            preview = new ImagePanel(groupb);
            preview.Dock = Pos.Top;
            preview.Height = 200;
            groupb_Y += 210;

            Button refresh = new Button(groupb);
            refresh.SetPosition(5, groupb_Y);
            refresh.SetText("Refresh");
            refresh.Clicked += refresh_Clicked;
            groupb_Y += 20;

            fullscreen = new CheckBox(groupb);
            fullscreen.SetPosition(5, groupb_Y);
            fullscreen.IsChecked = false;
            fullscreen.IsDisabled = true;
            fullscreen.CheckChanged += fullscreen_CheckChanged;

            Label fullscreen_l = new Label(groupb);
            fullscreen_l.SetPosition(25, groupb_Y);
            fullscreen_l.Text = "Fullscreen";
            groupb_Y += 20;

            width = new HorizontalSlider(groupb);
            width.SetPosition(5, groupb_Y);
            width.Max = Scene.Game.Width;
            width.Width = 75;
            width.Height = 25;
            width.IsDisabled = true;
            width.ValueChanged += width_ValueChanged;

            height = new HorizontalSlider(groupb);
            height.SetPosition(80, groupb_Y);
            height.Max = Scene.Game.Height;
            height.Width = 75;
            height.Height = 25;
            height.IsDisabled = true;
            height.ValueChanged += height_ValueChanged;

            groupb_Y += 20;

            positX = new HorizontalSlider(groupb);
            positX.SetPosition(5, groupb_Y);
            positX.Width = 75;
            positX.Height = 25;
            positX.Max = Scene.Game.Width;
            positX.IsDisabled = true;
            positX.ValueChanged += positX_ValueChanged;

            positY = new HorizontalSlider(groupb);
            positY.SetPosition(80, groupb_Y);
            positY.Width = 75;
            positY.Height = 25;
            positY.Max = Scene.Game.Height;
            positY.IsDisabled = true;
            positY.ValueChanged += positY_ValueChanged;

            groupb_Y += 20;
        }

        Screen SelectedScreen;
        void screenlist_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            //fullscreen.IsDisabled = false;
            width.IsDisabled = false;
            height.IsDisabled = false;
            positX.IsDisabled = false;
            positY.IsDisabled = false;

            SelectedScreen = (Screen)screenlist.SelectedRow.UserData;
            width.Value = SelectedScreen.Width;
            height.Value = SelectedScreen.Height;
            positX.Value = SelectedScreen.X;
            positY.Value = SelectedScreen.Y;
            preview.ImageHandle = (int)SelectedScreen.RenderTarget.OutTexture;
        }

        void width_ValueChanged(Base sender, EventArgs arguments)
        {
            if(SelectedScreen != null)
                SelectedScreen.Width = (int)width.Value;
        }

        void height_ValueChanged(Base sender, EventArgs arguments)
        {
            if (SelectedScreen != null)
                SelectedScreen.Height = (int)height.Value;
        }

        void positX_ValueChanged(Base sender, EventArgs arguments)
        {
            if (SelectedScreen != null)
                SelectedScreen.X = (int)positX.Value;
        }

        void positY_ValueChanged(Base sender, EventArgs arguments)
        {
            if (SelectedScreen != null)
                SelectedScreen.Y = (int)positY.Value;
        }

        void fullscreen_CheckChanged(Base sender, EventArgs arguments)
        {

        }

        void refresh_Clicked(Base sender, ClickedEventArgs arguments)
        {
            screenlist.Clear();
            foreach (Screen screen in ((Scenes.Game)Scene).Screens)
            {
                screenlist.AddRow(screen.ToString(), "", screen);
            }
        }

        /*void adress_TextChanged(Base sender, EventArgs arguments)
        {
            if (adress.Text != "" && username.Text != "")
                connect.IsDisabled = false;
            else
                connect.IsDisabled = true;
        }

        void connect_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var client = new OutpostOmega.Network.nClient(username.Text);
            //client.Connect(adress.Text, )
        }*/
    }
}
