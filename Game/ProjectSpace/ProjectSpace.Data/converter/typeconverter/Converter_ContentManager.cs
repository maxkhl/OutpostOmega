using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using OutpostOmega.Game.Content;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Used to convert the World class
    /// </summary>
    public class Converter_ContentManager : cConverter
    {
        public Converter_ContentManager()
            : base()
        {
            SupportedTypes.Add(typeof(ContentManager));
            cID = (Int16)ConverterID.Converter_ContentManager;
            Options = new Converter_Options()
            {
                HandlesProperties = false
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

            return newObject;
        }

        // Default should work aswell
        public override object Deserialize(string SenderID, XElement element)
        {
            ContentManager cmanager = new ContentManager();

            RegisterObject(SenderID, cmanager);

            return cmanager;
        }
    }
}
