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
    public partial class NewEnum : Form
    {
        Type EnumType;
        public object ReturnEnum;
        public NewEnum(string Title, Type EnumType)
        {
            InitializeComponent();
            this.Text = Title;
            this.EnumType = EnumType;

            var names = Enum.GetNames(EnumType);
            foreach(var name in names)
            {
                comboBox1.Items.Add(name);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnEnum = Enum.Parse(EnumType, comboBox1.SelectedItem.ToString());
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
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
