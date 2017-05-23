using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OutpostOmega.Drawing
{
    class Mesh : IDisposable, iDrawable, iUpdateable
    {
        /// <summary>
        /// Default shader that gets pulled of no shader is selected
        /// </summary>
        public static Shader DefaultShader { get; set; }

        /// <summary>
        /// Specifies if this mesh should be visible
        /// </summary>
        public bool Visible { get; set; }

        public int Handle
        {
            get
            {
                // Create an id on first use.
                if (_Handle == 0)
                {
                    GraphicsContext.Assert();

                    GL.GenBuffers(1, out _Handle);
                    if (_Handle == 0)
                        throw new Exception("Could not create VBO.");
                }

                return _Handle;
            }
        }
        private int _Handle;

        private Vertex[] _vertices;
        private uint[] _indices;

        public Dictionary<TextureUnit, KeyValuePair<string, Texture2D>> Textures { get; set; }
        public Shader Shader { get; set; }

        public Matrix4 Translation = Matrix4.Identity;

        public bool UseAlpha = false;

        public string Name = "";

        public float SatMin = 0.1f;
        public float SatMax = 1.0f;

        private PrimitiveType _primitiveType = PrimitiveType.Triangles;

        public Mesh(PrimitiveType PrimitiveType, Vertex[] vertices, uint[] indices)
        {
            this.Visible = true;

            Textures = new Dictionary<TextureUnit, KeyValuePair<string, Texture2D>>();

            this._primitiveType = PrimitiveType;
            //this.Wireframe = true;
            SetData(vertices, indices);

            LoadDefaultShader();
        }
        public Mesh(PrimitiveType PrimitiveType, Vertex[] vertices, uint[] indices, Texture2D Texture)
        {
            this.Visible = true;

            Textures = new Dictionary<TextureUnit, KeyValuePair<string, Texture2D>>();

            this._primitiveType = PrimitiveType;

            SetData(vertices, indices);
            this.Textures.Add(TextureUnit.Texture0, new KeyValuePair<string, Texture2D>("colorMap", Texture));

            LoadDefaultShader();
        }

        public void LoadDefaultShader()
        {
            if(DefaultShader == null)
            {
                DefaultShader = Shader.Load(
                    new System.IO.FileInfo(@"Content\Shader\Deferred\Deferred_VS.glsl"),
                    new System.IO.FileInfo(@"Content\Shader\Deferred\Deferred_FS.glsl"));
            }
        }

        /// <summary>
        /// Used to set the VBO data
        /// </summary>
        /// <param name="vertices">Vertex Array</param>
        /// <param name="indices">Indices</param>
        public void SetData(Vertex[] vertices, uint[] indices)
        {
            /*foreach(var vert in vertices)
                if(vert.TexCoord2.X >= 0)
                {

                }*/

            if (vertices == null)
                throw new ArgumentNullException("data");

            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Drawing.Vertex.Stride), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            this._vertices = vertices;
            this._indices = indices;
        }

        public virtual void Update(double ElapsedTime)
        { }

        /// <summary>
        /// Preparation for mesh drawing. (Sets all the GL Stuff)
        /// </summary>
        private static void Prepare(RenderOptions renderOptions)
        {
            //Clean up textures
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture0);

            //Set Wireframe
            if (renderOptions.Wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.Enable(EnableCap.Texture2D);
            //GL.EnableClientState(ArrayCap.ColorArray);
            /*if (renderOptions.Color)
            {
                //Color
                GL.Disable(EnableCap.Texture2D);
            }
            else
            {
                //Texture
            }*/

            if (renderOptions.Shader != null)
                renderOptions.Shader.Bind();

            if (renderOptions.SetUniform != null)
                renderOptions.SetUniform(renderOptions.Shader);
        }

        /// <summary>
        /// Finalizes the draw cycle. (Sets all GL things to default again)
        /// </summary>
        /// <param name="renderOptions"></param>
        private static void Finalize(RenderOptions renderOptions)
        {
            //Unbind Shader
            if (renderOptions.Shader != null)
                renderOptions.Shader.UnBind();

            //Disabel Vertex attributes
            GL.DisableVertexAttribArray(0); // Tangents
            GL.DisableVertexAttribArray(1); // BiTangents

            //Reset Client states
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);

            GL.DisableClientState(ArrayCap.ColorArray);

            GL.ClientActiveTexture(TextureUnit.Texture3);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.ClientActiveTexture(TextureUnit.Texture2);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.ClientActiveTexture(TextureUnit.Texture1);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.ClientActiveTexture(TextureUnit.Texture0);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            //Reset Transform Matrix
            var ident = Matrix4.Identity;
            GL.LoadMatrix(ref ident);

            //Reset Wireframe
            if (renderOptions.Wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            //Reset Active Textures
            GL.ClientActiveTexture(TextureUnit.Texture0);
            GL.ActiveTexture(TextureUnit.Texture0);

            //Check for errors
            Tools.OpenGL.CheckError();
        }

        /// <summary>
        /// Starts drawing the mesh (comes before DrawCore)
        /// </summary>
        public virtual void BeginDraw(ref RenderOptions renderOptions)
        {
            // Set shader when not specified
            if (renderOptions.Shader == null)
                if (this.Shader == null)
                {
                    renderOptions.Shader = DefaultShader;
                }
                else
                {
                    renderOptions.Shader = this.Shader;
                }


            // Set uniform-action when not specified
            if (renderOptions.SetUniform == null)
                renderOptions.SetUniform = SetShaderParameters;



            //var texIndexLocation = renderOptions.Shader.GetAttribLocation("TextureIndex");
            //if (texIndexLocation >= 0)
            //    GL.EnableVertexAttribArray(0);

            // Begin
            Prepare(renderOptions);


            GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);

            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Stride, 0);

            GL.NormalPointer(NormalPointerType.Float, Vertex.Stride, Vector3.SizeInBytes);



            GL.ClientActiveTexture(TextureUnit.Texture0);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Stride, 2 * Vector3.SizeInBytes);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.ClientActiveTexture(TextureUnit.Texture1);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Stride, (2 * Vector3.SizeInBytes) + (Vector2.SizeInBytes));
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.ClientActiveTexture(TextureUnit.Texture2);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Stride, (2 * Vector3.SizeInBytes) + (Vector2.SizeInBytes * 2));
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.ClientActiveTexture(TextureUnit.Texture3);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Stride, (2 * Vector3.SizeInBytes) + (Vector2.SizeInBytes * 3));
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            //GL.VertexAttribPointer()
            //GL.EnableClientState(ArrayCap.TextureCoordArray);
           // GL.EnableClientState(ArrayCap.TextureCoordArray);
            //GL.EnableClientState(ArrayCap.TextureCoordArray);

            //GL.ColorPointer(3, ColorPointerType.Float, Vertex.Stride, new IntPtr((2 * Vector3.SizeInBytes) + Vector2.SizeInBytes));

            //GL.VertexAttribPointer(0, 2, TexCoordPointerType.Float, Vertex.Stride, new IntPtr(2 * Vector3.SizeInBytes));
            //if (texIndexLocation >= 0)
            //    GL.VertexAttribPointer(texIndexLocation, 1, VertexAttribPointerType.Byte, false, Vertex.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vertex.ColorSize + 1));

            // Tangents
            /*GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vertex.ColorSize + Vector3.SizeInBytes));
            GL.BindAttribLocation(Shader.ProgramHandle, 0, "tangent");

            // BiTangents
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Stride, new IntPtr(2 * Vector3.SizeInBytes + Vertex.ColorSize + Vector3.SizeInBytes * 2));
            GL.BindAttribLocation(Shader.ProgramHandle, 1, "bitangent");*/
        }

        /// <summary>
        /// Loads the translation and draws the mesh
        /// </summary>
        public virtual void DrawCore(ref RenderOptions renderOptions)
        {
            if (this.UseAlpha)
            {
                GL.DepthMask(false);
                GL.Enable(EnableCap.Blend);
            }

            GL.MatrixMode(MatrixMode.Modelview);

            GL.LoadMatrix(ref Translation);

            Tools.OpenGL.CheckError();

            int ColorLocation = GL.GetUniformLocation(renderOptions.Shader.ProgramHandle, "def_Color");
            if (ColorLocation >= 0)
                GL.Uniform4(ColorLocation, renderOptions.Color);

            // Bind all textures
            if (Textures.Count > 0)
            {
                foreach (var entry in Textures)
                {
                    int mapLocation = GL.GetUniformLocation(renderOptions.Shader.ProgramHandle, entry.Value.Key);
                    if (mapLocation >= 0)
                        entry.Value.Value.Bind(entry.Key, mapLocation);
                }
            }

            if (_indices != null)
                GL.DrawElements(_primitiveType, _indices.Length, DrawElementsType.UnsignedInt, _indices);
            else
                GL.DrawArrays(_primitiveType, 0, _vertices.Length);


            if (Textures.Count > 0)
            {
                foreach (var entry in Textures)
                {
                    entry.Value.Value.UnBind(entry.Key);
                }
            }


            if (this.UseAlpha)
            {
                GL.DepthMask(true);
                GL.Disable(EnableCap.Blend);
            }
        }
        /// <summary>
        /// Ends drawing the mesh (comes after DrawCore)
        /// </summary>
        public virtual void EndDraw(ref RenderOptions renderOptions)
        {
            //Reset vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // End
            Finalize(renderOptions);
        }

        /// <summary>
        /// Draws the VBO of this mesh to the screen. Use SetData() to generate/update the VBO
        /// </summary>
        public virtual void Draw()
        {
            if (!Visible) return; //invisible duh

            var renderOptions = new RenderOptions();
            renderOptions.Color = Color4.White;

            BeginDraw(ref renderOptions);

            DrawCore(ref renderOptions);

            EndDraw(ref renderOptions);
        }

        /// <summary>
        /// Draws the VBO of this mesh to the screen. Use SetData() to generate/update the VBO
        /// </summary>
        /// <param name="renderOptions">Rendering options</param>
        public virtual void Draw(RenderOptions renderOptions)
        {
            if (!Visible) return; //invisible duh

            BeginDraw(ref renderOptions);

            DrawCore(ref renderOptions);

            EndDraw(ref renderOptions);
        }

        /// <summary>
        /// Used to draw a mesh in immediate mode. Great mode for small, fast changing, meshes. Be careful with big meshs, will slow everything down as fuck
        /// </summary>
        public static void DrawImmediate(RenderOptions renderOptions, Vertex[] vertices)
        {
            // Begin
            Prepare(renderOptions);

            GL.Begin(PrimitiveType.Quads);

            foreach(Vertex vertex in vertices)
            {
                GL.Vertex3(vertex.Position);
                GL.TexCoord2(vertex.TexCoord1);
                GL.Normal3(vertex.Normal);
            }

            GL.End();

            // End
            Finalize(renderOptions);
        }

        /// <summary>
        /// Can be used to set the parameters for the shader or modify the draw call right before the model is drawn
        /// </summary>
        public virtual void SetShaderParameters(Shader shader)
        {
            GL.Uniform1(shader.GetUniformLocation("UseAlpha"), UseAlpha ? 1 : 0);
            GL.Uniform1(shader.GetUniformLocation("SatMin"), SatMin);
            GL.Uniform1(shader.GetUniformLocation("SatMax"), SatMax);         
        }

        public bool Disposing { get; set; }
        public virtual void Dispose()
        {
            Disposing = true;

            GL.DeleteBuffer(Handle);

            if (Textures.Count > 0)
            {
                foreach (KeyValuePair<TextureUnit, KeyValuePair<string, Texture2D>> entry in Textures)
                    entry.Value.Value.Dispose();
                Textures.Clear();
            }


            if (Shader != null)
                Shader.Dispose();
        }
    }

}
