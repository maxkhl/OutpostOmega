using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OutpostOmega.Game.Turf;

namespace OutpostOmega.Drawing.Game
{
    /// <summary>
    /// Used to display a chunk in the opengl world
    /// </summary>
    class ChunkDrawer : Mesh
    {
        private static Texture2D ChunkTileset;
        private static Texture2D ChunkTilesetNormal;
        private static Texture2D CableTileset;

        public Chunk SourceChunk { get; set; }
        public ChunkDrawer(Chunk chunk)
            : base(PrimitiveType.Triangles, Tools.Convert.Mesh.Vertex.Jitter_To_OpenGL(chunk.mesh), Tools.Convert.Mesh.Index.Jitter_To_OpenGL(chunk.mesh))
        {
            this.SourceChunk = chunk;
            this.Translation = Matrix4.CreateTranslation(chunk.Position.X, chunk.Position.Y, chunk.Position.Z);
            
            if (ChunkTileset == null)
            {
                var file = new FileInfo(@"Content\Textures\Tileset.png");
                ChunkTileset = new Texture2D(file);
            }

            if (ChunkTilesetNormal == null)
            {
                var file = new FileInfo(@"Content\Textures\Tileset_Normal.png");
                ChunkTilesetNormal = new Texture2D(file);
            }

            if (CableTileset == null)
            {
                var file = new FileInfo(@"Content\Textures\Cables.png");
                CableTileset = new Texture2D(file);
            }



            this.Textures.Add(TextureUnit.Texture0, new KeyValuePair<string, Texture2D>("colorMap", ChunkTileset));
            this.Textures.Add(TextureUnit.Texture1, new KeyValuePair<string, Texture2D>("dec1Map", CableTileset));
        }

        //override 

        public override void Update(double ElapsedTime)
        {
            if (SourceChunk.NeedsRender)
            {
                SourceChunk.Render();
                SetData(Tools.Convert.Mesh.Vertex.Jitter_To_OpenGL(SourceChunk.mesh), Tools.Convert.Mesh.Index.Jitter_To_OpenGL(SourceChunk.mesh));
            }

            Translation = Matrix4.CreateTranslation(Tools.Convert.Vector.Jitter_To_OpenGL(SourceChunk.Position));

            base.Update(ElapsedTime);
        }

        public override void SetShaderParameters(Shader shader)
        {
            //Test
            // Bind Normalmap
            GL.Uniform1(shader.GetUniformLocation("useDec1"), 1);
            int normalMapLocation = GL.GetUniformLocation(shader.ProgramHandle, "normalMap");
            ChunkTilesetNormal.Bind(TextureUnit.Texture1, normalMapLocation);
            //bl1.Bind(TextureUnit.Texture2);
            //bl2.Bind(TextureUnit.Texture3);

            //int myUniformLocation = GL.GetUniformLocation(Shader.ProgramHandle, "myUniform");
            base.SetShaderParameters(shader);
        }
        public override void Dispose()
        {
            Disposing = true;
            if (ChunkTileset != null)
            {
                ChunkTileset.Dispose();
                ChunkTileset = null;
            }

            if (ChunkTilesetNormal != null)
            {
                ChunkTilesetNormal.Dispose();
                ChunkTilesetNormal = null;
            }
            base.Dispose();
        }
    }
}
