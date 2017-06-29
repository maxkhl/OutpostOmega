using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using OutpostOmega.Game.Turf.Types;
using OutpostOmega.Game.GameObjects;

namespace OutpostOmega.Game.Turf
{
    /// <summary>
    /// This is basically a collection of chunks in the game. It allowes to create multiple independent 
    /// objects in space like multiple stations, asteroids, satelites and so on
    /// </summary>
    public class Structure : IDisposable
    {
        /// <summary>
        /// All chunks featured in this structure
        /// </summary>
        public List<Chunk> Chunks { get; private set; }

        /// <summary>
        /// Unique ID of this structure
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Origin of this structure
        /// </summary>
        public JVector Origin { get; private set; }

        /// <summary>
        /// World, this structure is used in
        /// </summary>
        public World World { get; private set; }

        /// <summary>
        /// Height of this structure
        /// </summary>
        public int Height
        {
            get
            {
                int minHeight = 0,
                    maxHeight = 0;
                foreach(Chunk chunk in Chunks)
                {
                    int cMaxHeight = (int)chunk.Position.Y + Chunk.SizeXYZ;
                    if (cMaxHeight > maxHeight)
                        maxHeight = cMaxHeight;
                    
                    int cMinHeight = (int)chunk.Position.Y;
                    if (cMinHeight < minHeight)
                        minHeight = cMinHeight;
                }
                return Math.Abs(minHeight-maxHeight);
            }
        }

        /// <summary>
        /// Initializes a new structure with the given ID
        /// </summary>
        /// <param name="ID">ID of the new structure</param>
        public Structure(string ID = "structure")
        {
            this.ID = ID;
            Chunks = new List<Chunk>();
            Origin = new JVector();
        }

        /// <summary>
        /// Initializes a new structure with the given ID, bound to the given world
        /// </summary>
        /// <param name="World">World this structure should be bound to</param>
        /// <param name="ID">ID of the new structure</param>
        public Structure(World World, string ID = "structure")
        {
            this.ID = ID;
            Chunks = new List<Chunk>();
            Origin = new JVector();
            this.World = World;
        }

        /// <summary>
        /// Returns block relative to the world origin. F.e. [0, 0, 0] is the first block in this world
        /// </summary>
        /// <param name="wx">X-Coordinate</param>
        /// <param name="wy">Y-Coordinate</param>
        /// <param name="wz">Z-Coordinate</param>
        /// <returns></returns>
        public Block this[float wx, float wy, float wz]
        {
            get
            {
                JVector position = new JVector(wx,wy,wz);
                var hit = GetChunkAtPos(position);

                if (hit != null)
                {
                    JVector localPosition = position - hit.Position;
                    int x = (int)localPosition.X,
                        y = (int)localPosition.Y,
                        z = (int)localPosition.Z;

                    return hit[x, y, z];
                }
                else
                    return new Block();
            }
            set
            {
                JVector position = new JVector(wx, wy, wz);
                var hit = CreateChunkAtPos(position, true);
                if (hit != null)
                {
                    JVector localPosition = position - hit.Position;
                    int x = (int)localPosition.X,
                        y = (int)localPosition.Y,
                        z = (int)localPosition.Z;

                    hit[x, y, z] = value;
                }
            }
        }

        /// <summary>
        /// Use this to remove a block
        /// </summary>
        /// <param name="position">Position where the block should be removed</param>
        /// <returns>Block successfully removed?</returns>
        public bool Remove(JVector position)
        {
            return Add(TurfTypeE.space, position);
        }

        /// <summary>
        /// Adds a block to a chunk very fast. But be careful! This method wont do advanced security checks and can throw exceptions if fed wrong!
        /// </summary>
        /// <param name="chunk">Target chunk</param>
        /// <param name="block">Block that should be written</param>
        /// <param name="x">X-Coordinate inside the chunk</param>
        /// <param name="y">Y-Coordinate inside the chunk</param>
        /// <param name="z">Z-Coordinate inside the chunk</param>
        public void FastAdd(Chunk chunk, Block block, int x, int y, int z)
        {
            if (x < 0 || x > Chunk.SizeXYZ ||
                y < 0 || y > Chunk.SizeXYZ ||
                z < 0 || z > Chunk.SizeXYZ)
                throw new Exception("Fast block modify call out of bounds");

            chunk.Blocks[x, y, z] = block;

            RefreshIndoor((int)chunk.Position.X + x, (int)chunk.Position.Y + y, (int)chunk.Position.Z + z);
        }

        /// <summary>
        /// Returns the exact position of a block surrounding a given position
        /// </summary>
        public JVector GetBlockPosition(JVector position)
        {
            var targetChunk = CreateChunkAtPos(position, true);

            var relPosition = position - targetChunk.Position;
            relPosition *= Chunk.BlockSize;

            byte xB = (byte)relPosition.X,
                yB = (byte)relPosition.Y,
                zB = (byte)relPosition.Z;

            return targetChunk.Position + new JVector((float)xB, (float)yB, (float)zB);
        }

