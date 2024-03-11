using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiagnosticTool.Utilities.VectorCdd.DataTypes;

namespace DiagnosticTool.Utilities.VectorCdd.Common
{
    public interface IConvert
    {
        public BaseDataType.ConvertResult Convert(byte[] rawData);
    }
}
