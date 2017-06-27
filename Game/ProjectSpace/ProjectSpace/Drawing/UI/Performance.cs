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
    class Performance : Menu
    {
        long FrameTime = 0;
        public Performance(Scene Scene, Base parent)
            : base(Scene, parent, "Performance Tool")
        {
            this.SetSize(600, 560);
            var hSplit = new HorizontalSplitter(this);
            hSplit.Dock = Pos.Fill;
            hSplit.SetVValue(0.6f);

            var groupBox = new GroupBox(this);
            hSplit.SetPanel(0, groupBox);
            groupBox.Text = "Speed Indicators";


            FrameTimeLabel = new Label(groupBox);
            FrameTimeLabel.Font = FrameTimeLabel.Font.Copy();
            FrameTimeLabel.Font.Size = 15;
            FrameTimeLabel.Dock = Pos.Top;

            long FTime = 0;
            for (int i = 0; i < FrameTimeStat.Length; i++)
            {
                FTime += FrameTimeStat[i];
            }
            FTime /= FrameTimeStat.Length;
            FrameTimeLabel.Text = string.Format("Overall Frametime: {0} ms - Current Frametime: {1} ms", FTime, FrameTime);

            var sControl = new ScrollControl(groupBox);
            sControl.Dock = Pos.Fill;
            SetBars(sControl);

            var tabControl = new DockedTabControl(this);
            hSplit.SetPanel(1, tabControl);


            var overview = tabControl.AddPage("Overview");
            var wrldstat = tabControl.AddPage("World");
            //var uiperfrm = tabControl.AddPage("UI");

            BuildOverview(overview.Page);

            BuildWorldStatistic(wrldstat.Page);


            //ImagePanel screentest = new ImagePanel(this);
            //hSplit.SetPanel(0, screentest);
            //screentest = new ImagePanel(this);
            //hSplit.SetPanel(1, screentest);

        }

        Base BarParent;
        Label CaptureTimeLabel;
        Label FrameTimeLabel;
        public void SetBars(Base Parent)
        {
            FrameTime = (long)(Scene.Game.RenderTime * 1000 + Scene.Game.UpdateTime * 1000);

            BarParent = Parent;
            foreach (Base child in Parent.Children)
                child.Dispose();
            Parent.Children.Clear();

            CaptureTimeLabel = new Label(Parent);
            CaptureTimeLabel.Text = string.Format("Snapshot {1} - {0}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString());


            var button = new Button(Parent);
            button.SetPosition(220, 0);
            button.Text = "Toggle Capturemode";
            button.Clicked += button_Clicked;

            var refbars = new Button(Parent);
            refbars.SetPosition(340, 0);
            refbars.Text = "Reload Bars";
            refbars.Clicked += refbars_Clicked;

            int PosY = 20;


            foreach(Tools.Performance.Counter counter in Tools.Performance.CounterList)
            {
                var label = new Label(Parent);
                label.X = 5;
                label.Y = PosY;
                PosY += 15;
                label.Text = string.Format("{0} ({1} MS {2} Hz)", counter.Name, counter.RoundTripTime, (float)counter.RoundTripTime / 1000);
                label.Name = counter.Name;
                label.UserData = counter;

                var progressBar = new ProgressBar(Parent);
                progressBar.Text = counter.Name;
                progressBar.UserData = counter;
                progressBar.SetBounds(0, PosY, progressBar.Parent.InnerBounds.Width, 20);
                progressBar.Value = (float)counter.RoundTripTime / (float)FrameTime;
                PosY += 25;
            }
        }

        void refbars_Clicked(Base sender, ClickedEventArgs arguments)
        {
            SetBars(BarParent);
        }

        void button_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _LifeCapture = !_LifeCapture;
        }

        private void BuildOverview(Base Parent)
        {

            var listBox = new ListBox(Parent);
            listBox.Dock = Pos.Fill;
            
            //textBox.Disable();
            
            listBox.AddRow(string.Format("Current Scenetype: {0} \n", Scene.GetType().ToString()));

            var SceneType = Scene.GetType();
            var SceneProperties = SceneType.GetProperties();
            foreach (System.Reflection.PropertyInfo prop in SceneProperties)
            {
                listBox.AddRow(string.Format("{0}: {1} ({2}) \n", prop.Name, prop.GetValue(Scene).ToString(), prop.PropertyType.FullName));
            }

            var GameType = Scene.Game.GetType();
            var GameProperties = GameType.GetProperties();
            foreach(System.Reflection.PropertyInfo prop in GameProperties)
            {
                listBox.AddRow(string.Format("{0}: {1} ({2}) \n", prop.Name, prop.GetValue(Scene.Game).ToString(), prop.PropertyType.FullName));
            }
            
            /*label = new Label(Parent);
            label.Text = string.Format("Main Frame Counter: {0}", Scene.);
            label.SetPosition(PosY, PosX);
            PosY += 15;*/
        }

        private void BuildWorldStatistic(Base Parent)
        {
            if(Scene.GetType() == typeof(Scenes.Game))
            {
                var listBox = new ListBox(Parent);
                listBox.Dock = Pos.Fill;
                
                var gameScene = (Scenes.Game)Scene;

                listBox.AddRow(string.Format("{0}: {1}\n", "GameObject Count", gameScene.World.AllGameObjects.Count));
                listBox.AddRow(string.Format("{0}: {1}\n", "Structure Count", gameScene.World.Structures.Count));

                int chunkCount = 0;
                foreach(OutpostOmega.Game.Turf.Structure structure in gameScene.World.Structures)
                {
                    chunkCount += structure.chunks.Count;
                }
                listBox.AddRow(string.Format("{0}: {1}\n", "Overall Chunk Count", chunkCount));
                listBox.AddRow(string.Format("{0}: {1}\n", "Estimated Turf Count", chunkCount * Math.Pow(OutpostOmega.Game.Turf.Chunk.SizeXYZ, 3)));

                var WorldType = gameScene.World.GetType();
                var WorldProperties = WorldType.GetProperties();
                foreach (System.Reflection.PropertyInfo prop in WorldProperties)
                {
                    listBox.AddRow(string.Format("{0}: {1} ({2}) \n", prop.Name, prop.GetValue(gameScene.World).ToString(), prop.PropertyType.FullName));
                }
            }
        }

        long[] FrameTimeStat = new long[100];
        bool _LifeCapture = true;
        public void Update()
        {
            FrameTime = (long)(Scene.Game.RenderTime * 1000 + Scene.Game.UpdateTime * 1000);


            for (int i = 0; i < FrameTimeStat.Length; i++)
            {
                if (i + 1 >= FrameTimeStat.Length)
                    continue;

                FrameTimeStat[i + 1] = FrameTimeStat[i];
            }
            FrameTimeStat[0] = FrameTime;

            long FTime = 0;
            for (int i = 0; i < FrameTimeStat.Length; i++)
            {
                FTime += FrameTimeStat[i];
            }
            FTime /= FrameTimeStat.Length;
            FrameTimeLabel.Text = string.Format("Overall Frametime: {0} ms - Current Frametime: {1} ms", FTime, FrameTime);

            if (BarParent != null && _LifeCapture)
            {
                CaptureTimeLabel.Text = string.Format("Snapshot {1} - {0}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToShortDateString());
                foreach (Base child in BarParent.Children)
                {
                    if (child.GetType() == typeof(Label) && child.UserData != null)
                    {
                        var counter = (Tools.Performance.Counter)child.UserData;
                        ((Label)child).Text = string.Format("{0} ({1} MS {2} Hz)", counter.Name, counter.RoundTripTime, (float)counter.RoundTripTime / 1000);
                    }

                    if (child.GetType() == typeof(ProgressBar))
                    {
                        var counter = (Tools.Performance.Counter)child.UserData;
                        ((ProgressBar)child).Value = (float)counter.RoundTripTime / (float)FrameTime;


                        if (child.Width != child.Parent.InnerBounds.Width)
                            child.Width = child.Parent.InnerBounds.Width;
                    }
                }
            }
        }

        public override void Think()
        {
            Update();
            base.Think();
        }
    }
}
