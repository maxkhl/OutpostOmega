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
    public partial class EditObject : Form
    {
        public EditObject(object DisplayObject)
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = DisplayObject;

            if (typeof(Game.GameObject).IsAssignableFrom(DisplayObject.GetType()))
            {
                moveToolStripMenuItem.Enabled = true;
            }
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var gameObject = (Game.GameObject)propertyGrid1.SelectedObject;
            var newVectorDialog = new EditVector3(gameObject.Position, "Edit Position");
            if (newVectorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                gameObject.SetPosition(newVectorDialog.NewVector);

            propertyGrid1.Refresh();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("You try to delete a object. Let me make clear that this could break your game! There could be other gameobjects that rely on this one. I warned you!", "Object deletion", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                var gameObject = (Game.GameObject)propertyGrid1.SelectedObject;
                gameObject.Dispose();
                this.Close();
            }
        }
    }
}
