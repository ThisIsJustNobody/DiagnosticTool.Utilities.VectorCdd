using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

using static DiagnosticTool.Utilities.VectorCdd.DataTypes.TextTable;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[XmlType("MUXDT")]
public class Multiplexer : BaseDataType
{
    public class Case
    {
        [XmlIgnore]
        public int Start { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "s")]
        public string StartXmlValue
        {
            get => Start.ToString();
            set
            {
                Start = int.Parse(value);
            }
        }

        [XmlIgnore]
        public int End { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "e")]
        public string EndXmlValue
        {
            get => End.ToString();
            set
            {
                End = int.Parse(value);
            }
        }

        [XmlElement("STRUCTURE", Type = typeof(Packet))]
        public Packet DataType { get; set; } = new();

        public bool InRange(int index)
        {
            return index >= Start && index <= End;
        }
    }


    [XmlElement("STRUCTURE", Type = typeof(Packet))]
    public Packet DataType { get; set; } = new();

    [XmlElement("CASE")]
    public List<Case> Cases { get; set; } = [];

    public override void SetCanDelaReference(CanDela? canDela = null)
    {
        base.SetCanDelaReference(canDela);
        DataType.SetCanDelaReference(canDela);
        Cases.ForEach(x => x.DataType.SetCanDelaReference(canDela));
    }

    public override ConvertResult Convert(byte[] rawData)
    {
        var result = new ConvertResult() { Name = Name ?? DefaultName, RawData = rawData };
        result.CodeResult.Unit = CodeValueType.Unit;
        result.PhysicalResult.Unit = PhysicalValueType.Unit;

        byte[] restData = [.. rawData];
        byte[] interceptingData = [.. restData];
        var interceptedData = InterceptBytes(interceptingData, out restData);
        var codeResult = ConvertCodeValue(interceptedData);
        var value = codeResult.CodeResult.Value;

        if (Cases.FirstOrDefault(x => x.InRange(System.Convert.ToInt32(value!))) is { } @case)
        {
            return @case.DataType.Convert(restData);
        }
        return DataType.Convert(restData);
    }
}
