using System.Collections.Generic;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.RecordTmpls;

[XmlType("RECORDTMPL")]
public class RecordTemplate : General
{
    [XmlElement("CVALUETYPE")]
    public DataTypes.ValueType? CodeValueType { get; set; }

    [XmlElement("PVALUETYPE")]
    public DataTypes.ValueType? PhysicalValueType { get; set; }

    [XmlElement("TRRECORDITEMTMPL", typeof(TextRecordItemTemplate))]
    [XmlElement("ENUMRECORDITEMTMPL", typeof(EnumRecordItemTemplate))]
    [XmlElement("UNSRECORDITEMTMPL", typeof(UnsignedRecordItemTemplate))]
    public List<RecordItemTemplate> ItemTemplates { get; set; } = [];
}

public abstract class RecordItemTemplate : General
{
    [XmlAttribute("mayBeDup")]
    public int MayBeDuplicate { get; set; }

    [XmlAttribute("conv")]
    public string? Conversion { get; set; }
}

[XmlType("TRRECORDITEMTMPL")]
public class TextRecordItemTemplate : RecordItemTemplate
{
}

[XmlType("ENUMRECORDITEMTMPL")]
public class EnumRecordItemTemplate : RecordItemTemplate
{
    [XmlAttribute("v")]
    public int DefaultValue { get; set; }

    [XmlAttribute("sort")]
    public string? Sort { get; set; }

    [XmlElement("ETAG")]
    public List<EnumTag> Tags { get; set; } = [];
}

[XmlType("UNSRECORDITEMTMPL")]
public class UnsignedRecordItemTemplate : RecordItemTemplate
{
    [XmlAttribute("v")]
    public string? DefaultValue { get; set; }

    [XmlAttribute("df")]
    public string? Format { get; set; }
}

[XmlType("ETAG")]
public class EnumTag
{
    [XmlAttribute("v")]
    public int Value { get; set; }

    [XmlElement("TUV")]
    public List<General.MultiLanguageText> Texts { get; set; } = [];
}
