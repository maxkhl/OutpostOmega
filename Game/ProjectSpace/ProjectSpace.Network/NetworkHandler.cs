using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OutpostOmega.Game;
using OutpostOmega.Data;
using System.Xml.Linq;

namespace OutpostOmega.Network
{
    /// <summary>
    /// Used to manage the transfer of the ingame data
    /// </summary>
    public class NetworkHandler
    {
        public World World { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="World">Game World this handler should work with</param>
        public NetworkHandler(World World)
        {
            this.World = World;
        }

        public byte[] GetObjectData(string SenderID, object Object)
        {

            // Serialization
            XElement xml = cConverter.SerializeObject(SenderID, Object, "Object");
            if (xml == null)
                throw new Exception("Unable to serialize object");
            //Converter.UnloadConverter();

            // Compression
            var memoryStream = new MemoryStream();
            DataHandler.XmlToStream(xml, memoryStream, true);

            return memoryStream.GetBuffer();
        }

        public static object ReadSerializedData(byte[] Data)
        {
            // Decompression
            var memoryStream = new MemoryStream(Data);
            var xml = DataHandler.StreamToXml(memoryStream, true);

            // Deserialize
            var obj = DataHandler.XMLtoObject(xml, false);
            if(obj == null)
            {
                obj = DataHandler.XMLtoObject(xml, false);
            }
            return obj;            
        }
    }
}
