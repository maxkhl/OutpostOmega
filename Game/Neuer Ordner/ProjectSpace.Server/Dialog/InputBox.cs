using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutpostOmega.Server.Dialog
{
    public partial class InputBox : Form
    {
        /// <summary>
        /// Text, the user typed in
        /// </summary>
        public string InputText { get; private set; }

        public InputBox(string Title, string Text = "")
        {
            InitializeComponent();
            this.Text = Title;
            textBox1.Text = Text;
        }

        bool ResultOk = false;
        private void button1_Click(object sender, EventArgs e)
        {
            InputText = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            ResultOk = true;
            this.Close();
        }

        private void InputBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ResultOk)
                this.DialogResult = DialogResult.Cancel;
        }
    }
}
