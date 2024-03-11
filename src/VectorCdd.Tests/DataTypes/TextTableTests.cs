using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.XmlParser;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[TestClass]
public class TextTableTests
{
    private const string TextTableXmlTemplate = """
        <TEXTTBL {0}>
        <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
        <PVALUETYPE bl='16' bo='21' enc='utf' sig='0' df='text' qty='field' sz='no' minsz='0' maxsz='65535'/>
        <TEXTMAP s='0' e='0'>
        <TEXT><TUV xml:lang='en-US'>Normal</TUV></TEXT>
        </TEXTMAP>
        <TEXTMAP s='1' e='1'>
        <TEXT><TUV xml:lang='en-US'>Transport</TUV></TEXT>
        </TEXTMAP>
        <TEXTMAP s='2' e='2'>
        <TEXT><TUV xml:lang='en-US'>Factory</TUV></TEXT>
        </TEXTMAP>
        </TEXTTBL>
        """;

    /// <summary>
    /// 测试 TextTable 值映射：输入 0x00 应匹配 "Normal"
    /// </summary>
    [TestMethod]
    public void Convert_MatchingValue0_ReturnsNormal()
    {
        var xml = string.Format(TextTableXmlTemplate, "");
        var tt = xml.DeserializeXmlToObject<TextTable>();
        var result = tt.Convert(Convert.FromHexString("00"));

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("Normal", result.PhysicalResult.ValueString);
    }

    /// <summary>
    /// 测试 TextTable 值映射：输入 0x01 应匹配 "Transport"
    /// </summary>
    [TestMethod]
    public void Convert_MatchingValue1_ReturnsTransport()
    {
        var xml = string.Format(TextTableXmlTemplate, "");
        var tt = xml.DeserializeXmlToObject<TextTable>();
        var result = tt.Convert(Convert.FromHexString("01"));

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("Transport", result.PhysicalResult.ValueString);
    }

    /// <summary>
    /// 测试 TextTable 值映射：输入 0x02 应匹配 "Factory"
    /// </summary>
    [TestMethod]
    public void Convert_MatchingValue2_ReturnsFactory()
    {
        var xml = string.Format(TextTableXmlTemplate, "");
        var tt = xml.DeserializeXmlToObject<TextTable>();
        var result = tt.Convert(Convert.FromHexString("02"));

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("Factory", result.PhysicalResult.ValueString);
    }

    /// <summary>
    /// 测试 TextTable 值不在任何范围内：输入 0xFF 不匹配任何 TextMap
    /// </summary>
    [TestMethod]
    public void Convert_NoMatchingRange_ReturnsUnsuccessful()
    {
        var xml = string.Format(TextTableXmlTemplate, "");
        var tt = xml.DeserializeXmlToObject<TextTable>();
        var result = tt.Convert(Convert.FromHexString("FF"));

        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual("OutOfRange", result.Error.ErrorType);
    }

    /// <summary>
    /// 测试 TextTable 范围映射：s=0 e=5 应匹配 0 到 5 之间的任何值
    /// </summary>
    [TestMethod]
    public void Convert_RangeMapping_MatchesValueInRange()
    {
        var xml = """
            <TEXTTBL>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='16' bo='21' enc='utf' sig='0' df='text' qty='field' sz='no' minsz='0' maxsz='65535'/>
            <TEXTMAP s='0' e='5'>
            <TEXT><TUV xml:lang='en-US'>Low Range</TUV></TEXT>
            </TEXTMAP>
            <TEXTMAP s='10' e='20'>
            <TEXT><TUV xml:lang='en-US'>High Range</TUV></TEXT>
            </TEXTMAP>
            </TEXTTBL>
            """;
        var tt = xml.DeserializeXmlToObject<TextTable>();
        var result = tt.Convert(Convert.FromHexString("03"));

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("Low Range", result.PhysicalResult.ValueString);
    }

    /// <summary>
    /// 测试 TextTable 带位掩码
    /// </summary>
    [TestMethod]
    public void Convert_WithBitMask_AppliesCorrectly()
    {
        var xml = """
            <TEXTTBL bm='15'>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='16' bo='21' enc='utf' sig='0' df='text' qty='field' sz='no' minsz='0' maxsz='65535'/>
            <TEXTMAP s='0' e='0'>
            <TEXT><TUV xml:lang='en-US'>Off</TUV></TEXT>
            </TEXTMAP>
            <TEXTMAP s='1' e='1'>
            <TEXT><TUV xml:lang='en-US'>On</TUV></TEXT>
            </TEXTMAP>
            </TEXTTBL>
            """;
        var tt = xml.DeserializeXmlToObject<TextTable>();
        // 0xF1 & 0x0F = 0x01 -> "On"
        var rawData = Convert.FromHexString("F1");
        rawData = tt.ApplyBitMask(rawData);
        var result = tt.Convert(rawData);

        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("On", result.PhysicalResult.ValueString);
    }
}
