using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OOContent = OutpostOmega.Game.Content;
using System.IO;
using grendgine_collada;

namespace OutpostOmega.Drawing.Game
{
    class gameObjectMesh : Mesh
    {
        public OOContent.Mesh cMesh { get; set; }

        public gameObjectMesh(OOContent.Mesh cMesh, PrimitiveType PrimitiveType, Vertex[] vertices, uint[] indices)
            : base(PrimitiveType, vertices, indices)
        {
            this.cMesh = cMesh;
        }

        public override void Update(double ElapsedTime)
        {
            base.Update(ElapsedTime);
        }

        public override void Draw(RenderOptions renderOptions)
        {
            Draw(renderOptions, false);
        }

        public void Draw(RenderOptions renderOptions, bool Transparent)
        {
            if (cMesh.Count < 1)
                return;


            bool needsDraw = false;
            foreach (var gOTexPairs in cMesh.gOTexPairs)
            {
                if (gOTexPairs.gameObject.Visible && DrawCondition(gOTexPairs.gameObject))
                {
                    if ((Transparent && gOTexPairs.UseAlpha) || (!Transparent && !gOTexPairs.UseAlpha))
                    {
                        needsDraw = true;
                        if (Transparent)
                        { }
                    }
                }
            }

            if (needsDraw)
            {
                base.BeginDraw(ref renderOptions);
                foreach (var gOTexPair in cMesh.gOTexPairs)
                {
                    // Prepare texture
                    if (this.Textures.ContainsKey(TextureUnit.Texture0))
                    {
                        if (gOTexPair.UserInterface != null)
                        { }
                        if (gOTexPair.UserInterface != null && gOTexPair.UserInterface.Data != null && ((dUserInterface)gOTexPair.UserInterface.Data).Loaded && ((dUserInterface)gOTexPair.UserInterface.Data).Handle != this.Textures[TextureUnit.Texture0].Value.Handle)
                        {
                            this.Textures[TextureUnit.Texture0] = new KeyValuePair<string, Texture2D>("colorMap", ((dUserInterface)gOTexPair.UserInterface.Data).RenderTarget.Texture);
                            this.Textures[TextureUnit.Texture1] = new KeyValuePair<string, Texture2D>("dec1Map", ((dUserInterface)gOTexPair.UserInterface.Data).RenderTarget.Texture);
                        }
                        else if (gOTexPair.Texture != null && gOTexPair.UserInterface == null && gOTexPair.Texture.Data != null && ((Texture2D)gOTexPair.Texture.Data).Handle != this.Textures[TextureUnit.Texture0].Value.Handle)
                        {
                            this.Textures[TextureUnit.Texture0] = new KeyValuePair<string, Texture2D>("colorMap", (Texture2D)gOTexPair.Texture.Data);
                            this.Textures[TextureUnit.Texture1] = new KeyValuePair<string, Texture2D>("dec1Map", (Texture2D)gOTexPair.Texture.Data);
                        }
                    }

                    if (this.UseAlpha != gOTexPair.UseAlpha)
                    {
                        this.UseAlpha = gOTexPair.UseAlpha;
                    }

                    if (gOTexPair.gameObject.Visible && DrawCondition(gOTexPair.gameObject))
                    {

                        if (typeof(OutpostOmega.Game.GameObjects.Structures.Machines.Doors.Airlock.AirlockDoor) == gOTexPair.gameObject.GetType())
                        {

                        }

                        if (typeof(OutpostOmega.Game.GameObjects.Structures.Machines.Doors.Airlock) == gOTexPair.gameObject.GetType())
                        {

                        }

                        if (gOTexPair.UserInterface != null)
                        { }

                        //Vector3 position = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(gOTexPair.gameObject.Position);
                        //Vector3 offset = OutpostOmega.Tools.Convert.Vector.Jitter_To_OpenGL(gOTexPair.gameObject.Offset);

                        //var Translationz = OutpostOmega.Tools.Convert.Matrix.Jitter_To_OpenGL_4(gOTexPair.gameObject.OTKTranslation);

                        //this.Translation = Matrix4.CreateTranslation(offset) * Tools.Convert.Matrix.Jitter_To_OpenGL_4(gOTexPair.gameObject.Orientation) * Matrix4.CreateTranslation(position) * Matrix4.CreateScale(gOTexPair.gameObject.Scale);


                        this.Translation = gOTexPair.gameObject.OTKTranslation;
                        base.DrawCore(ref renderOptions);
                    }
                }
                base.EndDraw(ref renderOptions);
            }
        }

        /// <summary>
        /// Used to specify draw conditions. (can be overridden)
        /// </summary>
        public virtual bool DrawCondition(OutpostOmega.Game.GameObject gameObject)
        {
            return gameObject.Registered;
        }

        public static List<gameObjectMesh> LoadModel(OutpostOmega.Game.Content.Model model, List<Tools.Collada.Mesh> ColladaMeshs)
        {
            var lShader = Shader.Load(
                    new System.IO.FileInfo(@"Content\Shader\Deferred\Deferred_VS.glsl"),
                    new System.IO.FileInfo(@"Content\Shader\Deferred\Deferred_FS.glsl"));

            // Get model path
            var path = new FileInfo(model.Path);

            // See if there is a image with the same name - this will be a fallback option
            var fallback_file = new FileInfo(path.Directory.FullName + "\\" + path.Name.Split('.')[0] + ".png");

            List<gameObjectMesh> ModelMeshs = new List<gameObjectMesh>();

            foreach (Tools.Collada.Mesh mesh in ColladaMeshs)
            {
                var cmMesh = (from mMesh in model.Meshs
                                 where mMesh.Key == mesh.Name
                                 select mMesh).First().Value;

                var modelMesh = new gameObjectMesh(cmMesh, OpenTK.Graphics.OpenGL.PrimitiveType.Triangles, mesh.Vertices, mesh.Indices[Grendgine_Collada_Input_Semantic.VERTEX]);
                modelMesh.Name = mesh.Name;

                var gameTexture = cmMesh.DefaultTexture;
                Drawing.Texture2D Texture = null;

                if (gameTexture != null)
                {
                    if (gameTexture.Data != null)
                        Texture = (Drawing.Texture2D)gameTexture.Data;
                    else
                        Texture = new Texture2D(gameTexture.FileInfo);
                }
                else if (fallback_file.Exists)
                {
                    Texture = new Texture2D(fallback_file);
                }
                else
                    throw new Exception("Unable to load model. No texture found");

                modelMesh.Textures.Add(OpenTK.Graphics.OpenGL.TextureUnit.Texture0, new KeyValuePair<string, Texture2D>("colorMap", Texture));

                modelMesh.Shader = lShader;
                ModelMeshs.Add(modelMesh);
            }
            return ModelMeshs;
        }
    }
}
