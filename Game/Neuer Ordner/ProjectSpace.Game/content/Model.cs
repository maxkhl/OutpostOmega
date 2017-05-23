using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Jitter.LinearMath;
using System.IO;

namespace OutpostOmega.Game.Content
{
    public class Model : ContentFile
    {
        public Dictionary<string, Mesh> Meshs { get; set; }

        public Model(string Path)
            : base(Path)
        {
            ReadMeshData();
        }

        public Model(string Path, ContentManager Manager)
            : base(Path, Manager)
        {
            ReadMeshData();
        }

        public void ReadMeshData()
        {
            Meshs = new Dictionary<string, Mesh>();

            // Get all mesh names
            var colladaDef = XDocument.Load(Path);

            if (colladaDef == null)
                throw new Exception("Invalid model file " + Path);

            var colladaElements = colladaDef.Elements().First().Elements();

            IEnumerable<XElement> xmlMeshs = null;
            foreach (var element in colladaElements)
            {
                if (element.Name.LocalName == "library_geometries")
                    xmlMeshs = element.Elements();
            }

            foreach (var xmlMesh in xmlMeshs)
            {
                var name = xmlMesh.Attribute("name").Value;
                Meshs.Add(name, new Mesh(name, this));
            }
        }

        /// <summary>
        /// Returns all mesh names in this model
        /// </summary>
        public string[] GetMeshNames()
        {
            return Meshs.Keys.ToArray();
        }

        /// <summary>
        /// Assigns a texture to a specific model
        /// </summary>
        public void AssignTexture(string MeshName, GameObject GameObject, Texture texture)
        {
            if (!Meshs.ContainsKey(MeshName))
                throw new ArgumentException("Mesh name '" + MeshName + "' could not be found in this model. Use getmeshnames() to get all meshs in this model.");

            Meshs[MeshName][GameObject].Texture = texture;
        }

        /// <summary>
        /// Assigns a texture to a specific model
        /// </summary>
        public void AssignUserInterface(string MeshName, GameObject GameObject, UserInterface uInterface)
        {
            if (!Meshs.ContainsKey(MeshName))
                throw new ArgumentException("Mesh name '" + MeshName + "' could not be found in this model. Use getmeshnames() to get all meshs in this model.");

            Meshs[MeshName][GameObject].UserInterface = uInterface;
        }

        public void AssignGameObject(GameObject GameObject)
        {
            foreach (var meshPair in this.Meshs)
                meshPair.Value.gOTexPairs.Add(new Mesh.gOPair() { gameObject = GameObject, Texture = null });
        }

        public void AssignGameObject(string MeshName, GameObject GameObject)
        {
            foreach (var meshPair in this.Meshs)
                if (meshPair.Key == MeshName)
                    meshPair.Value.gOTexPairs.Add(new Mesh.gOPair() { gameObject = GameObject, Texture = null });
        }

        public void ReleaseGameObject(GameObject GameObject)
        {
            foreach (var meshPair in this.Meshs)
                if (meshPair.Value.Contains(GameObject))
                    meshPair.Value.Remove(GameObject);
        }

        public void ReleaseGameObject(string MeshName, GameObject GameObject)
        {
            foreach (var meshPair in this.Meshs)
                if (meshPair.Key == MeshName)
                    if (meshPair.Value.Contains(GameObject))
                        meshPair.Value.Remove(GameObject);
        }

        public Mesh[] GetMesh(GameObject AssignedGameObject)
        {
            return (from mesh in Meshs
                    where mesh.Value[AssignedGameObject] != null
                    select mesh.Value).ToArray();
        }
        
        public Mesh GetMesh(string Name)
        {
            return Meshs[Name];
        }

        /// <summary>
        /// Returns the texture that is bound to the given mesh. Might return null if no texture was assigned
        /// </summary>
        public Texture GetTexture(string MeshName, GameObject gameObject)
        {
            if (!Meshs.ContainsKey(MeshName))
                throw new ArgumentException("Mesh name '" + MeshName + "' could not be found in this model. Use getmeshnames() to get all meshs in this model.");

            return Meshs[MeshName][gameObject].Texture;
        }

        /// <summary>
        /// Returns the texture that is bound to the given mesh. Might return null if no texture was assigned
        /// </summary>
        public static Texture GetTexture(string MeshName, GameObject gameObject, Model model)
        {
            if (!model.Meshs.ContainsKey(MeshName))
                throw new ArgumentException("Mesh name '" + MeshName + "' could not be found in this model. Use getmeshnames() to get all meshs in this model.");

            return model.Meshs[MeshName][gameObject].Texture;
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (var meshKeyPair in Meshs)
                meshKeyPair.Value.Dispose();
        }
    }
}
