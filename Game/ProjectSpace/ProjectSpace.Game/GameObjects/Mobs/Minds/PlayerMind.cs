using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Mobs.Minds
{
    public abstract class PlayerMind : Mind
    {



        /// <summary>
        /// Account-username of this Player
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Usergroup that determins what rights this mind has
        /// </summary>
        [Attributes.Access(datums.UserGroup.Administrator)]
        public datums.UserGroup Group { get; set; }




        public PlayerMind(World world, string ID = "player")
            : base(world, ID)
        {
            this.Group = world.Settings.DefaultUserGroup;
        }


        /// <summary>
        /// Applies and interprets a mousestate for this player-mind.
        /// </summary>
        /// <param name="MouseState">Delta-state of the mouse.</param>
        public virtual void ApplyMouseState(Tools.MouseState MouseState)
        {
            this.Mob.View.AddRotation(
                (float)MouseState.X,
                (float)MouseState.Y);
        }

    }
}
