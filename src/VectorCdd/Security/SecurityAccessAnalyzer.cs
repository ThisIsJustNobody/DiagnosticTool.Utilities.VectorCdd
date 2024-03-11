using System;
using System.Collections.Generic;
using System.Linq;

using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.DefineAttributes;
using DiagnosticTool.Utilities.VectorCdd.DiagnosticClassTemplates;
using DiagnosticTool.Utilities.VectorCdd.ECU;

namespace DiagnosticTool.Utilities.VectorCdd.Security;

public class SecurityAccessAnalyzer
{
    private readonly CanDela _canDela;

    public SecurityAccessAnalyzer(CanDela canDela)
    {
        _canDela = canDela ?? throw new ArgumentNullException(nameof(canDela));
    }

    public SecurityAccessSummary Analyze()
    {
        var summary = new SecurityAccessSummary();

        // 1. Find the sec DCLTMPL
        var secTmpl = FindSecTemplate();
        if (secTmpl == null)
            return summary;

        var requestSrvTmplId = secTmpl.DiagnosticClassServerTemplate
            .FirstOrDefault(d => d.Qualified == "Request")?.Id;
        var sendSrvTmplId = secTmpl.DiagnosticClassServerTemplate
            .FirstOrDefault(d => d.Qualified == "Send")?.Id;

        // 2. Extract protocol service summaries
        summary.ProtocolServices = ExtractProtocolServices(requestSrvTmplId, sendSrvTmplId);

        // 3. Build UNSDEF lookup: QUAL -> id
        var unsdefLookup = BuildUnsdefLookup();

        // 4. Collect all security DIAGINSTs from ECU hierarchy
        var seedInstances = new Dictionary<byte, DiagnosticInstance>();
        var keyInstances = new Dictionary<byte, DiagnosticInstance>();

        foreach (var variant in _canDela.EcuDocument.Ecu.Variants)
        {
            CollectSecurityInstances(variant.DiagnosticObjects, requestSrvTmplId, sendSrvTmplId, seedInstances, keyInstances);
        }

        // 5. Pair seed/key instances into security levels
        var processedKeys = new HashSet<byte>();

        foreach (var (subfn, seedInst) in seedInstances.OrderBy(x => x.Key))
        {
            byte keySubfn = (byte)(subfn + 1);
            keyInstances.TryGetValue(keySubfn, out var keyInst);

            var level = BuildSecurityLevel(seedInst, keyInst, subfn, keySubfn, unsdefLookup);
            summary.Levels.Add(level);

            if (keyInst != null)
                processedKeys.Add(keySubfn);
        }

        // Handle unpaired key instances
        foreach (var (subfn, keyInst) in keyInstances.OrderBy(x => x.Key))
        {
            if (!processedKeys.Contains(subfn))
            {
                var level = BuildSecurityLevel(null, keyInst, 0, subfn, unsdefLookup);
                summary.Levels.Add(level);
            }
        }

        return summary;
    }

    private DiagnosticClassTemplate? FindSecTemplate()
    {
        return _canDela.EcuDocument.DiagnosticClassTemplates.DiagnosticClassTemplates
            .FirstOrDefault(t => t.Class == "sec");
    }

    private SecurityAccessProtocolServices ExtractProtocolServices(string? requestTmplId, string? sendTmplId)
    {
        var result = new SecurityAccessProtocolServices();

        if (requestTmplId != null)
        {
            var dclSrvTmpl = _canDela.EcuDocument.DiagnosticClassTemplates.DiagnosticClassTemplates
                .SelectMany(t => t.DiagnosticClassServerTemplate)
                .FirstOrDefault(d => d.Id == requestTmplId);
            var ps = dclSrvTmpl?.ProtocolService;
            if (ps != null)
            {
                result.RequestSeed = new ProtocolServiceSummary
                {
                    Qual = ps.Qualified,
                    Name = ps.Name,
                    SidRequest = ps.Request?.RequestId ?? 0,
                    SidPositive = ps.PositiveResponse?.RequestId ?? 0,
                };
            }
        }

        if (sendTmplId != null)
        {
            var dclSrvTmpl = _canDela.EcuDocument.DiagnosticClassTemplates.DiagnosticClassTemplates
                .SelectMany(t => t.DiagnosticClassServerTemplate)
                .FirstOrDefault(d => d.Id == sendTmplId);
            var ps = dclSrvTmpl?.ProtocolService;
            if (ps != null)
            {
                result.SendKey = new ProtocolServiceSummary
                {
                    Qual = ps.Qualified,
                    Name = ps.Name,
                    SidRequest = ps.Request?.RequestId ?? 0,
                    SidPositive = ps.PositiveResponse?.RequestId ?? 0,
                };
            }
        }

        return result;
    }

    private Dictionary<string, string> BuildUnsdefLookup()
    {
        var lookup = new Dictionary<string, string>();

        var diagsInstAtts = _canDela.EcuDocument.DefineAttributes.DiagInstAttributes;
        foreach (var def in diagsInstAtts.Defines.OfType<UnsignedDefine>())
        {
            if (def.Qualified != null && def.Id != null)
                lookup[def.Qualified] = def.Id;
        }

        return lookup;
    }

