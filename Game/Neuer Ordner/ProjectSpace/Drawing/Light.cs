using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing
{
    class Light
    {
        public Vector3 Position { get; set; }
        public float Brightness { get; set; }
        public float Range { get; set; }
        public Color Color { get; set; }

        public Light(Vector3 Position, Color Color = new Color(), float Brightness = 1, float Range = 5)
        {
            this.Position = Position;
            this.Brightness = Brightness;
            this.Range = Range;
            this.Color = Color;
        }
    }
}
