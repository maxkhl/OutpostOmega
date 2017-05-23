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
    public class CharacterController
    {
        private World World;

        /// <summary>
        /// Velocity, the character should try to accelerate to
        /// </summary>
        public JVector TargetVelocity = JVector.Zero;

        /// <summary>
        /// The actual velocity
        /// </summary>
        public JVector Velocity = JVector.Zero;


        /// <summary>
        /// If the character should try to jump
        /// </summary>
        public bool TryJump { get; set; }

        /// <summary>
        /// Relative feet height
        /// </summary>
        public float FeetHeight { get; set; }

        /// <summary>
        /// Relative feet sidecheck height
        /// </summary>
        public float FeetSideHeight { get; set; }

        /// <summary>
        /// Radius 
        /// </summary>
        public float Radius { get; set; }

        public JVector Position { get; set; }

        public JVector PositionDelta { get; set; }

        private bool TouchingGround = false;

        private JVector[] SideCheckTargets;
        private bool[] SideCheckHits;

        public bool Fly { get; set; }

        public Collision.RaycastCallback RayCallback = null;

        public CharacterController(World world, JVector Position, float Width, float Height)
        {
            this.World = world;

            this.Position = Position;
            PositionDelta = JVector.Zero;

            FeetHeight = 0;
            FeetSideHeight = 0.3f; //step-height = 30cm

            SetSize(Width, Height);
            /*SideCheckHits = new bool[8];
            SideCheckTargets = new JVector[8]
            {
                JVector.Forward,
                JVector.Transform(JVector.Forward, JMatrix.CreateRotationY((float)JMath.DegreeToRadian(45))),
                JVector.Transform(JVector.Forward, JMatrix.CreateRotationY((float)JMath.DegreeToRadian(90))),
                JVector.Transform(JVector.Forward, JMatrix.CreateRotationY((float)JMath.DegreeToRadian(135))),
                JVector.Transform(JVector.Forward, JMatrix.CreateRotationY((float)JMath.DegreeToRadian(180))),
                JVector.Transform(JVector.Forward, JMatrix.CreateRotationY((float)JMath.DegreeToRadian(225))),
                JVector.Transform(JVector.Forward, JMatrix.CreateRotationY((float)JMath.DegreeToRadian(270))),
                JVector.Transform(JVector.Forward, JMatrix.CreateRotationY((float)JMath.DegreeToRadian(315))),
            };*/
        }

        public void SetSize(float Width, float Height)
        {

            Radius = Width / 2;


            int NumChecks = 8;
            int NumLayer = 3;

            SideCheckHits = new bool[NumChecks * NumLayer];
            SideCheckTargets = new JVector[NumChecks * NumLayer];

            for (int y = 0; y < NumLayer; y++)
            {
                for (int i = 0; i < NumChecks; i++)
                {
                    var DirVec = JVector.Transform(JVector.Forward * Radius, JMatrix.CreateRotationY((float)JMath.DegreeToRadian((360 / NumChecks) * i)));
                    DirVec.Y = (Height / (float)NumLayer) * (float)y;

                    SideCheckTargets[i + NumChecks * y] = DirVec;
                }
            }

        }

        RigidBody bottomBody = null;
        JVector bottomBodyLastPosition = JVector.Zero;
        public void Update(float ElapsedTime)
        {
            RigidBody resultingBody = null;
            JVector normal; float frac;
            TouchingGround = World.CollisionSystem.Raycast((Position + JVector.Down * FeetHeight) + JVector.Up, JVector.Down, RayCallback,
                out resultingBody, out normal, out frac);

            if (TargetVelocity != JVector.Zero)
            { }

            if (frac > 1.1f)
               TouchingGround = false;

            if (!Fly)
            {
                if (!TouchingGround)
                    TargetVelocity.Y = World.Gravity.Y * 0.1f;
                else
                {
                    TargetVelocity = new JVector(TargetVelocity.X, 0, TargetVelocity.Z);
                    Velocity = new JVector(Velocity.X, 1-frac, Velocity.Z);

                    if(bottomBody == null && resultingBody != null || bottomBody != resultingBody)
                    {
                        bottomBody = resultingBody;
                        bottomBodyLastPosition = resultingBody.Position;
                    }
                    else if (bottomBody == resultingBody && (bottomBodyLastPosition - resultingBody.Position) != JVector.Zero)
                    {
                        var mVec = resultingBody.Position - bottomBodyLastPosition;
                        mVec.Y = 0;
                        mVec = ((JVector)bottomBody.Tag.GetType().GetProperty("LastMove").GetValue(bottomBody.Tag));
                        mVec.Negate();
                        mVec += bottomBody.LinearVelocity;

                        this.PositionDelta += mVec;
                        bottomBodyLastPosition = resultingBody.Position;
                    }
                    else
                        bottomBody = resultingBody;

                    /*if (resultingBody != null)
                        this.Position += resultingBody.AngularVelocity;*/
                }

                if(TryJump)
                {
                    if(TouchingGround)
                        Velocity.Y = 0.09f;
                    else
                    {
                    }
                    TryJump = false;
                }

                JVector diff = new JVector(
                    (TargetVelocity.X - Velocity.X),
                    (TargetVelocity.Y - Velocity.Y) * 0.03f,
                    (TargetVelocity.Z - Velocity.Z)

                    );


                Velocity += diff * (ElapsedTime * 10);
                //Check feet sidecollision
                for (int i = 0; i < SideCheckTargets.Length; i++)
                {
                    var Origin = Position + (new JVector(SideCheckTargets[i].X, 0, SideCheckTargets[i].Z) * 0.9f) + (JVector.Up * FeetSideHeight) + (JVector.Up * SideCheckTargets[i].Y);
                    var Target = SideCheckTargets[i] - (new JVector(SideCheckTargets[i].X, 0, SideCheckTargets[i].Z) * 0.9f);

                    bool SideHit = World.CollisionSystem.Raycast(
                        Origin, 
                        Target, 
                        RayCallback,
                        out resultingBody, out normal, out frac);
                    if (SideHit)
                    {
                        if (frac > 2)
                            continue;

                        var RejectedVelocity = JVector.Rejection(Velocity, normal); // Movement that is going over the surface of the rayhit
                        var PressureVelocity = Velocity - RejectedVelocity; // Movement that is pressing against/away from the hit surface
                        PressureVelocity.Negate();

                        var DotProduct = JVector.Dot(PressureVelocity, normal); // Get the cross-product to determine if we are moving into the surface or away from it

                        if (DotProduct >= 0)
                        {
                            Velocity = RejectedVelocity;
                            //Velocity += normal * frac * 0.003f;
                            SideCheckHits[i] = true;
                            //JVector.Transform(Velocity, JMatrix.CreateTranslation(normal));
                        }

                    }
                    else
                        SideCheckHits[i] = false;
                }

                if(Velocity != JVector.Zero)
                { }
                PositionDelta += Velocity;




            }
            else // FlyMode
            {
                if (TryJump)
                {
                    Velocity.Y += 0.1f;
                    TryJump = false;
                }
                Velocity = TargetVelocity;
                PositionDelta += TargetVelocity;
            }
        }


        public void DebugDraw(IDebugDrawer debugDrawer)
        {
            for (int i = 0; i < SideCheckTargets.Length; i++)
            {
                byte R = 255,
                     G = 255,
                     B = 255;

                if(SideCheckHits[i])
                {
                    R = 0;
                    G = 255;
                    B = 0;
                }

                debugDrawer.DrawLine(Position + (new JVector(SideCheckTargets[i].X, 0, SideCheckTargets[i].Z) * 0.9f) + JVector.Up * FeetSideHeight + (JVector.Up * SideCheckTargets[i].Y), (Position + JVector.Up * FeetSideHeight) + SideCheckTargets[i], R, G, B);
            }

            debugDrawer.DrawLine(Position, Position + Velocity * 5, 255, 0, 0);
        }
    }
}
