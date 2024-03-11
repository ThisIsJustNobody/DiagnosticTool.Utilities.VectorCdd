using DiagnosticTool.Utilities.VectorCdd.XmlParser;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static DiagnosticTool.Utilities.VectorCdd.Extend;
using static DiagnosticTool.Utilities.VectorCdd.XmlParser.Common;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

public class ValueType
{
    #region 定义

    /// <summary>
    /// 字节序类型
    /// </summary>
    public enum ByteOrderTypes
    {
        /// <summary>
        /// 英特尔字节序（12）：[0x89, 0x34, 0x12, 0x98] => 0x98123489
        /// </summary>
        /// <remarks>
        /// 从左到右的数字，对应从右（下，高地址）到左（上，低地址）的字节数组。
        /// 从左到右的字节数组，对应从右到左的数字。
        /// 计算机领域默认为此字节序。
        /// </remarks>
        [XmlValue("12")]
        LowHigh,

        /// <summary>
        /// 摩托罗拉字节序（21）：[0x89, 0x34, 0x12, 0x98] => 0x89341298
        /// </summary>
        /// <remarks>
        /// 从左到右的数字，对应左（上，低地址）从到右（下，高地址）的字节数组。
        /// 从左（上，低地址）到右（下，高地址）的字节数组，对应从左到右的数字。
        /// 通信领域默认为此字节序。
        /// </remarks>
        [XmlValue("21")]
        HighLow,
    }

    /// <summary>
    /// 数据尺寸描述类型
    /// </summary>
    public enum SizeDescriptionTypes
    {
        /// <summary>
        /// 不可分的固定长度
        /// </summary>
        [XmlValue("atom")]
        Atom,

        /// <summary>
        /// 数组
        /// </summary>
        [XmlValue("field")]
        Field,
    }

    /// <summary>
    /// 编码类型
    /// </summary>
    public enum EncodingTypes
    {
        /// <summary>
        /// 无符号整数
        /// </summary>
        /// <remarks>
        /// 1 bit ... 4 bytes
        /// </remarks>
        [NumbericalValue(BitLength: 8, IsInteger: true, IsUnsigned: true)]
        [XmlValue("uns")]
        Unsigned,

        /// <summary>
        /// 有符号整数
        /// </summary>
        /// <remarks>
        /// 2 bits ... 4 bytes
        /// </remarks>
        [NumbericalValue(BitLength: 8, IsInteger: true, IsUnsigned: false)]
        [XmlValue("sgn")]
        Signed,

        /// <summary>
        /// 8421 BCD 码
        /// </summary>
        /// <remarks>
        /// 4 bits ... 4 bytes, 4k bits, k ∈ Z*
        /// </remarks>
        [NumbericalValue(BitLength: 8, IsInteger: true, IsUnsigned: true)]
        [XmlValue("bcd")]
        BCD,

        /// <summary>
        /// 64 位无符号整数
        /// </summary>
        [NumbericalValue(BitLength: 64, IsInteger: true, IsUnsigned: true)]
        [XmlValue("uns64")]
        Unsigned64,

        /// <summary>
        /// 64 位有符号整数
        /// </summary>
        [NumbericalValue(BitLength: 64, IsInteger: true, IsUnsigned: false)]
        [XmlValue("sgn64")]
        Signed64,

        /// <summary>
        /// 64 位 BCD 码
        /// </summary>
        [NumbericalValue(BitLength: 64, IsInteger: true, IsUnsigned: true)]
        [XmlValue("bcd64")]
        BCD64,

        /// <summary>
        /// 单精度浮点数
        /// </summary>
        /// <remarks>
        /// 4 bytes; IEEE 754 single precison, 1 + 23 + 8, sign + mantissa + exponent
        /// </remarks>
        [NumbericalValue(BitLength: 32, IsInteger: false, IsUnsigned: false)]
        [XmlValue("flt")]
        Float,

