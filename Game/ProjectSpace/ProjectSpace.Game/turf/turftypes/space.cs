﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Jitter.LinearMath;
using OutpostOmega.Game.structures;

namespace OutpostOmega.Game.turf.types
{
    /// <summary>
    /// Space
    /// </summary>
    public class space : turfType
    {
        /// <summary>
        /// Space is not visible
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
                return false;
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
