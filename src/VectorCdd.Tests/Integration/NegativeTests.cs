using DiagnosticTool.Utilities.VectorCdd.DataTypes;

using DiagnosticTool.Utilities.VectorCdd.TestHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.XmlParser;

namespace DiagnosticTool.Utilities.VectorCdd.Integration;

[TestClass]
public class NegativeTests
{
    /// <summary>
    /// Identity 输入空字节不应抛异常
    /// </summary>
    [TestMethod]
    public void Identity_Convert_EmptyBytes_HandlesGracefully()
    {
        var xml = """
            <IDENT>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </IDENT>
            """;
        var identity = xml.DeserializeXmlToObject<Identity>();
        var result = identity.Convert([]);

        // 不应抛异常，结果可能是不成功状态
        Assert.IsNotNull(result);
    }

    /// <summary>
    /// Linear 值低于下限应返回 UndefinedRange 错误
    /// </summary>
    [TestMethod]
    public void Linear_ValueBelowLowerLimit_ReturnsUndefinedRange()
    {
        var xml = """
            <LINCOMP>
            <CVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
            <PVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
            <COMP f='1' div='1' o='0' s='10' e='100'/>
            </LINCOMP>
            """;
        var linear = xml.DeserializeXmlToObject<Linear>();
        // 值 5 < 下限 10
        var result = linear.Convert(Convert.FromHexString("0005"));

        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual("UndefinedRange", result.Error.ErrorType);
    }

    /// <summary>
    /// Linear 值高于上限应返回 UndefinedRange 错误
    /// </summary>
    [TestMethod]
    public void Linear_ValueAboveUpperLimit_ReturnsUndefinedRange()
    {
        var xml = """
            <LINCOMP>
            <CVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
            <PVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
            <COMP f='1' div='1' o='0' s='10' e='100'/>
            </LINCOMP>
            """;
        var linear = xml.DeserializeXmlToObject<Linear>();
        // 值 200 > 上限 100
        var result = linear.Convert(Convert.FromHexString("00C8"));

        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual("UndefinedRange", result.Error.ErrorType);
    }

    /// <summary>
    /// TextTable 值不在任何映射范围
    /// </summary>
    [TestMethod]
    public void TextTable_OutOfRangeValue_ReturnsUnsuccessful()
    {
        var xml = """
            <TEXTTBL>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <TEXTMAP s='0' e='5'>
            <TEXT><TUV xml:lang='en-US'>Low</TUV></TEXT>
            </TEXTMAP>
            <TEXTMAP s='10' e='15'>
            <TEXT><TUV xml:lang='en-US'>High</TUV></TEXT>
            </TEXTMAP>
            </TEXTTBL>
            """;
        var tt = xml.DeserializeXmlToObject<TextTable>();
        // 值 7 不在 0-5 或 10-15 范围内
        var result = tt.Convert(Convert.FromHexString("07"));

        Assert.IsFalse(result.IsSuccessful);
    }

    /// <summary>
    /// 无效 XML 反序列化应抛异常
    /// </summary>
    [TestMethod]
    public void Deserialize_InvalidXml_ThrowsException()
    {
        var invalidXml = "<INVALID><UNCLOSED>";
        Assert.ThrowsExactly<InvalidOperationException>(
            () => invalidXml.DeserializeXmlToObject<CanDela>());
    }

    /// <summary>
    /// Packet 输入过短字节应能处理
    /// </summary>
    [TestMethod]
    public void Identity_Convert_TooShortBytes_HandlesGracefully()
    {
        var xml = """
            <IDENT>
            <CVALUETYPE bl='32' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='4' maxsz='4'/>
            <PVALUETYPE bl='32' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='4' maxsz='4'/>
            </IDENT>
            """;
        var identity = xml.DeserializeXmlToObject<Identity>();
        // 只提供 1 字节，但需要 4 字节
        var result = identity.Convert(Convert.FromHexString("AB"));

        // 不应抛异常
        Assert.IsNotNull(result);
    }
}
