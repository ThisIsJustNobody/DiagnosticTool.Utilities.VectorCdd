using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.ECU
{
    [XmlType("VAR")]
    public class Variant : General
    {
        [XmlElement("DIAGCLASS", typeof(DiagnosticClass))]
        [XmlElement("DIAGINST", typeof(DiagnosticInstance))]
        public List<General> DiagnosticObjects { get; set; } = [];

        public override void SetCanDelaReference(CanDela? canDela = null)
        {
            base.SetCanDelaReference(canDela);
            DiagnosticObjects.ForEach(x => x.SetCanDelaReference(canDela));
        }
    }
}
