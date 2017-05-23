using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.datums.program
{
    /// <summary>
    /// Contains a Lua script that can be executed
    /// </summary>
    public class program : datum
    {
        /// <summary>
        /// The code of this program
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Assembly the program is running in
        /// </summary>
        public Lua.Assembly Assembly { get; set; }

        public string Name { get; private set; }

        public program(string Name, World world)
            : base(world)
        {
            this.Name = Name;
            this.Assembly = new Lua.Assembly(world);
        }

        public program(string Name, World world, Lua.Assembly Assembly)
            : base(world)
        {
            this.Name = Name;
            this.Assembly = Assembly;
        }

        /// <summary>
        /// Executes the program in its current assembly
        /// </summary>
        public void Execute()
        {
            this.Assembly.Execute(Code);
        }

        /// <summary>
        /// Resets the assembly of this program to its default state. Clears all declarations
        /// </summary>
        public void ResetAssembly()
        {
            this.Assembly = new Lua.Assembly(World);
        }
    }
}
