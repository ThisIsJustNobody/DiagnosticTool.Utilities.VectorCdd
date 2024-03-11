using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.DiagnosticClassTemplates
{
    [XmlType("DCLTMPL")]
    public class DiagnosticClassTemplate : General
    {
        [XmlAttribute(DataType = "string", AttributeName = "cls")]
        public string? Class { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "single")]
        public string? Single { get; set; }


        [XmlElement("DCLSRVTMPL")]
        public List<DiagnosticClassServerTemplate> DiagnosticClassServerTemplate { get; set; } = [];

        [XmlElement("SHSTATIC")]
        public ShStatic ShStatic { get; set; } = new();

        public override void SetCanDelaReference(CanDela? canDela = null)
        {
            base.SetCanDelaReference(canDela);
            DiagnosticClassServerTemplate.ForEach(x => x.SetCanDelaReference(canDela));
            ShStatic.SetCanDelaReference(canDela);
        }
    }
}
