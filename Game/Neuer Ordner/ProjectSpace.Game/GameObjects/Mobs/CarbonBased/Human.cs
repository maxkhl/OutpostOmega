using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Mobs.CarbonBased
{
    /// <summary>
    /// Hhhhuuuumaaahhhnnnn
    /// </summary>
    public class Human : Carbon
    {
        /// <summary>
        /// LeftHand Item (simple version of what will come)
        /// </summary>
        public Item LeftHand { get; set; }

        /// <summary>
        /// RightHand Item (simple version of what will come)
        /// </summary>
        public Item RightHand
        {
            get
            {
                return _RightHand;
            }
            set
            {
                _RightHand = value;
            }        
        }
        private Item _RightHand;

        public Human(World world, string ID = "human")
            : base(world, 1.5f, 0.60f, 0.19477874f, ID)
        {
            this.View = new View(world); //Give him eyes
            View.Register();
            View.localPosition = new JVector(0, -.25f, 0);
            
            JVector vec = JVector.Down;
            JVector result = JVector.Zero;
            //this._RigidBody.Shape.SupportMapping(ref vec, out result);
            this.View.localPosition = JVector.Up * this.Height; //Set eye position

            var model = LoadModel(@"Content\Model\Character\Human\Hooman.dae");
            //this.AddMesh(model.Meshs["Body"]);
            model.Meshs["Head"].Remove(this);
            //View.AddMesh(model.Meshs["Head"]);

        }

        public override void Update(double ElapsedTime)
        {

            base.Update(ElapsedTime);
        }

        public override void Drop()
        {
            _RightHand = null;
            base.Drop();
        }

        public override void OnDeserialization()
        {

            base.OnDeserialization();
        }
    }
}
