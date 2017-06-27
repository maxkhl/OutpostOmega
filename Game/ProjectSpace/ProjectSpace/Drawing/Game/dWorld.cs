using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OutpostOmega.Game;
using OutpostOmega.Game.Turf;
using System.IO;
using OutpostOmega.Game.Tools;

namespace OutpostOmega.Drawing.Game
{
    /// <summary>
    /// Used to handle the gameworld on the frontend
    /// </summary>
    class WorldDrawer : IDisposable
    {
        /// <summary>
        /// Assigned World
        /// </summary>
        public World World { get; set; }

        //
        public List<ChunkDrawer> Chunks { get; set; }
        //public List<dGameObject> GameObjects { get; set; }
        public List<Light> Lights { get; set; }

        public List<gameObjectMesh> Meshs { get; set; }

        public Other.HighlightArea Highlight { get; set; }

        public List<Texture2D> Textures { get; set; }

        public List<dUserInterface> uInterfaces { get; set; }

        public Tools.DebugDrawer debugDrawer = new Tools.DebugDrawer();

        public Shader ChunkShader { get; set; }

        public Skybox skybox { get; set; }

        public Scenes.Game Scene { get; protected set; }

        public WorldDrawer(World World, Scenes.Game Parent)
        {
            this.World = World;
            this.Scene = Parent;
            this.Chunks = new List<ChunkDrawer>();
            this.Meshs = new List<gameObjectMesh>();
            this.Textures = new List<Texture2D>();
            this.uInterfaces = new List<dUserInterface>();
            this.Highlight = new Other.HighlightArea();

            // Load all existing content objects
            foreach (var cfile in World.ContentManager.LoadedContent)
                ContentManager_ContentChanged(cfile, OutpostOmega.Game.Content.ContentManager.ContentChange.Loaded, null);

            // Hook to the content event to see if something changes
            World.ContentManager.ContentChanged += ContentManager_ContentChanged;

            //this.GameObjects = new List<dGameObject>();
            World.Structures.CollectionChanged += Structures_CollectionChanged;

            //World.NewGameObject += World_NewGameObject;
            //World.GameObjectRemoved += World_GameObjectRemoved;


            skybox = new Skybox("Space02", "jpg");


            /*ChunkShader = new Drawing.Shader(
                new FileInfo(@"Content\Shader\SimplePointLight\SimplePointLight_VS.glsl"),
                new FileInfo(@"Content\Shader\SimplePointLight\SimplePointLight_FS.glsl"));*/
            ChunkShader = Drawing.Shader.Load(
                new FileInfo(@"Content\Shader\Deferred\Deferred_VS.glsl"),
                new FileInfo(@"Content\Shader\Deferred\Deferred_FS.glsl"));

            // Triggers all the new-object events in the world. 
            // Forcing it to re-send every, already existing, object
            World.ResendEvents();
        }

        void ContentManager_ContentChanged(OutpostOmega.Game.Content.ContentFile contentFile, OutpostOmega.Game.Content.ContentManager.ContentChange change, object Data)
        {
            var contentType = contentFile.GetType();

            // Model
            if (contentType == typeof(OutpostOmega.Game.Content.Model))
            {
                var model = (OutpostOmega.Game.Content.Model)contentFile;
                switch(change)
                {
                    case OutpostOmega.Game.Content.ContentManager.ContentChange.Loaded:
                        var newMeshs = Game.gameObjectMesh.LoadModel(model, Tools.Collada.ReadModel(contentFile.Path));
                        this.Meshs.AddRange(newMeshs);
                        break;
                    case OutpostOmega.Game.Content.ContentManager.ContentChange.Disposed:
                        var hits = (from mesh in this.Meshs
                                    where mesh.cMesh.Model == model
                                    select mesh);
                        foreach(var hit in hits)
                        {
                            hit.Dispose();
                        }
                        break;
                }
            }

            // Texture
            if (contentType == typeof(OutpostOmega.Game.Content.Texture))
            {
                switch (change)
                {
                    case OutpostOmega.Game.Content.ContentManager.ContentChange.Loaded:
                        var newTex = new Texture2D(contentFile.FileInfo);
                        contentFile.Data = newTex;
                        Textures.Add(newTex);
                        break;
                    case OutpostOmega.Game.Content.ContentManager.ContentChange.Disposed:
                        var target = (from tex in Textures
                             where tex.File.FullName == contentFile.FileInfo.FullName
                             select tex).SingleOrDefault();
                        if (target != null)
                        {
                            Textures.Remove(target);
                            target.Dispose();
                        }
                        break;
                }
            }
            
            // UserInterface
            if (contentType == typeof(OutpostOmega.Game.Content.UserInterface))
            {
                switch (change)
                {
                    case OutpostOmega.Game.Content.ContentManager.ContentChange.Loaded:
                        uInterfaces.Add(new dUserInterface(this.Scene, (OutpostOmega.Game.Content.UserInterface)contentFile));
                        break;
                    case OutpostOmega.Game.Content.ContentManager.ContentChange.Disposed:
                        var target = (from interf in uInterfaces
                                      where interf.UserInterface.FileInfo.FullName == contentFile.FileInfo.FullName
                                      select interf).SingleOrDefault();
                        if (target != null)
                        {
                            uInterfaces.Remove(target);
                            target.Dispose();
                        }
                        break;
                }
            }
        }

