using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures.Machines
{ 
    /// <summary>
    /// Standard door
    /// </summary>
    public class Door : Machine
    {
        public Door(int X, int Y, int Z, Turf.Structure Structure, World World, string ID = "door")
            : base(X, Y, Z, Structure, World, ID)
        {
            this.SpaceRequirement = new bool[2, 2, 1] { { { true }, { true } }, { { true }, { true } } };
            /*this.Shape = new BoxShape(1, 2, 2);
            this.Mass = 10f;
            //this.Anchored = true;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();*/
        }
    }
}
