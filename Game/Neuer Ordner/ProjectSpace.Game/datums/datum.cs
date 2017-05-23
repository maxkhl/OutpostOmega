using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.datums
{
    /// <summary>
    /// A datum is a definition that is influencing the game.
    /// It could be a gamerule, a job or several other things
    /// </summary>
    public abstract class datum
    {
        /// <summary>
        /// The connected world
        /// </summary>
        public World World { get; private set; }

        /// <summary>
        /// Tells if the update method should be called
        /// </summary>
        public bool NeedsUpdate { get; set; }

        public datum(World World)
        {
            this.World = World;
            this.NeedsUpdate = false;
        }

        /// <summary>
        /// Introduces this datum to the world and makes it updateable
        /// </summary>
        public void Register()
        {
            if(!this.World.Datums.Contains(this))
                this.World.Datums.Add(this);
        }
        
        /// <summary>
        /// Updates this datum
        /// </summary>
        public virtual void Update()
        { }
    }
}
