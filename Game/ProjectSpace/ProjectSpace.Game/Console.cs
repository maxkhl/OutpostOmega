using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LuaInterface;

namespace OutpostOmega.Game
{
    /// <summary>
    /// LUA Console for testing/admin stuffz
    /// </summary>
    public class Console
    {
        World world;
        public Console(World world)
        {
            this.world = world;
            //Print("Console initialized");
        }

        public List<Lua.Assembly.Message> Execute(string Command)
        {
            var api = new Lua.Assembly(world);
            api.Execute(Command);

            List<Lua.Assembly.Message> messageList = new List<Lua.Assembly.Message>();
            while (api.Output.Count > 0)
                messageList.Add(api.Output.Dequeue());
            return messageList;
        }

        public List<Lua.Assembly.Message> ExecuteFile(string FileName)
        {
            var api = new Lua.Assembly(world);
            api.ExecuteFile(FileName);

            List<Lua.Assembly.Message> messageList = new List<Lua.Assembly.Message>();
            while (api.Output.Count > 0)
                messageList.Add(api.Output.Dequeue());
            return messageList;
        }
    }
}
