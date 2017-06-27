using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using OutpostOmega.Game.Turf;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Used to convert the World class
    /// </summary>
    public class Converter_Structure : cConverter
    {
        public Converter_Structure()
            : base()
        {
            SupportedTypes.Add(typeof(Structure));
            cID = (Int16)ConverterID.Converter_Structure;
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

        public override string GetID(object obj)
        {
            var structure = (Structure)obj;
            if (structure != null && structure.ID != null)
            {
                return structure.ID;
            }
            else
                return this.ID; 
        }

        //Thats hacky but idc
        string ID { get; set; }

        // Default should work aswell
        public override object Deserialize(string senderID, XElement element)
        {
            ID = element.Attribute(XPropAttrInst).Value;
            var structure = (Structure)base.Deserialize(senderID, element);
            ID = "";

            return structure;
        }
    }
}
