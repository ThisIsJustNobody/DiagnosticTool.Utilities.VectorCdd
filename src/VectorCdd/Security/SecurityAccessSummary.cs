using System.Collections.Generic;

namespace DiagnosticTool.Utilities.VectorCdd.Security;

public class SecurityAccessSummary
{
    public SecurityAccessProtocolServices ProtocolServices { get; set; } = new();
    public List<SecurityLevel> Levels { get; set; } = [];
}

public class SecurityAccessProtocolServices
{
    public ProtocolServiceSummary? RequestSeed { get; set; }
    public ProtocolServiceSummary? SendKey { get; set; }
}

public class ProtocolServiceSummary
{
    public string? Qual { get; set; }
    public string? Name { get; set; }
    public byte SidRequest { get; set; }
    public byte SidPositive { get; set; }
}

public class SecurityLevel
{
    public string? Name { get; set; }
    public bool Active { get; set; }
    public byte RequestSeedSubFunction { get; set; }
    public byte SendKeySubFunction { get; set; }
    public string? SeedDataTypeName { get; set; }
    public int? SeedByteSize { get; set; }
    public string? KeyDataTypeName { get; set; }
    public int? KeyByteSize { get; set; }
    public int? MaxAttemptsToDelay { get; set; }
    public int? MaxAttemptsToLock { get; set; }
    public int? DelayTimeMs { get; set; }
    public string? Sessions { get; set; }
    public string? Transitions { get; set; }
}
