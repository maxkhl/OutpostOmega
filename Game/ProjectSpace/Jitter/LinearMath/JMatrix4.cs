#region Using Statements
using System;
using System.Collections.Generic;

using Jitter.Dynamics;
using Jitter.LinearMath;
using Jitter.Collision.Shapes;
#endregion

namespace Jitter.LinearMath
{

    /// <summary>
    /// 4x4 Matrix. Member of the math namespace, so every method
    /// has it's 'by reference' equivalent to speed up time critical
    /// math operations.
    /// </summary>
    public struct JMatrix4
    {
        public float M00;
        public float M01;
        public float M02;
        public float M03;

        public float M10;
        public float M11;
        public float M12;
        public float M13;

        public float M20;
        public float M21;
        public float M22;
        public float M23;

        public float M30;
        public float M31;
        public float M32;
        public float M33;

        internal static JMatrix4 InternalIdentity;

        /// <summary>
        /// Identity matrix.
        /// </summary>
        public static readonly JMatrix4 Identity;
        public static readonly JMatrix4 Zero;

        static JMatrix4()
        {
            Zero = new JMatrix4();

            Identity = new JMatrix4();
            Identity.M00 = 1.0f;
            Identity.M11 = 1.0f;
            Identity.M22 = 1.0f;

            InternalIdentity = Identity;
        }

        /*public static JMatrix4 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            JMatrix4 matrix;
            JQuaternion quaternion;
            JQuaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
            CreateFromQuaternion(ref quaternion, out matrix);
            return matrix;
        }*/

        public static JMatrix4 CreateScale(JVector Factor)
        {
            JMatrix4 matrix;
            matrix.M00 = Factor.X;
            matrix.M01 = 0f;
            matrix.M02 = 0f;
            matrix.M03 = 0f;
            matrix.M10 = 0f;
            matrix.M11 = Factor.Y;
            matrix.M12 = 0f;
            matrix.M13 = 0f;
            matrix.M20 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = Factor.Z;
            matrix.M23 = 0f;
            matrix.M30 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 0f;
            return matrix;
        }

        public static JMatrix4 CreateRotationX(float radians)
        {
            JMatrix4 matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M00 = 1f;
            matrix.M01 = 0f;
            matrix.M02 = 0f;
            matrix.M03 = 0f;
            matrix.M10 = 0f;
            matrix.M11 = num2;
            matrix.M12 = num;
            matrix.M13 = 0f;
            matrix.M20 = 0f;
            matrix.M21 = -num;
            matrix.M22 = num2;
            matrix.M23 = 0f;
            matrix.M30 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 0f;
            return matrix;
        }

        public static void CreateRotationX(float radians, out JMatrix4 result)
        {
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            result.M00 = 1f;
            result.M01 = 0f;
            result.M02 = 0f;
            result.M03 = 0f;
            result.M10 = 0f;
            result.M11 = num2;
            result.M12 = num;
            result.M13 = 0f;
            result.M20 = 0f;
            result.M21 = -num;
            result.M22 = num2;
            result.M23 = 0f;
            result.M30 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = 0f;
        }

        public static JMatrix4 CreateRotationY(float radians)
        {
            JMatrix4 matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M00 = num2;
            matrix.M01 = 0f;
            matrix.M02 = -num;
            matrix.M03 = 0f;

            matrix.M10 = 0f;
            matrix.M11 = 1f;
            matrix.M12 = 0f;
            matrix.M13 = 0f;

            matrix.M20 = num;
            matrix.M21 = 0f;
            matrix.M22 = num2;
            matrix.M23 = 0f;

            matrix.M30 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 0f;
            return matrix;
        }

        public static void CreateRotationY(float radians, out JMatrix4 result)
        {
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            result.M00 = num2;
            result.M01 = 0f;
            result.M02 = -num;
            result.M03 = 0f;

            result.M10 = 0f;
            result.M11 = 1f;
            result.M12 = 0f;
            result.M13 = 0f;

            result.M20 = num;
            result.M21 = 0f;
            result.M22 = num2;
            result.M23 = 0f;

            result.M30 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = 0f;
        }

