
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.atmospherics
{
    /// <summary>
    /// Used to describe a specific gas and its state inside of a block
    /// </summary>
    public struct GasState
    {
        /// <summary>
        /// The ID of the gas type
        /// </summary>
        public byte GasID;

        /// <summary>
        /// Amount of gas-units
        /// </summary>
        public float Units;
    }
}
