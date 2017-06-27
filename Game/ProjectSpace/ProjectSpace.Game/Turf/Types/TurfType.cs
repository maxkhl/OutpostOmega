using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OutpostOmega.Game.GameObjects;
using OutpostOmega.Game.Structures;

namespace OutpostOmega.Game.Turf.Types
{
    public interface TurfType
    {
        bool IsVisible { get; }
        bool IsAirtight { get; }
        Dictionary<Direction, uvCoord> GetUVCoords(Block Block);
        Dictionary<Direction, uvCoord> UVCoords { get; }
    }
    public enum TurfTypeE
    {
        space = 0,
        floor = 1,
        //gobject = 2,
    }
}