        /// <summary>
        /// 双精度浮点数
        /// </summary>
        /// <remarks>
        /// 8 bytes; IEEE 754 double precison, 1 + 52 + 11, sign + mantissa + exponent
        /// </remarks>
        [NumbericalValue(BitLength: 64, IsInteger: false, IsUnsigned: false)]
        [XmlValue("dbl")]
        DoubleFloat,

        /// <summary>
        /// ASCII 码
        /// </summary>
        /// <remarks>
        /// 1 byte, or 7 bits
        /// </remarks>
        [TextValue(7, 8)]
        [XmlValue("asc")]
        ASCII,

        /// <summary>
        /// Unicode 编码
        /// </summary>
        /// <remarks>
        /// 2 bytes
        /// </remarks>
        [TextValue(16)]
        [XmlValue("utf")]
        UNICODE,
    }

    /// <summary>
    /// 显示格式类型
    /// </summary>
    public enum DisplayFormatTypes
    {
        /// <summary>
        /// 十进制数
        /// </summary>
        [XmlValue("dec")]
        Decimal,

        /// <summary>
        /// 十六进制数
        /// </summary>
        [XmlValue("hex")]
        Hexadecimal,

        /// <summary>
        /// 八进制数
        /// </summary>
        [XmlValue("oct")]
        Octal,

        /// <summary>
        /// 二进制数
        /// </summary>
        [XmlValue("bin")]
        Binary,

        /// <summary>
        /// 浮点数
        /// </summary>
        [XmlValue("flt")]
        FloatingPoint,

        /// <summary>
        /// 文本
        /// </summary>
        [XmlValue("text")]
        Text,
    }

    /// <summary>
    /// 字符串中止类型
    /// </summary>
    /// <remarks>
    /// 仅对于 ASCII 和 UNICODE 有效。
    /// </remarks>
    public enum StringTerminationTypes
    {
        /// <summary>
        /// 未提供额外信息
        /// </summary>
        /// <remarks>
        /// 默认以 \0 中止，对于 ASCII 为 1 字节，对于 UNICODE 为 2 字节。</remarks>
        [XmlValue("no")]
        NoSizeinfo,

        /// <summary>
        /// 1 字节前导长度
        /// </summary>
        [LeadingSize(1)]
        [XmlValue("lszbyte")]
        LeadingSizeBytes_1,

        /// <summary>
        /// 2 字节前导长度
        /// </summary>
        [LeadingSize(2)]
        [XmlValue("lsz2bytes")]
        LeadingSizeBytes_2,

        /// <summary>
        /// 3 字节前导长度
        /// </summary>
        [LeadingSize(3)]
        [XmlValue("lsz3bytes")]
        LeadingSizeBytes_3,

        /// <summary>
        /// 4 字节前导长度
        /// </summary>
        [LeadingSize(4)]
        [XmlValue("lsz4bytes")]
        LeadingSizeBytes_4,

        /// <summary>
        /// 5 字节前导长度
        /// </summary>
        [LeadingSize(5)]
        [XmlValue("lsz5bytes")]
        LeadingSizeBytes_5,

        /// <summary>
        /// 以 \0 作为文本结束标志，\0 不作为文本内容
        /// </summary>
        /// <remarks>
        /// 有 2 种字符串结束的情况：
        /// 1. 遇到 \0 标志。
        /// 2. 遇到内容结束标志 EOS。
        /// </remarks>
        [ZeroTerminated]
        [XmlValue("zTermOrEos")]
        ZeroTerminatedNoPadding,

        /// <summary>
        /// 最大长度范围内以 \0 作为文本结束标志，\0 不作为文本内容
        /// </summary>
        /// <remarks>
        /// 有 3 种字符串结束的情况：
        /// 1. 截止到预设的最大长度。
        /// 2. 遇到 \0 标志。
        /// 3. 遇到内容结束标志 EOS。
        /// </remarks>
        [ZeroTerminated(InMaxLength: true)]
        [XmlValue("zTermOrMlOrEos")]
        ZeroTerminatedNoPaddingInMaxLength,

