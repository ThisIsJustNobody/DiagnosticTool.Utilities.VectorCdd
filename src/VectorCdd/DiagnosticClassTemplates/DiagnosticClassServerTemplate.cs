using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.ProtocolServices;

namespace DiagnosticTool.Utilities.VectorCdd.DiagnosticClassTemplates
{
    [XmlType("DCLSRVTMPL")]
    public class DiagnosticClassServerTemplate : General
    {
        [XmlAttribute(DataType = "string", AttributeName = "tmplref")]
        public string? TemplateRef { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "conv")]
        public string? Conv { get; set; }

        public ProtocolService? ProtocolService
        {
            get
            {
                if (TemplateRef != null && CanDela?.EcuDocument.ProtocolServices.ProtocolServices.FirstOrDefault(x => x.Id == TemplateRef) is { } protocolService)
                {
                    return protocolService;
                }
                return null;
            }
            set => TemplateRef = value?.Id;
        }
    }
}
