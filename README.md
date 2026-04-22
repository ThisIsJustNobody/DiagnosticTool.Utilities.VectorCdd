# VectorCdd

用于解析 Vector CANoe 软件的 `.cdd`（CANdela Diagnostic Description）文件的 C# 类库。`.cdd` 文件基于 CANDELA XML 格式，描述了 ECU 诊断的完整规范，包括数据类型、数据标识符（DID）、协议服务、诊断类等。

本库可将 `.cdd` 文件反序列化为强类型对象模型，并能根据其中定义的规则将原始 UDS 诊断字节数据转换为可读的用户信息。

## 环境要求

- .NET 10.0

## 构建与测试

```bash
dotnet build
dotnet test
dotnet pack   # 生成 NuGet 包（配置了 GeneratePackageOnBuild）
```

## 快速上手

```csharp
using DiagnosticTool.Utilities.VectorCdd.Common;
using DiagnosticTool.Utilities.VectorCdd.XmlParser;

// 1. 读取并解析 .cdd 文件
var xml = File.ReadAllText("sample.cdd");
var candela = xml.DeserializeXmlToObject<CanDela>();
candela.SetCanDelaReference();  // 建立交叉引用

// 2. 获取 ECU 变体
var variant = candela.EcuDocument.Ecu.Variants.First();

// 3. 根据 DID 解析原始字节数据
var dataIdentity = candela.EcuDocument.DataIdentities.DataIdentities
    .First(d => d.DataIdentityValue == 0xF190);
var result = dataIdentity.Convert(rawBytes);

// 4. 获取可读文本
Console.WriteLine(result.DisplayText());
// 输出示例: Boot Software:  SWT500_DBOOT_2604B
```

## 架构概览

### 对象模型继承链

所有 CDD 模型对象继承自 `CanDelaBase` → `General` → 各领域类。`CanDelaBase` 持有对根文档 `CanDela` 的引用，`General` 提供通用 XML 属性（`id`、`oid`、`NAME`、`DESC`、`dtref` 等）。

### 核心模块

| 模块 | 说明 |
|------|------|
| `Common/CanDela` | 根元素 `<CANDELA>`，包含版本号和 `EcuDocument` |
| `Common/EcuDocument` | 顶层容器 `<ECUDOC>`，聚合数据类型、DID、协议服务、ECU 变体等 |
| `DataTypes/` | 数据类型体系 — 解码引擎的核心（见下文） |
| `DataIdentities/` | DID（数据标识符），每个 DID 包含一个 Packet 数据类型 |
| `ECU/` | ECU 模型：ECU → Variant → DiagnosticClass → DiagnosticInstance → Service |
| `ProtocolServices/` | 协议服务定义：Request / PositiveResponse / NegativeResponse |
| `XmlParser/` | XML 序列化/反序列化工具，包含自定义 `XmlValueAttribute` |
| `Extend.cs` | 扩展方法：进制转换、BCD 解码、位操作等 |

### 数据类型体系（DataTypes/）

这是解码引擎的核心，所有类型实现了 `IConvert` 接口的 `Convert(byte[] rawData)` 方法：

| 类型 | 用途 |
|------|------|
| `BaseDataType` | 基类，处理位掩码、字节截取、编码值/物理值转换与有效性校验 |
| `ValueType` | 底层二进制解析：编码类型（Unsigned/Signed/BCD/Float/ASCII/Unicode）、字节序（Intel/Motorola）、显示格式（Dec/Hex/Oct/Bin） |
| `Identity` | 直通类型，不做任何转换 |
| `Linear` | 线性变换：`物理值 = (编码值 × 系数 / 除数) + 偏移量`，带有效范围校验 |
| `TextTable` | 值到多语言文本的映射 |
| `Packet` | 复合/结构化数据类型，子数据对象按顺序解码 |
| `BitsField` | 位级结构化数据（继承 Packet，按位截取） |
| `Multiplexer` | 基于选择器的多路复用解码 |

### 转换流程

```
原始字节 → InterceptBytes（截取）→ ApplyBitMask（位掩码）
→ CodeValueType.GetResult（编码值解码）→ 有效性校验
→ ConvertPhysicalValue（物理值运算）→ ConvertResult
```

`ConvertResult` 包含：
- `CodeResult` — 编码值（原始解码结果）
- `PhysicalResult` — 物理值（经过运算的最终结果，含单位）
- `DisplayText()` — 生成可读文本

## 目录结构

```
src/
├── VectorCdd/              # 主库
│   ├── Common/             # 基类与共享类型
│   ├── DataTypes/          # 数据类型定义
│   ├── DataIdentities/     # DID 定义
│   ├── ECU/                # ECU 模型
│   ├── ProtocolServices/   # 协议服务
│   ├── DiagnosticClassTemplates/
│   ├── RecordDataTablePool/
│   ├── ExtStorageItems/
│   ├── TargetGroups/
│   ├── DefineAttributes/
│   ├── Authors/
│   └── XmlParser/          # XML 序列化工具
└── VectorCdd.Tests/        # 测试项目（MSTest）
```
