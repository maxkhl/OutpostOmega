using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using grendgine_collada;
using OpenTK;
using System.IO;

namespace OutpostOmega.Tools
{
    static class Collada
    {
        public struct Mesh
        {
            public string Name;
            public Drawing.Vertex[] Vertices;
            public Dictionary<Grendgine_Collada_Input_Semantic, uint[]> Indices;
        }

        public static List<Mesh> ReadModel(string File)
        {
            if (!System.IO.File.Exists(File))
                throw new System.IO.FileNotFoundException(string.Format("Could not find model at '{0}'", File));

            var Model = grendgine_collada.Grendgine_Collada.Grendgine_Load_File(File);
            var Meshs = new List<Mesh>();


            foreach (Grendgine_Collada_Geometry geometry in Model.Library_Geometries.Geometry)
            {
                if ((from input in geometry.Mesh.Polylist[0].Input
                     where input.Semantic == Grendgine_Collada_Input_Semantic.TEXCOORD
                     select input.source).SingleOrDefault() == null) // No texture and thats not allowed
                    continue;

                var modelMesh = new Mesh();
                modelMesh.Name = geometry.Name;

                // Read all indices
                #region INDICES
                var pList = geometry.Mesh.Polylist[0];

                var indexData = pList.P.ValueUint();

                modelMesh.Indices = new Dictionary<Grendgine_Collada_Input_Semantic, uint[]>();

                foreach (Grendgine_Collada_Input_Shared input in pList.Input)
                {
                    modelMesh.Indices.Add(input.Semantic, new uint[pList.Count * 3]);
                }

                for (int i = 0; i < indexData.Count(); i += pList.Input.Count())
                {
                    foreach (Grendgine_Collada_Input_Shared input in pList.Input)
                    {
                        modelMesh.Indices[input.Semantic][i / pList.Input.Count()] = indexData[i + input.Offset];
                    }
                }
                #endregion

                // Read all vertices
                #region VERTICES
                var VertexSourceID = (from input in geometry.Mesh.Vertices.Input
                                      where input.Semantic == Grendgine_Collada_Input_Semantic.POSITION
                                      select input.source).SingleOrDefault().Substring(1);

                var VertexPositions = (from source in geometry.Mesh.Source
                                       where source.ID == VertexSourceID
                                       select source.Float_Array).Single();

                var vertArray = VertexPositions.Value();
                #endregion

                // Read all normals
                #region NORMALS
                var NormalSourceID = (from input in geometry.Mesh.Polylist[0].Input
                                      where input.Semantic == Grendgine_Collada_Input_Semantic.NORMAL
                                      select input.source).SingleOrDefault().Substring(1);

                var Normals = (from source in geometry.Mesh.Source
                               where source.ID == NormalSourceID
                               select source.Float_Array).Single();

                var normArray = Normals.Value();
                #endregion

                // Read all texcoords
                #region TEXCOORDS
                var TexCoordSourceID = (from input in geometry.Mesh.Polylist[0].Input
                                        where input.Semantic == Grendgine_Collada_Input_Semantic.TEXCOORD
                                        select input.source).SingleOrDefault().Substring(1);

                var TexCoords = (from source in geometry.Mesh.Source
                                 where source.ID == TexCoordSourceID
                                 select source.Float_Array).Single();

                var TexCoordArray = TexCoords.Value();
                #endregion

                // Repair texcoords because colladashit is using indexed texture coordinates -.-***** - in the future this has to be pre-processed and applied to the files before distribution
                var vert = modelMesh.Indices[Grendgine_Collada_Input_Semantic.VERTEX];
                Tools.Collada.FixTexCoords(ref vertArray, ref vert, modelMesh.Indices[Grendgine_Collada_Input_Semantic.TEXCOORD], ref TexCoordArray);
                Tools.Collada.FixNormalCoords(ref vertArray, ref vert, modelMesh.Indices[Grendgine_Collada_Input_Semantic.NORMAL], ref normArray);
                modelMesh.Indices[Grendgine_Collada_Input_Semantic.VERTEX] = vert;

                // Build VertexArray
                #region VERTARRAY
                modelMesh.Vertices = new Drawing.Vertex[VertexPositions.Count / 3];
                for (int i = 0; i < VertexPositions.Count; i += 3)
                {
                    var newVert = new Drawing.Vertex()
                    {
                        Position = new OpenTK.Vector3(vertArray[i], vertArray[i + 1], vertArray[i + 2]),
                        //Normal = new OpenTK.Vector3(normArray[i], normArray[i + 1], normArray[i + 2]),
                        //TexCoord = new Vector2(TexCoordArray[i / 3 * 2], TexCoordArray[i / 3 * 2 + 1]),
                        //Color = OpenTK.Graphics.Color4.Beige
                    };
                    modelMesh.Vertices[i / 3] = newVert;
                }

                for (int i = 0; i < normArray.Count(); i += 3)
                {
                    modelMesh.Vertices[i / 3].Normal = new OpenTK.Vector3(normArray[i], normArray[i + 1], normArray[i + 2]);
                    //modelMesh.Vertices[i / 3 + 1].Normal = new OpenTK.Vector3(normArray[i], normArray[i + 1], normArray[i + 2]);
                    //modelMesh.Vertices[i / 3 + 2].Normal = new OpenTK.Vector3(normArray[i], normArray[i + 1], normArray[i + 2]);
                }

                for (int i = 0; i < TexCoordArray.Count(); i += 2)
                {
                    modelMesh.Vertices[i / 2].TexCoord1 = new Vector2(TexCoordArray[i], TexCoordArray[i + 1]);
                }
                #endregion


                Meshs.Add(modelMesh);
            }
            return Meshs;
        }

