using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures.Machines
{
    /// <summary>
    /// A classic vending machine
    /// </summary>
    [Attributes.Definition(
        "Vending Machine",
        "Looks like a usual vending machine. Nothing special about it.")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tools.Wrench))]
    public class Vendingmachine : Machine
    {
        /// <summary>
        /// Available types of machines
        /// </summary>
        public enum machineType
        {
            GalaxPlus,
        }

        /// <summary>
        /// Type of this machine
        /// </summary>
        public machineType Type
        {
            get
            {
                return _Type;
            }
            protected set
            {
                _Type = value;

                switch(value)
                {
                    case machineType.GalaxPlus:
                        this.localPosition = new Jitter.LinearMath.JVector(0, 0, 0);
                        break;
                }
            }

        }
        private machineType _Type;

        public Vendingmachine(int X, int Y, int Z, Turf.Structure Structure, World World, string ID = "vendingmachine")
            : base(X, Y, Z, Structure, World, ID)
        {
            this.SpaceRequirement = new bool[1, 2, 1] { { { true }, { true } } };
            this.BlockOffset = new JVector(0.5f, 1, .35f);

            this.Type = machineType.GalaxPlus;
            var model = LoadModel(@"Content\Model\Vendingmachine\GalaxPlus.dae");
            model.AssignTexture("Cube.010", this, LoadTexture(@"Content\Model\Vendingmachine\GalaxPlus.png"));


            this.rigidBodyOffset = new JVector(-.5f, -2, -.35f);

            this.Shape = new Jitter.Collision.Shapes.BoxShape(1, 2f, .7f);
            this.Mass = 20;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }

        public override void Update(double ElapsedTime)
        {
            base.Update(ElapsedTime);
            //this.BlockOffset = new JVector(0, 1, 0);
        }
    }
}