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
    class Default_Deferred : Screen
    {
        private Drawing.RenderOptions renderOptions;

        public Default_Deferred(Scenes.Game GameScene)
            : base(GameScene, GameScene.Game.Width, GameScene.Game.Height, new RenderTargets.DefferedRenderTarget(GameScene.Game.Width, GameScene.Game.Height))
        {
            this.Camera = new View.GOCamera(GameScene.World.Player.Mob.View, this);

            this.Shader = new Drawing.PPShader(
                new System.IO.FileInfo(@"Content\Shader\Deferred\Deferred_PPS.glsl"),
                new System.IO.FileInfo(@"Content\Shader\Deferred\Deferred_PPVS.glsl"),
                1); // needs 4 passes for that pretty glow ;>

            renderOptions = new RenderOptions()
            {
                Wireframe = false,
                Color = OpenTK.Graphics.Color4.White,
            };
            var dRenderTarget = (RenderTargets.DefferedRenderTarget)RenderTarget;
            dRenderTarget.CleanUpDraw += CleanUpDraw;
            dRenderTarget.PrepareDraw += PrepareDraw;
        }

        void Game_FocusedChanged(object sender, EventArgs e)
        {

        }

        public float amount = 0.25f; // test
        protected override void SetUniform(int Pass)
        {
            //var dRenderTarget = (RenderTargets.DefferedRenderTarget)RenderTarget;



            //GL.Uniform1(Shader.GetUniformLocation("Diffuse"), dRenderTarget.DiffuseTexture);
            //GL.Uniform1(Shader.GetUniformLocation("Position"), dRenderTarget.PositionTexture);
            //GL.Uniform1(Shader.GetUniformLocation("Normal"), dRenderTarget.NormalTexture);

            GL.Uniform3(Shader.GetUniformLocation("cameraPosition"), Camera.Position);

            GL.Uniform3(Shader.GetUniformLocation("Lights[0].Position"), new Vector3(2, 2, 2));
            GL.Uniform4(Shader.GetUniformLocation("Lights[0].Color"), OpenTK.Graphics.Color4.White);
            /*amount = 0.2f;
            if (Pass == 0)
                GL.Uniform2(Shader.GetUniformLocation("uShift"), new Vector2(amount / (float)Width, 0));

            if (Pass == 1)
                GL.Uniform2(Shader.GetUniformLocation("uShift"), new Vector2(0, amount / (float)Height));

            if (Pass == 2)
                GL.Uniform2(Shader.GetUniformLocation("uShift"), new Vector2(amount / (float)Width, amount / (float)Height));

            if (Pass == 3)
                GL.Uniform2(Shader.GetUniformLocation("uShift"), new Vector2(-(amount / (float)Width), amount / (float)Height));*/
            
            base.SetUniform(Pass);
        }

        private void PrepareDraw()
        {
            var dRenderTarget = (RenderTargets.DefferedRenderTarget)RenderTarget;
            Texture2D.Bind(dRenderTarget.DiffuseTexture, Texture2D.FilterMode.Clamp, TextureUnit.Texture1, Shader.GetUniformLocation("Diffuse"));
            Texture2D.Bind(dRenderTarget.PositionTexture, Texture2D.FilterMode.Clamp, TextureUnit.Texture1, Shader.GetUniformLocation("Position"));
            Texture2D.Bind(dRenderTarget.NormalTexture, Texture2D.FilterMode.Clamp, TextureUnit.Texture2, Shader.GetUniformLocation("Normal"));
        }

        private void CleanUpDraw()
        {
            var dRenderTarget = (RenderTargets.DefferedRenderTarget)RenderTarget;
            Texture2D.UnBind(dRenderTarget.DiffuseTexture, TextureUnit.Texture1);
            Texture2D.UnBind(dRenderTarget.PositionTexture, TextureUnit.Texture1);
            Texture2D.UnBind(dRenderTarget.NormalTexture, TextureUnit.Texture2);
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
