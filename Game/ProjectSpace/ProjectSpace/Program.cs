using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using grendgine_collada;
using System.IO;
using System.Diagnostics;

namespace OutpostOmega
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

#if DEBUG
            var Game = new MainGame();
#else
            try
            {
                var Game = new MainGame();
            }
            catch(Exception e)
            {
                Application.Run(new Error.CrashReport(e));
            }
#endif
        }
    }
}
