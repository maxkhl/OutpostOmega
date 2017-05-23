using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutpostOmega.Game;
using OutpostOmega.Game.Lua;
using System.Xml.Linq;

namespace OutpostOmega.ModMaker.Dialog
{
    public partial class NewMod : Form
    {
        public ModPack NewModPack;
        public NewMod()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tB_Name.Text) &&
                !String.IsNullOrWhiteSpace(tB_Author.Text))
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                //fbd.Title = "ModPack Destination";
                //sfd.Filter = "Mod Definition (*.xml)|*.xml";
                //sfd.DefaultExt = ".xml";

                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    NewModPack = new ModPack(tB_Name.Text, tB_Author.Text, version.Value.ToString(), null);

                    var error = NewModPack.Save(new System.IO.DirectoryInfo(fbd.SelectedPath));

                    if (error == "")
                    {

                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                    else
                        MessageBox.Show(error);
                }
            }
            else
                MessageBox.Show("Please fill all fields");
        }

        private void NewMod_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }
    }
}