        public static JMatrix4 CreateRotationZ(float radians)
        {
            JMatrix4 matrix;
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            matrix.M00 = num2;
            matrix.M01 = num;
            matrix.M02 = 0f;
            matrix.M03 = 0f;

            matrix.M10 = -num;
            matrix.M11 = num2;
            matrix.M12 = 0f;
            matrix.M13 = 0f;

            matrix.M20 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = 1f;
            matrix.M23 = 0f;

            matrix.M30 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 0f;

            return matrix;
        }


        public static void CreateRotationZ(float radians, out JMatrix4 result)
        {
            float num2 = (float)Math.Cos((double)radians);
            float num = (float)Math.Sin((double)radians);
            result.M00 = num2;
            result.M01 = num;
            result.M02 = 0f;
            result.M03 = 0f;

            result.M10 = -num;
            result.M11 = num2;
            result.M12 = 0f;
            result.M13 = 0f;

            result.M20 = 0f;
            result.M21 = 0f;
            result.M22 = 1f;
            result.M23 = 0f;

            result.M30 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = 0f;
        }

        public static bool Compare(JMatrix4 matrix1, JMatrix4 matrix2)
        {
            return  matrix1.M00 == matrix2.M00 &&
                    matrix1.M01 == matrix2.M01 &&
                    matrix1.M02 == matrix2.M02 &&
                    matrix1.M10 == matrix2.M10 &&
                    matrix1.M11 == matrix2.M11 &&
                    matrix1.M12 == matrix2.M12 &&
                    matrix1.M20 == matrix2.M20 &&
                    matrix1.M21 == matrix2.M21 &&
                    matrix1.M22 == matrix2.M22;
        }


        /// <summary>
        /// Initializes a new instance of the matrix structure.
        /// </summary>
        public JMatrix4(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            this.M00 = m11;
            this.M01 = m12;
            this.M02 = m13;
            this.M03 = m14;

            this.M10 = m21;
            this.M11 = m22;
            this.M12 = m23;
            this.M13 = m24;

            this.M20 = m31;
            this.M21 = m32;
            this.M22 = m33;
            this.M23 = m34;

            this.M30 = m41;
            this.M31 = m42;
            this.M32 = m43;
            this.M33 = m44;
        }

        /// <summary>
        /// Multiply two matrices. Notice: matrix multiplication is not commutative.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>The product of both matrices.</returns>
        public static JMatrix4 Multiply(JMatrix4 matrix1, JMatrix4 matrix2)
        {
            JMatrix4 result;
            JMatrix4.Multiply(ref matrix1, ref matrix2, out result);
            return result;
        }

