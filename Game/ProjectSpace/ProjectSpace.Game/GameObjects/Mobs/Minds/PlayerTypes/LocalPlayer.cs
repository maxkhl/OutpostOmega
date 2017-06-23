using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK;
using Jitter.LinearMath;
using OutpostOmega.Game.Tools;

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
            //OldMouseState = new Tools.MouseState();
            base.OnDeserialization();
        }

        /*public Tools.MouseState DeltaMouseState = new Tools.MouseState();
        public override void ApplyMouseState(Tools.MouseState MouseState)
        {
            if (DeltaMouseState == null) DeltaMouseState = new Tools.MouseState();

            lock (DeltaMouseState)
            {
                int x = DeltaMouseState.X + MouseState.X,
                    y = DeltaMouseState.Y + MouseState.Y;

                DeltaMouseState = new Tools.MouseState(MouseState);
                DeltaMouseState.X = x;
                DeltaMouseState.Y = y;
            }

            base.ApplyMouseState(MouseState);
        }*/

        
        //public void Action(Tools.Action actionType, Tools.ActionState actionState)
        //{
        //    var mVec = Mob.View.Forward;

        //    if (!Mob.FlyMode)
        //    {
        //        mVec.Y = 0;
        //    }
        //    mVec.Normalize();
        //}


        //public Tools.KeybeardState OldKeyboardState = new Tools.KeybeardState(OpenTK.Input.Keyboard.GetState());

        //private bool InspectPressed = false;
        //public Vector2 lastMousePos = new Vector2();
        


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
