using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;

namespace Jitter.Dynamics.Constraints
{
    public class CharacterControlleralt : Constraint
    {
        private const float JumpVelocity = 1.5f;

        private float feetPosition;

        public CharacterControlleralt(World world, RigidBody body)
            : base(body, null)
        {
            this.World = world;
            
            /* OK we need to do that using rays.
             * The Capsule could still stay there but as a static object.
             * We need a ray pointing to the ground and the air for height and jumping and then we need a
             * star out of rays near the feet and near the head pointing into 8 directions each. This will find the
             * walls and stairs and everything around the character */
            // determine the position of the feets of the character
            // this can be done by supportmapping in the down direction.
            // (furthest point away in the down direction)
            JVector vec = JVector.Down;
            JVector result = JVector.Zero;

            body.Material.Restitution = -1.0f;
            body.Material.KineticFriction = 0.0f;
            body.Material.StaticFriction = 0.0f;

            // Note: the following works just for normal shapes, for multishapes (compound for example)
            // you have to loop through all sub-shapes -> easy.
            body.Shape.SupportMapping(ref vec, out result);

            // feet position is now the distance between body.Position and the feets
            // Note: the following '*' is the dot product.
            feetPosition = result * JVector.Down;
        }

        public World World { private set; get; }
        public JVector TargetVelocity
        {
            get
            {
                return _TargetVelocity;
            }
            set
            {
                body1.IsActive = true;
                _TargetVelocity = value;
            }
        }
        private JVector _TargetVelocity { get; set; }
        public bool TryJump
        {
            get
            {
                return _TryJump;
            }
            set
            {
                if (value)
                    body1.IsActive = true;

                _TryJump = value;
            }
        }
        private bool _TryJump { get; set; }
        public RigidBody BodyWalkingOn { get; set; }

        private JVector deltaVelocity = JVector.Zero;
        private bool shouldIJump = false;

        public override void PrepareForIteration(float timestep)
        {

            RigidBody resultingBody = null;
            JVector normal; float frac;
            bool result = World.CollisionSystem.Raycast(Body1.Position + JVector.Down * (feetPosition - 0.1f), JVector.Down, RaycastCallback,
                out resultingBody, out normal, out frac);

            BodyWalkingOn = (result && frac <= 0.2f) ? resultingBody : null;
            shouldIJump = (result && frac <= 0.2f && Body1.LinearVelocity.Y < JumpVelocity && TryJump);
        }

        private bool RaycastCallback(RigidBody body, JVector normal, float fraction)
        {
            // prevent the ray to collide with ourself!
            return (body != this.Body1);
        }

        public override void Iterate()
        {
            deltaVelocity = TargetVelocity - Body1.LinearVelocity;
            deltaVelocity.Y = 0.0f;

            // determine how 'stiff' the character follows the target velocity
            //if (BodyWalkingOn == null) deltaVelocity *= 0.0001f;
            //else deltaVelocity *= 0.02f;
            deltaVelocity *= 0.1f;

            if (deltaVelocity.LengthSquared() != 0.0f)
            {
                // activate it, in case it fall asleep :)
                Body1.IsActive = true;
                Body1.ApplyImpulse(deltaVelocity * Body1.Mass);
            }

            if (shouldIJump)
            {
                Body1.IsActive = true;
                Body1.ApplyImpulse(JumpVelocity * JVector.Up * Body1.Mass);
                //System.Diagnostics.Debug.WriteLine("JUMP! " + DateTime.Now.Second.ToString());

                if (!BodyWalkingOn.IsStatic)
                {
                    BodyWalkingOn.IsActive = true;
                    // apply the negative impulse to the other body
                    BodyWalkingOn.ApplyImpulse(-1.0f * JumpVelocity * JVector.Up * Body1.Mass);
                }

            }
        }

        public override void DebugDraw(IDebugDrawer drawer)
        {
            base.DebugDraw(drawer);
        }
    }
}
