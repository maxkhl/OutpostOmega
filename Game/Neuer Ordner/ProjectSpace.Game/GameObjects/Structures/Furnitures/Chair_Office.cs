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
    [Attributes.Definition("Office chair", "Used to sit on it (office version)")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tool))]
    public class Chair_Office : Structure
    {
        public Chair_Office(int X, int Y, int Z, turf.Structure Structure, World World, string ID = "chair_office")
            : base(X, Y, Z, Structure, World, ID)
        {
            this.SpaceRequirement = new bool[1, 2, 1] { { { true }, { true } } };
            this.BlockOffset = new JVector(0.5f, .5f, 0.5f);

            LoadModel(@"Content\Model\Structure\Furniture\chair_office.dae");

            this.rigidBodyOffset = new JVector(-0.5f, -1, -0.5f);

            this.Shape = new Jitter.Collision.Shapes.BoxShape(1f, 2f, 1f);
            this.Mass = 20;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();

        }
    }
}
