using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Mobs.Minds.PlayerTypes
{
    public class RemotePlayer : PlayerMind
    {
        public RemotePlayer(World world, string ID = "remoteplayer")
            : base(world, ID)
        {

        }

    }
}
