using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

[XmlType("NEGRESCODE")]
public class NegResCode : General
{
    [XmlAttribute("v")]
    public int Value { get; set; }
}
