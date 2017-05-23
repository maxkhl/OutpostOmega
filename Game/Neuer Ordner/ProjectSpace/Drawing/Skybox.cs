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

            Size = 50;
            Position = Vector3.Zero;

            SkyShader = Shader.Load(
                new FileInfo(@"Content\Shader\Skybox\Skybox_VS.glsl"),
                new FileInfo(@"Content\Shader\Skybox\Skybox_FS.glsl"));
        }

        public float Brightness = 1.5f;
        public void Draw()
        {
            if (Disposing)
                return;

            SkyShader.Bind();      
            int colMapLocation = SkyShader.GetUniformLocation("colorMap");
            
            int brightnessLocation = SkyShader.GetUniformLocation("brightness");
            GL.Uniform1(brightnessLocation, Brightness);
            

            GL.Enable(EnableCap.Texture2D);
            for (int i = 0; i < Sides.Count; i++)
            {
                Matrix4 rotation = Sides[i].Rotation * Matrix4.CreateScale(Size) * Matrix4.CreateTranslation(Position);
                GL.LoadMatrix(ref rotation);

                Sides[i].Texture.Bind(TextureUnit.Texture0, colMapLocation);

                GL.Begin(PrimitiveType.Quads);

                //if (Sides[i].Rotation != Matrix4.Identity)
                //{
                    GL.Vertex3(new Vector3(1, 1, 1));
                    GL.TexCoord2(0, 0);
                    GL.Vertex3(new Vector3(1, 1, -1));
                    GL.TexCoord2(0, 1);
                    GL.Vertex3(new Vector3(1, -1, -1));
                    GL.TexCoord2(1, 1);
                    GL.Vertex3(new Vector3(1, -1, 1));
                    GL.TexCoord2(1, 0);
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

                GL.End();
            }
            SkyShader.UnBind();

            GL.Disable(EnableCap.Texture2D);

            //Reset Transform Matrix
            var ident = Matrix4.Identity;
            GL.LoadMatrix(ref ident);

            Tools.OpenGL.CheckError();
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
