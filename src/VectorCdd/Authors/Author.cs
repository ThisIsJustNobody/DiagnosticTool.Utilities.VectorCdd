using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.Authors
{
    [XmlType("AUTHOR")]
    public class Author
    {
        [XmlAttribute(DataType = "string", AttributeName = "id")]
        public string? Id { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "obs")]
        public string? Obs { get; set; }

        [XmlElement("LASTNAME")]
        public string? LastName { get; set; }

        [XmlElement("FIRSTNAME")]
        public string? FirstName { get; set; }

        [XmlElement("SHORTNAME")]
        public string? ShortName { get; set; }

        [XmlElement("COMPANY")]
        public string? Company { get; set; }

        [XmlElement("EMAIL")]
        public string? Email { get; set; }
    }
}
