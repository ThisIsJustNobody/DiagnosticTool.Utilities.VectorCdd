using System.Collections.Generic;
using System.Xml.Serialization;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

[XmlType("STATEGROUP")]
public class StateGroup : General
{
    [XmlElement("STATE")]
    public List<State> States { get; set; } = [];
}

[XmlType("STATE")]
public class State : General
{
}
