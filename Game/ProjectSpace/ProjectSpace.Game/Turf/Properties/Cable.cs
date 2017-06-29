using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Turf.Properties
{
    /// <summary>
    /// Definition for turf-mounted cables
    /// </summary>
    public struct Cable
    {
        /// <summary>
        /// Type of this cable
        /// </summary>
        public CableType Type { get; private set; }

        /// <summary>
        /// Color of this cable
        /// </summary>
        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb(
                    _Color[0],
                    _Color[1],
                    _Color[2],
                    _Color[3]);
            }
            set
            {
                _Color = new byte[4] { value.A, value.R, value.G, value.B };
            }
        }
        private byte[] _Color;

        /// <summary>
        /// Direction, this cable is placed onto its block
        /// </summary>
        public Structures.Direction Direction { get; set; }

        /// <summary>
        /// Initializes a new cable object
        /// </summary>
        /// <param name="Type">Type of the cable</param>
        /// <param name="Color">Color of the cable</param>
        public Cable(CableType Type, Structures.Direction Direction, System.Drawing.Color Color)
        {
            this.Type = Type;
            this.Direction = Direction;
            _Color = new byte[4] { 255, 255, 255, 255 };
            this.Color = Color;
        }


        /// <summary>
        /// Type of cable
        /// </summary>
        public enum CableType
        {
            LineNS = 1,
            LineWE = 2,
            CurveNE = 3,
            CurveES = 4,
            CurveSW = 5,
            CurveWN = 6
        }

        /// <summary>
        /// Returns uv-coordinates for the given cable type
        /// </summary>
        /// <returns>UV-Coordinates</returns>
        public Structures.uvCoord GetUV()
        {
            var uvCoord = new Structures.uvCoord();

            switch(Type)
            {
                case CableType.LineNS:
                    uvCoord.X = 0;
                    uvCoord.Y = 0;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case CableType.LineWE:
                    uvCoord.X = 0.5f;
                    uvCoord.Y = 0;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case CableType.CurveNE:
                    uvCoord.X = 0;
                    uvCoord.Y = (float)1 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case CableType.CurveES:
                    uvCoord.X = 0.5f;
                    uvCoord.Y = (float)1 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case CableType.CurveSW:
                    uvCoord.X = 0;
                    uvCoord.Y = (float)2 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case CableType.CurveWN:
                    uvCoord.X = 0.5f;
                    uvCoord.Y = (float)2 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
            }
            return uvCoord;
        }

    }
}
