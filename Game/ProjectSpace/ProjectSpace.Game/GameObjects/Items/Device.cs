using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Items
{
    /// <summary>
    /// Devices are items that contain their own functionality instead of relying on other objects they are used with
    /// </summary>
    public class Device : Item
    {
        public Device(World world, string ID = "device")
            : base(world, ID)
        {

        }

        /// <summary>
        /// Called whenever this decive is used
        /// </summary>
        public virtual void UseDevice(GameObject Target, Mob User, Game.Tools.Action Action)
        { }
    }
}
