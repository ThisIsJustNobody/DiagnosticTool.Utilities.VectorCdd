# CddReaderLib

用于解析 Vector CANoe 软件的 `.cdd`（CANdela Diagnostic Description）文件的 .NET 8.0 类库。

将 CANDELA XML 格式的 `.cdd` 文件反序列化为强类型对象模型，并根据其中定义的编码规则将原始 UDS 诊断字节数据转换为可读的用户信息。

主要入口类型为 `CddReaderLib.Common.CanDela`，通过 `XmlParser` 扩展方法 `DeserializeXmlToObject<CanDela>()` 完成解析。

详见上层目录 [README](../README.md)。
