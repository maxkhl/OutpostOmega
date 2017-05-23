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
    public class Converter_ContentFile : cConverter
    {
        public Converter_ContentFile()
            : base()
        {
            SupportedTypes.Add(typeof(OutpostOmega.Game.Content.Model));
            SupportedTypes.Add(typeof(OutpostOmega.Game.Content.Sound));
            SupportedTypes.Add(typeof(OutpostOmega.Game.Content.Texture));
            SupportedTypes.Add(typeof(OutpostOmega.Game.Content.Video));
            cID = (Int16)ConverterID.Converter_ContentFile;
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

            var cFile = (ContentFile)obj;

            newObject.Add(new XAttribute("path", DataHandler.GetRelativePath(cFile.Path, Environment.CurrentDirectory)));

            return newObject;
        }

        // Default should work aswell
        public override object Deserialize(string SenderID, XElement element)
        {
            var path = element.Attribute("path").Value;
            Type type = GetType(element.Element(XPropType).Value);

            ContentFile retFile = null;

            if (type == typeof(Model))
                retFile = new Model(path);
            else if (type == typeof(Texture))
                retFile = new Texture(path);
            else if (type == typeof(Video))
                retFile = new Video(path);
            else if (type == typeof(Sound))
                retFile = new Sound(path);
            else
                retFile = null;


            RegisterObject(SenderID, retFile, element.Attribute(XPropAttrInst).Value);

            return retFile;
        }
    }
}
