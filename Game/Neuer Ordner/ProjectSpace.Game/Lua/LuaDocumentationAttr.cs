using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OutpostOmega.Game.Lua
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class LuaDocumentationAttr : Attribute
    {
        /// <summary>
        /// Description of the method
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Return description
        /// </summary>
        public string Return { get; set; }

        /// <summary>
        /// Parameter description
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// Method category
        /// </summary>
        public string Category { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public LuaDocumentationAttr(string Category, string Description, string Return, string Parameters)
        {
            this.Category = Category;
            this.Description = Description;
            this.Return = Return == "" ? "nothing" : Return;
            this.Parameters = Parameters == "" ? "nothing" : Parameters;
        }
    }
}
