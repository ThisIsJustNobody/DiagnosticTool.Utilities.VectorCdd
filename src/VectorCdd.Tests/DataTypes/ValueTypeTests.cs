using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticTool.Utilities.VectorCdd.DataTypes
{
    [TestClass()]
    public class ValueTypeTests
    {
        [TestMethod()]
        [DataRow("01", "0", 4, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78", "305419896", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78", "0x12345678", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Hexadecimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78", "2215053170", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Octal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78", "10010001101000101011001111000", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Binary, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78", "305419896", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned64, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("81", "-127", 8, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Signed, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78", "305419896", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Signed, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78", "305419896", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Signed64, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78", "12345678", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.BCD, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("12 34 56 78 12 34 56 78", "1234567812345678", 64, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.BCD64, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("3F 9E 06 52", "1.2345679", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Float, 7, ValueType.DisplayFormatTypes.FloatingPoint, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("BF 9E 06 52", "-1.23", 32, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Float, 2, ValueType.DisplayFormatTypes.FloatingPoint, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("3F F3 C0 CA 42 8C 59 FB", "1.2345678901234567", 64, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.DoubleFloat, 16, ValueType.DisplayFormatTypes.FloatingPoint, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("BF F3 C0 CA 42 8C 59 FB", "-1.2345678901234567", 64, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.DoubleFloat, 16, ValueType.DisplayFormatTypes.FloatingPoint, ValueType.SizeDescriptionTypes.Atom)]
        [DataRow("31", "1", 8, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.ASCII, 0, ValueType.DisplayFormatTypes.Text, ValueType.SizeDescriptionTypes.Field)]
        [DataRow("48 00 65 00 6C 00 6C 00 6F 00", "Hello", 16, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.UNICODE, 0, ValueType.DisplayFormatTypes.Text, ValueType.SizeDescriptionTypes.Field, ValueType.StringTerminationTypes.NoSizeinfo, 5, 5)]
        [DataRow("12 34 56 78", "0x12 0x34 0x56 0x78", 8, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Hexadecimal, ValueType.SizeDescriptionTypes.Field, ValueType.StringTerminationTypes.NoSizeinfo, 4, 4)]
        [DataRow("15", "0x00", 1, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Hexadecimal, ValueType.SizeDescriptionTypes.Atom, ValueType.StringTerminationTypes.NoSizeinfo, 1, 1)]
        [DataRow("FF FF", "11111111", 8, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Binary, ValueType.SizeDescriptionTypes.Atom, ValueType.StringTerminationTypes.NoSizeinfo, 1, 1)]
        [DataRow("30 31 32", "012", 8, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.ASCII, 0, ValueType.DisplayFormatTypes.Text, ValueType.SizeDescriptionTypes.Field, ValueType.StringTerminationTypes.NoSizeinfo, 3, 3)]
        [DataRow("12 34 56 78", "12 34 56 78", 8, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.BCD, 0, ValueType.DisplayFormatTypes.Decimal, ValueType.SizeDescriptionTypes.Field, ValueType.StringTerminationTypes.NoSizeinfo, 4, 4)]
        [DataRow("12 34 56 78 12 34 56 78", "0x12 0x34 0x56 0x78 0x12 0x34 0x56 0x78", 8, ValueType.ByteOrderTypes.HighLow, ValueType.EncodingTypes.Unsigned, 0, ValueType.DisplayFormatTypes.Hexadecimal, ValueType.SizeDescriptionTypes.Field, ValueType.StringTerminationTypes.NoSizeinfo, 8, 8)]
        public void ValueTypeTest(
            string input, string output,
            int bitLength,
            ValueType.ByteOrderTypes byteOrderType,
            ValueType.EncodingTypes encodingType,
            int precision,
            ValueType.DisplayFormatTypes displayFormatType,
            ValueType.SizeDescriptionTypes sizeDescriptionType,
            ValueType.StringTerminationTypes stringTerminationType = ValueType.StringTerminationTypes.NoSizeinfo,
            int minsz = 1, int maxsz = 1)
        {
            ValueType valueType = new()
            {
                BitLength = bitLength,
                ByteOrderType = byteOrderType,
                EncodingType = encodingType,
                Precision = precision,
                DisplayFormatType = displayFormatType,
                SizeDescriptionType = sizeDescriptionType,
                StringTerminationType = stringTerminationType,
                MinFieldCount = minsz,
                MaxFieldCount = maxsz,
            };
            var rawData = Convert.FromHexString(input.Replace(" ", ""));
            rawData = valueType.InterceptBytes(rawData, out var cutBytes);
            var value = valueType.GetResultAndDisplayValue(rawData);
            Assert.AreEqual(output, value);
        }
    }
}
