using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[XmlType("STRUCT")]
public class BitsField : Packet
{
    /// <summary>
    /// 总位长度范围
    /// </summary>
    [XmlIgnore]
    public override (int, int) TotalBitLengthRange
    {
        get
        {
            return (
                DataObjs.Select(x => x.TotalBitLengthRange.Item1).Sum(),
                DataObjs.Select(x => x.TotalBitLengthRange.Item2).Sum()
            );
        }
    }

    public override ConvertResult Convert(byte[] rawData)
    {
        var result = new ConvertResult() { Name = Name ?? DefaultName, RawData = rawData };
        result.CodeResult.Unit = CodeValueType.Unit;
        result.PhysicalResult.Unit = PhysicalValueType.Unit;

        List<ConvertResult> results = [];
        byte[] restData = [.. rawData];
        foreach (var dtObj in DataObjs)
        {
            try
            {
                byte[] interceptingData = [.. restData];
                var interceptedData = dtObj.InterceptBits(interceptingData, out restData);
                var r = dtObj.Convert(interceptedData);
                r.Name = dtObj.Name ?? DefaultName;
                results.Add(r);
            }
            catch (Exception ex)
            {
                var r = new ConvertResult()
                {
                    Name = dtObj.Name ?? DefaultName,
                    RawData = [.. rawData],
                    IsSuccessful = false,
                    Error = new ConvertResult.ErrorInfo()
                    {
                        ErrorType = nameof(ex),
                        ErrorMessage = ex.Message
                    }
                };
                results.Add(r);
            }
        }
        result.PhysicalResult.Value = results;
        result.IsSuccessful = true;
        return result;
    }
}
