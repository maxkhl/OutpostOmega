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
        /// Indicates a crash occured
        /// </summary>
        public static bool Crashed = false;

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var main = new Main();

#if DEBUG
            Application.Run(main);
#else
            try
            {
                Application.Run(main);
            }
            catch(Exception e)
            {
                Crashed = true;

                //Stop game and other asynchronous stuff on the mainform
                main.Stop();

                //Shutdown server
                if(main.Host.netServer.Status == Lidgren.Network.NetPeerStatus.Running)
                    main.Host.netServer.Shutdown("Server Crashed"); // notifies clients

                //Eliminate client threads
                foreach (var client in main.Host.Clients)
                    client.WorkerThreadRunning = false; // Turn off client threads

                Application.Run(new Error.CrashReport(e)); // Show crash report                
            }
#endif
        }
    }
}
