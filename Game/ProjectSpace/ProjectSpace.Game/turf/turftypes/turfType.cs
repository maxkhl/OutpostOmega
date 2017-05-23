using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OutpostOmega.Game.GameObjects;
using OutpostOmega.Game.structures;

namespace OutpostOmega.Game.turf.types
{
    public interface turfType
    {
        bool IsVisible { get; }
        bool IsAirtight { get; }
        Dictionary<Direction, uvCoord> GetUVCoords(Block Block);
        Dictionary<Direction, uvCoord> UVCoords { get; }
    }
    public enum turfTypeE
    {
        space = 0,
        floor = 1,
        //gobject = 2,
    }
}
