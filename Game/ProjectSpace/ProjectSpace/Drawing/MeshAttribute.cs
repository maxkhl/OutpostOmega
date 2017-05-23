using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing
{
    /// <summary>
    /// Contains a VBO-attribute that can be attached to a mesh to be passed to the shader
    /// </summary>
    class MeshAttribute<T>
    {
        private int Handle
        {
            get
            {
                if (_Handle == 0)
                {
                    GL.GenBuffers(1, out _Handle);
                    if (_Handle == 0)
                        throw new Exception("Could not create attribute buffer.");
                }

                return _Handle;
            }
        }
        private int _Handle;

        private int Stride = 0;
        private int Size = 0;
        private TexCoordPointerType DataType;


        public MeshAttribute(TexCoordPointerType DataType, int Size)
        {
            this.DataType = DataType;
            this.Size = Size;
        }
        public MeshAttribute(TexCoordPointerType DataType, int Size, int Stride)
        {
            this.DataType = DataType;
            this.Size = Size;
            this.Stride = Stride;
        }

        public void SetData(T[] Data)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
            //GL.BufferData<T>(BufferTarget.ArrayBuffer, new IntPtr(Data.Length * Marshal.SizeOf(typeof(T))), Data, BufferUsageHint.StaticDraw);
        }
    }
}
