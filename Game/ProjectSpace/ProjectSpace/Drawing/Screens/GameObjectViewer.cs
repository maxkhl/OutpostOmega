using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Drawing.Screens
{
    /// <summary>
    /// Used as a basic GameObject viewer. No fancy effects.
    /// </summary>
    class GameObjectViewer : Screen
    {
        /// <summary>
        /// The GameObject that should be shown
        /// </summary>
        public OutpostOmega.Game.GameObject GameObject
        {
            get
            {
                return _GameObject;
            }
            set
            {
                if (_GameObject != null)
                    _GameObject.Dispose();

                _GameObject = value;
                if (value != null)
                {
                    if (value.Meshs.Count > 0)
                    {
                        _GameObjectDrawer = new Game.GameObjectDrawer(value);
                        _GameObjectDrawer.NoTranslation = true; //Keep it at 0 0 0
                        _GameObjectDrawer.ForceDraw = true;

                        float size = 5;
                        if (_GameObject.RigidBody != null)
                            size = (_GameObject.RigidBody.BoundingBox.Max - _GameObject.RigidBody.BoundingBox.Min).Length();
                        _OrbitCam.radius = size + size * 0.1f; //Size + 10%
                    }
                }
            }
        }

        private OutpostOmega.Game.GameObject _GameObject;
        private Game.GameObjectDrawer _GameObjectDrawer;

        private Drawing.RenderOptions _renderOptions;

        private View.OrbitCamera _OrbitCam; //Avoiding excessive typecasting

        public GameObjectViewer(Scenes.Game GameScene)
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

            GameScene.Game.FocusedChanged += Game_FocusedChanged;
        }

        void Game_FocusedChanged(object sender, EventArgs e)
        {

        }

        protected override void DrawScene()
        {
            if(_GameObjectDrawer != null)
            {
                _GameObjectDrawer.Draw(this._renderOptions);
            }
        }

        public override void Update(double ElapsedTime)
        {
            if (_GameObjectDrawer != null)
            {
                _OrbitCam.AddRotation(0.005f, 0.005f); //Rotate slowly
                _GameObjectDrawer.Update();
            }
            base.Update(ElapsedTime);
        }
    }
}
