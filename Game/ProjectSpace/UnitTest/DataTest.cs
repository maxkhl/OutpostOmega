using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OutpostOmega.Game;
using Jitter.LinearMath;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class DataTest
    {
        public World newWorld;

        [TestMethod]
        [TestCategory("World&Serialization")]
        public void NewWorldTest()
        {
            var rnd = new Random();

            newWorld = new World("Testworld");
            newWorld.Structures.Add(new OutpostOmega.Game.turf.Structure(newWorld, "TestStructure"));
            var Turfs = Enum.GetNames(typeof(OutpostOmega.Game.turf.types.turfTypeE));

            var number = rnd.Next(10, 100);

            for (int i = 0; i < number; i++)
            {
                var turf = Turfs[rnd.Next(0, Turfs.Length - 1)];
                newWorld.Structures[0].Add(
                    (OutpostOmega.Game.turf.types.turfTypeE)Enum.Parse(typeof(OutpostOmega.Game.turf.types.turfTypeE), turf),
                    new JVector(
                        ((float)rnd.Next(1, 1000)) / 10,
                        ((float)rnd.Next(1, 1000)) / 10,
                        ((float)rnd.Next(1, 1000)) / 10), true);
            }

            
            List<Assembly> assemblies = new List<Assembly>();
            var assemblyName = Assembly.GetExecutingAssembly().GetReferencedAssemblies().SingleOrDefault(t => t.Name == "OutpostOmega.Game");
            assemblies.Add(Assembly.Load(assemblyName));

            if (OutpostOmega.Game.GameObject.AddonAssembly != null)
                assemblies.Add(OutpostOmega.Game.GameObject.AddonAssembly);

            var types = new List<Type>();
            foreach(Assembly GameAssembly in assemblies)
                types.AddRange(GameAssembly.GetTypes().Where(t => typeof(GameObject).IsAssignableFrom(t)));
            
            foreach(var goType in types)
            {
                if(!goType.IsAbstract)
                {
                    var newGO = GameObject.GenerateNew(goType, newWorld);
                    if (newGO == null)
                        continue;
                    newGO.SetPosition(new JVector(
                        ((float)rnd.Next(1, 1000)) / 10,
                        ((float)rnd.Next(1, 1000)) / 10,
                        ((float)rnd.Next(1, 1000)) / 10));
                    newGO.Register();
                }
            }
        }

        [TestMethod]
        [TestCategory("World&Serialization")]
        public void SimulateTest()
        {
            if (newWorld == null)
                NewWorldTest();

            for (int i = 0; i < 100; i++)
                newWorld.Update(
                    new OutpostOmega.Game.Tools.KeybeardState(),
                    new OutpostOmega.Game.Tools.MouseState(),
                    0.1f);
        }

        MemoryStream SerializedWorld;
        long SerializedWorldLength;

        [TestMethod]
        [TestCategory("World&Serialization")]
        public void SerializeTest()
        {
            if (newWorld == null)
                SimulateTest();

            SerializedWorld = new MemoryStream();
            if (!OutpostOmega.Data.DataHandler.SaveToStream(newWorld, "UnitTest", SerializedWorld))
                throw new Exception("Serialization failed!");

            SerializedWorldLength = SerializedWorld.Length;
        }

        World DeserializedWorld;
        [TestMethod]
        [TestCategory("World&Serialization")]
        public void DeserializeTest()
        {
            if (SerializedWorld == null)
                SerializeTest();

            List<Assembly> assemblies = new List<Assembly>();
            var assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var assemblyName in assemblyNames)
                assemblies.Add(Assembly.Load(assemblyName));

            OutpostOmega.Data.cConverter.Assemblies = assemblies;
            DeserializedWorld = OutpostOmega.Data.DataHandler.LoadWorldFromStream(SerializedWorld, false);
            SerializedWorld = null;
        }

        [TestMethod]
        [TestCategory("World&Serialization")]
        public void ConsistencyTest()
        {
            if (DeserializedWorld == null)
                DeserializeTest();

            CompareWorlds(newWorld, DeserializedWorld);
        }

        public void CompareWorlds(World world1, World world2)
        {
            for (int i = 0; i < world2.AllGameObjects.Count; i++)
            {
                var OriginalGameObject = world1.AllGameObjects[i];
                var SerializedGameObject = world2.AllGameObjects[i];

                bool Similar = true;

                string Detail = "";
                if (OriginalGameObject.ToString() != SerializedGameObject.ToString())
                {
                    Similar = false;
                }
                else
                {
                    var properties = OriginalGameObject.GetType().GetProperties();
                    foreach (var property in properties)
                    {
                        if (!property.CanWrite) continue; // unwriteable properties cant be de-/serialized

                        object original = property.GetValue(OriginalGameObject),
                            serialized = property.GetValue(SerializedGameObject);

                        if (original == null && serialized == null)
                            continue;
                        else if ((original == null) != (serialized == null))
                        {
                            Detail = String.Format("Property '{0}' one property is filled, the other one is empty", property.Name);
                            Similar = false;
                        }
                        else if (original.ToString() != serialized.ToString())
                        {
                            Detail = String.Format("Property '{0}' does not match '{1}' it should be {2}", property.Name, serialized.ToString(), original.ToString());
                            Similar = false;
                        }
                    }
                }

                if (!Similar)
                    throw new Exception(string.Format("Difference between {0} and {1} detected. Details: {2}", OriginalGameObject.ToString(), SerializedGameObject.ToString(), Detail));
            }

            for (int i = 0; i < world2.Structures.Count; i++)
            {
                for (int c = 0; c < world2.Structures[i].chunks.Count; c++)
                {
                    bool Similar = true;
                    var OriginalChunk = world1.Structures[i].chunks[c];
                    var SerializedChunk = world2.Structures[i].chunks[c];
                    string Detail = "";

                    for (int x = 0; x < OutpostOmega.Game.turf.Chunk.SizeXYZ; x++)
                        for (int y = 0; y < OutpostOmega.Game.turf.Chunk.SizeXYZ; y++)
                            for (int z = 0; z < OutpostOmega.Game.turf.Chunk.SizeXYZ; z++)
                            {
                                var OriginalBlock = OriginalChunk[x, y, z];
                                var SerializedBlock = OriginalChunk[x, y, z];

                                var properties = OriginalBlock.GetType().GetProperties();
                                foreach (var property in properties)
                                {
                                    if (!property.CanWrite) continue; // unwriteable properties cant be de-/serialized

                                    object original = property.GetValue(OriginalBlock),
                                         serialized = property.GetValue(SerializedBlock);

                                    if (original == null && serialized == null)
                                        continue;
                                    else if ((original == null) != (serialized == null))
                                    {
                                        Detail = String.Format("Property '{0}' one property is filled, the other one is empty", property.Name);
                                        Similar = false;
                                    }
                                    else if (original.ToString() != serialized.ToString())
                                    {
                                        Detail = String.Format("Property '{0}' does not match '{1}' it should be {2}", property.Name, serialized.ToString(), original.ToString());
                                        Similar = false;
                                    }
                                }
                            }


                    if (!Similar)
                        throw new Exception(string.Format("Difference between {0} and {1} detected. Details: {2}", OriginalChunk.ToString(), SerializedChunk.ToString(), Detail));
                }
            }
        }


        /* I'll remove this test for now because it simply cannot work this way.
         * After every saving the instance ID's that .net provides are different. So the amount of bytes is ALWAYS different.
         * We need a check like this but it needs to compare every single line and take the instance ID's into consideration.
         * Update: Check done see method above
         */
        /*
        MemoryStream SecondSerializedWorld;
        [TestMethod]
        [TestCategory("World&Serialization")]
        public void ByteleakCheck()
        {
            if (DeserializedWorld == null)
                DeserializeTest();

            SecondSerializedWorld = new MemoryStream();
            if (!OutpostOmega.Data.DataHandler.SaveToStream(DeserializedWorld, "UnitTest", SecondSerializedWorld))
                throw new Exception("Serialization failed!");

            if (SecondSerializedWorld.Length != SerializedWorldLength)
                throw new Exception(string.Format("Byteleak detected. Original serialization is {0} bytes long. Second serialization is {1} bytes long. ({2})", SerializedWorldLength, SecondSerializedWorld.Length, SecondSerializedWorld.Length - SerializedWorldLength));

            SecondSerializedWorld.Dispose();
        }*/

        [TestMethod]
        [TestCategory("World&Serialization")]
        public void SimulateAfterDeserializeTest()
        {
            if (DeserializedWorld == null)
                ConsistencyTest();

            for (int i = 0; i < 100; i++)
                DeserializedWorld.Update(
                    new OutpostOmega.Game.Tools.KeybeardState(),
                    new OutpostOmega.Game.Tools.MouseState(),
                    0.1f);
        }

        [TestMethod]
        [TestCategory("World&Serialization")]
        public void MultiDeserializeTest()
        {
            World firstWorld = null;
            World lastWorld = null;
            for(int i = 0; i < 10; i++)
            {
                var oldLength = SerializedWorldLength;
                SerializeTest();

                if (firstWorld == null)
                    firstWorld = newWorld;

                DeserializeTest();

                newWorld = DeserializedWorld;
                lastWorld = DeserializedWorld;
                DeserializedWorld = null;
            }
            CompareWorlds(firstWorld, lastWorld);
        }
    }
}
