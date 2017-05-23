using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwen;
using Gwen.Control;
using System.Diagnostics;
using OpenTK.Input;

namespace OutpostOmega.Scenes
{
    /// <summary>
    /// Intro Scene
    /// </summary>
    class Intro : Scene
    {
        private Stopwatch _introTimer;

        Drawing.Video TestVideo;
        public Intro(MainGame game)
            : base(game)
        {
            _introTimer = new Stopwatch();

            //TestVideo = new Drawing.Video(new System.IO.FileInfo(@"Content\Video\Wildlife.wmv"));
            //TestVideo.Play();
        }
        ImagePanel _image;
        public override void Initialize()
        {
            if (AppSettings.Default.SkipIntro)
            {
                CloseIntro();
            }
            else
            {
                _image = new ImagePanel(this.Canvas);
                _image.ImageName = @"Content\Image\Intro1.png";

                _image.Width = 969;
                _image.Height = 602;

                _image.X = Game.Width / 2 - _image.Width / 2;
                _image.Y = Game.Height / 2 - _image.Height / 2;

                _introTimer.Start();
            }
            base.Initialize();
        }

        private void CloseIntro()
        {
            int Handle = Game.SceneManager.AddScene(new Menu(Game));
            Game.SceneManager.MakeSceneActive(Handle);
        }

        private int _introStage = 0;

        protected override void UpdateScene()
        {
            if (_introTimer.ElapsedMilliseconds >= 15000 && _introStage == 0)
            {
                _image.ImageName = @"Content\Image\Intro2.png";
                _introStage++;
            }

            if (_introTimer.ElapsedMilliseconds >= 20000)
            {
                _introTimer.Stop();
                CloseIntro();
            }
            if (Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Escape))
                CloseIntro();
            
            /*TestVideo.Update();
            _image.ImageHandle = TestVideo.Handle;

            if (_image.Width != TestVideo.frameWidth)
                _image.Width = TestVideo.frameWidth;
            if (_image.Height != TestVideo.frameHeight)
                _image.Height = TestVideo.frameHeight;


            _image.X = Game.Width / 2 - _image.Width / 2;
            _image.Y = Game.Height / 2 - _image.Height / 2;

            if (TestVideo.End)
                CloseIntro();*/
            base.UpdateScene();
        }
    }
}
