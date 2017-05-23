using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using System.Reflection;

namespace OutpostOmega.Data
{
    /// <summary>
    /// This part contains all helper-methods a converter needs. Methods that are here are usualy used multiple times at different locations
    /// </summary>
    public abstract partial class cConverter
    {
        /// <summary>
        /// Turns a float correctly into a string (culture independent)
        /// </summary>
        /// <param name="value">float to convert</param>
        /// <returns>converted float as string</returns>
        public static string FloatToString(float value)
        {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Turns a string correctly into a float (culture independent)
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>converted string as float</returns>
        public static float StringToFloat(string value)
        {
            return float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compresses a byte array
        /// </summary>
        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Decompresses a byte array
        /// </summary>
        public static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// Writes the converterID into the stream. This is used to determine wich converter has to be used while deserialization
        /// </summary>
        protected void WriteCID(XElement element)
        {
            element.Add(new XAttribute(XPropAttrCID, cID));
        }

        /// <summary>
        /// Reads and returns the converterID
        /// </summary>
        protected static Int16 ReadCID(XElement element)
        {
            return Int16.Parse(element.Attributes(XPropAttrCID).Single().Value);
        }

        /// <summary>
        /// Can be used to introduce a specific type of object to its converter
        /// </summary>
        /// <param name="SenderID">ID of the sender</param>
        /// <param name="obj">Object that should be registered</param>
        public static void Register(string SenderID, object obj)
        {
            LoadConverter();

            
            cConverter conv = (from lconv in AllConverter
                              where lconv.CheckCompatibility(obj)
                              select lconv).FirstOrDefault();
            if (conv != null)
            {
                conv.RegisterObject(SenderID, obj);
            }
        }

        /// <summary>
        /// Remove illegal XML characters from a string.
        /// </summary>
        private static string SanitizeString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            s = s.Replace("[", "");
            s = s.Replace("]", "");
            s = s.Replace(",", "");

            StringBuilder buffer = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; i++)
            {
                int code;
                try
                {
                    code = Char.ConvertToUtf32(s, i);
                }
                catch (ArgumentException)
                {
                    continue;
                }
                if (IsLegalXmlChar(code))
                    buffer.Append(Char.ConvertFromUtf32(code));
                if (Char.IsSurrogatePair(s, i))
                    i++;
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Whether a given character is allowed by XML 1.0.
        /// </summary>
        private static bool IsLegalXmlChar(int character)
        {
            return
            (
                character != 0x60

            ) &&
            (
                 character == 0x9 /* == '\t' == 9   */          ||
                 character == 0xA /* == '\n' == 10  */          ||
                 character == 0xD /* == '\r' == 13  */          ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF)
            );
        }

        /// <summary>
        /// Used to save string-type relations to avoid searching through every assembly all the time
        /// </summary>
        private static Dictionary<string, Type> TypeBuffer = new Dictionary<string, Type>();

        /// <summary>
        /// Gets a specific type from a string
        /// </summary>
        /// <param name="FullName">Full name of the type</param>
        /// <returns>Found type. null when not found</returns>
        public static Type GetType(string FullName)
        {
            Type fType = null;

            if (TypeBuffer.ContainsKey(FullName))
                fType = TypeBuffer[FullName];

            fType = Type.GetType(FullName);

            if (fType == null)
            {
                foreach (var assembly in Assemblies)
                {
                    if (assembly != null)
                    {
                        fType = assembly.GetType(FullName);
                        if (fType != null)
                            break;
                    }
                }
            }

            if (fType == null)
            {
                List<System.Reflection.Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

                foreach (var assembly in assemblies)
                {
                    fType = assembly.GetType(FullName, false);
                    if (fType != null)
                        break;
                }
            }

            lock (TypeBuffer)
            {
                if (fType != null && !TypeBuffer.ContainsKey(FullName))
                    TypeBuffer.Add(FullName, fType);
            }

            return fType;
        }
    }
}
