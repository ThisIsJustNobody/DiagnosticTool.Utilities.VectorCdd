using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;

namespace DiagnosticTool.Utilities.VectorCdd.XmlParser;

public static class Common
{
    [AttributeUsage(AttributeTargets.Field)]
    public class XmlValueAttribute(string Value) : Attribute
    {
        public string Value { get; set; } = Value;
    }

    public static string GetXmlValueAttributeValue<T>(this T value)
        where T : Enum
    {
        if (value.GetType().GetField(value.ToString())!.GetCustomAttribute(typeof(XmlValueAttribute)) is XmlValueAttribute xmlValueAttribute)
        {
            return xmlValueAttribute.Value;
        }
        throw new ArgumentOutOfRangeException(nameof(value), "未知的枚举值！");
    }

    public static T ParseXmlValueAttributeValue<T>(this string value)
        where T : Enum
    {
        var e = typeof(T).GetFields().FirstOrDefault(x =>
        {
            if (x.GetCustomAttribute(typeof(XmlValueAttribute)) is XmlValueAttribute xmlValueAttribute)
            {
                if (xmlValueAttribute.Value == value)
                {
                    return true;
                }
            }
            return false;
        });
        if (e != null)
        {
            return (T)e.GetValue(null)!;
        }
        throw new Exception("未知的枚举值！");
    }

    public static string SerializeObjectToXml<T>(this T obj)
    {
        StringWriter stringWriter = new();
        XmlSerializer serializer = new(typeof(T));
        XmlSerializerNamespaces @namespace = new();
        @namespace.Add("", "");
        serializer.Serialize(stringWriter, obj, @namespace);
        return stringWriter.ToString();
    }

    public static T DeserializeXmlToObject<T>(this string xml)
    {
        XmlSerializer serializer = new(typeof(T));
        using StringReader reader = new(xml);
        var obj = serializer.Deserialize(reader);
        if (obj is T result)
        {
            return result;
        }
        else
        {
            throw new ArgumentException("未能转为目标类型！", nameof(xml));
        }
    }
}
