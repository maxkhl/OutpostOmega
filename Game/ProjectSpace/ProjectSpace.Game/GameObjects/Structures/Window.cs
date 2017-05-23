using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures
{
    /// <summary>
    /// A basic window. Use this to enhance optics/functionality
    /// Possible use would be reinforced windows or windows with different styles (f.e. plasma window)
    /// </summary>
    [Attributes.Definition("Window", "Basic window")]
    [Attributes.Construction(typeof(Frame), typeof(Items.Tool))]
    public class Window : Structure
    {
        public Window(int X, int Y, int Z, turf.Structure Structure, World World, string ID = "Window")
            : base(X, Y, Z, Structure, World, ID)
        {
            //this.IsPassable = true;
            this.SpaceRequirement = new bool[1, 1, 1] { { { true } } };
            this.BlockOffset = new JVector(.5f, .5f, .5f);

            var Model = LoadModel(@"Content\Model\Structure\Window.dae");
            Model.AssignTexture("Cube", this, LoadTexture(@"Content\Model\Structure\Window.png"));
            Model.Meshs["Cube"][this].UseAlpha = true;

            this.rigidBodyOffset = new JVector(0);

            this.Shape = new Jitter.Collision.Shapes.BoxShape(1f, 1f, 1f);
            this.Mass = 20;
            this.Static = true;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();

        }

        //bool first;
        public override void Update(double ElapsedTime)
        {
            /*if (!first)
            {
                var Model = (Content.Model)World.ContentManager.Load(@"Content\Model\Structure\Window.dae");
                //var Model = LoadModel(@"Content\Model\Structure\Window.dae");
                if (Model.Meshs["Cube"][this] != null)
                {
                    Model.Meshs["Cube"][this].UseAlpha = true;
                    first = true;
                }
            }*/

            base.Update(ElapsedTime);
        }
    }
}

