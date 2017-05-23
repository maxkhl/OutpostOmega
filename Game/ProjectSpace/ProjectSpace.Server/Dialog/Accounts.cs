using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace OutpostOmega.Server.Dialog
{
    public partial class Accounts : Form
    {
        public Accounts(Data.Account[] Accounts)
        {
            InitializeComponent();

            var properties = typeof(Data.Account).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsEnum)
                {
                    var column = new DataGridViewComboBoxColumn() { Name = property.Name };
                    foreach (var opt in property.PropertyType.GetEnumNames())
                        column.Items.Add(opt);

                    dataview.Columns.Add(column);
                }
                else
                    dataview.Columns.Add(new DataGridViewTextBoxColumn() { Name = property.Name });
            }
        }
    }
}
