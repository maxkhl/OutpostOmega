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
    [Attributes.Definition("Table", "Used to put stuff on it")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tool))]
    public class Table : Structure
    {
        public Table(int X, int Y, int Z, turf.Structure Structure, World World, string ID = "table")
            : base(X, Y, Z, Structure, World, ID)
        {
            this.SpaceRequirement = new bool[1, 1, 1] { { { true } } };
            this.BlockOffset = new JVector(0.5f, .5f, 0.5f);

            var model = LoadModel(@"Content\Model\Structure\Furniture\table.dae");
            model.AssignTexture("Cube.001", this, LoadTexture(@"Content\Model\Structure\Furniture\table.png"));

            this.rigidBodyOffset = new JVector(-0.5f, -1, -0.5f);

            this.Shape = new Jitter.Collision.Shapes.BoxShape(1f, 1f, 1f);
            this.Mass = 20;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();

        }
    }
}
