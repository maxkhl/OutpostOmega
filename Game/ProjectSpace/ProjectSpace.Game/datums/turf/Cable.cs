using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.datums.turf
{
    /// <summary>
    /// Definition for turf-mounted cables
    /// </summary>
    public static class Cable
    {
        public enum cableType
        {
            LineNS = 1,
            LineWE = 2,
            CurveNE = 3,
            CurveES = 4,
            CurveSW = 5,
            CurveWN = 6
        }

        public static structures.uvCoord GetUV(cableType cableID)
        {
            if (!Enum.IsDefined(typeof(cableType), cableID))
                throw new Exception(String.Format("Unknown cable ID '{0}'", cableID));

            var uvCoord = new structures.uvCoord();

            switch((cableType)cableID)
            {
                case cableType.LineNS:
                    uvCoord.X = 0;
                    uvCoord.Y = 0;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case cableType.LineWE:
                    uvCoord.X = 0.5f;
                    uvCoord.Y = 0;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case cableType.CurveNE:
                    uvCoord.X = 0;
                    uvCoord.Y = (float)1 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case cableType.CurveES:
                    uvCoord.X = 0.5f;
                    uvCoord.Y = (float)1 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case cableType.CurveSW:
                    uvCoord.X = 0;
                    uvCoord.Y = (float)2 / 3;
                    uvCoord.Width = 0.5f;
                    uvCoord.Height = (float)1 / 3;
                    break;
                case cableType.CurveWN:
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
