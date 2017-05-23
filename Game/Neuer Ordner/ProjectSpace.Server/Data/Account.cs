using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OutpostOmega.Server.Data
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class Account
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("guid")]
        public string GUID { get; set; }

        [XmlAttribute("email")]
        public string Email { get; set; }

        [XmlAttribute("group")]
        public OutpostOmega.Game.datums.UserGroup Group { get; set; }
    }
}
