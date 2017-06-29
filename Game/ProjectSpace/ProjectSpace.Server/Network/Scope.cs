using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using OutpostOmega.Game.GameObjects.Mobs;
using OutpostOmega.Game;
using OutpostOmega.Game.Turf;
using System.Threading;
using Lidgren.Network;

namespace OutpostOmega.Server.Network
{
    /// <summary>
    /// Used to manage a clients "scope" (the area that a client should be interested)
    /// This will determine the clients interest radius (more a area later) and watch everything inside of this area
    /// </summary>
    public class Scope : IDisposable
    {
        public float Range = 20;

        /// <summary>
        /// Determine if this scope needs an update in the next loop
        /// </summary>
        public Boolean NeedsUpdate { get; set; }

        private Client _client { get; set; }

        private List<GameObject> _ObjectScope;

        public Scope(Client client)
        {
            this._client = client;
            this._ObjectScope = new List<GameObject>();
            this.NeedsUpdate = true;

            _client.Host.networkHandler.World.GameObjectRemoved += World_GameObjectRemoved;
            _client.Host.networkHandler.World.NewGameObject += World_NewGameObject;
            _client.Host.networkHandler.World.Structures[0].Changed += StructureChanged;

            //_client.Host.networkHandler.World.Structures[0].newChunk += Scope_newChunk;
        }

        private void StructureChanged(Structure sender, JVector Position, Block block, bool Added)
        {
            var outgoingMessage = _client.GetOM(
                OutpostOmega.Network.Command.Data,                 
                Added ? OutpostOmega.Network.SecondCommand.CreateBlock : OutpostOmega.Network.SecondCommand.RemoveBlock);

            //outgoingMessage.Write(_client.Host.networkHandler.GetObjectData(_client.ID, block));

            outgoingMessage.Write(block.Type);
            outgoingMessage.Write(Position.X);
            outgoingMessage.Write(Position.Y);
            outgoingMessage.Write(Position.Z);
        }

        /*void Scope_newChunk(Chunk newChunk)
        {
            if (Disposing || Idling) return;

            var outgoingMessage = _client.GetOM(OutpostOmega.Network.Command.Create, OutpostOmega.Network.SecondCommand.Chunk);
            outgoingMessage.Write(_client.Host.networkHandler.GetObjectData(_client.ID, newChunk));
        }*/

        void World_NewGameObject(GameObject newGameObject)
        {
            if (Disposing || Idling || GameObjectNeeded(newGameObject)) return;

            

            CreateRemoteGameObject(newGameObject);
            newGameObject.PropertyChanged += gObject_PropertyChanged;
            _ObjectScope.Add(newGameObject);
        }

        void World_GameObjectRemoved(GameObject removedGameObject)
        {
            if (Disposing || Idling || GameObjectNeeded(removedGameObject)) return;

            DeleteRemoteGameObject(removedGameObject);
            removedGameObject.PropertyChanged -= gObject_PropertyChanged;
            _ObjectScope.Remove(removedGameObject);
        }

        /// <summary>
        /// Checks if the given gameobject is relevant for the player
        /// Filters out stuff like the player and his mob
        /// </summary>
        private bool GameObjectNeeded(GameObject gameObject)
        {
            if (typeof(Game.GameObjects.Mob).IsAssignableFrom(gameObject.GetType()))
                return ((Game.GameObjects.Mob)gameObject).Mind != this._client.Mind;
            if (typeof(Game.GameObjects.Mobs.Mind).IsAssignableFrom(gameObject.GetType()))
                return ((Game.GameObjects.Mobs.Mind)gameObject) != this._client.Mind;
            if (typeof(Game.GameObjects.Mobs.View).IsAssignableFrom(gameObject.GetType()))
                return ((Game.GameObjects.Mobs.View)gameObject) != this._client.Mind.Mob.View;

            return true;
        }

        private bool _firstUpdate = true;

