using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OutpostOmega.Game;
using OutpostOmega.Game.Structures;
using OutpostOmega.Game.GameObjects;
using OutpostOmega.Game.Turf.Types;
using Jitter.LinearMath;
using Jitter;
using Jitter.Dynamics;

namespace OutpostOmega.Game.Turf
{
    /// <summary>
    /// A chunk contains a specific amount of blocks and is part of a structure
    /// Also a chunk is a independent generated model
    /// </summary>
    public class Chunk : IDisposable
    {
        public const float BlockSize = 1f;
        /// <summary>
        /// SizeXYZ = 1 * 2^LogSizeXYZ = 2 ^ 5 = 32. Used for performance optimization
        /// </summary>
        public const int LogSizeXYZ = 4;

        /// <summary>
        /// Size of the chunk in every dimension. (chunks are cubes here)
        /// </summary>
        public const int SizeXYZ = 1 << LogSizeXYZ;
        
        /// <summary>
        /// Can be used to calculate x % 32
        /// </summary>
        public const int MaskXYZ = SizeXYZ - 1;

        public Block[, ,] blocks { get; set; }

        /// <summary>
        /// Used to tell if the chunk got updated and needs to get rendered again
        /// </summary>
        public bool NeedsRender { get; set; }

        /// <summary>
        /// Atmosphere active in this chunk?
        /// </summary>
        public bool AtmosActive { get; set; }
                
        /// <summary>
        /// ID of this chunk
        /// </summary>
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }
        private string _ID;

