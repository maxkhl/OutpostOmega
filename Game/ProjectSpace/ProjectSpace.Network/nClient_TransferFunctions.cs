using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Network
{
    /// <summary>
    /// Clients transferfunctions to send specific data to the server
    /// </summary>
    public partial class nClient
    {
        /// <summary>
        /// Contains inputpackages that are about to be sent to the server
        /// Use InputPackageQueueFlush() to send them
        /// </summary>
        public List<InputPackage> InputPackageQueue = new List<InputPackage>();

        /// <summary>
        /// Sends all inputpackages waiting in the queue
        /// </summary>
        public void InputPackageQueueFlush()
        {
            Lidgren.Network.NetOutgoingMessage om;
            lock (InputPackageQueue)
            {
                om = InputPackage.CreateOutgoingMessage(this, InputPackageQueue.ToArray());
                InputPackageQueue.Clear();
            }
            this.netClient.SendMessage(om, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Adds mouse coordinates to the packet queue (that sends it to the server when flushed)
        /// </summary>
        /// <param name="X">X-delta coordinate</param>
        /// <param name="Y">Y-delta coordinate</param>
        public void AddInput(int X, int Y)
        {
            InputPackageQueue.Add(
                new InputPackage(
                    this.Clock,
                    X,
                    Y));
        }

        /// <summary>
        /// Adds a action to the packet queue (that sends it to the server when flushed)
        /// </summary>
        /// <param name="Action">Action</param>
        /// <param name="ActionState">State of the action</param>
        public void AddInput(Game.Tools.Action Action, Game.Tools.ActionState ActionState)
        {
            InputPackageQueue.Add(
                new InputPackage(
                    this.Clock,
                    Action,
                    ActionState));
        }

        public void SendMouseState(OutpostOmega.Game.Tools.MouseState MouseState, OutpostOmega.Game.Tools.MouseState OldMouseState)
        {
            var om = GetOM(Command.Data, SecondCommand.InputMouseDelta);
            
            om.Write(Clock);
            om.Write(MouseState.LeftKey);
            om.Write(MouseState.MiddleKey);
            om.Write(MouseState.RightKey);
            om.Write(MouseState.X - OldMouseState.X);
            om.Write(MouseState.Y - OldMouseState.Y);
            this.netClient.SendMessage(om, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);

        }
    }
}
