using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

using static DiagnosticTool.Utilities.VectorCdd.DataTypes.BaseDataType.ConvertResult;
using static DiagnosticTool.Utilities.VectorCdd.DataTypes.ValueType;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[XmlType("DATAOBJ")]
public class BaseDataType : General, IConvert
{
    #region 定义

    public class InvalidValue
    {
        [XmlAttribute(DataType = "string", AttributeName = "s")]
        public string Start { get; set; } = string.Empty;

        [XmlAttribute(DataType = "string", AttributeName = "e")]
        public string End { get; set; } = string.Empty;

        [XmlAttribute(DataType = "string", AttributeName = "inv")]
        public string? Invalidity { get; set; }

        protected bool ShouldSerializeInvalidity() => Invalidity != null;


        [XmlAttribute(DataType = "string", AttributeName = "text")]  // TODO: 未验证属性名称！
        public string? AssignedText { get; set; }

        protected bool ShouldSerializeAssignedText() => AssignedText != null;
    }

    public class ConvertResult
    {
        public struct DisplayResult
        {
            public bool IsSuccessful { get; set; }

            public string Name { get; set; }

            public object Value { get; set; }
        }

        public class Result
        {
            /// <summary>
            /// 解码/运算后的值（数字、文本，及对应的数组形式）
            /// </summary>
            public object? Value { get; set; }

            /// <summary>
            /// 解码/运算后的字符串
            /// </summary>
            public string? ValueString { get; set; }

            /// <summary>
            /// 单位
            /// </summary>
            public string? Unit { get; set; }
        }

        public class ErrorInfo
        {
            public string ErrorType { get; set; } = string.Empty;

            public string ErrorMessage { get; set; } = string.Empty;
        }

        public string Name { get; set; } = DefaultName;

        public bool IsSuccessful { get; set; }

        public byte[] RawData { get; set; } = [];

        /// <summary>
        /// 解码结果
        /// </summary>
        public Result CodeResult { get; set; } = new();
        
        /// <summary>
        /// 运算结果
        /// </summary>
        public Result PhysicalResult { get; set; } = new();

        /// <summary>
        /// 错误信息
        /// </summary>
        public ErrorInfo Error { get; set; } = new();

        public string DisplayText(int depth = 0)
        {
            string result;
            if (!IsSuccessful)
            {
                result = $"{Error.ErrorType}: {Error.ErrorMessage}";
                return $"[Error!]{Name}:\t{result.Replace(Environment.NewLine, $"{Environment.NewLine}{new string('\t', depth + 1)}")}";
            }

            if (PhysicalResult.ValueString != null)
            {
                if (PhysicalResult.Unit != null)
                {
                    result = $"{PhysicalResult.ValueString} [{PhysicalResult.Unit}]";
                }
                else
                {
                    result = PhysicalResult.ValueString;
                }
                return $"{Name}:\t{result.Replace(Environment.NewLine, $"{Environment.NewLine}{new string('\t', depth + 1)}")}";
            }

            if (PhysicalResult.Value == null)
            {
                result = "0x" + BitConverter.ToString(RawData).Replace("-", " 0x");
            }
            else if (PhysicalResult.Value.GetType() == typeof(ConvertResult))
            {
                var convertResult = (ConvertResult)PhysicalResult.Value;
                result = convertResult.DisplayText();
            }
            else if (PhysicalResult.Value.GetType() == typeof(List<ConvertResult>))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("0x" + BitConverter.ToString(RawData).Replace("-", " 0x"));
                var convertResults = (List<ConvertResult>)PhysicalResult.Value;
                var childTexts = convertResults.Select(x =>
                {
                    var childText = x.DisplayText(depth + 1);
                    //childText = childText.Replace(Environment.NewLine, $"{Environment.NewLine}{new string('\t', depth)}");
                    //childText = new string('\t', depth) + childText;
                    return childText;
                }).ToArray();
                stringBuilder.AppendJoin(Environment.NewLine, childTexts);
                result = stringBuilder.ToString().Replace(Environment.NewLine, $"{Environment.NewLine}{new string('\t', 1)}");
            }
            else
            {
                throw new Exception("预期之外的错误！");
            }

