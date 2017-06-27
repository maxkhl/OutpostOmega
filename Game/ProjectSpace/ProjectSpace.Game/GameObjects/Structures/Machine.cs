using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures
{
    /// <summary>
    /// The "intelligent" structure. Use this class for advanced logic and interaction of your structure
    /// F.e. doors, computers, ... (basicaly every physical device that works with energy and/or needs an interface/interaction possibility)
    /// </summary>
    public abstract class Machine : Structure
    {
        /// <summary>
        /// Energy Drain (how much energy this object consumes) 0 = nothing, negative values will produce energy. standard battery contains 1000 units. Energy gets drained every second
        /// </summary>
        public float EnergyDrain
        {
            get { return _EnergyDrain; }
            set { _EnergyDrain = value; NotifyPropertyChanged("EnergyDrain"); }
        }
        private float _EnergyDrain = 0;

        /// <summary>
        /// PowerState of this machine. Will fire the powerStateChanged Event when changes occur
        /// </summary>
        public powerState powerState
        {
            get { return _powerState; }
            set 
            {
                if (_powerState != value)
                {
                    if(powerStateChanged != null)
                        powerStateChanged(this, new EventArgs());
                    _powerState = value;
                    NotifyPropertyChanged("powerState");
                }
            }
        }
        private powerState _powerState = powerState.Enabled;

        /// <summary>
        /// Fires when the powerState-Property changes (only if real changes occur f.e. off -> on)
        /// </summary>
        public event EventHandler powerStateChanged;

        public Machine(int X, int Y, int Z, Turf.Structure Structure, World World, string ID = "machinery")
            : base(X, Y, Z, Structure, World, ID)
        {

        }
    }

    /// <summary>
    /// (Power-)State of a machinery-gameobject
    /// </summary>
    public enum powerState
    {
        Enabled,
        Disabled,
        zeroEnergy
    }
}
