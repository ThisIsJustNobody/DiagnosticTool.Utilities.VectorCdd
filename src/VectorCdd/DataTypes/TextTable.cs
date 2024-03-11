using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.DataTypes;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[XmlType("TEXTTBL")]
public partial class TextTable : BaseDataType
{
    #region 定义
    [XmlType("TEXTMAP")]
    public partial class TextMap
    {
        //[XmlElement]  // 不能有此标记，如果有的话，会将 Text 的所有子项作为该节点的直接元素。
        [XmlArray("TEXT")]
        public List<General.MultiLanguageText> Text { get; set; } = [];

        [XmlIgnore]
        public uint Start { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "s")]
        public string StartXmlValue
        {
            get => Start.ToString();
            set
            {
                var m = TextTableStartAndEndAttributeTextType1().Match(value);
                if (m.Success)
                {
                    Start = uint.Parse(m.Value);
                    return;
                }
                m = TextTableStartAndEndAttributeTextType2().Match(value);
                if (m.Success)
                {
                    Start = uint.Parse(m.Groups[1].Value);
                    return;
                }
                
                throw new Exception();
            }
        }

        [XmlIgnore]
        public uint End { get; set; }

        [XmlAttribute(DataType = "string", AttributeName = "e")]
        public string EndXmlValue
        {
            get => End.ToString();
            set
            {
                var m = TextTableStartAndEndAttributeTextType1().Match(value);
                if (m.Success)
                {
                    End = uint.Parse(m.Value);
                    return;
                }
                m = TextTableStartAndEndAttributeTextType2().Match(value);
                if (m.Success)
                {
                    End = uint.Parse(m.Groups[1].Value);
                    return;
                }

                throw new Exception();
            }
        }

        public bool InRange(uint index)
        {
            return index >= Start && index <= End;
        }

        [GeneratedRegex(@"\((\d+)\)")]
        private static partial Regex TextTableStartAndEndAttributeTextType2();
        [GeneratedRegex(@"\d+")]
        private static partial Regex TextTableStartAndEndAttributeTextType1();
    }

    #endregion 定义

    [XmlElement("TEXTMAP")]
    public List<TextMap> TextMaps { get; set; } = [];

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

        if (TextMaps.FirstOrDefault(x => x.InRange((uint)value)) is { } textMap)
        {
            var mappedValue = PhysicalValueType.DisplayValue(textMap.Text.First(x => x.Language == Language).Text);
            result.PhysicalResult.Value = mappedValue;
            result.PhysicalResult.ValueString = PhysicalValueType.DisplayValue(mappedValue);
            result.IsSuccessful = true;
            return result;
        }
        result.Error.ErrorType = "OutOfRange";
        result.Error.ErrorMessage = "没有匹配的映射！";
        return result;
    }
}
