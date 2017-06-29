using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Turf.Properties
{
    /// <summary>
    /// Definition for turf-mounted pipes
    /// </summary>
    public struct Pipe
    {
        /// <summary>
        /// Type of this cable
        /// </summary>
        public PipeType Type { get; private set; }

        /// <summary>
        /// Color of this Pipe
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
        /// Direction, this Pipe is placed onto its block
        /// </summary>
        public Structures.Direction Direction { get; set; }

        /// <summary>
        /// Initializes a new Pipe object
        /// </summary>
        /// <param name="Type">Type of the Pipe</param>
        /// <param name="Color">Color of the Pipe</param>
        public Pipe(PipeType Type, Structures.Direction Direction, System.Drawing.Color Color)
        {
            this.Type = Type;
            this.Direction = Direction;
            _Color = new byte[4] { 255, 255, 255, 255 };
            this.Color = Color;
        }


        /// <summary>
        /// Type of Pipe
        /// </summary>
        public enum PipeType
        {
            LineNS = 1,
            LineWE = 2,
            CurveNE = 3,
            CurveES = 4,
            CurveSW = 5,
            CurveWN = 6
        }

        /// <summary>
        /// Returns uv-coordinates of this pipe
        /// </summary>
        /// <returns>UV-Coordinates</returns>
        public Structures.uvCoord GetUV()
        {
            var uvCoord = new Structures.uvCoord();

            switch(Type)
            {
                case PipeType.LineNS:
                    uvCoord.X = 0;
                    uvCoord.Y = 0;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case PipeType.LineWE:
                    uvCoord.X = 0.5f;
                    uvCoord.Y = 0;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case PipeType.CurveNE:
                    uvCoord.X = 0;
                    uvCoord.Y = (float)1 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case PipeType.CurveES:
                    uvCoord.X = 0.5f;
                    uvCoord.Y = (float)1 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case PipeType.CurveSW:
                    uvCoord.X = 0;
                    uvCoord.Y = (float)2 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case PipeType.CurveWN:
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
