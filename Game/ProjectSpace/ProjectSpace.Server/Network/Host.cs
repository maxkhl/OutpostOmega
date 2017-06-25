using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using OutpostOmega.Network;
using OutpostOmega.Data;
using OutpostOmega.Game;

namespace OutpostOmega.Server.Network
{
    /// <summary>
    /// Host class that manages all the connections
    /// </summary>
    public class Host : IDisposable
    {
        /// <summary>
        /// Lidgren Server object. Contains all the technical network stuff
        /// </summary>
        public NetServer netServer { get; set; }

        /// <summary>
        /// Blocks all new connections if true.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Reason for locking the server
        /// </summary>
        public string Lockreason { get; private set; }

        /// <summary>
        /// Port, this server is listening on
        /// </summary>
        public int Port
        {
            get
            {
                return netServer.Port;
            }
        }

        /// <summary>
        /// Name of this host instance
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Handles all the ingame Networking
        /// </summary>
        public NetworkHandler networkHandler { get; set; }

        /// <summary>
        /// All Clients that are currently active
        /// </summary>
        public ObservableCollection<Client> ConnectedClients;

        /// <summary>
        /// All known Clients
        /// </summary>
        public List<Client> Clients;

        /// <summary>
        /// Assigned world
        /// </summary>
        public World World { get; protected set; }

        public Host(string Hostname, World World)
        {
            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            // set peer
            NetPeerConfiguration config = new NetPeerConfiguration("pspace_network")
            {
                MaximumConnections = HostSettings.Default.MaxConnections,
                Port = HostSettings.Default.Port,
                ConnectionTimeout = HostSettings.Default.Timeout,
                //SimulatedLoss = 0.1f,
                //SimulatedMinimumLatency = 0.2f,
                //SimulatedRandomLatency = 0.5f
            };
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            netServer = new NetServer(config);
            this.Hostname = Hostname;

            this.World = World;

            this.networkHandler = new NetworkHandler(World);

            ConnectedClients = new ObservableCollection<Client>();
            Clients = new List<Client>();

            Main.Message("Host initialized and ready to start");
        }

