using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing.Screens
{
    /// <summary>
    /// Default - human eye - screen
    /// </summary>
    class Default : Screen
    {
        private Drawing.RenderOptions renderOptions;

        public Default(Scenes.Game GameScene)
            : base(GameScene, GameScene.Game.Width, GameScene.Game.Height)
        {
            this.Camera = new View.GOCamera(GameScene.World.Player.Mob.View, this);

            this.Shader = new Drawing.PPShader(
                new System.IO.FileInfo(@"Content\Shader\Default\Default_PPS.glsl"),
                4); // needs 4 passes for that pretty glow ;>

            renderOptions = new RenderOptions()
            {
                Wireframe = false,
                Color = OpenTK.Graphics.Color4.White,
            };

        }

        void Game_FocusedChanged(object sender, EventArgs e)
        {

        }

        public float amount = 0.25f; // test
        protected override void SetUniform(int Pass)
        {
            amount = 0.2f;
            if (Pass == 0)
                GL.Uniform2(Shader.GetUniformLocation("uShift"), new Vector2(amount / (float)Width, 0));

            if (Pass == 1)
                GL.Uniform2(Shader.GetUniformLocation("uShift"), new Vector2(0, amount / (float)Height));

            if (Pass == 2)
                GL.Uniform2(Shader.GetUniformLocation("uShift"), new Vector2(amount / (float)Width, amount / (float)Height));

            if (Pass == 3)
                GL.Uniform2(Shader.GetUniformLocation("uShift"), new Vector2(-(amount / (float)Width), amount / (float)Height));
            
            base.SetUniform(Pass);
        }

        protected override void DrawScene()
        {
            if (GameScene.Drawer == null)
                return;

            GameScene.Drawer.Draw(renderOptions);
            Tools.Draw.Atmos(GameScene.World);
        }
    }
}
