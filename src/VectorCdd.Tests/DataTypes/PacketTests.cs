using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiagnosticTool.Utilities.VectorCdd.Common;
using System.Text.RegularExpressions;
using DiagnosticTool.Utilities.VectorCdd.XmlParser;
using DiagnosticTool.Utilities.VectorCdd.TestHelpers;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes
{
    [TestClass()]
    public class PacketTests
    {
        [TestMethod()]
        public void PacketTest()
        {
            var xml = """
                <STRUCTDT id='_0000022E1F148E20' oid='296C1788C61F45b485D803752B4C67D0' bm='4294967295'>
                <NAME>
                <TUV xml:lang='en-US'>B03B FDR Internal Fault Status (MCU)</TUV>
                </NAME>
                <QUAL>B03B_FDR_Internal_Fault_Status_MCU</QUAL>
                <CVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='field' sz='no' minsz='6' maxsz='6'/>
                <PVALUETYPE bl='8' bo='12' enc='uns' sig='0' df='hex' qty='field' sz='no' minsz='6' maxsz='6'/>
                <STRUCT oid='7D1A7303849342aa8BE38761EE23E9C1' spec='no' dtref='_0000022E1EE62330'>
                <NAME>
                <TUV xml:lang='en-US'>Byte 1</TUV>
                </NAME>
                <QUAL>Byte_1</QUAL>
                <DATAOBJ oid='2BA48A993B0542ec9EAF09D83FC05F72' spec='no' dtref='_0000022E1F121AC0'>
                <NAME>
                <TUV xml:lang='en-US'>FAULT_MICRO_XBAR internal fault is set</TUV>
                </NAME>
                <QUAL>FAULT_MICRO_XBAR_internal_fault_is_set</QUAL>
                </DATAOBJ>
                <DATAOBJ oid='E352CBA4FC654c328927547EC9990FAB' spec='no' dtref='_0000022E1F121AC0'>
                <NAME>
                <TUV xml:lang='en-US'>FAULT_MICRO_RAM internal fault is set</TUV>
                </NAME>
                <QUAL>FAULT_MICRO_RAM_internal_fault_is_set</QUAL>
                </DATAOBJ>
                </STRUCT>
                <STRUCT oid='64A2A7746E6D4992957B53C154B6DB2F' spec='no' dtref='_0000022E1EE62330'>
                <NAME>
                <TUV xml:lang='en-US'>Byte 2</TUV>
                </NAME>
                <QUAL>Byte_2</QUAL>
                <DATAOBJ oid='0045F667689C4398B1CEF034A1795743' spec='no' dtref='_0000022E1F121AC0'>
                <NAME>
                <TUV xml:lang='en-US'>FAULT_MICRO_OVERTEMPERATURE internal fault is set</TUV>
                </NAME>
                <QUAL>FAULT_MICRO_OVERTEMPERATURE_internal_fault_is_set</QUAL>
                </DATAOBJ>
                </STRUCT>
                </STRUCTDT>
                """;
            var packet = xml.DeserializeXmlToObject<Packet>();
            Assert.IsNotNull(packet);
            Assert.AreEqual(2, packet.DataObjs.Count);
            Assert.AreEqual("Byte 1", packet.DataObjs[0].Name);
            Assert.AreEqual("Byte 2", packet.DataObjs[1].Name);
        }

        [TestMethod()]
        [DataRow(0xFD1Du, "00000000000000")]
        [DataRow(0xFD50u, "32312E41342E3136")]
        [DataRow(0xFF00u, "02000000")]
        [DataRow(0xFEF1u, "20230714")]
        [DataRow(0xFEE0u, "100100")]
        [DataRow(0xFD71u, "00518E7E0B")]
        [DataRow(0xFD1Au, "00000041342E31")]
        [DataRow(0xB03Bu, "123456789ABC")]
        [DataRow(0xF1A0u, "6608118654202041")]
        [DataRow(0xF1AEu, "02 66 08 11 86 55 20 20 41 00 00 00 00 00 00 00 00")]
        [DataRow(0xF180u, "02 5A 4B 5F 42 58 5F 4D 52 47 5F 52 42 6F 5F 4D 38 5F 52 30 35 41 30 30")]
        [DataRow(0xFD13u, "00")]
        [DataRow(0xFD46u, "00 00 00 00 BF D9 99 9A BF 80 00 00 BF 33 33 33 40 D8 90 00 41 28 66 00 40 9C 72 00 00 00 00 00 00 59 00 64 00 00 0A 98 0D 28 00 00")]
        [DataRow(0xFD45u, "C1 C8 00 00 3F 80 00 00 C1 A0 00 00 00 00 00 00 3F 7A E1 48 3F 7A E1 48 3F 7A E1 48")]
        [DataRow(0xFD20u, "33 20 33 20 0C D8 03 34 13 9B 00 00 00 00 00 00")]
        [TestCategory("Integration")]
        public void ConvertTest(uint dataId, string input)
        {
            var candela = CddTestData.Flr3;
            var rawData = Convert.FromHexString(Regex.Replace(input, @"\s", ""));
            var did = candela.EcuDocument.DataIdentities.DataIdentities.First(x => x.DataIdentityValue == dataId);
            var r = did.Convert(rawData);

            Assert.IsNotNull(r);
            Assert.IsTrue(r.IsSuccessful, $"DID 0x{dataId:X} 转换失败: {r.Error.ErrorType} - {r.Error.ErrorMessage}");
            Assert.IsNotNull(r.DisplayText());
            Assert.AreNotEqual(string.Empty, r.DisplayText());
        }
    }
}
