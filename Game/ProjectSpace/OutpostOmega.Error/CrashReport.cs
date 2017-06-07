using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutpostOmega.Error
{
    public partial class CrashReport : Form
    {
        public CrashReport(Exception e)
        {
            InitializeComponent();

            report_output.Text += e.GetType().ToString() + Environment.NewLine;
            report_output.Text += e.Message + Environment.NewLine;
            report_output.Text += e.Source + Environment.NewLine;
            report_output.Text += e.StackTrace + Environment.NewLine;
            report_output.Text += e.InnerException + Environment.NewLine;
            report_output.Text += e.TargetSite + Environment.NewLine;
            foreach (var pair in e.Data)
                report_output.Text += pair.ToString() + Environment.NewLine;
        }

        private void CrashReport_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill(); // AAARGHHHGHglglgghh...
        }
    }
}