        /// <summary>
        /// Sorts the collada texture coordinates in the vertex-order. Used to avoid using the texcoord index (wich is not possible in opengl :P)
        /// </summary>
        static public void FixTexCoords(ref float[] Vertices, ref uint[] VertexIndices, uint[] TexCoordIndices, ref float[] TexCoords)
        {
            float[] newTexCoordArray = new float[(Vertices.Count() / 3) * 2];

            //Array.Sort(VertexIndices);
            //VertexIndices = VertexIndices.Distinct().ToArray();
            for (int c = 0; c < TexCoordIndices.Count(); c++)
            {
                var U = TexCoords[TexCoordIndices[c] * 2];
                var V = TexCoords[TexCoordIndices[c] * 2 + 1];

                newTexCoordArray[VertexIndices[c] * 2] = U;
                newTexCoordArray[VertexIndices[c] * 2 + 1] = (V - 1) * -1; // Invert V
            }
            TexCoords = newTexCoordArray;


            /*uint[] newIndices = new uint[VertexIndices.Length];
            float[] newVertices = new float[Vertices.Length];
            float[] newUVs = new float[(Vertices.Length / 3) * 2];

            // Create a unique vertex for every index in the original Mesh:
            for (uint i = 0; i < VertexIndices.Length; i++)
            {
                newIndices[i] = i;
                newVertices[i] = Vertices[VertexIndices[i]*3];
                newVertices[i] = Vertices[VertexIndices[i]*3+1];
                newVertices[i] = Vertices[VertexIndices[i]*3+2];
                newUVs[i] = TexCoords[VertexIndices[i]*2];
                newUVs[i] = TexCoords[VertexIndices[i]*2+1];
            }
            Vertices = newVertices;
            VertexIndices = newIndices;*/

            //unsharedVertexMesh.vertices = newVertices;
            //usharedVertexMesh.uv = newUVs;

            //unsharedVertexMesh.SetTriangles(newIndices, 0);


            



            //float[] newTexCoordArray = unifyIndices(VertexCount, VertexIndices.Count()/3, VertexIndices, TexCoordIndices, TexCoords);
        }
        static public void FixNormalCoords(ref float[] Vertices, ref uint[] VertexIndices, uint[] NormalCoordIndices, ref float[] NormalCoords)
        {
            float[] newNormalCoordArray = new float[(Vertices.Count() / 3) * 3];

            for (int c = 0; c < NormalCoordIndices.Count(); c++)
            {
                var X = NormalCoords[NormalCoordIndices[c] * 3];
                var Y = NormalCoords[NormalCoordIndices[c] * 3 + 1];
                var Z = NormalCoords[NormalCoordIndices[c] * 3 + 2];

                newNormalCoordArray[VertexIndices[c] * 3] = X;
                newNormalCoordArray[VertexIndices[c] * 3 + 1] = Y;
                newNormalCoordArray[VertexIndices[c] * 3 + 2] = Z;
            }
            NormalCoords = newNormalCoordArray;
        }

