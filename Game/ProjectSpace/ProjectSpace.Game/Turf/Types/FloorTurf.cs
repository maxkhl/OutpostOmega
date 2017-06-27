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
    /// Basic floor
    /// </summary>
    public class FloorTurf : TurfType
    {
        /// <summary>
        /// Floor is solid (means, objects can collide with it)
        /// </summary>
        public bool IsVisible 
        { 
            get
            {
                return true;
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

        //new float[] { 0.5f, 0.0f, 0.5f, 0.5f }
        /// <summary>
        /// UV Coords of the texture
        /// </summary>
        public Dictionary<Direction, uvCoord> UVCoords
        {
            get
            {
                return _uvcoords;
            }
        }

        public FloorTurf()
        {
            var UVC = new uvCoord() { X = 0.5f, Y = 0.0f, Width = 0.5f, Height = 0.5f };

            _uvcoords.Add(Direction.Front, UVC);
            _uvcoords.Add(Direction.Back, UVC);
            _uvcoords.Add(Direction.Left, UVC);
            _uvcoords.Add(Direction.Right, UVC);

            UVC = new uvCoord() { X = 0.0f, Y = 0.0f, Width = 0.5f, Height = 0.5f };
            _uvcoords.Add(Direction.Top, UVC);

            UVC = new uvCoord() { X = 0.0f, Y = 0.0f, Width = 0.5f, Height = 0.5f };
            _uvcoords.Add(Direction.Bottom, UVC);
        }

        public Dictionary<Direction, uvCoord> GetUVCoords(Block Block)
        {


            var uvCoords = new Dictionary<Direction, uvCoord>();

            uvCoords.Add(Direction.Front, GetCoord(Block.UVFront > 0 ? Block.UVFront : (short)1 ));
            uvCoords.Add(Direction.Back, GetCoord(Block.UVBack > 0 ? Block.UVBack : (short)1 ));
            uvCoords.Add(Direction.Left, GetCoord(Block.UVLeft > 0 ? Block.UVLeft : (short)1 ));
            uvCoords.Add(Direction.Right, GetCoord(Block.UVRight > 0 ? Block.UVRight : (short)1 ));
            uvCoords.Add(Direction.Top, GetCoord(Block.UVTop > 0 ? Block.UVTop : (short)0 ));
            uvCoords.Add(Direction.Bottom, GetCoord(Block.UVBottom > 0 ? Block.UVBottom : (short)3 ));
            return uvCoords;
            /*var UVC = new uvCoord() { X = 0.5f, Y = 0.0f, Width = 0.5f, Height = 0.5f };

            _uvcoords.Add(Direction.Front, UVC);
            _uvcoords.Add(Direction.Back, UVC);
            _uvcoords.Add(Direction.Left, UVC);
            _uvcoords.Add(Direction.Right, UVC);

            UVC = new uvCoord() { X = 0.0f, Y = 0.0f, Width = 0.5f, Height = 0.5f };
            _uvcoords.Add(Direction.Top, UVC);

            UVC = new uvCoord() { X = 0.0f, Y = 0.0f, Width = 0.5f, Height = 0.5f };
            _uvcoords.Add(Direction.Bottom, UVC);

            return null;*/
        }

        public const int TurfTextureSize = 2;
        private uvCoord GetCoord(Int16 ID)
        {            
            var Yam = Math.Floor((double)ID / TurfTextureSize);
            var X = ((double)ID / TurfTextureSize - Yam);
            var Y = Yam * (1 / (double)TurfTextureSize);

            return new uvCoord() { X = (float)X, Y = (float)Y, Width = (1 / (float)TurfTextureSize), Height = (1 / (float)TurfTextureSize) };
        }
    }
}
