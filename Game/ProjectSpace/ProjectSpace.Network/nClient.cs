using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using Lidgren.Network;
using OutpostOmega.Network;
using OutpostOmega.Data;
using OutpostOmega.Game;
using System.Reflection;

namespace OutpostOmega.Network
{
    /// <summary>
    /// Client for handling networkstuff on the gameclient
    /// </summary>
    public partial class GameNetClient : IDisposable
    {
        /// <summary>
        /// Lidgren netClient object
        /// </summary>
        public NetClient NetClient { get; set; }
        
		private uint s_timeInitialized = (uint)Environment.TickCount;

        public double Clock 
        { 
            get 
            {
                if (this.NetClient.ConnectionStatus == NetConnectionStatus.Connected)
                    return this.NetClient.Connections[0].GetRemoteTime(NetTime.Now);
                else return 0;
                //return (double)((uint)Environment.TickCount - s_timeInitialized) / 1000.0; 
            } 
        }

        private struct TimedObjectState
        {
            public double Time { get; set; }
            public object Instance { get; set; }
        }

        /// <summary>
        /// Contains historical data about properties with enabled client-prediction
        /// </summary>
        private ConcurrentDictionary<object, ConcurrentDictionary<PropertyInfo, ConcurrentQueue<TimedObjectState>>> PredictionData = new ConcurrentDictionary<object, ConcurrentDictionary<PropertyInfo, ConcurrentQueue<TimedObjectState>>>();

        /// <summary>
        /// Username of the client
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The World this UClient is working with. Can return null if none got transmitted
        /// </summary>
        public World World
        {
            get
            {
                return _World;
            }
            set
            {
                NewWorldReceived?.Invoke(_World, value);

                Output.Enqueue("World '" + value.ID + "' received!");
                _World = value;
            }
        }
        private World _World = null;


        public delegate void NewWorldReceivedHandler(World oldWorld, World newWorld);
        public event NewWorldReceivedHandler NewWorldReceived;

        public delegate void DisconnectedHandler(GameNetClient sender, string reason);
        public event DisconnectedHandler Disconnected;

        /// <summary>
        /// Output queue that stores all the debug/error/warning/status messages (Threadsafe - use TryDequeue)
        /// </summary>
        public NetQueue<string> Output { get; set; }
                
        /// <summary>
        /// Used to determine if there is an active connection to a server. Can be set to false to disconnect. Error when set to true - use Connect()
        /// </summary>
        public bool Online
        {
            get
            {
                return _Online;
            }
            set
            {
                if(value)
                    throw new Exception("Please use the Connect()-method to start a connection");
                else
                    Disconnect();
            }
        }
        private bool _Online = false;

        /// <summary>
        /// Constructor. This wont start the communication. Use Connect to connect to a server.
        /// </summary>
        /// <param name="Username"></param>
        public GameNetClient(string Username)
        {
            this.Username = Username;

            Output = new NetQueue<string>(100);

            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            //Establish Connection
            NetPeerConfiguration config = new NetPeerConfiguration("pspace_network");
            //config.SimulatedMinimumLatency = 0;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            NetClient = new NetClient(config);

            NetClient.RegisterReceivedCallback(new SendOrPostCallback(MessageReceived));

            ThreadPool.QueueUserWorkItem(new WaitCallback(CalculatePPS));
        }