        /// <summary>
        /// Multiply two matrices. Notice: matrix multiplication is not commutative.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <param name="result">The product of both matrices.</param>
        public static void Multiply(ref JMatrix4 matrix1, ref JMatrix4 matrix2, out JMatrix4 result)
        {
            float m11 = (((matrix1.M00 * matrix2.M00) + (matrix1.M01 * matrix2.M10)) + (matrix1.M02 * matrix2.M20)) + (matrix1.M03 * matrix2.M30);
            float m12 = (((matrix1.M00 * matrix2.M01) + (matrix1.M01 * matrix2.M11)) + (matrix1.M02 * matrix2.M21)) + (matrix1.M03 * matrix2.M31);
            float m13 = (((matrix1.M00 * matrix2.M02) + (matrix1.M01 * matrix2.M12)) + (matrix1.M02 * matrix2.M22)) + (matrix1.M03 * matrix2.M32);
            float m14 = (((matrix1.M00 * matrix2.M03) + (matrix1.M01 * matrix2.M13)) + (matrix1.M02 * matrix2.M23)) + (matrix1.M03 * matrix2.M33);

            float m21 = (((matrix1.M10 * matrix2.M00) + (matrix1.M11 * matrix2.M10)) + (matrix1.M12 * matrix2.M20)) + (matrix1.M13 * matrix2.M30);
            float m22 = (((matrix1.M10 * matrix2.M01) + (matrix1.M11 * matrix2.M11)) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31);
            float m23 = (((matrix1.M10 * matrix2.M02) + (matrix1.M11 * matrix2.M12)) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32);
            float m24 = (((matrix1.M10 * matrix2.M03) + (matrix1.M11 * matrix2.M13)) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33);

            float m31 = (((matrix1.M20 * matrix2.M00) + (matrix1.M21 * matrix2.M10)) + (matrix1.M22 * matrix2.M20)) + (matrix1.M23 * matrix2.M30);
            float m32 = (((matrix1.M20 * matrix2.M01) + (matrix1.M21 * matrix2.M11)) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31);
            float m33 = (((matrix1.M20 * matrix2.M02) + (matrix1.M21 * matrix2.M12)) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32);
            float m34 = (((matrix1.M20 * matrix2.M03) + (matrix1.M21 * matrix2.M13)) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33);

            float m41 = (((matrix1.M30 * matrix2.M00) + (matrix1.M31 * matrix2.M10)) + (matrix1.M32 * matrix2.M20)) + (matrix1.M33 * matrix2.M30);
            float m42 = (((matrix1.M30 * matrix2.M01) + (matrix1.M31 * matrix2.M11)) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31);
            float m43 = (((matrix1.M30 * matrix2.M02) + (matrix1.M31 * matrix2.M12)) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32);
            float m44 = (((matrix1.M30 * matrix2.M03) + (matrix1.M31 * matrix2.M13)) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33);

            result.M00 = m11;
            result.M01 = m12;
            result.M02 = m13;
            result.M03 = m14;

            result.M10 = m21;
            result.M11 = m22;
            result.M12 = m23;
            result.M13 = m24;

            result.M20 = m31;
            result.M21 = m32;
            result.M22 = m33;
            result.M23 = m34;

            result.M30 = m41;
            result.M31 = m42;
            result.M32 = m43;
            result.M33 = m44;
        }

        /// <summary>
        /// Matrices are added.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>The sum of both matrices.</returns>
        #region public static JMatrix4 Add(JMatrix4 matrix1, JMatrix4 matrix2)
        public static JMatrix4 Add(JMatrix4 matrix1, JMatrix4 matrix2)
        {
            JMatrix4 result;
            JMatrix4.Add(ref matrix1, ref matrix2, out result);
            return result;
        }

        /// <summary>
        /// Matrices are added.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <param name="result">The sum of both matrices.</param>
        public static void Add(ref JMatrix4 matrix1, ref JMatrix4 matrix2, out JMatrix4 result)
        {
            result.M00 = matrix1.M00 + matrix2.M00;
            result.M01 = matrix1.M01 + matrix2.M01;
            result.M02 = matrix1.M02 + matrix2.M02;
            result.M03 = matrix1.M03 + matrix2.M03;

            result.M10 = matrix1.M10 + matrix2.M10;
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;

            result.M20 = matrix1.M20 + matrix2.M20;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;

            result.M30 = matrix1.M30 + matrix2.M30;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
        }
        #endregion

        /*/// <summary>
        /// Calculates the inverse of a give matrix.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <returns>The inverted JMatrix4.</returns>
        public static JMatrix4 Inverse(JMatrix4 matrix)
        {
            JMatrix4 result;
            JMatrix4.Inverse(ref matrix, out result);
            return result;
        }*/

        public float Determinant()
        {
              return
                 M03 * M12 * M21 * M30 - M02 * M13 * M21 * M30 -
                 M03 * M11 * M22 * M30 + M01 * M13 * M22 * M30 +
                 M02 * M11 * M23 * M30 - M01 * M12 * M23 * M30 -
                 M03 * M12 * M20 * M31 + M02 * M13 * M20 * M31 +
                 M03 * M10 * M22 * M31 - M00 * M13 * M22 * M31 -
                 M02 * M10 * M23 * M31 + M00 * M12 * M23 * M31 +
                 M03 * M11 * M20 * M32 - M01 * M13 * M20 * M32 -
                 M03 * M10 * M21 * M32 + M00 * M13 * M21 * M32 +
                 M01 * M10 * M23 * M32 - M00 * M11 * M23 * M32 -
                 M02 * M11 * M20 * M33 + M01 * M12 * M20 * M33 +
                 M02 * M10 * M21 * M33 - M00 * M12 * M21 * M33 -
                 M01 * M10 * M22 * M33 + M00 * M11 * M22 * M33;
        }

