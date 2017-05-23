using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OutpostOmega.Game.Content
{
    /// <summary>
    /// A models mesh
    /// </summary>
    public class Mesh : IDisposable
    {

        /// <summary>
        /// Gets or sets the <see cref="Base"/> with the specified name.
        /// </summary>
        public gOPair this[GameObject gameObject]
        {
            get
            {
                gOPair Hit = null;
                if (Contains(gameObject, out Hit))
                {
                    /*var hit = (from gOTPair in gOTexPairs
                            where gOTPair.gameObject == gameObject
                            select gOTPair).SingleOrDefault().Texture;*/
                    return Hit;
                }
                else
                    return null;
            }
            set
            {
                if (Contains(gameObject))
                {
                    for (int i = 0; i < gOTexPairs.Count; i++)
                        if (gOTexPairs[i].gameObject == gameObject)
                            gOTexPairs[i] = new gOPair() { gameObject = gameObject, Texture = value.Texture };
                }
            }
        }

        /// <summary>
        /// Name of the mesh
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Texture of the mesh
        /// </summary>
        //public Texture Texture { get; set; }

        public Texture DefaultTexture { get; set; }

        /// <summary>
        /// Amount of gameObjects
        /// </summary>
        public int Count
        {
            get
            {
                if (gOTexPairs == null)
                    return 0;
                else
                    return gOTexPairs.Count;
            }
        }

        /// <summary>
        /// The meshs model
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// gameObjects that are using this mesh
        /// </summary>
        public List<gOPair> gOTexPairs { get; private set; }

        public class gOPair
        {
            public GameObject gameObject { get; set; }
            public Texture Texture { get; set; }
            public UserInterface UserInterface { get; set; }
            public bool UseAlpha = false;
        }

        public Mesh(string Name, Model Model)
        {
            this.Name = Name;
            this.Model = Model;
            this.gOTexPairs = new List<gOPair>();
        }

        /// <summary>
        /// Only used for serialization!
        /// </summary>
        public Mesh()
        { }

        public void AssignTexture(Texture Tex, GameObject Sender)
        {
            for(int i = 0; i < gOTexPairs.Count; i++)
                if(gOTexPairs[i].gameObject == Sender)
                    gOTexPairs[i] = new gOPair() { gameObject = Sender, Texture = Tex };
        }
        public void Add(GameObject gameObject)
        {
            bool contains = false;
            foreach (var pair in gOTexPairs)
                if (pair.gameObject == gameObject)
                    contains = true;

            if (!contains)
                gOTexPairs.Add(new gOPair() { gameObject = gameObject });
        }
        public void Remove(GameObject gameObject)
        {
            for (int i = 0; i < gOTexPairs.Count; i++)
                if (gOTexPairs[i].gameObject == gameObject)
                    gOTexPairs.RemoveAt(i);
        }

        public bool Contains(GameObject gameObject)
        {
            if (gOTexPairs == null)
                return false;

            bool contains = false;
            foreach (var pair in gOTexPairs)
                if (pair.gameObject == gameObject)
                    contains = true;
            return contains;
        }

        public gOPair GetGOPair(GameObject gameObject)
        {
            return this[gameObject];
        }

        public bool Contains(GameObject gameObject, out gOPair Hit)
        {
            Hit = null;
            if (gOTexPairs == null)
                return false;
            bool contains = false;
            foreach (var pair in gOTexPairs)
                if (pair.gameObject == gameObject)
                {
                    contains = true;
                    Hit = pair;
                }
            return contains;
        }

        public bool Disposing { get; set; }
        public void Dispose()
        {
            this.Disposing = true;
            foreach(var gOPair in gOTexPairs)
            {
                gOPair.gameObject.RemoveMesh(this);
            }
            gOTexPairs.Clear();
        }
    }
}
