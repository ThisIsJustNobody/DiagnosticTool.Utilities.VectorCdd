using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.ProtocolServices
{
    [XmlType("PROTOCOLSERVICE")]
    public class ProtocolService : General
    {
        [XmlAttribute(DataType = "boolean", AttributeName = "func")]
        public bool FunctionalAddress { get; set; }

        [XmlAttribute(DataType = "boolean", AttributeName = "phys")]
        public bool PhysicalAddress { get; set; }

        [XmlAttribute(DataType = "boolean", AttributeName = "mresp")]
        public bool MixResponse { get; set; }

        [XmlAttribute(DataType = "boolean", AttributeName = "respOnFunc")]
        public bool ResponseOnFunctionalAddress { get; set; }

        [XmlAttribute(DataType = "boolean", AttributeName = "respOnPhys")]
        public bool ResponseOnPhysicalAddress { get; set; }

        [XmlAttribute(DataType = "boolean", AttributeName = "maycombcont")]
        public bool MayCombCount { get; set; }

        [XmlElement("REQ")]
        public RequestResponseBase Request { get; set; } = new();

        [XmlElement("POS")]
        public RequestResponseBase PositiveResponse { get; set; } = new();

        [XmlElement("NEG")]
        public RequestResponseBase NegativeResponse { get; set; } = new();

        public override void SetCanDelaReference(CanDela? canDela = null)
        {
            base.SetCanDelaReference(canDela);
            Request.SetCanDelaReference(canDela);
            PositiveResponse.SetCanDelaReference(canDela);
            NegativeResponse.SetCanDelaReference(canDela);
        }
    }
}
