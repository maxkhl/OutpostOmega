using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace OutpostOmega.Game.Lua
{
    public abstract class Parser : LuaInterface.Lua
    {
        public string AppPath = "";
        public Parser()
            : base()
        {
            AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            
        }
        public void SetObject(string Identification, object Object)
        {
            this[Identification] = Object;
        }

        public virtual void Execute(string Command)
        {
            this.DoString(Command);
        }

        public virtual void ExecuteFile(string FileName)
        {
            Execute("G_EXECPATH = \"" + (new FileInfo(FileName)).DirectoryName.Replace("\\", "/") + "/\"");
            this.DoFile(FileName);
            Execute("G_EXECPATH = \"" + AppPath.Replace("\\", "/") + "/\"");
        }
    }
}
