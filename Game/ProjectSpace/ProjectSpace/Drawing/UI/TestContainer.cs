using System;
using System.Drawing;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;

namespace ProjectSpace.Drawing.UI
{
    class TestContainer : DockBase
    {
        public TestContainer(Base parent, int Width, int Height)
            : base(parent)
        {
            Dock = Pos.Fill;
            SetSize(Width, Height);
            SetPosition(0, 0);

            //new PauseMenu(parent, 200, 100);

            Label label = new Label(this);
            label.Text = "Testtext";
            label.SetPosition(5, 5);
            label.Font = new Gwen.Font(parent.GetCanvas().Skin.Renderer, "Consolas", 11);
            label.TextColor = Color.Blue;

            Button button = new Button(this);
            button.Width = 50;
            button.Height = 20;
            button.SetPosition(20, 80);
        }
    }
}
