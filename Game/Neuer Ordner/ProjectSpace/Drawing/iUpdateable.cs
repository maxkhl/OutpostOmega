using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// Interface for updateable objects
    /// </summary>
    interface iUpdateable
    {
        void Update(double ElapsedTime);
    }
}
