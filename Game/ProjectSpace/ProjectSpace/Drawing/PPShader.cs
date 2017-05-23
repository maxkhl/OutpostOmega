using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// Post Processing Shader
    /// </summary>
    class PPShader : Shader
    {
        /// <summary>
        /// Number of passes (is that even a word?), this shader needs
        /// </summary>
        public int PassCount { get; set; }

        public PPShader(FileInfo FragmentFile, int PassCount = 1)
            : base(new FileInfo(@"Content\Shader\PP_VS.glsl"), FragmentFile)
        {
            this.PassCount = PassCount;
        }

        public PPShader(FileInfo FragmentFile, FileInfo VertexFile, int PassCount = 1)
            : base(VertexFile, FragmentFile)
        {
            this.PassCount = PassCount;
        }
    }
}
