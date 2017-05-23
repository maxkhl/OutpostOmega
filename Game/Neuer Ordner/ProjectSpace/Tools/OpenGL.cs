using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Tools
{
    static class OpenGL
    {
        /// <summary>
        /// Reads the GL Error and throws a exception
        /// </summary>
        /// <param name="Info">Addition info that'll get attached to the dump message</param>
        public static void CheckError(string Info = "")
        {
            var eCode = GL.GetError();
            if (eCode != ErrorCode.NoError)
            {
                throw new Exception(eCode.ToString() + " # " + Info);
            }
        }

        public static void CheckFboError()
        {
            switch (GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt))
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    break; // Everything cool
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                        throw new Exception("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                        throw new Exception("FBO: There are no attachments.");
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                        throw new Exception("FBO: Attachments are of different size. All attachments must have the same width and height.");
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                        throw new Exception("FBO: The color attachments have different format. All color attachments must have the same format.");
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                        throw new Exception("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                        throw new Exception("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                        throw new Exception("FBO: This particular FBO configuration is not supported by the implementation.");
                default:
                        throw new Exception("FBO: Status unknown. (yes, this is really bad.)");
            }
        }
    }
}
