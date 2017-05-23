using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Physics part of a gameObject. Handles most of the interaction with Jitter
    /// </summary>
    public partial class GameObject
    {

        #region Public properties
        /// <summary>
        /// Shape of the RigidBody (see Jitter.Collision.Shapes). Contains values like mass and torque!
        /// </summary>
        public Jitter.Collision.Shapes.Shape Shape
        {
            get
            {
                if (RigidBody == null)
                    return _Shape;
                else
                    return RigidBody.Shape;
            }
            set
            {
                if (RigidBody == null)
                    _Shape = value;
                else
                    RigidBody.Shape = value;
                NotifyPropertyChanged();
            }
        }
        public Jitter.Collision.Shapes.Shape _Shape;

        public bool IsPassable
        {
            get
            {
                return _IsPassable;
            }
            set
            {
                _IsPassable = value;
                NotifyPropertyChanged();
            }
        }
        private bool _IsPassable = false;

        /// <summary>
        /// If this object has a physical body
        /// </summary>
        public bool IsPhysical 
        { 
            get
            {
                return RigidBody != null;
            }
        }

        /// <summary>
        /// Physics-Material of this gameobject
        /// </summary>
        public Jitter.Dynamics.Material Material
        {
            get
            {
                if (RigidBody == null)
                    return _Material;
                else
                    return RigidBody.Material;
            }
            set
            {
                if (RigidBody == null)
                    _Material = value;
                else
                    RigidBody.Material = value;
                NotifyPropertyChanged("Material");
            }
        }
        public Jitter.Dynamics.Material _Material;

        public float Mass
        {
            get
            {
                if (RigidBody == null)
                    return _Mass;
                else
                    return RigidBody.Mass;
            }
            set
            {
                if (RigidBody == null)
                    _Mass = value;
                else
                    RigidBody.Mass = value;
                NotifyPropertyChanged("Mass");
            }
        }
        private float _Mass = 0;

        public bool Static
        {
            get
            {
                if (RigidBody == null)
                    return _Static;
                else
                    return RigidBody.IsStatic;
            }
            set
            {
                if (RigidBody == null)
                    _Static = value;
                else
                    RigidBody.IsStatic = value;
                NotifyPropertyChanged("Mass");
            }
        }
        private bool _Static = false;

        #endregion
        
        #region Private Properties

        /// <summary>
        /// RigidBody of this gameObject. Could be null if physics not enabled!
        /// </summary>
        [GameObjects.Attributes.Serialize(GameObjects.Attributes.SerializeState.DoNotSerialize)]
        public Jitter.Dynamics.RigidBody RigidBody { get; set; }

        #endregion


        /// <summary>
        /// Creates a rigidBody for this gameObject and adds it to the current physicsSystem
        /// </summary>
        public virtual void PhysicEnable()
        {
            if (this.Shape == null)
                throw new Exception("Tried to initialize GO-Physic without a shape!");

            //Add this GO as tag for backtracking after raycasts

            RigidBody = new RigidBody(this.Shape, this.Material);
            RigidBody.Tag = this;
            RigidBody.Damping = RigidBody.DampingType.None;
            this.World.PhysicSystem.AddBody(RigidBody);

            RigidBody.IsActive = true;
            RigidBody.Position = this._Position;
            RigidBody.Orientation = this._Orientation;
            RigidBody.IsStatic = this._Static;

            //RigidBody.AffectedByGravity = false;
        }


        /// <summary>
        /// Removes the current RigidBody of this GameObject from the game
        /// </summary>
        public void PhysicDisable()
        {
            if (this.RigidBody == null)
                return; // Physics already disabled

            this.World.PhysicSystem.RemoveBody(RigidBody);
            this.RigidBody = null;
        }
        
        /// <summary>
        /// Creates a new Material for this go's rigidBody
        /// </summary>
        /// <param name="kineticFriction">Friction between moving bodies</param>
        /// <param name="staticFriction">Friction between static bodies</param>
        /// <param name="restitution">How hard it is to stop this moving body. If set to 1, this object won't lose energy to collisions.</param>
        public void PhysicCreateMaterial(float kineticFriction = 0.3f, float staticFriction = 0.6f, float restitution = 0.1f)
        {
            this.Material = new Material() {KineticFriction = kineticFriction, StaticFriction = staticFriction, Restitution = restitution};
        }

        /// <summary>
        /// Used to move this object to a specific position
        /// </summary>
        /// <param name="newPosition">New Position</param>
        public void PhysicSetPosition(Jitter.LinearMath.JVector newPosition)
        {
            PhysicWakeup();

            if (this.RigidBody != null)
                this.RigidBody.Position = newPosition;
            else
                this.Position = newPosition;
        }

        /// <summary>
        /// Makes the angle of this object static (no rotation)
        /// </summary>
        public void PhysicFixAngle(float Mass)
        {
            if (this.RigidBody == null)
                throw new Exception("Enable physics first!");

            this.RigidBody.SetMassProperties(Jitter.LinearMath.JMatrix.Zero, 1 / Mass, true);
            /*Jitter.Dynamics.Constraints.SingleBody.FixedAngle fac =
                new Jitter.Dynamics.Constraints.SingleBody.FixedAngle(this._RigidBody);
            this.World.PhysicSystem.AddConstraint(fac);*/
        }

        /// <summary>
        /// Wakes the Rigidbody up and prepares it for movement
        /// </summary>
        public void PhysicWakeup()
        {
            if(this.RigidBody != null)
                this.RigidBody.IsActive = true;
        }

        /// <summary>
        /// Applies force to the Rigidbody of this shape
        /// </summary>
        /// <param name="Direction">Direction the object should pushed at</param>
        /// <param name="Strength">Strength of the push</param>
        public void PhysicApplyForce(Jitter.LinearMath.JVector Direction, float Strength)
        {
            if (this.RigidBody == null)
                throw new Exception("Enable physics first!");

            PhysicWakeup();

            this.RigidBody.AddForce(Direction * Strength);
        }

        /// <summary>
        /// Enables debug drawing for this rigidBody
        /// </summary>
        public void PhysicEnableDebug()
        {
            if (this.RigidBody == null)
                throw new Exception("Enable physics first!");

            this.RigidBody.EnableDebugDraw = true;
        }

        private void PhysicSetParent(GameObject newParent)
        {
            if (newParent.Shape == null)
                return; //Nothing to do in this case

            var Shapes = new List<CompoundShape.TransformedShape>();
            //Shapes.Add(new CompoundShape.TransformedShape(

            //var cShape = new Jitter.Collision.Shapes.CompoundShape(
        }

        /// <summary>
        /// Generates a shape from a mesh. Pretty slow right now. Needs optimization
        /// </summary>
        public Shape MeshToShape(Content.Model Model, Content.Mesh Mesh)
        {
            return Tools.Collada.ShapeFromMesh(Model.Path, Mesh.Name);
        }
    }
}
