using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutpostOmega.Game;
using grendgine_collada;
using OpenTK;
using System.IO;

namespace OutpostOmega.Drawing.Game
{
    /// <summary>
    /// Used to draw a drawable GameObject
    /// </summary>
    class dGameObject : IDisposable
    {
        public GameObject AssignedGameObject { get; set; }

        public struct ModelMesh
        {
            public string Name;
            public Drawing.Mesh Mesh;
        }

        /// <summary>
        /// Blocks all translations for this object. The model will be drawn at its origin
        /// </summary>
        public bool NoTranslation { get; set; }

        /// <summary>
        /// Ignores all states on the gameobject to not draw it. (Almost guaranteed to get the object drawn - except NoDraw property)
        /// </summary>
        public bool ForceDraw { get; set; }

        public List<ModelMesh> ModelMeshs { get; set; }
        
        public dGameObject(GameObject GameObject)
            //: base(OpenTK.Graphics.OpenGL.PrimitiveType.Quads, GetVertices(GameObject, Model), GetIndices(GameObject, Model))
        {
            ModelMeshs = new List<ModelMesh>();
            this.AssignedGameObject = GameObject;

            // Watch the Properties

            // Load if it should be drawn
            //if (!GameObject.NoDraw)
                //LoadModel();
        }

        /*private static Vertex[] GetVertices(gameObject GameObject, Grendgine_Collada Model)
        {
            var vertices = new Vertex[1];


            return vertices;
        }
        private static uint[] GetIndices(gameObject GameObject, Grendgine_Collada Model)
        {
            var indices = new uint[1];


            return indices;
        }*/

        public void Update()
        {
            if (ModelMeshs.Count == 0) // No meshs? Return
                return;
        }

        public void Draw(RenderOptions renderOptions)
        {
            if (ModelMeshs.Count == 0) // No meshs? Return
                return;

            if (this.AssignedGameObject.Visible || ForceDraw)
            {
                //renderOptions.Color = true;
                //renderOptions.Shader = this.Shader;
                foreach (ModelMesh mMesh in ModelMeshs)
                    mMesh.Mesh.Draw(renderOptions);
            }
        }

        public bool Disposing { get; set; }
        public void Dispose()
        {
            Disposing = true;


            // Dispose every mesh
            foreach (ModelMesh mMesh in ModelMeshs)
                mMesh.Mesh.Dispose();
        }
    }
}