        /// <summary>
        /// Updates the scope
        /// </summary>
        public void Update()
        {
            if (Disposing || Idling) return;

            if (_client.Mind == null)
                return;

            JVector position = _client.Mind.Position;

            // First update? Send the general world data first
            if (_firstUpdate)
            {
                SendInitialData();
                _firstUpdate = false;
            }
                

            //Check for gObjects that need to be added or removed from the scopeList
            /*foreach(gameObject gObject in _client.Host.networkHandler.World.GameObjects)
            {
                float distance = (position - gObject.Position).Length();

                //Check if the gObject is inside of our Sphere
                if (distance <= Range && distance >= Range * -1)
                {
                    if (!_ObjectScope.Contains(gObject))
                    {
                        CreateRemoteGameObject(gObject);
                        gObject.PropertyChanged += gObject_PropertyChanged;
                        _ObjectScope.Add(gObject);
                    }
                }
                else if(gObject.GetType().IsSubclassOf(typeof(mob)))
                {
                    if (!_ObjectScope.Contains(gObject))
                    {
                        CreateRemoteGameObject(gObject);
                        gObject.PropertyChanged += gObject_PropertyChanged;
                        _ObjectScope.Add(gObject);
                    }
                }
                else
                    if (_ObjectScope.Contains(gObject))
                    {
                        DeleteRemoteGameObject(gObject);
                        gObject.PropertyChanged -= gObject_PropertyChanged;
                        _ObjectScope.Remove(gObject);
                    }
            }

            //Check for chunks that need to be added or removed from the scopeList
            foreach(Structure structure in _client.Host.networkHandler.World.Structures)
                foreach(Chunk chunk in structure.chunks)
                {
                    if (chunk.bounds.Contains(position) == JBBox.ContainmentType.Contains)
                    {
                        if (!_ChunkScope.Contains(chunk))
                        {
                            _ChunkScope.Add(chunk);
                            var outgoingMessage = _client.GetOM(OutpostOmega.Network.Command.Create, OutpostOmega.Network.SecondCommand.Chunk);
                            outgoingMessage.Write(_client.Host.networkHandler.GetObjectData(_client.ID, chunk));
                        }
                    }
                    else
                        if (_ChunkScope.Contains(chunk))
                        {
                            _ChunkScope.Remove(chunk);
                            var outgoingMessage = _client.GetOM(OutpostOmega.Network.Command.Delete, OutpostOmega.Network.SecondCommand.Chunk);
                            outgoingMessage.Write(structure.ID);
                            outgoingMessage.Write(chunk.ID);
                        }
                }*/

            this.NeedsUpdate = false;
        }

        public static int PacketsSent = 0;
        public void FlushMessageQueue()
        {
            if (Disposing || Idling) return;

            lock(PropertyQueue)
            {
                var c = PropertyQueue.Count;
                while (c > 0)
                {
                    var pchange = PropertyQueue.Dequeue(); c--;
                    if (pchange.Instance == null) continue;

                    var gObject = (GameObject)pchange.Instance;

                    var om = _client.GetOM(OutpostOmega.Network.Command.Data, OutpostOmega.Network.SecondCommand.GameObject);

                    // Get property value
                    var value = pchange.Property.GetValue(gObject);

                    if (gObject.GetType().Name == "Mesh")
                    { }


                    om.Write(gObject.ID);
                    om.Write(pchange.Property.Name);

                    
                    var Attr = (from attr in pchange.Property.GetCustomAttributes(true)
                                       where attr.GetType() == typeof(OutpostOmega.Game.GameObjects.Attributes.SynchronizationAttr)
                                       select attr).FirstOrDefault();
                    OutpostOmega.Game.GameObjects.Attributes.SynchronizationAttr NetAttr = null;
                    if (Attr != null)
                    {
                        NetAttr = (OutpostOmega.Game.GameObjects.Attributes.SynchronizationAttr)Attr;

                        if (NetAttr.State == Game.GameObjects.Attributes.SynchronizeState.Prediction)
                            om.Write(_client.LastReceivedMouseTime); // Attach timestamp if prediction enabled
                    }


                    // Serialize value
                    if (!SpecialSerialize(value, ref om))
                    {
                        var data = _client.Host.networkHandler.GetObjectData(_client.ID, value);

                        // Send data
                        om.Write(data.Length);
                        om.Write(data);
                    }
                    

                    if (NetAttr != null)
                    {


                        if(NetAttr.Priority != Game.GameObjects.Attributes.SynchronizePriority.NoSynchronization)
                            _client.Connection.SendMessage(om, (Lidgren.Network.NetDeliveryMethod)(int)NetAttr.Priority, 1);
                    }
                    else
                        _client.Connection.SendMessage(om, Lidgren.Network.NetDeliveryMethod.ReliableOrdered, 1);

                    PacketsSent++;
                }
                PropertyQueue.Clear();
            }
        }

