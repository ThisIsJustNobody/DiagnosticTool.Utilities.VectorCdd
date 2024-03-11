using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

[XmlRoot("CANDELA")]
public class CanDela : CanDelaBase
{
    [XmlAttribute(DataType = "string", AttributeName = "dtdvers")]
    public string Version { get; set; } = "";

    [XmlElement("ECUDOC")]
    public EcuDocument EcuDocument { get; set; } = new();

    public override void SetCanDelaReference(CanDela? canDela = null)
    {
        canDela ??= this;
        base.SetCanDelaReference(canDela);
        EcuDocument.SetCanDelaReference(canDela);
    }
}
