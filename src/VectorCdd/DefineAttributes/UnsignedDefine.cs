using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.DefineAttributes;

[XmlType("UNSDEF")]
public class UnsignedDefine : BaseDefine
{
    [XmlAttribute("v")]
    public string? DefaultValue { get; set; }

    [XmlAttribute("df")]
    public string? Format { get; set; }
}