        /*public static void Invert(ref JMatrix4 matrix, out JMatrix4 result)
        {
            float determinantInverse = 1 / matrix.Determinant();
            float m11 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * determinantInverse;
            float m12 = (matrix.M02 * matrix.M21 - matrix.M22 * matrix.M01) * determinantInverse;
            float m13 = (matrix.M01 * matrix.M12 - matrix.M11 * matrix.M02) * determinantInverse;

            float m21 = (matrix.M12 * matrix.M20 - matrix.M10 * matrix.M22) * determinantInverse;
            float m22 = (matrix.M00 * matrix.M22 - matrix.M02 * matrix.M20) * determinantInverse;
            float m23 = (matrix.M02 * matrix.M10 - matrix.M00 * matrix.M12) * determinantInverse;

            float m31 = (matrix.M10 * matrix.M21 - matrix.M11 * matrix.M20) * determinantInverse;
            float m32 = (matrix.M01 * matrix.M20 - matrix.M00 * matrix.M21) * determinantInverse;
            float m33 = (matrix.M00 * matrix.M11 - matrix.M01 * matrix.M10) * determinantInverse;

            result.M00 = m11;
            result.M01 = m12;
            result.M02 = m13;

            result.M10 = m21;
            result.M11 = m22;
            result.M12 = m23;

            result.M20 = m31;
            result.M21 = m32;
            result.M22 = m33;
        }

        /// <summary>
        /// Calculates the inverse of a give matrix.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <param name="result">The inverted JMatrix4.</param>
        public static void Inverse(ref JMatrix4 matrix, out JMatrix4 result)
        {
            float det = matrix.M00 * matrix.M11 * matrix.M22 -
                matrix.M00 * matrix.M12 * matrix.M21 -
                matrix.M01 * matrix.M10 * matrix.M22 +
                matrix.M01 * matrix.M12 * matrix.M20 +
                matrix.M02 * matrix.M10 * matrix.M21 -
                matrix.M02 * matrix.M11 * matrix.M20;

            float num11 = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
            float num12 = matrix.M02 * matrix.M21 - matrix.M01 * matrix.M22;
            float num13 = matrix.M01 * matrix.M12 - matrix.M11 * matrix.M02;

            float num21 = matrix.M12 * matrix.M20 - matrix.M22 * matrix.M10;
            float num22 = matrix.M00 * matrix.M22 - matrix.M20 * matrix.M02;
            float num23 = matrix.M02 * matrix.M10 - matrix.M12 * matrix.M00;

            float num31 = matrix.M10 * matrix.M21 - matrix.M20 * matrix.M11;
            float num32 = matrix.M01 * matrix.M20 - matrix.M21 * matrix.M00;
            float num33 = matrix.M00 * matrix.M11 - matrix.M10 * matrix.M01;

            result.M00 = num11 / det;
            result.M01 = num12 / det;
            result.M02 = num13 / det;
            result.M10 = num21 / det;
            result.M11 = num22 / det;
            result.M12 = num23 / det;
            result.M20 = num31 / det;
            result.M21 = num32 / det;
            result.M22 = num33 / det;
        }*/

        /// <summary>
        /// Multiply a matrix by a scalefactor.
        /// </summary>
        /// <param name="matrix1">The matrix.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>A JMatrix4 multiplied by the scale factor.</returns>
        #region public static JMatrix4 Multiply(JMatrix4 matrix1, float scaleFactor)
        public static JMatrix4 Multiply(JMatrix4 matrix1, float scaleFactor)
        {
            JMatrix4 result;
            JMatrix4.Multiply(ref matrix1, scaleFactor, out result);
            return result;
        }

