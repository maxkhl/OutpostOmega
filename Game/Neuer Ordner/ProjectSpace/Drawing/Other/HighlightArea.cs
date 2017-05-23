using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing.Other
{
    class HighlightArea
    {
        public Vector3 Start { get; private set; }
        public Vector3 End { get; private set; }

        private RenderOptions rOptions;
        public bool Visible = false;

        public OpenTK.Graphics.Color4 Color = OpenTK.Graphics.Color4.Yellow;

        private Matrix4 Translation = Matrix4.Identity;
        
        public HighlightArea()
        {
            rOptions = new RenderOptions()
            {
                Shader = Shader.Load(
                        new System.IO.FileInfo(@"Content\Shader\Highlight\Highlight_VS.glsl"),
                        new System.IO.FileInfo(@"Content\Shader\Highlight\Highlight_FS.glsl")
                    ),
                Color = OpenTK.Graphics.Color4.Yellow
            };
            rOptions.SetUniform = new Action<Shader>(SetShaderParams);
        }

        private void SetShaderParams(Shader shader)
        {
            GL.Uniform1(shader.GetUniformLocation("Transparency"), Transparency);
            GL.Uniform4(shader.GetUniformLocation("Color"), Color);
        }

        public float Transparency = 0.24f;
        public bool PulseRising = false;
        public void Update(double ElapsedTime)
        {
            if(Visible)
            {
                if (PulseRising)
                {
                    Transparency += (float)(ElapsedTime / 2);
                    if (Transparency >= 0.24f)
                        PulseRising = false;
                }
                else
                {
                    Transparency -= (float)(ElapsedTime / 2);
                    if (Transparency <= 0.10f)
                        PulseRising = true;
                }
            }
        }

        public void Draw()
        {
            if (!Visible) return;

            // Transparency
            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);

            GL.LoadMatrix(ref Translation);
            Mesh.DrawImmediate(rOptions, GetAreaVertices(Start, End));

            var ident = Matrix4.Identity;
            GL.LoadMatrix(ref ident);

            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
        }

        public void SetBounds(Vector3 Start, Vector3 End)
        {
            if (Start == this.Start && End == this.End) return;

            this.Visible = true;

            this.Start = Start;
            this.End = End;
            this.Translation = Matrix4.CreateTranslation(Start);
        }

        public static Vertex[] GetAreaVertices(Vector3 Start, Vector3 End)
        {
            var vStart = Vector3.Zero;
            var vEnd = End - Start;

            var vertices = new Vertex[24];

            vertices[0] = new Vertex() { Position = vStart };
            vertices[1] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y, vStart.Z) };
            vertices[2] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y, vStart.Z + vEnd.Z) };
            vertices[3] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y, vStart.Z + vEnd.Z) };


            vertices[4] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y + vEnd.Y, vStart.Z) };
            vertices[5] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y + vEnd.Y, vStart.Z) };
            vertices[6] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y, vStart.Z) };
            vertices[7] = new Vertex() { Position = vStart };


            vertices[8] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y + vEnd.Y, vStart.Z) };
            vertices[9] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y + vEnd.Y, vStart.Z + vEnd.Z) };
            vertices[10] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y, vStart.Z + vEnd.Z) };
            vertices[11] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y, vStart.Z) };


            vertices[12] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y + vEnd.Y, vStart.Z + vEnd.Z) };
            vertices[13] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y + vEnd.Y, vStart.Z + vEnd.Z) };
            vertices[14] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y, vStart.Z + vEnd.Z) };
            vertices[15] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y, vStart.Z + vEnd.Z) };


            vertices[16] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y + vEnd.Y, vStart.Z + vEnd.Z) };
            vertices[17] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y + vEnd.Y, vStart.Z) };
            vertices[18] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y, vStart.Z) };
            vertices[19] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y, vStart.Z + vEnd.Z) };


            vertices[20] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y + vEnd.Y, vStart.Z + vEnd.Z) };
            vertices[21] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y + vEnd.Y, vStart.Z + vEnd.Z) };
            vertices[22] = new Vertex() { Position = new Vector3(vStart.X + vEnd.X, vStart.Y + vEnd.Y, vStart.Z) };
            vertices[23] = new Vertex() { Position = new Vector3(vStart.X, vStart.Y + vEnd.Y, vStart.Z) };



            return vertices;
        }
        public static uint[] GetAreaIndices()
        {
            var indices = new uint[36];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;

            indices[3] = 1;
            indices[4] = 2;
            indices[5] = 3;


            indices[6] = 4;
            indices[7] = 5;
            indices[8] = 6;

            indices[9] = 5;
            indices[10] = 6;
            indices[11] = 7;


            indices[12] = 8;
            indices[13] = 9;
            indices[14] = 10;

            indices[15] = 9;
            indices[16] = 10;
            indices[17] = 11;


            indices[18] = 12;
            indices[19] = 13;
            indices[20] = 14;

            indices[21] = 13;
            indices[22] = 14;
            indices[23] = 15;


            indices[24] = 16;
            indices[25] = 17;
            indices[26] = 18;

            indices[27] = 17;
            indices[28] = 18;
            indices[29] = 19;


            indices[30] = 20;
            indices[31] = 21;
            indices[32] = 22;

            indices[33] = 21;
            indices[34] = 22;
            indices[35] = 23;

            return indices;
        }
    }
}
