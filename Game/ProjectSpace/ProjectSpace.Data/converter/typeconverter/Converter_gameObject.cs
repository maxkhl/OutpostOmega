using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Runtime.Serialization;
using OutpostOmega.Game;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Used to convert the gameObject class
    /// </summary>
    public class Converter_gameObject : cConverter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Converter_gameObject()
            : base()
        {
            SupportedTypes.Add(typeof(GameObject));
            cID = (Int16)ConverterID.Converter_gameObject;
            Options = new Converter_Options()
            {
                HandlesProperties = false,
                ConvertSubtypes = true
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

            /*if (First)
            {
                World world = (World)obj;

                XElement data = new XElement(XPropData);
                data.Add(new XElement("Name", world.Name));
                newObject.Add(data);
            }*/

            return newObject;
        }

        public override object Deserialize(string SenderID, XElement element)
        {
            Type type = GetType(element.Element(XPropType).Value);
            GameObject gobj = (GameObject)FormatterServices.GetUninitializedObject(type);
            gobj.ID = element.Attribute(XPropAttrInst).Value; // Important to get the correct id!
            RegisterObject(SenderID, gobj, element.Attribute(XPropAttrInst).Value);
            return gobj;
        }
    }
}