        /// <summary>
        /// Gets/Sets the position of this chunk and refreshs the bounds-property
        /// </summary>
        public JVector Position 
        { 
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                bounds = new JBBox(_position, _position + new JVector(SizeXYZ));
                if (rigidBody != null)
                    rigidBody.Position = value;
            }
        }
        private JVector _position;

        /// <summary>
        /// Boundingbox around the whole chunk (even if its empty)
        /// </summary>
        public JBBox bounds { get; set; }

        /// <summary>
        /// Returns block relative to the chunk origin. F.e. [0, 0, 0] is the first block in this chunk
        /// </summary>
        /// <param name="lx">X-Coordinate</param>
        /// <param name="ly">Y-Coordinate</param>
        /// <param name="lz">Z-Coordinate</param>
        /// <returns>Chunk-relative Block</returns>
        public Block this[int lx, int ly, int lz]
        {
            get
            {
                return blocks[lx, ly, lz];
            }
            set
            {
                this.NeedsRender = true;

                blocks[lx, ly, lz] = value;

                //Raise changed event
                Changed?.Invoke(this, lx, ly, lz, value, value.type != (byte)TurfTypeE.space);
            }
        }

        public RigidBody rigidBody { get; set; }

        static private int _IDCounter = 1;

        /// <summary>
        /// Structure, this chunk is assigned to. Could be null because the structure has to assign this value
        /// </summary>
        public Turf.Structure AssignedStructure { get; set; }

        
        public Chunk()
        {
            blocks = new Block[SizeXYZ, SizeXYZ, SizeXYZ];
            this.Position = JVector.Zero;

            _ID = _IDCounter.ToString();
            _IDCounter++;
            NeedsRender = true;
        }

        public Chunk(Block[, ,] blocks, JVector Position)
        {
            this.blocks = blocks;
            this.Position = Position;

            _ID = _IDCounter.ToString();
            _IDCounter++;
            NeedsRender = true;
        }

        public delegate void ChangedHandler(Chunk sender, int X, int Y, int Z, Block block, bool Added);
        public event ChangedHandler Changed;

        private Jitter.Collision.Octree _octree;
        private Jitter.Collision.Shapes.TriangleMeshShape _shape;
        public void CreatePhysics()
        {
            /*if (mesh.Vertices == null ||
                this.rigidBody != null)
                return;*/

            var triList = new List<Jitter.Collision.TriangleVertexIndices>();
			for(int i = 0; i < mesh.Triangles.Length; i += 3)
                triList.Add(new Jitter.Collision.TriangleVertexIndices(mesh.Triangles[i+0], mesh.Triangles[i+2], mesh.Triangles[i+1]));


            if (this.rigidBody == null)
            {
                _octree = new Jitter.Collision.Octree(new List<JVector>(mesh.Vertices), triList);
                _octree.BuildOctree();
                _shape = new Jitter.Collision.Shapes.TriangleMeshShape(_octree);
                this.rigidBody = new RigidBody(_shape);
                this.rigidBody.IsStatic = true;
                this.rigidBody.AffectedByGravity = false;
                this.rigidBody.EnableDebugDraw = true;
                this.rigidBody.Material.KineticFriction = 0.3f;
                this.rigidBody.Material.StaticFriction = 0.6f;
                this.rigidBody.Material.Restitution = 0.1f;
                this.rigidBody.Mass = 8000;
                this.rigidBody.Position = this.Position;
            }
            else
            {
                _octree.SetTriangles(new List<JVector>(mesh.Vertices), triList);
                _octree.BuildOctree();
                _shape.UpdateShape();
            }
        }

        public struct Mesh
        {
            public JVector[] Vertices;
            public JVector[] Normals;
            public JVector2[] UV1;
            public JVector2[] UV2; //Used for cables
            public JVector2[] UV3; //Used for pipes
            public JVector2[] UV4; //unused
            public int[] Triangles;
            public JVector[] Tangents;
            public JVector[] BiTangents;
        }

        public Mesh mesh;
        public Mesh decMesh;

        /// <summary>
        /// Renders a mesh out of the blocks
        /// </summary>
        /// <returns>Mesh of this chunk</returns>
        public Mesh Render(Func<Block, bool> condition = null, bool MainMesh = true)
        {
            if (this.mesh.Vertices != null && !this.NeedsRender)
                return this.mesh;

            List<JVector> vertices = new List<JVector>();
            List<JVector> normals = new List<JVector>();
            List<JVector2> uv1 = new List<JVector2>();
            List<JVector2> uv2 = new List<JVector2>();
            List<JVector2> uv3 = new List<JVector2>();
            List<JVector2> uv4 = new List<JVector2>();
            List<int> triangles = new List<int>();
            List<JVector> tangents = new List<JVector>();
            List<JVector> bitangents = new List<JVector>();

            for (int x = 0; x < SizeXYZ; x++)
                for (int y = 0; y < SizeXYZ; y++)
                    for (int z = 0; z < SizeXYZ; z++)
                    {
                        Block block = blocks[x, y, z];

                        float xV = (float)x * BlockSize,
                            yV = (float)y * BlockSize,
                            zV = (float)z * BlockSize;

                        if (!block.IsVisible)
                            continue;

                        if (condition != null && !condition(block))
                            continue;

                        JVector[][] ltangents;

                        #region top&bottom
                        if (CheckNeighbour(x, y, z, JVector.Up))
                        {
                            int vertexIndex = vertices.Count;
                            vertices.Add(new JVector(xV, yV + BlockSize, zV));
                            vertices.Add(new JVector(xV, yV + BlockSize, zV + BlockSize));
                            vertices.Add(new JVector(xV + BlockSize, yV + BlockSize, zV + BlockSize));
                            vertices.Add(new JVector(xV + BlockSize, yV + BlockSize, zV));

                            normals.AddRange(GetNormals(ref vertices));

                            triangles.AddRange(GetIndices(vertexIndex));

                            uv1.AddRange(GetUV(block.TurfType.GetUVCoords(block)[Direction.Top]));

                            SetDecoys(GetCableUV(block, Direction.Top), uv2, uv3, uv4);

                                /*var uvs = new JVector2[4];
                                uvs[0] = new JVector2(0, 0);
                                uvs[1] = new JVector2(0.5f, 0);
                                uvs[2] = new JVector2(0.5f, 0.5f);
                                uvs[3] = new JVector2(0, 0.5f);
                                uv.AddRange(uvs);*/
                                //uv.AddRange(GetUV(block.GetTurfType()));

                                ltangents = GetTangents(ref vertices, ref uv1);
                            tangents.AddRange(ltangents[0]);
                            bitangents.AddRange(ltangents[1]);
                        }


                        if (CheckNeighbour(x, y, z, JVector.Down))
                        {
                            int vertexIndex = vertices.Count;
                            vertices.Add(new JVector(xV + BlockSize, yV, zV));
                            vertices.Add(new JVector(xV + BlockSize, yV, zV + BlockSize));
                            vertices.Add(new JVector(xV, yV, zV + BlockSize));
                            vertices.Add(new JVector(xV, yV, zV));

                            normals.AddRange(GetNormals(ref vertices));

                            triangles.AddRange(GetIndices(vertexIndex));

                            uv1.AddRange(GetUV(block.TurfType.GetUVCoords(block)[Direction.Bottom]));

                            SetDecoys(GetCableUV(block, Direction.Bottom), uv2, uv3, uv4);
                            /*var uvs = new JVector2[4];
                            uvs[0] = new JVector2(0, 0.5f);
                            uvs[1] = new JVector2(0.5f, 0.5f);
                            uvs[2] = new JVector2(0.5f, 1);
                            uvs[3] = new JVector2(0, 1);
                            uv.AddRange(uvs);*/
                            //uv.AddRange(GetUV(block.GetTurfType()));

                            ltangents = GetTangents(ref vertices, ref uv1);
                            tangents.AddRange(ltangents[0]);
                            bitangents.AddRange(ltangents[1]);
                        }
                        #endregion

                        #region left&right
                        if (CheckNeighbour(x, y, z, JVector.Left))
                        {
                            int vertexIndex = vertices.Count;
                            vertices.Add(new JVector(xV + BlockSize, yV + BlockSize, zV));
                            vertices.Add(new JVector(xV + BlockSize, yV + BlockSize, zV + BlockSize));
                            vertices.Add(new JVector(xV + BlockSize, yV, zV + BlockSize));
                            vertices.Add(new JVector(xV + BlockSize, yV, zV));

                            normals.AddRange(GetNormals(ref vertices));

                            triangles.AddRange(GetIndices(vertexIndex));

                            uv1.AddRange(GetUV(block.TurfType.GetUVCoords(block)[Direction.Left]));

                            SetDecoys(GetCableUV(block, Direction.Left), uv2, uv3, uv4);

                            ltangents = GetTangents(ref vertices, ref uv1);
                            tangents.AddRange(ltangents[0]);
                            bitangents.AddRange(ltangents[1]);
                        }

                        if (CheckNeighbour(x, y, z, JVector.Right))
                        {
                            int vertexIndex = vertices.Count;
                            vertices.Add(new JVector(xV, yV, zV));
                            vertices.Add(new JVector(xV, yV, zV + BlockSize));
                            vertices.Add(new JVector(xV, yV + BlockSize, zV + BlockSize));
                            vertices.Add(new JVector(xV, yV + BlockSize, zV));

                            normals.AddRange(GetNormals(ref vertices));

                            triangles.AddRange(GetIndices(vertexIndex));

                            uv1.AddRange(GetUV(block.TurfType.GetUVCoords(block)[Direction.Right]));

                            SetDecoys(GetCableUV(block, Direction.Right), uv2, uv3, uv4);

                            ltangents = GetTangents(ref vertices, ref uv1);
                            tangents.AddRange(ltangents[0]);
                            bitangents.AddRange(ltangents[1]);
                        }
                        #endregion

                        #region front&back
                        if (CheckNeighbour(x, y, z, JVector.Backward))
                        {
                            int vertexIndex = vertices.Count;
                            vertices.Add(new JVector(xV + BlockSize, yV, zV + BlockSize));
                            vertices.Add(new JVector(xV + BlockSize, yV + BlockSize, zV + BlockSize));
                            vertices.Add(new JVector(xV, yV + BlockSize, zV + BlockSize));
                            vertices.Add(new JVector(xV, yV, zV + BlockSize));

                            normals.AddRange(GetNormals(ref vertices));

                            triangles.AddRange(GetIndices(vertexIndex));

                            uv1.AddRange(GetUV(block.TurfType.GetUVCoords(block)[Direction.Back]));

                            SetDecoys(GetCableUV(block, Direction.Back), uv2, uv3, uv4);

                            ltangents = GetTangents(ref vertices, ref uv1);
                            tangents.AddRange(ltangents[0]);
                            bitangents.AddRange(ltangents[1]);
                        }

                        if (CheckNeighbour(x, y, z, JVector.Forward))
                        {
                            int vertexIndex = vertices.Count;
                            vertices.Add(new JVector(xV, yV, zV));
                            vertices.Add(new JVector(xV, yV + BlockSize, zV));
                            vertices.Add(new JVector(xV + BlockSize, yV + BlockSize, zV));
                            vertices.Add(new JVector(xV + BlockSize, yV, zV));

                            normals.AddRange(GetNormals(ref vertices));

                            triangles.AddRange(GetIndices(vertexIndex));

                            uv1.AddRange(GetUV(block.TurfType.GetUVCoords(block)[Direction.Front]));

                            SetDecoys(GetCableUV(block, Direction.Front), uv2, uv3, uv4);

                            ltangents = GetTangents(ref vertices, ref uv1);
                            tangents.AddRange(ltangents[0]);
                            bitangents.AddRange(ltangents[1]);
                        }
                        #endregion
                    }

            this.mesh = new Mesh()
                {
                    Vertices = vertices.ToArray(),
                    Triangles = triangles.ToArray(),
                    Normals = normals.ToArray(),
                    UV1 = uv1.ToArray(),
                    UV2 = uv2.ToArray(),
                    UV3 = uv3.ToArray(),
                    UV4 = uv4.ToArray(),
                    Tangents = tangents.ToArray(),
                    BiTangents = bitangents.ToArray(),
                };

            if (MainMesh)
            {
                CreatePhysics();

                this.NeedsRender = false;
            }
            return this.mesh;
        }

        /*public Mesh RenderAtmos()
        {
            return Render(delegate(turf turf) { return turf.Pressure(turf) > 0; }, false);
        }*/

        private int[] GetIndices(int vIndex)
        {
            return new int[6]
            {
                vIndex + 1,
                vIndex + 2,
                vIndex + 3,
                vIndex + 1,
                vIndex + 3,
                vIndex + 0
            };
        }

        private JVector[] GetNormals(ref List<JVector> Vertices)
        {
            int index = Vertices.Count - 4;

            JVector[] normals = new JVector[4];

            normals[0] = JVector.Cross(Vertices[index + 1] - Vertices[index], Vertices[index + 3] - Vertices[index]);
            normals[1] = normals[0]; // JVector.Cross(Vertices[index] - Vertices[index + 1], Vertices[index + 2] - Vertices[index + 1]);
            normals[2] = normals[0]; //JVector.Cross(Vertices[index + 1] - Vertices[index + 2], Vertices[index + 3] - Vertices[index + 2]);
            normals[3] = normals[0]; //JVector.Cross(Vertices[index + 2] - Vertices[index + 3], Vertices[index] - Vertices[index + 3]);
            
            for (int i = 0; i < 4; i++)
                normals[i].Normalize();
            
            return normals;
        }

        private JVector[][] GetTangents(ref List<JVector> Vertices, ref List<JVector2> UVs)
        {
            int index = Vertices.Count - 4;
            int uvindex = UVs.Count - 4;

            var Tangents = new JVector[2][] { new JVector[2], new JVector[2] };
            
            JVector2 deltaUV11 = UVs[index + 1] - UVs[index];
            JVector2 deltaUV12 = UVs[index + 2] - UVs[index];

            JVector deltaPos11 = Vertices[index + 1] - Vertices[index];
            JVector deltaPos12 = Vertices[index + 2] - Vertices[index];
            

            float r1 = 1.0f / (deltaUV11.X * deltaUV12.Y - deltaUV11.Y * deltaUV12.X);
            Tangents[0][0] = (deltaPos11 * deltaUV12.Y   - deltaPos12 * deltaUV11.Y)*r1;
            Tangents[1][0] = (deltaPos12 * deltaUV11.X - deltaPos11 * deltaUV12.X) * r1;
            
            JVector2 deltaUV21 = UVs[index + 2] - UVs[index + 1];
            JVector2 deltaUV22 = UVs[index + 3] - UVs[index + 1];

            JVector deltaPos21 = Vertices[index + 2] - Vertices[index + 1];
            JVector deltaPos22 = Vertices[index + 3] - Vertices[index + 1];
            
            float r2 = 1.0f / (deltaUV21.X * deltaUV22.Y - deltaUV21.Y * deltaUV22.X);
            Tangents[0][1] = (deltaPos21 * deltaUV22.Y - deltaPos22 * deltaUV21.Y) * r2;
            Tangents[1][1] = (deltaPos22 * deltaUV21.X - deltaPos21 * deltaUV22.X) * r2;

            return Tangents;
        }

        private JVector2[] GetUV(uvCoord coords)
        {

            //   2 --- 3
            //   |     |
            //   |     |
            //   0 --- 1


            // 0 X; 1 Y; 2 W; 3 H
            var uvs = new JVector2[4];
            uvs[0] = new JVector2(coords.X, coords.Y);
            uvs[1] = new JVector2(coords.X + coords.Width, coords.Y);
            //uvs[2] = new JVector2(tType.UVCoords.X, tType.UVCoords.Y + tType.UVCoords.Height);

            uvs[2] = new JVector2(coords.X + coords.Width, coords.Y + coords.Height);
            uvs[3] = new JVector2(coords.X, coords.Y + coords.Height);
            //uvs[5] = new JVector2(tType.UVCoords.X, tType.UVCoords.Y + tType.UVCoords.Height);

            /*for (int i = 0; i < 4; i++)
                uvs[i].Normalize();*/

            return uvs;
        }

        private JVector2[] GetCableUV(Block block, Direction direction)
        {
            if (!block.HasDecoy(direction)) return null;

            var Cables = block.GetCables(direction);

            List<JVector2> retList = new List<JVector2>();
            foreach (var Cable in Cables)
                retList.AddRange(GetUV(datums.turf.Cable.GetUV(Cable)));

            return retList.ToArray();
        }

        private void SetDecoys(JVector2[] UVs, List<JVector2> Layer1, List<JVector2> Layer2, List<JVector2> Layer3)
        {
            int count = 0;
            if (UVs != null) count = UVs.Length;


            for (int i = 0; i < count; i += 4)
            {
                switch (i / 4)
                {
                    case 0:
                        Layer1.Add(UVs[i]);
                        Layer1.Add(UVs[i + 1]);
                        Layer1.Add(UVs[i + 2]);
                        Layer1.Add(UVs[i + 3]);
                        break;
                    case 1:
                        Layer2.Add(UVs[i]);
                        Layer2.Add(UVs[i + 1]);
                        Layer2.Add(UVs[i + 2]);
                        Layer2.Add(UVs[i + 3]);
                        break;
                    case 2:
                        Layer3.Add(UVs[i]);
                        Layer3.Add(UVs[i + 1]);
                        Layer3.Add(UVs[i + 2]);
                        Layer3.Add(UVs[i + 3]);
                        break;
                    default:
#if DEBUG
                        throw new Exception("Not enough decoy-layers");
#else
                                        break;
#endif
                }
            }

            int numLayer = count / 4;
            var m1Vec = JVector2.One;
            m1Vec.Negate();

            switch (numLayer)
            {
                case 0:
                    Layer1.Add(m1Vec);
                    Layer1.Add(m1Vec);
                    Layer1.Add(m1Vec);
                    Layer1.Add(m1Vec);

                    Layer2.Add(m1Vec);
                    Layer2.Add(m1Vec);
                    Layer2.Add(m1Vec);
                    Layer2.Add(m1Vec);

                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    break;
                case 1:
                    Layer2.Add(m1Vec);
                    Layer2.Add(m1Vec);
                    Layer2.Add(m1Vec);
                    Layer2.Add(m1Vec);

                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    break;
                case 2:
                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    Layer3.Add(m1Vec);
                    break;
            }
        }

        /// <summary>
        /// Checks for a specific neighbour-block. Does all the security checks.
        /// </summary>
        private bool CheckNeighbour(int origX, int origY, int origZ, JVector direction)
        {
            int movX = (int)direction.X;
            int movY = (int)direction.Y;
            int movZ = (int)direction.Z;

            int targX = movX + origX;
            int targY = movY + origY;
            int targZ = movZ + origZ;

                if (targX >= 0 &&
                    targY >= 0 &&
                    targZ >= 0 &&
                    targX < SizeXYZ &&
                    targY < SizeXYZ &&
                    targZ < SizeXYZ)
                    return !blocks[targX, targY, targZ].IsVisible;
                else
                    if (AssignedStructure != null)
                    {
                        targX += (int)this.Position.X;
                        targY += (int)this.Position.Y;
                        targZ += (int)this.Position.Z;

                        return !this.AssignedStructure[targX, targY, targZ].IsVisible;
                    }
                    else
                        return true; //border and no structure available (failsave)
        }
        private bool CheckVertNeighbour(int origX, int origY, int origZ, JVector direction, bool[,,] Vertices)
        {
            int movX = (int)direction.X;
            int movY = (int)direction.Y;
            int movZ = (int)direction.Z;

            int targX = movX + origX;
            int targY = movY + origY;
            int targZ = movZ + origZ;
            
            if (targX >= 0 &&
                targY >= 0 &&
                targZ >= 0 &&
                targX < SizeXYZ &&
                targY < SizeXYZ &&
                targZ < SizeXYZ)
                return !blocks[targX, targY, targZ].IsVisible;
            else
                if (AssignedStructure != null)
                {
                    targX += (int)this.Position.X;
                    targY += (int)this.Position.Y;
                    targZ += (int)this.Position.Z;

                    return !this.AssignedStructure[targX, targY, targZ].IsVisible;
                }
                else
                    return true; //border and no structure available (failsave)
        }

        /*public static Chunk GenerateTestChunk(Structure)
        {
            var blocks = new turf[Chunk.SizeXYZ, Chunk.SizeXYZ, Chunk.SizeXYZ];

            var heightMap = Tools.heightmap.generate(Chunk.SizeXYZ, Chunk.SizeXYZ);

            var gSize = Chunk.SizeXYZ; //Chunk.SizeXYZ

            for (byte x = 0; x < gSize; x++)
                for (byte z = 0; z < gSize; z++)
                    /*if (heightMap[x, z] != 0)
                        for (int y = 0; y < 1; y++) //heightMap[x, z] / (Chunk.SizeXYZ * 4)*/
                            /*blocks[x, 0, z] = turf.Create(turfTypeE.floor, x, 0, z);

            //blocks[0, 1, 0] = turfTypes.GetNewBlock(turfTypeE.floor);
            var chunk = new Chunk(blocks, JVector.Zero, );
            return chunk;
        }*/

        public bool Disposing { get; set; }
        public void Dispose()
        {
            Disposing = true;
        }

        public override string ToString()
        {
            if (AssignedStructure != null)
                return "Chunk." + Position.ToString() + "." + AssignedStructure.ToString();
            else
                return "Chunk." + Position.ToString();
        }
    }
}