        /// <summary>
        /// 对于固定长度，以 \0 作为文本结束标志，剩余内容填充 \0
        /// </summary>
        /// <remarks>
        /// 可以使用 \0 填充剩余字节。
        /// </remarks>
        [ZeroTerminated(Padding: true)]
        ZeroTerminatedWithPadding,

        /// <summary>
        /// 对于固定长度，最大长度范围内以 \0 填充作为文本结束标志
        /// </summary>
        /// <remarks>
        /// 对于固定长度，可以使用 \0 填充剩余字节。
        /// </remarks>
        [ZeroTerminated(Padding: true, InMaxLength: true)]
        ZeroTerminatedWithPaddingInMaxLength,
    }

    /// <summary>
    /// 数字值特性
    /// </summary>
    /// <param name="IsInteger"></param>
    /// <param name="IsUnsigned"></param>
    [AttributeUsage(AttributeTargets.Field)]
    public class NumbericalValueAttribute(int BitLength = 8, bool IsInteger = false, bool IsUnsigned = false) : Attribute
    {
        /// <summary>
        /// 以数组形式解析时的单位位长
        /// </summary>
        public int BitLength { get; set; } = BitLength;

        /// <summary>
        /// 是否为整数
        /// </summary>
        public bool IsInteger { get; set; } = IsInteger;

        /// <summary>
        /// 是否为无符号数
        /// </summary>
        public bool IsUnsigned { get; set; } = IsUnsigned;
    }

    /// <summary>
    /// 文本值特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TextValueAttribute : Attribute
    {
        private readonly int singleTextMinBitLength;

        private readonly int singleTextMaxBitLength;

        /// <summary>
        /// 单字最小位长
        /// </summary>
        public int SingleTextMinBitLength => Math.Min(singleTextMinBitLength, singleTextMaxBitLength);

        /// <summary>
        /// 单字最大位长
        /// </summary>
        public int SingleTextMaxBitLength => Math.Max(singleTextMinBitLength, singleTextMaxBitLength);

        /// <summary>
        /// 可变长度
        /// </summary>
        public bool Variale => singleTextMinBitLength != singleTextMaxBitLength;

        public TextValueAttribute(int SingleTextBitLength = 8)
        {
            singleTextMinBitLength = SingleTextBitLength;
            singleTextMaxBitLength = SingleTextBitLength;
        }

        public TextValueAttribute(int SingleTextMinBitLength, int SingleTextMaxBitLength)
        {
            singleTextMinBitLength = SingleTextMinBitLength;
            singleTextMaxBitLength = SingleTextMaxBitLength;
        }
    }

    /// <summary>
    /// 前导码特性
    /// </summary>
    /// <param name="SizeBytes"></param>
    [AttributeUsage(AttributeTargets.Field)]
    public class LeadingSizeAttribute(int SizeBytes = 1) : Attribute
    {
        public int SizeBytes { get; set; } = SizeBytes;
    }

    /// <summary>
    /// 零终止特性
    /// </summary>
    /// <param name="Padding"></param>
    /// <param name="InMaxLength"></param>
    [AttributeUsage(AttributeTargets.Field)]
    public class ZeroTerminatedAttribute(bool Padding = false, bool InMaxLength = false) : Attribute
    {
        /// <summary>
        /// 填充
        /// </summary>
        /// <remarks>
        /// true: 文本在遇到首个 \0 时结束。
        /// false: 不允许填充时，仅处理最后一个 \0 之前的内容。
        /// </remarks>
        public bool Padding { get; set; } = Padding;

        /// <summary>
        /// 最大长度范围限制
        /// </summary>
        /// <remarks>
        /// true: \0 被包含在指定长度内。
        /// false: 不设置限制时，允许 \0 超出指定长度 1 个字长。
        /// </remarks>
        public bool InMaxLength { get; set; } = InMaxLength;
    }

    #endregion 定义

    #region 静态
    
