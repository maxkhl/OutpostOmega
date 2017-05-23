using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OutpostOmega.Game.GameObjects.Attributes
{
    /// <summary>
    /// Used to add a name, description and all the other stuff to a gameObject
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class Definition : Attribute
    {
        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the item
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Constructor (General)
        /// </summary>
        /// <param name="Name">Name of the item</param>
        /// <param name="Description">Description</param>
        /// <param name="Attributes">Custom Attributes (will be displayed on the datasheet of the item)</param>
        public Definition(string ObjectName, string Description)
        {
            this.Name = ObjectName;
            this.Description = Description;
        }
    }
}
