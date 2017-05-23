using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutpostOmega.ModMaker.Dialog
{
    public partial class NewString : Form
    {
        public NewString(string Title)
        {
            InitializeComponent();
            this.Text = Title;
        }

        public string ReturnString;
        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                ReturnString = textBox1.Text;
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            else
                MessageBox.Show("Please fill the field");
        }

        private void String_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }
    }
}
