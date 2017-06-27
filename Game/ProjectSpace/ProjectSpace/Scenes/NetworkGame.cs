using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OutpostOmega.Game;
using OutpostOmega.Network;
using OutpostOmega.Game.GameObjects.Mobs;
using OutpostOmega.Game.GameObjects.Mobs.Minds;
using OutpostOmega.Game.GameObjects.Mobs.Minds.PlayerTypes;
using OpenTK.Input;
using System.Diagnostics;

namespace OutpostOmega.Scenes
{
    class NetworkGame : Game
    {
        public GameNetClient Client;

        private Drawing.UI.Chat Chat;

        Stopwatch sendTimer;

        /// <summary>
        /// True if mouse is active, false if camera is controlled
        /// </summary>
        public new bool MouseMode
        {
            get
            {
                return _mouseMode;
            }
            set
            {
                _mouseMode = value;

                if (value)
                    Game.UnlockCursor();
                else
                    Game.LockCursor();
            }
        }
        private bool _mouseMode = true;


        public NetworkGame(GameNetClient Client, MainGame game, World world)
            : base(game, world)
        {
            this.Client = Client;
            this.KeyStateChanged += NetworkGame_KeyStateChanged;
            this.MouseMoved += NetworkGame_MouseMoved;
            world.ClientMode = true;
            sendTimer = new Stopwatch();

            //if(world.Player == null)
            //{
                world.Player = (from gobj in world.AllGameObjects
                                where gobj.GetType().IsAssignableFrom(typeof(LocalPlayer))
                                select (LocalPlayer)gobj).FirstOrDefault();
                if (world.Player == null)
                    throw new Exception("No mind for the player found. Need to add joinmenu or spectator");
            //}                
        }


        /// <summary>
        /// Sends the input over the network
        /// </summary>
        private void NetworkGame_MouseMoved(int X, int Y)
        {
            if (!this.MouseMode && this.Game.Focused) // Only pass if mousemode is disabled
            {
                Client.AddInput(X, Y);
            }
        }

        /// <summary>
        /// Sends actions over the network
        /// </summary>
        private void NetworkGame_KeyStateChanged(OutpostOmega.Game.Tools.Action action, OutpostOmega.Game.Tools.ActionState actionState)
        {
            if (!this.Game.Focused) return;

            if (!this.MouseMode) // Only pass if mousemode is disabled
            {
                Client.AddInput(action, actionState);
            }
        }

        /// <summary>
        /// Disposes this network game and disconnects the client
        /// </summary>
        public override void Dispose()
        {
            this.Client.Disconnect();
            base.Dispose();
        }

        const double SendUpdatesPerSecond = 20;
        //OpenTK.Input.MouseState OldMouseState = OpenTK.Input.Mouse.GetState();
        protected override void UpdateScene()
        {
            if (!sendTimer.IsRunning) sendTimer.Start();

            if (1000 / SendUpdatesPerSecond <= sendTimer.ElapsedMilliseconds)
            {
                sendTimer.Reset();

                if (Client.Output.Count > 0 && Client.Output.TryDequeue(out string text))
                    Chat.Message(text);


                Client.InputPackageQueue.Add(
                    new InputPackage(Client.Clock, World.Player.Mob.Position));

                Client.InputPackageQueue.Add(
                    new InputPackage(Client.Clock, World.Player.Mob.View.Orientation));

                Client.InputPackageQueueFlush();

                //var mouseState = OpenTK.Input.Mouse.GetState();

                //if (MouseMode || !Game.Focused)
                //{
                //}
                //else
                //{

                //    /*lock (World.Player.DeltaMouseState)
                //    {
                //        Client.AddInput(World.Player.DeltaMouseState.X, World.Player.DeltaMouseState.Y);
                //        //Client.SendMouseState(new OutpostOmega.Game.Tools.MouseState(), new OutpostOmega.Game.Tools.MouseState());
                //        World.Player.DeltaMouseState.X = 0;
                //        World.Player.DeltaMouseState.Y = 0;
                //    }*/
                //    /*var outgoingMouseMessage = Client.GetOM(Command.Data, SecondCommand.InputMouseDelta);
                //    outgoingMouseMessage.Write(Client.Clock);
                //    //outgoingMouseMessage.Write(mouseState.LeftButton == ButtonState.Pressed);
                //    //outgoingMouseMessage.Write(mouseState.MiddleButton == ButtonState.Pressed);
                //    //outgoingMouseMessage.Write(mouseState.RightButton == ButtonState.Pressed);
                //    outgoingMouseMessage.Write(mouseState.X - OldMouseState.X);
                //    outgoingMouseMessage.Write(mouseState.Y - OldMouseState.Y);
                //    Client.netClient.SendMessage(outgoingMouseMessage, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
                //    */
                //    OldMouseState = mouseState;

                //}
            }


            //if (MouseMode || !Game.Focused)
            //{
            //}
            //else
            //{
            //    var kstate = OpenTK.Input.Keyboard.GetState();
            //    var outgoingMessage = Client.GetOM(Command.Data, SecondCommand.Input);
            //    outgoingMessage.Write(Client.Clock);
            //    uint ret = 0;
            //    ret += SendKeyStateChange(outgoingMessage, Key.W, kstate);
            //    ret += SendKeyStateChange(outgoingMessage, Key.A, kstate);
            //    ret += SendKeyStateChange(outgoingMessage, Key.S, kstate);
            //    ret += SendKeyStateChange(outgoingMessage, Key.D, kstate);
            //    ret += SendKeyStateChange(outgoingMessage, Key.ShiftLeft, kstate);
            //    ret += SendKeyStateChange(outgoingMessage, Key.Space, kstate);
            //    if (ret > 0)
            //        Client.netClient.SendMessage(outgoingMessage, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
            //}
            base.UpdateScene();
        }

        Dictionary<Key, bool> KeyStates = new Dictionary<Key, bool>();
        private uint SendKeyStateChange(Lidgren.Network.NetOutgoingMessage om, Key key, KeyboardState keyState)
        {
            var IsPressed = keyState.IsKeyDown(key);

            if(!KeyStates.ContainsKey(key))
            {
                KeyStates.Add(key, !IsPressed);
            }

            if(KeyStates[key] != IsPressed)
            {
                KeyStates[key] = IsPressed;
                om.Write((byte)key);
                om.Write(IsPressed);
                if (IsPressed)
                { }
                return 1;
            }
            return 0;
        }
        public override void Initialize()
        {
            Chat = new Drawing.UI.Chat(this, this.Canvas);
            base.Initialize();
        }
    }
}
