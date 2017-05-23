using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace OutpostOmega.Data
{
	public static partial class DataHandler
	{
        public static UTF8Encoding Encoder = new UTF8Encoding();

        public static void CompressStream(Stream stream)
        {

            MemoryStream mst = new MemoryStream();
            GZipStream gzStream = new GZipStream(mst, CompressionMode.Compress);

            stream.Seek(0, SeekOrigin.Begin);
            
            stream.CopyTo(gzStream);
            
            mst.Seek(0, SeekOrigin.Begin);

            stream.SetLength(0);
            stream.Flush();

            stream.Write(mst.GetBuffer(), 0, (int)mst.Length);
            gzStream.Close();
            mst.Close();
        }

        public static void XmlToStream(XElement xElement, Stream stream, bool Compression = true)
        {
            if (Compression)
            {

                var gZipStream = new GZipStream(stream, CompressionMode.Compress);
                xElement.Save(gZipStream);
                gZipStream.Close();
            }
            else
            {
                xElement.Save(stream);
            }
        }

        public static void StringToStream(Stream stream, string Text)
        {
            stream.Write(DataHandler.Encoder.GetBytes(Text), 0, DataHandler.Encoder.GetByteCount(Text));
        }

        public static XElement StreamToXml(Stream stream, bool Decompression = true)
        {
            Stream lStream = stream;

            if (Decompression)
            {
                try
                {
                    if (lStream.CanSeek)
                        lStream.Seek(0, SeekOrigin.Begin);

                    GZipStream gStream = new GZipStream(lStream, CompressionMode.Decompress);

                    MemoryStream mStream = new MemoryStream();
                    
                    byte[] buffer = new byte[4096];
                    int numRead;
                    while ((numRead = gStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        mStream.Write(buffer, 0, numRead);
                    }
                    gStream.Flush();

                    return DataHandler.LoadXML(mStream);
                }
                catch(Exception e)
                {
                    // Try it without decompression
                    stream.Seek(0, SeekOrigin.Begin);
                    return StreamToXml(stream, false);
                }
            }

            return LoadXML(lStream);
        }

        /// <summary>
        /// Reads an XElement from a stream
        /// </summary>
        public static XElement LoadXML(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);

                bool foundStart = false;
                int XMLStart = 0;
                while (!foundStart)
                {
                    var currByte = stream.ReadByte();
                    if (currByte == 60)
                        foundStart = true;
                    else
                        XMLStart++;
                }
                stream.Seek(XMLStart, SeekOrigin.Begin);
            }

            var byteBuffer = new Byte[stream.Length - (stream.Position - 1) - 1];
            stream.Read(byteBuffer, 0, byteBuffer.Length);

            StringReader stringReader = new StringReader(Encoder.GetString(byteBuffer));


            return XElement.Load(stringReader);
        }

        public static string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec, UriKind.RelativeOrAbsolute);

            if (pathUri.IsAbsoluteUri)
            {
                // Folders must end in a slash
                if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    folder += Path.DirectorySeparatorChar;
                }
                Uri folderUri = new Uri(folder);
                return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
            }
            else
                return filespec;
        }

        internal static List<cConverter.UnidentifiedReference> UnloadedObjects = new List<cConverter.UnidentifiedReference>();
        public static void ProcessUnloadedObjects(string Sender = "")
        {
            while(UnloadedObjects.Count > 0)
            {
                var uObject = UnloadedObjects[0];
                var cObject = cConverter.DeserializeObject(uObject.Sender, uObject.Data, uObject.TargetProperty, uObject.TargetInstance);
                /*if (cObject == null)
                {
                    //UnloadedObjects.RemoveAt(0);
                    //UnloadedObjects.RemoveAt(UnloadedObjects.Count-1);
                    continue;
                }*/

                if (uObject.TargetProperty.PropertyType.IsGenericType)
                {
                    var targetValue = uObject.TargetProperty.GetValue(uObject.TargetInstance);
                    if (targetValue == null) // New list required
                    {
                        List<object> objList = new List<object>();
                        objList.Add(cObject);
                        var objArray = Array.CreateInstance(uObject.TargetProperty.PropertyType.GetProperty("Item").PropertyType, objList.Count);
                        
                        for (int i = 0; i < objList.Count; i++)
                            objArray.SetValue(objList[i], i);

                        var newList = Activator.CreateInstance(uObject.TargetProperty.PropertyType, new object[] { objArray });
                        uObject.TargetProperty.SetValue(uObject.TargetInstance, newList);
                    }
                    else
                    {
                        List<object> objList = new List<object>();
                        foreach(var dObject in ((IEnumerable<dynamic>)targetValue))
                        {
                            objList.Add((object)dObject);
                        }
                        objList.Add(cObject);
                        var objArray = Array.CreateInstance(uObject.TargetProperty.PropertyType.GetProperty("Item").PropertyType, objList.Count);

                        for (int i = 0; i < objList.Count; i++)
                            objArray.SetValue(objList[i], i);

                        var newList = Activator.CreateInstance(uObject.TargetProperty.PropertyType, new object[] { objArray });
                        uObject.TargetProperty.SetValue(uObject.TargetInstance, newList);
                    }
                }
                else
                    uObject.TargetProperty.SetValue(uObject.TargetInstance, cObject);
                UnloadedObjects.Remove(uObject);
            }
            UnloadedObjects.Clear();
            //cConverter.UnloadConverter(Sender);
        }
	}
}

