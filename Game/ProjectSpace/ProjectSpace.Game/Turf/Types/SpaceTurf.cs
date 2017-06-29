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
    /// Space
    /// </summary>
    public class SpaceTurf : TurfType
    {
        /// <summary>
        /// Space is not visible
        /// </summary>
        public override bool IsVisible 
        { 
            get
            {
                return false;
            }
        }

        public override bool IsAirtight
        {
            get
            {
                return false;
            }
        }

        private Dictionary<Direction, uvCoord> _uvcoords = new Dictionary<Direction, uvCoord>();
        /// <summary>
        /// Not visible
        /// </summary>
        public override Dictionary<Direction, uvCoord> UVCoords
        {
            get
            {
                return _uvcoords;
            }
        }

        public override Dictionary<Direction, uvCoord> GetUVCoords(Block Block)
        {
            return new Dictionary<Direction, uvCoord>();
        }
    }
}
