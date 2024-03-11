using System.Linq;

using DiagnosticTool.Utilities.VectorCdd.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DiagnosticTool.Utilities.VectorCdd.TestHelpers;

namespace DiagnosticTool.Utilities.VectorCdd.Security;

[TestClass]
public class SecurityAccessAnalyzerTests
{
    [TestMethod]
    public void Analyze_Sweet500_FindsSecurityLevels()
    {
        var canDela = CddTestData.Sweet500;
        var analyzer = new SecurityAccessAnalyzer(canDela);
        var summary = analyzer.Analyze();

        Assert.IsNotNull(summary);
        Assert.IsTrue(summary.Levels.Count >= 3, $"Expected at least 3 security levels, got {summary.Levels.Count}");

        // Level 1: subfn 0x01/0x02, seed 256 bytes
        var level1 = summary.Levels.First(l => l.RequestSeedSubFunction == 0x01);
        Assert.AreEqual(0x01, level1.RequestSeedSubFunction);
        Assert.AreEqual(0x02, level1.SendKeySubFunction);
        Assert.AreEqual(256, level1.SeedByteSize);
        Assert.AreEqual(256, level1.KeyByteSize);
        Assert.IsTrue(level1.Active, "Level 1 should be active");
        Assert.AreEqual(10, level1.MaxAttemptsToDelay);
        Assert.AreEqual(3, level1.MaxAttemptsToLock);
        Assert.AreEqual(960000, level1.DelayTimeMs);
    }

    [TestMethod]
    public void Analyze_Sweet500_Level2Inactive()
    {
        var canDela = CddTestData.Sweet500;
        var analyzer = new SecurityAccessAnalyzer(canDela);
        var summary = analyzer.Analyze();

        var level2 = summary.Levels.FirstOrDefault(l => l.RequestSeedSubFunction == 0x03);
        Assert.IsNotNull(level2, "Level 2 (subfn 0x03) should exist");
        Assert.IsFalse(level2.Active, "Level 2 should be inactive (act=0)");
    }

    [TestMethod]
    public void Analyze_Sweet500_ProtocolServices()
    {
        var canDela = CddTestData.Sweet500;
        var analyzer = new SecurityAccessAnalyzer(canDela);
        var summary = analyzer.Analyze();

        Assert.IsNotNull(summary.ProtocolServices.RequestSeed);
        Assert.AreEqual("SA_RS", summary.ProtocolServices.RequestSeed.Qual);
        Assert.AreEqual(0x27, summary.ProtocolServices.RequestSeed.SidRequest);

        Assert.IsNotNull(summary.ProtocolServices.SendKey);
        Assert.AreEqual("SA_SK", summary.ProtocolServices.SendKey.Qual);
        Assert.AreEqual(0x27, summary.ProtocolServices.SendKey.SidRequest);
    }

    [TestMethod]
    public void Analyze_AllCddFiles_DoesNotThrow()
    {
        foreach (var (name, canDela) in CddTestData.All)
        {
            var analyzer = new SecurityAccessAnalyzer(canDela);
            var summary = analyzer.Analyze();
            Assert.IsNotNull(summary, $"Analysis for {name} should not return null");
        }
    }

    [TestMethod]
    public void Analyze_CddFilesWithoutSecurity_ReturnsEmptyLevels()
    {
        // Some CDD files may not have security access definitions
        foreach (var (name, canDela) in CddTestData.All)
        {
            var analyzer = new SecurityAccessAnalyzer(canDela);
            var summary = analyzer.Analyze();
            // Should not throw, even if no security definitions exist
            Assert.IsNotNull(summary.Levels, $"{name}: Levels list should not be null");
        }
    }
}
