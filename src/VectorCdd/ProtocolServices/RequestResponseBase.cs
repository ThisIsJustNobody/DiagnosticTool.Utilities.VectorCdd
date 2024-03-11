using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.ProtocolServices
{
    public class RequestResponseBase : General
    {
        [XmlElement("CONSTCOMP", typeof(ConstComponent))]
        [XmlElement("STATICCOMP", typeof(StaticComponent))]
        [XmlElement("CONTENTCOMP", typeof(ContentComponent))]
        [XmlElement("SIMPLEPROXYCOMP", typeof(SimpleProxyComponent))]
        [XmlElement("GROUPOFDTCPROXYCOMP", typeof(BaseComponent))]
        public List<BaseComponent> Components { get; set; } = [];

        public byte RequestId =>
            Components.Where(c => c is ConstComponent { Spec: "sid" }).SelectMany(c => ((ConstComponent)c).Data).First();

        public byte[] RequestData =>
            Components.Where(c => c is ConstComponent cc && cc.Spec != "sid").SelectMany(c => ((ConstComponent)c).Data).ToArray();

        public override void SetCanDelaReference(CanDela? canDela = null)
        {
            base.SetCanDelaReference(canDela);
            Components.ForEach(c => c.SetCanDelaReference(canDela));
        }
    }
}
