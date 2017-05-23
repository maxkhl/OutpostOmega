using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures
{
    /// <summary>
    /// Basic metal frame
    /// Used as base for further construction
    /// </summary>
    [Attributes.Definition("Void Industries Logo", "The companies logo")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tool))]
    public class VoidLogo : Structure
    {
        public VoidLogo(int X, int Y, int Z, turf.Structure Structure, World World, string ID = "VoidLogo")
            : base(X, Y, Z, Structure, World, ID)
        {
            //this.IsPassable = true;
            this.SpaceRequirement = new bool[2, 2, 1] { { { true }, { true } }, { { true }, { true } } };
            //this.BlockOffset = new JVector(0.5f, .5f, 0.5f);

            var model = LoadModel(@"Content\Model\Structure\VoidLogo.dae");
            this.Shape = MeshToShape(model, model.Meshs["Colission"]);

            //this.rigidBodyOffset = new JVector(-.5f, -1, -.5f);

            //this.Shape = new Jitter.Collision.Shapes.BoxShape(1f, 1f, 1f);
            this.Mass = 20;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();

        }
    }
}