        static public float[] unifyIndices(int totalVertexCount, int totalTrianglesCount, uint[] lesserIndices, uint[] greaterIndices, float[] greaterData)
        {
            //http://collada.org/public_forum/showthread.php/1223-Deindexing-Unifying-Vertex-Normal-Texccord-Indices

            //This function unifies the three indices into a unified index by removing the duplicated indices and data from the greater buffer so as to match the smaller one

            //We create a map for referencing lesserIndices with greaterIndices
            Dictionary<uint, uint> indexMap = new Dictionary<uint, uint>();

            var rearrGreaterData = new float[totalVertexCount*3];

            for(int i=0;i<totalTrianglesCount*3;i++)
            {
                if (!indexMap.ContainsKey(lesserIndices[i]))
                    indexMap.Add(lesserIndices[i], 0);

                if( indexMap[lesserIndices[i]] == 0 )
                {
                    //This is the first time this lesserIndex has been encountered
                    //so add this to the map right away with the corresponding greater index
                    indexMap[ lesserIndices[i] ] = greaterIndices[i];

                    //Lookup the data from greaterData and add it to the hashMap
                    //(Following 3 lines expanded instead of putting in a subloop for clarity)
                    rearrGreaterData[ 3*lesserIndices[i]     ] = greaterData [ greaterIndices[i]*3     ];
                    rearrGreaterData[ 3*lesserIndices[i] + 1 ] = greaterData [ greaterIndices[i]*3 + 1 ];
                    rearrGreaterData[ 3*lesserIndices[i] + 2 ] = greaterData [ greaterIndices[i]*3 + 2 ];
                }

            }

            return rearrGreaterData;
        }
    }
}



 /*
 * Copyright 2006 Sony Computer Entertainment Inc.
 * 
 * Licensed under the SCEA Shared Source License, Version 1.0 (the "License"); you may not use this 
 * file except in compliance with the License. You may obtain a copy of the License at:
 * http://research.scea.com/scea_shared_source_license.html
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License 
 * is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
 * implied. See the License for the specific language governing permissions and limitations under the 
 * License.
 */

