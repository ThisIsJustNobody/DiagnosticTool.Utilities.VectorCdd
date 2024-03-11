using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.DataTypes;

namespace DiagnosticTool.Utilities.VectorCdd.ProtocolServices
{
    public class BaseComponent : BaseDataType
    {
        [XmlAttribute(DataType = "boolean", AttributeName = "must")]
        public bool Must { get; set; }

        public bool ShouldSerializeMust()
        {
            // 当 Must 的值不是默认值 false 时，返回 true，表示需要序列化
            return Must != false;
        }

        [XmlAttribute(DataType = "unsignedInt", AttributeName = "bl")]
        public uint ComponentBitLength { get; set; }
    }
}
