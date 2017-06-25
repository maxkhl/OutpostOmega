using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Jitter.Collision;
using Jitter.LinearMath;
using OpenTK.Input;
using System.Runtime.CompilerServices;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Contains informations about the current loaded World
    /// </summary>
    public partial class World : IDisposable
    {
        /// <summary>
        /// ID of this world
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Every GameObject in this World
        /// </summary>
        public List<GameObject> AllGameObjects { get; set; }

        public Content.ContentManager ContentManager { get; set; }

        public List<datums.datum> Datums { get; set; }

        public Dictionary<string, string> IDCounter { get; set; }

        public Queue<string> DebugMessages = new Queue<string>();

        /// <summary>
        /// The player that is playing the game. This mind will receive all the input. All other minds are AI or network-controlled
        /// </summary>
        public GameObjects.Mobs.Minds.PlayerTypes.LocalPlayer Player 
        { 
            get
            {
                return _Player;
            }
            set
            {
                if (_Player != null)
                {
                    _Player.Mob.Visible = true;
                }

                _Player = value;
                if (_Player != null)
                {
                    //_Player.Mob.Visible = false; //Hide the player
                }
            }
        }
        private GameObjects.Mobs.Minds.PlayerTypes.LocalPlayer _Player = null;

        /// <summary>
        /// Every structure in this world
        /// </summary>
        public ObservableCollection<turf.Structure> Structures { get; set; }

        /// <summary>
        /// Debug mode enables several options for debugging and debug-drawing
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Client-(stupid-)mode. The gameworld does only very basic simulations (for prediction and stuff). Almost none logic is being processed. A world in this mode should be updated by a network client object
        /// </summary>
        public bool ClientMode { get; set; }

        /// <summary>
        /// Handles all bodies and constraints
        /// </summary>
        [GameObjects.Attributes.Serialize(GameObjects.Attributes.SerializeState.DoNotSerialize)]
        public Jitter.World PhysicSystem { get; set; }

        /// <summary>
        /// Atmospherics system
        /// </summary>
        [GameObjects.Attributes.Serialize(GameObjects.Attributes.SerializeState.DoNotSerialize)]
        public GTPS.GTPS AtmosSystem { get; set; }

        /// <summary>
        /// Console for LUA parsing and I/O stuffz
        /// </summary>
        public Console Console { get; set; }

        /// <summary>
        /// Default settings for this world
        /// </summary>
        public WorldSettings Settings { get; set; }

        public World(string Name = "Generic World")
        {
            this.ID = Name;
            AllGameObjects = new List<GameObject>();
            Datums = new List<datums.datum>();
            //var type = gameObject.Generate("Tehest", new Action<object, MouseState, KeyboardState, double>(UpdateTest));
            Structures = new ObservableCollection<turf.Structure>();

            IDCounter = new Dictionary<string, string>();

            PhysicSystem = new Jitter.World(new CollisionSystemSAP());
            PhysicSystem.Gravity = new JVector(0, -5, 0);

            AtmosSystem = new GTPS.GTPS(this);
            AtmosSystem.Start();

            Settings = new WorldSettings()
            {
                DefaultUserGroup = datums.UserGroup.User,
            };

            ContentManager = new Content.ContentManager();

            Console = new Console(this);

            Mods = new List<Lua.ModPack>();

            //PhysicSystem.SetDampingFactors(0.5f, 0.5f);
            //PhysicSystem.ContactSettings.MaterialCoefficientMixing = Jitter.Dynamics.ContactSettings.MaterialCoefficientMixingType.TakeMaximum;
            //PhysicSystem.ContactSettings.MaterialCoefficientMixing = Jitter.Dynamics.ContactSettings.MaterialCoefficientMixingType.TakeMaximum;
            //PhysicSystem.ContactSettings.MaterialCoefficientMixing = Jitter.Dynamics.ContactSettings.MaterialCoefficientMixingType.TakeMinimum;            
        }
        static public void UpdateTest(object sender, MouseState mouseState = new MouseState(), KeyboardState keyboardState = new KeyboardState(), double ElapsedTime = 0)
        {
            UpdateTest(sender, mouseState, keyboardState, ElapsedTime);
        }

        /// <summary>
        /// Stops asynchronus operations in this world. (f.e. atmos system)
        /// </summary>
        public void Stop()
        {
            AtmosSystem.Stop();
        }


        /// <summary>
        /// Updates the physics and all GameObjects (should be called every frame)
        /// </summary>
        public void Update(Tools.KeybeardState keyboardState, Tools.MouseState mouseState, double ElapsedTime = 0)
        {
            //PhysicSystem.Step((float)ElapsedTime, false);
           // ContentManager.Update();
            if (!ClientMode)
            {
                PhysicSystem.Step((float)ElapsedTime, false);
                ContentManager.Update();
                // Datums
                for (int i = 0; i < Datums.Count; i++)
                    if (Datums[i].NeedsUpdate)
                        Datums[i].Update();

                // GameObjects
                var Priorities = Enum.GetValues(typeof(GameObject.ProcessPriority));

                foreach (object priority in Priorities)
                {
                    for (int i = 0; i < AllGameObjects.Count; i++)
                    {
                        if (AllGameObjects[i].Priority != (GameObject.ProcessPriority)priority) continue;

                        AllGameObjects[i].Update(ElapsedTime);
                    }
                }

                // Structures
                for (int i = 0; i < Structures.Count; i++)
                    Structures[i].Update();
            }
            else
            {
                var kState = keyboardState;
                var mState = mouseState;
                Player.Update(ElapsedTime);
                Player.Mob.Update((float)ElapsedTime);
                Player.Mob.View.Update(ElapsedTime);
//Player.Update(mouseState, keyboardState, ElapsedTime);
                //Player.Mob.Update(mouseState, keyboardState, ElapsedTime);
                //Player.Mob.View.Update(mouseState, keyboardState, ElapsedTime);
            }
        }

        /// <summary>
        /// Used to recognize global keys (like wasd or escape), react and finaly forbid usage for ingame objects
        /// </summary>
        /// <returns>Key is globaly assigned = true</returns>
        private bool CatchGlobalKey(OpenTK.Input.Key Key)
        {
            bool IsGlobalKey = false;

            if (Key == OpenTK.Input.Key.O)
                this.Debug = !this.Debug;

            //if (Key == OpenTK.Input.Key.W)
            //{
            //    var mVec = Player.Mob.View.Forward;
            //    mVec.Y = 0;
            //    if (Player != null && Player.Mob != null && Player.Mob.View != null)
            //    {                    
            //        Player.Mob.charController.TargetVelocity = mVec*50;
            //        //Player.Mob.PhysicSetPosition(Player.Mob.Position + mVec);
            //        //Player.Mob.PhysicApplyForce(mVec, 200);
            //    }

            //    IsGlobalKey = true;
            //}
            //if (Key == OpenTK.Input.Key.Space)
            //{
            //    var mVec = JVector.Up;
            //    if (Player != null && Player.Mob != null && Player.Mob.View != null)
            //        Player.Mob.charController.TryJump = true;

            //    IsGlobalKey = true;
            //}

            return IsGlobalKey;
        }

        public void Add(GameObject gameObject)
        {
            AllGameObjects.Add(gameObject);

            if (NewGameObject != null)
                NewGameObject(gameObject);
        }

        public void Remove(GameObject gameObject)
        {
            if (AllGameObjects.Contains(gameObject))
            {
                AllGameObjects.Remove(gameObject);

                if (GameObjectRemoved != null)
                    GameObjectRemoved(gameObject);
            }
        }

        public GameObject GetGameObject(string ID)
        {
            return (from gObject in AllGameObjects
                    where gObject.ID == ID
                    select gObject).FirstOrDefault();
        }
        public GameObject GetGameObject(Jitter.Dynamics.RigidBody RigidBody)
        {
            if (RigidBody == null)
                return null;

            return (from gObject in AllGameObjects
                    where gObject.RigidBody == RigidBody
                    select gObject).FirstOrDefault();
        }

        public GameObject[] GetGameObjects(Type type)
        {
            return (from gObject in AllGameObjects
                    where gObject.GetType() == type
                    select gObject).ToArray();
        }
        public T[] GetGameObjects<T>()
        {
            return (from gObject in AllGameObjects
                    where gObject is T
                    select (T)(object)gObject).ToArray();
        }
        public turf.Structure GetStructure(Jitter.Dynamics.RigidBody RigidBody)
        {
            if (RigidBody == null)
                return null;

            foreach (turf.Structure structure in Structures)
            {
                var targetChunk = (from chunk in structure.chunks
                                   where chunk.rigidBody == RigidBody
                                   select chunk).FirstOrDefault();

                if (targetChunk != null)
                    return structure;
            }
            return null;
        }
        public void ResendEvents()
        {
            if (NewGameObject != null)
                foreach (GameObject gobject in AllGameObjects)
                    NewGameObject(gobject);

            var tempList = new List<turf.Structure>(Structures);
            Structures.Clear();

            foreach (turf.Structure structure in tempList)
            {
                Structures.Add(structure);
                structure.ResendEvents();
            }
        }

        /// <summary>
        /// Checks for chunks that need to be rendered and renders them
        /// </summary>
        public void UpdateChunkRender()
        {
            foreach (var structure in Structures)
                foreach (var chunk in structure.chunks)
                    if (chunk.NeedsRender)
                        chunk.Render();
        }

        /// <summary>
        /// Used to modify gas
        /// </summary>
        /// <param name="Position">Position where the gas should be modified</param>
        /// <param name="GasID">ID of the gas</param>
        /// <param name="Amount">Amount of the gas</param>
        public void Gas(JVector Position, byte GasID, short Amount)
        {
            var block = Structures[0][Position.X, Position.Y, Position.Z];
            turf.Block.ModGas(ref block, GasID, Amount);
            Structures[0][Position.X, Position.Y, Position.Z] = block;
        }


        /// <summary>
        /// Creates a blank world out of this world. This means the copy will have all the attributes but no gameObjects and structures
        /// </summary>
        /// <returns>Blank world</returns>
        public World GetBlankCopy()
        {
            return new World(this.ID);
        }

        public void DebugPhysics(Jitter.IDebugDrawer debugDrawer)
        {
            foreach (Jitter.Dynamics.RigidBody rigidBody in PhysicSystem.RigidBodies)
                rigidBody.DebugDraw(debugDrawer);
        }

        public delegate void NewGameObjectHandler(GameObject newGameObject);
        public event NewGameObjectHandler NewGameObject;

        public delegate void GameObjectRemovedHandler(GameObject removedGameObject);
        public event GameObjectRemovedHandler GameObjectRemoved;

        /// <summary>
        /// Sends a request to the UI
        /// </summary>
        public void CallUI(GameObject sender, UICommand command, object data)
        {
            if (UICall != null)
                UICall(sender, command, data);
#if DEBUG
            //else
                //throw new Exception("UI call triggered without connected frontend");
#endif
        }

        public delegate void UICallHandler(GameObject sender, UICommand command, object data);
        public event UICallHandler UICall;

        /// <summary>
        /// Determins if object is currently in a disposal process
        /// </summary>
        [GameObjects.Attributes.Serialize(GameObjects.Attributes.SerializeState.DoNotSerialize)]
        public bool Disposing { get; set; }
        public void Dispose()
        {
            Disposing = true;

            PhysicSystem.Clear();


            for(int i = 0; i < AllGameObjects.Count; i++)
                AllGameObjects[i].Dispose();

            AtmosSystem.Dispose();

            foreach (turf.Structure structure in Structures)
                structure.Dispose();
        }
    }
}
