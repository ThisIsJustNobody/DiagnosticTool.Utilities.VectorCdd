using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.ECU
{
    [XmlType("DIAGCLASS")]
    public class DiagnosticClass : General
    {
        [XmlElement("DIAGINST")]
        public List<DiagnosticInstance> DiagInstances { get; set; } = [];

        public override void SetCanDelaReference(CanDela? canDela = null)
        {
            base.SetCanDelaReference(canDela);
            DiagInstances.ForEach(x => x.SetCanDelaReference(canDela));
        }
    }
}
