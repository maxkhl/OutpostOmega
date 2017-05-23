using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// Animated Version of the 2d texture (supports only gif)
    /// </summary>
    class AnimatedTexture2D : Texture2D
    {
        private string[] SupportedFiles = new string[1] { "gif" };

        public AnimatedTexture2D(FileInfo file)
            : base(file)
        {
            if (!SupportedFiles.Contains(file.Name.Split('.')[1]))
                throw new Exception("Fileformat not supported for animations '" + file.FullName + "'");

            TargetFPS = 30;

        }

        Bitmap[] Frames;

        protected override int Load(string FilePath)
        {
            if (String.IsNullOrEmpty(FilePath))
                throw new ArgumentException(FilePath);

            // GL Textur vorbereiten
            //Handle = GL.GenTexture();
            

            Bitmap source = new Bitmap(FilePath);
            Frames = ParseFrames(source);
            return base.Load(FilePath);
        }

        public bool Play { get; set; }
        public bool Stop { get; set; }
        public bool Repeat { get; set; }
        public int TargetFPS { get; set; }

        public int CurrentFrame 
        { 
            get
            {
                return _CurrentFrame;
            }
            set
            {
                if (value >= Frames.Length)
                {
                    if (Repeat)
                    {
                        Play = true;
                        _CurrentFrame = 0;
                    }
                    else
                        Play = false;
                }
                else
                    _CurrentFrame = value;
            }
        }
        private int _CurrentFrame = 0;

        private Stopwatch FrameTime = new Stopwatch();

        protected override void PrepareBind()
        {
            Update();
            base.PrepareBind();
        }

        public void Update()
        {
            if (Disposing)
                return;

            FrameTime.Stop();

            if (Play)
            {
                // Next frame if we reached the target frametime
                if (FrameTime.ElapsedMilliseconds > 1000 / TargetFPS)
                {
                    CurrentFrame++;
                    LoadFrame(Frames[CurrentFrame]);
                    FrameTime.Reset();
                }
            }

            FrameTime.Start();
        }

        public void LoadFrame(Bitmap frame)
        {
            int currentTexture = 0;
            GL.GetInteger(GetPName.TextureBinding2D, out currentTexture);

            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            BitmapData bmp_data = frame.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            frame.UnlockBits(bmp_data);

            GL.BindTexture(TextureTarget.Texture2D, currentTexture);

            this.Source = frame;
        }

        private Bitmap[] ParseFrames(Bitmap Animation)
        {
            // Get the number of animation frames to copy into a Bitmap array

            int Length = Animation.GetFrameCount(FrameDimension.Time);

            // Allocate a Bitmap array to hold individual frames from the animation

            Bitmap[] Frames = new Bitmap[Length];

            // Copy the animation Bitmap frames into the Bitmap array

            for (int Index = 0; Index < Length; Index++)
            {
                // Set the current frame within the animation to be copied into the Bitmap array element

                Animation.SelectActiveFrame(FrameDimension.Time, Index);

                // Create a new Bitmap element within the Bitmap array in which to copy the next frame

                Frames[Index] = new Bitmap(Animation.Size.Width, Animation.Size.Height);

                // Copy the current animation frame into the new Bitmap array element

                Graphics.FromImage(Frames[Index]).DrawImage(Animation, 0,0);
            }

            // Return the array of Bitmap frames

            return Frames;
        }

        public void Reset()
        { CurrentFrame = 0; }

        public override void Dispose()
        {
            
            base.Dispose();
        }
    }
}
