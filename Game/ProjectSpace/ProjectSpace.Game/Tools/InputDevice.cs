using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Tools
{
    /// <summary>
    /// Used to differentiate input devices. Pretty simple, eh? I wont document this any further because it makes absolutely no sense to spend that much time on documenting something as simple as that. I am just trying to waste time to not having to deal with this input shit any further because I am pretty sick of it right now. Shit I ran out of bullshit I guess I need to go back to work now. Meh...
    /// </summary>
    public enum InputDevice : byte
    {
        Keyboard = 1,
        Mouse = 2,
        Undefined = 9
    }
}
