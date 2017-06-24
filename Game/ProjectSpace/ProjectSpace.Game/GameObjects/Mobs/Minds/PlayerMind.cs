using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Mobs.Minds
{
    /// <summary>
    /// Type of mind that contains a player
    /// </summary>
    public abstract class PlayerMind : Mind
    {
        /// <summary>
        /// Account-username of this Player
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Usergroup that determins what rights this mind has
        /// </summary>
        [Attributes.Access(datums.UserGroup.Administrator)]
        public datums.UserGroup Group { get; set; }




        public PlayerMind(World world, string ID = "player")
            : base(world, ID)
        {
            this.Group = world.Settings.DefaultUserGroup;
            this.ActionActivated += LocalPlayer_ActionActivated;
            this.ActionDeactivated += LocalPlayer_ActionDeactivated;
        }

        private void LocalPlayer_ActionActivated(Tools.Action Action)
        {
            switch (Action)
            {
                case Tools.Action.Jump:
                    Mob.Jump();
                    break;
                case Tools.Action.Inspect:
                    this.Mob.DoUse(Tools.Action.Inspect);
                    break;
                case Tools.Action.Run:
                    this.Mob.Running = true;
                    break;
                case Tools.Action.Fart:
                    this.Mob.Fart();
                    break;
                case Tools.Action.Drop:
                    this.Mob.Drop();
                    break;
                case Tools.Action.SelectQuickslot1:
                    this.Mob.SelectedQuickslot = 0;
                    break;
                case Tools.Action.SelectQuickslot2:
                    this.Mob.SelectedQuickslot = 1;
                    break;
                case Tools.Action.SelectQuickslot3:
                    this.Mob.SelectedQuickslot = 2;
                    break;
                case Tools.Action.SelectQuickslot4:
                    this.Mob.SelectedQuickslot = 3;
                    break;
                case Tools.Action.SelectQuickslot5:
                    this.Mob.SelectedQuickslot = 4;
                    break;
                case Tools.Action.SelectQuickslot6:
                    this.Mob.SelectedQuickslot = 5;
                    break;
                case Tools.Action.SelectQuickslot7:
                    this.Mob.SelectedQuickslot = 6;
                    break;
                case Tools.Action.SelectQuickslot8:
                    this.Mob.SelectedQuickslot = 7;
                    break;
                case Tools.Action.SelectQuickslot9:
                    this.Mob.SelectedQuickslot = 8;
                    break;
            }
        }

        private void LocalPlayer_ActionDeactivated(Tools.Action Action)
        {
            switch (Action)
            {
                case Tools.Action.Run:
                    this.Mob.Running = false;
                    break;
            }
        }

        /// <summary>
        /// Used to bypass the protection of the ExecuteAction method of the mind only for the player
        /// </summary>
        public void InjectAction(Tools.Action Action, Tools.ActionState ActionState)
        {
            this.ExecuteAction(Action, ActionState);
        }

        /// <summary>
        /// This method is used to apply mouse delta to this player
        /// </summary>
        /// <param name="X">X delta mouse location</param>
        /// <param name="Y">Y delta mouse location</param>
        public void ApplyMouseDelta(int X, int Y)
        {
            this.Mob?.Turn(X, Y);
        }

        public override void Update(double ElapsedTime)
        {
            if (Mob != null)
            {
                //this.Position = Mob.Position;

                //if (this == World.Player || World.Player == null) //This is the player and he has a body -> control the body
                //{
                //Hah its that easy omg
                var mVec = Mob.View.Forward;
                mVec.Normalize();

                //bool run = false;
                //if (keyboardState[Key.ShiftLeft])
                //    run = true;

                Mob.charController.TargetVelocity = new JVector(0, Mob.charController.TargetVelocity.Y, 0);
                //if (Mob.FlyMode)
                //    Mob.charController.TargetVelocity = JVector.Zero;

                lock (ActiveActions)
                {
                    if (this.GetType().IsAssignableFrom(typeof(PlayerTypes.RemotePlayer)))
                    { }
                    if (ActiveActions.Contains(Tools.Action.MoveForward))
                    {
                        mVec = Mob.View.Forward;
                        mVec.Normalize();
                        Mob.Move(mVec);
                    }

                    if (ActiveActions.Contains(Tools.Action.StrafeLeft))
                    {
                        mVec = Mob.View.Forward;
                        mVec.Normalize();
                        mVec = JVector.Transform(new JVector(mVec.X, 0, mVec.Z), JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(90)));
                        Mob.Move(mVec);
                    }

                    if (ActiveActions.Contains(Tools.Action.StrafeRight))
                    {
                        mVec = Mob.View.Forward;
                        mVec.Normalize();
                        mVec = JVector.Transform(new JVector(mVec.X, 0, mVec.Z), JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(270)));
                        Mob.Move(mVec);
                    }


                    if (ActiveActions.Contains(Tools.Action.MoveBack))
                    {
                        mVec = Mob.View.Forward;
                        mVec.Normalize();
                        mVec.Negate();
                        Mob.Move(mVec);
                    }
                }

                //if (keyboardState[Key.S])
                //{
                //    mVec.Negate();

                //    Mob.Move(mVec, run);
                //}

                //if (keyboardState[Key.A])
                //{
                //    mVec = JVector.Transform(new JVector(mVec.X, 0, mVec.Z), JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(90)));
                //    if (keyboardState[Key.S])
                //        mVec.Negate();

                //    Mob.Move(mVec, run);
                //}

                //if (keyboardState[Key.D])
                //{
                //    mVec = JVector.Transform(new JVector(mVec.X, 0, mVec.Z), JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(270)));
                //    if (keyboardState[Key.S])
                //        mVec.Negate();

                //    Mob.Move(mVec, run);
                //}

                //if (keyboardState[Key.Space])
                //    Mob.Jump();

                //var deltaMouseState = new Tools.MouseState(mouseState);
                //deltaMouseState.X -= OldMouseState.X;
                //deltaMouseState.Y -= OldMouseState.Y;
                //ApplyMouseState(deltaMouseState);


                //if (mouseState.X != OldMouseState.X || mouseState.Y != OldMouseState.Y)
                //{

                // Rotates the playerview
                //    Mob.View.AddRotation(
                //        ((float)(mouseState.X - OldMouseState.X) /*+ (World.ClientMode ? 2 : 0)*/) /** (float)ElapsedTime*/, (float)(mouseState.Y - OldMouseState.Y) /** (float)ElapsedTime*/);
                //}



                //Mob.View.MouseOrientation = new JVector((Mob.View.MouseOrientation.X + delta.X) % ((float)Math.PI * 2.0f),
                //                     Math.Max(Math.Min(Mob.View.MouseOrientation.Y + delta.Y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f), 0);

                //Mob.View.MouseOrientation += new JVector(mouseState.X - OldMouseState.X * Mob.View.MouseSensitivity, mouseState.Y - OldMouseState.Y * Mob.View.MouseSensitivity, 0);

                //var Lookat = new JVector();
                //Lookat.X = (float)(Math.Sin((float)Mob.View.MouseOrientation.X) * Math.Cos((float)Mob.View.MouseOrientation.Y));
                //Lookat.Y = (float)Math.Sin((float)Mob.View.MouseOrientation.Y);
                //Lookat.Z = (float)(Math.Cos((float)Mob.View.MouseOrientation.X) * Math.Cos((float)Mob.View.MouseOrientation.Y));

                //Mob.View.Orientation = OutpostOmega.Tools.Convert.Matrix.OpenGL_To_Jitter_4(OpenTK.Matrix4.LookAt(
                //    new OpenTK.Vector3(Mob.View.Position.X, Mob.View.Position.Y, Mob.View.Position.Z),
                //    new OpenTK.Vector3(Mob.View.Position.X, Mob.View.Position.Y, Mob.View.Position.Z) + new OpenTK.Vector3(Lookat.X, Lookat.Y, Lookat.Z),
                //    OpenTK.Vector3.UnitY));
                //}



                // Basic interaction (depends on held item)

                //if (mouseState.LeftKey &&
                //    !OldMouseState.LeftKey)
                //    this.Mob.DoUse(Tools.Action.InteractPrimary);

                //if (mouseState.RightKey &&
                //    !OldMouseState.RightKey)
                //    this.Mob.DoUse(Tools.Action.InteractSecondary);

                //if (mouseState.MiddleKey &&
                //    !OldMouseState.MiddleKey)
                //    this.Mob.DoUse(Tools.Action.InteractTertiary);

                //if (mouseState.MiddleKey &&
                //    !OldMouseState.MiddleKey)
                //    this.Mob.DoUse(Tools.Action.InteractTertiary);

                //if (keyboardState[Key.F] && !InspectPressed)
                //{
                //    this.Mob.DoUse(Tools.Action.Inspect);
                //    InspectPressed = true;
                //}
                //if (!keyboardState[Key.F])
                //    InspectPressed = false;


                /*if (SelectedBuildObject != null)
                {
                    if (mouseState.LeftKey && 
                        //!OldMouseState.LeftKey && 
                        this.Mob.View.TargetStructure != null)
                    {
                        var ObjectType = SelectedBuildObject.GetType();
                        if(typeof(Structures.Structure).IsAssignableFrom(ObjectType))
                        {
                            var structureObject = (Structures.Structure)GameObject.GenerateNew(ObjectType, this.World);

                            var orientation = JMatrix.CreateRotationY(MathHelper.DegreesToRadians(90 * SpawnDirection));
                            structureObject.Orientation = orientation;

                            var nullX = (int)Math.Floor(this.Mob.View.TargetHit.X);
                            var nullY = (int)Math.Floor(this.Mob.View.TargetHit.Y);
                            var nullZ = (int)Math.Floor(this.Mob.View.TargetHit.Z);

                            var center = new JVector((float)nullX + 0.5f, 0, (float)nullZ + 0.5f);

                            center += JVector.Transform(new JVector(-0.5f, 0, -0.5f), orientation);

                            //center += JVector.Transform(new JVector(structureObject.BlockOffset.X, 0, structureObject.BlockOffset.Z), JMatrix.CreateRotationY(MathHelper.DegreesToRadians(90 * SpawnDirection)));
                            center.Y = nullY;

                            structureObject.SetPosition(
                                (int)Math.Floor(center.X),
                                (int)Math.Floor(center.Y),
                                (int)Math.Floor(center.Z));

                            structureObject.Register();
                        }
                        else if (ObjectType == typeof(turf.types.turfTypeE))
                            this.Mob.View.TargetStructure.Add((turf.types.turfTypeE)SelectedBuildObject, this.Mob.View.TargetHit);
                        else
                        {
                            var Object = GameObject.GenerateNew(ObjectType, this.World);
                            Object.SetPosition(this.Mob.View.TargetHit);
                            Object.Register();
                        }

                    }
                    if (mouseState.RightKey && !OldMouseState.RightKey)
                    {
                        //TODO
                        if (this.Mob.View.TargetStructure != null)
                            this.Mob.View.TargetStructure.Remove(this.Mob.View.TargetHitInside);
                        else if (this.Mob.View.TargetGameObject != null)
                            this.Mob.View.TargetGameObject.Dispose();
                    }

                    if (mouseState.ScrollWheel > OldMouseState.ScrollWheel)
                    {
                        if (SpawnDirection == 3)
                            SpawnDirection = 0;
                        else
                            SpawnDirection++;
                    }
                    else if (mouseState.ScrollWheel < OldMouseState.ScrollWheel)
                    {
                        if (SpawnDirection == 0)
                            SpawnDirection = 3;
                        else
                            SpawnDirection--;
                    }
                }*/
            }


            base.Update(ElapsedTime);
        }
    }
}
