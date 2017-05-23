using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using OutpostOmega.Game.datums.UserInterface;

namespace OutpostOmega.Game.Content
{
    /// <summary>
    /// XML UserInterface ContentFile
    /// </summary>
    public class UserInterface : ContentFile
    {
        /// <summary>
        /// The loaded UI Object (null if not loaded or unsuccessful)
        /// </summary>
        public Base UIBase 
        { 
            get
            {
                return _UIBase;
            }
        }
        private Base _UIBase;

        public int ResolutionX { get; set; }
        public int ResolutionY { get; set; }

        public UserInterface(string Path)
            : base(Path)
        {
            ResolutionX = 1000;
            ResolutionY = 1000;
            _UIBase = LoadUI(FileInfo);
        }
        public UserInterface(string Path, ContentManager Manager)
            : base(Path, Manager)
        {
            ResolutionX = 1000;
            ResolutionY = 1000;
            _UIBase = LoadUI(FileInfo);
        }

        public static Base LoadUI(FileInfo fileInfo)
        {

            var fInfo = fileInfo;

            if (!fInfo.Exists)
                throw new FileNotFoundException("XML Interface '" + fInfo.FullName + "' not found");

            var TopElement = StreamToXml(fInfo.OpenRead(), false);

            return ParseBase(TopElement);
        }

        private static Base ParseBase(XElement Data)
        {
            try
            {
                var Type = (BaseType)Enum.Parse(typeof(BaseType), Data.Name.LocalName, true);

                var Base = new Base(Type);

                foreach(var xAttrib in Data.Attributes())
                {
                    try
                    {
                        var AttrType = (AttributeType)Enum.Parse(typeof(AttributeType), xAttrib.Name.LocalName, true);


                        Base[AttrType] = new BaseAttribute()
                        {
                            Type = AttrType,
                            Value = xAttrib.Value,
                        };
                    }
                    catch
                    {
                        throw new Exception("Could not interpret '" + xAttrib.Name.LocalName + "'");
                    }
                }


                foreach (var child in Data.Elements())
                {
                    var newChild = ParseBase(child);
                    if (newChild != null)
                        Base[newChild.Type] = newChild;
                }

                return Base;
            }
            catch
            {
                throw new Exception("Could not interpret '" + Data.Name.LocalName + "'");
            }
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

                    return LoadXML(mStream);
                }
                catch (Exception e)
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

            var byteBuffer = new Byte[stream.Length - (stream.Position - 1) - 1];
            stream.Read(byteBuffer, 0, byteBuffer.Length);

            StringReader stringReader = new StringReader(ContentFile.Encoder.GetString(byteBuffer));


            return XElement.Load(stringReader);
        }
    }
}
