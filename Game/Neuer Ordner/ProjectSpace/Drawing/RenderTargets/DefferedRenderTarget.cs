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
    /// <summary>
    /// Similar to RenderTarget but with additional normal, diffuse and position Rendertargets for deffered rendering
    /// </summary>
    class DefferedRenderTarget : RenderTarget
    {
        private uint DiffuseRB;
        public int DiffuseTexture { get; protected set; }

        private uint NormalRB;
        public int NormalTexture { get; protected set; }

        private uint PositionRB;
        public int PositionTexture { get; protected set; }

        private uint DepthRB;
        public int DepthTexture { get; protected set; }

        public DefferedRenderTarget(int Width, int Height) : base(Width, Height)
        {
            BufferTargets.Add(DrawBuffersEnum.ColorAttachment1);
            BufferTargets.Add(DrawBuffersEnum.ColorAttachment2);
        }

        protected override void Load(bool Refresh = true)
        {
            // Create RenderBuffers
            GL.Ext.GenRenderbuffers(1, out DiffuseRB);
            GL.Ext.GenRenderbuffers(1, out NormalRB);
            GL.Ext.GenRenderbuffers(1, out PositionRB);
            GL.Ext.GenRenderbuffers(1, out DepthRB);

            // Create Diffuse Texture
            if (Refresh)
                GL.DeleteTexture(DiffuseTexture);

            int TexHandle;
            GL.GenTextures(1, out TexHandle);
            DiffuseTexture = TexHandle;

            GL.BindTexture(TextureTarget.Texture2D, DiffuseTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, this.Width, this.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);


            // Create Position Texture
            if (Refresh)
                GL.DeleteTexture(PositionTexture);

            GL.GenTextures(1, out TexHandle);
            PositionTexture = TexHandle;

            GL.BindTexture(TextureTarget.Texture2D, PositionTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, this.Width, this.Height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);


            // Create Normal Texture
            if (Refresh)
                GL.DeleteTexture(NormalTexture);

            GL.GenTextures(1, out TexHandle);
            NormalTexture = TexHandle;

            GL.BindTexture(TextureTarget.Texture2D, NormalTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, this.Width, this.Height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);



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


            OutTexture = DiffuseTexture;


            base.Load(Refresh);
        }

        public override void BindRenderBuffers(uint FBOHandle)
        {

            // Bind the diffuse render target
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, DiffuseRB);
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.Rgba16, this.Width, this.Height);
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, RenderbufferTarget.RenderbufferExt, DiffuseRB);

            // Bind the position render target
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, PositionRB);
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.Rgba32f, this.Width, this.Height);
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment1Ext, RenderbufferTarget.RenderbufferExt, PositionRB);

            // Bind the normal render target
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, NormalRB);
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.Rgba16f, this.Width, this.Height);
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment2Ext, RenderbufferTarget.RenderbufferExt, NormalRB);

            // Bind the depth buffer
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, DepthRB);
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, RenderbufferStorage.DepthComponent24, this.Width, this.Height);
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, DepthRB);


            // Bind Diffuse Texture
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, DiffuseTexture, 0);
            
            // Bind Position Texture
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment1Ext, TextureTarget.Texture2D, PositionTexture, 0);

            // Bind Normal Texture
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment2Ext, TextureTarget.Texture2D, NormalTexture, 0);

            // Bind Depth Texture
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, DepthTexture, 0);

            base.BindRenderBuffers(FBOHandle);
        }

        public override void Dispose()
        {
            GL.DeleteTexture(DiffuseTexture);
            GL.DeleteTexture(PositionTexture);
            GL.DeleteTexture(NormalTexture);
            GL.DeleteTexture(DepthTexture);

            GL.Ext.DeleteRenderbuffer(DiffuseRB);
            GL.Ext.DeleteRenderbuffer(PositionRB);
            GL.Ext.DeleteRenderbuffer(NormalRB);
            GL.Ext.DeleteRenderbuffer(DepthRB);

            base.Dispose();
        }
    }
}