        private void SendInitialData()
        {

            var blankWorld = _client.Host.networkHandler.World;
            var data = _client.Host.networkHandler.GetObjectData(_client.ID, blankWorld);

            //Send message
            _client.SendMessage(String.Format("Transfering world '{0}' ({1} bytes)", blankWorld.ID, data.Length), 1);

            //Send world afterwards
            var om = _client.GetOM(OutpostOmega.Network.Command.Create, OutpostOmega.Network.SecondCommand.Null);
            om.Write(data.Length);
            om.Write(data);
            _client.Connection.SendMessage(om, Lidgren.Network.NetDeliveryMethod.ReliableOrdered, 1);

            foreach (GameObject gObject in _client.Host.networkHandler.World.AllGameObjects)
            {
                gObject.PropertyChanged += gObject_PropertyChanged;
            }
        }

        private void CreateRemoteGameObject(GameObject GameObject)
        {
            if (Disposing || Idling) return;

            var om = _client.GetOM(OutpostOmega.Network.Command.Create, OutpostOmega.Network.SecondCommand.GameObject);
            var data = _client.Host.networkHandler.GetObjectData(_client.ID, GameObject);
            om.Write(data.Length);
            om.Write(data);
            _client.Connection.SendMessage(om, Lidgren.Network.NetDeliveryMethod.ReliableOrdered, 1);
        }

        private void DeleteRemoteGameObject(GameObject GameObject)
        {
            if (Disposing || Idling) return;

            var om = _client.GetOM(OutpostOmega.Network.Command.Delete, OutpostOmega.Network.SecondCommand.GameObject);
            om.Write(GameObject.ID);
            _client.Connection.SendMessage(om, Lidgren.Network.NetDeliveryMethod.ReliableOrdered, 1);
        }

        private struct PropertyChange
        {
            public System.Reflection.PropertyInfo Property;
            public object Instance;
        }

        private Queue<PropertyChange> PropertyQueue = new Queue<PropertyChange>();

        /// <summary>
        /// Used to send gameObject properties to the client
        /// </summary>
        void gObject_PropertyChanged(GameObject sender, string PropertyName, bool MinorRaise)
        {
            if (Disposing || Idling) return;

            // This change was raised by a gameObject because its parent has changed. Not directly necessary for networking
            if (MinorRaise) return;

            var gObject = (GameObject)sender;
            var propInfo = gObject.GetType().GetProperty(PropertyName);

            if (propInfo == null)
            {
                Main.Message("Error: Unknown propertychange detected '" + PropertyName + "'", System.Drawing.Color.Red);
                return;
            }

            bool found = false;

            PropertyChange[] queue = null;
            lock(PropertyQueue)
            {
                if (PropertyQueue.Count > 0)
                {
                    queue = PropertyQueue.ToArray();
                }
            }

            if(queue != null)
            {
                for (int i = 0; i < queue.Length; i++)
                {
                    if (queue[i].Instance == sender && queue[i].Property == propInfo)
                        found = true;
                }
            }
            if (!found)
                lock(PropertyQueue)
                {
                    PropertyQueue.Enqueue(new PropertyChange() { Property = propInfo, Instance = sender });
                }

        }

