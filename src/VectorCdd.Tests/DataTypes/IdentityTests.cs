using DiagnosticTool.Utilities.VectorCdd.XmlParser;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[TestClass]
public class IdentityTests
{
    /// <summary>
    /// Identity 透传：输入字节经 Intel 字节序处理后显示为 hex
    /// bo='12'(Intel) 会翻转字节序，hex 格式带 0x 前缀
    /// </summary>
    [TestMethod]
    public void Convert_PassesThroughUnchanged()
    {
        var xml = """
            <IDENT>
            <CVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='2' maxsz='2'/>
            <PVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='2' maxsz='2'/>
            </IDENT>
            """;
        var identity = xml.DeserializeXmlToObject<Identity>();
        var result = identity.Convert(Convert.FromHexString("1234"));

        Assert.IsTrue(result.IsSuccessful);
        // bo='12' (Intel) 翻转字节: 0x1234 -> 字节序翻转 -> 显示为 0x3412
        Assert.AreEqual("0x3412", result.PhysicalResult.ValueString);
    }

    /// <summary>
    /// Identity 十进制显示格式
    /// </summary>
    [TestMethod]
    public void Convert_DecimalFormat_ReturnsDecimal()
    {
        var xml = """
            <IDENT>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </IDENT>
            """;
        var identity = xml.DeserializeXmlToObject<Identity>();
        var result = identity.Convert(Convert.FromHexString("42"));

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("66", result.PhysicalResult.ValueString);
    }

    /// <summary>
    /// Identity 带位掩码 bm='255' 不改变 0xFF 的值
    /// </summary>
    [TestMethod]
    public void Convert_WithBitMask_AppliesCorrectly()
    {
        var xml = """
            <IDENT bm='255'>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </IDENT>
            """;
        var identity = xml.DeserializeXmlToObject<Identity>();
        var result = identity.Convert(Convert.FromHexString("FF"));

        Assert.IsTrue(result.IsSuccessful);
        // bl='8' 表示 1 字节，hex 格式带 0x 前缀，bm='255' 即 0xFF 掩码不改变值
        Assert.AreEqual("0x00FF", result.PhysicalResult.ValueString);
    }

    /// <summary>
    /// Identity 多字节 Intel 字节序：bo='12' 翻转字节序
    /// </summary>
    [TestMethod]
    public void Convert_MultiByteIntel_ReturnsCorrectValue()
    {
        var xml = """
            <IDENT>
            <CVALUETYPE bl='32' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='4' maxsz='4'/>
            <PVALUETYPE bl='32' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='4' maxsz='4'/>
            </IDENT>
            """;
        var identity = xml.DeserializeXmlToObject<Identity>();
        var result = identity.Convert(Convert.FromHexString("01020304"));

        Assert.IsTrue(result.IsSuccessful);
        // bo='12' (Intel) 翻转字节序: 01 02 03 04 -> 04 03 02 01，hex 带 0x 前缀
        Assert.AreEqual("0x04030201", result.PhysicalResult.ValueString);
    }
}
