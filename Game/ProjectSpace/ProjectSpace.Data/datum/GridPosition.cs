using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutpostOmega.Data.datum
{
    /// <summary>
    /// Position relative to main grid
    /// </summary>
    [Serializable]
    public class GridPosition
    {
        public Int16 X { get; set; }
        public Int16 Y { get; set; }
        public Int16 Z { get; set; }

        public GridPosition(Int16 X, Int16 Y, Int16 Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
}
