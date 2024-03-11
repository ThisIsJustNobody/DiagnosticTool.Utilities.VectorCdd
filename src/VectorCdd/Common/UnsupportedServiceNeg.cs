using System.Collections.Generic;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.ProtocolServices;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

[XmlType("UNSUPPSRVNEG")]
public class UnsupportedServiceNeg : General
{
    [XmlElement("CONSTCOMP", typeof(ConstComponent))]
    [XmlElement("STATICCOMP", typeof(StaticComponent))]
    [XmlElement("CONTENTCOMP", typeof(ContentComponent))]
    public List<BaseComponent> Components { get; set; } = [];
}
