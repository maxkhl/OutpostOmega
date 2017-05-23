using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutpostOmega.Server
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var main = new Main();
            try
            {
                Application.Run(main);
            }
            catch(Exception e)
            {
                if(main.Host.netServer.Status == Lidgren.Network.NetPeerStatus.Running)
                    main.Host.netServer.Shutdown("Server Crashed"); // Shutdown and notify clients

                foreach (var client in main.Host.Clients)
                    client.WorkerThreadRunning = false; // Turn off client threads

                main._mainGame._mainThreadAlive = false; // Turn off main processing thread

                Application.Run(new Error.CrashReport(e)); // Show crash report
            }
        }
    }
}
