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
    /// Extends functionality of some classes (mainly with de-/serialization logic)
    /// </summary>
	public static partial class DataHandler
    {
        #region Stream
        /// <summary>
        /// Extension method for stream
        /// </summary>
        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
        #endregion

        #region XElement
        /// <summary>
        /// Saves a XElement to a stream
        /// </summary>
        public static void Save(this XElement baseElement, Stream stream)
        {
            StringWriter stringWriter = new StringWriter();
            baseElement.Save(stringWriter);
            DataHandler.StringToStream(stream, stringWriter.ToString());
        }
        #endregion

        #region GameObject

        /// <summary>
        /// Serializes this gameObject (including all convertable properties) into a stream. Use the static LoadFromStream-method to load a object from a stream
        /// </summary>
        /// <returns>Serialized gameObject</returns>
        static XElement SaveXML(this GameObject gObject, string SenderID)
        {
            return cConverter.SerializeObject(SenderID, gObject, "GameWorld");
        }

        #endregion

        #region World

        /// <summary>
        /// Serializes the World Object and returns a xml structure out of it
        /// </summary>
        /// <param name="world">World object</param>
        /// <returns>XML structure</returns>
        public static XElement GetXML(this World world, string SenderID)
        {
            XElement worldObj = cConverter.SerializeObject(SenderID, world, "GameWorld");
            cConverter.UnloadConverter(SenderID);
            return worldObj;
        }

        /// <summary>
        /// Saves this World to a specific file
        /// </summary>
        /// <param name="world">World object</param>
        /// <param name="target">Targetfile</param>
        /// <param name="Compression">Use Compression?</param>
        /// <returns>Fileinfo of created file</returns>
        public static FileInfo SaveToFile(this World world, FileInfo target, bool Compression = true)
        {
            using( FileStream fstream = target.Create() )
            {
                DataHandler.XmlToStream(world.GetXML(DataHandler.ConverterFileID), fstream, Compression);

                fstream.Close();
                fstream.Dispose();
                return new FileInfo(fstream.Name);
            }
        }

        /// <summary>
        /// Saves this World to a stream
        /// </summary>
        /// <param name="world">World object</param>
        /// <param name="output">Targetstream</param>
        /// <returns>True = everything went fine</returns>
        public static bool SaveToStream(this World world, string SenderID, Stream output)
        {
            XElement worldElem = world.GetXML(SenderID);

            worldElem.Save(output);

            output.Flush();
            return true;
        }
        #endregion
	}
}

