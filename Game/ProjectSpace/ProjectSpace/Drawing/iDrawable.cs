using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// Interface for drawable objects
    /// </summary>
    interface iDrawable
    {
        void Draw();
        void Draw(RenderOptions renderOptions);
    }
}
