using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutpostOmega.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using DragonOgg.Interactive;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace OutpostOmega.Scenes
{
    /// <summary>
    /// Main Menu Scene
    /// </summary>
    class Menu : Scene
    {
        /// <summary>
        /// Main Menu GWEN Container
        /// </summary>
        public Drawing.UI.MainMenu mainMenu { get; set; }

        public RenderTarget renderTarget { get; set; }

        public Mesh[] Meshs { get; set; }

        private View.OrbitCamera _OrbitCam;

        private RenderOptions _renderOptions { get; set; }
        
        private Drawing.PPShader PostProcessShader;

        private Drawing.Skybox Skybox;

        //public Drawing.Screens.GameObjectViewer

        public Menu(MainGame game)
            : base(game)
        {
            
        }

        public override void Initialize()
        {

            if(AppSettings.Default.AutoStart)
            {
                OutpostOmega.Game.World world = null;

                world = OutpostOmega.Game.World.CreateTest();
                world.MakePlayer();
                //}

                int Handle = Game.SceneManager.AddScene(new Game(Game, world));
                var bleh = world.GetDebugString();
                Game.SceneManager.MakeSceneActive(Handle);
            }

            /*AudioContext ac = new AudioContext();
            XRamExtension xram = new XRamExtension();

            string spessAmbientSoundFile = @"Content\Sound\Ambient\Tristan Lohengrin - Vaisseau Alien.ogg";
            AudioClip spessAmbientSound = new AudioClip(spessAmbientSoundFile);*/

            //AL.Source(spessAmbientSound)
            //spessAmbientSound.Play();

            PostProcessShader = new PPShader(new System.IO.FileInfo(@"Content\Shader\Default\Default_PPS.glsl"));
            PostProcessShader.PassCount = 4;
            //GL.Uniform2(PostProcessShader.GetUniformLocation("uShift"), new Vector2(0, 0));

            renderTarget = new Drawing.RenderTargets.SimpleRenderTarget(Game.Width, Game.Height);
            mainMenu = new Drawing.UI.MainMenu(this, Canvas);

            _OrbitCam = new View.OrbitCamera(renderTarget);
            _OrbitCam.radius = 20;
            _OrbitCam.AddRotation(0, 0);

            var Shader = Drawing.Shader.Load(
                    new System.IO.FileInfo(@"Content\Shader\Default\Default_VS.glsl"),
                    new System.IO.FileInfo(@"Content\Shader\Default\Default_FS.glsl"));            

            _renderOptions = new RenderOptions()
            {
                Shader = Shader
            };

            Skybox = new Drawing.Skybox("Space02", "jpg");

            var ModelPath = @"Content\Model\Other\Station.dae";
            var cMeshs = Tools.Collada.ReadModel(ModelPath); //Content\Model\Other\Station.dae
            //cMeshs.AddRange(Tools.Collada.ReadModel(@"Content\Model\Other\Skysphere.dae"));

            var meshs = new List<Mesh>();
            foreach(Tools.Collada.Mesh cmesh in cMeshs)
            {
                var mesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, cmesh.Vertices, cmesh.Indices[grendgine_collada.Grendgine_Collada_Input_Semantic.VERTEX]);

                var ModelFI = new System.IO.FileInfo(ModelPath);
                var tfile = new System.IO.FileInfo(ModelFI.Directory.FullName + "\\" + ModelFI.Name.Split('.')[0] + ".png");
                mesh.Textures.Add(TextureUnit.Texture0, new KeyValuePair<string, Texture2D>("colorMap", new Texture2D(tfile))); // TODO - support multiple textures
                meshs.Add(mesh);
            }
            Meshs = meshs.ToArray();

            foreach (Mesh mesh in Meshs)
            {
                mesh.Translation = Matrix4.CreateRotationY((float)OutpostOmega.Game.Tools.MathHelper.DegreeToRadian(-180)) * Matrix4.CreateTranslation(0, -5, 0);
            }

            base.Initialize();
        }

        protected override void RefreshSceneView()
        {
            if(renderTarget != null)
            {
                renderTarget.Width = Game.Width;
                renderTarget.Height = Game.Height;
            }

            base.RefreshSceneView();
        }

        protected override void DrawSceneFree()
        {
            if (renderTarget != null)
            {
                //Set camera
                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 viewProj = _OrbitCam.ViewProjectionMatrix;
                GL.LoadMatrix(ref viewProj);

                renderTarget.Start();
                foreach (Mesh mesh in Meshs)
                {
                    mesh.Draw(_renderOptions);
                }
                Skybox.Draw();
                renderTarget.End();
            }
            base.DrawSceneFree();
        }

        protected override void DrawSceneOrtho()
        {
            if (renderTarget != null)
            {
                PostProcessShader.Bind();
                for (int i = 0; i < PostProcessShader.PassCount; i++)
                {
                    float amount = 0.15f;
                    if (i == 0)
                        GL.Uniform2(PostProcessShader.GetUniformLocation("uShift"), new Vector2(amount / (float)Game.Width, 0));

                    if (i == 1)
                        GL.Uniform2(PostProcessShader.GetUniformLocation("uShift"), new Vector2(0, amount / (float)Game.Height));

                    if (i == 2)
                        GL.Uniform2(PostProcessShader.GetUniformLocation("uShift"), new Vector2(amount / (float)Game.Width, amount / (float)Game.Height));

                    if (i == 3)
                        GL.Uniform2(PostProcessShader.GetUniformLocation("uShift"), new Vector2(-(amount / (float)Game.Width), amount / (float)Game.Height));

                    renderTarget.Draw(0, 0, Game.Width, Game.Height);
                }
                PostProcessShader.UnBind();
            }
            base.DrawSceneOrtho();
        }

        protected override void UpdateScene()
        {
            if (_OrbitCam != null)
            {
                _OrbitCam.AddRotation(0.0005f, 0); //Rotate slowly
                _OrbitCam.Refresh();
            }

            base.UpdateScene();
        }

        public override void Dispose()
        {
            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }
            _OrbitCam = null;
            
            if (PostProcessShader != null)
                PostProcessShader.Dispose();

            if(Meshs != null)
                foreach (Mesh mesh in Meshs)
                {
                    mesh.Dispose();
                }
            base.Dispose();
        }
    }
}
