using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Network
{
    /// <summary>
    /// Defines a input package that is about to be transfered to the server
    /// </summary>
    public struct InputPackage
    {
        public double Time;
        public InputType Type;
        public object Data;
        public bool Written;

        /// <summary>
        /// Creates a new input package containing mouse data
        /// </summary>
        /// <param name="Time">Time, this package was captured</param>
        /// <param name="X">X coordinate</param>
        /// <param name="Y">Y coordinate</param>
        public InputPackage(double Time, int X, int Y)
        {
            this.Written = false;
            this.Time = Time;
            this.Type = InputType.Mouse;
            this.Data = Tuple.Create(X, Y);
        }

        /// <summary>
        /// Creates a new input package containing a action
        /// </summary>
        /// <param name="Time">Time, this package was captured</param>
        /// <param name="Action">Type of action</param>
        /// <param name="State">The actions state</param>
        public InputPackage(double Time, Game.Tools.Action Action, Game.Tools.ActionState State)
        {
            this.Written = false;
            this.Time = Time;
            this.Type = InputType.Action;
            this.Data = Tuple.Create(Action, State);
        }

        /// <summary>
        /// Creates a outgoing message out of this inputpackage ready to be sent to the server
        /// </summary>
        /// <param name="Client">Client this message should be generated for</param>
        /// <returns>Generated and ready to send message</returns>
        public Lidgren.Network.NetOutgoingMessage CreateOutgoingMessage(nClient Client)
        {
            var om = Client.GetOM(Command.Data, SecondCommand.Input);

            WriteOutgoingMessage(ref om);

            return om;
        }

        /// <summary>
        /// Creates a outgoing message out the given inputpackages
        /// </summary>
        /// <param name="Client">Client the messages should be generated for</param>
        /// <param name="Packages">Packages that are used</param>
        /// <returns>Generated and ready to send message</returns>
        public static Lidgren.Network.NetOutgoingMessage CreateOutgoingMessage(nClient Client, InputPackage[] Packages)
        {
            var om = Client.GetOM(Command.Data, SecondCommand.Input);

            for (int i = 0; i < Packages.Length; i++)
            {
                Packages[i].WriteOutgoingMessage(ref om);

                //if(i < Packages.Length)
                //    om.Write((byte)InputType.Separator);
            }

            return om;
        }

        /// <summary>
        /// Reads input packages out of a incoming message
        /// </summary>
        /// <param name="incomingMessage">Incoming message that should be read</param>
        /// <returns>Created input packages in an array</returns>
        public static InputPackage[] ReadIncommingMessage(Lidgren.Network.NetIncomingMessage incomingMessage)
        {
            List<InputPackage> packages = new List<InputPackage>();
            while(incomingMessage.Position < incomingMessage.LengthBits)
            {
                var inputType = (InputType)incomingMessage.ReadByte();
                var time = incomingMessage.ReadDouble();
                InputPackage newPackage;

                switch(inputType)
                {
                    case InputType.Mouse:
                        var X = incomingMessage.ReadInt32();
                        var Y = incomingMessage.ReadInt32();
                        newPackage = new InputPackage(time, X, Y);
                        break;
                    case InputType.Action:
                        var action = (Game.Tools.Action)incomingMessage.ReadByte();
                        var actionState = (Game.Tools.ActionState)incomingMessage.ReadByte();
                        newPackage = new InputPackage(time, action, actionState);
                        break;
                    default:
                        throw new Exception("Unreadable inputpackage. Something is very wrong here");
                }

                packages.Add(newPackage);
            }

            return packages.ToArray();
        }

        /// <summary>
        /// Fills a outgoing message with this packages data
        /// </summary>
        /// <param name="outgoingMessage"></param>
        public void WriteOutgoingMessage(ref Lidgren.Network.NetOutgoingMessage outgoingMessage)
        {
            outgoingMessage.Write((byte)Type);
            outgoingMessage.Write(Time);

            switch (this.Type)
            {
                case InputType.Mouse:

                    var tupleMouse = (Tuple<int, int>)this.Data;
                    outgoingMessage.Write(tupleMouse.Item1); //X
                    outgoingMessage.Write(tupleMouse.Item2); //Y
                    break;

                case InputType.Action:

                    var tupleAction = (Tuple<Game.Tools.Action, Game.Tools.ActionState>)this.Data;
                    outgoingMessage.Write((byte)tupleAction.Item1);
                    outgoingMessage.Write((byte)tupleAction.Item2);
                    break;
            }
            this.Written = true;
        }

        public static bool operator ==(InputPackage ip1, InputPackage ip2)
        {
            return ip1.Type == ip2.Type &&
                   ip1.Data == ip2.Data &&
                   ip1.Time == ip2.Time;
        }

        public static bool operator !=(InputPackage ip1, InputPackage ip2)
        {
            return ip1.Type != ip2.Type ||
                   ip1.Data != ip2.Data ||
                   ip1.Time != ip2.Time;
        }
    }
}
