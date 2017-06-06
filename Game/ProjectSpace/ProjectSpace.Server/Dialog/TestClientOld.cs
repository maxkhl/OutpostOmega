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
    public partial class TestClientOld : Form
    {
        public nClient uClient { get; set; }

        Timer timer;

        public TestClientOld()
        {
            InitializeComponent();

            string Username = "TestClient";
            var inpBox = new InputBox("Username", "TestClient");
            if (inpBox.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                Username = inpBox.InputText;

            uClient = new nClient(Username);

            uClient.Connect("localhost", 12041);

            timer = new Timer();
            timer.Tick +=timer_Tick;
            timer.Interval = 20;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            string msg = "";
            if (uClient.Output.TryDequeue(out msg))
                tB_Output.Text += msg + Environment.NewLine;
        }

        private void TestClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            uClient.Disconnect("Dialog closed");
        }
    }
}
