using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OutpostOmega.Network;

namespace OutpostOmega.Server.Dialog
{
    public partial class TestClient : Form
    {
        public nClient uClient { get; set; }

        Timer timer;

        public TestClient()
        {
            InitializeComponent();

            uClient = new nClient("TestClient");

            timer = new Timer();
            timer.Tick +=timer_Tick;
            timer.Interval = 20;
            timer.Start();
        }

        void uClient_CreateNewObject(object Object)
        {

        }

        void timer_Tick(object sender, EventArgs e)
        {
            string msg = "";
            if (uClient.Output.TryDequeue(out msg))
                tB_Output.Text += msg + Environment.NewLine;

            packetcounter.Text = "Packets per second: " + uClient.PacketsPerSecond.ToString();
        }

        private void TestClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            uClient.Disconnect("Dialog closed");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            uClient.Connect("localhost", 12041);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string msg = "";
            if(uClient.Output.TryDequeue(out msg))
                tB_Output.Text += msg + Environment.NewLine;
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            uClient.Connect("localhost", 12041);
        }
    }
}