        /// <summary>
        /// Optimized way of transfering very basic objects. Everything else gets handled by the default compressed xml serialization
        /// </summary>
        private static bool SpecialSerialize(object data, ref Lidgren.Network.NetOutgoingMessage om)
        {
            if (data == null) return false;

            bool Serialized = false;

            if(data.GetType() == typeof(Jitter.LinearMath.JMatrix))
            {
                var matrix = (Jitter.LinearMath.JMatrix)data;

                //om.Write((ushort)OutpostOmega.Network.SpecialSerializeTypes.JMatrix);

                om.Write(matrix.M11);
                om.Write(matrix.M12);
                om.Write(matrix.M13);

                om.Write(matrix.M21);
                om.Write(matrix.M22);
                om.Write(matrix.M23);

                om.Write(matrix.M31);
                om.Write(matrix.M32);
                om.Write(matrix.M33);

                Serialized = true;
            }

            if (data.GetType() == typeof(Jitter.LinearMath.JVector2))
            {
                var vector = (Jitter.LinearMath.JVector2)data;

                //om.Write((ushort)OutpostOmega.Network.SpecialSerializeTypes.JVector2);

                om.Write(vector.X);
                om.Write(vector.Y);

                Serialized = true;
            }

            if (data.GetType() == typeof(Jitter.LinearMath.JVector))
            {
                var vector = (Jitter.LinearMath.JVector)data;

                //    om.Write((ushort)OutpostOmega.Network.SpecialSerializeTypes.JVector);

                om.Write(vector.X);
                om.Write(vector.Y);
                om.Write(vector.Z);

                Serialized = true;
            }
            if (data.GetType() == typeof(string))
            {
                om.Write((string)data);
                Serialized = true;
            }
            if (data.GetType() == typeof(short))
            {
                om.Write((short)data);
                Serialized = true;
            }
            if (data.GetType() == typeof(int))
            {
                om.Write((int)data);
                Serialized = true;
            }
            if (data.GetType() == typeof(long))
            {
                om.Write((long)data);
                Serialized = true;
            }
            if (data.GetType() == typeof(bool))
            {
                om.Write((bool)data);
                Serialized = true;
            }
            if (data.GetType() == typeof(double))
            {
                om.Write((double)data);
                Serialized = true;
            }
            if (data.GetType() == typeof(float))
            {
                om.Write((float)data);
                Serialized = true;
            }
            if(!Serialized)
            { }
            return Serialized;
        }

        public bool Idling { get; protected set; }
        public void Idle()
        {
            Idling = true; // Prevents processing

            _client.Host.networkHandler.World.GameObjectRemoved -= World_GameObjectRemoved;
            _client.Host.networkHandler.World.NewGameObject -= World_NewGameObject;
            _client.Host.networkHandler.World.Structures[0].Changed -= StructureChanged;

            //_client.Host.networkHandler.World.Structures[0].newChunk -= Scope_newChunk;

            foreach (var gameObject in _ObjectScope)
                gameObject.PropertyChanged -= gObject_PropertyChanged;
        }
        public void Restore(bool NewClient)
        {
            if (Idling) // Only allowed if we were idling
            {
                Idling = false; // Allowes processing again

                if (NewClient)
                {
                    _firstUpdate = true;
                    NeedsUpdate = true;
                }

                _client.Host.networkHandler.World.GameObjectRemoved += World_GameObjectRemoved;
                _client.Host.networkHandler.World.NewGameObject += World_NewGameObject;
                //_client.Host.networkHandler.World.Structures[0].newChunk += Scope_newChunk;
                _client.Host.networkHandler.World.Structures[0].Changed += StructureChanged;

                foreach (var gameObject in _ObjectScope)
                    gameObject.PropertyChanged += gObject_PropertyChanged;
            }
        }

        public bool Disposing { get; set; }
        public void Dispose()
        {
            Disposing = true;
            Idle(); // Idle
            _ObjectScope.Clear(); // Aaaand release all data
        }
    }
}
