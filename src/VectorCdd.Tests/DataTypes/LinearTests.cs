using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiagnosticTool.Utilities.VectorCdd.XmlParser;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes
{
    [TestClass()]
    public class LinearTests
    {
        [TestMethod()]
        public void LinearTest()
        {
            var linearXmlNode = """
                <LINCOMP id='_0000022E001C0870' oid='94113CEF9325422a979B0296B3CE00FF' bm='4294967295'>
                <NAME>
                <TUV xml:lang='en-US'>Global Real Time</TUV>
                </NAME>
                <QUAL>Global_Real_Time</QUAL>
                <CVALUETYPE bl='32' bo='21' enc='uns' sig='0' df='hex' qty='atom' sz='no' minsz='0' maxsz='255'/>
                <EXCL s='4294967295' e='4294967295' inv='invalidSignal'>
                </EXCL>
                <PVALUETYPE bl='32' bo='21' enc='flt' sig='4' df='flt' qty='atom' sz='no' minsz='0' maxsz='255'>
                <UNIT>s</UNIT>
                </PVALUETYPE>
                <COMP e='4294967294' f='1' div='600' o='0'>
                </COMP>
                </LINCOMP>
                """;
            var linear = linearXmlNode.DeserializeXmlToObject<Linear>();

            var str = linear.SerializeObjectToXml();

            var rawData = Convert.FromHexString("00001000");
            var value = linear.Convert(rawData);
            Assert.IsTrue(value.IsSuccessful);
            // 4096 / 600 ≈ 6.8267
            Assert.AreEqual("6.8267", value.PhysicalResult.ValueString);
            Assert.AreEqual("s", value.PhysicalResult.Unit);
        }

        /// <summary>
        /// 线性转换基本公式: f/div * value + o
        /// bo='12'(Intel) 字节序翻转: 0x0500 = 1280, 1280 * 2 / 1 + 10 = 2570
        /// </summary>
        [TestMethod]
        public void Convert_BasicFormula_ReturnsCorrectValue()
        {
            var xml = """
                <LINCOMP>
                <CVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
                <PVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
                <COMP f='2' div='1' o='10'/>
                </LINCOMP>
                """;
            var linear = xml.DeserializeXmlToObject<Linear>();
            // bo='12' 字节翻转: 0x0005 -> 0x0500 = 1280, 1280 * 2 + 10 = 2570
            var result = linear.Convert(Convert.FromHexString("0005"));

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("2570", result.PhysicalResult.ValueString);
        }

        /// <summary>
        /// 线性转换带偏移量
        /// bo='12'(Intel) 字节序翻转: 0x6400 = 25600, 25600 * 1 / 10 + 5 = 2565
        /// </summary>
        [TestMethod]
        public void Convert_WithOffset_ReturnsCorrectValue()
        {
            var xml = """
                <LINCOMP>
                <CVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
                <PVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
                <COMP f='1' div='10' o='5'/>
                </LINCOMP>
                """;
            var linear = xml.DeserializeXmlToObject<Linear>();
            // bo='12' 字节翻转: 0x0064 -> 0x6400 = 25600, 25600/10 + 5 = 2565
            var result = linear.Convert(Convert.FromHexString("0064"));

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("2565", result.PhysicalResult.ValueString);
        }

        /// <summary>
        /// 线性转换值在有效范围内
        /// bo='12'(Intel) 字节序翻转: 0x3200 = 12800, 范围扩大以包含此值
        /// </summary>
        [TestMethod]
        public void Convert_ValueInRange_IsSuccessful()
        {
            var xml = """
                <LINCOMP>
                <CVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
                <PVALUETYPE bl='16' bo='12' enc='uns' sig='0' df='dec' qty='atom' sz='no' minsz='2' maxsz='2'/>
                <COMP f='1' div='1' o='0' s='10' e='20000'/>
                </LINCOMP>
                """;
            var linear = xml.DeserializeXmlToObject<Linear>();
            // bo='12' 字节翻转: 0x0032 -> 0x3200 = 12800, 在 [10, 20000] 范围内
            var result = linear.Convert(Convert.FromHexString("0032"));

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual("12800", result.PhysicalResult.ValueString);
        }

        /// <summary>
        /// Linear 序列化往返一致性
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
}
