using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Items.Stacks
{
    [Attributes.IconAttribute(@"Content\Model\Items\Stacks\MetalSheetsIcon.png")]
    [Attributes.Definition("Mineral Stack", "Its a stack of minerals")]
    public class Mineral : Stack
    {
        public Mineral(World world, string ID = "Mineral")
            : base(world, ID)
        {
            var model = LoadModel(@"Content\Model\Items\Stacks\Metalsheets.dae");
            model.AssignTexture("Cube.010", this, LoadTexture(@"Content\Model\Items\Stacks\Metalsheets.png"));

            this.Shape = MeshToShape(model, model.Meshs["Colission"]);

            //this.Shape = new Jitter.Collision.Shapes.BoxShape(0.07f, 0.3f, 0.7f);
            this.Mass = 200;
            this.Static = false;
            this.PhysicCreateMaterial(0.5f, 0.8f, 0.3f);
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }
    }
}
