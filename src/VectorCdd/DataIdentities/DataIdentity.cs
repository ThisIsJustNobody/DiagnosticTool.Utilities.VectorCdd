using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.DataTypes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static DiagnosticTool.Utilities.VectorCdd.DataTypes.BaseDataType;

namespace DiagnosticTool.Utilities.VectorCdd.DataIdentities;

[XmlType("DID")]
public class DataIdentity : General, IConvert
{
    [XmlAttribute(DataType = "unsignedInt", AttributeName = "n")]
    public uint DataIdentityValue { get; set; }
    
    [XmlElement("STRUCTURE", Type = typeof(Packet))]
    public Packet DataType { get; set; } = new();

    public ConvertResult Convert(byte[] rawData)
    {
        var r = DataType.Convert(rawData);
        r.Name = Name ?? DefaultName;
        return r;
    }

    public override void SetCanDelaReference(CanDela? canDela = null)
    {
        base.SetCanDelaReference(canDela);
        DataType.SetCanDelaReference(canDela);
    }
}
