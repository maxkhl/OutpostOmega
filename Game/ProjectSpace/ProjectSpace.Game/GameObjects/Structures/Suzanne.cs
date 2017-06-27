using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures
{
    /// <summary>
    /// Just a teststructure
    /// </summary>
    public class Suzanne : Structure
    {
        public Suzanne(int X, int Y, int Z, Turf.Structure Structure, World world, string ID = "Suzanne")
            : base(X, Y, Z, Structure, world, ID)
        {
            this.SpaceRequirement = new bool[1, 1, 1] { { { true } } };
            this.BlockOffset = JVector.Zero;
            LoadModel(@"Content\Model\BoxUV.dae");
            /*this.Shape = new Jitter.Collision.Shapes.BoxShape(1, 1, 1);
            this.Mass = 50;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();*/
        }
    }
}
