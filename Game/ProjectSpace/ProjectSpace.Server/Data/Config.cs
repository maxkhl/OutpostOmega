using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace OutpostOmega.Server.Data
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class Config
    {
        [XmlElement(ElementName = "accounts")]
        public Account[] Accounts;

        public void Save(string Location)
        {
            var fileinfo = new FileInfo("Server.xml");

            var serializer = new XmlSerializer(this.GetType());
            using(var stream = fileinfo.OpenWrite())
            {
                serializer.Serialize(stream, this);
            }
        }

        public static Config Load(string Location)
        {
            var fileinfo = new FileInfo("Server.xml");

            if (!fileinfo.Exists)
                return new Config();

            var serializer = new XmlSerializer(typeof(Config));

            Config result = null;
            try
            {
                using (var stream = fileinfo.OpenRead())
                {
                    result = (Config)serializer.Deserialize(stream);
                }
            }
            catch { }
            if (result != null)
                throw new Exception("Could not read config file!");

            return result;
        }
    }
}
