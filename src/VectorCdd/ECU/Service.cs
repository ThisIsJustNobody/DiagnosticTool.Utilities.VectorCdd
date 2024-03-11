using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.DiagnosticClassTemplates;
using DiagnosticTool.Utilities.VectorCdd.ProtocolServices;

namespace DiagnosticTool.Utilities.VectorCdd.ECU
{
    [XmlType("SERVICE")]
    public class Service : General
    {
        [XmlAttribute("mayBeExec")]
        public string? MayBeExec { get; set; }

        [XmlAttribute("trans")]
        public string? Trans { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "tmplref")]
        public string? TemplateRef { get; set; }

        public DiagnosticClassTemplate? DiagnosticClassTemplate
        {
            get
            {
                if (TemplateRef != null && CanDela?.EcuDocument.DiagnosticClassTemplates.DiagnosticClassTemplates.FirstOrDefault(dct => dct.DiagnosticClassServerTemplate.FirstOrDefault(dcst => dcst.Id == TemplateRef) != null) is { } diagnosticClassTemplate)
                {
                    return diagnosticClassTemplate;
                }
                return null;
            }
            set => TemplateRef = value?.Id;
        }

        public DiagnosticClassServerTemplate? DiagnosticClassServerTemplate
        {
            get
            {
                if (TemplateRef != null && CanDela?.EcuDocument.DiagnosticClassTemplates.DiagnosticClassTemplates.SelectMany(t => t.DiagnosticClassServerTemplate).FirstOrDefault(x => x.Id == TemplateRef) is { } diagnosticClassServerTemplate)
                {
                    return diagnosticClassServerTemplate;
                }
                return null;
            }
            set => TemplateRef = value?.Id;
        }

        public ProtocolService? ProtocolService
        {
            get
            {
                if (TemplateRef != null && DiagnosticClassServerTemplate?.ProtocolService is { } protocolService)
                {
                    return protocolService;
                }
                return null;
            }
            set => TemplateRef = value?.Id;
        }
    }
}
