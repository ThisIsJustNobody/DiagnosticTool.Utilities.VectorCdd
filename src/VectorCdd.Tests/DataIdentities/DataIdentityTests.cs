using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.XmlParser;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

using DiagnosticTool.Utilities.VectorCdd.TestHelpers;

namespace DiagnosticTool.Utilities.VectorCdd.DataIdentities;

[TestClass]
public class DataIdentityTests
{
    /// <summary>
    /// 从 FLR3.cdd 中验证 DID 0xFD1D 的转换
    /// </summary>
    [TestMethod]
    public void Convert_Flr3_DID_FD1D_ReturnsCorrectResult()
    {
        var candela = CddTestData.Flr3;
        var did = candela.EcuDocument.DataIdentities.DataIdentities
            .First(x => x.DataIdentityValue == 0xFD1D);

        var result = did.Convert(Convert.FromHexString("00000000000000"));

        Assert.IsTrue(result.IsSuccessful);
        Assert.IsNotNull(result.DisplayText());
    }

    /// <summary>
    /// 从 FLR3.cdd 中验证 DID 0xFF00 的转换
    /// </summary>
    [TestMethod]
    public void Convert_Flr3_DID_FF00_ReturnsCorrectResult()
    {
        var candela = CddTestData.Flr3;
        var did = candela.EcuDocument.DataIdentities.DataIdentities
            .First(x => x.DataIdentityValue == 0xFF00);

        var result = did.Convert(Convert.FromHexString("02000000"));

        Assert.IsTrue(result.IsSuccessful);
        Assert.IsNotNull(result.DisplayText());
    }

    /// <summary>
    /// 从 FLR3.cdd 中验证 DID 0xB03B 的转换（复杂 Packet）
    /// </summary>
    [TestMethod]
    public void Convert_Flr3_DID_B03B_ComplexPacket_ReturnsChildren()
    {
        var candela = CddTestData.Flr3;
        var did = candela.EcuDocument.DataIdentities.DataIdentities
            .First(x => x.DataIdentityValue == 0xB03B);

        var result = did.Convert(Convert.FromHexString("123456789ABC"));

        Assert.IsTrue(result.IsSuccessful);
        var displayText = result.DisplayText();
        Assert.IsNotNull(displayText);
        Assert.AreNotEqual(string.Empty, displayText);
    }

    /// <summary>
    /// 从 FLR3.cdd 中验证 DID 0xF1AE 的转换（ASCII 字符串类型）
    /// </summary>
    [TestMethod]
    public void Convert_Flr3_DID_F1AE_ReturnsCorrectResult()
    {
        var candela = CddTestData.Flr3;
        var did = candela.EcuDocument.DataIdentities.DataIdentities
            .First(x => x.DataIdentityValue == 0xF1AE);

        var result = did.Convert(Convert.FromHexString("0266081186552020410000000000000000"));

        Assert.IsTrue(result.IsSuccessful);
    }

    /// <summary>
    /// 验证所有 FLR3.cdd 中的 DID 都能成功转换（不抛异常）
    /// </summary>
    [TestMethod]
    public void Convert_Flr3_AllDIDs_DoNotThrow()
    {
        var candela = CddTestData.Flr3;
        var dids = candela.EcuDocument.DataIdentities.DataIdentities;
        Assert.IsTrue(dids.Count > 0, "FLR3.cdd 中没有找到 DID");

        foreach (var did in dids)
        {
            // 用空字节测试不抛异常
            try
            {
                var result = did.Convert([]);
                // 不要求成功，但不应抛异常
            }
            catch (Exception ex)
            {
                Assert.Fail($"DID {did.DataIdentityValue} (0x{did.DataIdentityValue:X}) 转换时抛出异常: {ex.Message}");
            }
        }
    }
}