    private void CollectSecurityInstances(
        List<General> diagnosticObjects,
        string? requestTmplId,
        string? sendTmplId,
        Dictionary<byte, DiagnosticInstance> seedInstances,
        Dictionary<byte, DiagnosticInstance> keyInstances)
    {
        foreach (var obj in diagnosticObjects)
        {
            switch (obj)
            {
                case DiagnosticClass diagClass:
                    CollectSecurityInstances(diagClass.DiagInstances.Cast<General>().ToList(), requestTmplId, sendTmplId, seedInstances, keyInstances);
                    break;
                case DiagnosticInstance diagInst:
                    var svc = diagInst.Services.FirstOrDefault();
                    if (svc == null) break;

                    var svcTmplRef = svc.TemplateRef;
                    var subfn = GetSubFunctionValue(diagInst);

                    if (svcTmplRef == requestTmplId && subfn.HasValue)
                        seedInstances[subfn.Value] = diagInst;
                    else if (svcTmplRef == sendTmplId && subfn.HasValue)
                        keyInstances[subfn.Value] = diagInst;
                    break;
            }
        }
    }

    private static byte? GetSubFunctionValue(DiagnosticInstance diagInst)
    {
        var sv = diagInst.StaticValue;
        if (sv != null && sv.Value > 0)
            return (byte)sv.Value;
        return null;
    }

    private SecurityLevel BuildSecurityLevel(
        DiagnosticInstance? seedInst,
        DiagnosticInstance? keyInst,
        byte seedSubfn,
        byte keySubfn,
        Dictionary<string, string> unsdefLookup)
    {
        var level = new SecurityLevel
        {
            Name = seedInst?.Name ?? keyInst?.Name ?? $"Security Level (subfn 0x{seedSubfn:X2})",
            Active = seedInst?.IsActive ?? keyInst?.IsActive ?? false,
            RequestSeedSubFunction = seedSubfn,
            SendKeySubFunction = keySubfn,
            Sessions = seedInst?.Services.FirstOrDefault()?.MayBeExec ?? keyInst?.Services.FirstOrDefault()?.MayBeExec,
        };

        // Extract seed data type info
        if (seedInst != null)
        {
            var seedDtRef = ResolveDataTypeInfo(seedInst);
            level.SeedDataTypeName = seedDtRef.Name;
            level.SeedByteSize = seedDtRef.ByteSize;
        }

        // Extract key data type info
        if (keyInst != null)
        {
            var keyDtRef = ResolveDataTypeInfo(keyInst);
            level.KeyDataTypeName = keyDtRef.Name;
            level.KeyByteSize = keyDtRef.ByteSize;
        }

        // Extract UNS attributes (use key instance primarily, fall back to seed)
        var sourceInst = keyInst ?? seedInst;
        if (sourceInst != null)
        {
            level.MaxAttemptsToDelay = GetUnsInt(sourceInst, unsdefLookup, "MaxAttemptsToDelay");
            level.MaxAttemptsToLock = GetUnsInt(sourceInst, unsdefLookup, "MaxAttemptsToLock");
            level.DelayTimeMs = GetUnsInt(sourceInst, unsdefLookup, "DelayTimeMs");
        }

        // Extract transitions from key instance
        if (keyInst != null)
        {
            level.Transitions = keyInst.Services.FirstOrDefault()?.Trans;
        }

        return level;
    }

    private static (string? Name, int? ByteSize) ResolveDataTypeInfo(DiagnosticInstance diagInst)
    {
        // SIMPLECOMPCONT extends Packet, which has DataObjs children.
        // The DATAOBJ child has the dtref pointing to the actual data type.
        var firstScc = diagInst.SimpleCompConts.FirstOrDefault();
        if (firstScc == null) return (null, null);

        // Look for a child with a DataTypeReferenceId (dtref)
        var childWithDtRef = firstScc.DataObjs.FirstOrDefault(d => d.DataTypeReferenceId != null);
        if (childWithDtRef == null) return (null, null);

        var dtRef = diagInst.CanDela?.EcuDocument.DataTypes.DataObjects
            .FirstOrDefault(d => d.Id == childWithDtRef.DataTypeReferenceId);
        if (dtRef != null)
        {
            var name = dtRef.Name ?? dtRef.Qualified;
            int? byteSize = null;
            if (dtRef.CodeValueType != null)
            {
                var minSz = dtRef.CodeValueType.MinFieldCount;
                var maxSz = dtRef.CodeValueType.MaxFieldCount;
                if (minSz == maxSz && minSz > 0)
                    byteSize = minSz;
                else if (minSz > 0)
                    byteSize = minSz;
            }
            return (name, byteSize);
        }

        return (null, null);
    }

    private static int? GetUnsInt(
        DiagnosticInstance diagInst,
        Dictionary<string, string> unsdefLookup,
        string qualName)
    {
        if (!unsdefLookup.TryGetValue(qualName, out var attrId))
            return null;

        var uns = diagInst.UnsAttributes.FirstOrDefault(u => u.AttributeRef == attrId);
        if (uns?.Value != null && int.TryParse(uns.Value, out var val))
            return val;

        return null;
    }
}
