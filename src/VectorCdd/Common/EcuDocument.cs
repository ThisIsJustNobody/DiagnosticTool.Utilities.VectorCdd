using DiagnosticTool.Utilities.VectorCdd.DataTypes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static DiagnosticTool.Utilities.VectorCdd.Common.General;
using DiagnosticTool.Utilities.VectorCdd.DefineAttributes;
using DiagnosticTool.Utilities.VectorCdd.RecordTmpls;
using DiagnosticTool.Utilities.VectorCdd.RecordDataTablePool;
using DiagnosticTool.Utilities.VectorCdd.DocTmpl;
using DiagnosticTool.Utilities.VectorCdd.ProtocolServices;
using DiagnosticTool.Utilities.VectorCdd.Authors;
using DiagnosticTool.Utilities.VectorCdd.ExtStorageItems;
using DiagnosticTool.Utilities.VectorCdd.TargetGroups;
using DiagnosticTool.Utilities.VectorCdd.DiagnosticClassTemplates;
using DiagnosticTool.Utilities.VectorCdd.HistItems;
using DiagnosticTool.Utilities.VectorCdd.AttrCats;
using DiagnosticTool.Utilities.VectorCdd.DataIdentities;

namespace DiagnosticTool.Utilities.VectorCdd.Common;

[XmlType("ECUDOC")]
public class EcuDocument : CanDelaBase
{
    #region 定义

    public class ExtStorageItemsNode
    {
        [XmlElement("EXTSTORAGEITEM")]
        public List<ExtStorageItem> ExtStorageItems { get; set; } = [];
    }

    public class QualGenOptionsNode
    {
        [XmlAttribute(DataType = "string", AttributeName = "case")]
        public string? Case { get; set; } = string.Empty;

        [XmlAttribute(DataType = "int", AttributeName = "minLen")]
        public int MinLength { get; set; }

        [XmlAttribute(DataType = "int", AttributeName = "maxLen")]
        public int MaxLength { get; set; }
    }

    public class DefineAttributesNode
    {
        [XmlElement("DATAOBJATTS")]
        public BaseAttributes DataObjectAttributes { get; set; } = new();

        [XmlElement("DATATYPEATTS")]
        public BaseAttributes DataTypeAttributes { get; set; } = new();

        [XmlElement("DIAGCLASSATTS")]
        public BaseAttributes DiagnosisClassAttributes { get; set; } = new();

        [XmlElement("ECUATTS")]
        public BaseAttributes EcuAttributes { get; set; } = new();

        [XmlElement("JOBATTS")]
        public BaseAttributes JobAttributes { get; set; } = new();

        [XmlElement("JOBCNRATTS")]
        public BaseAttributes JobCnrAttributes { get; set; } = new();

        [XmlElement("RECORDATTS")]
        public BaseAttributes RecordAttributes { get; set; } = new();

        [XmlElement("SERVICEATTS")]
        public BaseAttributes ServiceAttributes { get; set; } = new();

        [XmlElement("VARATTS")]
        public BaseAttributes VarAttributes { get; set; } = new();

        [XmlElement("STATEGROUPATTS")]
        public BaseAttributes StateGroupAttributes { get; set; } = new();

        [XmlElement("DCLTMPLATTS")]
        public BaseAttributes DclTmplAttributes { get; set; } = new();

        [XmlElement("DCLSRVTMPLATTS")]
        public BaseAttributes DclSrvTmplAttributes { get; set; } = new();

        [XmlElement("SHPROXYATTS")]
        public BaseAttributes ShproxyAttributes { get; set; } = new();

        [XmlElement("DIDATTS")]
        public BaseAttributes DidAttributes { get; set; } = new();

        [XmlElement("DIAGINSTATTS")]
        public BaseAttributes DiagInstAttributes { get; set; } = new();
    }

    public class AuthorsNode
    {
        [XmlElement("AUTHOR")]
        public List<Author> Authors { get; set; } = [];
    }

    public class TargetGroupsNode
    {
        [XmlElement("TARGETGROUP")]
        public List<TargetGroup> TargetGroups { get; set; } = [];
    }

    public class DataTypesNode
    {
        [XmlElement("IDENT", Type = typeof(Identity))]
        [XmlElement("TEXTTBL", Type = typeof(TextTable))]
        [XmlElement("LINCOMP", Type = typeof(Linear))]
        [XmlElement("STRUCTDT", Type = typeof(Packet))]
        [XmlElement("MUXDT", Type = typeof(Multiplexer))]
        public List<BaseDataType> DataObjects { get; set; } = [];
    }

    public class DataIdentitiesNode
    {
        [XmlElement("DID")]
        public List<DataIdentity> DataIdentities { get; set; } = [];
    }

    public class RecordDataTableNode
    {
        [XmlElement("RECORDDT")]
        public List<RecordDataTable> RecordDataTables { get; set; } = [];
    }

