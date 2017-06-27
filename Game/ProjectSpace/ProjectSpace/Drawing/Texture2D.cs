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

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// A 2-dimensional texture that can be used in opengl
    /// </summary>
    class Texture2D : IDisposable
    {
        public int Handle { get; set; }
        public Bitmap Source { get; set; }
        public FileInfo File { get; set; }

        /// <summary>
        /// This shit is just not working and I'm sick of looking at it
        /// </summary>
        public Matrix4 TextureMatrix 
        { 
            get
            {
                return _TextureMatrix;
            }
            set
            {
                _TextureMatrix = value;
            }
        }
        private Matrix4 _TextureMatrix = Matrix4.Identity;

        public enum FilterMode
        {
            Clamp,
            Repeat,
            None
        }

        /// <summary>
        /// Specify the filter mode of this texture
        /// </summary>
        public FilterMode filterMode
        {
            get
            {
                return _filterMode; 
            }
            set
            {
                _filterMode = value;
            }
        }
        private FilterMode _filterMode = FilterMode.Repeat;

        protected Texture2D()
        {

        }

        public Texture2D(FileInfo File)
        {
            string filename = File.FullName;
            this.Handle = Load(filename);
            this.File = File;
        }

        public Texture2D(Bitmap bitmap)
        {
            this.Handle = Load(bitmap);
            this.File = null;
        }

        /// <summary>
        /// Imports a image from the harddrive
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        /// <returns>OpenGL Image Pointer</returns>
        protected virtual int Load(string FilePath)
        {
            if (String.IsNullOrEmpty(FilePath))
                throw new ArgumentException(FilePath);

            Bitmap bmp = new Bitmap(FilePath);
            return Load(bmp);
        }

        /// <summary>
        /// Imports a bitmap to OpenGL
        /// </summary>
        /// <returns>OpenGL Image Pointer</returns>
        protected virtual int Load(Bitmap bmp)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapNearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
            
            GL.GetInteger((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out int maxAniso);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);

            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            this.Source = bmp;


            return id;
        }

        /// <summary>
        /// Can be used to prepare the binding of the texture
        /// </summary>
        protected virtual void PrepareBind()
        {

        }
        
        /// <summary>
        /// Binds the texture to a specified target (shader variable)
        /// </summary>
        public void Bind(TextureUnit Unit, int Target)
        {
            if (Target == -1)
                return;

            PrepareBind();

            Bind(Handle, filterMode, Unit, Target);
        }

        public static void Bind(int TextureHandle, FilterMode filterMode, TextureUnit Unit, int Target)
        {
            if (Target == -1)
                return;

            //PrepareBind();

            // NOT WORKING
            /*if (_TextureMatrix != Matrix4.Identity)
            {
                GL.GetFloat(GetPName.TextureMatrix, out PushMatrix);
                GL.MatrixMode(MatrixMode.Texture);
                GL.LoadMatrix(ref _TextureMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
            }*/

            GL.ActiveTexture(Unit);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.Uniform1(Target, Unit - TextureUnit.Texture0);
            switch (filterMode)
            {
                case FilterMode.Clamp:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Clamp);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Clamp);
                    break;
                case FilterMode.Repeat:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);
                    break;
                case FilterMode.None:
                    // Do nothing
                    break;
                default:
                    throw new Exception("How is this even possible???");
            }
        }
        
        
        /// <summary>
        /// Binds the texture to a specified target (shader variable)
        /// </summary>
        public void UnBind(TextureUnit Unit)
        {
            UnBind(Handle, Unit);
        }
        public static void UnBind(int Handle, TextureUnit Unit)
        {
            /*if (TextureMatrix != Matrix4.Identity)
            {
                GL.MatrixMode(MatrixMode.Texture);
                GL.LoadMatrix(ref PushMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
            }*/

            GL.ActiveTexture(Unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public bool Disposing { get; set; }
        public virtual void Dispose()
        {
            Disposing = true;
            GL.DeleteTexture(Handle);
            if(Source != null)
            {
                Source.Dispose();
                Source = null;
            }
        }
    }
}
