using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using Jitter.LinearMath;
using System.Reflection;
using OutpostOmega.Game.GameObjects.Attributes;

namespace OutpostOmega.Drawing.UI
{
    class VideoPlayer : Menu
    {
        HorizontalSplitter vsplitter;
        ImagePanel VideoPanel;
        HorizontalSlider slider;
        Video Video;
        public VideoPlayer(Scene Scene, Base parent)
            : base(Scene, parent, "Video Player")
        {
            this.SetSize(500, 600);

            MenuStrip mstrip = new MenuStrip(this);
            var file = mstrip.AddItem("File");
            var open = file.Menu.AddItem("Open");
            open.Pressed += open_Pressed;

            var control = mstrip.AddItem("Control");
            var play = control.Menu.AddItem("Play");
            play.Pressed += play_Pressed;
            var pause = control.Menu.AddItem("Pause");
            pause.Pressed += pause_Pressed;

            var examples = mstrip.AddItem("Control");
            var nasa = examples.Menu.AddItem("Nasa");
            nasa.Pressed += nasa_Pressed;

            var apollo = examples.Menu.AddItem("Apollo");
            apollo.Pressed += apollo_Pressed;

            vsplitter = new HorizontalSplitter(this);
            vsplitter.Dock = Pos.Fill;
            vsplitter.SetVValue(0.9f);

            /*TreeControl treeV = new TreeControl(this);
            treeV.Dock = Pos.Fill;
            splitter.SetPanel(0, treeV);*/

            VideoPanel = new ImagePanel(this);
            vsplitter.SetPanel(0, VideoPanel);

            slider = new HorizontalSlider(this);
            slider.ValueChanged += slider_ValueChanged;
            vsplitter.SetPanel(1, slider);
        }

        void apollo_Pressed(Base sender, EventArgs arguments)
        {
            LoadVideo(@"Content\Video\apo16004.wmv");
        }

        void nasa_Pressed(Base sender, EventArgs arguments)
        {
            LoadVideo(@"Content\Video\anni001.wmv");
        }

        void pause_Pressed(Base sender, EventArgs arguments)
        {
            if (Video != null)
                Video.Pause();
            else
                new MessageBox(this, "Open a video file first", "No video loaded");
        }

        void play_Pressed(Base sender, EventArgs arguments)
        {
            if (Video != null)
                Video.Play();
            else
                new MessageBox(this, "Open a video file first", "No video loaded");
        }

        public override void Think()
        {
            if (Video != null)
                Video.Update();
            base.Think();
        }

        void open_Pressed(Base sender, EventArgs arguments)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadVideo(ofd.FileName);
            }
        }

        void LoadVideo(string FilePath)
        {
            if (Video != null)
                Video.Dispose();

            Video = new Video(new System.IO.FileInfo(FilePath));
            VideoPanel.ImageHandle = Video.Handle;
        }

        void slider_ValueChanged(Base sender, EventArgs arguments)
        {
        }
    }
}
