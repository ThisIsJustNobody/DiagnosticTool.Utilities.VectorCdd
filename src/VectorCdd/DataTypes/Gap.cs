using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[XmlType("GAPDATAOBJ")]
public class Gap : BaseDataType
{
    [XmlIgnore]
    protected int bitLength;

    /// <summary>
    /// 单个元素的位长（bl）
    /// </summary>
    [XmlAttribute(AttributeName = "bl")]
    public int BitLength
    {
        get => bitLength;
        set => bitLength = value;
    }

    public override byte[] InterceptBytes(byte[] longBytes, out byte[] cutBytes)
    {
        return ValueType.InterceptByByteSize(longBytes, out cutBytes, BitLength / 8);
    }

    public override byte[] InterceptBits(byte[] longBytes, out byte[] cutBytes)
    {
        return ValueType.InterceptByBitSize(longBytes, out cutBytes, BitLength);
    }

    public override ConvertResult Convert(byte[] rawData)
    {
        var result = ConvertCodeValue(rawData);
        result.IsSuccessful = true;
        return result;
    }
}
