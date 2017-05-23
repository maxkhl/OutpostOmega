using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures
{
   /* /// <summary>
    /// Basic metal frame
    /// Used as base for further construction
    /// </summary>
    [attributes.Definition("Wall light (fixture)", "Fixture for a wall light")]
    [attributes.Construction(typeof(Frame), typeof(item.tool), @"Content\Model\Structure\Furniture\wall_light_fixture.dae")]
    public class Wall_light_fixture : structure
    {
        public Wall_light_fixture(int X, int Y, int Z, world.Structure Structure, World World, string ID = "wall_light_fixture")
            : base(new bool[1, 1, 1] { { { true } } }, X, Y, Z, Structure, new JVector(0.5f, .5f, 0.1f), World, ID)
        {
            this.Model = @"Content\Model\Structure\Furniture\wall_light_fixture.dae";

            this.rigidBodyOffset = new JVector(0, -.5f, -.3f);

            this.Shape = new Jitter.Collision.Shapes.BoxShape(1f, 1f, .3f);
            this.Mass = 20;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();

        }
    }*/
}
