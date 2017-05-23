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
    [Attributes.Definition("Builder", "Used to build stuff")]
    public class Builder : Device
    {
        public Builder(World world, string ID = "builder")
            : base(world, ID)
        {
            var model = LoadModel(@"Content\Model\Items\Devices\Gun.dae");

            this.Shape = MeshToShape(model, model.Meshs["Colission"]);

            model.Meshs["Display"][this].Texture = (Content.Texture)World.ContentManager.Load(@"Content\Model\Items\Devices\Spawner.png");

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

        bool FirstClick = true;
        JVector FirstPosition;
        public override void UseDevice(GameObject Target, Mob User, UseAction Action)
        {
            if (User.Mind != null && User.Mind.SelectedBuildObject != null && Action == UseAction.Primary)
            {
                if (User.View.TargetStructure != null)
                {
                    var ObjectType = User.Mind.SelectedBuildObject.GetType();
                    if (ObjectType == typeof(turf.types.turfTypeE))
                    {
                        if(FirstClick)
                        {
                            FirstPosition = User.View.TargetHit;
                            FirstClick = false;
                        }
                        else
                        {
                            var diff = (FirstPosition - User.View.TargetHit);
                            int xStep = diff.X > 0 ? 1 : -1,
                                yStep = diff.Y > 0 ? 1 : -1,
                                zStep = diff.Z > 0 ? 1 : -1;

                            if (diff.X < 1 && diff.X > 0) diff.X = 1;
                            if (diff.X > -1 && diff.X < 0) diff.X = -1;

                            if (diff.Y < 1 && diff.Y > 0) diff.Y = 1;
                            if (diff.Y > -1 && diff.Y < 0) diff.Y = -1;

                            if (diff.Z < 1 && diff.Z > 0) diff.Z = 1;
                            if (diff.Z > -1 && diff.Z < 0) diff.Z = -1;

                            for (int x = 0; x < diff.X && diff.X > 0 || x > diff.X && diff.X < 0; x += xStep)
                                for (int y = 0; y < diff.Y && diff.Y > 0 || y > diff.Y && diff.Y < 0; y += yStep)
                                    for (int z = 0; z < diff.Z && diff.Z > 0 || z > diff.Z && diff.Z < 0; z += zStep)
                                    {
                                        var pos = FirstPosition + new JVector(x * -1, y * -1, z * -1);
                                        User.View.TargetStructure.Add((turf.types.turfTypeE)User.Mind.SelectedBuildObject, pos);
                                    }

                                FirstClick = true;
                        }
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
            if (Action == UseAction.Secondary)
                if (User.View.TargetStructure != null)
                    User.View.TargetStructure.Remove(User.View.TargetHitInside);
                else if (User.View.TargetGameObject != null)
                    User.View.TargetGameObject.Dispose();
            if (Action == UseAction.Tertiary)
                if (User.Mind.SpawnDirection == 3)
                    User.Mind.SpawnDirection = 0;
                else
                    User.Mind.SpawnDirection++;
        }
    }
}
