using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.IO;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Used to convert the gameObject class
    /// </summary>
    public class Converter_FileInfo : cConverter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Converter_FileInfo()
            : base()
        {
            SupportedTypes.Add(typeof(FileInfo));
            cID = (Int16)ConverterID.Converter_FileInfo;
            Options = new Converter_Options()
            {
                HandlesProperties = true
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

            string RelativePath = DataHandler.GetRelativePath(((FileInfo)obj).FullName, Environment.CurrentDirectory);
            newObject.Add(new XElement("path", RelativePath));

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
            var newFileInfo = new FileInfo(element.Element("path").Value);

            RegisterObject(SenderID, newFileInfo);

            return newFileInfo;
        }
    }
}
