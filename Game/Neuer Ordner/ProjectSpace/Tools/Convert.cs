using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Jitter.LinearMath;

namespace OutpostOmega.Tools
{
    static public class Convert
    {
        public static class Vector
        {
            public static Vector3 Jitter_To_OpenGL(JVector Vector)
            {
                return new Vector3(Vector.X, Vector.Y, Vector.Z);
            }
            public static JVector OpenGL_To_Jitter(Vector3 Vector)
            {
                return new JVector(Vector.X, Vector.Y, Vector.Z);
            }
        }
        public static class Vector2D
        {
            public static Vector2 Jitter_To_OpenGL(JVector2 Vector)
            {
                return new Vector2(Vector.X, Vector.Y);
            }
            public static JVector2 OpenGL_To_Jitter(Vector2 Vector)
            {
                return new JVector2(Vector.X, Vector.Y);
            }
        }
        public static class Matrix
        {
            public static Matrix3 Jitter_To_OpenGL_3(JMatrix matrix)
            {
                return new Matrix3(
                    matrix.M11, matrix.M12, matrix.M13,
                    matrix.M21, matrix.M22, matrix.M23,
                    matrix.M31, matrix.M32, matrix.M33);
            }
            public static Matrix4 Jitter_To_OpenGL_4(JMatrix matrix)
            {
                /*return new Matrix4(
                    new Vector4(matrix.M11, matrix.M21, matrix.M31, 0),
                    new Vector4(matrix.M12, matrix.M22, matrix.M32, 0),
                    new Vector4(matrix.M13, matrix.M23, matrix.M33, 0),
                    new Vector4(0, 0, 0, 1));*/
                return new Matrix4(
                    new Vector4(matrix.M11, matrix.M12, matrix.M13, 0),
                    new Vector4(matrix.M21, matrix.M22, matrix.M23, 0),
                    new Vector4(matrix.M31, matrix.M32, matrix.M33, 0),
                    new Vector4(0, 0, 0, 1));
            }
            public static JMatrix OpenGL_To_Jitter_4(Matrix4 matrix)
            {
                return new JMatrix(
                    matrix.M11, matrix.M12, matrix.M13,
                    matrix.M21, matrix.M22, matrix.M23,
                    matrix.M31, matrix.M32, matrix.M33);
            }
        }
        public static class Mesh
        {
            public static class Vertex
            {
                /// <summary>
                /// Used to translate the vertices of a chunk from the jitter (physics) world to the opengl (graphics) world
                /// </summary>
                public static Drawing.Vertex[] Jitter_To_OpenGL(OutpostOmega.Game.turf.Chunk.Mesh mesh)
                {

                    var JitterMesh = mesh;

                    var Vertices = new Drawing.Vertex[JitterMesh.Vertices.Count()];

                    /// Translate the Vertex-Data
                    for (int i = 0; i < JitterMesh.Vertices.Count(); i++)
                    {
                        Vertices[i] = new Drawing.Vertex()
                        {
                            Position = new Vector3(JitterMesh.Vertices[i].X, JitterMesh.Vertices[i].Y, JitterMesh.Vertices[i].Z),
                            TexCoord1 = new Vector2(JitterMesh.UV1[i].X, JitterMesh.UV1[i].Y),
                            TexCoord2 = new Vector2(JitterMesh.UV2[i].X, JitterMesh.UV2[i].Y),
                            TexCoord3 = new Vector2(JitterMesh.UV3[i].X, JitterMesh.UV3[i].Y),
                            TexCoord4 = new Vector2(JitterMesh.UV4[i].X, JitterMesh.UV4[i].Y),
                            Normal = new Vector3(JitterMesh.Normals[i].X, JitterMesh.Normals[i].Y, JitterMesh.Normals[i].Z)
                            //Tangent = new Vector3(Tools.Convert.Vector.Jitter_To_OpenGL(JitterMesh.Tangents[(int)Math.Floor((double)i / 2)])),
                            //BiTangent = new Vector3(Tools.Convert.Vector.Jitter_To_OpenGL(JitterMesh.BiTangents[(int)Math.Floor((double)i / 2)]))
                        };
                    }

                    return Vertices;
                }
            }
            public static class Index
            {
                /// <summary>
                /// Used to translate the indices of a chunk from the jitter (physics) world to the opengl (graphics) world
                /// </summary>
                public static uint[] Jitter_To_OpenGL(OutpostOmega.Game.turf.Chunk.Mesh mesh)
                {
                    var JitterMesh = mesh;

                    var Indices = new uint[JitterMesh.Triangles.Count()];
                    for (int i = 0; i < Indices.Count(); i++)
                        Indices[i] = (uint)JitterMesh.Triangles[i];

                    return Indices;
                }
            }
        }
    }
}
