using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing
{
    abstract class RenderTarget : IDisposable
    {
        private uint FBOHandle;

        /// <summary>
        /// Final Texture that will be drawn to the screen
        /// </summary>
        public int OutTexture { get; protected set; }

        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                if (value <= 0)
                    _Width = 1;
                else
                    _Width = value;

                Load();
            }

        }
        private int _Width = 512;

        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                if (value <= 0)
                    _Height = 1;
                else
                    _Height = value;
                Load();
            }

        }
        private int _Height = 512;

        /// <summary>
        /// CullFace-mode enabled while drawing?
        /// </summary>
        public bool CullFace { get; set; }
        private bool PushCullFace;

        /// <summary>
        /// Depht Buffer enabled while drawing?
        /// </summary>
        public bool DephtBuffer { get; set; }
        private bool PushDephtBuffer;

        private Matrix4 PushMatrix;

        public RenderTexture Texture { get; protected set; }

        protected HashSet<DrawBuffersEnum> BufferTargets;

        public RenderTarget(int Width = 512, int Height = 512)
        {
            BufferTargets = new HashSet<DrawBuffersEnum>();
            BufferTargets.Add(DrawBuffersEnum.ColorAttachment0);

            CullFace = true;
            DephtBuffer = true;

            _Width = Width;
            _Height = Height;

            Load(false);

            Texture = new RenderTexture(this);
        }

        protected virtual void Load(bool Refresh = true)
        {
            if (Refresh)
                GL.DeleteFramebuffer(FBOHandle);

            // Create FBO
            GL.Ext.GenFramebuffers(1, out FBOHandle);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);

            BindRenderBuffers(FBOHandle);

            // Check for FBO specific errors
            Tools.OpenGL.CheckFboError();

            // Release FBO
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Check for general opengl errors
            Tools.OpenGL.CheckError("FBO Generation");
        }

        public virtual void BindRenderBuffers(uint FBOHandle)
        {
            
        }

        /// <summary>
        /// Start drawing into this render target
        /// </summary>
        public void Start()
        {

            GL.GetFloat(GetPName.ProjectionMatrix, out PushMatrix);
            GL.GetBoolean(GetPName.CullFace, out PushCullFace);
            GL.GetBoolean(GetPName.DepthTest, out PushDephtBuffer);

            // Bind FBO.
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FBOHandle);

            Tools.OpenGL.CheckFboError();

            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Disable(EnableCap.CullFace);
            GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters
            GL.Viewport(0, 0, Width, Height);

            if (DephtBuffer)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);

            

            if (CullFace)
                GL.Enable(EnableCap.CullFace);
            else
                GL.Disable(EnableCap.CullFace);

            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.One);

            // Set default values and enable/disable caps.


            //GL.ShadeModel(ShadingModel.Smooth);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.AlphaFunc(AlphaFunction.Greater, 1.0f);
            //GL.Disable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Multisample);

            if (BufferTargets != null)
                GL.DrawBuffers(BufferTargets.Count, BufferTargets.ToArray());

            Tools.OpenGL.CheckError();
        }

        /// <summary>
        /// Stop drawing into this render target
        /// </summary>
        public void End()
        {
            Tools.OpenGL.CheckError();

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer

            //GL.Disable(EnableCap.AlphaTest);

            GL.PopAttrib(); // restores GL.Viewport() parameters
            //GL.DrawBuffer(DrawBufferMode.Back);

            GL.Disable(EnableCap.Multisample);

            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Color3(0f, 0f, 0f);
            GL.BindTexture(TextureTarget.Texture2D, 0); // bind default texture
            GL.Enable(EnableCap.Texture2D); // enable Texture Mapping


            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref PushMatrix);

            if (PushCullFace)
                GL.Enable(EnableCap.CullFace);
            else
                GL.Disable(EnableCap.CullFace);

            if (PushDephtBuffer)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);
            

            Tools.OpenGL.CheckError();            
        }
        /// <summary>
        /// Draws the FBO in its original size to the target coordinates
        /// </summary>
        public void Draw(float X, float Y)
        {
            DrawTexture(X, Y, this.Width, this.Height);
        }

        /// <summary>
        /// Draws a scaled FBO to the target coordinates
        /// </summary>
        public void Draw(float X, float Y, float Widht, float Height)
        {
            DrawTexture(X, Y, Width, Height);
        }

        private void DrawTexture(float X = 0, float Y = 0, float Widht = 0, float Height = 0)
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            //GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.AlphaFunc(AlphaFunction.Greater, 1.0f);
            //GL.Disable(EnableCap.Blend);
            //GL.Enable(EnableCap.Blend);

            //GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.AlphaFunc(AlphaFunction.Greater, 1.0f);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            //GL.AlphaFunc(AlphaFunction.Greater, 0.95f);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            //byte[] texels = new byte[(int)(Width * Height * 4)];
            //GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, texels);
            GL.PushMatrix();
            {

                if (PrepareDraw != null)
                    PrepareDraw();
                else
                    GL.BindTexture(TextureTarget.Texture2D, OutTexture);

                GL.Begin(PrimitiveType.Quads);
                {
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex2(X, Y + Height);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex2(X + Width, Y + Height);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex2(X + Width, Y);
                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex2(X, Y);
                }
                GL.End();
            }
            GL.PopMatrix();
            if (CleanUpDraw != null)
                CleanUpDraw();
            GL.Enable(EnableCap.Blend);
            Tools.OpenGL.CheckError();
        }
        public delegate void PrepareDrawHandler();
        public event PrepareDrawHandler PrepareDraw;

        public delegate void CleanUpDrawHandler();
        public event CleanUpDrawHandler CleanUpDraw;

        public void ClearDepthbuffer()
        {
            //GL.ClearBuffer(ClearBuffer.Depth, DepthTexture, new float[1] { 0 });
        }

        public bool Disposing { get; set; }
        public virtual void Dispose()
        {
            Disposing = true;
            GL.DeleteFramebuffer(FBOHandle);
        }
    }
}
