using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OutpostOmega.Network;
using OutpostOmega.Game;

namespace OutpostOmega.Server.Dialog
{
    public partial class TestClient : Form
    {
        public nClient uClient { get; set; }

        World GameWorld;
        World ServerWorld;

        Timer timer;

        public TestClient(Game.World ServerWorld)
        {
            this.ServerWorld = ServerWorld;
            InitializeComponent();

            uClient = new nClient("TestClient");
            uClient.NewWorldReceived += uClient_NewWorldReceived;

            timer = new Timer();
            timer.Tick +=timer_Tick;
            timer.Interval = 20;
            timer.Start();
        }

        void uClient_NewWorldReceived(World oldWorld, World newWorld)
        {
            if (oldWorld != null) oldWorld.Dispose();

            this.GameWorld = newWorld;

            this.GameWorld.Player.Mob.View.PropertyChanged += delegate(GameObject Object, string PropertyName, bool IndirectChange)
            {
                if (PropertyName == "Orientation")
                {

                }
            };
        }

        void uClient_CreateNewObject(object Object)
        {

        }
        Game.Tools.MouseState MouseState = new Game.Tools.MouseState();
        void timer_Tick(object sender, EventArgs e)
        {
            string msg = "";
            if (uClient.Output.TryDequeue(out msg))
                tB_Output.Text += msg + Environment.NewLine;

            tsl_status.Text = "Packets per second: " + uClient.PacketsPerSecond.ToString();


            var OldMouseState = new Game.Tools.MouseState(MouseState);
            MouseState.X += inp_cursor.X - 75;
            MouseState.Y += inp_cursor.Y - 75;
            if (this.GameWorld != null)
            {
                this.GameWorld.Update(new Game.Tools.KeybeardState(), MouseState, 20);
                uClient.SendMouseState(MouseState, OldMouseState);
            }

            
            RefreshAimSim();
        }

        private void TestClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            uClient.Disconnect("Dialog closed");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string msg = "";
            if(uClient.Output.TryDequeue(out msg))
                tB_Output.Text += msg + Environment.NewLine;
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uClient.Connect("localhost", 12041);
        }

        private void RefreshAimSim()
        {
            DrawDarts();
            DrawCharState();
        }

        private void DrawCharState()
        {
            if (this.GameWorld == null) return;

            var bitmap = new Bitmap(150, 150);
            Pen redPen = new Pen(Color.Red, 1);
            Pen bluePen = new Pen(Color.Blue, 1);
            Pen greenPen = new Pen(Color.Green, 1);

            var forwV = this.GameWorld.Player.Mob.View.Forward;

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawLine(redPen, new Point(75, 75), new Point((int)(forwV.X * 75), (int)(forwV.Y * 75)));
                graphics.DrawLine(bluePen, new Point(75, 75), new Point((int)(forwV.X * 75), (int)(forwV.Z * 75)));
                graphics.DrawLine(greenPen, new Point(75, 75), new Point((int)(forwV.Y * 75), (int)(forwV.Z * 75)));
            }

            pB_CharState.Image = (Image)bitmap;

        }

        private void DrawDarts()
        {
            var bitmap = new Bitmap(150, 150);

            Pen blackPen = new Pen(Color.Black, 3);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                //graphics.DrawLine(blackPen, x1, y1, x2, y2);
                graphics.DrawEllipse(blackPen, 0, 0, 150, 150);
                graphics.DrawEllipse(blackPen, 25, 25, 100, 100);
                graphics.DrawEllipse(blackPen, 50, 50, 50, 50);
                graphics.DrawEllipse(blackPen, 60, 60, 30, 30);
                graphics.DrawEllipse(blackPen, 70, 70, 10, 10);

                graphics.DrawLine(blackPen, 0, 75, 150, 75);
                graphics.DrawLine(blackPen, 75, 0, 75, 150);
            }

            var inp_bmp = (Bitmap)bitmap.Clone();
            var out_bmp = bitmap;

            Pen redPen = new Pen(Color.Red, 2);
            using (var graphics = Graphics.FromImage(inp_bmp))
            {
                graphics.DrawEllipse(redPen, inp_cursor.X - 2, inp_cursor.Y - 2, 4, 4);
            }
            using (var graphics = Graphics.FromImage(out_bmp))
            {
                graphics.DrawEllipse(redPen, out_cursor.X - 2, out_cursor.Y - 2, 4, 4);
            }

            pB_input.Image = (Image)inp_bmp;
            pB_output.Image = (Image)out_bmp;
        }

        Point inp_cursor = new Point(75, 75);
        private void pB_input_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            inp_cursor = me.Location;
        }

        Point out_cursor = new Point(75, 75);
    }
}
