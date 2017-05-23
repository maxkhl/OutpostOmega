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

namespace OutpostOmega.Server.Dialog
{
    public partial class NewWorld : Form
    {
        /// <summary>
        /// New World
        /// </summary>
        public World World { get; set; }

        public NewWorld()
        {
            InitializeComponent();
            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            if (tB_Name.Text == "")
            {
                MessageBox.Show("Please enter a text");
                return;
            }

            World = new World(tB_Name.Text);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
