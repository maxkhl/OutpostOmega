using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutpostOmega.Game;
using OutpostOmega.Network;
using OutpostOmega.Game.GameObjects.Mobs;
using OpenTK.Input;
using Lidgren.Network;
using System.Threading;
using System.Diagnostics;

namespace OutpostOmega.Scenes
{
    class NetworkGame : Game
    {
        public nClient Client;

        private Drawing.UI.Chat Chat;

        Stopwatch sendTimer;

        public NetworkGame(nClient Client, MainGame game, World world)
            : base(game, world)
        {
            this.Client = Client;

            sendTimer = new Stopwatch();
            world.ClientMode = true;

            //if(world.Player == null)
            //{
                world.Player = (from gobj in world.AllGameObjects
                                where gobj.GetType().IsAssignableFrom(typeof(Mind)) &&
                                      ((Mind)gobj).Username == Client.Username
                                select (Mind)gobj).FirstOrDefault();
                if (world.Player == null)
                    throw new Exception("No mind for the player found. Need to add joinmenu or spectator");
            //}         

                world.Player.Mob.PropertyChanged += Player_PropertyChanged;
        }

        private bool SendPosition = false;
        private bool SendOrientation = false;

        void Player_PropertyChanged(GameObject Object, string PropertyName, bool IndirectChange)
        {
            if (IndirectChange) return;

            switch(PropertyName)
            {
                case "Position":
                    SendPosition = true;
                    break;
                case "Orientation":
                    SendOrientation = true;
                    break;
            }
        }

        public override void Dispose()
        {
            this.Client.Disconnect();
            base.Dispose();
        }

        const double SendUpdatesPerSecond = 2;
        OpenTK.Input.MouseState OldMouseState = OpenTK.Input.Mouse.GetState();
        protected override void UpdateScene()
        {
            if (!sendTimer.IsRunning) sendTimer.Start();


                string text;
                if (Client.Output.Count > 0 && Client.Output.TryDequeue(out text))
                    Chat.Message(text);


                var kstate = OpenTK.Input.Keyboard.GetState();
                var mouseState = OpenTK.Input.Mouse.GetState();

                if (MouseMode || !Game.Focused)
                {
                    kstate = new KeyboardState();
                    mouseState = new MouseState(); // Do not send mouse input either to prevent accidential clicking ingame
                }
                else
                { }

                if (OldMouseState != mouseState)
                {
                    var outgoingMouseMessage = Client.GetOM(Command.Data, SecondCommand.InputMouseDelta);
                    outgoingMouseMessage.Write(Client.Clock);
                    outgoingMouseMessage.Write(mouseState.LeftButton == ButtonState.Pressed);
                    outgoingMouseMessage.Write(mouseState.MiddleButton == ButtonState.Pressed);
                    outgoingMouseMessage.Write(mouseState.RightButton == ButtonState.Pressed);
                    outgoingMouseMessage.Write(mouseState.X);
                    outgoingMouseMessage.Write(mouseState.Y);
                    Client.netClient.SendMessage(outgoingMouseMessage, Lidgren.Network.NetDeliveryMethod.Unreliable);
                }



                var outgoingMessage = Client.GetOM(Command.Data, SecondCommand.Input);
                outgoingMessage.Write(Client.Clock);
                uint ret = 0;
                //ret += SendKeyStateChange(outgoingMessage, Key.W, kstate);
                //ret += SendKeyStateChange(outgoingMessage, Key.A, kstate);
                //ret += SendKeyStateChange(outgoingMessage, Key.S, kstate);
                //ret += SendKeyStateChange(outgoingMessage, Key.D, kstate);
                //ret += SendKeyStateChange(outgoingMessage, Key.ShiftLeft, kstate);
                //ret += SendKeyStateChange(outgoingMessage, Key.Space, kstate);
                if (ret > 0)
                    Client.netClient.SendMessage(outgoingMessage, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);


            if (1000 / SendUpdatesPerSecond <= sendTimer.ElapsedMilliseconds)
            {
                sendTimer.Reset();
                if (SendPosition)
                {
                    var omP = Client.GetOM(Command.Data, SecondCommand.Position);
                    omP.Write(World.Player.Position.X);
                    omP.Write(World.Player.Position.Y);
                    omP.Write(World.Player.Position.Z);
                    Client.netClient.SendMessage(omP, Lidgren.Network.NetDeliveryMethod.Unreliable);
                    SendPosition = false;
                }

                if (SendOrientation)
                {
                    var omO = Client.GetOM(Command.Data, SecondCommand.Orientation);
                    omO.Write(World.Player.Orientation.M11);
                    omO.Write(World.Player.Orientation.M12);
                    omO.Write(World.Player.Orientation.M13);

                    omO.Write(World.Player.Orientation.M21);
                    omO.Write(World.Player.Orientation.M22);
                    omO.Write(World.Player.Orientation.M23);

                    omO.Write(World.Player.Orientation.M31);
                    omO.Write(World.Player.Orientation.M32);
                    omO.Write(World.Player.Orientation.M33);
                    Client.netClient.SendMessage(omO, Lidgren.Network.NetDeliveryMethod.Unreliable);
                    SendOrientation = false;
                }
            }

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