    /// <summary>
    /// 按照字节尺寸截取（从前往后截取指定数量的字节）
    /// </summary>
    /// <param name="longBytes"></param>
    /// <param name="cutBytes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static byte[] InterceptByByteSize(byte[] longBytes, out byte[] cutBytes, int byteLength)
    {
        cutBytes = longBytes.Skip(byteLength).ToArray();
        return longBytes.Take(byteLength).ToArray();
    }

    /// <summary>
    /// 按照位尺寸截取
    /// </summary>
    /// <remarks>
    /// 按照 byte0-byte* bit7-bit0 位序处理。
    /// </remarks>
    /// <param name="longBytes"></param>
    /// <param name="cutBytes"></param>
    /// <returns></returns>
    public static byte[] InterceptByBitSize(byte[] longBytes, out byte[] cutBytes, int bitLength)
    {
        var binsString = longBytes.ToBinaryString();
        //cutBytes = string.Join("", binsString.Take(binsString.Length - bitLength)).FromBinarysString();
        cutBytes = binsString[..Math.Min(binsString.Length, binsString.Length - bitLength)].FromBinarysString();
        return binsString[Math.Min(binsString.Length, binsString.Length - bitLength)..].FromBinarysString();
    }

    /// <summary>
    /// 解释为文本
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="encodingType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string InterpretToText(byte[] bytes, EncodingTypes encodingType, ByteOrderTypes byteOrderType = ByteOrderTypes.HighLow, StringTerminationTypes stringTerminationType = StringTerminationTypes.NoSizeinfo)
    {
        if (byteOrderType == ByteOrderTypes.LowHigh)
        {
            bytes = bytes.Reverse().ToArray();
        }
        // TODO: 未验证 StringTerminationTypes 的效果
        if (stringTerminationType.GetType().GetField(stringTerminationType.ToString())!.GetCustomAttribute(typeof(LeadingSizeAttribute)) is LeadingSizeAttribute leadingSizeAttribute)
        {
            bytes = bytes[leadingSizeAttribute.SizeBytes..];
        }
        if (stringTerminationType.GetType().GetField(stringTerminationType.ToString())!.GetCustomAttribute(typeof(ZeroTerminatedAttribute)) is ZeroTerminatedAttribute zeroTerminatedAttribute)
        {
            if (zeroTerminatedAttribute.Padding)
            {
                var newBytes = new List<byte>();
                foreach (var b in bytes)
                {
                    if (b == 0)
                    {
                        break;
                    }
                    newBytes.Add(b);
                }
                bytes = [.. newBytes];
            }
            else
            {
                if (bytes.Last() == 0)
                {
                    bytes = bytes[..^1];
                }
            }
        }
        return encodingType switch
        {
            EncodingTypes.ASCII => Encoding.ASCII.GetString(bytes),
            EncodingTypes.UNICODE => Encoding.Unicode.GetString(bytes),
            _ => throw new Exception("该类型不可被解释为文本！"),
        };
    }

    public static string[] InterpretToTextField(byte[] bytes, EncodingTypes encodingType, ByteOrderTypes byteOrderType = ByteOrderTypes.HighLow)
    {
        if (encodingType.GetType().GetField(encodingType.ToString())!.GetCustomAttribute(typeof(TextValueAttribute)) is TextValueAttribute textValueAttribute)
        {
            List<string> texts = [];
            var binString = bytes.ToBinaryString();
            var atomDatas = binString.Select((bit, index) => (bit, index))
            .GroupBy(x => x.index / textValueAttribute.SingleTextMaxBitLength)
            .Select(g =>
            {
                string groupedBinString;
                groupedBinString = new string(g.Select(x => x.bit == '0' ? '0' : '1').ToArray());
                return groupedBinString.FromBinarysString();
            })
            .ToList();
            foreach (var atomData in atomDatas)
            {
                texts.Add(InterpretToText(atomData, encodingType, byteOrderType));
            }
            return [.. texts];
        }
        throw new Exception("该类型不可被解释为文本！");
    }

