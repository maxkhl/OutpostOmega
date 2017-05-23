using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using OutpostOmega.Network;
using OutpostOmega.Game.GameObjects.Mobs;
using OutpostOmega.Game.GameObjects.Mobs.Minds;
using OutpostOmega.Game.GameObjects.Mobs.Minds.PlayerTypes;
using OutpostOmega.Game;
using System.Diagnostics;

namespace OutpostOmega.Server.Network
{
    /// <summary>
    /// Used to manage a single client that is connected to the server
    /// </summary>
    public class Client
    {
        private uint s_timeInitialized = (uint)Environment.TickCount;

        public double Clock 
        { 
            get 
            {
                return this.Connection.GetRemoteTime(NetTime.Now);
                //return (double)((uint)Environment.TickCount - s_timeInitialized) / 1000.0;  
            } 
        }

        /// <summary>
        /// The Mind of this client
        /// </summary>
        public PlayerMind Mind { get; set; }

        /// <summary>
        /// The clients scope. This will watch the clients situation and send him the necessary data
        /// </summary>
        public Scope Scope { get; set; }

        public string ID
        {
            get
            {
                if (Mind == null)
                    throw new Exception("Not gud. Mhmm...");
                return Mind.Username;
            }
        }

        /// <summary>
        /// Online status of this client
        /// </summary>
        public bool Online
        {
            get
            {
                return _Online;
            }
            set
            {
                if (_Online != value)
                {
                    if (value && !Host.ConnectedClients.Contains(this))
                    {
                        lock (Host.ConnectedClients)
                        {
                            Host.ConnectedClients.Add(this);
                        }
                        this.Scope.Restore(true); // wakes up the scope
                        StartThread();
                    }
                    else if (!value)
                    {
                        StopThread();
                        if (Connection.Status == NetConnectionStatus.Connected)
                            Connection.Disconnect("N/A");
                        
                        lock (Host.ConnectedClients)
                        {
                            if (Host.ConnectedClients.Contains(this))
                                Host.ConnectedClients.Remove(this);
                        }

                        OutpostOmega.Data.cConverter.UnloadConverter(this.ID);

                        this.Scope.Idle(); // puts the scope to sleep
                    }
                    _Online = value;
                }
            }
        }
        private bool _Online = false;

        /// <summary>
        /// Lidgren-connection object
        /// </summary>
        public NetConnection Connection { get; set; }

        /// <summary>
        /// Host, this client is connected to
        /// </summary>
        public Host Host { get; set; }

        public Task WorkerTask;
        public bool WorkerThreadRunning;

        /*/// <summary>
        /// Determins if this client is sending Heartbeats
        /// </summary>
        public bool Heartbeat { get; set; }

        /// <summary>
        /// Timestamp of latest heartbeat
        /// </summary>
        public double LastHeartbeat { get; set; }*/

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Host">Assigned Host</param>
        /// <param name="Username">Username</param>
        /// <param name="Connection">Assigned Connection</param>
        public Client(string Username, Host Host, NetConnection Connection)
        {
            this.Host = Host;
            this.Connection = Connection;

            //Get the Username and create mind with it
            foreach (GameObject gameObject in Host.networkHandler.World.AllGameObjects)
            {
                if (typeof(PlayerMind).IsAssignableFrom(gameObject.GetType()) && ((PlayerMind)gameObject).Username == Username)
                {
                    this.Mind = ((PlayerMind)gameObject);
                    if (!this.Mind.Registered) this.Mind.Register();

                    Main.Message(Connection.RemoteEndPoint.Address.ToString() + " logged in as '" + Username + "'. Existing mind assigned.");
                    break;
                }

            }
            if (this.Mind == null)
            {
                Main.Message(Connection.RemoteEndPoint.Address.ToString() + " logged in as '" + Username + "'. New mind assigned.");
                this.Mind = new RemotePlayer(Host.networkHandler.World);
                this.Mind.Username = Username;
                this.Mind.Register();

                this.Mind.Mob = new OutpostOmega.Game.GameObjects.Mobs.CarbonBased.Human(Host.networkHandler.World);
                this.Mind.Mob.SetPosition(new Jitter.LinearMath.JVector(0, 100, 0));
                this.Mind.Mob.Register();
            }


            
            // Do that when the Mind is assigned!
            lock (Host.ConnectedClients)
            {
                Host.ConnectedClients.Add(this);
            }
            Host.Clients.Add(this);
            this.Online = true;


            this.Scope = new Scope(this);
            this.Scope.NeedsUpdate = true;
            StartThread();
        }

