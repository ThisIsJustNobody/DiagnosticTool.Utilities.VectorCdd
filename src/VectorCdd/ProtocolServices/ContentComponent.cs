using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.ECU;

namespace DiagnosticTool.Utilities.VectorCdd.ProtocolServices
{
    [XmlType("CONTENTCOMP")]
    public class ContentComponent : BaseComponent
    {
        [XmlElement("SIMPLECOMPCONT")]
        public SimpleComponentCont SimpleComponent { get; set; } = new();
    }
}
