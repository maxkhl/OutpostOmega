using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Items
{
    abstract public class Stack : Item
    {        
        public Stack(World world, string ID = "stack")
            : base(world, ID)
        {

        }
    }
}