        public void CalculatePPS(object state)
        {
            while (!Disposing)
            {
                if (DateTime.Now.Subtract(PacketTime).Seconds > 1)
                {
                    PacketsPerSecond = PacketCounter;
                    PacketCounter = 0;
                    PacketTime = DateTime.Now;
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Tries to connect to the given server. Returns false if connection is already going on. Check 'Online' to see if the client is connected
        /// </summary>
        /// <param name="ServerAdress">Target Serveradress</param>
        /// <param name="ServerPort">Target Port</param>
        /// <returns>True = Initial connection</returns>
        public bool Connect(string ServerAdress, int ServerPort)
        {


            if( NetClient.ConnectionStatus == NetConnectionStatus.None ||
                NetClient.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                NetClient.Start();
                var approval = NetClient.CreateMessage();
                approval.Write(Username);

                NetConnection connection = NetClient.Connect(ServerAdress, ServerPort, approval);
                Output.Enqueue("Connecting to " + ServerAdress + ":" + ServerPort.ToString());
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Used to disconnect this client
        /// </summary>
        /// <param name="Reason">Reason for disconnect</param>
        public void Disconnect(string Reason = "Requested by user")
        {
            if (NetClient.ConnectionStatus == NetConnectionStatus.Connected)
            {
                NetClient.Disconnect(Reason);

                // Fire Dc Event
                Disconnected?.Invoke(this, Reason);
            }
            _Online = false;
        }

        /// <summary>
        /// Returns a new NetOutgoingMessage with an assigned Type for this Client. Messages have to contain a type!
        /// </summary>
        /// <param name="Type">Type of the command.</param>
        /// <param name="SecondType">Second command of the package.</param>
        /// <returns>new NetOutgoingMessage</returns>
        public NetOutgoingMessage GetOM(Command Type, SecondCommand SecondType = SecondCommand.Null)
        {
            var om = NetClient.CreateMessage();
            om.Write((byte)Type);
            om.Write((byte)SecondType);
            return om;
        }

        /// <summary>
        /// Called when the server sends a message
        /// </summary>
        private void MessageReceived(object peer)
        {
            NetIncomingMessage im;
            while (NetClient != null && (im = NetClient.ReadMessage()) != null)
            {
                string text;
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                        text = im.ReadString();
                        Output.Enqueue(text);
                        break;
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
#if DEBUG
                        text = im.ReadString();
                        //Output.Enqueue(text);
#endif
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        if (status == NetConnectionStatus.Connected)
                        {
                            string adress = im.SenderConnection.RemoteEndPoint.Address.ToString();
                            _Online = true;
                            Output.Enqueue("Connected to " + adress);
                            Output.Enqueue("Average roundtrip time is " + im.SenderConnection.AverageRoundtripTime.ToString() + " seconds");
                            Output.Enqueue("Connection status is " + im.SenderConnection.Status.ToString());
                            Output.Enqueue("Logged in as " + Username);
                        }
                        else if (status == NetConnectionStatus.Disconnected)
                        {
                            string Reason = im.ReadString();
                            Output.Enqueue("Disconnected with reason '" + Reason + "'");

                            // Fire Dc Event
                            Disconnected?.Invoke(this, Reason);

                            _Online = false;
                        }
                        break;
                    // Application data
                    case NetIncomingMessageType.Data:
                        AddPackage(im);
                        
                        break;
                    default:
                        string pmsg = im.ReadString();
                        Output.Enqueue("Unknown messagetype '" + im.MessageType.ToString() + "' (" + pmsg + ")");
                        break;

                }
            }
        }

        private void WatchPredictionProperties(GameObject gameObject)
        {
            if (PredictionData.ContainsKey(gameObject)) return;

            bool NeedsPrediction = false;

            foreach(var prop in gameObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var attrib = prop.GetCustomAttribute(typeof(OutpostOmega.Game.GameObjects.Attributes.SynchronizationAttr), true);
                if(attrib != null)
                {
                    var netAttrib = (OutpostOmega.Game.GameObjects.Attributes.SynchronizationAttr)attrib;

                    if(netAttrib.State == Game.GameObjects.Attributes.SynchronizeState.Prediction)
                    {
                        // Build prediction dictionary
                        if (!PredictionData.ContainsKey(gameObject))
                        {
                            if(!PredictionData.TryAdd(gameObject, new ConcurrentDictionary<PropertyInfo,ConcurrentQueue<TimedObjectState>>()))
                                throw new Exception("Shouldn't happen.. mhm");
                            if(!PredictionData[gameObject].TryAdd(prop, new ConcurrentQueue<TimedObjectState>()))
                                throw new Exception("Shouldn't happen.. mhm");
                        }

                        NeedsPrediction = true;
                    }
                }
            }
            if(NeedsPrediction)
                gameObject.PropertyChanged += GameObject_PropertyChanged;
        }

        void GameObject_PropertyChanged(GameObject sender, string PropertyName, bool ChildChange)
        {
            //Those are not interesting for networking. The parent will report the change already
            if (ChildChange) return;
            //return;
            if(PredictionData.ContainsKey(sender))
            {
                var property = sender.GetType().GetProperty(PropertyName);

                if (!PredictionData.ContainsKey(sender)) return;
                if (!PredictionData[sender].ContainsKey(property)) return;

                var states = PredictionData[sender][property].ToArray();

                object delta = null;
                if(states.Length == 0)
                    delta = GetDelta(property.GetValue(sender), property.GetValue(sender));
                else
                    delta = GetDelta(states[states.Length-1].Instance, property.GetValue(sender));

                if (delta == null)
                    return;
                    
                // Add new prediction step
                PredictionData[sender][property].Enqueue(new TimedObjectState()
                {
                    Time = Clock,
                    Instance = delta
                });

                //Clean up old data
                while (PredictionData[sender][property].TryPeek(out TimedObjectState oldestState) && oldestState.Time - Clock < 1) // We buffer max. 1 second
                {
                    PredictionData[sender][property].TryDequeue(out oldestState); // Dequeue it and ignore it because its too old
                }
            }
        }

        private void ModifyPredictedValue(object Instance, PropertyInfo Property, double Time, ref object Value)
        {
            if (!PredictionData.ContainsKey(Instance)) return;
            if (!PredictionData[Instance].ContainsKey(Property)) return;

            //Clean up data older then the current package
            while (PredictionData[Instance][Property].TryPeek(out TimedObjectState oldestState) && oldestState.Time < Time)
            {
                PredictionData[Instance][Property].TryDequeue(out oldestState); // Dequeue it and ignore it because its too old
            }

            /*if(PredictionData[Instance][Property].TryPeek(out oldestState))
                if(oldestState.Time - Time > 1)
                {

                }*/



            // Find closest historical entry for that specific property
            List<TimedObjectState> PropData;
            if(PredictionData[Instance][Property].Count > 0)
                PropData = PredictionData[Instance][Property].ToList();
            else
                PropData = new List<TimedObjectState>();

            var result = PropData.Select(p => new { Value = p, Difference = Math.Abs(p.Time - Time) })
                .OrderBy(p => p.Difference)
                .FirstOrDefault();

            if(result != null)
            {
                var ClosestData = result.Value;
                var Index = PropData.IndexOf(ClosestData);

                bool oldValues = false;
                for (int i = 0; i < PredictionData[Instance][Property].Count; i++)
                {
                    TimedObjectState peektos = new TimedObjectState();
                    if (PredictionData[Instance][Property].TryPeek(out peektos) && 
                        peektos.Time == ClosestData.Time && 
                        peektos.Instance == ClosestData.Instance)
                    {
                        oldValues = true;
                    }

                    if (!oldValues)
                    {
                        TimedObjectState tos = new TimedObjectState();
                        PredictionData[Instance][Property].TryDequeue(out tos); // Delete values, older then the last server message
                    }
                    else if (i > Index)
                        Value = Add(Value, PropData[i]);
                }
            }
        }

        private NetQueue<NetIncomingMessage> _packetQueue = new NetQueue<NetIncomingMessage>(25);
        private Thread _processThread;

        private void AddPackage(NetIncomingMessage im)
        {
            if (_processThread == null ||
                _processThread.ThreadState != ThreadState.Running)
            {
                _processThread = new Thread(ProcessPackageWorker)
                {
                    Priority = ThreadPriority.Lowest
                };
                //Output.Enqueue("Processthread started");
            }

            _packetQueue.Enqueue(im);

            if (_processThread.ThreadState == ThreadState.Unstarted)
            {
                try
                {
                    _processThread.Start();
                }
                catch { }
            }
        }

        private List<GameObject> _unprocessedObjects = new List<GameObject>();


        public long PacketsPerSecond { get; private set; }
        private long PacketCounter { get; set; }

        private DateTime PacketTime = DateTime.Now;

        /// <summary>
        /// Worker that starts package processing
        /// </summary>
        public void ProcessPackageWorker()
        {
            while (_packetQueue.Count > 0 && !Disposing)
            {
#if DEBUG
                ProcessNextPackage();
#else
                try
                {
                    ProcessNextPackage();
                }
                catch(Exception e)
                {
                    new OutpostOmega.Error.CrashReport(e);
                }
#endif
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Processes the next package in the packet queue
        /// </summary>
        private void ProcessNextPackage()
        {
            if (_packetQueue.TryDequeue(out NetIncomingMessage im))
            {
                if (im == null)
                    return;

                var Type = im.ReadByte();
                var subType = im.ReadByte();
                switch (Type)
                {
                    case (byte)Command.Create: // New object received
                        object obj = null;
#if DEBUG
                        obj = NetworkHandler.ReadSerializedData(im.ReadBytes(im.ReadInt32()));
#else
                        try
                        {
                            obj = NetworkHandler.ReadSerializedData(im.ReadBytes(im.ReadInt32()));
                        }
                        catch (Exception e)
                        {
                            Output.Enqueue("ERROR: " + e.Message + e.Source);
                        }
#endif
                        if (obj == null)
#if DEBUG
                            throw new Exception("Could not serialize that shit");
#else
                            Output.Enqueue("ERROR: New object received but unable to read it");
#endif

                        // World
                        if (obj.GetType() == typeof(World))
                        {
                            this.World = (World)obj;
                            foreach (GameObject gO in _unprocessedObjects)
                                this.World.Add(gO);
                            _unprocessedObjects.Clear();

                            foreach (var gameObject in this.World.AllGameObjects)
                                WatchPredictionProperties(gameObject);
                        }

                        // Gameobjects
                        if (obj.GetType().IsSubclassOf(typeof(GameObject)))
                        {
                            if (this.World != null)
                                this.World.Add((GameObject)obj);
                            else
                                _unprocessedObjects.Add((GameObject)obj);

                            //WatchPredictionProperties((GameObject)obj);
                        }
                        break;
                    case (byte)Command.Delete:
                        string id = im.ReadString();
                        var delobject = (from gobject in World.AllGameObjects
                                         where gobject.ID == id
                                         select gobject).FirstOrDefault();
                        if (delobject != null)
                            delobject.Dispose();

                        break;
                    // Data update
                    case (byte)Command.Data:
                        switch (subType)
                        {
                            case (byte)SecondCommand.CreateBlock:

                                break;
                            case (byte)SecondCommand.RemoveBlock:

                                break;
                            case (byte)SecondCommand.GameObject:
                                if (World != null)
                                {
                                    var gameObject_ID = im.ReadString();
                                    var property_name = im.ReadString();
                                    if (property_name == "TargetGameObject")
                                    { }

                                    var gameObject = World.GetGameObject(gameObject_ID);
                                    System.Reflection.PropertyInfo property = null;
                                    if (gameObject != null)
                                        property = gameObject.GetType().GetProperty(property_name);


                                    if (property == null)
                                        return; //Nothing to do then


                                    bool Predict = false;
                                    double SendTime = 0;
                                    var attribute = property.GetCustomAttribute(typeof(OutpostOmega.Game.GameObjects.Attributes.SynchronizationAttr), true);
                                    if (attribute != null && ((OutpostOmega.Game.GameObjects.Attributes.SynchronizationAttr)attribute).State == Game.GameObjects.Attributes.SynchronizeState.Prediction)
                                    {
                                        SendTime = im.ReadDouble();
                                        Predict = true;
                                    }


                                    if (!SpecialDeserialize(im, property, out object new_value))
                                    {
                                        var data = im.ReadBytes(im.ReadInt32());
                                        new_value = NetworkHandler.ReadSerializedData(data);
                                    }


                                    object oldvalue = null;
                                    if (gameObject != null)
                                    {
                                        oldvalue = property.GetValue(gameObject);
                                        if (Predict)
                                            ModifyPredictedValue(gameObject, property, SendTime, ref new_value);


                                        property.SetValue(gameObject, new_value, null);
                                    }
                                }

                                break;
                        }
                        break;
                    case (byte)Command.Message: // Message
                        var message = im.ReadString();
                        Output.Enqueue("Server: " + message);
                        break;
                    default:
                        Output.Enqueue("Unknown command received. Type: " + Type.ToString() + " Subtype: " + subType.ToString());
                        break;
                }
            }

            PacketCounter++;
        }

        public bool Disposing = false;
        public void Dispose()
        {
            Disposing = true;
            this.Disconnect("Client closing");
            _packetQueue.Clear();
            PredictionData.Clear();
            Output.Clear();
        }
         /// <summary>
        /// Optimized way of transfering very basic objects. Everything else gets handled by the default compressed xml serialization
        /// </summary>
        private static bool SpecialDeserialize(Lidgren.Network.NetIncomingMessage im, System.Reflection.PropertyInfo property, out object data)
        {
            data = null;
            if(property.PropertyType == typeof(Jitter.LinearMath.JMatrix))
                        data = new Jitter.LinearMath.JMatrix()
                        {
                            M11 = im.ReadFloat(),
                            M12 = im.ReadFloat(),
                            M13 = im.ReadFloat(),

                            M21 = im.ReadFloat(),
                            M22 = im.ReadFloat(),
                            M23 = im.ReadFloat(),

                            M31 = im.ReadFloat(),
                            M32 = im.ReadFloat(),
                            M33 = im.ReadFloat()
                        };

            if(property.PropertyType == typeof(Jitter.LinearMath.JVector))
                        data = new Jitter.LinearMath.JVector()
                        {
                            X = im.ReadFloat(),
                            Y = im.ReadFloat(),
                            Z = im.ReadFloat(),
                        };

            if(property.PropertyType == typeof(Jitter.LinearMath.JVector2))
                        data = new Jitter.LinearMath.JVector2()
                        {
                            X = im.ReadFloat(),
                            Y = im.ReadFloat(),
                        };
            
            if(property.PropertyType == typeof(string))
                        data = im.ReadString();
            if(property.PropertyType == typeof(short))
                        data = im.ReadInt16();
            if(property.PropertyType == typeof(int))
                        data = im.ReadInt32();
            if(property.PropertyType == typeof(long))
                        data = im.ReadInt64();
            if(property.PropertyType == typeof(bool))
                        data = im.ReadBoolean();
            if(property.PropertyType == typeof(double))
                        data = im.ReadDouble();
            if(property.PropertyType == typeof(float))
                        data = im.ReadFloat();

            if (data == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Tries to compute a delta out of two unknown objects
        /// </summary>
        private static object GetDelta(object oldValue, object newValue)
        {
            if (oldValue == null)
                return newValue;

            if (oldValue == null || newValue == null || 
                oldValue.GetType() != newValue.GetType()) return null;


            object Delta = null;

            var Type = oldValue.GetType();

            if (Type == typeof(float))
                Delta = (float)newValue - (float)oldValue;
            else if (Type == typeof(decimal))
                Delta = (decimal)newValue - (decimal)oldValue;
            else if (Type == typeof(short))
                Delta = (short)newValue - (short)oldValue;
            else if (Type == typeof(int))
                Delta = (int)newValue - (int)oldValue;
            else if (Type == typeof(long))
                Delta = (long)newValue - (long)oldValue;
            else if (Type == typeof(Jitter.LinearMath.JVector))
            {
                var oldVec = (Jitter.LinearMath.JVector)oldValue;
                var newVec = (Jitter.LinearMath.JVector)newValue;
                Delta = new Jitter.LinearMath.JVector(
                    (float)GetDelta(oldVec.X, newVec.X),
                    (float)GetDelta(oldVec.Y, newVec.Y),
                    (float)GetDelta(oldVec.Z, newVec.Z));
            }
            else if (Type == typeof(Jitter.LinearMath.JVector2))
            {
                var oldVec = (Jitter.LinearMath.JVector2)oldValue;
                var newVec = (Jitter.LinearMath.JVector2)newValue;
                Delta = new Jitter.LinearMath.JVector2(
                    (float)GetDelta(oldVec.X, newVec.X),
                    (float)GetDelta(oldVec.Y, newVec.Y));
            }
            else if (Type == typeof(Jitter.LinearMath.JMatrix))
            {
                var oldMat = (Jitter.LinearMath.JMatrix)oldValue;
                var newMat = (Jitter.LinearMath.JMatrix)newValue;
                Delta = new Jitter.LinearMath.JMatrix()
                    {
                        M11 = (float)GetDelta(oldMat.M11, oldMat.M11),
                        M12 = (float)GetDelta(oldMat.M12, oldMat.M12),
                        M13 = (float)GetDelta(oldMat.M13, oldMat.M13),

                        M21 = (float)GetDelta(oldMat.M21, oldMat.M21),
                        M22 = (float)GetDelta(oldMat.M22, oldMat.M22),
                        M23 = (float)GetDelta(oldMat.M23, oldMat.M23),

                        M31 = (float)GetDelta(oldMat.M31, oldMat.M31),
                        M32 = (float)GetDelta(oldMat.M32, oldMat.M32),
                        M33 = (float)GetDelta(oldMat.M33, oldMat.M33),

                    };
            }


            return Delta;
        }
        
        /// <summary>
        /// Tries to compute a addition of two unknown objects
        /// </summary>
        private static object Add(object Value1, object Value2)
        {
            if (Value1 == null || Value2 == null || Value1.GetType() != Value2.GetType()) return null;


            object NewValue = null;

            var Type = Value1.GetType();

            if (Type == typeof(float))
                NewValue = (float)Value2 + (float)Value1;
            else if (Type == typeof(decimal))
                NewValue = (decimal)Value2 + (decimal)Value1;
            else if (Type == typeof(short))
                NewValue = (short)Value2 + (short)Value1;
            else if (Type == typeof(int))
                NewValue = (int)Value2 + (int)Value1;
            else if (Type == typeof(long))
                NewValue = (long)Value2 + (long)Value1;
            else if (Type == typeof(Jitter.LinearMath.JVector))
            {
                var oldVec = (Jitter.LinearMath.JVector)Value1;
                var newVec = (Jitter.LinearMath.JVector)Value2;
                NewValue = new Jitter.LinearMath.JVector(
                    (float)Add(oldVec.X, newVec.X),
                    (float)Add(oldVec.Y, newVec.Y),
                    (float)Add(oldVec.Z, newVec.Z));
            }
            else if (Type == typeof(Jitter.LinearMath.JVector2))
            {
                var oldVec = (Jitter.LinearMath.JVector2)Value1;
                var newVec = (Jitter.LinearMath.JVector2)Value2;
                NewValue = new Jitter.LinearMath.JVector2(
                    (float)Add(oldVec.X, newVec.X),
                    (float)Add(oldVec.Y, newVec.Y));
            }
            else if (Type == typeof(Jitter.LinearMath.JMatrix))
            {
                var oldMat = (Jitter.LinearMath.JMatrix)Value1;
                var newMat = (Jitter.LinearMath.JMatrix)Value2;
                NewValue = new Jitter.LinearMath.JMatrix()
                {
                    M11 = (float)Add(oldMat.M11, oldMat.M11),
                    M12 = (float)Add(oldMat.M12, oldMat.M12),
                    M13 = (float)Add(oldMat.M13, oldMat.M13),

                    M21 = (float)Add(oldMat.M21, oldMat.M21),
                    M22 = (float)Add(oldMat.M22, oldMat.M22),
                    M23 = (float)Add(oldMat.M23, oldMat.M23),

                    M31 = (float)Add(oldMat.M31, oldMat.M31),
                    M32 = (float)Add(oldMat.M32, oldMat.M32),
                    M33 = (float)Add(oldMat.M33, oldMat.M33),

                };
            }

            return NewValue;
        }
    }
    
       
}
