using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutpostOmega.Game.GameObjects.Mobs
{
    /// <summary>
    /// Not a real mob. Basicaly a flying camera
    /// </summary>
    class Observer : GameObject
    {
        public Observer(World world, string ID = "observer")
            : base(world, ID)
        {

        }
    }
}
