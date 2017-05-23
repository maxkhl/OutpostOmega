using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using System.Drawing;

namespace OutpostOmega.Drawing.UI
{
    class Chat : DockBase
    {
        MultilineTextBox output;
        Scenes.Game Scene;
        public Chat(Scenes.Game Scene, Base Parent)
            : base(Parent)
        {
            this.Scene = Scene;
            this.Height = 100;
            //this.Width = Scene.Game.Width;
            this.Dock = Pos.Bottom;



            output = new MultilineTextBox(this);
            output.KeyboardInputEnabled = false;
            //output.MouseInputEnabled = false;
            output.Dock = Pos.Fill;
            output.SetSkin(Scene.HUDSkin);
            output.Redraw();
            output.Margin = new Margin(20, 20, 20, 10);
        }

        public void Message(string Text)
        {
            output.Text += Text + Environment.NewLine;
        }

        public void Clear()
        {
            output.Text = "";
        }

        public override void Think()
        {

            base.Think();
        }
    }
}
