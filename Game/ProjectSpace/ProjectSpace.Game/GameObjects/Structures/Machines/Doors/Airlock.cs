using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures.Machines.Doors
{
    /// <summary>
    /// A completely airtight door
    /// </summary>
    [Attributes.Definition("Airlock", "A standard airlock")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tools.Knife))]
    public class Airlock : Door
    {
        public AirlockDoor UpDoor { get; set; }
        public AirlockDoor DownDoor { get; set; }

        private Content.Model _Model;

        public Airlock(int X, int Y, int Z, turf.Structure Structure, World World, string ID = "airlock")
            : base(X, Y, Z, Structure, World, ID)
        {
            this.BlockOffset = new JVector(0, 1, .5f);

            _Model = LoadModel(@"Content\Model\Structure\Machinery\Doors\Airlock.dae");
            _Model.AssignTexture("DoorFrame", this, LoadTexture(@"Content\Model\Structure\Machinery\Doors\Airlock.png"));
            _Model.AssignTexture("DoorUp", this, LoadTexture(@"Content\Model\Structure\Machinery\Doors\Airlock.png"));
            _Model.AssignTexture("DoorDown", this, LoadTexture(@"Content\Model\Structure\Machinery\Doors\Airlock.png"));

            _Model.ReleaseGameObject("DoorUp", this);
            _Model.ReleaseGameObject("DoorDown", this);


            this.rigidBodyOffset = new JVector(0, -2, -.5f);

            this.Shape = new Jitter.Collision.Shapes.BoxShape(2, 2f, 1f);
            this.Mass = 20;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();

            // Makes the body passable for players
            this.IsPassable = true;

            // Makes the body passable for evertything
            //this.World.PhysicSystem.CollisionSystem.RemoveEntity(this.RigidBody);

            UpDoor = new AirlockDoor(World);
            UpDoor.Parent = this;
            UpDoor.SetUp();
            UpDoor.Register();
            DownDoor = new AirlockDoor(World);
            DownDoor.Parent = this;
            DownDoor.SetDown();
            DownDoor.Register();
        }
        public override void Constructor()
        {
            UpDoor.CommandExecuted += UpDoor_CommandExecuted;
            base.Constructor();
        }

        public bool IsOpen { get; set; }
        public override bool Use(Mob User, Item Item, Game.Tools.Action Action)
        {
            /*if(_Model == null)
            {
                foreach (Content.Model model in World.ContentManager.Get<Content.Model>())
                {
                    if (model.Path == @"Content\Model\Structure\Machinery\Doors\Airlock.dae")
                        _Model = model;
                }
            }*/
            //_Model.Meshs["DoorFrame"][this].Alpha = 0.5f;

            if (Action != Tools.Action.InteractSecondary) return false;

            if(Item == null) // Hands
            {
                if (IsOpen)
                {
                    var ret1 = UpDoor.Close();
                    var ret2 = DownDoor.Close();
                    if (ret1 && ret2)
                    {
                        IsOpen = !IsOpen;
                        _Model.Meshs["DoorFrame"][this].Texture = (Content.Texture)this.World.ContentManager.Load(@"Content\Model\Structure\Machinery\Doors\AirlockMoving.png");
                    }
                }
                else
                {
                    var ret1 = UpDoor.Open();
                    var ret2 = DownDoor.Open();
                    if (ret1 && ret2)
                    {
                        IsOpen = !IsOpen;
                        _Model.Meshs["DoorFrame"][this].Texture = (Content.Texture)this.World.ContentManager.Load(@"Content\Model\Structure\Machinery\Doors\AirlockMoving.png");
                    }
                }
            }

            return base.Use(User, Item, Action);
        }

        void UpDoor_CommandExecuted()
        {
            if (IsOpen)
                _Model.Meshs["DoorFrame"][this].Texture = (Content.Texture)this.World.ContentManager.Load(@"Content\Model\Structure\Machinery\Doors\AirlockActive.png");
            else
                _Model.Meshs["DoorFrame"][this].Texture = (Content.Texture)this.World.ContentManager.Load(@"Content\Model\Structure\Machinery\Doors\Airlock.png");
        }

        public class AirlockDoor : GameObject
        {
            //public Airlock Airlock { get; set; }
            public bool Moving { get; set; }
            public bool IsUp { get; set; }
            private Content.Model Model;
            public AirlockDoor(World World, string ID = "AirlockDoor")
                : base(World, ID)
            {
                //this.Airlock = Parent;
                //this.Parent = Parent;
                Model = LoadModel(@"Content\Model\Structure\Machinery\Doors\Airlock.dae");


                /*if (_IsUp)
                    this.Shape = MeshToShape(model, model.Meshs["DoorUp"]);
                else
                    this.Shape = MeshToShape(model, model.Meshs["DoorDown"]);
                this.Mass = 20;
                this.Static = true;
                this.PhysicCreateMaterial();
                this.PhysicEnable();*/
                //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
                //this.PhysicEnableDebug();

                Model.ReleaseGameObject("DoorFrame", this);
            }
            public void SetUp()
            {
                Model.ReleaseGameObject("DoorDown", this);
                this.localPosition = new JVector(0, 0, 0);
                IsUp = true;
            }
            public void SetDown()
            {
                Model.ReleaseGameObject("DoorUp", this);
                this.localPosition = new JVector(0, 0, 0);
                IsUp = false;
            }

            bool _Opening = false;
            public bool Open()
            {
                bool retb = false;
                if (IsUp)
                    retb = MoveUp();
                else
                    retb = MoveDown();

                if(retb)
                    _Opening = true;

                return retb;
            }
            public bool Close()
            {
                bool retb = false;
                if (IsUp)
                    retb = MoveDown();
                else
                    retb = MoveUp();

                if (retb)
                {
                    _Opening = false;
                }

                return retb;
            }

            private bool MoveUp()
            {
                if (Moving) return false;

                Moving = true;
                var anim = this.Animate("localPosition", new JVector(0, 1.0f, 0) + this.localPosition, 1000, Animation.EaseFunction.CubicEaseOut);
                anim.AnimationDone += anim_AnimationDone;
                return true;
            }
            private bool MoveDown()
            {
                if (Moving) return false;

                Moving = true;
                var anim = this.Animate("localPosition", new JVector(0, -1.0f, 0) + this.localPosition, 1000, Animation.EaseFunction.CubicEaseOut);
                anim.AnimationDone += anim_AnimationDone;
                return true;
            }

            public delegate void CommandExecutedHandler();
            public event CommandExecutedHandler CommandExecuted;

            void anim_AnimationDone(Animation sender)
            {
                Moving = false;
                if(CommandExecuted != null)
                    CommandExecuted();
            }
        }
    }
}
