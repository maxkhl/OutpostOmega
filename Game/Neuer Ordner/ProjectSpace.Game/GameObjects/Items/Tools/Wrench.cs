using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Items.Tools
{
    /// <summary>
    /// A wrench...?
    /// </summary>
    [Attributes.Definition("Wrench", "Used to do wrench stuff")]
    public class Wrench : Tool
    {
        public Wrench(World world, string ID = "Wrench")
            : base(world, ID)
        {
            LoadModel(@"Content\Model\Items\Tools\Wrench.dae");

            this.Shape = new Jitter.Collision.Shapes.BoxShape(0.249f, 0.018f, 0.071f);
            this.Mass = 20;
            this.Static = false;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }
        /*public override void UseDevice(GameObject Target, Mob User, GameObject.UseAction Action)
        {
            if (Action == UseAction.Secondary)
                if (Target != null && Target.GetType() == typeof(Structures.Frame))
                {
                    var frame = (Structures.Frame)Target;
                    frame.Dispose();
                }

            base.UseDevice(Target, User, Action);
        }*/
    }
}
