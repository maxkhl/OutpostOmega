using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using Jitter.Dynamics;

namespace OutpostOmega.Game.GameObjects.Mobs
{
    /// <summary>
    /// Determins the current view of a mob (basically the eyes/head)
    /// It also does the whole raycasting
    /// </summary>
    public class View : GameObject
    {
        /// <summary>
        /// Determins if this view is blind (true)
        /// </summary>
        public bool Blind
        {
            get
            {
                return _Blind;
            }
            set
            {
                _Blind = value;
                NotifyPropertyChanged();
            }
        }
        private bool _Blind = false;

        [GameObjects.Attributes.SynchronizationAttr(GameObjects.Attributes.SynchronizePriority.UnreliableSequenced, GameObjects.Attributes.SynchronizeState.Prediction)]
        [Attributes.Access(datums.UserGroup.User)]
        public override JMatrix Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                base.Orientation = value;
            }
        }
        
        public View(World world, string ID = "View")
            : base(world, ID)
        {
            this.Priority = ProcessPriority.First;
        }

        public delegate void TargetGameObjectChangedHandler(GameObject oldTarget, GameObject newTarget);
        public event TargetGameObjectChangedHandler TargetGameObjectChanged;

        public GameObject TargetGameObject
        { 
            get
            {
                return _TargetGameObject;
            }
            set
            {
                if (_TargetGameObject != value)
                {
                    if (TargetGameObjectChanged != null)
                        TargetGameObjectChanged(_TargetGameObject, value);
                    _TargetGameObject = value;

                    NotifyPropertyChanged();
                }
            }
        }
        private GameObject _TargetGameObject;
        public turf.Structure TargetStructure 
        { 
            get; 
            set; 
        }
        public JVector TargetHit 
        { 
            get; 
            set; 
        }
        public JVector TargetHitInside { get; set; }
        public JVector TargetHitNormal
        {
            get;
            set;
        }

        [GameObjects.Attributes.SynchronizationAttr(GameObjects.Attributes.SynchronizePriority.UnreliableSequenced, GameObjects.Attributes.SynchronizeState.Prediction)]
        public JVector2 MouseOrientation
        {
            get
            {
                return _MouseOrientation;
            }
            set
            {
                if (_MouseOrientation != value)
                {
                    _MouseOrientation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private JVector2 _MouseOrientation;

        public float MouseSensitivity = 2.6f;


        [GameObjects.Attributes.SynchronizationAttr(GameObjects.Attributes.SynchronizePriority.UnreliableSequenced, GameObjects.Attributes.SynchronizeState.Prediction)]
        public JVector Rotation 
        { 
            get
            {
                return _Rotation;
            }
            set
            {
                if (_Rotation != value)
                {
                    _Rotation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private JVector _Rotation = JVector.Forward;

        public override void Update(double ElapsedTime)
        {
            var Target = CastRay(20);
            TargetGameObject = World.GetGameObject(Target);
            TargetStructure = World.GetStructure(Target);
            base.Update(ElapsedTime);
        }

        public void AddRotation(float x, float y)
        {
            //if (x == 0 && y == 0) return;

            MouseSensitivity = 0.01f;
            y = y * -1 * MouseSensitivity;
            x = x * -1 * MouseSensitivity;

            var lRotation = Rotation;
            lRotation.X = (Rotation.X + x) % ((float)Math.PI * 2.0f);
            lRotation.Y = Math.Max(Math.Min(Rotation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
            Rotation = lRotation;

            JVector lookat = new JVector();

            lookat.X = (float)(Math.Sin((float)Rotation.X) * Math.Cos((float)Rotation.Y));
            lookat.Y = (float)Math.Sin((float)Rotation.Y);
            lookat.Z = (float)(Math.Cos((float)Rotation.X) * Math.Cos((float)Rotation.Y));

            //return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
            //return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);

            Orientation = OutpostOmega.Tools.Convert.Matrix.OpenGL_To_Jitter_4(OpenTK.Matrix4.LookAt(OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Position), OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Position + lookat), OpenTK.Vector3.UnitY));

            /*if (z > 0.0f)
            {
                Rotation.Z = (float)(Rotation.Z + System.Math.Sin(z) * tVecPos.X + System.Math.Cos(z) * tVecPos.Z);
                Rotation.X = (float)(Rotation.X + System.Math.Cos(z) * tVecPos.X - System.Math.Sin(z) * tVecPos.Z);
            }*/

            /*pitch += y * MouseSensitivity;
            facing += x * -1 * MouseSensitivity;
            JVector lookatPoint = new JVector((float)Math.Cos(facing), pitch + (1 / 2) * (pitch * pitch), (float)Math.Sin(facing));
            //JVector lookatPoint = new JVector((float)Math.Cos(facing), (float)Math.Sin(pitch), (float)Math.Sin(facing));
            Orientation = OutpostOmega.Tools.Convert.Matrix.OpenGL_To_Jitter_4(OpenTK.Matrix4.LookAt(OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Position), OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(Position + lookatPoint), OpenTK.Vector3.UnitY));
            */
            /*MouseOrientation.X = (MouseOrientation.X + x) % ((float)Math.PI * 2.0f);
            MouseOrientation.Y = Math.Max(Math.Min(MouseOrientation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
            //MouseOrientation.Z = 0;

            MouseOrientation = new JVector2((MouseOrientation.X + x) % ((float)Math.PI * 2.0f),
                                 Math.Max(Math.Min(MouseOrientation.Y + x, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f));

            //MouseOrientation += new JVector( * MouseSensitivity, mouseState.Y - OldMouseState.Y * MouseSensitivity, 0);

            var Lookat = new JVector();
            Lookat.X = (float)(Math.Sin(MouseOrientation.X) * Math.Cos(MouseOrientation.Y));
            Lookat.Y = (float)Math.Sin(MouseOrientation.Y);
            Lookat.Z = (float)(Math.Cos(MouseOrientation.X) * Math.Cos(MouseOrientation.Y));

            var ogl_Position = new OpenTK.Vector3(Position.X, Position.Y, Position.Z);
            var ogl_Lookat = new OpenTK.Vector3(Lookat.X, Lookat.Y, Lookat.Z);

            Orientation = OutpostOmega.Tools.Convert.Matrix.OpenGL_To_Jitter_4(OpenTK.Matrix4.LookAt(
                ogl_Position,
                ogl_Position + ogl_Lookat,
                OpenTK.Vector3.UnitY));*/
        }

        /// <summary>
        /// Casts a ray in the physics system. The ray is aligned to the direction and position of this view
        /// </summary>
        /// <param name="Distance">The distance of the check</param>
        /// <returns>Hit RigidBody. Null when no hit</returns>
        public RigidBody CastRay(float Distance = 2)
        {
            RigidBody rbHit = null;
            JVector normal;
            float fraction;

            bool Success = false;

            // Do the cast
            Success = this.World.PhysicSystem.CollisionSystem.Raycast(
                this.Position, //Position
                this.Forward * Distance, //Direction
                RayCastCallback,
                out rbHit, out normal, out fraction);

            TargetHitNormal = normal;
            if (Success)
            {
                this.TargetHit = Tools.MathHelper.GetRayHit(this.Position, this.Forward * Distance, fraction);
                this.TargetHitInside = Tools.MathHelper.GetRayHit(this.Position, this.Forward * (Distance + 1.3f), fraction) - JVector.Divide(normal, 2);//Tools.MathHelper.GetRayHit(this.Position, this.Forward * (Distance + 2.3f), fraction);
            }
            else
            {
                this.TargetHit = JVector.Zero;
            }

            return rbHit;
        }

        private bool RayCastCallback(RigidBody body, JVector normal, float fraction)
        {
            if (body != this.Parent.RigidBody)
                return true;
            else
                return false;
        }
    }
}
