using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.IO;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Converts all types that can be serialized through standard .net serialization
    /// </summary>
    public class Converter_NetSerialization : cConverter
    {
        public Converter_NetSerialization()
            : base()
        {
            cID = (Int16)ConverterID.Converter_NetSerialization;
            Options = new Converter_Options()
                {
                    HandlesProperties = true
                };
        }

        public override bool CheckCompatibility(object obj)
        {
            return obj.GetType().IsSerializable;// && !obj.GetType().IsArray;
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

            XElement data = new XElement(XPropData);
            data.Add(ToXElement(obj, obj.GetType()));
            newObject.Add(data);

            return newObject;
        }
        public override object Deserialize(string SenderID, XElement element)
        {
            XElement dataElement = element.Element(XPropData);
            Type type = GetType(element.Element(XPropType).Value);
            if (type == null)
                throw new Exception("XML deserialization: Could not identify object");
            object result = FromXElement(dataElement, type);
            RegisterObject(SenderID, result, element.Attribute(XPropAttrInst).Value);
            return result;
        }

        /// <summary>
        /// Serializes an object to XElement. Only use objects that are supported by the XMLSerializer
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="obj">Object that should be serialized</param>
        /// <returns>Serialized object as XElement</returns>
        protected XElement ToXElement(object obj, Type type)
        {

            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(type);
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }
        }

        /// <summary>
        /// Deserializes an XElement. Only handles types that are deserializable by the XmlSerializer
        /// </summary>
        /// <typeparam name="T">Type of the Element</typeparam>
        /// <param name="element">Serialized XElement</param>
        /// <returns>Deserialized Object</returns>
        protected object FromXElement(XElement element, Type type)
        {
            string xmlData = "";
            if (element.Name == XPropData)
            {
                foreach (XElement elem in element.Elements())
                    xmlData += elem.ToString();
            }
            else
                xmlData = element.ToString();

            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xmlData)))
            {
                var xmlSerializer = new XmlSerializer(type);
                return xmlSerializer.Deserialize(memoryStream);
            }
        }
    }
}
