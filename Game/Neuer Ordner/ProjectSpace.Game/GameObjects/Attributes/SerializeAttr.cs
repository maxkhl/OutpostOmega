using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OutpostOmega.Game.gObject.attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    class SerializeAttr : Attribute
    {
        public SerializeState Serializable { get; set; }

        public SerializeAttr(SerializeState Serializable)
        {
            this.Serializable = Serializable;

        }
    }
    public enum SerializeState
    {
        Serializable,
        NotSerializable
    }
}