    public class ProtocolServicesNode
    {
        [XmlElement("PROTOCOLSERVICE")]
        public List<ProtocolService> ProtocolServices { get; set; } = [];
    }

    public class DclTmplsNode
    {
        [XmlElement("DCLTMPL")]
        public List<DiagnosticClassTemplate> DiagnosticClassTemplates { get; set; } = [];
    }

    public class AttrCatsNode
    {
        [XmlElement("ATTRCAT")]
        public List<AttrCat> AttrCats { get; set; } = [];
    }

    public class HistItemsNode
    {
        [XmlElement("HISTITEM")]
        public List<HistItem> HistItems { get; set; } = [];
    }

    public class RecordTmplsNode
    {
        [XmlElement("RECORDTMPL")]
        public List<RecordTemplate> RecordTemplates { get; set; } = [];
    }

    #endregion

    [XmlAttribute(DataType = "string", AttributeName = "temploid")]
    public string? TemplOid { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "manufacturer")]
    public string? Manufacturer { get; set; }

    [XmlAttribute(DataType = "string", AttributeName = "mid")]
    public string? Mid { get; set; }

    [XmlElement("EXTSTORAGEITEMS")]
    public ExtStorageItemsNode ExtStorageItems { get; set; } = new();

    [XmlElement("PROTOCOLSTANDARD")]
    public string? ProtocolStandard { get; set; } = string.Empty;

    [XmlElement("SPECOWNER")]
    public string? SpecOwner { get; set; } = string.Empty;

    [XmlElement("DTID")]
    public string? DtId { get; set; } = string.Empty;

    [XmlElement("QUALGENOPTIONS")]
    public QualGenOptionsNode QualGenOptions { get; set; } = new();

    [XmlElement("ATTRCATS")]
    public AttrCatsNode AttrCats { get; set; } = new();

    [XmlElement("DEFATTS")]
    public DefineAttributesNode DefineAttributes { get; set; } = new();

    [XmlElement("AUTHORS")]
    public AuthorsNode Authors { get; set; } = new();

    [XmlElement("HISTITEMS")]
    public HistItemsNode HistItems { get; set; } = new();

    [XmlElement("TARGETGROUPS")]
    public TargetGroupsNode TargetGroups { get; set; } = new();

    public class NegResCodesNode
    {
        [XmlElement("NEGRESCODE")]
        public List<NegResCode> NegResCodes { get; set; } = [];
    }

    public class StateGroupsNode
    {
        [XmlElement("STATEGROUP")]
        public List<StateGroup> StateGroups { get; set; } = [];
    }

    [XmlElement("NEGRESCODES")]
    public NegResCodesNode NegResCodes { get; set; } = new();

    [XmlElement("STATEGROUPS")]
    public StateGroupsNode StateGroups { get; set; } = new();

    [XmlElement("VCKMGR")]
    public VersionCheckManager? VersionCheckManager { get; set; }

    [XmlElement("DATATYPES")]
    public DataTypesNode DataTypes { get; set; } = new();

    [XmlElement("DOCTMPL")]
    public DocTemplate? DocTemplate { get; set; }

    [XmlElement("RECORDTMPLS")]
    public RecordTmplsNode RecordTemplates { get; set; } = new();

    [XmlElement("RECORDDTPOOL")]
    public RecordDataTableNode RecordDataTablePool { get; set; } = new();

    [XmlElement("DTCSTATUSMASK")]
    public DtcStatusMask? DtcStatusMask { get; set; }

    [XmlElement("UNSUPPSRVNEG")]
    public UnsupportedServiceNeg? UnsupportedServiceNeg { get; set; }

    [XmlElement("DIDS")]
    public DataIdentitiesNode DataIdentities { get; set; } = new();

    [XmlElement("PROTOCOLSERVICES")]
    public ProtocolServicesNode ProtocolServices { get; set; } = new();

    [XmlElement("DCLTMPLS")]
    public DclTmplsNode DiagnosticClassTemplates { get; set; } = new();

    [XmlElement("ECU")]
    public ECU.ECU Ecu { get; set; } = new();

    public override void SetCanDelaReference(CanDela? canDela = null)
    {
        base.SetCanDelaReference(canDela);
        DataTypes.DataObjects.ForEach(x => x.SetCanDelaReference(canDela));
        DataIdentities.DataIdentities.ForEach(x => x.SetCanDelaReference(canDela));
        RecordDataTablePool.RecordDataTables.ForEach(x => x.SetCanDelaReference(canDela));
        ProtocolServices.ProtocolServices.ForEach(x => x.SetCanDelaReference(canDela));
        DiagnosticClassTemplates.DiagnosticClassTemplates.ForEach(x => x.SetCanDelaReference(canDela));
        Ecu.SetCanDelaReference(canDela);
    }
}
