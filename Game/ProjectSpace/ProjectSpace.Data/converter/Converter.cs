using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace OutpostOmega.Data
{
    /// <summary>
    /// Converts objects of a specific type to and from a string
    /// </summary>
    public abstract partial class cConverter
    {
        protected const string XPropName  = "prop";
        protected const string XPropCName = "pclass";
        protected const string XPropType  = "type";
        protected const string XPropData = "data";
        protected const string XPropSub = "sub";

        protected const string XPropAttrCID = "cID";
        protected const string XPropAttrList = "list";
        protected const string XPropAttrInst = "inst";

        protected const string XPropTrue = "true";
        protected const string XPropFalse = "false";


        /// <summary>
        /// Delimiter bytes that are used to mark the end of a object inside a stream
        /// TODO: Pretty sure its too long. Is there a better way?
        /// </summary>
        public static byte[] Delimiter = new byte[10] { 15, 42, 87, 180, 130, 15, 42, 87, 180, 130 };

        public static List<Assembly> Assemblies = new List<Assembly>() { Assembly.GetEntryAssembly() };

        /// <summary>
        /// All loaded converters. Will be filled by the LoadConverter() method
        /// </summary>
        protected static List<cConverter> AllConverter;

        /// <summary>
        /// List of all supported object types. 
        /// This list gets checked by the CheckCompatibility()-method 
        /// (wich can be overridden so this list should be only filled if you plan to use the base method)
        /// </summary>
        protected List<Type> SupportedTypes { get; set; }

        /// <summary>
        /// Options for default converter logic
        /// </summary>
        public converter.Converter_Options Options { get; set; }

        /// <summary>
        /// Converter ID - will be written to the stream for each property to recognize serialization-method while deserializing
        /// </summary>
        protected Int16 cID = (Int16)ConverterID.Default;

        /// <summary>
        /// Has Dispose already been called? 
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public cConverter()
        {
            SupportedTypes = new List<Type>();
            Options = new converter.Converter_Options();
        }

        /// <summary>
        /// Checks the compatibility between a specific object and this converter.
        /// Make sure to ALWAYS call this before de/serialization!
        /// </summary>
        /// <param name="obj">Object that you plan to de/serialize</param>
        /// <param name="type">Type of the object</param>
        /// <returns>false = not compatible</returns>
        public virtual bool CheckCompatibility(object obj)
        {
            if (obj == null)
                return false;

            foreach (Type t in SupportedTypes)
                if (
                    obj.GetType() == t ||
                        ( // Check Subclass when option enabled
                            Options.ConvertSubtypes && 
                            obj.GetType().IsSubclassOf(t)
                        )
                    )
                    return true;

            return false;
        }

        /// <summary>
        /// Checks the compatibility between a specific object and this converter.
        /// Make sure to ALWAYS call this before de/serialization!
        /// </summary>
        /// <param name="element">Element that should be deserialized</param>
        /// <returns>false = not compatible</returns>
        public virtual bool CheckCompatibility(XElement element)
        {
            short cID = ReadCID(element);
            if (cID == this.cID)
                return true;

            Type type = null;
            foreach(var assembly in Assemblies)
            {
                if (assembly != null)
                {
                    type = assembly.GetType(element.Element(XPropType).Value);
                    if (type != null)
                        break;
                }
            }
            if (type == null)
            {
                type = Type.GetType(element.Element(XPropType).Value);
                if(type == null)
                    return false;
            }

            foreach (Type t in SupportedTypes)
                if (
                    type == t ||
                        ( // Check Subclass when option enabled
                            Options.ConvertSubtypes && 
                            type.IsSubclassOf(t)
                        )
                    )
                    return true;

            return false;
        }

        /// <summary>
        /// Loads all available converters into AllConverter. Does nothing when already loaded
        /// </summary>
        private static void LoadConverter()
        {

            if (AllConverter == null)
            {
                AllConverter = new List<cConverter>();

                var allConverterTypes = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                         from lType in lAssembly.GetTypes()
                                         where typeof(cConverter).IsAssignableFrom(lType) && lType != typeof(cConverter)
                                         select lType).ToArray();

                foreach (Type conv in allConverterTypes)
                {
                    AllConverter.Add((cConverter)Activator.CreateInstance(conv));
                }
            }
        }

        /// <summary>
        /// Unloads and disposes all previously loaded converter. Call this when you are done with conversion!
        /// </summary>
        public static void UnloadConverter(string SenderID)
        {
            if (AllConverter != null &&                
                DataHandler.UnloadedObjects.Count == 0)
            {
                foreach (cConverter converter in AllConverter)
                    converter.Dispose(SenderID);

                AllConverter = null;
            }
        }

        /// <summary>
        /// Used to Dispose this converter
        /// </summary>
        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Used to Dispose this converter
        /// </summary>
        public void Dispose(string SenderID)
        {
            Dispose(SenderID, true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Used to Dispose this converter
        /// </summary>
        /// <param name="disposing">First dispose call?</param>
        protected virtual void Dispose(string SenderID = "", bool disposing = true)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (SenderID == "")
                    _objectData.Clear();
                else
                    _objectData[SenderID] = null;
            }
            disposed = true;
        }
    }

   


    /// <summary>
    /// Converter ID's. Used to stamp serialized objects with the converters ID.
    /// With this, the deserialization-program can easily recognize the used converter.
    /// If you dont want to fuck something up, just extend this list and dont touch the existing entries.
    /// </summary>
    public enum ConverterID
    {
        /// <summary>
        /// Default (Null) Converter
        /// </summary>
        Default = 0,
        /// <summary>
        /// .Net Serialization
        /// </summary>
        Converter_NetSerialization = 1,
        /// <summary>
        /// World Converter
        /// </summary>
        Converter_World = 2,
        /// <summary>
        /// GameObject Converter
        /// </summary>
        Converter_gameObject = 3,
        /// <summary>
        /// Unity-Vector Converter
        /// </summary>
        Converter_Vector = 4,
        /// <summary>
        /// Unity-Matrix Converter
        /// </summary>
        Converter_Matrix = 5,
        /// <summary>
        /// Jitter-Vector Converter
        /// </summary>
        Converter_JVector = 6,
        /// <summary>
        /// Jitter-Matrix Converter
        /// </summary>
        Converter_JMatrix = 7,
        /// <summary>
        /// Jitter-Quaternion Converter
        /// </summary>
        Converter_JQuaternion = 8,
        /// <summary>
        /// Unity-Quaternion Converter
        /// </summary>
        Converter_Quaternion = 9,
        /// <summary>
        /// Structure Converter
        /// </summary>
        Converter_Structure = 10,
        /// <summary>
        /// Chunk Converter
        /// </summary>
        Converter_Chunk = 11,
        /// <summary>
        /// Jitter-Material Converter
        /// </summary>
        Converter_JMaterial = 12,
        /// <summary>
        /// Jitter-Shape Converter
        /// </summary>
        Converter_JShape = 13,
        /// <summary>
        /// BoolArray Converter
        /// </summary>
        Converter_BoolArray = 14,
        /// <summary>
        /// Content Converter
        /// </summary>
        Converter_Content = 15,
        /// <summary>
        /// Mesh Converter
        /// </summary>
        Converter_Mesh = 16,
        /// <summary>
        /// FileInfo Converter
        /// </summary>
        Converter_FileInfo = 17,
        /// <summary>
        /// ContentManager Converter
        /// </summary>
        Converter_ContentManager = 18,
        /// <summary>
        /// ContentFile Converter
        /// </summary>
        Converter_ContentFile = 19,   
    }
}
