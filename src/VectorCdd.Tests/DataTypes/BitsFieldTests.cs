using DiagnosticTool.Utilities.VectorCdd.XmlParser;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes;

[TestClass]
public class BitsFieldTests
{
    /// <summary>
    /// 验证 TotalBitLengthRange 不再抛出 NotImplementedException
    /// </summary>
    [TestMethod]
    public void TotalBitLengthRange_DoesNotThrow()
    {
        var xml = """
            <STRUCT>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='field' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='field' sz='no' minsz='1' maxsz='1'/>
            <DATAOBJ>
              <NAME><TUV xml:lang='en-US'>Bit 0</TUV></NAME>
              <CVALUETYPE bl='1' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
              <PVALUETYPE bl='1' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </DATAOBJ>
            <DATAOBJ>
              <NAME><TUV xml:lang='en-US'>Bit 1</TUV></NAME>
              <CVALUETYPE bl='1' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
              <PVALUETYPE bl='1' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </DATAOBJ>
            </STRUCT>
            """;
        var bf = xml.DeserializeXmlToObject<BitsField>();
        var range = bf.TotalBitLengthRange;

        Assert.AreEqual(2, range.Item1);
        Assert.AreEqual(2, range.Item2);
    }

    /// <summary>
    /// BitsField 位级提取
    /// </summary>
    [TestMethod]
    public void Convert_BitFieldExtractsCorrectly()
    {
        var xml = """
            <STRUCT>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <DATAOBJ>
              <NAME><TUV xml:lang='en-US'>High Nibble</TUV></NAME>
              <CVALUETYPE bl='4' bo='21' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
              <PVALUETYPE bl='4' bo='21' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </DATAOBJ>
            <DATAOBJ>
              <NAME><TUV xml:lang='en-US'>Low Nibble</TUV></NAME>
              <CVALUETYPE bl='4' bo='21' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
              <PVALUETYPE bl='4' bo='21' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </DATAOBJ>
            </STRUCT>
            """;
        var bf = xml.DeserializeXmlToObject<BitsField>();
        var result = bf.Convert(Convert.FromHexString("AB"));

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccessful);
        Assert.IsInstanceOfType(result.PhysicalResult.Value, typeof(System.Collections.Generic.List<BaseDataType.ConvertResult>));
        var children = (System.Collections.Generic.List<BaseDataType.ConvertResult>)result.PhysicalResult.Value!;
        Assert.AreEqual(2, children.Count);
    }

    /// <summary>
    /// BitsField 包含 Gap 位的情况
    /// </summary>
    [TestMethod]
    public void Convert_WithGapBits_ProcessesCorrectly()
    {
        var xml = """
            <STRUCT>
            <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='1' maxsz='1'/>
            <GAPDATAOBJ bl='3'>
              <NAME><TUV xml:lang='en-US'>(reserved)</TUV></NAME>
            </GAPDATAOBJ>
            <DATAOBJ>
              <NAME><TUV xml:lang='en-US'>Value</TUV></NAME>
              <CVALUETYPE bl='5' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
              <PVALUETYPE bl='5' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
            </DATAOBJ>
            </STRUCT>
            """;
        var bf = xml.DeserializeXmlToObject<BitsField>();
        var result = bf.Convert(Convert.FromHexString("FF"));

        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccessful);
        var children = (System.Collections.Generic.List<BaseDataType.ConvertResult>)result.PhysicalResult.Value!;
        Assert.AreEqual(2, children.Count);
    }
}
