using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// Used to store mesh-data of one object
    /// </summary>
    public struct Vertex
    {
        public Vector3 Position, 
                       Normal;
        public Vector2 TexCoord1;
        //public Vector2 TecCoord1;
        public Vector2 TexCoord2;
        public Vector2 TexCoord3;
        public Vector2 TexCoord4;
        //public Color4 Color;
        //public Vector3 Tangent, BiTangent;
        //public byte Texture;

        public static readonly int Stride = Marshal.SizeOf(default(Vertex));
        public static readonly int ColorSize = Marshal.SizeOf(default(Color4));
    }
}
