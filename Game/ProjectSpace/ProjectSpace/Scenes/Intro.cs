using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwen;
using Gwen.Control;
using System.Diagnostics;
using OpenTK.Input;
using System.IO;

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
        ImagePanel LogoImage;
        ImagePanel LoadingImage;

        TextBox MessageBox;

        OutpostOmega.Game.Tools.Animation testAnimation;
        public override void Initialize()
        {
            base.Initialize();


            if (AppSettings.Default.SkipIntro)
            {
                CloseIntro();
            }
            else
            {



                LoadingImage = new ImagePanel(this.Canvas);
                LoadingImage.ImageName = @"Content\Image\MinimalisticLogo.png";

                LoadingImage.Width = 499;
                LoadingImage.Height = 57;

                LoadingImage.X = Game.Width / 2 - LoadingImage.Width / 2;
                LoadingImage.Y = -LoadingImage.TextureHeight;
                LoadingImage.Animate(
                    "Y", 
                    LoadingImage.TextureHeight + 20, 
                    1000, 
                    OutpostOmega.Game.Tools.Easing.EaseFunction.CircEaseOut,
                    delegate(OutpostOmega.Game.Tools.Animation sender)
                    {
                        MessageBox.Animate(
                            "Y",
                            0, 
                            2000, 
                            OutpostOmega.Game.Tools.Easing.EaseFunction.CubicEaseOut);
                    });

                MessageBox = new TextBox(this.Canvas);
                MessageBox.Margin = new Margin(20, 20, 20, 20);
                MessageBox.Width = this.Game.Width;
                MessageBox.Height = this.Game.Height;
                MessageBox.X = 0;
                MessageBox.Y = this.Game.Height;
                //MessageBox.Dock = Pos.Fill;
                MessageBox.Font = new Font(this.renderer, "Arial", 15);
                MessageBox.Alignment = Pos.Center;
                MessageBox.TextColor = System.Drawing.Color.FromArgb(143, 143, 143); //Gray
                MessageBox.ShouldDrawBackground = false;

                AddTextLine("Please keep in mind that this is a VERY early version.");
                AddTextLine("Feel free to contact me at maxkhl@outpost-omega.com");
                AddTextLine("");
                AddTextLine("Wait or press <ESCAPE> to continue");



                

                /*LogoImage.X = Game.Width / 2 - LogoImage.Width / 2;
                LogoImage.Y = 20;*/

                _introTimer.Start();
            }
        }

        public void AddTextLine(string Line)
        {
            MessageBox.Text += Line + "\n";
        }

        private void CloseIntro()
        {
            int Handle = Game.SceneManager.AddScene(new Menu(Game));
            Game.SceneManager.MakeSceneActive(Handle);
        }

        private int _introStage = 0;

        protected override void UpdateScene()
        {

            /*if (_introTimer.ElapsedMilliseconds >= 15000 && _introStage == 0)
            {
                //LogoImage.ImageName = @"Content\Image\Intro2.png";
                _introStage++;
            }*/

            /*if (_introTimer.ElapsedMilliseconds >= 20000)
            {
                _introTimer.Stop();
                CloseIntro();
            }*/
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
