using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[XmlType("STRUCTDT")]
public class Packet : BaseDataType
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

    [XmlElement("STRUCT", Type = typeof(BitsField))]
    [XmlElement("DATAOBJ", Type = typeof(BaseDataType))]
    [XmlElement("GAPDATAOBJ", Type = typeof(Gap))]
    public List<BaseDataType> DataObjs { get; set; } = [];

    public override void SetCanDelaReference(CanDela? canDela = null)
    {
        base.SetCanDelaReference(canDela);
        DataObjs.ForEach(x => x.SetCanDelaReference(canDela));
    }

    public override byte[] InterceptBytes(byte[] longBytes, out byte[] cutBytes)
    {
        if (DataTypeReference != null)
            return DataTypeReference.InterceptBytes(longBytes, out cutBytes);
        if (CodeValueType.TotalBitLengthRange.Item2 != 0)
            return CodeValueType.InterceptBytes(longBytes, out cutBytes);
        return ValueType.InterceptByByteSize(
            longBytes,
            out cutBytes,
            TotalBitLengthRange.Item2 / 8
        );
    }

    public override byte[] InterceptBits(byte[] longBytes, out byte[] cutBytes)
    {
        if (DataTypeReference != null)
            return DataTypeReference.InterceptBits(longBytes, out cutBytes);
        if (CodeValueType.TotalBitLengthRange.Item2 != 0)
            return CodeValueType.InterceptBits(longBytes, out cutBytes);
        return ValueType.InterceptByBitSize(longBytes, out cutBytes, TotalBitLengthRange.Item2);
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
                var interceptedData = dtObj.InterceptBytes(interceptingData, out restData);
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
