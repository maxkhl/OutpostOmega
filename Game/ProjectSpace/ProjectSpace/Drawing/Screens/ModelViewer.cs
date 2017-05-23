using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using grendgine_collada;
using OutpostOmega.Game.Content;

namespace OutpostOmega.Drawing.Screens
{
    /// <summary>
    /// Used as a basic GameObject viewer. No fancy effects.
    /// </summary>
    class ModelViewer : Screen
    {
        /// <summary>
        /// Assigend meshes
        /// </summary>
        public List<Drawing.Mesh> Meshs { get; set; }

        private Drawing.RenderOptions _renderOptions;

        private View.OrbitCamera _OrbitCam; //Avoiding excessive typecasting

        public ModelViewer(Model Model, Scenes.Game GameScene)
            : base(GameScene, 1, 1)
        {
            _OrbitCam = new View.OrbitCamera(this);
            this.Camera = _OrbitCam;

            this._renderOptions = new RenderOptions()
            {
                Wireframe = false,
                Shader = Drawing.Shader.Load(
                    new System.IO.FileInfo(@"Content\Shader\Simple\Simple_VS.glsl"),
                    new System.IO.FileInfo(@"Content\Shader\Simple\Simple_FS.glsl")),
                Color = OpenTK.Graphics.Color4.White,
            };


            var meshs = Tools.Collada.ReadModel(Model.Path);

            foreach (Tools.Collada.Mesh mesh in meshs)
            {
                var modelMesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, mesh.Vertices, mesh.Indices[Grendgine_Collada_Input_Semantic.VERTEX]);
                
                Texture2D texture;
                /*if (Model.GetTexture(mesh.Name) != null)
                {
                    if (Model.GetTexture(mesh.Name).Data == null)
                        Model.GetTexture(mesh.Name).Data = new Texture2D(Model.GetTexture(mesh.Name).FileInfo);

                    texture = (Texture2D)Model.GetTexture(mesh.Name).Data;
                }
                else
                {
                    var tfile = new System.IO.FileInfo(Model.FileInfo.Directory.FullName + "\\" + Model.FileInfo.Name.Split('.')[0] + ".png");
                    texture = new Texture2D(tfile);
                }*/

                /*modelMesh.Textures.Add(OpenTK.Graphics.OpenGL.TextureUnit.Texture0, texture);

                modelMesh.Shader = Shader;
                Meshs.Add(modelMesh);*/
            }

            GameScene.Game.FocusedChanged += Game_FocusedChanged;
        }

        void Game_FocusedChanged(object sender, EventArgs e)
        {

        }

        protected override void DrawScene()
        {
            foreach (Drawing.Mesh mesh in Meshs)
                mesh.Draw(_renderOptions);
        }

        public override void Update(double ElapsedTime)
        {
            _OrbitCam.AddRotation(0.1f, 0); //Rotate slowly
            base.Update(ElapsedTime);
        }
    }
}
