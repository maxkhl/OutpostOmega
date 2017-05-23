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
    [Attributes.Definition("Frame", "Used as a construction base")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tool))]
    public class Frame : Structure
    {
        public Frame(int X, int Y, int Z, turf.Structure Structure, World World, string ID = "Frame")
            : base(X, Y, Z, Structure, World, ID)
        {
            //this.IsPassable = true;
            this.SpaceRequirement = new bool[1, 1, 1] { { { true } } };
            this.BlockOffset = new JVector(0.5f, .5f, 0.5f);

            var model = LoadModel(@"Content\Model\Structure\Frame.dae");
            model.AssignTexture("Cube", this, LoadTexture(@"Content\Model\Structure\Frame.png"));

            this.rigidBodyOffset = new JVector(-.5f, -1, -.5f);

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
