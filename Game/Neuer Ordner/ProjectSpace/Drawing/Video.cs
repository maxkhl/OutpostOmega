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
using DirectShowLib;
using System.Runtime.InteropServices;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// A 2-dimensional texture that can be used in opengl
    /// </summary>
    class Video : Texture2D, ISampleGrabberCB
    {
        //private FilterGraph filterGraph;
        #region APIs
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);
        #endregion
                
        public Video(FileInfo File)
            : base(File)
        {
        }

        protected override int Load(string FilePath)
        {            
            int id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);

            // https://www.opengl.org/discussion_boards/showthread.php/173152-Texture-looks-strange
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Tools.OpenGL.CheckError();

            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(Process), this);
            return id;
        }
        public void Play()
        {
            //mediaControl.Run();
        }
        public void Pause()
        {
            //mediaControl.Pause();
        }

        public IMediaControl mediaControl;
        public AMMediaType mediaType;
        public ISampleGrabber sampleGrabber;
        private static void Process(Object Source)
        {
            Video sourceVideo = (Video)Source;
            //filterGraph = new FilterGraph();
            Type comType = Type.GetTypeFromCLSID(new Guid("e436ebb3-524f-11ce-9f53-0020af0ba770"));
            IGraphBuilder graphBuilder = (IGraphBuilder)Activator.CreateInstance(comType);

            comType = Type.GetTypeFromCLSID(new Guid("C1F400A0-3F08-11d3-9F0B-006008039E37"));
            ISampleGrabber sampleGrabber = (ISampleGrabber)Activator.CreateInstance(comType);

            graphBuilder.AddFilter((IBaseFilter)sampleGrabber, "samplegrabber");

            AMMediaType mediaType = new AMMediaType();
            mediaType.majorType = MediaType.Video;
            mediaType.subType = MediaSubType.UYVY;
            mediaType.formatType = FormatType.VideoInfo;
            sampleGrabber.SetMediaType(mediaType);

            //sourceVideo.mediaType = mediaType;

            int hr = graphBuilder.RenderFile(sourceVideo.File.FullName, null);

            IMediaEventEx mediaEvent = (IMediaEventEx)graphBuilder;
            sourceVideo.mediaControl = (IMediaControl)graphBuilder;
            IVideoWindow videoWindow = (IVideoWindow)graphBuilder;
            IBasicAudio basicAudio = (IBasicAudio)graphBuilder;

            sourceVideo.sampleGrabber = sampleGrabber;
            
            videoWindow.put_AutoShow(OABool.False);
            basicAudio.put_Volume(-10000);

            sampleGrabber.SetOneShot(false);
            sampleGrabber.SetBufferSamples(true);

            //the same object has implemented the ISampleGrabberCB interface.
            //0 sets the callback to the ISampleGrabberCB::SampleCB() method.
            sampleGrabber.SetCallback(sourceVideo, 0);

            sourceVideo.mediaControl.Run();

            EventCode eventCode;
            mediaEvent.WaitForCompletion(-1, out eventCode);

            Marshal.ReleaseComObject(sampleGrabber);
            Marshal.ReleaseComObject(graphBuilder);
            sourceVideo.End = true;
        }

        public bool End = false;

        IntPtr ImageBuffer = IntPtr.Zero;
        public int frameWidth;
        public int frameHeight;
        int stride;
        public int SampleCB(double sampleTime, IMediaSample mediaSample)
        {
            int hr;
            IntPtr buffer;
            AMMediaType nmediaType;
            VideoInfoHeader videoInfo;
            int bufferLength;

            hr = mediaSample.GetMediaType(out nmediaType);
            DsError.ThrowExceptionForHR(hr);

            long start = 0;
            long end = 0;

            int length = mediaSample.GetActualDataLength();
            int bleh = mediaSample.GetTime(out start, out end);

            int dlength = mediaSample.GetActualDataLength();

            if (nmediaType != null)
                this.mediaType = nmediaType;

            if (mediaType == null)
            {
                mediaType = new AMMediaType();
                hr = sampleGrabber.GetConnectedMediaType(mediaType);
                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);
            }

            hr = mediaSample.GetPointer(out buffer);
            DsError.ThrowExceptionForHR(hr);

            bufferLength = mediaSample.GetSize();
            videoInfo = new VideoInfoHeader();
            Marshal.PtrToStructure(mediaType.formatPtr, videoInfo);

            frameWidth = videoInfo.BmiHeader.Width;
            frameHeight = videoInfo.BmiHeader.Height;
            stride = frameWidth * (videoInfo.BmiHeader.BitCount / 8);

            //CopyMemory(ImageBuffer, buffer, bufferLength);
            if (ImageBuffer == IntPtr.Zero)
                ImageBuffer = Marshal.AllocHGlobal(bufferLength);

            byte[] Data = new byte[bufferLength];
            //Marshal.Copy(Data, 0, buffer, bufferLength);
            Marshal.Copy(buffer, Data, 0, bufferLength);
            Marshal.Copy(Data, 0, ImageBuffer, bufferLength);

            //var bitmapOfFrame = new Bitmap(new MemoryStream(Data));


            Marshal.ReleaseComObject(mediaSample);

            return 0;

            /*Console.WriteLine("SampleCB Callback");
            Console.WriteLine(mediaSample.IsSyncPoint() + " " + mediaSample.GetActualDataLength());
            //check if its a keyframe using mediaSample.IsSyncPoint()
            //and convert the buffer into image and save it.
            return 0;*/
        }
        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            Handle = pBuffer.ToInt32();
            return 0;
        }

        public void Update()
        {
            if (ImageBuffer != IntPtr.Zero)
            {
                Bitmap bitmapOfFrame = new Bitmap(frameWidth, frameHeight, stride, System.Drawing.Imaging.PixelFormat.Format16bppRgb565, ImageBuffer);

                Tools.OpenGL.CheckError();
                GL.BindTexture(TextureTarget.Texture2D, Handle);
                BitmapData bmp_data = bitmapOfFrame.LockBits(new Rectangle(0, 0, bitmapOfFrame.Width, bitmapOfFrame.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);


                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb5A1, frameWidth, frameHeight, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedShort565, bmp_data.Scan0);

                Tools.OpenGL.CheckError();

                bitmapOfFrame.UnlockBits(bmp_data);
            }

            //Handle = bvp;
            //Bitmap image = new Bitmap(480, 320, 480 * (24 / 8), System.Drawing.Imaging.PixelFormat.Format24bppRgb, bvp);
        }

        protected override void PrepareBind()
        {
            Update();
            base.PrepareBind();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
