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
    /// Cybernet view. Thats how the AI sees the station
    /// </summary>
    class Cybernet : Screen
    {
        private Drawing.RenderOptions renderOptions;

        public Cybernet(Scenes.Game GameScene)
            : base(GameScene, GameScene.Game.Width, GameScene.Game.Height)
        {
            this.Camera = new View.GOCamera(GameScene.World.Player.Mob.View, this); //We want a FPS camera
            //this.Camera = new View.OrbitCamera(); //Test


            //Assign the cybah shader
            this.Shader = new Drawing.PPShader(
                new System.IO.FileInfo(@"Content\Shader\Cyber\Cyber_PPS.glsl"),
                4); // needs 2 passes for that pretty glow ;>

            renderOptions = new RenderOptions()
            {
                Wireframe = true, // Wireframe mode is important!
                Shader = Drawing.Shader.Load(
                    new System.IO.FileInfo(@"Content\Shader\Cyber\Cyber_VS.glsl"),
                    new System.IO.FileInfo(@"Content\Shader\Cyber\Cyber_FS.glsl")),
                Color = OpenTK.Graphics.Color4.White,
            };

            GameScene.Game.FocusedChanged += Game_FocusedChanged;
        }

        void Game_FocusedChanged(object sender, EventArgs e)
        {

        }

        public float amount = 1.4f; // test
        protected override void SetUniform(int Pass)
        {

            if(Pass == 0)
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
        }
    }
}