    /// <summary>
    /// 解释为数字
    /// </summary>
    /// <param name="bytes">原始值</param>
    /// <param name="encodingType">编码类型</param>
    /// <param name="byteOrderType">提供的原始值的字节序</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static decimal InterpretToDecimal(byte[] bytes, EncodingTypes encodingType, ByteOrderTypes byteOrderType = ByteOrderTypes.HighLow)
    {
        if (encodingType.GetType().GetField(encodingType.ToString())!.GetCustomAttribute(typeof(NumbericalValueAttribute)) is NumbericalValueAttribute numericalValueAttribute)
        {
            switch (encodingType)
            {
                case EncodingTypes.BCD:
                case EncodingTypes.BCD64:
                    {
                        var hexString = Convert.ToHexString(bytes);
                        return decimal.Parse(hexString);
                    }
                case EncodingTypes.Float:
                    {
                        if (byteOrderType == ByteOrderTypes.HighLow)
                        {
                            bytes = bytes.Reverse().ToArray();
                        }
                        var floatValue = BitConverter.ToSingle(bytes);
                        return decimal.Parse(floatValue.ToString(), System.Globalization.NumberStyles.Float);
                    }
                case EncodingTypes.DoubleFloat:
                    {
                        if (byteOrderType == ByteOrderTypes.HighLow)
                        {
                            bytes = bytes.Reverse().ToArray();
                        }
                        var doubleValue = BitConverter.ToDouble(bytes);
                        return decimal.Parse(doubleValue.ToString(), System.Globalization.NumberStyles.Float);
                    }
                default:
                    {
                        if (numericalValueAttribute.IsInteger)
                        {
                            var bigInt = new BigInteger(bytes, numericalValueAttribute.IsUnsigned, byteOrderType == ByteOrderTypes.HighLow);
                            return decimal.Parse(bigInt.ToString());
                        }
                        break;
                    }
            }
        }
        throw new Exception("该类型不可被解释为数字！");
    }

    public static decimal[] InterpretToDecimalField(byte[] bytes, EncodingTypes encodingType, ByteOrderTypes byteOrderType = ByteOrderTypes.HighLow)
    {
        if (encodingType.GetType().GetField(encodingType.ToString())!.GetCustomAttribute(typeof(NumbericalValueAttribute)) is NumbericalValueAttribute numericalValueAttribute)
        {
            List<decimal> decimals = [];
            var binString = bytes.ToBinaryString();
            var atomDatas = binString.Select((bit, index) => (bit, index))
            .GroupBy(x => x.index / numericalValueAttribute.BitLength)
            .Select(g =>
            {
                string groupedBinString;
                groupedBinString = new string(g.Select(x => x.bit == '0' ? '0' : '1').ToArray());
                return groupedBinString.FromBinarysString();
            })
            .ToList();
            foreach(var atomData in atomDatas)
            {
                decimals.Add(InterpretToDecimal(atomData, encodingType, byteOrderType));
            }
            return [.. decimals];
        }
        throw new Exception("该类型不可被解释为数字！");
    }

    /// <summary>
    /// 从数字展示文本
    /// </summary>
    /// <param name="value"></param>
    /// <param name="displayFormatType"></param>
    /// <param name="precision"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string DisplayValueFromValue(decimal value, DisplayFormatTypes displayFormatType, int precision = 2)
    {
        switch (displayFormatType)
        {
            case DisplayFormatTypes.Hexadecimal:
                {
                    var bigInt = new BigInteger(value);
                    var bytes = bigInt.ToByteArray().Reverse().ToArray();
                    var hexString = bytes.ToHexadecimalString(removeLeadingZero: false);
                    return $"0x{hexString}";
                }
            case DisplayFormatTypes.Decimal:
                return value.ToString("F0");
            case DisplayFormatTypes.Octal:
                {
                    var bigInt = new BigInteger(value);
                    var bytes = bigInt.ToByteArray().Reverse().ToArray();
                    return bytes.ToOctalString(removeLeadingZero: true);
                }
            case DisplayFormatTypes.Binary:
                {
                    var bigInt = new BigInteger(value);
                    var bytes = bigInt.ToByteArray().Reverse().ToArray();
                    return bytes.ToBinaryString(removeLeadingZero: true);
                }
            case DisplayFormatTypes.FloatingPoint:
                return value.ToString($"F{precision}");
            case DisplayFormatTypes.Text:
                return value.ToString();
            default:
                throw new Exception("该类型不可按照既定规则转换！");
        }
    }

