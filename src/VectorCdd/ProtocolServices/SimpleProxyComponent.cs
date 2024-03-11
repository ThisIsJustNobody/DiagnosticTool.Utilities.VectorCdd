using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.ProtocolServices
{
    [XmlType("SIMPLEPROXYCOMP")]
    public class SimpleProxyComponent : BaseComponent
    {
        [XmlAttribute(DataType = "string", AttributeName = "data")]
        public string? Dest { get; set; }

        [XmlAttribute(DataType = "unsignedInt", AttributeName = "minbl")]
        public uint MinBitLength { get; set; }

        [XmlAttribute(DataType = "unsignedInt", AttributeName = "maxbl")]
        public uint MaxBitLength { get; set; }
    }
}
