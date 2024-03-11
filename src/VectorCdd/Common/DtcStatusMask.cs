using System.Collections.Generic;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

[XmlType("DTCSTATUSMASK")]
public class DtcStatusMask
{
    [XmlAttribute("dtref")]
    public string? DataTypeReferenceId { get; set; }

    [XmlElement("DTCSTATUSBITGROUP")]
    public List<DtcStatusBitGroup> BitGroups { get; set; } = [];
}

[XmlType("DTCSTATUSBITGROUP")]
public class DtcStatusBitGroup
{
    [XmlAttribute("conv")]
    public string? Conversion { get; set; }

    [XmlAttribute("dtref")]
    public string? DataTypeReferenceId { get; set; }
}
