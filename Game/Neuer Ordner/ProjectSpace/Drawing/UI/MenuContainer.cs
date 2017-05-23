using System;
using System.Drawing;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;

namespace OutpostOmega.Drawing.UI
{
    class MenuContainer : Gwen.Control.Base
    {
        Scenes.Game GameScene;
        public MenuContainer(Scenes.Game GameScene, Base parent)
            : base(parent)
        {
            this.GameScene = GameScene;
            Dock = Pos.Fill;
            SetSize(GameScene.Game.Width, GameScene.Game.Height);
            SetPosition(0, 0);

            //new Drawing.UI.PauseMenu(game, this, Width / 2, Height - 200);

            Label label = new Label(this);
            label.Text = "Project Space - Dev Build";
            label.SetPosition(5, 5);
            label.Font = new Gwen.Font(parent.GetCanvas().Skin.Renderer, "Consolas", 11);
            label.TextColor = Color.White;

            var toolbar = new ToolBar(GameScene, this);
            MenuBar mb = new MenuBar(GameScene, this);

            var chat = new Chat(GameScene, this);


            /*Button button = new Button(this);
            button.Width = 50;
            button.Height = 20;
            button.SetPosition(20, 80);*/

            //Gwen.Control.

            /*output = new MultilineTextBox(this);
            output.KeyboardInputEnabled = false;
            output.MouseInputEnabled = false;
            output.SetBounds(0, 0, 200, 100);*/
        }
    }
}