        /*void World_GameObjectRemoved(gameObject removedGameObject)
        {
            var dObjects = (from dObj in GameObjects
                            where dObj.GameObject == removedGameObject
                            select dObj).ToArray();
            for(int i = 0; i < dObjects.Length; i++)
            {
                dObjects[i].Dispose();
                GameObjects.Remove(dObjects[i]);
            }
        }

        void World_NewGameObject(gameObject newGameObject)
        {
            if (newGameObject.Model != null)
            {
                var newDrawGO = new dGameObject(newGameObject);
                GameObjects.Add(newDrawGO);
            }
        }*/

        void Structures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Handler anmelden
            if (e.NewItems != null)
                foreach (object newStructure in e.NewItems)
                {
                    var nStructure = (Structure)newStructure;
                    nStructure.newChunk += nStructure_newChunk;
                }

            // Handler abmelden
            if(e.OldItems != null)
                foreach (object oldStructure in e.OldItems)
                {
                    var oStructure = (Structure)oldStructure;
                    foreach (Chunk chunk in oStructure.chunks)
                        foreach (ChunkDrawer dchunk in (from cnk in Chunks where cnk.SourceChunk == chunk select cnk))
                            Chunks.Remove(dchunk);
                    oStructure.newChunk -= nStructure_newChunk;
                }
        }

        void nStructure_newChunk(Chunk newChunk)
        {
            var newdChunk = new ChunkDrawer(newChunk)
            {
                Shader = ChunkShader
            };
            Chunks.Add(newdChunk);
        }

        public float SimulationSpeedFactor = 1;
        public void Update(MouseState mouseState, OpenTK.Input.KeyboardState kState, double ElapsedTime)
        {
            World.Update(new OutpostOmega.Game.Tools.KeybeardState(kState), mouseState, ElapsedTime * SimulationSpeedFactor);

            Tools.Performance.Start("Update World Chunks");
            for (int i = 0; i < Chunks.Count; i++)
                Chunks[i].Update(ElapsedTime);
            Tools.Performance.Stop("Update World Chunks");

            Tools.Performance.Start("Update World Meshs");
            foreach (var mesh in Meshs)
                mesh.Update(ElapsedTime);
            //foreach (dGameObject gameobject in GameObjects)
            //    gameobject.Update();
            Highlight.Update(ElapsedTime);
            Tools.Performance.Stop("Update World Meshs");

            skybox.Position = Tools.Convert.Vector.Jitter_To_OpenGL(World.Player.Mob.View.Position);
        }

        public void Draw(RenderOptions renderOptions)
        {
            if (World.Debug)
                World.DebugPhysics(debugDrawer);

            Tools.Draw.Cursor(World);

            //World.Player.Mob.charController.DebugDraw(debugDrawer);




            Tools.Performance.Start("Draw World Meshs");

            foreach (var mesh in Meshs)
                mesh.Draw(renderOptions, false);
            //foreach (dGameObject gameobject in GameObjects)
            //    gameobject.Draw(renderOptions);
            Tools.Performance.Stop("Draw World Meshs");
            Tools.Performance.Start("Draw World Chunks");
            foreach (ChunkDrawer chunk in Chunks)
                chunk.Draw(renderOptions);
            Tools.Performance.Stop("Draw World Chunks");

            skybox.Draw();

            Tools.Performance.Start("Draw Transparent World Meshs");

            /*var sortedMeshs = new List<Mesh>();
            for (int i = 0; i < Meshs.Count; i++)
            {

            }*/

            foreach (var mesh in Meshs)
                mesh.Draw(renderOptions, true); // Transparent stuff last

            Highlight.Draw();
            Tools.Performance.Stop("Draw Transparent World Meshs");
        }

        public bool Disposing { get; set; }
        public void Dispose()
        {
            Disposing = true;

            World.ContentManager.ContentChanged -= ContentManager_ContentChanged;

            /*foreach (dGameObject gameobject in GameObjects)
                gameobject.Dispose();*/

            foreach (ChunkDrawer chunk in Chunks)
                chunk.Dispose();
        }
    }
}
