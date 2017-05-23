using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Structures.Machines
{

    [Attributes.Definition(
        "Large Display",
        "A display")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tools.Wrench))]
    class Display : Machine
    {
        public Display(int X, int Y, int Z, turf.Structure Structure, World World, string ID = "display")
            : base(X, Y, Z, Structure, World, ID)
        {
            this.SpaceRequirement = new bool[2, 2, 1] { { { true }, { true } }, { { true }, { true } } };

            var model = LoadModel(@"Content\Model\Structure\Machinery\Display\display_big.dae");
            model.AssignTexture("Cube", this, LoadTexture(@"Content\Model\Structure\Machinery\Display\display_big.png"));
            model.AssignTexture("Cube.001", this, LoadTexture(@"Content\Model\Structure\Machinery\Display\display_big.png"));
            /*this.Shape = new BoxShape(1, 2, 2);
            this.Mass = 10f;
            //this.Anchored = true;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();*/
        }
    }
}