    /// <summary>
    /// 从文本展示文本
    /// </summary>
    /// <param name="value"></param>
    /// <param name="displayFormatType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string DisplayValueFromText(string value, DisplayFormatTypes displayFormatType)
    {
        return displayFormatType switch
        {
            DisplayFormatTypes.Text => value,
            _ => throw new Exception("该类型不可按照既定规则转换！"),
        };
    }

    #endregion 静态


    #region 属性

    /// <summary>
    /// 字节序（bo）
    /// </summary>
    [XmlIgnore]
    public ByteOrderTypes ByteOrderType { get; set; } = ByteOrderTypes.HighLow;

    [XmlAttribute(AttributeName = "bo")]
    public string ByteOrderTypeXmlValue
    {
        get
        {
            return ByteOrderType.GetXmlValueAttributeValue();
        }
        set
        {
            ByteOrderType = value.ParseXmlValueAttributeValue<ByteOrderTypes>();
        }
    }

    [XmlIgnore]
    protected SizeDescriptionTypes sizeDescriptionType;

    /// <summary>
    /// 数据长度描述类型（qty）
    /// </summary>
    [XmlIgnore]
    public virtual SizeDescriptionTypes SizeDescriptionType
    {
        get => sizeDescriptionType;
        set => sizeDescriptionType = value;
    }

    [XmlAttribute(AttributeName = "qty")]
    public string SizeDescriptionTypeXmlValue
    {
        get
        {
            return SizeDescriptionType.GetXmlValueAttributeValue();
        }
        set
        {
            SizeDescriptionType = value.ParseXmlValueAttributeValue<SizeDescriptionTypes>();
        }
    }


    [XmlIgnore]
    protected EncodingTypes encodingType;

    /// <summary>
    /// 原始数据的编码类型（enc）
    /// </summary>
    /// <remarks>
    /// 指明原始数据（Raw Value）是以什么编码类型进行编码的，应当以相同的方式进行解码。
    /// </remarks>
    [XmlIgnore]
    public virtual EncodingTypes EncodingType
    {
        get => encodingType;
        set => encodingType = value;
    }

    [XmlAttribute(AttributeName = "enc")]
    public string EncodingTypeXmlValue
    {
        get
        {
            return EncodingType.GetXmlValueAttributeValue();
        }
        set
        {
            EncodingType = value.ParseXmlValueAttributeValue<EncodingTypes>();
        }
    }


    [XmlIgnore]
    protected int bitLength;

