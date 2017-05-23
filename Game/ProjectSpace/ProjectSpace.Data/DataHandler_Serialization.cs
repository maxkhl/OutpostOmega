using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Security.Cryptography;
using OutpostOmega.Game;

namespace OutpostOmega.Data
{
    /// <summary>
    /// Contains main-serialization logic
    /// </summary>
	public static partial class DataHandler
	{
        public const string ConverterFileID = "FileSaveLoad";

        /// <summary>
        /// Loads a world from a xml file
        /// </summary>
        /// <param name="file">File to load from</param>
        /// <param name="Decompression">Is file compressed?</param>
        /// <param name="Encryption">Is file encrypted?</param>
        /// <returns>Deserialized world object</returns>
        public static World LoadWorldFromFile(FileInfo file, bool Decompression = true)
        {
            if (file.Exists)
            {
                World loadedWorld = null;

                using(var fileRead = file.OpenRead())
                {
                    loadedWorld = (World)cConverter.DeserializeObject(
                                            ConverterFileID,
                                            DataHandler.StreamToXml(fileRead, Decompression)
                                        );
                }

                cConverter.UnloadConverter(ConverterFileID);
                return loadedWorld;
            }
            else
                return null;
        }

        /// <summary>
        /// Deserializes a world from an xml stream
        /// </summary>
        /// <param name="input">XML Stream with serialized data</param>
        /// <param name="Compressed">Is stream compressed?</param>
        /// <returns>Deserialized world object</returns>
        public static World LoadWorldFromStream(Stream input, bool Compressed = true)
        {
            XElement element = null;
            if (Compressed)
                element = DataHandler.LoadXML(new GZipStream(input, CompressionMode.Decompress));
            else
                element = DataHandler.LoadXML(input);
            input.Close();

            World newWorld = (World)cConverter.DeserializeObject(ConverterFileID, element);
            cConverter.UnloadConverter(ConverterFileID);

            return newWorld;
        }

        /// <summary>
        /// Tries to deserialize a xml to a object
        /// </summary>
        /// <param name="xml">Xml with serialized object data</param>
        /// <returns>Deserialized object</returns>
        public static object XMLtoObject(XElement xml, bool Unload = true)
        {
            var obj = cConverter.DeserializeObject(ConverterFileID, xml);

            if (Unload)
                cConverter.UnloadConverter(ConverterFileID);

            return obj;
        }
	}
}

