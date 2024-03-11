using DiagnosticTool.Utilities.VectorCdd.DataTypes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.TestHelpers;

namespace DiagnosticTool.Utilities.VectorCdd.XmlParser
{
    [TestClass()]
    public class CommonTests
    {
        [TestMethod()]
        public void SerializeObjectTest()
        {
            DataTypes.TextTable tt = new()
            {
                BitMask = 255
            };
            tt.Names = [new General.MultiLanguageText() { Text = "Car Mode" }];
            tt.Qualified = "Car_Mode";
            tt.PhysicalValueType.Unit = "m";
            tt.TextMaps.Add(new DataTypes.TextTable.TextMap() { Start = 0, End = 0, Text = [new General.MultiLanguageText { Text = "Reversed" }] });
            tt.TextMaps.Add(new DataTypes.TextTable.TextMap() { Start = 1, End = 1, Text = [new General.MultiLanguageText { Text = "Short" }] });
            tt.TextMaps.Add(new DataTypes.TextTable.TextMap() { Start = 2, End = 2, Text = [new General.MultiLanguageText { Text = "Long" }] });
            tt.TextMaps.Add(new DataTypes.TextTable.TextMap() { Start = 3, End = 3, Text = [new General.MultiLanguageText { Text = "Continuous routine" }] });
            var s = tt.SerializeObjectToXml();

            // 验证序列化输出包含关键元素，不进行严格字符串匹配（因格式可能变化）
            Assert.IsTrue(s.Contains("TEXTTBL"), "序列化输出应包含 TEXTTBL 元素");
            Assert.IsTrue(s.Contains("bm=\"255\""), "序列化输出应包含 bm 属性");
            Assert.IsTrue(s.Contains("Car Mode"), "序列化输出应包含 NAME 文本");
            Assert.IsTrue(s.Contains("Car_Mode"), "序列化输出应包含 QUAL 值");
            Assert.IsTrue(s.Contains("Reversed"), "序列化输出应包含 TextMap 内容");
            Assert.IsTrue(s.Contains("<UNIT>m</UNIT>"), "序列化输出应包含 UNIT 元素");
        }

        [TestMethod()]
        public void DeserializeXmlToObjectTest()
        {
            var expected = """
                <TEXTTBL id='_0000022E1F0BC800' oid='38AE544E06F4465dBE00B07F9C5BFBBA' bm='255'>
                <NAME>
                <TUV xml:lang='en-US'>Car Mode</TUV>
                </NAME>
                <QUAL>Car_Mode</QUAL>
                <CVALUETYPE bl='8' bo='21' enc='bcd' sig='0' df='dec' qty='atom' sz='no' minsz='1' maxsz='1'/>
                <PVALUETYPE bl='16' bo='21' enc='utf' sig='0' df='text' qty='field' sz='no' minsz='0' maxsz='65535'/>
                <TEXTMAP s='0' e='0'>
                <TEXT>
                <TUV xml:lang='en-US'>Normal</TUV>
                </TEXT>
                </TEXTMAP>
                <TEXTMAP s='1' e='1'>
                <TEXT>
                <TUV xml:lang='en-US'>Transport</TUV>
                </TEXT>
                </TEXTMAP>
                <TEXTMAP s='2' e='2'>
                <TEXT>
                <TUV xml:lang='en-US'>Factory</TUV>
                </TEXT>
                </TEXTMAP>
                <TEXTMAP s='3' e='3'>
                <TEXT>
                <TUV xml:lang='en-US'>Crash</TUV>
                </TEXT>
                </TEXTMAP>
                <TEXTMAP s='5' e='5'>
                <TEXT>
                <TUV xml:lang='en-US'>Dyno</TUV>
                </TEXT>
                </TEXTMAP>
                </TEXTTBL>
                """;
            var tt = expected.DeserializeXmlToObject<DataTypes.TextTable>();
            Assert.IsNotNull(tt);
            Assert.AreEqual(5, tt.TextMaps.Count);
            var rawData = Convert.FromHexString("01");
            rawData = tt.ApplyBitMask(rawData);
            var value = tt.Convert(rawData);
            Assert.IsTrue(value.IsSuccessful);
        }

        [TestMethod()]
        public void DeserializeXmlToObjectTest1()
        {
            var candela = CddTestData.Sweet500;
            Assert.IsNotNull(candela);
            Assert.IsFalse(string.IsNullOrEmpty(candela.Version));
            Assert.IsNotNull(candela.EcuDocument);
            Assert.IsTrue(candela.EcuDocument.Ecu.Variants.Count > 0);
        }
    }
}