/*
#region Using Statements
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using COLLADA;

#endregion

namespace COLLADA
{
    public class Conditioner
    {
        /// <summary>This method will convert convex polygons to triangles
        /// <para>A more advanced condionner would be required to handle convex, complex polygons</para>
        /// </summary>
        static public void ConvexTriangulator(Document doc)
        {
            foreach (Document.Geometry geo in doc.geometries)
            {
                List<Document.Primitive> triangles = new List<Document.Primitive>();
                foreach (Document.Primitive primitive in geo.mesh.primitives)
                {
                    if (primitive is Document.Polylist)
                    {
                        int triangleCount = 0;

                        foreach (int vcount in primitive.vcount) triangleCount += vcount - 2;
                        int[] newP = new int[primitive.stride * triangleCount * 3];
                        int count = 0;
                        int offset = 0;
                        int first = 0;
                        int last = 0;
                        int j, k;

                        foreach (int vcount in primitive.vcount)
                        {
                            first = offset;
                            last = first + 1;
                            for (j = 0; j < vcount - 2; j++)
                            {
                                // copy first
                                for (k = 0; k < primitive.stride; k++)
                                    newP[count++] = primitive.p[k + first * primitive.stride];
                                // copy previous last
                                for (k = 0; k < primitive.stride; k++)
                                    newP[count++] = primitive.p[k + last * primitive.stride];
                                last += 1;
                                // last = new point
                                for (k = 0; k < primitive.stride; k++)
                                    newP[count++] = primitive.p[k + last * primitive.stride];
                            }
                            offset = last + 1;
                        }
                        // Make a triangle out of this Polylist
                        Document.Triangle triangle = new Document.Triangle(doc, count / primitive.stride / 3, primitive.Inputs, newP);
                        triangle.name = primitive.name;
                        triangle.material = primitive.material;
                        triangle.extras = primitive.extras;
                        triangles.Add(triangle);
                    }
                    else if (primitive is Document.Triangle)
                    {
                        triangles.Add(primitive);
                    }
                    else if (primitive is Document.Line)
                    {
                        // remove lines for now...
                    }
                    else
                        throw new Exception("Unsurpoted primitive" + primitive.GetType().ToString() + " in Conditioner::ConvexTriangle");
                }
                geo.mesh.primitives = triangles;
            }
        }


        /// <summary>This method will create a single index for the primitives
        /// This will not work on polygons and polylist, since vcount is not taken in count there
        /// So you need to first run ConvexTriangulator (or equivalent) 
        /// <para>This will make it directly usable as a index vertexArray for drawing</para>
        /// </summary>
        static public void Reindexor(Document doc)
        {
            String[] channelFlags =
            {
                "POSITION",
                "NORMAL",
                "TEXCOORD",
                "COLOR",
                "TANGENT",
                "BINORMAL",
                "UV",
                "TEXBINORMAL",
                "TEXTANGENT"
            };

            Dictionary<string,int> channelCount = new Dictionary<string,int>();
            Dictionary<string, int> maxChannelCount = new Dictionary<string, int>();
            Dictionary<string, List<Document.Input>> inputs = new Dictionary<string, List<Document.Input>>(); ;

            foreach (string i in channelFlags)
                maxChannelCount[i] = 0;

            foreach (Document.Geometry geo in doc.geometries)
            {
                // Check if all parts have the same vertex definition
                bool first=true;
                foreach (Document.Primitive primitive in geo.mesh.primitives)
                {
                    foreach (string i in channelFlags) 
                        channelCount[i] = 0;

                    foreach (Document.Input input in Util.getAllInputs(doc, primitive))
                    {
                        channelCount[input.semantic]++;
                    }
                    if (first)
                    {
                        foreach (string i in channelFlags)
                            if (maxChannelCount[i] < channelCount[i]) maxChannelCount[i] = channelCount[i];
                        first = false;
                    }
                     else
                    {
                        foreach (string i in channelFlags)
                            if (maxChannelCount[i] != channelCount[i])
                                throw new Exception("TODO:  mesh parts have different vertex buffer definition");
                    }
                }

                // create new float array and index
                List<List<int>> indexList = new List<List<int>>();
                List<int> indexes;
                List<float> farray = new List<float>();
                Dictionary<string,int> checkIndex = new Dictionary<string,int>();
                int index=0;

                foreach (Document.Primitive primitive in geo.mesh.primitives)
                {
                    foreach (string i in channelFlags) 
                        inputs[i] = new List<Document.Input>();

                    foreach (Document.Input input in Util.getAllInputs(doc, primitive))
                        inputs[input.semantic].Add(input);

                    indexes = new List<int>();
                    indexList.Add(indexes);

                    int k=0;
                    string indexKey;
                    List<float> tmpValues;
                    try
                    {
                        while (true)
                        {
                            indexKey = "";
                            tmpValues = new List<float>();
                            foreach (string i in channelFlags)
                                foreach (Document.Input input in inputs[i])
                                {
                                    int j = Util.GetPValue(input, primitive, k);
                                    indexKey += j.ToString() + ",";
                                    float[] values = Util.GetSourceElement(doc,input, j);
                                    for (int l = 0; l < values.Length; l++) tmpValues.Add(values[l]);
                                }
                            k++;
                            if (checkIndex.ContainsKey(indexKey))
                                indexes.Add(checkIndex[indexKey]);
                            else
                            {
                                indexes.Add(index);
                                checkIndex[indexKey] = index++;
                                foreach (float f in tmpValues) farray.Add(f);
                            }
                        }
                    }
                    catch { } // catch for index out of range.
                }
                // remove old sources and array
                foreach (Document.Source source in geo.mesh.sources)
                {
                    if (source.array != null)
                        doc.dic.Remove(((Document.Array<float>)source.array).id);
                    doc.dic.Remove(source.id);
                }

                // create all the new source
                int stride = 0;
                foreach (Document.Source source in geo.mesh.sources)
                {
                    stride += source.accessor.stride;
                }

                List<Document.Source> newSources = new List<Document.Source>();
                Document.Source newSource;
                Document.Accessor newAccessor;
                Document.Array<float> newArray;
                int offset = 0;
                string positionId = ((Document.Source)inputs["POSITION"][0].source).id ;
                foreach (Document.Source source in geo.mesh.sources)
                {
                    newAccessor = new Document.Accessor(doc, farray.Count / stride, offset, stride, "#"+geo.id + "-vertexArray", source.accessor.parameters);
                    offset += source.accessor.stride;
                    if (source.id == positionId)
                    {
                        newArray = new Document.Array<float>(doc, geo.id + "-vertexArray", farray.ToArray());
                    }
                    else
                    {
                        newArray = null;
                    }
                    newSource = new Document.Source(doc, source.id, newArray, newAccessor);
                    newSources.Add(newSource);
                }

                // Create the new vertices
                List<Document.Input> newInputs = new List<Document.Input>();
                Document.Input newInput;
                foreach (string i in channelFlags)
                    foreach (Document.Input input in inputs[i])
                    {
                        // no offset, all inputs share the same index
                        newInput = new Document.Input(doc, 0, input.semantic, input.set, ((Document.Source)input.source).id);
                        newInputs.Add(newInput);
                    }
                Document.Vertices newVertices = new Document.Vertices(doc, geo.mesh.vertices.id, newInputs);

                // now create the new primitives
                List<Document.Primitive> newPrimitives = new List<Document.Primitive>();
                Document.Primitive newPrimitive;

                index = 0;
                offset = 0;
                foreach (Document.Primitive primitive in geo.mesh.primitives)
                {

                    newInputs = new List<Document.Input>();
                    newInput = new Document.Input(doc, 0, "VERTEX", -1, geo.mesh.vertices.id);
                    newInputs.Add(newInput);

                    if (primitive is Document.Triangle)
                        newPrimitive = new Document.Triangle(doc, primitive.count, newInputs, indexList[index].ToArray());
                    else if (primitive is Document.Line)
                        newPrimitive = new Document.Line(doc, primitive.count, newInputs, indexList[index].ToArray());
                    else
                        throw new Exception("TODO: need to take care of " + primitive.GetType().ToString());
                    newPrimitive.material = primitive.material;
                    newPrimitive.extras = primitive.extras;
                    newPrimitive.name = primitive.name;
                    newPrimitives.Add(newPrimitive);

                    index++;
                }
                
                // change the primitive to use the new array and indexes.
       
                // 1) - remove the old sources, vertices and primitives

                geo.mesh.sources.Clear();
                geo.mesh.vertices.inputs.Clear();
                geo.mesh.primitives.Clear();

                // 2) - Add all the sources, only the POSITION will have the values

                geo.mesh.sources = newSources;
                geo.mesh.primitives = newPrimitives;
                geo.mesh.vertices = newVertices;

            } // foreach geometry
        } // Reindexor()

    } // Conditionner class
} // namespace COLLADA

 */
