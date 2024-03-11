using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.DiagnosticClassTemplates;

namespace DiagnosticTool.Utilities.VectorCdd.ECU
{
    [XmlType("STATICVALUE")]
    public class StaticValue : General
    {
        [XmlAttribute(DataType = "string", AttributeName = "shstaticref")]
        public string? ShStaticRef { get; set; }

        [XmlAttribute(DataType = "unsignedInt", AttributeName = "v")]
        public uint Value { get; set; }

        public ShStatic? ShStatic =>
            CanDela
                ?.EcuDocument.DiagnosticClassTemplates.DiagnosticClassTemplates.Select(d =>
                    d.ShStatic
                )
                .FirstOrDefault(s => s.Id == ShStaticRef);

        public byte[] Data
        {
            get
            {
                var bitLength = ShStatic?.StaticCompRefs.FirstOrDefault()?.StaticComponent?.TotalBitLengthRange.Item2;
                if (bitLength == null)
                {
                    return [];
                }
                return bitLength switch
                {
                    <= 8 => [(byte)Value],
                    16 => BitConverter.GetBytes((ushort)Value).Reverse().ToArray(),
                    32 => BitConverter.GetBytes(Value).Reverse().ToArray(),
                    _ => throw new InvalidOperationException($"Invalid bit length: {bitLength}")
                };
            }
        }
            
    }
}
