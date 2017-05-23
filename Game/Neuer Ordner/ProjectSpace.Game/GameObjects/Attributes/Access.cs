using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OutpostOmega.Game.GameObjects.Attributes
{
    /// <summary>
    /// Used to define access for specific groups
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class Access : Attribute
    {
        /// <summary>
        /// The Usergroup, that is needed to edit this property
        /// </summary>
        public datums.UserGroup Group { get; set; }

        /// <summary>
        /// Constructor (General)
        /// </summary>
        public Access(datums.UserGroup Group)
        {
            this. Group =  Group;
        }

        /// <summary>
        /// Determines whether the specified group has access.
        /// </summary>
        public bool HasAccess(datums.UserGroup Group)
        {
            return (int)Group > (int)this.Group;
        }
    }
}
