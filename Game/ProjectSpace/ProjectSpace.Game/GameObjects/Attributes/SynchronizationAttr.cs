using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SynchronizationAttr : Attribute
    {
        public SynchronizePriority Priority { get; set; }
        public SynchronizeState State { get; set; }
        public bool NoSynch { get; set; }

        public SynchronizationAttr(
            SynchronizePriority Priority = SynchronizePriority.ReliableOrdered, 
            SynchronizeState State = SynchronizeState.Default,
            bool NoSynch = false)
        {
            this.Priority = Priority;
            this.State = State;
            this.NoSynch = NoSynch;
        }
    }
    public enum SynchronizeState
    {
        Default,
        Prediction
    }
    public enum SynchronizePriority
    {

        /// <summary>
        /// Unreliable, unordered delivery
        /// </summary>
        Unreliable = 1,

        /// <summary>
        /// Unreliable delivery, but automatically dropping late messages
        /// </summary>
        UnreliableSequenced = 2,

        /// <summary>
        /// Reliable delivery, but unordered
        /// </summary>
        ReliableUnordered = 34,

        /// <summary>
        /// Reliable delivery, except for late messages which are dropped
        /// </summary>
        ReliableSequenced = 35,

        /// <summary>
        /// Reliable, ordered delivery
        /// </summary>
        ReliableOrdered = 67,

        NoSynchronization = 0,
    }
}
