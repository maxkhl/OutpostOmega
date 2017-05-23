using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// Texture that got rendered by a RenderTarget
    /// </summary>
    class RenderTexture : Texture2D
    {
        public RenderTarget AssignedRenderTarget { get; protected set; }

        public RenderTexture(RenderTarget renderTarget)
            : base()
        {
            AssignedRenderTarget = renderTarget;
            Handle = (int)renderTarget.OutTexture;
            this.filterMode = FilterMode.Clamp; // This is important!
            this.TextureMatrix = Matrix4.CreateScale(1.0f, -1.0f, 1.0f); // Invert to get origin to top left not bottom left
        }

        protected override void PrepareBind()
        {
            /*GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Texture2D);*/
            base.PrepareBind();
        }
    }
}
