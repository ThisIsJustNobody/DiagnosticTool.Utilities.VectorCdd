using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.AttrCats;

[XmlType("ATTRCAT")]
public class AttrCat : General
{
    [XmlAttribute("usage")]
    public string? Usage { get; set; }

    [XmlAttribute("xauth")]
    public string? AccessAuth { get; set; }
}