        /// <summary>
        /// Multiply a matrix by a scalefactor.
        /// </summary>
        /// <param name="matrix1">The matrix.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <param name="result">A JMatrix4 multiplied by the scale factor.</param>
        public static void Multiply(ref JMatrix4 matrix1, float scaleFactor, out JMatrix4 result)
        {
            float num = scaleFactor;
            result.M00 = matrix1.M00 * num;
            result.M01 = matrix1.M01 * num;
            result.M02 = matrix1.M02 * num;
            result.M03 = matrix1.M03 * num;

            result.M10 = matrix1.M10 * num;
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;

            result.M20 = matrix1.M20 * num;
            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;

            result.M30 = matrix1.M30 * num;
            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
        }
        #endregion

       /* /// <summary>
        /// Creates a JMatrix4 representing an orientation from a quaternion.
        /// </summary>
        /// <param name="quaternion">The quaternion the matrix should be created from.</param>
        /// <returns>JMatrix4 representing an orientation.</returns>
        #region public static JMatrix4 CreateFromQuaternion(JQuaternion quaternion)

        public static JMatrix4 CreateFromQuaternion(JQuaternion quaternion)
        {
            JMatrix4 result;
            JMatrix4.CreateFromQuaternion(ref quaternion,out result);
            return result;
        }

        /// <summary>
        /// Creates a JMatrix4 representing an orientation from a quaternion.
        /// </summary>
        /// <param name="quaternion">The quaternion the matrix should be created from.</param>
        /// <param name="result">JMatrix4 representing an orientation.</param>
        public static void CreateFromQuaternion(ref JQuaternion quaternion, out JMatrix4 result)
        {
            float num9 = quaternion.X * quaternion.X;
            float num8 = quaternion.Y * quaternion.Y;
            float num7 = quaternion.Z * quaternion.Z;
            float num6 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num4 = quaternion.Z * quaternion.X;
            float num3 = quaternion.Y * quaternion.W;
            float num2 = quaternion.Y * quaternion.Z;
            float num = quaternion.X * quaternion.W;
            result.M00 = 1f - (2f * (num8 + num7));
            result.M01 = 2f * (num6 + num5);
            result.M02 = 2f * (num4 - num3);
            result.M10 = 2f * (num6 - num5);
            result.M11 = 1f - (2f * (num7 + num9));
            result.M12 = 2f * (num2 + num);
            result.M20 = 2f * (num4 + num3);
            result.M21 = 2f * (num2 - num);
            result.M22 = 1f - (2f * (num8 + num9));
        }
        #endregion*/

