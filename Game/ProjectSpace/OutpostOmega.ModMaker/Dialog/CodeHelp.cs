using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutpostOmega.Game.Lua;

namespace OutpostOmega.ModMaker.Dialog
{
    public partial class CodeHelp : Form
    {
        public CodeHelp()
        {
            InitializeComponent();
        }

        private void CodeHelp_Load(object sender, EventArgs e)
        {
            var methods = typeof(Assembly).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            List<OutpostOmega.Game.Lua.LuaDocumentationAttr> Attributes = new List<OutpostOmega.Game.Lua.LuaDocumentationAttr>();
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(LuaDocumentationAttr), false);
                if (attributes.Length == 1)
                {
                    ((LuaDocumentationAttr)attributes[0]).MethodInfo = method;
                    Attributes.Add((LuaDocumentationAttr)attributes[0]);
                }
            }
            Attributes = Attributes.OrderBy(o => o.Category).ToList();
            string OldCategory = "";
            string Output = "";
            foreach (var attribute in Attributes)
            {
                if (OldCategory != attribute.Category)
                {

                    Output += "####" + new String('#', attribute.Category.Length) + "####" + Environment.NewLine;
                    Output += "### " + attribute.Category + " ###" + Environment.NewLine;
                    Output += "####" + new String('#', attribute.Category.Length) + "####" + Environment.NewLine;
                }
                OldCategory = attribute.Category;

                Output += attribute.MethodInfo.ToString() + Environment.NewLine;
                Output += "Description: " + attribute.Description + Environment.NewLine;
                Output += "Parameters: " + attribute.Parameters + Environment.NewLine;
                Output += "Returns: " + attribute.Return + Environment.NewLine;
                Output += Environment.NewLine;
            }
            textBox1.Text = Output;
        }
    }
}
