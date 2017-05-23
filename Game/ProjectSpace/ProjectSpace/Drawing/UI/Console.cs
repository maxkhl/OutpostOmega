using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;

namespace OutpostOmega.Drawing.UI
{
    class Console : DockBase
    {
        MultilineTextBox output;
        public Console(Scenes.Game Scene, Base Parent)
            : base(Parent)
        {
            this.Height = 400;
            //this.Width = Scene.Game.Width;
            this.Dock = Pos.Bottom;
            this.Hide();


            output = new MultilineTextBox(this);
            output.KeyboardInputEnabled = false;
            //output.MouseInputEnabled = false;
            output.Dock = Pos.Fill;
            output.SetSkin(Scene.HUDSkin);
            output.Redraw();
            output.Margin = new Margin(20, 20, 20, 10);
        }

        public void Toggle()
        {
            if (this.IsHidden)
            {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }

        protected override bool OnKeyPressed(Key key, bool down = true)
        {
            /*if (key == Key.Home && down == false)
            {

            }*/

            return base.OnKeyPressed(key, down);
        }

        public void Message(string Text)
        {
            output.Text += Text + Environment.NewLine;
#if DEBUG
            System.Diagnostics.Debug.Print("CONSOLE: " + Text);
#endif
        }

        public void Clear()
        {
            output.Text = "";
        }
    }
}
