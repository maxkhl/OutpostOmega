using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.atmospherics
{
    /// <summary>
    /// Basic gas class
    /// </summary>
    public static class Gas
    {
        /// <summary>
        /// Get the name of a specific gas
        /// </summary>
        /// <param name="ID">Gas ID</param>
        public static string Name(short ID)
        {
            switch(ID)
            {
                case 0:
                    return "Oxygen";
                case 1:
                    return "Fart";
                default:
                    return "Unknown";
            }
        }
    }
}
