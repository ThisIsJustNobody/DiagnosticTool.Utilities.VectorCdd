using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.DefineAttributes;

public class BaseAttributes
{
    [XmlElement("ENUMDEF", Type = typeof(EnumDefine))]
    [XmlElement("UNSDEF", Type = typeof(UnsignedDefine))]
    [XmlElement("CSTRDEF", Type = typeof(CStrDefine))]
    [XmlElement("STRDEF", Type = typeof(StrDefine))]
    public List<BaseDefine> Defines { get; set; } = [];
}