        private void StopThread()
        {
            if (WorkerTask != null && WorkerTask.Status == TaskStatus.Running)
            {
                WorkerThreadRunning = false;
                while (WorkerTask.Status == TaskStatus.Running)
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void StartThread()
        {
            StopThread();

            WorkerThreadRunning = true;
            WorkerTask = new Task(DoWork);
            WorkerTask.Start();
        }

        private void DoWork()
        {
            Stopwatch stopwatch = new Stopwatch();
            decimal Interval = 0;
            stopwatch.Start();
            while (WorkerThreadRunning && this.Online)
            {
                this.Scope.FlushMessageQueue();

                /*if (HostSettings.Default.Heartbeat)
                {
                    // Send Heartbeat
                    if (Clock - LastHeartbeat > 0.05)
                        this.Connection.SendMessage(this.GetOM(Command.Heartbeat), NetDeliveryMethod.ReliableOrdered, 1);

                    if (Clock - LastHeartbeat > 0.5)
                        Heartbeat = false;
                }*/

                Interval = stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();

                int targetStep = (int)(1000 / HostSettings.Default.TargetTransferRate - Interval);
                if (targetStep > 0)
                {
                    stopwatch.Stop();
                    Thread.Sleep(targetStep);
                    stopwatch.Start();
                }
                else
                { } // Server is unable to send stuff in time
            }
            stopwatch.Stop();
        }

        /// <summary>
        /// Used to watch the minds properties
        /// </summary>
        void Mind_PropertyChanged(GameObject sender, string PropertyName, bool ChildRaise)
        {
            // Update the scope if the mind's position is changing
            if (PropertyName == "Position")
            {
                var gObject = sender;
                var Position = gObject.Position;
                var LastPosition = gObject.LastMove + Position;

                int xP = (int)Position.X,
                    yP = (int)Position.Y,
                    zP = (int)Position.Z;

                int xL = (int)LastPosition.X,
                    yL = (int)LastPosition.Y,
                    zL = (int)LastPosition.Z;

                // Check if the position changed a full integer. If true, update the scope
                if (xP != xL ||
                    yP != yL ||
                    zP != zL)
                    this.Scope.NeedsUpdate = true;
            }
        }

        public Dictionary<SecondCommand, double> LastReceivedPackageTime = new Dictionary<SecondCommand, double>();
        public double LastReceivedMouseTime = 0;

        /// <summary>
        /// Used to process a specific message adressed to this client
        /// </summary>
        public void ProcessPackage(Object state)
        {
            NetIncomingMessage im = (NetIncomingMessage)state;
            if (im == null)
                return;

            var Type = im.ReadByte();
            var subType = im.ReadByte();
            if (LastReceivedPackageTime.ContainsKey((SecondCommand)subType))
                LastReceivedPackageTime[(SecondCommand)subType] = this.Clock;
            else
                LastReceivedPackageTime.Add((SecondCommand)subType, this.Clock);

            switch (Type)
            {
                case (byte)Command.Login:

                    if ((byte)SecondCommand.Request == im.ReadByte())
                    {
                        this.Mind.PropertyChanged += Mind_PropertyChanged;
                        this.Mind.PhysicSetPosition(Jitter.LinearMath.JVector.Forward);

                        // Tell the client that Login was ok/not ok
                        var om = GetOM(Command.Login, SecondCommand.Confirmed);
                        om.Write(Mind.Username);
                        SendOM(om, NetDeliveryMethod.ReliableOrdered);
                        Main.Message(im.SenderConnection.RemoteEndPoint.Address.ToString() + " logged in as " + Mind.Username);

                        // Send initialization-data
                        /*om = GetOM(Command.Create);
                        var data = Host.networkHandler.GetObjectData(Host.networkHandler.World);
                        om.Write(data.Length);
                        om.Write(data);
                        SendOM(om, NetDeliveryMethod.ReliableOrdered);*/
                    }
                    break;
                case (byte)Command.Data:
                    switch(subType)
                    {
                        case (byte)SecondCommand.Input:

                            OutpostOmega.Game.Tools.KeybeardState kState;

                            if (this.Mind.SimulatedKeyInput != null)
                                kState = this.Mind.SimulatedKeyInput;
                            else
                                kState = new OutpostOmega.Game.Tools.KeybeardState();

                            LastReceivedPackageTime[(SecondCommand)subType] = im.ReadDouble();
                            //kState.ElapsedTime = Clock - im.ReadDouble();

                            while(im.Position < im.LengthBits)
                            {
                                var key = (OpenTK.Input.Key)im.ReadByte();
                                var btnstate = im.ReadBoolean();
                                if(btnstate)
                                { }
                                kState[key] = btnstate;
                            }
                            this.Mind.SimulatedKeyInput = kState;

                            break;
                        case (byte)SecondCommand.InputMouseDelta:

                            if(this.Mind.SimulatedMouseInput == null)
                                this.Mind.SimulatedMouseInput = new Game.Tools.MouseState();

                            var remoteTime = im.ReadDouble();
                            LastReceivedPackageTime[(SecondCommand)subType] = remoteTime;
                            LastReceivedMouseTime = remoteTime;
                            //im.ReadDouble(); // müll
                            //this.Mind.SimulatedMouseInput.ElapsedTime = -1;//Clock - 
                            //this.Mind.SimulatedMouseInput.LeftKey = im.ReadBoolean();
                            //this.Mind.SimulatedMouseInput.MiddleKey = im.ReadBoolean();
                            //this.Mind.SimulatedMouseInput.RightKey = im.ReadBoolean();
                            this.Mind.SimulatedMouseInput.X -= im.ReadInt32();
                            this.Mind.SimulatedMouseInput.Y -= im.ReadInt32();
                            //this.Mind.SimulatedMouseInput = newMS;

                            break;
                        case (byte)SecondCommand.GameObject:
                            if (this.Mind.World != null)
                            {
                                var gameObject_ID = im.ReadString();
                                var property_name = im.ReadString();
                                var data = im.ReadBytes(im.ReadInt32());
                                var new_value = NetworkHandler.ReadSerializedData(data);

                                var gameObject = this.Mind.World.GetGameObject(gameObject_ID);
                                if (gameObject != null)
                                {
                                    var accessAttrList = gameObject.GetType().GetProperty(property_name).GetCustomAttributes(typeof(Game.GameObjects.Attributes.Access), false);
                                    if (accessAttrList.Length > 0 &&
                                        ((Game.GameObjects.Attributes.Access)accessAttrList[0]).HasAccess(this.Mind.Group))
                                    {
                                        gameObject.GetType().GetProperty(property_name).SetValue(gameObject, new_value, null);
                                    }
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Disconnects the client
        /// </summary>
        /// <param name="Reason">Reason for forced disconnect</param>
        public void Disconnect(string Reason)
        {
            Main.Message("Closing connection to " + Mind.Username + " (" + Connection.RemoteEndPoint.Address.ToString() + ") with reason '" + Reason + "'");

            Connection.Disconnect(Reason);
            this.Online = false;

            lock (Host.ConnectedClients)
            {
                Host.ConnectedClients.Remove(this);
            }
        }

        /// <summary>
        /// Returns a new NetOutgoingMessage with an assigned Type for this Client. Messages have to contain a type!
        /// </summary>
        /// <param name="Type">Type of the command.</param>
        /// <param name="SecondType">Second command of the package.</param>
        /// <returns>new NetOutgoingMessage</returns>
        public NetOutgoingMessage GetOM(Command Type, SecondCommand SecondType = SecondCommand.Null)
        {
            var om = Connection.Peer.CreateMessage();
            om.Write((byte)Type);
            om.Write((byte)SecondType);
            return om;
        }

        /// <summary>
        /// Sends a message to the client
        /// </summary>
        /// <param name="om"></param>
        /// <param name="deliveryMethod"></param>
        public void SendOM(NetOutgoingMessage om, NetDeliveryMethod deliveryMethod)
        {
            Connection.SendMessage(om, deliveryMethod, 0);
        }

        /// <summary>
        /// Sends a message to this client
        /// </summary>
        public void SendMessage(string Text)
        {
            var om = GetOM(Command.Message);
            om.Write(Text);
            SendOM(om, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
