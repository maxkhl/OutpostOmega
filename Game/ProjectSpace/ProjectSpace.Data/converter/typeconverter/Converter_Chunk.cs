using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using OutpostOmega.Game.Turf;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Used to convert the World class
    /// </summary>
    public class Converter_Chunk : cConverter
    {
        public Converter_Chunk()
            : base()
        {
            SupportedTypes.Add(typeof(Chunk));
            cID = (Int16)ConverterID.Converter_Chunk;
            Options = new Converter_Options()
            {
                HandlesProperties = true
            };
        }


        /// <summary>
        /// Serializes the object
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>XML Structure</returns>
        public override XElement Serialize(string SenderID, object obj)
        {
            XElement newObject = base.Serialize(SenderID, obj);

            bool First = false;
            string ID = CheckObject(SenderID, obj, out First);
            newObject.Add(new XAttribute(XPropAttrInst, ID));

            var chunk = (Chunk)obj;
            
            byte[] chunkData = new byte[(int)Math.Pow(Chunk.SizeXYZ, 3)];

            for(int x = 0; x < Chunk.SizeXYZ; x++)
                for(int y = 0; y < Chunk.SizeXYZ; y++)
                    for(int z = 0; z < Chunk.SizeXYZ; z++)
                        chunkData[x + Chunk.SizeXYZ * (y + Chunk.SizeXYZ * z)] = chunk.blocks[x,y,z].type;

            newObject.Add(new XElement("chunkData", Convert.ToBase64String(Compress(chunkData))));
            newObject.Add(new XAttribute("X", FloatToString(chunk.Position.X)));
            newObject.Add(new XAttribute("Y", FloatToString(chunk.Position.Y)));
            newObject.Add(new XAttribute("Z", FloatToString(chunk.Position.Z)));

            return newObject;
        }

        // Default should work aswell
        public override object Deserialize(string SenderID, XElement element)
        {
            var chunkData = Decompress(Convert.FromBase64String(element.Element("chunkData").Value));

            var chunkBlocks = new OutpostOmega.Game.Turf.Block[Chunk.SizeXYZ, Chunk.SizeXYZ, Chunk.SizeXYZ];
            for (byte x = 0; x < Chunk.SizeXYZ; x++)
                for (byte y = 0; y < Chunk.SizeXYZ; y++)
                    for (byte z = 0; z < Chunk.SizeXYZ; z++)
                        chunkBlocks[x, y, z] = OutpostOmega.Game.Turf.Block.Create((OutpostOmega.Game.Turf.Types.TurfTypeE)Enum.Parse(typeof(OutpostOmega.Game.Turf.Types.TurfTypeE), chunkData[x + Chunk.SizeXYZ * (y + Chunk.SizeXYZ * z)].ToString()), x, y, z);

            

            var chunk = new Chunk(
                chunkBlocks, 
                new Jitter.LinearMath.JVector(
                    StringToFloat(element.Attribute("X").Value),
                    StringToFloat(element.Attribute("Y").Value),
                    StringToFloat(element.Attribute("Z").Value)
                    ));

            RegisterObject(SenderID, chunk);
            //chunk.Render();
            return chunk;
        }
    }
}
