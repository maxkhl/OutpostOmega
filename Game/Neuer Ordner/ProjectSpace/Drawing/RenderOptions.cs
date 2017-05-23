using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// Options for model rendering (can be passed through draw method)
    /// </summary>
    struct RenderOptions
    {
        /// <summary>
        /// Draw a model in Wireframe mode
        /// </summary>
        public bool Wireframe;

        /// <summary>
        /// This shader will be used instead of the intended shader. Careful with that!
        /// </summary>
        public Shader Shader;

        /// <summary>
        /// Can be used to set the shader uniforms
        /// </summary>
        public Action<Shader> SetUniform;

        /// <summary>
        /// The objects color
        /// </summary>
        public OpenTK.Graphics.Color4 Color;
    }
}
