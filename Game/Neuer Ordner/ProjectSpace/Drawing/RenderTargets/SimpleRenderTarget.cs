using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing.RenderTargets
{
    class SimpleRenderTarget : RenderTarget
    {
        public int ColorTexture { get; protected set; }

        private uint DepthRB;
        public int DepthTexture { get; protected set; }


        public SimpleRenderTarget(int Width, int Height)
            : base(Width, Height)
        {

        }

        protected override void Load(bool Refresh = true)
        {

            // Color Texture
            if (Refresh)
                GL.DeleteTexture(ColorTexture);

            int Tex;
            GL.GenTextures(1, out Tex);
            ColorTexture = Tex;
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);

            /*GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, this.Width, this.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);*/
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, this.Width, this.Height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            
            // This is the final output
            this.OutTexture = ColorTexture;


            // Depth Texture
            if (Refresh)
                GL.DeleteTexture(DepthTexture);

            int Depth;
            GL.GenTextures(1, out Depth);
            DepthTexture = Depth;
            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, this.Width, this.Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            
            GL.Ext.GenRenderbuffers(1, out DepthRB);

            base.Load(Refresh);
        }
        public override void BindRenderBuffers(uint FBOHandle)
        {
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, DepthRB);
            GL.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.DepthComponent24, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, DepthRB);

            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);

            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, DepthTexture, 0);

            base.BindRenderBuffers(FBOHandle);
        }

        public override void Dispose()
        {
            GL.DeleteTexture(ColorTexture);
            GL.DeleteTexture(DepthTexture);
            base.Dispose();
        }

    }
}
