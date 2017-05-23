using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing
{
    class Skybox : IDisposable
    {
        private struct SkyboxSide
        {
            public Texture2D Texture;
            public Matrix4 Rotation;
        }

        private List<SkyboxSide> Sides;

        private Shader SkyShader;

        private Vertex[] Vertices;

        private RenderOptions renderOptions;

        /// <summary>
        /// Size of the skybox
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Position of the skybox
        /// </summary>
        public Vector3 Position { get; set; }

        public Skybox(string SkyboxID, string FileEnding = "bmp")
        {
            Sides = new List<SkyboxSide>();

            var tTex = new Texture2D(new FileInfo(@"Content\Image\Skybox\" + SkyboxID + @"\Front." + FileEnding));
            Sides.Add(new SkyboxSide()
            {
                Texture = tTex,
                Rotation = Matrix4.CreateTranslation(9999,9999,9999)
            });

            Sides.Add(new SkyboxSide()
                {
                    Texture = tTex,
                    Rotation = Matrix4.Identity
                });
            
            Sides.Add(new SkyboxSide()
            {
                Texture = new Texture2D(new FileInfo(@"Content\Image\Skybox\" + SkyboxID + @"\Left." + FileEnding)),
                Rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90))
            });

            Sides.Add(new SkyboxSide()
            {
                Texture = new Texture2D(new FileInfo(@"Content\Image\Skybox\" + SkyboxID + @"\Back." + FileEnding)),
                Rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(180))
            });

            Sides.Add(new SkyboxSide()
            {
                Texture = new Texture2D(new FileInfo(@"Content\Image\Skybox\" + SkyboxID + @"\Right." + FileEnding)),
                Rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-90))
            });

            Sides.Add(new SkyboxSide()
            {
                Texture = new Texture2D(new FileInfo(@"Content\Image\Skybox\" + SkyboxID + @"\Top." + FileEnding)),
                Rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90)) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90))
            });

            Sides.Add(new SkyboxSide()
            {
                Texture = new Texture2D(new FileInfo(@"Content\Image\Skybox\" + SkyboxID + @"\Bottom." + FileEnding)),
                Rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90)) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-90))
            });

            foreach(SkyboxSide side in Sides)
                side.Texture.filterMode = Texture2D.FilterMode.Clamp;

            Size = 500;
            Position = Vector3.Zero;

            SkyShader = new Shader(
                new FileInfo(@"Content\Shader\Skybox\Skybox_VS.glsl"),
                new FileInfo(@"Content\Shader\Skybox\Skybox_FS.glsl"));

            var lvert = new List<Vertex>();

            for (int i = 0; i < Sides.Count; i++)
            {
                lvert.Add(new Vertex()
                {
                    Position = Vector3.Transform(new Vector3(1, 1, 1), Sides[i].Rotation * Matrix4.CreateScale(Size)),
                    TexCoord = new Vector2(0, 0)
                });

                lvert.Add(new Vertex()
                {
                    Position = Vector3.Transform(new Vector3(1, 1, -1), Sides[i].Rotation * Matrix4.CreateScale(Size)),
                    TexCoord = new Vector2(0, 1)
                });

                lvert.Add(new Vertex()
                {
                    Position = Vector3.Transform(new Vector3(1, -1, -1), Sides[i].Rotation * Matrix4.CreateScale(Size)),
                    TexCoord = new Vector2(1, 1)
                });

                lvert.Add(new Vertex()
                {
                    Position = Vector3.Transform(new Vector3(1, -1, 1), Sides[i].Rotation * Matrix4.CreateScale(Size)),
                    TexCoord = new Vector2(1, 0)
                });
            }

            Vertices = lvert.ToArray();

            renderOptions = new RenderOptions()
            {
                Shader = SkyShader,
                SetUniform = new Action<Shader>(SetUniform)
            };
        }

        public float Brightness = 1.5f;

        private int SideIndex = 0;
        public void Draw()
        {
            if (Disposing)
                return;

            //SkyShader.Bind();      
            
            

           // GL.Enable(EnableCap.Texture2D);
            for (int i = 0; i < Sides.Count; i++)
            {
                SideIndex = i;

                Mesh.DrawImmediate(renderOptions, Vertices);
                //GL.Begin(PrimitiveType.Quads);

                //if (Sides[i].Rotation != Matrix4.Identity)
                //{
                    /*GL.Vertex3(new Vector3(1, 1, 1));
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(new Vector3(1, 1, -1));
                    GL.TexCoord2(0, 1);
                    GL.Vertex3(new Vector3(1, -1, -1));
                    GL.TexCoord2(1, 1);
                    GL.Vertex3(new Vector3(1, -1, 1));
                    GL.TexCoord2(1, 0);*/
                //}
                //else
                //{
                //    GL.Vertex3(new Vector3(1, 1, 1));
                //    GL.TexCoord2(1, 1);
                //    GL.Vertex3(new Vector3(1, 1, -1));
                //    GL.TexCoord2(1, 0);
                //    GL.Vertex3(new Vector3(1, -1, -1));
                //    GL.TexCoord2(0, 1);
                //    GL.Vertex3(new Vector3(1, -1, 1));
                //    GL.TexCoord2(0, 0);
                //}

                //GL.End();
            }
            //SkyShader.UnBind();

            //GL.Disable(EnableCap.Texture2D);

            //Reset Transform Matrix
            var ident = Matrix4.Identity;
            GL.LoadMatrix(ref ident);

            //Disabel Vertex attributes
            /*GL.DisableVertexAttribArray(0); // Tangents
            GL.DisableVertexAttribArray(1); // BiTangents

            //Reset Client states
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);

            GL.DisableClientState(ArrayCap.ColorArray);

            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.Clear(ClearBufferMask.DepthBufferBit);*/

            Tools.OpenGL.CheckError();
        }

        public void SetUniform(Shader shader)
        {
            int colMapLocation = shader.GetUniformLocation("colorMap");
            Sides[SideIndex].Texture.Bind(TextureUnit.Texture0, colMapLocation);

            int brightnessLocation = SkyShader.GetUniformLocation("brightness");
            GL.Uniform1(brightnessLocation, Brightness);

            Matrix4 rotation = Matrix4.CreateTranslation(Position);
            GL.LoadMatrix(ref rotation);
        }

        public bool Disposing { get; set; }
        public void Dispose()
        {
            Disposing = true;
            for (int i = 0; i < Sides.Count; i++)
            {
                Sides[i].Texture.Dispose();                
            }
            Sides.Clear();
        }
    }
}
