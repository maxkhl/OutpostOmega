using System;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using System.IO;

namespace OutpostOmega.Drawing.UI
{
    class LoadingScreen : DockBase
    {
        /// <summary>
        /// Currently active loading screen
        /// </summary>
        public static LoadingScreen Active { get; set; }

        public static LoadingScreen Start(Scene Scene, Base parent, string InitMessage = "Loading...")
        {
            if (Active == null)
                Active = new LoadingScreen(Scene, parent, InitMessage);
            else
                Active.ProgressText.Text = InitMessage;

            return Active;
        }

        public static void Stop()
        {
            if (Active != null)
                Active.Parent.RemoveChild(Active, true);
        }

        Scene Scene;
        public Label ProgressText;
        Label ProgressText2;
        Label ProgressText3;
        Label ProgressText4;
        AnimatedTexture2D Animation;
        ImagePanel ImgPanel;
        ImagePanel Background;
        public LoadingScreen(Scene Scene, Base parent, string InitMessage = "Loading...")
            : base(parent)
        {
            this.Scene = Scene;
            this.Dock = Pos.Fill;
            this.Width = parent.Width;
            this.Height = parent.Height;

            Background = new ImagePanel(this);
            Background.Width = this.Width;
            Background.Height = this.Height;
            Background.X = 0;
            Background.Y = 0;
            Background.ImageName = @"Content\Image\TpxBlackFull.png";


            ImgPanel = new ImagePanel(this);

            ImgPanel.Width = 250;
            ImgPanel.Height = 250;

            ImgPanel.X = this.Width / 2 - ImgPanel.Width / 2;
            ImgPanel.Y = this.Height / 2 - ImgPanel.Height / 2;

            Animation = new AnimatedTexture2D(new FileInfo(@"Content\Image\Loading.gif")); //rakete_rotating.gif
            Animation.Play = true;
            Animation.Repeat = true;
            Animation.TargetFPS = 25;

            ImgPanel.ImageHandle = Animation.Handle;

            ProgressText = new Label(this);
            ProgressText.Text = InitMessage;
            Message = InitMessage;

            ProgressText.X = this.Width / 2 - ProgressText.Width / 2;
            ProgressText.Y = this.Height - ( ProgressText.Height * 4 + 25 );
            ProgressText.BringToFront();


            ProgressText2 = new Label(this)
            {
                TextColor = System.Drawing.Color.FromArgb(205, 205, 205),
                X = ProgressText.X,
                Y = ProgressText.Y + ProgressText.Height,
            };
            ProgressText3 = new Label(this)
            {
                TextColor = System.Drawing.Color.FromArgb(155, 155, 155),
                X = ProgressText.X,
                Y = ProgressText.Y + ProgressText.Height * 2,
            };
            ProgressText4 = new Label(this)
            {
                TextColor = System.Drawing.Color.FromArgb(105, 105, 105),
                X = ProgressText.X,
                Y = ProgressText.Y + ProgressText.Height * 3,
            };

            this.BoundsChanged += LoadingScreen_BoundsChanged;

            Parent.MouseInputEnabled = false;
            Parent.KeyboardInputEnabled = false;

            Active = this;
        }

        /// <summary>
        /// Set this value to change the progress text
        /// </summary>
        public string Message { get; set; }

        public override void Think()
        {
            if(this.AbortFlag)
            {
                new MessageBox(this.Parent, AbortMessage, "Error");
                this.Parent.RemoveChild(this, true);
            }


            if (ProgressText.Text != Message)
            {
                ProgressText4.Text = ProgressText3.Text;
                ProgressText3.Text = ProgressText2.Text;
                ProgressText2.Text = ProgressText.Text;
                ProgressText.Text = Message;

                ProgressText.X = this.Width / 2 - ProgressText.Width / 2;
                ProgressText2.X = this.Width / 2 - ProgressText2.Width / 2;
                ProgressText3.X = this.Width / 2 - ProgressText3.Width / 2;
                ProgressText4.X = this.Width / 2 - ProgressText4.Width / 2;
                //ProgressText.Y = this.Height / 2 - ProgressText.Height / 2 + ImgPanel.Height;
            }
            Animation.Update();
            base.Think();
        }

        void LoadingScreen_BoundsChanged(Base sender, EventArgs arguments)
        {
            ImgPanel.X = this.Width / 2 - ImgPanel.Width / 2;
            ImgPanel.Y = this.Height / 2 - ImgPanel.Height / 2;

            ProgressText.X = this.Width / 2 - ProgressText.Width / 2;
            ProgressText.Y = this.Height - (ProgressText.Height * 4 + 25);

            ProgressText2.X = this.Width / 2 - ProgressText2.Width / 2;
            ProgressText2.Y = ProgressText.Y + ProgressText.Height;

            ProgressText3.X = this.Width / 2 - ProgressText3.Width / 2;
            ProgressText3.Y = ProgressText.Y + ProgressText.Height * 2;

            ProgressText4.X = this.Width / 2 - ProgressText4.Width / 2;
            ProgressText4.Y = ProgressText.Y + ProgressText.Height * 3;

            Background.Width = this.Width;
            Background.Height = this.Height;
        }

        private bool AbortFlag = false;
        private string AbortMessage = "";

        /// <summary>
        /// Aborts the loading screen and displays the message as a popup
        /// </summary>
        public void Abort(string Message)
        {
            AbortFlag = true;
            AbortMessage = Message;
        }

        public override void Dispose()
        {
            Parent.MouseInputEnabled = true;
            Parent.KeyboardInputEnabled = true;

            base.Dispose();
            //ImgPanel.Dispose();
            //ProgressText.Dispose();
            //Background.Dispose();
            Animation.Dispose();

            Active = null;
        }
    }
}
