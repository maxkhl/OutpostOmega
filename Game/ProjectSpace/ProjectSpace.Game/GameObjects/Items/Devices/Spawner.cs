using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using OpenTK;

namespace OutpostOmega.Game.GameObjects.Items.Devices
{
    [Attributes.IconAttribute(@"Content\Model\Items\Devices\Spawner.png")]
    [Attributes.Definition("Spawner", "Used to spawn stuff")]
    public class Spawner : SpawnTool
    {


        public Spawner(World world, string ID = "spawner")
            : base(world, ID)
        {
            var model = LoadModel(@"Content\Model\Items\Devices\Gun.dae");
            model.AssignTexture("Gun", this, LoadTexture(@"Content\Model\Items\Devices\Gun.png"));
            model.AssignTexture("Display", this, LoadTexture(@"Content\Model\Items\Devices\Spawner.png"));

            this.Shape = MeshToShape(model, model.Meshs["Colission"]);


            this.Mass = 20;
            this.Static = false;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }

        public override void Update(double ElapsedTime)
        {
            if (Holder != null && Holder.Mind != null && Holder.View != null)
            {
                if (Holder.View.TargetStructure != null)
                {
                    if (this.SelectedBuildObject != null && this.SelectedBuildObject.GetType() == typeof(Turf.Types.TurfTypeE))
                    {
                        var tpos = Holder.View.TargetStructure.GetBlockPosition(Holder.View.TargetHitInside);
                        World.CallUI(this, UICommand.Highlight, new Jitter.LinearMath.JBBox(tpos - new JVector(0.1f), tpos + JVector.One + new JVector(0.1f)));
                    }
                    else
                        World.CallUI(this, UICommand.HighlightStop, null);
                }
                else
                    World.CallUI(this, UICommand.HighlightStop, null);
            }

            base.Update(ElapsedTime);
        }

        public override void UseDevice(GameObject Target, Mob User, Game.Tools.Action Action)
        {
            if (User.Mind != null && this.SelectedBuildObject != null && Action == Game.Tools.Action.InteractPrimary)
            {
                if (User.View.TargetStructure != null)
                {
                    var ObjectType = this.SelectedBuildObject.GetType();
                    if (typeof(Structures.Structure).IsAssignableFrom(ObjectType))
                    {
                        var structureObject = (Structures.Structure)GameObject.GenerateNew(ObjectType, this.World);

                        var orientation = JMatrix.CreateRotationY(MathHelper.DegreesToRadians(90) * this.SpawnDirection);
                        structureObject.Orientation = orientation;

                        var nullX = (int)Math.Floor(User.View.TargetHit.X);
                        var nullY = (int)Math.Floor(User.View.TargetHit.Y);
                        var nullZ = (int)Math.Floor(User.View.TargetHit.Z);

                        var center = new JVector((float)nullX + 0.5f, 0, (float)nullZ + 0.5f);

                        center += JVector.Transform(new JVector(-0.5f, 0, -0.5f), orientation);



                        center += JVector.Transform(new JVector(structureObject.BlockOffset.X, 0, structureObject.BlockOffset.Z), JMatrix.CreateRotationY(MathHelper.DegreesToRadians(90 * this.SpawnDirection)));
                        center.Y = nullY;

                        structureObject.SetPosition(
                            (int)Math.Floor(center.X),
                            (int)Math.Floor(center.Y),
                            (int)Math.Floor(center.Z));

                        structureObject.Register();
                    }
                    else if (ObjectType == typeof(Turf.Types.TurfTypeE))
                        User.View.TargetStructure.Add((Turf.Types.TurfTypeE)this.SelectedBuildObject, User.View.TargetHit);
                    else
                    {
                        var Object = GameObject.GenerateNew(ObjectType, this.World);
                        Object.SetPosition(User.View.TargetHit);
                        Object.Register();
                    }

                }


                /*if (mouseState.RightKey && !OldMouseState.RightKey)
                {
                    //TODO
                    if (this.Mob.View.TargetStructure != null)
                        this.Mob.View.TargetStructure.Remove(this.Mob.View.TargetHitInside);
                    else if (this.Mob.View.TargetGameObject != null)
                        this.Mob.View.TargetGameObject.Dispose();
                }

                if (mouseState.ScrollWheel > OldMouseState.ScrollWheel)
                {
                    if (SpawnDirection == 3)
                        SpawnDirection = 0;
                    else
                        SpawnDirection++;
                }
                else if (mouseState.ScrollWheel < OldMouseState.ScrollWheel)
                {
                    if (SpawnDirection == 0)
                        SpawnDirection = 3;
                    else
                        SpawnDirection--;
                }*/
            }
            if (Action == Game.Tools.Action.InteractSecondary)
                if (User.View.TargetStructure != null)
                    User.View.TargetStructure.Remove(User.View.TargetHitInside);
                else if (User.View.TargetGameObject != null)
                    User.View.TargetGameObject.Dispose();

            if (Action == Game.Tools.Action.Inspect)
                World.CallUI(this, UICommand.Open, null);
        }
    }
}
