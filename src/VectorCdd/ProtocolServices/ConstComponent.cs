using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;

namespace DiagnosticTool.Utilities.VectorCdd.ProtocolServices
{
    [XmlType("CONSTCOMP")]
    public class ConstComponent : BaseComponent
    {
        [XmlAttribute(DataType = "unsignedInt", AttributeName = "v")]
        public uint Value { get; set; }

        public byte[] Data => ComponentBitLength switch
        {
            <= 8 => [(byte)Value],
            16 => BitConverter.GetBytes((ushort)Value).Reverse().ToArray(),
            32 => BitConverter.GetBytes(Value).Reverse().ToArray(),
            _ => throw new InvalidOperationException($"Invalid bit length: {ComponentBitLength}")
        };
    }
}
