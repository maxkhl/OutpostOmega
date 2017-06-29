using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OutpostOmega.Game.GameObjects;
using OutpostOmega.Game.Structures;

namespace OutpostOmega.Game.Turf.Types
{
    /// <summary>
    /// Abstract turf type to describe properties of turfs
    /// </summary>
    public abstract class TurfType
    {
        /// <summary>
        /// Describes if the block is visible
        /// </summary>
        public abstract bool IsVisible { get; }

        /// <summary>
        /// Describes if air can pass through it
        /// </summary>
        public abstract bool IsAirtight { get; }

        /// <summary>
        /// Returns uv-coords of this block
        /// </summary>
        /// <param name="Block"></param>
        /// <returns></returns>
        public abstract Dictionary<Direction, uvCoord> GetUVCoords(Block Block);
        public abstract Dictionary<Direction, uvCoord> UVCoords { get; }

        /// <summary>
        /// Every available turf type. Do not change the order!
        /// </summary>
        public static TurfType[] Types = new TurfType[]
        {
            new Types.SpaceTurf(),
            new Types.FloorTurf(),
        };
    }
    public enum TurfTypeE
    {
        space = 0,
        floor = 1,
        //gobject = 2,
    }
}
