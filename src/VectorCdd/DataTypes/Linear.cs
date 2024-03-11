using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static DiagnosticTool.Utilities.VectorCdd.DataTypes.BaseDataType;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[XmlType("LINCOMP")]
public class Linear : BaseDataType
{
    [XmlType("COMP")]
    public class Component
    {
        /// <summary>
        /// 系数（f）
        /// </summary>
        [XmlAttribute(DataType = "float", AttributeName = "f")]
        public float Factor { get; set; } = 1;

        /// <summary>
        /// 除数（div）
        /// </summary>
        [XmlAttribute(DataType = "int", AttributeName = "div")]
        public int Divisor { get; set; } = 1;

        protected bool ShouldSerializeDivisor() => Divisor != 1;

        /// <summary>
        /// 偏移量（o）
        /// </summary>
        [XmlAttribute(DataType = "float", AttributeName = "o")]
        public float Offset { get; set; } = 0;

        /// <summary>
        /// 下限，起始位置（s）
        /// </summary>
        [XmlIgnore]
        public double? LowerLimit { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "s")]
        public string LowerLimitXmlValue
        {
            get => LowerLimit.ToString() ?? string.Empty;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    LowerLimit = null;
                }
                else
                {
                    LowerLimit = double.Parse(value);
                }
            }
        }

        protected bool ShouldSerializeLowerLimitXmlValue() => LowerLimit.HasValue;


        /// <summary>
        /// 上限，结束位置（e）
        /// </summary>
        [XmlIgnore]
        public double? UpperLimit { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "e")]
        public string UpperLimitXmlValue
        {
            get => UpperLimit.ToString() ?? string.Empty;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    UpperLimit = null;
                }
                else
                {
                    UpperLimit = double.Parse(value);
                }
            }
        }

        protected bool ShouldSerializeUpperLimitXmlValue() => UpperLimit.HasValue;

        /// <summary>
        /// 倒数
        /// </summary>
        [XmlAttribute(DataType = "unsignedInt", AttributeName = "inv")]  // TODO: 没有验证此参数名！
        public uint InverseValue { get; set; } = 0;

        protected bool ShouldSerializeInverseValue() => Factor == 0;
    }

    [XmlElement("COMP")]
    public Component ComponentValue { get; set; } = new();

    public decimal? LowerLimitValue
    {
        get
        {
            if (!ComponentValue.LowerLimit.HasValue)
            {
                return null;
            }
            return CodeValueType.GetValue(BitConverter.GetBytes(ComponentValue.LowerLimit.Value).Reverse().ToArray());
        }
    }

    public decimal? UpperLimitValue
    {
        get
        {
            if (!ComponentValue.UpperLimit.HasValue)
            {
                return null;
            }
            return CodeValueType.GetValue(BitConverter.GetBytes(ComponentValue.UpperLimit.Value).Reverse().ToArray());
        }
    }

    public string ValidRange
    {
        get
        {
            if (LowerLimitValue.HasValue)
            {
                if (UpperLimitValue.HasValue)
                {
                    return $"[{LowerLimitValue.Value}, {UpperLimitValue.Value}]";
                }
                return $"[{LowerLimitValue.Value} ..]";
            }
            else
            {
                if (UpperLimitValue.HasValue)
                {
                    return $"[.. {UpperLimitValue.Value}]";
                }
                else
                {
                    return "[..]";
                }
            }
        }
    }

    /// <summary>
    /// 转换公式
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public decimal ConversionFormula(decimal data)
    {
        if (ComponentValue.Factor == 0)
        {
            // TODO: 该算法未经验证！
            return ComponentValue.InverseValue / ComponentValue.Divisor / data + (decimal)ComponentValue.Offset;
        }
        else
        {
            return (decimal)ComponentValue.Factor / ComponentValue.Divisor * data + (decimal)ComponentValue.Offset;
        }
    }

    public override ConvertResult Convert(byte[] rawData)
    {
        var result = ConvertCodeValue(rawData);
        if (!CheckValidity(ref result))
        {
            return result;
        }

        if (result.CodeResult.Value == null || result.CodeResult.Value.GetType() != typeof(decimal))
        {
            throw new Exception();
        }

        var value = (decimal)result.CodeResult.Value;

        if (LowerLimitValue.HasValue && value < LowerLimitValue.Value)
        {
            result.Error.ErrorType = "UndefinedRange";
            result.Error.ErrorMessage = $"{System.Convert.ToHexString(rawData)} (no interpretation possible){Environment.NewLine}Error: Invalid parameter value!{Environment.NewLine}Valid range: {ValidRange}";
            return result;
        }
        if (UpperLimitValue.HasValue && value > UpperLimitValue.Value)
        {
            result.Error.ErrorType = "UndefinedRange";
            result.Error.ErrorMessage = $"{System.Convert.ToHexString(rawData)} (no interpretation possible){Environment.NewLine}Error: Invalid parameter value!{Environment.NewLine}Valid range: {ValidRange}";
            return result;
        }

        var calculatedValue = ConversionFormula(value);

        result.PhysicalResult.Value = calculatedValue;
        result.PhysicalResult.ValueString = PhysicalValueType.DisplayValue(calculatedValue);
        result.IsSuccessful = true;
        return result;
    }
}
