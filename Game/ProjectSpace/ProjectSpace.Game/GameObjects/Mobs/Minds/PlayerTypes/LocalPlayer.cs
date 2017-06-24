using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK;
using Jitter.LinearMath;
using OutpostOmega.Game.Tools;

namespace OutpostOmega.Game.GameObjects.Mobs.Minds.PlayerTypes
{
    public class LocalPlayer : PlayerMind
    {
        public LocalPlayer(World world, string ID = "localplayer")
            : base(world, ID)
        {
        }
    }
}
