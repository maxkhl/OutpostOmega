using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using OpenTK;

namespace OutpostOmega.Game.GameObjects.Items.Devices
{
    [Attributes.IconAttribute(@"Content\Model\Items\Devices\Write.png")]
    [Attributes.Definition("Drawer", "Used to draw stuff")]
    public class Drawer : Device
    {
        public Drawer(World world, string ID = "drawer")
            : base(world, ID)
        {
            var model = LoadModel(@"Content\Model\Items\Devices\Gun.dae");

            this.Shape = MeshToShape(model, model.Meshs["Colission"]);

            model.Meshs["Display"][this].Texture = (Content.Texture)World.ContentManager.Load(@"Content\Model\Items\Devices\Write.png");

            this.Mass = 20;
            this.Static = false;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }

        public override void Update(double ElapsedTime)
        {
            if (Holder != null)
            {
                if (Holder.View.Forward.X > 0)
                {
                    if (Holder.View.Forward.X > Holder.View.Forward.Z)
                        Holder.Mind.SpawnDirection = 0;
                    else if (Holder.View.Forward.X < Holder.View.Forward.Z)
                        Holder.Mind.SpawnDirection = 2;
                }
                else
                    if (Holder.View.Forward.X > Holder.View.Forward.Z)
                        Holder.Mind.SpawnDirection = 1;
                    else if (Holder.View.Forward.X < Holder.View.Forward.Z)
                        Holder.Mind.SpawnDirection = 3;
            }

            base.Update(ElapsedTime);
        }

        public override void UseDevice(GameObject Target, Mob User, UseAction Action)
        {
            if (User.Mind != null && Action == UseAction.Primary)
            {
                if (User.View.TargetStructure != null)
                {
                    var hitPoint = User.View.TargetHitInside;
                    var hitBlock = User.View.TargetStructure[hitPoint.X, hitPoint.Y, hitPoint.Z];

                    if(turf.Block.IsVisible(hitBlock))
                    {
                        var Chunk = User.View.TargetStructure.GetChunkAtPos(hitPoint);
                        var BlockPos = Chunk.Position + new JVector(hitBlock.X, hitBlock.Y, hitBlock.Z) + new JVector(0.5f);

                        if (User.View.TargetHitNormal == JVector.Forward)
                            hitBlock.UVFront = AddBlockID(hitBlock.UVFront);

                        if (User.View.TargetHitNormal == JVector.Backward)
                            hitBlock.UVBack = AddBlockID(hitBlock.UVBack);

                        if (User.View.TargetHitNormal == JVector.Left)
                            hitBlock.UVLeft = AddBlockID(hitBlock.UVLeft);

                        if (User.View.TargetHitNormal == JVector.Right)
                            hitBlock.UVRight = AddBlockID(hitBlock.UVRight);

                        if (User.View.TargetHitNormal == JVector.Up)
                            hitBlock.UVTop = AddBlockID(hitBlock.UVTop);

                        if (User.View.TargetHitNormal == JVector.Down)
                            hitBlock.UVBottom = AddBlockID(hitBlock.UVBottom);

                        User.View.TargetStructure[hitPoint.X, hitPoint.Y, hitPoint.Z] = hitBlock;
                        User.View.TargetStructure.GetChunkAtPos(hitPoint).NeedsRender = true;
                    }


                    //User.View.TargetStructure.Add((turf.types.turfTypeE)User.Mind.SelectedBuildObject, User.View.TargetHit);
                }
            }
            /*if (Action == UseAction.Secondary)
                if (User.View.TargetStructure != null)
                    User.View.TargetStructure.Remove(User.View.TargetHitInside);
                else if (User.View.TargetGameObject != null)
                    User.View.TargetGameObject.Dispose();
            if (Action == UseAction.Tertiary)
                if (User.Mind.SpawnDirection == 3)
                    User.Mind.SpawnDirection = 0;
                else
                    User.Mind.SpawnDirection++;*/
        }
        private short AddBlockID(short ID)
        {
            if (ID < 1)
                ID = 2;
            else if (ID < 4)
                ID++;
            else
                ID = 1;

            return ID;
        }
    }
}
