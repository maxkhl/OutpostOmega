using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.Tools
{
    static public class MathHelper
    {
        /// <summary>
        /// Calculates the hit-point of a raycast
        /// </summary>
        /// <param name="Origin">Where the ray came from</param>
        /// <param name="Forward">Direction of the ray</param>
        /// <param name="Fraction">Fraction (returned from rayhit)</param>
        /// <returns>Position, the ray hit something</returns>
        public static JVector GetRayHit(JVector Origin, JVector Forward, float Fraction)
        {
            return Origin + Fraction * Forward;
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static float Clamp(float val, float min, float max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
