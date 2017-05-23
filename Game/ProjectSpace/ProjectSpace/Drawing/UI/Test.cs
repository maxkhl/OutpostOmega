using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using OpenTK;
using System.IO;

namespace OutpostOmega.Drawing.UI
{
    class Test : WindowControl
    {
        Scene Scene;
        Button connect;
        TextBox adress;
        TextBox username;
        //ListBox output;
        public Test(Scene Scene, Base parent)
            : base(parent, "TestWindow")
        {
            this.Scene = Scene;
            this.SetSkin(Scene.Skin, true);

            this.SetSize(230, 190);

            int posY = 20;

            /*output = new ListBox(this);
            output.SetPosition(5, posY);
            output.Width = 200;
            output.Height = 100;*/
            //posY += 105;


            Label username_l = new Label(this);
            username_l.SetPosition(10, posY);
            username_l.Text = "Username";
            posY += 20;

            var img = new System.Drawing.Bitmap(200, 100);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(img);
            graphics.DrawString("Test", System.Drawing.SystemFonts.DefaultFont, new System.Drawing.Pen(System.Drawing.Color.White, 1).Brush, new System.Drawing.PointF(0, 0));

            graphics.DrawLine(new System.Drawing.Pen(System.Drawing.Color.White, 5), new System.Drawing.Point(5, 60), new System.Drawing.Point(80, 60));

            ImagePanel imgPanel = new ImagePanel(this);
            imgPanel.Width = 200;
            imgPanel.Height = 100;
            Texture2D tex = new Texture2D(img);
            imgPanel.ImageHandle = tex.Handle;

        }

        public override void Think()
        {
            base.Think();
        }
    }
}
