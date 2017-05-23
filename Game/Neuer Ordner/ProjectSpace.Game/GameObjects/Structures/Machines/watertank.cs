using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures.Machines
{
    /// <summary>
    /// A watertank. Can store water
    /// </summary>
    [Attributes.Definition(
        "Watertank",
        "Caution! Might contain water.")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tools.Wrench))]
    public class Watertank : Machine
    {

        public Watertank(int X, int Y, int Z, turf.Structure Structure, World World, string ID = "watertank")
            : base(X, Y, Z, Structure, World, ID)
        {
            this.SpaceRequirement = new bool[2, 2, 2] { { { true, true }, { true, true } }, { { true, true }, { true, true } } };
            this.BlockOffset = new JVector(0f, 1, 1f);
            this.rigidBodyOffset = new JVector(0, -2, -1.4f);

            LoadModel(@"Content\Model\Structure\Machinery\watertank.dae");

            this.Shape = new Jitter.Collision.Shapes.BoxShape(2, 2f, 2f);
            this.Mass = 20;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }
    }
}