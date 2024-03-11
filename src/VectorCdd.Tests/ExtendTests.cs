using DiagnosticTool.Utilities.VectorCdd;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticTool.Utilities.VectorCdd
{
    [TestClass()]
    public class ExtendTests
    {
        [TestMethod()]
        public void ToOctsStringTest()
        {
            // 0x1234 = 00010010 00110100 -> 000001 001 000 110 100 = 011064 octal
            var o = (new byte[] { 0x12, 0x34 }).ToOctalString();
            Assert.AreEqual("011064", o);
        }

        [TestMethod()]
        public void ToHexadecimalString_StandardInput()
        {
            var result = new byte[] { 0xAB, 0xCD }.ToHexadecimalString();
            Assert.AreEqual("ABCD", result);
        }

        [TestMethod()]
        public void ToHexadecimalString_WithSeparator()
        {
            var result = new byte[] { 0xAB, 0xCD }.ToHexadecimalString(" ");
            Assert.AreEqual("AB CD", result);
        }

        [TestMethod()]
        public void ToBinaryString_SingleByte()
        {
            var result = ((byte)0xAB).ToBinaryString();
            Assert.AreEqual("10101011", result);
        }

        [TestMethod()]
        public void ToBinaryString_ByteArray()
        {
            var result = new byte[] { 0x01, 0x02 }.ToBinaryString();
            Assert.IsTrue(result.Contains("00000001"));
            Assert.IsTrue(result.Contains("00000010"));
        }

        [TestMethod()]
        public void FromBinarysString_StandardInput()
        {
            var result = "00000001".FromBinarysString();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(0x01, result[0]);
        }

        [TestMethod()]
        public void FromBinarysString_MultiByte()
        {
            var result = "0000000100000010".FromBinarysString();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(0x01, result[0]);
            Assert.AreEqual(0x02, result[1]);
        }

        [TestMethod()]
        public void FromBinaryString_SingleByte()
        {
            var result = "10101011".FromBinaryString();
            Assert.AreEqual(0xAB, result);
        }
    }
}
