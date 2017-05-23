﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Collision.Shapes;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects
{
    /// <summary>
    /// A mob is a object that can be entered and controlled by a mind
    /// </summary>
    public class Mob : GameObject
    {
        /// <summary>
        /// (Display-)Name of this mob
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        private string _Name = "";

        public CharacterController charController
        {
            get
            {
                if(_charController == null)
                    _charController = new Jitter.Dynamics.Constraints.CharacterController(this.World.PhysicSystem, this.Position, this.Width, this.Height);

                return _charController;
            }
            set
            {
                if (_charController != value)
                {
                    _charController = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private CharacterController _charController;

        public bool FlyMode
        {
            get
            {
                return _FlyMode;
            }
            set
            {
                if (_FlyMode != value)
                {
                    //if(this.Mind)
                    if (this.RigidBody != null)
                        if (value)
                            this.RigidBody.AffectedByGravity = false;
                        else
                            this.RigidBody.AffectedByGravity = true;

                    if (this.charController != null)
                        this.charController.Fly = value;

                    _FlyMode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _FlyMode;

        public int SelectedQuickslot
        { 
            get
            {
                return _SelectedQuickslot;
            }
            set
            {
                if (Quickslot == null)
                {
                    Quickslot = new List<Item>(9);
                    for (int i = 0; i < 9; i++)
                        Quickslot.Add(null);
                }

                if(Quickslot[_SelectedQuickslot] != null)
                {
                    Quickslot[_SelectedQuickslot].Visible = false;
                    Quickslot[_SelectedQuickslot].Holder = null;
                }

                if (Quickslot[value] != null)
                {
                    Quickslot[value].Visible = true;
                    Quickslot[value].Holder = this;
                }
                var oldval = _SelectedQuickslot;
                _SelectedQuickslot = value;

                if (QuickslotSelectedChanged != null)
                    QuickslotSelectedChanged(oldval, _SelectedQuickslot);

                NotifyPropertyChanged();
            }
        }
        private int _SelectedQuickslot = 0;

        public List<Item> Quickslot { get; set; }

        /// <summary>
        /// Mind that is assigned to this mob (can be null)
        /// </summary>
        public Mobs.Mind Mind
        {
            get
            {
                return _Mind;
            }
            set
            {
                _Mind = value;
                NotifyPropertyChanged();
            }
        }
        private Mobs.Mind _Mind = null;

        /// <summary>
        /// View that is assigned to this mob (can be null)
        /// </summary>
        public Mobs.View View
        {
            get
            {
                return _View;
            }
            set
            {
                if (_View != value)
                {
                    _View = value;
                    _View.Parent = this;
                    NotifyPropertyChanged();
                }
            }
        }
        private Mobs.View _View;

        /// <summary>
        /// Height of the mob
        /// </summary>
        public float Height
        {
            get
            {
                return _Height;
            }
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    if(_charController != null) _charController.SetSize(Width, _Height);
                    NotifyPropertyChanged();
                }
            }
        }
        private float _Height;

        /// <summary>
        /// Width of the mob
        /// </summary>
        public float Width
        {
            get
            {
                return _Width;
            }
            set
            {
                if (this._Width != value)
                {
                    _Width = value;
                    if (_charController != null) _charController.SetSize(_Width, Height);
                    NotifyPropertyChanged();
                }
            }
        }
        private float _Width;

        public GameObjects.Item HoldItem
        {
            get
            {
                return Quickslot[SelectedQuickslot];
            }
        }
        private GameObjects.Item _HoldItem = null;
        private Jitter.LinearMath.JVector HoldOffset;

        public Mob(World world, string ID = "mob", float Height = 1.8f, float Width = 0.80f, float Mass = 0.19477874f)
            : base(world, ID)
        {
            //Remove the caps of the capsule to get the correct height
            //Height -= Width * 2;
            //this.Shape = new CapsuleShape(Height, Width);
            this.Position = new Jitter.LinearMath.JVector(0, 20, 0);

            this.Priority = ProcessPriority.Second;

            Quickslot = new List<Item>(9);
            for (int i = 0; i < 9; i++)
                Quickslot.Add(null);

            var spawner = new Items.Devices.Spawner(World);
            spawner.Register();
            this.AddToQuickslot(spawner);
            this.SelectedQuickslot = 0;

            var builder = new Items.Devices.Builder(World);
            builder.Register();
            this.AddToQuickslot(builder);

            var drawer = new Items.Devices.Drawer(World);
            drawer.Register();
            this.AddToQuickslot(drawer);

            var cablespawner = new Items.Devices.CableSpawner(World);
            cablespawner.Register();
            this.AddToQuickslot(cablespawner);
            //this.SelectedQuickslot = 0;

            //this.Mass = Mass;
            //this.Static = false;
            //this.PhysicCreateMaterial(0, 0, 0);
            //this.PhysicEnable();
            //this.PhysicFixAngle(Mass);
            this.Height = Height;
            this.Width = Width;

            //this._RigidBody.EnableSpeculativeContacts = true;
        }

        public override void Initialise()
        {
            MovementSpeed = 0.07f;
            MovementRunSpeed = 0.15f;

            HoldOffset = new JVector(-0.15f, 0.1f, 0.4f);

            _charController = new Jitter.Dynamics.Constraints.CharacterController(World.PhysicSystem, this.Position, Width, Height);
            _charController.RayCallback = RayCallback;


            base.Initialise();
        }
        
        private bool RayCallback(Jitter.Dynamics.RigidBody body, JVector normal, float fraction)
        {
            if (body.Tag != null && typeof(GameObject).IsAssignableFrom(body.Tag.GetType()) && ((GameObject)body.Tag).IsPassable)
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Executes the use command for this mob
        /// </summary>
        public void DoUse(UseAction Action)
        {
            //if (HoldItem == null) return;

            if (HoldItem != null && typeof(Items.Device).IsAssignableFrom(HoldItem.GetType()))
                ((Items.Device)HoldItem).UseDevice(this.View.TargetGameObject, this, Action);
            else if (this.View.TargetGameObject != null)
                this.View.TargetGameObject.Use(this, HoldItem, Action);                
        }

        /// <summary>
        /// Holds a specific item in front of the mob (imagine it like the pickup thing in hl2)
        /// </summary>
        /// <param name="Item">Item to hold</param>
        /// <param name="Offset">Offset</param>
        protected void Hold(GameObjects.Item Item)
        {
            //HoldItem = Item;
        }

        /// <summary>
        /// Drops every item, this mob is holding
        /// </summary>
        public virtual void Drop()
        {
            if (HoldItem == null)
                return;

            if (Quickslot[SelectedQuickslot] != null)
            {
                HoldItem.Visible = true;
                World.PhysicSystem.AddBody(HoldItem.RigidBody);
                Quickslot[SelectedQuickslot] = null;
                if (QuickslotChanged != null)
                    QuickslotChanged(null, SelectedQuickslot, false);
            }
        }

        public override void OnDeserialization()
        {
            
 	         base.OnDeserialization();
        }

        public int AddToQuickslot(Item item)
        {
            bool contains = false;
            for (int i = 0; i < Quickslot.Count; i++)
                if (Quickslot[i] == item)
                {
                    contains = true;
                    break;
                }

            if(!contains)
            {
                int start = 0;
                if (Quickslot[SelectedQuickslot] == null)
                    start = SelectedQuickslot;

                for (int i = start; i < Quickslot.Count; i++)
                    if (Quickslot[i] == null)
                    {
                        item.Visible = false;
                        World.PhysicSystem.RemoveBody(item.RigidBody);
                        Quickslot[i] = item;
                        if (QuickslotChanged != null)
                            QuickslotChanged(item, i, true);
                        return i;
                    }
            }


            return -1;
        }

        public delegate void QuickslotChangedHandler(Item item, int index, bool Added);
        public event QuickslotChangedHandler QuickslotChanged;

        public delegate void QuickslotSelectedChangedHandler(int oldIndex, int Index);
        public event QuickslotSelectedChangedHandler QuickslotSelectedChanged;

        /// <summary>
        /// The speed, this mob is able to move
        /// </summary>
        public float MovementSpeed { get; set; }

        /// <summary>
        /// The speed, this mob is able to move while sprinting
        /// </summary>
        public float MovementRunSpeed { get; set; }

        /// <summary>
        /// Moves this mob in the given direction
        /// </summary>
        public void Move(JVector MovementVector, bool Run)
        {
            MovementVector.Normalize();
            MovementVector *= Run ? MovementRunSpeed : MovementSpeed;
            charController.TargetVelocity += MovementVector;
        }

        public void Jump()
        {
            charController.TryJump = true;
        }

        public override void Update(double ElapsedTime)
        {
            _charController.Position = this.Position;
            _charController.Update((float)ElapsedTime);
            this.Position += _charController.PositionDelta;
            _charController.PositionDelta = JVector.Zero;
            //if(this.Position != _charController.Position)
            //    this.Position = _charController.Position;

            if (_View != null)
            {
                var lookat = _View.Forward;
                lookat.Y = 0;
                lookat.Normalize();
                lookat.Negate();
                Orientation =
                    JMatrix.CreateRotationX((float)Tools.MathHelper.DegreeToRadian(180)) *
                    OutpostOmega.Tools.Convert.Matrix.OpenGL_To_Jitter_4(OpenTK.Matrix4.LookAt(OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Position), OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Position + lookat), -OpenTK.Vector3.UnitY))
                    ;
            }
            //this.Orientation = _View.Orientation;
            this.Breathe();

            if(HoldItem != null)
            {
                HoldItem.Position = 
                    (this.View != null ? this.View.Position : this.Position) + 
                    JVector.Transform(HoldOffset * -1, JMatrix.Inverse(this.View != null ? this.View.Orientation : this.Orientation));
                //HoldItem.Orientation = this.View.Orientation * JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(80));
                //HoldItem.Orientation = JMatrix.CreateRotationX((float)Tools.MathHelper.DegreeToRadian(80)) * this.Orientation * JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(240));
                HoldItem.Orientation = JMatrix.Inverse(this.View != null ? this.View.Orientation : this.Orientation);
            }
            base.Update(ElapsedTime);
        }

        protected virtual void Breathe()
        {

        }

        /// <summary>
        /// hihi he said pehnis. Nah.. this is very serious atmos test!
        /// </summary>
        public void Fart()
        {
            World.Gas(this.Position, 1, 20);
        }
    }
}
