using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OutpostOmega.Game.GameObjects.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class IconAttribute : Attribute
    {
        /// <summary>
        /// Iconpath
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Constructor (General)
        /// </summary>
        public IconAttribute(string Path)
        {
            this.Path = Path;
        }
    }
}
