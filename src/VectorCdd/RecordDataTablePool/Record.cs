using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static DiagnosticTool.Utilities.VectorCdd.Common.General;

namespace DiagnosticTool.Utilities.VectorCdd.RecordDataTablePool;

[XmlType("RECORD")]
public class Record
{
    [XmlAttribute(DataType = "unsignedInt", AttributeName = "v")]
    public uint Value { get; set; }

    [XmlArray("TEXT")]
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
}