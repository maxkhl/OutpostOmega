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
    /// Tries to convert everything that is not covered by other converters
    /// </summary>
    public class Converter_Default : cConverter
    {
        public Converter_Default()
            : base()
        {
            // No cID needed. Default (0) is set in base constructor
            Options = new Converter_Options()
                {
                    HandlesProperties = false
                };
        }

        /// <summary>
        /// Checks if none of the other converters are capable of converting this object
        /// When yes it'll take over
        /// </summary>
        public override bool CheckCompatibility(object obj)
        {
            // Converter are already loaded at this point
            cConverter targetConverter = (from conv in AllConverter
                                          where 
                                            conv.GetType() != this.GetType() && // Exclude this converter
                                            conv.CheckCompatibility(obj)
                                                select conv).FirstOrDefault();



            return targetConverter == null &&
                (from constructor in obj.GetType().GetConstructors()
                 where
                    constructor.GetParameters().Length == 0
                 select
                     true).FirstOrDefault();
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
        public override object Deserialize(string SenderID, XElement element)
        {
            Type type = GetType(element.Element(XPropType).Value);

            var newObject = Activator.CreateInstance(type, new object[0]);

            RegisterObject(SenderID, newObject);
            return newObject;
        }
    }
}
