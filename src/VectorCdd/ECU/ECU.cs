using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.ECU;

[XmlType("ECU")]
public class ECU : General
{
    [XmlElement("VAR")]
    public List<Variant> Variants { get; set; } = [];

    public override void SetCanDelaReference(CanDela? canDela = null)
    {
        base.SetCanDelaReference(canDela);
        Variants.ForEach(x => x.SetCanDelaReference(canDela));
    }
}
