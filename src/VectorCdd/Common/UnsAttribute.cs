using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

[XmlType("UNS")]
public class UnsAttribute
{
    [XmlAttribute("attrref")]
    public string? AttributeRef { get; set; }

    [XmlAttribute("v")]
    public string? Value { get; set; }
}
