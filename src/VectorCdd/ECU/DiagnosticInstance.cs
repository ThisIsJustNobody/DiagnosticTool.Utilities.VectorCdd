using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

using static DiagnosticTool.Utilities.VectorCdd.DataTypes.BaseDataType;

namespace DiagnosticTool.Utilities.VectorCdd.ECU
{
    [XmlType("DIAGINST")]
    public class DiagnosticInstance : General, IConvert
    {
        [XmlAttribute("act")]
        public string? Act { get; set; }

        [XmlIgnore]
        public bool IsActive => Act != "0";

        [XmlElement("SERVICE")]
        public List<Service> Services { get; set; } = [];

        [XmlElement("STATICVALUE")]
        public StaticValue StaticValue { get; set; } = new();

        [XmlElement("UNS")]
        public List<UnsAttribute> UnsAttributes { get; set; } = [];

        [XmlElement("SIMPLECOMPCONT")]
        public List<SimpleComponentCont> SimpleCompConts { get; set; } = [];

        public ConvertResult Convert(byte[] rawData)
        {
            var r = SimpleCompConts.First().Convert(rawData);
            r.Name = Name ?? DefaultName;
            return r;
        }

        public override void SetCanDelaReference(CanDela? canDela = null)
        {
            base.SetCanDelaReference(canDela);
            Services.ForEach(x => x.SetCanDelaReference(canDela));
            StaticValue.SetCanDelaReference(canDela);
            SimpleCompConts.ForEach(x => x.SetCanDelaReference(canDela));
        }
    }
}
