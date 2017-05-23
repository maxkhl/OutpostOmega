using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Handles mods
    /// </summary>
    public partial class World
    {
        public List<Lua.ModPack> Mods { get; set; }
        
        public Lua.ModPack LoadMod(FileInfo ModXMLFile)
        {
            var newModPack = new Lua.ModPack(ModXMLFile, this);
            Mods.Add(newModPack);
            return newModPack;
        }
    }
}
