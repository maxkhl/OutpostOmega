using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OutpostOmega.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var dialog = new Server.Dialog.TestClient();
            dialog.ShowDialog();

            //var chunk = Game.world.Chunk.GenerateTestChunk();
            //var mesh = chunk.Render();
         
            //World.SaveToFile(Directory.CreateDirectory("Bleh/"), false);

            //Data.World test = Data.DataHandler.LoadWorldFromFile(new FileInfo("Bleh/Generic World.sav"), false);

            //Data.gObject.turf.floor testsad = (Data.gObject.turf.floor)test.GameObjects[3];
        }
    }
}
