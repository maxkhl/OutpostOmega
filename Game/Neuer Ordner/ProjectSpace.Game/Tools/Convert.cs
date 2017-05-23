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
                return new Matrix4(
                    new Vector4(matrix.M11, matrix.M21, matrix.M31, 0),
                    new Vector4(matrix.M12, matrix.M22, matrix.M32, 0),
                    new Vector4(matrix.M13, matrix.M23, matrix.M33, 0),
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
    }
}
