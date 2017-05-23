using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using OutpostOmega.Game;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Used to convert the World class
    /// </summary>
    public class Converter_World : cConverter
    {
        public Converter_World()
            : base()
        {
            SupportedTypes.Add(typeof(World));
            cID = (Int16)ConverterID.Converter_World;
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

            if (First)
            {
                World world = (World)obj;

                XElement data = new XElement(XPropData);
                data.Add(new XElement("Name", world.ID));
                newObject.Add(data);
            }

            return newObject;
        }

        // Default should work aswell
        public override object Deserialize(string SenderID, XElement element)
        {
            var obj = new World(element.Attribute(XPropAttrInst).Value);
            //var obj = (World)base.Deserialize(SenderID, element);
            

            //string Name = element.Element(XPropData).Element("Name").Value;
            //object result = new World(Name);
            RegisterObject(SenderID, obj);
            return obj;
        }
    }
}
