using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Items.Devices
{
    [Attributes.IconAttribute(@"Content\Model\Items\Devices\Physgun.png")]
    [Attributes.Definition("PhysGun", "Looks like a knife")]
    public class PhysGun : Device
    {
        public PhysGun(World world, string ID = "physGun")
            : base(world, ID)
        {
            var model = LoadModel(@"Content\Model\Items\Devices\Gun.dae");
            model.AssignTexture("Gun", this, LoadTexture(@"Content\Model\Items\Devices\Gun.png"));
            model.AssignTexture("Display", this, LoadTexture(@"Content\Model\Items\Devices\Physgun.png"));

            this.Shape = MeshToShape(model, model.Meshs["Colission"]);
            
            //this.Shape = new Jitter.Collision.Shapes.BoxShape(0.07f, 0.3f, 0.7f);
            this.Mass = 20;
            this.Static = false;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }
        float distance = 0;
        public override void Update(double ElapsedTime)
        {
            if(Target != null)
            {
                var TargetPosition = User.View.Position + User.View.Forward * distance; // This is the point, the object should move to

                if((Target.IsPhysical && Target.Static) || !Target.IsPhysical) // Move static and non-physical objects
                {
                    Target.Move += JVector.Multiply((TargetPosition - Target.Position) * 10, (float)ElapsedTime);
                    //Target.Position = TargetPosition;
                }
                else if(Target.IsPhysical && !Target.Static) // Push dynamic objects
                {
                    Target.RigidBody.LinearVelocity += JVector.Multiply((TargetPosition - Target.Position) * 30 + (Target.RigidBody.LinearVelocity * -1) * 6, (float)ElapsedTime);
                }
            }

            base.Update(ElapsedTime);
        }

        GameObject Target;
        Mob User;
        public override void UseDevice(GameObject Target, Mob User, Game.Tools.Action Action)
        {
            // Basic switch - use once => start move, use again => stop move
            if (this.Target == null && Target != null)
            {
                this.Target = Target;
                this.User = User;
                distance = (this.Position - Target.Position).Length(); // Distance between gun and target
            }
            else
                this.Target = null;

            base.UseDevice(Target, User, Action);
        }
    }
}
