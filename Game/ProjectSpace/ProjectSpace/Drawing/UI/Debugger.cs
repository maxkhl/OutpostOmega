using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using OpenTK;
using OutpostOmega.Game;

namespace OutpostOmega.Drawing.UI
{
    class Debugger : Menu
    {
        bool IsGameScene = false;
        GameObject Target = null;
        ScrollControl Scroll;
        Scenes.Game GameScene;
        List<Label> Labels = new List<Label>();
        public Debugger(Scenes.Game GameScene, Base parent)
            : base(GameScene, parent, "Debugger")
        {
            this.SetSize(220, 200);
            this.Position(Pos.Center);
            this.GameScene = GameScene;
            //IsGameScene = typeof(Scenes.Game) == Scene.GetType() && ((Scenes.Game)Scene).World != null;


            //Center meh
            this.SetPosition(Scene.Game.Width - this.Width, Scene.Game.Height - this.Height);


            Scroll = new ScrollControl(this);
            Scroll.Dock = Pos.Fill;
        }

        public override void Think()
        {
            if (Target != GameScene.World.Player.Mob.View.TargetGameObject)
            {
                Target = GameScene.World.Player.Mob.View.TargetGameObject;
                if (Target != null)
                    NewTarget();
                else
                    Clear();
            }
            base.Think();
        }

        private void NewTarget()
        {
            Clear();

            this.Title = "Debugger - " + Target.ToString();

            int posY = 5;

            var properties = Target.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach(var prop in properties)
            {
                var value = prop.GetValue(Target);
                Labels.Add(new Label(Scroll) { X = 5, Y = posY, Text = prop.Name + ": " + (value != null ? value.ToString() : "null") });
                posY += 15;
            }
        }

        private void Clear()
        {
            this.Title = "Debugger";
            foreach (var label in Labels)
                label.Dispose();
            Labels.Clear();
        }
    }
}
