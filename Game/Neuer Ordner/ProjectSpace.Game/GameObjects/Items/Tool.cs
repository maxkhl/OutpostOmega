using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Items
{
    /// <summary>
    /// Tools are items, that can be used to construct/deconstruct stuff in the gameworld
    /// </summary>
    public abstract class Tool : Item
    {
        public Tool(World world, string ID = "tool")
            : base(world, ID)
        {

        }
    }
}
