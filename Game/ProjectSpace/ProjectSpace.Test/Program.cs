using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutpostOmega.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var dialog = new Server.Dialog.TestClient();
            Application.Run(dialog);

            //var chunk = Game.world.Chunk.GenerateTestChunk();
            //var mesh = chunk.Render();
         
            //World.SaveToFile(Directory.CreateDirectory("Bleh/"), false);

            //Data.World test = Data.DataHandler.LoadWorldFromFile(new FileInfo("Bleh/Generic World.sav"), false);

            //Data.gObject.turf.floor testsad = (Data.gObject.turf.floor)test.GameObjects[3];
        }
    }
}
