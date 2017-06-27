using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Jitter.LinearMath;
using OutpostOmega.Game.Structures;

namespace OutpostOmega.Game.Turf.Types
{
    /// <summary>
    /// Room consumed by a GameObject
    /// </summary>
    public class GameObjectTurf : TurfType
    {
        /// <summary>
        /// GameObject (space) is invisible
        /// </summary>
        public bool IsVisible 
        { 
            get
            {
                return false; 
            }
        }

        public bool IsAirtight
        {
            get
            {
                return true;
            }
        }

        private Dictionary<Direction, uvCoord> _uvcoords = new Dictionary<Direction, uvCoord>();

        /// <summary>
        /// Not visible
        /// </summary>
        public Dictionary<Direction, uvCoord> UVCoords
        {
            get
            {
                return _uvcoords;
            }
        }
        public Dictionary<Direction, uvCoord> GetUVCoords(Block Block)
        {
            return new Dictionary<Direction, uvCoord>();
        }
    }
}
