using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static DiagnosticTool.Utilities.VectorCdd.Common.General;

namespace DiagnosticTool.Utilities.VectorCdd.ExtStorageItems;

[XmlType("EXTSTORAGEITEM")]
public class ExtStorageItem
{
    public class ExtDoc
    {
        public class FileContents
        {
            [XmlAttribute(DataType = "unsignedInt", AttributeName = "len")]
            public uint Length { get; set; }

            [XmlText(DataType = "base64Binary")]
            public byte[] Content { get; set; } = [];
        }

        [XmlElement("FILENAME")]
        public string FileName { get; set; } = string.Empty;

        [XmlElement("FILECONTENTS")]
        public FileContents File { get; set; } = new();

        public void Save(string directory, string? filename = null)
        {
            filename ??= Path.GetFileName(FileName);
            var fullPath = Path.Combine(directory, filename);
            System.IO.File.WriteAllBytes(fullPath, File.Content);
        }
    }

    [XmlArray("CAPTION")]
    public List<MultiLanguageText> Captions { get; set; } = [];

    [XmlElement("EXTDOC")]
    public List<ExtDoc> ExtDocument { get; set; } = [];
}
