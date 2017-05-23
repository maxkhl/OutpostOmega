using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects
{
    /// <summary>
    /// Used to define exectuable functions in gameObjects
    /// </summary>
    public struct Function
    {
        /// <summary>
        /// Display text
        /// </summary>
        public string Text;

        /// <summary>
        /// Function ID
        /// </summary>
        public string ID;

        /// <summary>
        /// Function enabled?
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// Separator entry?
        /// </summary>
        public bool Separator;
    }
}
