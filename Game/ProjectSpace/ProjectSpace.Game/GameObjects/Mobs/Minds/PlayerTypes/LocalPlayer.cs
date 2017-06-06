using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Mobs.Minds.PlayerTypes
{
    public class LocalPlayer : PlayerMind
    {


        public LocalPlayer(World world, string ID = "localplayer")
            : base(world, ID)
        {

        }
        



        public override void OnDeserialization()
        {
            OldMouseState = new Tools.MouseState();
            base.OnDeserialization();
        }


        public Tools.KeybeardState OldKeyboardState = new Tools.KeybeardState(OpenTK.Input.Keyboard.GetState());

        private bool InspectPressed = false;
        public Vector2 lastMousePos = new Vector2();
        public void Update(Tools.MouseState mouseState, Tools.KeybeardState keyboardState, double ElapsedTime)
        {
            if (Mob != null)
            {
                this.Position = Mob.Position;

                //if (this == World.Player || World.Player == null) //This is the player and he has a body -> control the body
                //{
                //Hah its that easy omg
                var mVec = Mob.View.Forward;

                if (!Mob.FlyMode)
                {
                    mVec.Y = 0;
                }
                mVec.Normalize();

                bool run = false;
                if (keyboardState[Key.ShiftLeft])
                    run = true;

                Mob.charController.TargetVelocity = new JVector(0, Mob.charController.TargetVelocity.Y, 0);
                if (Mob.FlyMode)
                    Mob.charController.TargetVelocity = JVector.Zero;

                if (keyboardState[Key.W])
                {
                    Mob.Move(mVec, run);
                }

                if (keyboardState[Key.S])
                {
                    mVec.Negate();

                    Mob.Move(mVec, run);
                }

                if (keyboardState[Key.A])
                {
                    mVec = JVector.Transform(new JVector(mVec.X, 0, mVec.Z), JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(90)));
                    if (keyboardState[Key.S])
                        mVec.Negate();

                    Mob.Move(mVec, run);
                }

                if (keyboardState[Key.D])
                {
                    mVec = JVector.Transform(new JVector(mVec.X, 0, mVec.Z), JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(270)));
                    if (keyboardState[Key.S])
                        mVec.Negate();

                    Mob.Move(mVec, run);
                }

                if (keyboardState[Key.Space])
                    Mob.Jump();

                if (mouseState.X != OldMouseState.X || mouseState.Y != OldMouseState.Y)
                {

                    // Rotates the playerview
                    Mob.View.AddRotation(
                        ((float)(mouseState.X - OldMouseState.X) /*+ (World.ClientMode ? 2 : 0)*/) /** (float)ElapsedTime*/, (float)(mouseState.Y - OldMouseState.Y) /** (float)ElapsedTime*/);
                }



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
                if (mouseState.LeftKey &&
                    !OldMouseState.LeftKey)
                    this.Mob.DoUse(UseAction.Primary);

                if (mouseState.RightKey &&
                    !OldMouseState.RightKey)
                    this.Mob.DoUse(UseAction.Secondary);

                if (mouseState.MiddleKey &&
                    !OldMouseState.MiddleKey)
                    this.Mob.DoUse(UseAction.Tertiary);

                if (mouseState.MiddleKey &&
                    !OldMouseState.MiddleKey)
                    this.Mob.DoUse(UseAction.Tertiary);

                if (keyboardState[Key.F] && !InspectPressed)
                {
                    this.Mob.DoUse(UseAction.Inspect);
                    InspectPressed = true;
                }
                if (!keyboardState[Key.F])
                    InspectPressed = false;


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

            OldMouseState = new Tools.MouseState(mouseState);

            base.Update(ElapsedTime);
        }


        public override void KeyPress(Key Key, bool IsRepeat)
        {
            if (this.Mob != null && this.Mob.View != null)
            {

                /*if (Key == Key.B && !IsRepeat)
                    BuildMode = !BuildMode;*/

                if (Key == OpenTK.Input.Key.F && !IsRepeat)
                    this.Mob.Fart();

                if (Key == Key.Q && !IsRepeat)
                    this.Mob.Drop();

                switch (Key)
                {
                    case OpenTK.Input.Key.Number1:
                        this.Mob.SelectedQuickslot = 0;
                        break;
                    case OpenTK.Input.Key.Number2:
                        this.Mob.SelectedQuickslot = 1;
                        break;
                    case OpenTK.Input.Key.Number3:
                        this.Mob.SelectedQuickslot = 2;
                        break;
                    case OpenTK.Input.Key.Number4:
                        this.Mob.SelectedQuickslot = 3;
                        break;
                    case OpenTK.Input.Key.Number5:
                        this.Mob.SelectedQuickslot = 4;
                        break;
                    case OpenTK.Input.Key.Number6:
                        this.Mob.SelectedQuickslot = 5;
                        break;
                    case OpenTK.Input.Key.Number7:
                        this.Mob.SelectedQuickslot = 6;
                        break;
                    case OpenTK.Input.Key.Number8:
                        this.Mob.SelectedQuickslot = 7;
                        break;
                    case OpenTK.Input.Key.Number9:
                        this.Mob.SelectedQuickslot = 8;
                        break;
                }
            }

            base.KeyPress(Key, IsRepeat);
        }

    }
}