            return $"{Name}:\t{result}";
        }
    }

    #endregion 定义

    #region 属性

    [XmlElement("EXCL")]
    public List<InvalidValue> InvalidValues { get; set; } = [];

    [XmlIgnore]
    public bool IsInvalidValuesActivate
    {
        get => InvalidValues.Count > 0;
        set
        {
            if (!value)
            {
                InvalidValues.Clear();
            }
        }
    }

    protected bool ShouldSerializeInvalidValues() => IsInvalidValuesActivate;

    [XmlIgnore]
    public ulong? bitMask;

    /// <summary>
    /// 掩码
    /// </summary>
    [XmlIgnore]
    public ulong? BitMask
    {
        get => bitMask;
        set => bitMask = value;
    }

    [XmlAttribute(AttributeName = "bm")]
    public string BitMaskXmlValue
    {
        get
        {
            return BitMask.HasValue ? BitMask.Value.ToString() : string.Empty;
        }
        set
        {
            BitMask = ulong.TryParse(value, out var result) ? result : null;
        }
    }

    protected bool ShouldSerializeBitMaskValue()
    {
        return BitMask.HasValue;
    }

    /// <summary>
    /// 默认掩码
    /// </summary>
    /// <remarks>
    /// 要求 bitLength > 0
    /// </remarks>
    [XmlIgnore]
    public ulong DefaultBitMask => Math.Max(1, (ulong)Math.Pow(2, CodeValueType.BitLength) - 1);

    /// <summary>
    /// 编码值类型
    /// </summary>
    [XmlElement("CVALUETYPE")]
    public virtual ValueType CodeValueType { get; set; } = new();

    /// <summary>
    /// 物理值类型
    /// </summary>
    [XmlElement("PVALUETYPE")]
    public virtual ValueType PhysicalValueType { get; set; } = new();

    public virtual (int, int) TotalBitLengthRange => DataTypeReference != null ? (DataTypeReference.CodeValueType.TotalBitLengthRange.Item1, DataTypeReference.CodeValueType.TotalBitLengthRange.Item2) : (CodeValueType.TotalBitLengthRange.Item1, CodeValueType.TotalBitLengthRange.Item2);

    #endregion 属性

    #region 构造方法

    #endregion 构造方法

    #region 方法

    /// <summary>
    /// 应用掩码
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public byte[] ApplyBitMask(byte[] bytes)
    {
        if (BitMask == null || CodeValueType.SizeDescriptionType == SizeDescriptionTypes.Field)
            return bytes;
        var maskBytes = BitConverter.GetBytes(BitMask.Value).Reverse().ToArray();
        maskBytes = maskBytes[^bytes.Length..];
        return bytes.Select((x, i) => (byte)(x & maskBytes[i])).ToArray();
    }

    public bool IsValid(object? value, out string? invalidity, out string? assignedText)
    {
        if (!IsInvalidValuesActivate)
        {
            invalidity = null;
            assignedText = null;
            return true;
        }
        var inv = InvalidValues.FirstOrDefault(x =>
        {
            return value switch
            {
                null => false,
                decimal dec => decimal.Parse(x.Start) <= dec && decimal.Parse(x.End) >= dec,
                string s => new BigInteger(Encoding.Unicode.GetBytes(x.Start)) <=
                    new BigInteger(Encoding.Unicode.GetBytes(s)) && new BigInteger(Encoding.Unicode.GetBytes(x.End)) >=
                    new BigInteger(Encoding.Unicode.GetBytes(s)),
                _ => false
            };
        });
        if (inv == null)
        {
            invalidity = null;
            assignedText = null;
            return true;
        }
        invalidity = inv.Invalidity;
        assignedText = inv.AssignedText;
        return false;
    }

    /// <summary>
    /// 按照需要截取内容
    /// </summary>
    /// <param name="longBytes"></param>
    /// <param name="cutBytes"></param>
    /// <returns></returns>
    public virtual byte[] InterceptBytes(byte[] longBytes, out byte[] cutBytes)
    {
        if (DataTypeReference != null)
        {
            return DataTypeReference.InterceptBytes(longBytes, out cutBytes);
        }
        return CodeValueType.InterceptBytes(longBytes, out cutBytes);
    }

    public virtual byte[] InterceptBits(byte[] longBytes, out byte[] cutBytes)
    {
        if (DataTypeReference != null)
        {
            return DataTypeReference.InterceptBits(longBytes, out cutBytes);
        }
        return CodeValueType.InterceptBits(longBytes, out cutBytes);
    }

    protected ConvertResult ConvertCodeValue(byte[] rawData)
    {
        if (DataTypeReference != null)
            return DataTypeReference.ConvertCodeValue(rawData);

        var result = new ConvertResult() { Name = Name ?? DefaultName, RawData = rawData };
        result.CodeResult.Unit = CodeValueType.Unit;
        result.PhysicalResult.Unit = PhysicalValueType.Unit;

        rawData = ApplyBitMask(rawData);
        var value = CodeValueType.GetResult(rawData);
        result.CodeResult.Value = value;
        result.CodeResult.ValueString = CodeValueType.DisplayValue(value);

        return result;
    }

    protected bool CheckValidity(ref ConvertResult convertResult)
    {
        if (!IsValid(convertResult.CodeResult.Value, out var invalidity, out var assignedText))
        {
            convertResult.Error.ErrorType = "Invalid";
            convertResult.Error.ErrorMessage = $"InValid: {invalidity}{(assignedText != null ? $"Assigned Text: {assignedText}" : "")}";
            return false;
        }
        return true;
    }

    protected void ConvertPhysicalValue(ref ConvertResult convertResult)
    {
        if (DataTypeReference != null)
        {
            DataTypeReference.ConvertPhysicalValue(ref convertResult);
            return;
        }
        convertResult.PhysicalResult.Value = convertResult.CodeResult.Value;
        convertResult.PhysicalResult.ValueString = convertResult.PhysicalResult.Value == null ? null : PhysicalValueType.DisplayValue(convertResult.PhysicalResult.Value);
        convertResult.IsSuccessful = true;
    }

    /// <summary>
    /// 转换
    /// </summary>
    /// <remarks>
    /// ECU 获取的原始值，根据设定进行转换。
    /// 可能的情况：Identity 原样输出，Linear 转为 double，Text Table 转为 UNICODE。
    /// </remarks>
    /// <param name="rawData"></param>
    /// <returns></returns>
    public virtual ConvertResult Convert(byte[] rawData)
    {
        if (DataTypeReference != null)
            return DataTypeReference.Convert(rawData);
        try
        {
            var result = ConvertCodeValue(rawData);
            if (!CheckValidity(ref result))
            {
                return result;
            }

            ConvertPhysicalValue(ref result);

            return result;
        }
        catch (Exception ex)
        {
            return new ConvertResult() { Name = Name ?? DefaultName, IsSuccessful = false, Error = { ErrorType = ex.GetType().ToString(), ErrorMessage = ex.Message } };
        }
    }

    #endregion 方法
}
