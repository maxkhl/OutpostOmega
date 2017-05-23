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
        public nClient Client;

        private Drawing.UI.Chat Chat;

        Stopwatch sendTimer;

        public NetworkGame(nClient Client, MainGame game, World world)
            : base(game, world)
        {
            this.Client = Client;

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
            
            if (1000 / SendUpdatesPerSecond <= sendTimer.ElapsedMilliseconds)
            {
                sendTimer.Reset();

                string text;
                if (Client.Output.Count > 0 && Client.Output.TryDequeue(out text))
                    Chat.Message(text);


                var mouseState = OpenTK.Input.Mouse.GetState();

                if (MouseMode || !Game.Focused)
                {
                }
                else
                {
                    if (OldMouseState.X != mouseState.X || OldMouseState.Y != mouseState.Y)
                    {
                        var outgoingMouseMessage = Client.GetOM(Command.Data, SecondCommand.InputMouseDelta);
                        outgoingMouseMessage.Write(Client.Clock);
                        //outgoingMouseMessage.Write(mouseState.LeftButton == ButtonState.Pressed);
                        //outgoingMouseMessage.Write(mouseState.MiddleButton == ButtonState.Pressed);
                        //outgoingMouseMessage.Write(mouseState.RightButton == ButtonState.Pressed);
                        outgoingMouseMessage.Write(mouseState.X - OldMouseState.X);
                        outgoingMouseMessage.Write(mouseState.Y - OldMouseState.Y);
                        Client.netClient.SendMessage(outgoingMouseMessage, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

                        OldMouseState = mouseState;
                    }
                    
                }
            }


            if (MouseMode || !Game.Focused)
            {
            }
            else
            {
                var kstate = OpenTK.Input.Keyboard.GetState();
                var outgoingMessage = Client.GetOM(Command.Data, SecondCommand.Input);
                outgoingMessage.Write(Client.Clock);
                uint ret = 0;
                ret += SendKeyStateChange(outgoingMessage, Key.W, kstate);
                ret += SendKeyStateChange(outgoingMessage, Key.A, kstate);
                ret += SendKeyStateChange(outgoingMessage, Key.S, kstate);
                ret += SendKeyStateChange(outgoingMessage, Key.D, kstate);
                ret += SendKeyStateChange(outgoingMessage, Key.ShiftLeft, kstate);
                ret += SendKeyStateChange(outgoingMessage, Key.Space, kstate);
                if (ret > 0)
                    Client.netClient.SendMessage(outgoingMessage, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
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
