using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.DataIdentities;
using DiagnosticTool.Utilities.VectorCdd.DataTypes;

namespace DiagnosticTool.Utilities.VectorCdd.ECU
{
    [XmlType("SIMPLECOMPCONT")]
    public class SimpleComponentCont : Packet
    {
        [XmlElement("DIDDATAREF")]
        public DidDataRef? DidDataRef { get; set; }

        public DataIdentity? DataIdentity
        {
            get
            {
                if (DidDataRef != null && CanDela != null)
                {
                    return CanDela.EcuDocument.DataIdentities.DataIdentities.FirstOrDefault(x => x.Id == DidDataRef.DidRef);
                }

                return null;
            }
        }

        public override void SetCanDelaReference(CanDela? canDela = null)
        {
            base.SetCanDelaReference(canDela);
            DidDataRef?.SetCanDelaReference(canDela);
        }

        public override ConvertResult Convert(byte[] rawData)
        {
            return DataIdentity != null ? DataIdentity.Convert(rawData) : base.Convert(rawData);
        }
    }
}
