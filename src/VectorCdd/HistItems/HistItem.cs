using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.HistItems;

[XmlType("HISTITEM")]
public class HistItem
{
    [XmlAttribute("authorref")]
    public string? AuthorRef { get; set; }

    [XmlAttribute("stid")]
    public int StId { get; set; }

    [XmlAttribute("tool")]
    public string? Tool { get; set; }

    [XmlAttribute("dt")]
    public string? DateTime { get; set; }

    [XmlElement("LABEL")]
    public string? Label { get; set; }

    [XmlElement("MOD")]
    public string? Modification { get; set; }

    [XmlElement("REASON")]
    public string? Reason { get; set; }
}
