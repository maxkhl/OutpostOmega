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
    public partial class PropGrid : Form
    {
        public bool ValueChanged = false;

        public PropGrid(object Item, string Title)
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = Item;
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ValueChanged = true;
        }
    }
}
