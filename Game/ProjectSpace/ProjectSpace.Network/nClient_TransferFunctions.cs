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
