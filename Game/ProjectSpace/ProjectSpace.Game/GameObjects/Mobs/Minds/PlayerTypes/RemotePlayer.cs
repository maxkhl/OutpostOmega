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

        /// <summary>
        /// Applies and interprets a mousestate for this mind.
        /// </summary>
        /// <param name="MouseState">Delta-state of the mouse.</param>
        public void ApplyMouseState(Tools.MouseState MouseState)
        {
            this.Mob.View.AddRotation(
                (float)MouseState.X,
                (float)MouseState.Y);
        }
    }
}
