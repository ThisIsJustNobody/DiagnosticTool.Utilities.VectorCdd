using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.RecordDataTablePool;

[XmlType("RECORDDT")]
public class RecordDataTable : General
{
    [XmlElement("RECORD")]
    public List<Record> Records { get; set; } = [];
}