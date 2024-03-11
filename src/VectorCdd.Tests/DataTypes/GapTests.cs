using DiagnosticTool.Utilities.VectorCdd.XmlParser;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[TestClass]
public class GapTests
{
    /// <summary>
    /// Gap 转换应成功，仅解码代码值
    /// </summary>
    [TestMethod]
    public void Convert_ReturnsSuccessful()
    {
        var xml = """
            <GAPDATAOBJ bl='8'>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </GAPDATAOBJ>
            """;
        var gap = xml.DeserializeXmlToObject<Gap>();
        var result = gap.Convert(Convert.FromHexString("AB"));

        Assert.IsTrue(result.IsSuccessful);
    }

    /// <summary>
    /// Gap 应正确截取指定位长度的字节
    /// </summary>
    [TestMethod]
    public void InterceptBytes_CalculatesCorrectLength()
    {
        var xml = """
            <GAPDATAOBJ bl='8'>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </GAPDATAOBJ>
            """;
        var gap = xml.DeserializeXmlToObject<Gap>();
        var input = Convert.FromHexString("ABCD");
        var intercepted = gap.InterceptBytes(input, out var rest);

        Assert.AreEqual(1, intercepted.Length);
        Assert.AreEqual(0xAB, intercepted[0]);
        Assert.AreEqual(1, rest.Length);
        Assert.AreEqual(0xCD, rest[0]);
    }

    /// <summary>
    /// Gap bl=16 应截取 2 字节
    /// </summary>
    [TestMethod]
    public void InterceptBytes_16Bit_CalculatesCorrectLength()
    {
        var xml = """
            <GAPDATAOBJ bl='16'>
            <CVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='2' maxsz='2'/>
            <PVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='2' maxsz='2'/>
            </GAPDATAOBJ>
            """;
        var gap = xml.DeserializeXmlToObject<Gap>();
        var input = Convert.FromHexString("ABCD1234");
        var intercepted = gap.InterceptBytes(input, out var rest);

        Assert.AreEqual(2, intercepted.Length);
        Assert.AreEqual(2, rest.Length);
    }
}
