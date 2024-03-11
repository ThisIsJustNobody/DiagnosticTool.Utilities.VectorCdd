using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.DocTmpl;

[XmlType("DOCTMPL")]
public class DocTemplate : General
{
    [XmlAttribute("saveno")]
    public int SaveNumber { get; set; }
}