        /// <summary>
        /// Adds/overwrites a block at the given position. Nothing else to worry about with this function!
        /// </summary>
        /// <param name="block">Block that should be written</param>
        /// <param name="position">World-coordinate where to insert the block</param>
        /// <param name="CheckIntersection">Check for physical-objects that could collide (and probably totaly freak out) at the new blocks position?</param>
        /// <returns>Block added successful?</returns>
        public bool Add(TurfTypeE blockType, JVector position, bool CheckIntersection = true)
        {
            // Get the target chunk. (Involves check and create of the chunk if neccessary)
            Chunk targetChunk = CreateChunkAtPos(position, true);

            
            var relPosition = position - targetChunk.Position;
            relPosition *= Chunk.BlockSize;

            byte xB = (byte)relPosition.X,
                yB  = (byte)relPosition.Y,
                zB  = (byte)relPosition.Z;

            // Check Intersection
            if (CheckIntersection)
            {
                var originBlock = new JVector((float)xB, (float)yB, (float)zB);
                var newBlock = new JBBox(originBlock, originBlock + new JVector(Chunk.BlockSize));

                var gameObjects = (from gobj in World.AllGameObjects
                                   where gobj.IsPhysical
                                   select gobj);

                /*foreach (GameObject gObj in gameObjects)
                {
                    if (newBlock.Contains(gObj.RigidBody.BoundingBox) != JBBox.ContainmentType.Disjoint)
                        return false; //Got an Intersection -> forbid placement
                }*/
            }


            targetChunk[xB, yB, zB] = Block.Create(blockType, xB, yB, zB);

            RefreshIndoor((int)targetChunk.Position.X + xB, (int)targetChunk.Position.Y + yB, (int)targetChunk.Position.Z + zB);

            targetChunk.NeedsRender = true;

            /*bool[, ,] UpdateChunks = new bool[3, 3, 3] 
            {{{false, false, false},
             {false, true, false},
             {false, false, false}},

            {{false, true, false},
             {true, true, true},
             {false, true, false}},

            {{false, false, false},
             {false, true, false},
             {false, false, false}}};

            for (int x = 0; x < UpdateChunks.GetLength(0); x++)
                for (int y = 0; y < UpdateChunks.GetLength(1); y++)
                    for (int z = 0; z < UpdateChunks.GetLength(1); z++ )
                        if(UpdateChunks[x,y,z])
                        {
                            var neighbourChunk = this.GetChunkAtPos(
                                targetChunk.Position +
                                new JVector(x - 1, y - 1, z - 1));
                            if (neighbourChunk != null)
                                neighbourChunk.NeedsRender = true;
                        }*/

            return true;
        }

        /// <summary>
        /// Adds a new chunk to this structure and announces it to the game
        /// </summary>
        /// <param name="chunk">New chunk</param>
        public void Add(Chunk chunk)
        {
            Chunks.Add(chunk);
            chunk.Changed += Chunk_Changed;
            RegisterChunk(chunk);
        }

        /// <summary>
        /// Reacts to the chunk changed event. Translates the coordinates and then raises the structures event
        /// </summary>
        private void Chunk_Changed(Chunk sender, int X, int Y, int Z, Block block, bool Added)
        {
            var globalPosition = sender.Position + new JVector(X, Y, Z);
            Changed?.Invoke(this, globalPosition, block, Added);
        }
        
        /// <summary>
        /// Fired whenever changes to a block inside this structure happens
        /// </summary>
        public event ChangedHandler Changed;

        /// <summary>
        /// Handler for Changed event
        /// </summary>
        /// <param name="sender">Structure, that fired the event (this)</param>
        /// <param name="Position">Position, the change occured in world space</param>
        /// <param name="block">Copy of the block that got changed</param>
        /// <param name="Added">True if block got added</param>
        public delegate void ChangedHandler(Structure sender, JVector Position, Block block, bool Added);

        /// <summary>
        /// Used to announce a chunk to the gameworld
        /// </summary>
        private void RegisterChunk(Chunk chunk)
        {
            chunk.AssignedStructure = this;

            if (chunk.NeedsRender)
                chunk.Render(); // This will create the rigidbody aswell (if needed)

            World.PhysicSystem.AddBody(chunk.RigidBody);

            if (NewChunk != null)
                NewChunk(chunk);
        }

