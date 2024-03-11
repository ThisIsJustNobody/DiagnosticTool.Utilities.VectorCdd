using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

public class General : CanDelaBase
{
    #region 定义
    public const string DefaultLanguage = "en-US";

    public const string DefaultName = "NoName";

    [XmlType("TUV")]
    public class MultiLanguageText
    {
        [XmlAttribute(DataType = "string", AttributeName = "xml:lang")]
        public string Language { get; set; } = DefaultLanguage;

        [XmlText]
        public string Text { get; set; } = string.Empty;
    }
    #endregion 定义

    #region 静态
    /// <summary>
    /// 默认名称
    /// </summary>
    [XmlIgnore]
    public static string Language { get; set; } = DefaultLanguage;
    #endregion 静态

    [XmlAttribute(DataType = "string", AttributeName = "id")]
    public string? Id { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "oid")]
    public string? ObjectId { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "temploid")]
    public string? TemplateObjectId { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "spec")]
    public string? Spec { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "dtref")]
    public string? DataTypeReferenceId { get; set; }

    [XmlArray("NAME")]
    public List<MultiLanguageText> Names { get; set; } = [];

    [XmlIgnore]
    public string? Name
    {
        get
        {
            return Names.FirstOrDefault(x => x.Language == Language)?.Text;
        }
        set
        {
            if (Names.FirstOrDefault(x => x.Language == Language) is { } multiLanguageText)
            {
                if (value == null)
                {
                    Names.Remove(multiLanguageText);
                }
                else
                {
                    multiLanguageText.Text = value;
                }
            }
            else if (value != null)
            {
                multiLanguageText = new MultiLanguageText
                {
                    Language = Language,
                    Text = value
                };
                Names.Add(multiLanguageText);
            }
        }
    }

    [XmlElement("QUAL")]
    public string? Qualified { get; set; }

    [XmlElement("DESC")]
    public List<MultiLanguageText> Descriptions { get; set; } = [];

    [XmlIgnore]
    public string? Description
    {
        get
        {
            return Descriptions.FirstOrDefault(x => x.Language == Language)?.Text;
        }
        set
        {
            if (Descriptions.FirstOrDefault(x => x.Language == Language) is { } multiLanguageText)
            {
                if (value == null)
                {
                    Descriptions.Remove(multiLanguageText);
                }
                else
                {
                    multiLanguageText.Text = value;
                }
            }
            else if (value != null)
            {
                multiLanguageText = new MultiLanguageText
                {
                    Language = Language,
                    Text = value
                };
                Descriptions.Add(multiLanguageText);
            }
        }
    }

    [XmlIgnore]
    public DataTypes.BaseDataType? DataTypeReference
    {
        get
        {
            if (DataTypeReferenceId != null && CanDela?.EcuDocument.DataTypes.DataObjects.FirstOrDefault(x => x.Id == DataTypeReferenceId) is { } dataType)
            {
                return dataType;
            }
            return null;
        }
        set => DataTypeReferenceId = value?.Id;
    }

    protected bool ShouldSerializeId()
    {
        return !string.IsNullOrEmpty(Id);
    }

    protected bool ShouldSerializeObjectId()
    {
        return !string.IsNullOrEmpty(ObjectId);
    }

    protected bool ShouldSerializeTemplateObjectId()
    {
        return !string.IsNullOrEmpty(TemplateObjectId);
    }

    protected bool ShouldSerializeSpec()
    {
        return !string.IsNullOrEmpty(Spec);
    }

    protected bool ShouldSerializeDataTypeRef()
    {
        return !string.IsNullOrEmpty(DataTypeReferenceId);
    }

    protected bool ShouldSerializeQualified()
    {
        return !string.IsNullOrEmpty(Qualified);
    }

    protected bool ShouldSerializeDescription()
    {
        return Descriptions.Count > 0;
    }
}
