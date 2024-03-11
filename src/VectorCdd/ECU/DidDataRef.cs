using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.ECU
{
    [XmlType("DIDDATAREF")]
    public class DidDataRef : General
    {
        [XmlAttribute(DataType = "string", AttributeName = "didRef")]
        public string DidRef { get; set; } = string.Empty;
    }
}
