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
    [XmlType("SHSTATIC")]
    public class ShStatic : General
    {
        [XmlType("STATICCOMPREF")]
        public class StaticCompRef : General
        {
            [XmlAttribute(DataType = "string", AttributeName = "idref")]
            public string? IdRef { get; set; }

            public StaticComponent? StaticComponent =>
                CanDela
                    ?.EcuDocument.ProtocolServices.ProtocolServices.SelectMany(p =>
                        p.Request.Components.Concat(p.PositiveResponse.Components)
                            .Concat(p.NegativeResponse.Components)
                    )
                    .Where(c => c is StaticComponent)
                    .Cast<StaticComponent>()
                    .FirstOrDefault(x => x.Id == IdRef);
        }

        [XmlElement("STATICCOMPREF")]
        public List<StaticCompRef> StaticCompRefs { get; set; } = [];

        public override void SetCanDelaReference(CanDela? canDela = null)
        {
            base.SetCanDelaReference(canDela);
            StaticCompRefs.ForEach(x => x.SetCanDelaReference(canDela));
        }
    }
}
