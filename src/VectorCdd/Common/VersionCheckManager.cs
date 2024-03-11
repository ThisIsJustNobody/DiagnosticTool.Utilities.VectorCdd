using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

[XmlType("VCKMGR")]
public class VersionCheckManager
{
    [XmlAttribute("vckmode")]
    public string? Mode { get; set; }

    [XmlAttribute("vckmin")]
    public int Min { get; set; }

    [XmlAttribute("vckmax")]
    public int Max { get; set; }

    [XmlAttribute("vcknext")]
    public int Next { get; set; }
}
