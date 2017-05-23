using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using OpenTK;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Mobs
{
    /// <summary>
    /// A standard mind - interface between different types of inputs (local, remote or ai) and mob
    /// </summary>
    public abstract class Mind : GameObject
    {
        /// <summary>
        /// The Mob, this mind is assigned to
        /// </summary>
        public Mob Mob
        { 
            get
            {
                return _Mob;
            }
            set
            {
                if (this._Mob != value)
                {
                    _Mob = value;
                    this.Parent = _Mob;
                    _Mob.Mind = this;
                    NotifyPropertyChanged("Mob");
                }
            }
        }
        private Mob _Mob;

        /// <summary>
        /// Gets a value indicating whether this mind has body.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a body; otherwise, <c>false</c>.
        /// </value>
        public bool HasBody
        {
            get
            {
                return Mob != null;
            }
        }

        public Mind(World world, string ID = "mind") : base(world, ID)
        {

        }

        public override void Update(double ElapsedTime)
        {
            //Locks all updating after that
            //This should include playerinput and ai calculations
            if (!this.Freeze)
                base.Update(ElapsedTime);
        }

        /// <summary>
        /// Freezes this mind. It wont be able to think or control a mob anymore
        /// </summary>
        public bool Freeze { get; set; }
    }
}
