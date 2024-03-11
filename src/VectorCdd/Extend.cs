using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticTool.Utilities.VectorCdd;

/// <summary>
/// 扩展方法
/// </summary>
public static class Extend
{
    #region 私有辅助方法
    /// <summary>
    /// 从二进制字符串按组分割为字节数组
    /// </summary>
    /// <remarks>
    /// 先补齐长度，再按照每 8 bit 转为 1 byte。
    /// 反转时在右侧填充 0 进行补齐，每 8 bit 先反转为 bit7-bit0 再转为 1 byte。
    /// </remarks>
    /// <param name="binsString"></param>
    /// <param name="groupUnit"></param>
    /// <param name="reverseBitOrder">反转位序。false: bit7-bit0; true: bit0-bit7</param>
    /// <returns></returns>
    public static byte[] FromBinsStringByGroupUnit(this string binsString, int groupUnit = 8, bool reverseBitOrder = false)
    {
        if (groupUnit > 8 || groupUnit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(groupUnit), "组长度必须在 1 到 8 之间");
        }

        // 补齐长度
        if ((binsString.Length % groupUnit) != 0)
        {
            if (reverseBitOrder)
            {
                binsString = binsString.PadRight((binsString.Length / groupUnit + 1) * groupUnit, '0');
            }
            else
            {
                binsString = binsString.PadLeft((binsString.Length / groupUnit + 1) * groupUnit, '0');
            }
        }

        return binsString.Select((bit, index) => (bit, index))
            .GroupBy(x => x.index / groupUnit)
            .Select(g =>
            {
                string groupedBinString;
                if (reverseBitOrder)
                {
                    groupedBinString = new string(g.Select(x => x.bit == '0' ? '0' : '1').Reverse().ToArray());
                }
                else
                {
                    groupedBinString = new string(g.Select(x => x.bit == '0' ? '0' : '1').ToArray());
                }
                var groupedByte = Convert.ToByte(groupedBinString, 2);
                return groupedByte;
            })
            .ToArray();
    }

    /// <summary>
    /// 移除前导零
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string RemoveLeadingZero(string str)
    {
        // 移除前导 0
        while (str.StartsWith('0'))
            str = str[1..];

        // 如果为空字符串，返回 0
        if (str == "")
            str = "0";

        return str;
    }
    #endregion 私有辅助方法


    #region 二进制字符串与字节数组转换
    /// <summary>
    /// 二进制字符串转换为字节数组
    /// </summary>
    /// <remarks>
    /// 先补齐长度，再按照每 8 bit 转为 1 byte。
    /// 反转时在右侧填充 0 进行补齐，每 8 bit 先反转为 bit7-bit0 再转为 1 byte。
    /// </remarks>
    /// <param name="binsString"></param>
    /// <param name="reverseBitOrder">反转位序。false: bit7-bit0; true: bit0-bit7</param>
    /// <returns></returns>
    public static byte[] FromBinarysString(this string binsString, bool reverseBitOrder = false)
    {
        return FromBinsStringByGroupUnit(binsString, 8, reverseBitOrder);
    }

    /// <summary>
    /// 二进制字符串转换为字节
    /// </summary>
    /// <remarks>
    /// 按照每 8 bit 转为 1 byte。
    /// 反转时在右侧填充 0 进行补齐，每 8 bit 先反转为 bit7-bit0 再转为 1 byte。
    /// </remarks>
    /// <param name="binString"></param>
    /// <param name="reverseBitOrder">反转位序。false: bit7-bit0; true: bit0-bit7</param>
    /// <returns></returns>
    public static byte FromBinaryString(this string binString, bool reverseBitOrder = false)
    {
        if (reverseBitOrder)
        {
            binString = binString.PadRight((binString.Length / 8 + 1) * 8, '0');
            return Convert.ToByte(string.Join("", binString.Reverse()), 2);
        }
        else
        {
            binString = binString.PadLeft((binString.Length / 8 + 1) * 8, '0');
            return Convert.ToByte(binString, 2);
        }
    }

    /// <summary>
    /// 字节转为二进制字符串
    /// </summary>
    /// <param name="byte"></param>
    /// <param name="reverseBitOrder">反转位序。false: bit7-bit0; true: bit0-bit7</param>
    /// <returns></returns>
    public static string ToBinaryString(this byte @byte, bool reverseBitOrder = false, bool removeLeadingZero = false)
    {
        string binString;
        if (reverseBitOrder)
        {
            binString = string.Join("", Convert.ToString(@byte, 2).Reverse()).PadRight(8, '0');
        }
        else
        {
            binString = Convert.ToString(@byte, 2).PadLeft(8, '0');
        }

        if (removeLeadingZero)
        {
            RemoveLeadingZero(binString);
        }

        return binString;
    }

    /// <summary>
    /// 字节数组转为二进制字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="reverseBitOrder">反转位序。false: bit7-bit0; true: bit0-bit7</param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string ToBinaryString(this byte[] bytes, bool reverseBitOrder = false, string separator = "", bool removeLeadingZero = false)
    {
        var binString = string.Join(separator, bytes.Select(x =>
        {
            if (reverseBitOrder)
            {
                return string.Join("", Convert.ToString(x, 2).Reverse()).PadRight(8, '0');
            }
            else
            {
                return Convert.ToString(x, 2).PadLeft(8, '0');
            }
        }));

        if (removeLeadingZero)
            binString = RemoveLeadingZero(binString);

        return binString;
    }
    #endregion 二进制字符串与字节数组转换


    /// <summary>
    /// BCD 类型的二进制字符串（4 bit）转为数字
    /// </summary>
    /// <param name="bcdbinsString"></param>
    /// <returns></returns>
    public static BigInteger FromBinsStringInBcd(this string bcdbinsString, bool reverseBitOrder = false)
    {
        var bytes = bcdbinsString.FromBinsStringByGroupUnit(4, reverseBitOrder);
        if (bytes.Where(x => x >= 10).Any())
        {
            throw new ArgumentException("BCD 字符串中包含非法字符");
        }
        return BigInteger.Parse(string.Join("", bytes.Select(x => x.ToString())));
    }

    /// <summary>
    /// 转为十六进制字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="separator"></param>
    /// <param name="removeLeadingZero"></param>
    /// <returns></returns>
    public static string ToHexadecimalString(this byte[] bytes, string separator = "", bool removeLeadingZero = false)
    {
        var hexString = string.Join(separator, bytes.Select(x => x.ToString("X2")));

        if (removeLeadingZero)
            hexString = RemoveLeadingZero(hexString);

        return hexString;
    }

    /// <summary>
    /// 转为八进制字符串
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="separator"></param>
    /// <param name="removeLeadingZero">前导零</param>
    /// <returns></returns>
    public static string ToOctalString(this byte[] bytes, string separator = "", bool removeLeadingZero = false)
    {
        var binsString = bytes.ToBinaryString(false, separator);
        // 补齐长度
        if ((binsString.Length % 3) != 0)
        {
            binsString = binsString.PadLeft((binsString.Length / 3 + 1) * 3, '0');
        }

        var groupedBytes = binsString.FromBinsStringByGroupUnit(3);
        var octsString = string.Join(separator, groupedBytes.Select(x =>
        {
            return Convert.ToString(x, 8);
        }));

        if (removeLeadingZero)
            octsString = RemoveLeadingZero(octsString);

        return octsString;
    }
}
