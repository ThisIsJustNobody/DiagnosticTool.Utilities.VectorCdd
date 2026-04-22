using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.DataTypes;
using DiagnosticTool.Utilities.VectorCdd.XmlParser;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

using DiagnosticTool.Utilities.VectorCdd.TestHelpers;

namespace DiagnosticTool.Utilities.VectorCdd.Integration;

[TestClass]
[TestCategory("Integration")]
public class EndToEndTests
{
    /// <summary>
    /// 所有 .cdd 文件都能成功反序列化
    /// </summary>
    [TestMethod]
    public void ParseAllCddFiles_SuccessfullyDeserializes()
    {
        foreach (var (name, candela) in CddTestData.All)
        {
            Assert.IsNotNull(candela, $"{name}.cdd 反序列化返回 null");
            Assert.IsNotNull(candela.EcuDocument, $"{name}.cdd 的 EcuDocument 为 null");
            Assert.IsFalse(string.IsNullOrEmpty(candela.Version), $"{name}.cdd 的 Version 为空");
        }
    }

    /// <summary>
    /// 所有 .cdd 文件都包含数据类型定义
    /// </summary>
    [TestMethod]
    public void ParseAllCddFiles_HasDataTypes()
    {
        foreach (var (name, candela) in CddTestData.All)
        {
            var dataTypes = candela.EcuDocument.DataTypes.DataObjects;
            Assert.IsTrue(dataTypes.Count > 0, $"{name}.cdd 没有数据类型定义");
        }
    }

    /// <summary>
    /// 所有 .cdd 文件都包含协议服务
    /// </summary>
    [TestMethod]
    public void ParseAllCddFiles_HasProtocolServices()
    {
        foreach (var (name, candela) in CddTestData.All)
        {
            var services = candela.EcuDocument.ProtocolServices.ProtocolServices;
            Assert.IsTrue(services.Count >= 100,
                $"{name}.cdd 的协议服务数量 ({services.Count}) 少于预期 (>=100)");
        }
    }

    /// <summary>
    /// 所有 .cdd 文件都包含 ECU 变体
    /// </summary>
    [TestMethod]
    public void ParseAllCddFiles_HasEcuVariants()
    {
        foreach (var (name, candela) in CddTestData.All)
        {
            var variants = candela.EcuDocument.Ecu.Variants;
            Assert.IsTrue(variants.Count > 0, $"{name}.cdd 没有 ECU 变体");
        }
    }

    /// <summary>
    /// FLR3.cdd 包含 DID
    /// </summary>
    [TestMethod]
    public void ParseFlr3_HasDataIdentities()
    {
        var candela = CddTestData.Flr3;
        var dids = candela.EcuDocument.DataIdentities.DataIdentities;
        Assert.IsTrue(dids.Count > 0, "FLR3.cdd 没有 DID");
        Assert.IsTrue(dids.Count >= 80, $"FLR3.cdd DID 数量 ({dids.Count}) 少于预期");
    }

    /// <summary>
    /// FLR3.cdd 的数据类型包含所有五种类型
    /// </summary>
    [TestMethod]
    public void ParseFlr3_HasAllDataTypeTypes()
    {
        var candela = CddTestData.Flr3;
        var types = candela.EcuDocument.DataTypes.DataObjects;

        Assert.IsTrue(types.Any(t => t is Identity), "缺少 Identity 类型");
        Assert.IsTrue(types.Any(t => t is DataTypes.TextTable), "缺少 TextTable 类型");
        Assert.IsTrue(types.Any(t => t is Linear), "缺少 Linear 类型");
        Assert.IsTrue(types.Any(t => t is Packet), "缺少 Packet 类型");
        Assert.IsTrue(types.Any(t => t is Multiplexer), "缺少 Multiplexer 类型");
    }

    /// <summary>
    /// XML 序列化往返一致性：验证独立 Linear 对象的序列化/反序列化
    /// （完整 CanDela 对象含循环引用不支持直接序列化）
    /// </summary>
    [TestMethod]
    public void SerializeDeserialize_RoundTrip_Consistent()
    {
        var xml = """
            <LINCOMP>
            <CVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
            <PVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
            <COMP f='3' div='2' o='7'/>
            </LINCOMP>
            """;
        var linear = xml.DeserializeXmlToObject<Linear>();
        var reserialized = linear.SerializeObjectToXml();
        var restored = reserialized.DeserializeXmlToObject<Linear>();

        Assert.AreEqual(linear.ComponentValue.Factor, restored.ComponentValue.Factor);
        Assert.AreEqual(linear.ComponentValue.Divisor, restored.ComponentValue.Divisor);
        Assert.AreEqual(linear.ComponentValue.Offset, restored.ComponentValue.Offset);
    }
}