        /// <summary>
        /// Starts the server and listener
        /// </summary>
        /// <returns>True if server got started. Careful! The Startup could still fail!</returns>
        public bool Start()
        {
            if (netServer.Status == NetPeerStatus.NotRunning)
            {
                try
                {
                    netServer.Start();
                }
                catch(Exception e)
                {
                    if (e.GetType() == typeof(System.Net.Sockets.SocketException))
                        Main.Message("Socket already bound. Check if you got another server running on the same port.", System.Drawing.Color.Red);
                    else
                        Main.Message("Could not start server: " + e.Message, System.Drawing.Color.Red);
                    return false;
                }
                Main.Message("Server started with following configuration");
                Main.Message("MaxConnections: " + netServer.Configuration.MaximumConnections.ToString());
                Main.Message("ConnectionTimeout: " + netServer.Configuration.ConnectionTimeout.ToString());

                _serverThread = new Thread(Listener);
                _serverThread.Name = "MainListener";
                _serverThread.Priority = ThreadPriority.Lowest;
                _serverThread.Start();
                
                //netServer.RegisterReceivedCallback(new SendOrPostCallback(ProcessMessage));

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Looks for a active client assigned to the given connection
        /// </summary>
        /// <returns>Null or found Client</returns>
        private Client GetClient(NetConnection connection)
        {
            return (from client in ConnectedClients
                    where client.Connection == connection
                    select client).SingleOrDefault();
        }

        private Thread _serverThread;
        private bool _serverThreadAlive = true;

        /// <summary>
        /// Processes a incoming Message
        /// </summary>
        private void Listener()
        {
            while (netServer.Status != NetPeerStatus.NotRunning && !Disposing) // Loop as long as the netserver is running
            {
                netServer.MessageReceivedEvent.WaitOne(500);

                NetIncomingMessage im;
                while ((im = netServer.ReadMessage()) != null)
                {
#if DEBUG
                    ProcessIm(im);
#else
                    try
                    {
                        ProcessIm(im);
                    }
                    catch(Exception e)
                    {
                        new OutpostOmega.Error.CrashReport(e);
                    }
#endif
                }
            }
        }

        /// <summary>
        /// Processes the given incoming message
        /// </summary>
        /// <param name="im">Incoming message</param>
        private void ProcessIm(NetIncomingMessage im)
        {
            if (Disposing) return;

            //NetIncomingMessage im = netServer.ReadMessage();

            // Get general information about sender
            var client = GetClient(im.SenderConnection);

            string adress = "Unknown";
            if (im.SenderConnection != null)
                adress = im.SenderConnection.RemoteEndPoint.Address.ToString();

            switch (im.MessageType)
            {
                // The big Message Block. Contains System-Messages that will be posted to the output-box
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:

                    if (client != null)
                        Main.Message(im.MessageType.ToString() + " from " + client.Mind.Username + " (" + adress + "): " + im.ReadString());
                    else if (im.SenderConnection != null)
                        Main.Message(im.MessageType.ToString() + " from " + adress + " (no client): " + im.ReadString());
                    else
                        Main.Message(im.ReadString());

                    break;
                // Connection Approval
                case NetIncomingMessageType.ConnectionApproval:

                    if (Locked)
                    {
                        im.SenderConnection.Deny("Server is locked currently. Reason: " + Lockreason);
                        Main.Message("Blocked connection due to active server lock");
                    }
                    else
                    {
                        var Username = im.ReadString();

                        var User = (from clnt in Clients
                                    where
                                     clnt.Mind.Username == Username
                                    select clnt).SingleOrDefault();

                        if (User != null)
                        {
                            if (User.Online) // Same user already online
                            {
                                im.SenderConnection.Deny("User with the same name already online");
                                Main.Message(adress + ": Rejected. Username '" + Username + "' already in use.");
                            }
                            else // User known but not online. Here should some kind of authentication happen but for now we'll just assign him to that user
                            {
                                im.SenderConnection.Approve();
                                User.Online = true;
                                User.Connection = im.SenderConnection;
                                User.Host = this;
                            }
                        }
                        else // Totaly new user
                        {
                            im.SenderConnection.Approve();
                            User = new Client(Username, this, im.SenderConnection);
                        }
                    }
                    break;
                // Connections and Disconnections
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                    string Reason = im.ReadString();
                    switch (status)
                    {
                        case NetConnectionStatus.RespondedAwaitingApproval:
                            Main.Message("Incomming connection from " + adress);
                            break;
                        case NetConnectionStatus.Disconnected:
                            if (client != null)
                            {
                                client.Online = false;
                                Main.Message(client.Mind.Username + " (" + adress + ") disconnected. Reason: " + Reason);
                            }
                            break;
                        case NetConnectionStatus.RespondedConnect:
                        case NetConnectionStatus.Connected:

                            break;
                        case NetConnectionStatus.Disconnecting:
                            if (client != null)
                            {
                                client.Online = false;
                            }
                            break;
                        default:
                            Main.Message("Unknown statusmessage '" + status.ToString() + "' (" + Reason + ")");
                            break;
                    }
                    break;
                // This is the interesting stuff!
                case NetIncomingMessageType.Data:
                    if (client != null)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(
                            new System.Threading.WaitCallback(client.ProcessPackageWorker), im);
                    }
                    else
                        Main.Message("Error! Unadressed Data-Package from " + im.SenderConnection.RemoteEndPoint.Address.ToString());
                    break;
                default:
                    string pmsg = im.ReadString();
                    Main.Message("Unknown messagetype '" + im.MessageType.ToString() + "' (" + pmsg + ")");
                    break;
            }
        }

        /// <summary>
        /// Server-Shutdown
        /// </summary>
        public void Shutdown()
        {
            Main.Message("Shutting down Server");

            // Disconnect the clients gently
            for (int i = 0; i < ConnectedClients.Count; i++ )
                ConnectedClients[i].Disconnect("Server is shutting down");
            ConnectedClients.Clear();

            // Now kill everything
            netServer.Shutdown("Server offline");

            // Wait for the Server-shutdown
            while (netServer.Status != NetPeerStatus.NotRunning)
                Thread.Sleep(100);
        }

        public void Lock(string Reason)
        {
            Lockreason = Reason;
            Locked = true;
            Main.Message("Server locked with reason '" + Lockreason + "'");
        }
        public void Unlock()
        {
            Locked = false;
            Main.Message("Server unlocked");
        }

        public bool Disposing = false;
        public void Dispose()
        {
            Disposing = true;
            Shutdown();
        }
    }
}
