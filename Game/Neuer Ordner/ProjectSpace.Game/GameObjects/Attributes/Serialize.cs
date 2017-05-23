using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OutpostOmega.Game.GameObjects.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class Serialize : Attribute
    {
        public SerializeState State { get; set; }

        public bool DoSerialize
        {
            get
            {
                return !(State == SerializeState.DoNotSerialize);
            }
        }

        public Serialize(SerializeState Serializable)
        {
            this.State = Serializable;

        }
    }
    public enum SerializeState
    {
        DoNotSerialize,
    }
}
