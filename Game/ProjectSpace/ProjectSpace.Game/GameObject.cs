using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using Jitter.LinearMath;

namespace OutpostOmega.Game
{
	/// <summary>
	/// Basic object. Used for networking and serializing of objects in the game
	/// </summary>
	public partial class GameObject : IDisposable
    {
        #region Main Properties

        /// <summary>
        /// Full name/identification of the object
        /// </summary>
        public string ID 
        {
            get { return _ID; }
            set { if (_ID != value) { _ID = value; NotifyPropertyChanged("ID"); } }
        }
        private string _ID;

        /// <summary>
        /// World, this object was assigned to
        /// </summary>
        public World World { get; set; }

        /// <summary>
        /// Parent of this object
        /// </summary>
        public GameObject Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                if (_Parent != value)
                {
                    if (value != null && value.RigidBody != null && this.RigidBody != null)
                        throw new Exception("Parenting between physical objects not allowed");

                    // Unsubscribe old parents
                    if (_Parent != null)
                        _Parent.PropertyChanged -= _Parent_PropertyChanged;

                    _Parent = value;

                    // Subscribe to Parents property changes
                    if(_Parent != null)
                        _Parent.PropertyChanged += _Parent_PropertyChanged;

                    NotifyPropertyChanged();
                }
            }
        }

        // Raise changes sent from parent as minor event
        void _Parent_PropertyChanged(GameObject sender, string PropertyName, bool MinorChange)
        {
            NotifyPropertyChanged(PropertyName, true);
        }


        private GameObject _Parent = null;
        
        /// <summary>
        /// Gets all gameobjects, that have this object set as parent
        /// </summary>
        [GameObjects.Attributes.Serialize(GameObjects.Attributes.SerializeState.DoNotSerialize)]
        public GameObject[] Children
        {
            get
            {
                return (from gobject in this.World.AllGameObjects
                        where gobject.Parent == this
                        select gobject).ToArray();
            }
        }

        /// <summary>
        /// Local position to the parent
        /// </summary>
        public JVector localPosition
        {
            get
            {
                if (this.Parent == null)
                    return Position;
                else
                    return _localPosition;
            }
            set
            {
                if (_localPosition != value)
                {
                    if (this.Parent == null)
                        Position = value;
                    else
                        _localPosition = value;

                    NotifyPropertyChanged();
                }
            }
        }
        private JVector _localPosition = JVector.Zero;

        /// <summary>
        /// Overall draw-offset
        /// </summary>
        public virtual JVector Offset
        {
            get
            {
                return rigidBodyOffset;
            }
        }

        /// <summary>
        /// Offset value for rigidBody Position to model position
        /// </summary>
        public JVector rigidBodyOffset
        {
            get
            {
                return _rigidBodyOffset;
            }
            set
            {
                if (_rigidBodyOffset != value)
                {
                    _rigidBodyOffset = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private JVector _rigidBodyOffset = JVector.Zero;

        /// <summary>
        /// Simulates key input when this object is not null
        /// </summary>
        public Tools.KeybeardState SimulatedKeyInput { get; set; }

        /// <summary>
        /// Simulates key input when this object is not null
        /// </summary>
        //public Tools.MouseState SimulatedMouseInput { get; set; }

        public Tools.MouseState OldMouseState = new Tools.MouseState(OpenTK.Input.Mouse.GetState());
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject(World World, string ID = "GameObject")
		{
            this.Priority = ProcessPriority.Default;

            this.World = World;

            this.ID = GetUniqueID(ID);
            
            //WTF why did I put this shit in
            //_Position = new JVector((float)World.AllGameObjects.Count, (float)World.AllGameObjects.Count, (float)World.AllGameObjects.Count);

            Initialise();
		}

        /// <summary>
        /// Returns a unique ID
        /// </summary>
        /// <param name="ID">Suggested identifier.</param>
        /// <returns></returns>
        public string GetUniqueID(string ID)
        {
            if (World.IDCounter.ContainsKey(ID))
                World.IDCounter[ID] = (int.Parse(World.IDCounter[ID]) + 1).ToString();
            else
                World.IDCounter.Add(ID, "1");

            return ID + World.IDCounter[ID];
        }

        [GameObjects.Attributes.Serialize(GameObjects.Attributes.SerializeState.DoNotSerialize)]
        private bool Constructed = false;
        /// <summary>
        /// This is mainly used by the modding api
        /// </summary>
        public virtual void Constructor()
        { }

        /// <summary>
        /// Initialisation happens in the constructor and the deserialization
        /// </summary>
        public virtual void Initialise()
        {
            this.PropertyChanged += World.World_PropertyChanged;
        }

        /// <summary>
        /// Sets a new Position for this object
        /// </summary>
        /// <param name="newPosition"></param>
        public virtual void SetPosition(JVector newPosition)
        {
            if(this.RigidBody != null && this.IsPhysical)
            {
                PhysicSetPosition(newPosition);
            }
            else
            {
                this.Position = newPosition;
            }
        }

        public virtual void LuaUpdate()
        { }

        /// <summary>
        /// Update priority
        /// </summary>
        public ProcessPriority Priority { get; set; }

        public enum ProcessPriority
        {
            First = 10,
            Second = 20,
            Default = 30,
            Last = 40
        }

        /// <summary>
        /// Update-method of this gameObject
        /// </summary>
        public virtual void Update(double ElapsedTime)
        {
            if (!Constructed)
            {
                Constructor();
                Constructed = true;
            }

            // Refresh the gOs Position if necessary
            if (this.RigidBody != null) // && this._RigidBody.Position + this.rigidBodyOffset != this.Position)
                this.Position = this.RigidBody.Position; // +this.rigidBodyOffset;

            if (this.Parent != null)
            {
                /*if (this.Parent.Position != this.Position)
                {
                    this.Position = this.Parent.Position + localPosition;
                    this.Orientation = this.Parent.Orientation * this.localOrientation;
                }*/
                if (this.Parent.Disposing)
                {
                    this.Dispose();
                    return;
                }

                /*if (this.Parent.Visible != this.Visible)
                    this.Visible = this.Parent.Visible;*/
            }

            LuaUpdate();

            if (Animations == null) _Animations = new List<Animation>();
            for(int i = 0; i < Animations.Count; i++)
                Animations[i].Update((float)ElapsedTime);

            if (this.Move != JVector.Zero)
            {
                this.Position += this.Move;
                this.Move = JVector.Zero;
            }
        }

        /// <summary>
        /// Is called whenever a character is pressing a key and looks at this gameobject at a certain distance
        /// </summary>
        public virtual void KeyPress(OpenTK.Input.Key Key, bool IsRepeat)
        {

        }

        /// <summary>
        /// Used to reply a functionlist to a mind that is trying to access this gameObject
        /// </summary>
        public virtual GameObjects.Function[] FunctionRequest(GameObjects.Mobs.Minds.PlayerMind Requester)
        {
            var function = new List<GameObjects.Function>();

            // Admin Functions
            if (datums.UserGroupTools.HasAccess(datums.UserGroup.Administrator, Requester.Group))
            {
            }

            // GM Functions
            if(datums.UserGroupTools.HasAccess(datums.UserGroup.Gamemaster, Requester.Group))
            {
                function.Add(new GameObjects.Function() { Separator = true });
                function.Add(new GameObjects.Function()
                {
                    ID = "EDIT",
                    Text = string.Format("Edit", this.ID),
                });
                function.Add(new GameObjects.Function()
                {
                    ID = "DELETE",
                    Text = string.Format("Delete", this.ID),
                });
            }
            
            // Functions for everyone
            function.Add(new GameObjects.Function()
                {
                    ID = "INFO",
                    Text = "Info",
                });


            return function.ToArray();
        }

        public delegate void FunctionSelectedHandler(GameObject Sender, GameObjects.Function SelectedFunction, GameObjects.Mobs.Mind Requester);

        /// <summary>
        /// Occurs when a function on this object got exectued.
        /// Mainly used to notify the frontent.
        /// </summary>
        public event FunctionSelectedHandler FunctionSelected;

        public virtual void FunctionSelect(GameObjects.Function SelectedFunction, GameObjects.Mobs.Mind Requester)
        {

            if (FunctionSelected != null)
                FunctionSelected(this, SelectedFunction, Requester);
        }


        /// <summary>
        /// Determins if this gameObject is registered to a world. (Call Register() to resolve this issue)
        /// </summary>
        public bool Registered
        {
            get
            {
                if (World == null)
                    return false;
                return _Registered;
                //return World.GameObjects.Contains(this); the bool value is a lot faster
            }
        }
        private bool _Registered = false;

        /// <summary>
        /// Register this object to the assigned world (alternatively add it to the world)
        /// </summary>
        public void Register()
        {
            if(!this.World.AllGameObjects.Contains(this))
                this.World.Add(this);
            _Registered = true;
        }

        /// <summary>
        /// Determins if object is currently in a disposal process
        /// </summary>
        public bool Disposing { get; set; }
        public virtual void Dispose()
        {
            Disposing = true;
            this.World.Remove(this);

            if (this.RigidBody != null)
                PhysicDisable();

            // Unbind all meshs
            foreach (var mesh in this.Meshs)
                mesh.Remove(this);
            
        }

        /// <summary>
        /// Returns the unique ID of this gameobject
        /// </summary>
        public override string ToString()
        {
            return this.ID;
        }
	}
}