    /// <summary>
    /// 单个元素的位长（bl）
    /// </summary>
    [XmlAttribute(AttributeName = "bl")]
    public int BitLength
    {
        get => bitLength;
        set
        {
            if (value == 64 || value <= 32)
            {
                bitLength = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(BitLength), "位长度超出范围！");
            }
        }
    }


    [XmlIgnore]
    protected DisplayFormatTypes? displayFormatType;

    /// <summary>
    /// 显示格式类型（df）
    /// </summary>
    [XmlIgnore]
    public virtual DisplayFormatTypes DisplayFormatType
    {
        get
        {
            if (displayFormatType.HasValue)
            {
                return displayFormatType.Value;
            }
            return encodingType switch
            {
                EncodingTypes.Unsigned or EncodingTypes.Unsigned64 => DisplayFormatTypes.Hexadecimal,
                EncodingTypes.Signed or EncodingTypes.Signed64 or EncodingTypes.BCD or EncodingTypes.BCD64 => DisplayFormatTypes.Decimal,
                EncodingTypes.Float or EncodingTypes.DoubleFloat => DisplayFormatTypes.FloatingPoint,
                EncodingTypes.ASCII or EncodingTypes.UNICODE => DisplayFormatTypes.Text,
                _ => throw new ArgumentNullException(nameof(encodingType)),
            };
        }
        set => displayFormatType = value;
    }

    [XmlAttribute(AttributeName = "df")]
    public string DisplayFormatTypeXmlValue
    {
        get
        {
            return DisplayFormatType.GetXmlValueAttributeValue();
        }
        set
        {
            DisplayFormatType = value.ParseXmlValueAttributeValue<DisplayFormatTypes>();
        }
    }


    /// <summary>
    /// 小数位数（sig）
    /// </summary>
    /// <remarks>
    /// 推荐范围：对于 Float，范围在 2-7 之间。对于 Double，范围在 2-15 之间。
    /// </remarks>
    [XmlAttribute(AttributeName = "sig")]
    public int Precision { get; set; }

    [XmlIgnore]
    public StringTerminationTypes stringTerminationType = StringTerminationTypes.NoSizeinfo;

    /// <summary>
    /// 字符串类型（sz）
    /// </summary>
    [XmlIgnore]
    public virtual StringTerminationTypes StringTerminationType
    {
        get => stringTerminationType;
        set => stringTerminationType = value;
    }

    [XmlAttribute(AttributeName = "sz")]
    public string StringTerminationTypeXmlValue
    {
        get
        {
            return StringTerminationType.GetXmlValueAttributeValue();
        }
        set
        {
            StringTerminationType = value.ParseXmlValueAttributeValue<StringTerminationTypes>();
        }
    }


    [XmlIgnore]
    public int minFieldCount = 1;

    /// <summary>
    /// 最小尺寸（minsz）
    /// </summary>
    [XmlAttribute(AttributeName = "minsz")]
    public virtual int MinFieldCount
    {
        get => minFieldCount;
        set => minFieldCount = value;
    }


    [XmlIgnore]
    public int maxFieldCount = 1;

    /// <summary>
    /// 最大尺寸（mazsz）
    /// </summary>
    [XmlAttribute(AttributeName = "maxsz")]
    public virtual int MaxFieldCount
    {
        get => maxFieldCount;
        set => maxFieldCount = value;
    }

    /// <summary>
    /// 单位
    /// </summary>
    [XmlElement("UNIT")]
    public string? Unit { get; set; }

    protected bool ShouldSerializeUnit()
    {
        return !string.IsNullOrEmpty(Unit);
    }

    /// <summary>
    /// 总位长度范围
    /// </summary>
    [XmlIgnore]
    public (int, int) TotalBitLengthRange
    {
        get
        {
            switch (SizeDescriptionType)
            {
                case SizeDescriptionTypes.Atom:
                    return (BitLength, BitLength);
                case SizeDescriptionTypes.Field:
                    {
                        if (EncodingType.GetType().GetField(EncodingType.ToString())!.GetCustomAttribute(typeof(TextValueAttribute)) is TextValueAttribute textValueAttribute)
                        {
                            return (MinFieldCount * textValueAttribute.SingleTextMaxBitLength, MaxFieldCount * textValueAttribute.SingleTextMaxBitLength);
                        }
                        else if (EncodingType.GetType().GetField(EncodingType.ToString())!.GetCustomAttribute(typeof(NumbericalValueAttribute)) is NumbericalValueAttribute numbericalValueAttribute)
                        {
                            return (MinFieldCount * numbericalValueAttribute.BitLength, MaxFieldCount * numbericalValueAttribute.BitLength);
                        }
                        else
                        {
                            throw new Exception("未找到合适的特性信息！");
                        }
                    }
                default:
                    throw new Exception("未知的数据长度描述类型！");
            }
        }
    }


    /// <summary>
    /// 以字节为单位
    /// </summary>
    [XmlIgnore]
    public bool SizeByByte => BitLength % 8 == 0;

    /// <summary>
    /// 数字值
    /// </summary>
    [XmlIgnore]
    public bool IsNumber => EncodingType.GetType().GetField(EncodingType.ToString())!.GetCustomAttribute(typeof(NumbericalValueAttribute)) is NumbericalValueAttribute;

    /// <summary>
    /// 文本值
    /// </summary>
    [XmlIgnore]
    public bool IsText => EncodingType.GetType().GetField(EncodingType.ToString())!.GetCustomAttribute(typeof(TextValueAttribute)) is TextValueAttribute;
    #endregion 属性


    #region 方法

    /// <summary>
    /// 按照需要截取内容
    /// </summary>
    /// <param name="longBytes"></param>
    /// <param name="cutBytes"></param>
    /// <returns></returns>
    public byte[] InterceptBytes(byte[] longBytes, out byte[] cutBytes)
    {
        return InterceptByByteSize(longBytes, out cutBytes, TotalBitLengthRange.Item2 / 8);
    }

    /// <summary>
    /// 按照位截取内容
    /// </summary>
    /// <param name="longBytes"></param>
    /// <param name="cutBytes"></param>
    /// <returns></returns>
    public byte[] InterceptBits(byte[] longBytes, out byte[] cutBytes)
    {
        return InterceptByBitSize(longBytes, out cutBytes, TotalBitLengthRange.Item2);
    }

    /// <summary>
    /// 获取数值
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public decimal GetValue(byte[] bytes)
    {
        return InterpretToDecimal(bytes, EncodingType, ByteOrderType);
    }

    public decimal[] GetValues(byte[] bytes)
    {
        return InterpretToDecimalField(bytes, EncodingType, ByteOrderType);
    }

    /// <summary>
    /// 获取文本
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public string GetText(byte[] bytes)
    {
        return InterpretToText(bytes, EncodingType, ByteOrderType, StringTerminationType);
    }

    public string[] GetTexts(byte[] bytes)
    {
        return InterpretToTextField(bytes, EncodingType, ByteOrderType);
    }

    /// <summary>
    /// 获取结果（数字或文本）
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public object GetResult(byte[] bytes)
    {
        if (encodingType.GetType().GetField(encodingType.ToString())!.GetCustomAttribute(typeof(NumbericalValueAttribute)) is NumbericalValueAttribute)
        {
            if (SizeDescriptionType == SizeDescriptionTypes.Atom)
            {
                return GetValue(bytes);
            }
            else
            {
                return GetValues(bytes);
            }
        }
        else
        {
            if (SizeDescriptionType == SizeDescriptionTypes.Atom)
            {
                return GetText(bytes);
            }
            else
            {
                return GetTexts(bytes);
            }
        }
    }

    /// <summary>
    /// 展示文本
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string DisplayValue(decimal value)
    {
        var str = DisplayValueFromValue(value, DisplayFormatType, Precision);
        return str;
    }

    /// <summary>
    /// 展示文本
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string DisplayValue(string value)
    {
        var str = DisplayValueFromText(value, DisplayFormatType);
        return str;
    }

    /// <summary>
    /// 展示文本
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public string DisplayValue(object value, string separator = " ")
    {
        if (value.GetType() == typeof(string))
        {
            return DisplayValue((string)value);
        }
        else if (value.GetType() == typeof(decimal))
        {
            return DisplayValue((decimal)value);
        }
        else if (value.GetType() == typeof(decimal[]))
        {
            var values = (decimal[])value;
            return string.Join(separator, values.Select(DisplayValue));
        }
        else if (value.GetType() == typeof(string[]))
        {
            var values = (string[])value;
            return string.Join("", values.Select(DisplayValue));
        }
        else
        {
            throw new ArgumentException("值类型超出范围！", nameof(value));
        }
    }

    /// <summary>
    /// 获取结果并展示文本
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public string GetResultAndDisplayValue(byte[] bytes)
    {
        var result = GetResult(bytes);
        return DisplayValue(result);
    }

    #endregion 方法
}
