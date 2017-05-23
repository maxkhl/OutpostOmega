using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutpostOmega.Game.GameObjects.Mobs.CarbonBased
{
    /// <summary>
    /// Abstract carbon - mobtype. This is used for carbon-based mobs
    /// </summary>
    abstract public class Carbon : Mob
    {
        public Carbon(World world, float Height, float Width, float Mass, string ID = "carbon") 
            : base(world, ID, Height, Width, Mass)
        {

        }
    }
}
