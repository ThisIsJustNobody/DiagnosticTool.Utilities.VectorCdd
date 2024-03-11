using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticTool.Utilities.VectorCdd.Common
{
    public class CanDelaBase
    {
        public CanDela? CanDela { get; set; }

        public virtual void SetCanDelaReference(CanDela? canDela = null)
        {
            canDela ??= CanDela;
            CanDela = canDela ?? throw new InvalidOperationException(
                $"{GetType().Name}.SetCanDelaReference: canDela is null and CanDela is not yet set.");
        }
    }
}
