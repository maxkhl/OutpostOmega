using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OutpostOmega.Game.turf;

namespace OutpostOmega.Drawing.Screens
{
    /// <summary>
    /// Default - human eye - screen
    /// </summary>
    class Atmospherics : Screen
    {
        private Drawing.RenderOptions renderOptions;

        public Atmospherics(Scenes.Game GameScene)
            : base(GameScene, GameScene.Game.Width, GameScene.Game.Height)
        {
            this.Camera = new View.GOCamera(GameScene.World.Player.Mob.View, this);

            renderOptions = new RenderOptions()
            {
                Wireframe = false,
                Color = OpenTK.Graphics.Color4.White,
            };

            GameScene.Game.FocusedChanged += Game_FocusedChanged;
        }

        void Game_FocusedChanged(object sender, EventArgs e)
        {
            /*if (Game.Focused)
            {
                Camera.ResetCursor(Game);
            }*/
        }

        protected override void ViewChanged(object sender, EventArgs e)
        {
            //Re-scale the target to the screen resolution
            if (this.RenderTarget.Width != GameScene.Game.Width)
                this.RenderTarget.Width = GameScene.Game.Width;

            if (this.RenderTarget.Height != GameScene.Game.Height)
                this.RenderTarget.Height = GameScene.Game.Height;
        }

        protected override void DrawScene()
        {
            GameScene.Drawer.Draw(renderOptions);

            // Thats the whole trick here. We render all atmospherics shit in immediate mode
            for(int i = 0; i < GameScene.Drawer.Chunks.Count; i++)
            {
                // Render the chunk in special condition (only when pressure is higher than 0
                var jitterVertices = GameScene.Drawer.Chunks[i].SourceChunk.Render(
                    delegate( Block turf) { return Block.Pressure(turf) > 0; }, false);

                // Convert vertex data to opengl
                var vertices = Tools.Convert.Mesh.Vertex.Jitter_To_OpenGL(jitterVertices);

                // Draw the mesh in immediate mode
                Drawing.Mesh.DrawImmediate(
                    new RenderOptions() {
                        SetUniform = delegate(Drawing.Shader shader) {

                        },
                        Color = OpenTK.Graphics.Color4.White,
                    },
                    vertices);
            }
        }
    }
}