        /// <summary>
        /// Creates the transposed matrix.
        /// </summary>
        /// <param name="matrix">The matrix which should be transposed.</param>
        /// <returns>The transposed JMatrix4.</returns>
        #region public static JMatrix4 Transpose(JMatrix4 matrix)
        public static JMatrix4 Transpose(JMatrix4 matrix)
        {
            JMatrix4 result;
            JMatrix4.Transpose(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Creates the transposed matrix.
        /// </summary>
        /// <param name="matrix">The matrix which should be transposed.</param>
        /// <param name="result">The transposed JMatrix4.</param>
        public static void Transpose(ref JMatrix4 matrix, out JMatrix4 result)
        {
            result.M00 = matrix.M00;
            result.M01 = matrix.M10;
            result.M02 = matrix.M20;
            result.M03 = matrix.M30;

            result.M10 = matrix.M01;
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;

            result.M20 = matrix.M02;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;

            result.M30 = matrix.M03;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
        }
        #endregion

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The product of both values.</returns>
        #region public static JMatrix4 operator *(JMatrix4 value1,JMatrix4 value2)
        public static JMatrix4 operator *(JMatrix4 value1,JMatrix4 value2)
        {
            JMatrix4 result; JMatrix4.Multiply(ref value1, ref value2, out result);
            return result;
        }
        #endregion


        public float Trace()
        {
            return this.M00 + this.M11 + this.M22 + this.M33;
        }

        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The sum of both values.</returns>
        #region public static JMatrix4 operator +(JMatrix4 value1, JMatrix4 value2)
        public static JMatrix4 operator +(JMatrix4 value1, JMatrix4 value2)
        {
            JMatrix4 result; JMatrix4.Add(ref value1, ref value2, out result);
            return result;
        }
        #endregion

        /// <summary>
        /// Subtracts two matrices.
        /// </summary>
        /// <param name="value1">The first matrix.</param>
        /// <param name="value2">The second matrix.</param>
        /// <returns>The difference of both values.</returns>
        #region public static JMatrix4 operator -(JMatrix4 value1, JMatrix4 value2)
        public static JMatrix4 operator -(JMatrix4 value1, JMatrix4 value2)
        {
            JMatrix4 result; JMatrix4.Multiply(ref value2, -1.0f, out value2);
            JMatrix4.Add(ref value1, ref value2, out result);
            return result;
        }
        #endregion


        /*/// <summary>
        /// Creates a matrix which rotates around the given axis by the given angle.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="result">The resulting rotation matrix</param>
        #region public static void CreateFromAxisAngle(ref JVector axis, float angle, out JMatrix4 result)
        public static void CreateFromAxisAngle(ref JVector axis, float angle, out JMatrix4 result)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float num2 = (float)Math.Sin((double)angle);
            float num = (float)Math.Cos((double)angle);
            float num11 = x * x;
            float num10 = y * y;
            float num9 = z * z;
            float num8 = x * y;
            float num7 = x * z;
            float num6 = y * z;
            result.M00 = num11 + (num * (1f - num11));
            result.M01 = (num8 - (num * num8)) + (num2 * z);
            result.M02 = (num7 - (num * num7)) - (num2 * y);
            result.M10 = (num8 - (num * num8)) - (num2 * z);
            result.M11 = num10 + (num * (1f - num10));
            result.M12 = (num6 - (num * num6)) + (num2 * x);
            result.M20 = (num7 - (num * num7)) + (num2 * y);
            result.M21 = (num6 - (num * num6)) - (num2 * x);
            result.M22 = num9 + (num * (1f - num9));
        }

        /// <summary>
        /// Creates a matrix which rotates around the given axis by the given angle.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        /// <returns>The resulting rotation matrix</returns>
        public static JMatrix4 CreateFromAxisAngle(JVector axis, float angle)
        {
            JMatrix4 result; CreateFromAxisAngle(ref axis, angle, out result);
            return result;
        }

        #endregion*/


        public static JVector ViewMatrixLeft(JMatrix4 viewMatrix)
        {
            var vec = ViewMatrixRight(viewMatrix);
            vec.Negate();
            return vec;
        }

        public static JVector ViewMatrixRight(JMatrix4 viewMatrix)
        {
            return new JVector(viewMatrix.M00, viewMatrix.M10, viewMatrix.M20);
        }

        public static JVector ViewMatrixUp(JMatrix4 viewMatrix)
        {
            return new JVector(viewMatrix.M01, viewMatrix.M11, viewMatrix.M22);
        }

        public static JVector ViewMatrixDown(JMatrix4 viewMatrix)
        {
            var vec = ViewMatrixUp(viewMatrix);
            vec.Negate();
            return vec;
        }

        public static JVector ViewMatrixForward(JMatrix4 viewMatrix)
        {
            var vec = ViewMatrixBackward(viewMatrix);
            vec.Negate();
            return vec;
        }

        public static JVector ViewMatrixBackward(JMatrix4 viewMatrix)
        {
            return new JVector(viewMatrix.M02, viewMatrix.M12, viewMatrix.M22);
        }


        public static JMatrix4 CreateTranslation(JVector vector)
        {
            JMatrix4 Result = JMatrix4.Identity;
            Result.M20 = vector.X;
            Result.M21 = vector.Y;
            Result.M22 = vector.Z;
            return Result;            
        }
    }
}
