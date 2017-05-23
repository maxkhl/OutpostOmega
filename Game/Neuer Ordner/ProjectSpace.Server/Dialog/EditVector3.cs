using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jitter.LinearMath;

namespace OutpostOmega.Server.Dialog
{
    public partial class EditVector3 : Form
    {
        public JVector NewVector { get; set; }
        public EditVector3(JVector Vector, string Title = "Set Vector3")
        {
            InitializeComponent();
            nUD_X.Value = (decimal)Vector.X;
            nUD_Y.Value = (decimal)Vector.Y;
            nUD_Z.Value = (decimal)Vector.Z;
            this.Text = Title;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.NewVector = new JVector(
                (float)nUD_X.Value,
                (float)nUD_Y.Value,
                (float)nUD_Z.Value);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