        /// <summary>
        /// Used to find a chunk at a given coordinate
        /// </summary>
        /// <param name="Position">Position that the chunks bounds cover</param>
        /// <returns>Found chunk, otherwise null!</returns>
        public Chunk GetChunkAtPos(JVector Position)
        {
            Chunk targetChunk = null;
            foreach (Chunk chunk in Chunks)
            {
                var containment = chunk.Bounds.Contains(Position);
                if (containment == JBBox.ContainmentType.Contains ||
                    containment == JBBox.ContainmentType.Intersects)
                {
                    // for the small occurence of hitting the corner of the chunk perfect
                    if (Position.X == chunk.Bounds.Max.X ||
                        Position.Y == chunk.Bounds.Max.Y ||
                        Position.Z == chunk.Bounds.Max.Z)
                        continue;
                    targetChunk = chunk;
                }
            }

            return targetChunk;
        }

        /// <summary>
        /// Creates a new chunk at the given position (correctly aligned to the grid of course)
        /// </summary>
        /// <param name="Position">Position, where a new chunk is needed (f.e. to add a block)</param>
        /// <param name="CheckExist">Check if we got a chunk already there?</param>
        /// <returns>Found or created chunk at given position</returns>
        public Chunk CreateChunkAtPos(JVector Position, bool CheckExist = true)
        {
            Chunk newChunk = null;

            // Check existance if requested
            if (CheckExist)
            {
                newChunk = GetChunkAtPos(Position);
                if (newChunk != null)
                    return newChunk;
            }

            // Create a new chunk
            var relativePosition = Position - Origin;
            int x = (int)Math.Floor(relativePosition.X / Chunk.SizeXYZ),
                y = (int)Math.Floor(relativePosition.Y / Chunk.SizeXYZ),
                z = (int)Math.Floor(relativePosition.Z / Chunk.SizeXYZ);

            newChunk = new Chunk();
            newChunk.Position = new JVector(
                (float)x * (float)Chunk.SizeXYZ,
                (float)y * (float)Chunk.SizeXYZ,
                (float)z * (float)Chunk.SizeXYZ);

            this.Add(newChunk);

            return newChunk;
        }

        /// <summary>
        /// Sends all the "new" events again
        /// </summary>
        public void ResendEvents()
        {
            foreach (Chunk chunk in Chunks)
                NewChunk(chunk);
        }

        /// <summary>
        /// Updates this structure (call every update cycle)
        /// </summary>
        public void Update()
        {
            /*for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].NeedsRender)
                    chunks[i].Render();
            }*/

        }


        /// <summary>
        /// Handler for NewChunk
        /// </summary>
        /// <param name="newChunk">New chunk that got added</param>
        public delegate void NewChunkHandler(Chunk newChunk);

        /// <summary>
        /// Gets fired when a new chunk was added to this structure
        /// </summary>
        public event NewChunkHandler NewChunk;


        /// <summary>
        /// Adds a flat structure to the given World
        /// </summary>
        public static void AddFlat(World world, int Size, int Height)
        {
            var newStruct = new Structure(world, "Flat_x"+Size.ToString());

            world.Structures.Add(newStruct);

            for (int x = -(Size / 2); x < Size / 2; x++)
                for (int z = -(Size / 2); z < Size / 2; z++)
                    for (int y = 0; y < Height; y++)
                        newStruct.Add(TurfTypeE.floor, new JVector((float)x, (float)y, (float)z));

        }

        /// <summary>
        /// Triggered when structure gets deserialized. Used to re-register all chunks
        /// </summary>
        public void OnDeserialization()
        {
            foreach(Chunk chunk in Chunks)
            {
                RegisterChunk(chunk);
            }
        }

        /// <summary>
        /// Checks if a block is indoor and refreshes all blocks in the same room
        /// </summary>
        /// <param name="x">X-Coordinate</param>
        /// <param name="y">Y-Coordinate</param>
        /// <param name="z">Z-Coordinate</param>
        public void RefreshIndoor(int x, int y, int z)
        {
            int Height = this.Height;
            for(int i = y + 1; i < Height; i++)
            {
                var block = this[x, i, z];
                if (block.IsAirtight)
                {
                    for (int c = y + 1; c < i; c++)
                    {
                        var indoorBlock = this[x, c, z];
                        indoorBlock.IsIndoor = true;
                        this[x, c, z] = indoorBlock;
                    }
                }
            }

            for (int i = y - 1; i >= 0; i--)
            {
                var block = this[x, i, z];
                if (block.IsAirtight)
                {
                    for (int c = i; c < y - 1; c++)
                    {
                        var indoorBlock = this[x, c, z];
                        indoorBlock.IsIndoor = true;
                        this[x, c, z] = indoorBlock;
                    }
                }
            }
        }

        public bool Disposing { get; set; }
        public void Dispose()
        {
            Disposing = true;
            foreach (Chunk chunk in Chunks)
                chunk.Dispose();
        }
    }
}
