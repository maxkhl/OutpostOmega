using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Items.Tools
{
    /// <summary>
    /// Roberts Knife
    /// </summary>
    [Attributes.Definition("Knife", "Looks like a knife")]
    public class Knife : Tool
    {
        public Knife(World world, string ID = "Knife")
            : base(world, ID)
        {
            var model = LoadModel(@"Content\Model\Items\Tools\Knife.dae");
            model.AssignTexture("Cube", this, LoadTexture(@"Content\Model\Items\Tools\Knife.png"));

            this.Shape = new Jitter.Collision.Shapes.BoxShape(0.4f, 0.018f, 0.071f);
            this.Mass = 20;
            this.Static = false;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }
    }
}
