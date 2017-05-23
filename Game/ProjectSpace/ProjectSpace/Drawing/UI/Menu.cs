using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwen;
using Gwen.Control;

namespace OutpostOmega.Drawing.UI
{
    /// <summary>
    /// Base - menu class
    /// </summary>
    abstract class Menu : WindowControl
    {
        public Scene Scene { get; set; }
        public Menu(Scene Scene, Base parent, string Title) : base(parent, Title)
        {
            this.Scene = Scene;
        }

        protected override void CloseButtonPressed(Base control, EventArgs args)
        {
            if (this.Parent != null)
                this.Parent.RemoveChild(this, true);

            base.CloseButtonPressed(control, args);
        }
    }
}
