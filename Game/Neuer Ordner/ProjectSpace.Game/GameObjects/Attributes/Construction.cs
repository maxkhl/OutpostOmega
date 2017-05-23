using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Attributes
{
    /// <summary>
    /// Construction Attribute
    /// Used to define a construction progress
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class Construction : Attribute
    {

        /// <summary>
        /// Base that needs to be present to build this object
        /// </summary>
        public Type baseObject { get; set; }

        /// <summary>
        /// Tool, that needs to be used on the baseObject to construct this object
        /// </summary>
        public Type Tool { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseObject">Structure that needs to be present to build this object (Type structure!)</param>
        /// <param name="Tool">Tool, that needs to be used on the baseObject to construct this object (Type item.tool!)</param>
        public Construction(Type baseObject, Type Tool)
        {
            //
            //item.tool
            if (typeof(OutpostOmega.Game.GameObjects.Structures.Structure).IsAssignableFrom(baseObject))
                this.baseObject = baseObject;
            else
                throw new InvalidOperationException("baseObject needs to be type structure!");

            if (typeof(Items.Tool).IsAssignableFrom(Tool))
                this.Tool = Tool;
            else
                throw new InvalidOperationException("Tool needs to be type item.tool!");
        }
    }
}
