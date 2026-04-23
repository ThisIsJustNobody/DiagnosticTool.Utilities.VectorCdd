# CANDELA (.cdd) 文件格式技术文档

## 1. 概述

`.cdd` 文件是 Vector CANoe/CANdelaStudio 使用的诊断描述文件，基于 **CANDELA XML Schema**。它描述了一个 ECU 的完整诊断接口：支持的 UDS 服务、数据标识符（DID）、数据编码方式、ECU 变体等。

CddReaderLib 将 `.cdd` XML 反序列化为 C# 对象模型，并提供将原始 UDS 诊断字节解码为人类可读值的能力。

### 文件头

```xml
<?xml version='1.0' encoding='utf-8' standalone='no'?>
<!DOCTYPE CANDELA SYSTEM 'candela.dtd'>
<CANDELA dtdvers='12.0.104'>
```

- 根元素 `<CANDELA>`，无 XML 命名空间
- `dtdvers` 属性标识 DTD 版本（已见 `9.1.100`、`12.0.104`）

---

## 2. XML 文档结构

### 2.1 顶层元素

```
<CANDELA dtdvers='...'>
  └─ <ECUDOC oid='...' temploid='...' languages='(en-US)'>
       ├─ <EXTSTORAGEITEMS>      嵌入文档（图片等）
       ├─ <DESC>                  文档描述
       ├─ <PROTOCOLSTANDARD>      协议标准（如 'UDS'）
       ├─ <SPECOWNER>             规范所有者（如 'Vector'）
       ├─ <QUALGENOPTIONS>        限定符生成选项
       ├─ <ATTRCATS>              属性分类
       ├─ <DEFATTS>               自定义属性定义
       ├─ <AUTHORS>               作者列表
       ├─ <TARGETGROUPS>          目标组
       ├─ <DATATYPES>             数据类型定义 ★
       ├─ <RECORDDTPOOL>          记录数据表
       ├─ <DIDS>                  数据标识符定义 ★
       ├─ <PROTOCOLSERVICES>      协议服务定义 ★
       ├─ <DCLTMPLS>              诊断类模板 ★
       └─ <ECU>                   ECU 变体 ★
```

带 ★ 的为核心解析对象。

### 2.2 ECUDOC 常见属性

| 属性 | 说明 |
|------|------|
| `oid` | 对象 ID（十六进制 GUID） |
| `temploid` | 模板 OID |
| `doctype` | 文档类型，固定为 `'inst'` |
| `manufacturer` | 制造商，如 `'Vector'` |
| `languages` | 支持的语言，如 `'(en-US)'` |
| `saveno` | 保存计数 |
| `dtNesting` | 嵌套策略，如 `'all'` |

---

## 3. 多语言文本系统 (TUV)

整个文档使用 `<TUV>` 元素实现多语言文本：

```xml
<NAME>
  <TUV xml:lang='en-US'>Global Real Time</TUV>
  <TUV xml:lang='zh-CN'>全局实时时钟</TUV>
</NAME>
```

每个继承自 `General` 的对象都具有：
- `<NAME>` → `List<MultiLanguageText>` — 本地化名称
- `<DESC>` → `List<MultiLanguageText>` — 本地化描述

---

## 4. 通用属性 (General)

所有主要元素共享的属性：

| XML 属性 | C# 属性 | 说明 |
|----------|---------|------|
| `id` | `Id` | 唯一标识符（十六进制字符串） |
| `oid` | `ObjectId` | 对象 ID（十六进制 GUID） |
| `temploid` | `TemplateObjectId` | 模板引用 |
| `spec` | `Spec` | 专用化标记：`'sid'`（服务 ID）、`'sub'`（子功能）、`'no'` |
| `dtref` | `DataTypeReferenceId` | 引用数据类型的 `id`，运行时解析为实际数据类型对象 |

### 类继承层次

```
CanDelaBase                    持有 CanDela 根引用
  └─ General                   id/oid/dtref/NAME/DESC
       ├─ BaseDataType (IConvert)  位掩码/EXCL/编码值转换
       │    ├─ Identity            透传
       │    ├─ Linear              线性转换
       │    ├─ TextTable           文本映射
       │    ├─ Packet              复合结构
       │    │    └─ BitsField      位级字段
       │    └─ Multiplexer        多路复用
       ├─ DataIdentity            DID 定义
       ├─ ProtocolService         协议服务
       ├─ BaseComponent           组件基类
       ├─ Variant / DiagnosticClass / DiagnosticInstance  ECU 层级
       └─ DiagnosticClassTemplate / DCLSRVTMPL           模板层级
```

---

## 5. 数据类型定义 (DATATYPES)

五种数据类型，均实现 `IConvert` 接口：

| XML 元素 | C# 类 | 说明 |
|----------|-------|------|
| `<IDENT>` | `Identity` | 透传，无转换 |
| `<TEXTTBL>` | `TextTable` | 值→文本映射 |
| `<LINCOMP>` | `Linear` | 线性公式转换 |
| `<STRUCTDT>` | `Packet` | 复合结构（子字段序列） |
| `<MUXDT>` | `Multiplexer` | 多路复用（选择器+分支） |

### 5.1 值类型 (ValueType)

每个数据类型有两个值类型子元素：

- **`<CVALUETYPE>`** — 编码值类型（线上的原始编码方式）
- **`<PVALUETYPE>`** — 物理值类型（展示给用户的格式）

| XML 属性 | C# 属性 | 取值说明 |
|----------|---------|---------|
| `bl` | `BitLength` | 位长度（1–32 或 64） |
| `bo` | `ByteOrderType` | `'12'` Intel 小端序 / `'21'` Motorola 大端序 |
| `enc` | `EncodingType` | 编码类型（见下表） |
| `df` | `DisplayFormatType` | 显示格式 |
| `qty` | `SizeDescriptionType` | `'atom'` 固定长度 / `'field'` 数组 |
| `sig` | `Precision` | 小数精度 |
| `sz` | `StringTerminationType` | 字符串终止方式 |
| `minsz` | `MinFieldCount` | 数组最小元素数 |
| `maxsz` | `MaxFieldCount` | 数组最大元素数 |
| `<UNIT>` | `Unit` | 单位（如 `'ms'`、`'s'`） |

#### 编码类型

| 值 | 含义 | 位长 |
|----|------|------|
| `uns` | 无符号整数 | 8（默认） |
| `sgn` | 有符号整数 | 8 |
| `bcd` | 8421 BCD 码 | 8 |
| `uns64` / `sgn64` / `bcd64` | 64 位变体 | 64 |
| `flt` | IEEE 754 单精度浮点 | 32 |
| `dbl` | IEEE 754 双精度浮点 | 64 |
| `asc` | ASCII 文本 | 7 或 8 |
| `utf` | Unicode 文本 | 16 |

#### 字节序

| 值 | 含义 | 说明 |
|----|------|------|
| `12` | Intel（小端序） | 低字节在前 |
| `21` | Motorola（大端序） | 高字节在前（汽车领域默认） |

#### 显示格式

| 值 | 输出格式 | 示例 |
|----|---------|------|
| `dec` | 十进制 | `66` |
| `hex` | 十六进制（带 `0x` 前缀） | `0x42` |
| `oct` | 八进制 | `102` |
| `bin` | 二进制 | `1000010` |
| `flt` | 浮点数 | `3.14` |
| `text` | 文本 | `Hello` |

### 5.2 Identity (透传)

最简单的类型，直接将编码值作为物理值输出。

```xml
<IDENT id='_0000022E1EE62330' bm='255'>
  <CVALUETYPE bl='8' bo='21' enc='uns' sig='0' df='hex' qty='atom'/>
  <PVALUETYPE bl='8' bo='21' enc='uns' sig='0' df='hex' qty='atom'/>
</IDENT>
```

- `bm` 属性：位掩码，解码前对原始字节按位与

### 5.3 TextTable (文本映射表)

将离散整数值映射为多语言文本。

```xml
<TEXTTBL bm='255'>
  <CVALUETYPE bl='8' bo='21' enc='uns' sig='0' df='dec' qty='atom'/>
  <PVALUETYPE bl='16' bo='21' enc='utf' sig='0' df='text' qty='field'/>
  <TEXTMAP s='0' e='0'>
    <TEXT><TUV xml:lang='en-US'>Normal</TUV></TEXT>
  </TEXTMAP>
  <TEXTMAP s='1' e='1'>
    <TEXT><TUV xml:lang='en-US'>Transport</TUV></TEXT>
  </TEXTMAP>
  <TEXTMAP s='5' e='10'>
    <TEXT><TUV xml:lang='en-US'>Range Value</TUV></TEXT>
  </TEXTMAP>
</TEXTTBL>
```

**转换逻辑：**
1. 解码编码值（整数）
2. 查找第一个满足 `值 ∈ [s, e]` 的 `<TEXTMAP>`
3. 返回对应的 `<TUV>` 文本作为物理结果

- `s` / `e` 支持范围匹配（不仅限于单值）
- 值不匹配任何 TEXTMAP 时返回 `OutOfRange` 错误

### 5.4 Linear (线性转换)

线性公式：**`physical = (factor / divisor) × code + offset`**

```xml
<LINCOMP bm='65535'>
  <CVALUETYPE bl='16' bo='21' enc='uns' df='dec' qty='atom'/>
  <PVALUETYPE bl='32' bo='21' enc='flt' df='flt' qty='atom'>
    <UNIT>ms</UNIT>
  </PVALUETYPE>
  <COMP f='10' div='3' o='5' s='0' e='65534'/>
</LINCOMP>
```

**`<COMP>` 属性：**

| 属性 | C# 属性 | 默认值 | 说明 |
|------|---------|--------|------|
| `f` | `Factor` | 1 | 乘法系数 |
| `div` | `Divisor` | 1 | 除数 |
| `o` | `Offset` | 0 | 加法偏移量 |
| `s` | `LowerLimit` | null | 有效范围下限 |
| `e` | `UpperLimit` | null | 有效范围上限 |
| `inv` | `InverseValue` | 0 | 反模式值（当 factor=0 时） |

**特殊模式（factor == 0）：**
`physical = inverseValue / divisor / code + offset`

**范围检查：** 编码值超出 `[s, e]` 范围时返回 `UndefinedRange` 错误。

### 5.5 Packet / STRUCTDT (复合结构)

由多个子字段按顺序组成的结构体。

```xml
<STRUCTDT bm='4294967295'>
  <STRUCT spec='no' dtref='_0000022E1EE62330'>
    <DATAOBJ spec='no' dtref='_0000022E1EF613B0'/>
    <DATAOBJ spec='no' dtref='_0000022E1EF613B0'/>
    <GAPDATAOBJ bl='2'/>
    <DATAOBJ spec='no' dtref='_0000022E1EF62580'/>
  </STRUCT>
</STRUCTDT>
```

**子元素类型：**

| XML 元素 | C# 类 | 说明 |
|----------|-------|------|
| `<DATAOBJ>` | `DataObject` | 数据对象，通过 `dtref` 引用数据类型 |
| `<STRUCT>` | `BitsField` | 位级字段集合 |
| `<GAPDATAOBJ>` | `Gap` | 保留/填充位 |

**转换逻辑：**
1. 按顺序遍历所有子对象
2. 对每个子对象，从前端截取对应字节
3. 逐个调用 `Convert()` 解码
4. 收集结果为 `List<ConvertResult>`

### 5.6 BitsField / STRUCT (位级字段)

`Packet` 的子类，使用**位级截取**代替字节级截取。允许亚字节字段（如 1 位、4 位字段紧凑排列）。

```
字节: [1 0 1 1 0 0 1 0]
       ├─┤ ├─────┤ ├─┤
       2bit  5bit  1bit
```

通过将字节转为二进制字符串，再按位偏移精确切割实现。

### 5.7 Multiplexer (多路复用)

选择器字段 + 多个条件分支结构。

```xml
<MUXDT bm='4294967295' dtref='_0000022E1EFA53E0'>
  <STRUCTURE><!-- 默认结构 --></STRUCTURE>
  <CASE s='17' e='17'>
    <STRUCTURE><!-- 选择器=17 时的结构 --></STRUCTURE>
  </CASE>
  <CASE s='18' e='18'>
    <STRUCTURE><!-- 选择器=18 时的结构 --></STRUCTURE>
  </CASE>
</MUXDT>
```

**转换逻辑：**
1. 通过 `dtref` 引用的数据类型截取选择器字段的字节
2. 解码选择器值
3. 匹配满足 `值 ∈ [s, e]` 的 `<CASE>`
4. 使用该 CASE 的 `<STRUCTURE>` 转换剩余数据
5. 无匹配时使用默认 `<STRUCTURE>`

### 5.8 Gap (填充)

保留/填充位或字节，仅消耗指定的 `bl` 位长度，返回成功但无实际值。

```xml
<GAPDATAOBJ bl='8'/>
```

---

## 6. EXCL (无效值范围)

在数据类型中定义无效值区间：

```xml
<EXCL s='0' e='0' inv='invalidSignal'/>
<EXCL s='128' e='255' inv='invalidSignal'/>
```

| 属性 | 说明 |
|------|------|
| `s` | 范围起始值 |
| `e` | 范围结束值 |
| `inv` | 无效原因字符串 |
| `text` | 可选的说明文本 |

解码值落在 EXCL 范围内时，转换返回错误并附带无效原因。

---

## 7. 数据标识符 (DIDS)

`<DID>` 定义 UDS 22/2E 服务使用的数据标识符。

```xml
<DID id='_0000022E1EDA70F0' n='17392'>
  <NAME><TUV xml:lang='en-US'>Alignment Status</TUV></NAME>
  <STRUCTURE>
    <DATAOBJ spec='no' dtref='_0000022E1F00C520'>
      <NAME><TUV xml:lang='en-US'>Active alignment</TUV></NAME>
    </DATAOBJ>
    <DATAOBJ spec='no' dtref='_0000022E1F0A5710'>
      <NAME><TUV xml:lang='en-US'>Sensor Alignment Progress</TUV></NAME>
    </DATAOBJ>
  </STRUCTURE>
</DID>
```

- `n` 属性：DID 数字标识（如 `17392` = 0x43F0）
- `<STRUCTURE>`：包含 `Packet`，子 `<DATAOBJ>` 通过 `dtref` 引用数据类型
- `Convert()` 委托给内部 `Packet.Convert(rawData)`

---

## 8. 协议服务 (PROTOCOLSERVICES)

每个 `<PROTOCOLSERVICE>` 定义一个 UDS 服务，包含请求、肯定响应、否定响应。

```xml
<PROTOCOLSERVICE func='1' phys='1' respOnPhys='1' respOnFunc='1'>
  <NAME><TUV xml:lang='en-US'>($10) DiagnosticSessionControl</TUV></NAME>
  <QUAL>DSC</QUAL>
  <REQ>...</REQ>
  <POS>...</POS>
  <NEG>...</NEG>
</PROTOCOLSERVICE>
```

### 8.1 服务属性

| 属性 | 说明 |
|------|------|
| `func` | 支持功能寻址 |
| `phys` | 支持物理寻址 |
| `mresp` | 混合响应 |
| `respOnFunc` | 功能寻址响应 |
| `respOnPhys` | 物理寻址响应 |
| `maycombcont` | 可合并内容 |

### 8.2 组件类型

| XML 元素 | C# 类 | 说明 |
|----------|-------|------|
| `<CONSTCOMP>` | `ConstComponent` | 固定常量字节（SID 等） |
| `<STATICCOMP>` | `StaticComponent` | 静态参数（子功能） |
| `<CONTENTCOMP>` | `ContentComponent` | 内容组件（包裹 SIMPLECOMPCONT） |
| `<SIMPLEPROXYCOMP>` | `SimpleProxyComponent` | 变长数据代理 |
| `<GROUPOFDTCPROXYCOMP>` | `BaseComponent` | DTC 组代理 |

### 8.3 请求/响应数据提取

从组件中提取服务标识和数据：

```
RequestId  = 第一个 spec='sid' 的 CONSTCOMP 的值（1 字节）
RequestData = 其余 CONSTCOMP 的值
```

示例（$10 DiagnosticSessionControl）：

```
请求:  [0x10] [sub-function]
        ↑SID   ↑RequestData

肯定响应: [0x50] [sub-function] [data...]
           ↑SID+0x40

否定响应: [0x7F] [0x10] [NRC]
           ↑固定   ↑原SID  ↑响应码
```

---

## 9. ECU 变体与诊断层级

```
ECU
 └─ VAR (变体)                        如 "Base Variant"
     ├─ DIDREFS                       支持的 DID 引用列表
     ├─ DIAGCLASS (诊断类)            如 "Sessions"
     │    └─ DIAGINST (诊断实例)      如 "Default Session"
     │         ├─ SERVICE             通过 tmplref → DCLSRVTMPL → ProtocolService
     │         ├─ STATICVALUE         子功能字节值
     │         └─ SIMPLECOMPCONT      DID 数据引用
     └─ DIAGINST (诊断实例)           直接挂载在变体下
```

### 9.1 关键解析链

**服务解析链：**
```
Service.tmplref
  → DCLSRVTMPL.id（在 DCLTMPLS 中查找）
    → DCLSRVTMPL.tmplref
      → ProtocolService.id（在 PROTOCOLSERVICES 中查找）
```

**DID 数据解析链：**
```
SIMPLECOMPCONT.DidDataRef.didRef
  → DataIdentity.id（在 DIDS 中查找）
    → DataIdentity.DataType（Packet）
      → 通过 dtref 引用各数据类型
```

### 9.2 诊断类模板 (DCLTMPLS)

模板定义了诊断实例引用的服务结构：

```xml
<DCLTMPL id='...' cls='ses' single='0'>
  <NAME><TUV>Sessions</TUV></NAME>
  <DCLSRVTMPL id='...' tmplref='...' dtref='...' conv='req'>
    <NAME><TUV>Start</TUV></NAME>
  </DCLSRVTMPL>
  <SHSTATIC spec='sub'>
    <STATICCOMPREF idref='...'/>
  </SHSTATIC>
</DCLTMPL>
```

- `DCLSRVTMPL`：通过 `tmplref` 关联到具体 ProtocolService
- `SHSTATIC`：静态组件引用，将模板中的子功能定义链接到实际的 CONSTCOMP/STATICCOMP

---

## 10. 交叉引用系统

### 10.1 SetCanDelaReference

XML 反序列化完成后，必须调用 `SetCanDelaReference()` 递归传递根 `CanDela` 引用，否则所有运行时交叉引用（`dtref`、`tmplref`、`didRef`）均返回 null。

```
CanDela.SetCanDelaReference()
  └─ EcuDocument.SetCanDelaReference()
       ├─ DataTypes.DataObjects[].SetCanDelaReference()
       ├─ DataIdentities.DataIdentities[].SetCanDelaReference()
       ├─ ProtocolServices.ProtocolServices[].SetCanDelaReference()
       ├─ DiagnosticClassTemplates[].SetCanDelaReference()
       └─ ECU.SetCanDelaReference()
            └─ Variants[].SetCanDelaReference()
                 └─ DiagnosticObjects[].SetCanDelaReference()
```

### 10.2 dtref 引用解析

`General.DataTypeReference` 属性通过 `dtref` 查找匹配的数据类型：

```
CanDela.EcuDocument.DataTypes.DataObjects
  .Find(obj => obj.Id == this.DataTypeReferenceId)
```

---

## 11. 转换引擎

### 11.1 入口

```csharp
IConvert.Convert(byte[] rawData) → ConvertResult
```

**ConvertResult 结构：**

| 字段 | 说明 |
|------|------|
| `Name` | 字段名称 |
| `IsSuccessful` | 是否成功 |
| `RawData` | 原始字节 |
| `CodeResult` | 编码值（Value、ValueString、Unit） |
| `PhysicalResult` | 物理值（Value、ValueString、Unit） |
| `Error` | 错误类型和消息 |

### 11.2 转换流程

```
rawData
  │
  ▼
┌─ dtref 存在？ ─── 是 ──→ 委托给引用的 DataType.Convert()
│
│ 否
▼
ApplyBitMask(rawData)       ← bytes & maskBytes
  │
  ▼
ConvertCodeValue(rawData)   ← 用 CVALUETYPE 解码
  │  ├─ 数值: InterpretToDecimal(bytes, enc, bo)
  │  ├─ 文本: InterpretToText(bytes, enc, bo, sz)
  │  └─ 数组: InterpretToDecimalField() / InterpretToTextField()
  │
  ▼
CheckValidity()             ← 检查 EXCL 无效值范围
  │
  ▼
各子类实现转换逻辑:
  ├─ Identity:     physical = code（透传）
  ├─ TextTable:    physical = TEXTMAP 匹配的文本
  ├─ Linear:       physical = factor/div × code + offset
  ├─ Packet:       顺序解码子对象
  └─ Multiplexer:  先解码选择器，再按分支解码
```

### 11.3 字节截取

| 方法 | 用途 |
|------|------|
| `InterceptBytes(bytes, out rest)` | 按字节截取，用于 Packet |
| `InterceptBits(bytes, out rest)` | 按位截取，用于 BitsField |

**位级截取原理：** 将字节转为二进制字符串，按位偏移精确切割。例如从字节 `[10110010]` 中提取第 3–5 位 `110`。

---

## 12. 数据类型交叉引用关系

```
                         ┌─────────────────┐
                         │    CanDela       │
                         └────────┬────────┘
                                  │
                         ┌────────┴────────┐
                         │   EcuDocument    │
                         └──┬───┬───┬───┬──┘
              ┌─────────────┘   │   │   └──────────────┐
              ▼                 ▼   ▼                   ▼
       ┌──────────┐    ┌────────┐ ┌────────────┐  ┌─────────┐
       │ DataTypes│    │  DIDs  │ │ ProtocolSvc│  │   ECU   │
       └────┬─────┘    └───┬────┘ └─────┬──────┘  └────┬────┘
            │              │            │               │
    ┌───┬───┴──┬────┐     │      ┌─────┴─────┐    ┌───┴────┐
    │   │      │    │     │      │ REQ/POS/NEG│    │Variant │
   ID LT  TT  PKT  MUX   │      └─────┬─────┘    └───┬────┘
       │              │   │      ┌─────┴────┐    ┌────┴────┐
       │              │   │      │Component │    │DiagClass│
       │              │   │      └──────────┘    └────┬────┘
       │              │   │                      ┌────┴─────┐
       │              │   │                      │DiagInst  │
       │              │   │                      ├─Service──┤
       │              │   │                      │  ┌───┐   │
       │              │   │                      │  │ DCLSRVTMPL
       │              │   │                      │  └─┬─┘   │
       │              │   │                      │    │     │
       │              │   │                      │  ProtocolService
       │              │   │                      │
       │              │   └───── dtref ──────────┼──→ DataType
       │              │                          │
       │              └────── didRef ────────────┼──→ DataIdentity
       │                                         │    └── dtref ──→ DataType
       │                                         │
       └─────────── dtref ──────────────────────┘

图例: ID=Identity, LT=Linear, TT=TextTable, PKT=Packet, MUX=Multiplexer
```

### 关键引用关系汇总

| 引用类型 | 属性位置 | 解析目标 |
|---------|---------|---------|
| `dtref` | DATAOBJ / DCLSRVTMPL / 结构子元素 | DataType.id |
| `tmplref` (Service) | SERVICE 元素 | DCLSRVTMPL.id |
| `tmplref` (DCLSRVTMPL) | DCLSRVTMPL 元素 | ProtocolService.id |
| `didRef` | SIMPLECOMPCONT → DidDataRef | DataIdentity.id |
| `idref` | STATICCOMPREF / PROXYCOMPREF | 对应组件的 id |

---

## 13. 反序列化流程

```
 string xml
    │
    ▼
 DeserializeXmlToObject<CanDela>()
    │  (System.Xml.Serialization.XmlSerializer)
    ▼
 CanDela 对象树
    │
    ▼
 SetCanDelaReference()        ← 必须调用！递归传递根引用
    │
    ▼
 可用状态:
    ├─ 遍历 ECU → Variant → DiagnosticClass → DiagnosticInstance
    ├─ 解析 Service → ProtocolService（通过 DCLSRVTMPL）
    ├─ 解析 SimpleComponentCont → DataIdentity（通过 DidDataRef）
    └─ 调用 IConvert.Convert(rawBytes) 解码诊断数据
```

### 使用示例

```csharp
// 1. 加载 .cdd 文件
var xml = File.ReadAllText("FLR3.cdd");
var candela = xml.DeserializeXmlToObject<CanDela>();

// 2. 建立交叉引用
candela.SetCanDelaReference();

// 3. 遍历 ECU 变体
var variant = candela.EcuDocument.Ecu.Variants[0];
foreach (var didRef in variant.DidRefs)
{
    var did = didRef.DataIdentity;
    Console.WriteLine($"DID {did.N}: {did.Name}");
}

// 4. 解码原始字节
var rawData = new byte[] { 0x00, 0x05 };
var result = dataIdentity.Convert(rawData);
Console.WriteLine($"{result.Name}: {result.PhysicalResult.ValueString}");
```

---

## 14. 自定义属性定义 (DEFATTS)

| XML 元素 | C# 类 | 说明 |
|----------|-------|------|
| `<ENUMDEF>` | `EnumDefine` | 枚举属性 |
| `<UNSDEF>` | `UnsignedDefine` | 无符号整数属性 |
| `<CSTRDEF>` | `CStrDefine` | 常用字符串属性 |
| `<STRDEF>` | `StrDefine` | 字符串属性 |

分类包括：DATAOBJATTS、DATATYPEATTS、DIAGCLASSATTS、ECUATTS、JOBATTS、SERVICEATTS、VARATTS 等。

---

## 15. 版本差异

| 特性 | v9.x | v12.x |
|------|------|-------|
| `supportsTextIds` | 有 | 无 |
| `docCheckedAndValid` | 无 | 有 |
| `supportsEvents` | 无 | 有 |
| `defaultDataTypePathForBitfieldMember` | 有 | 无 |
| 数据类型集合 | 相同的五种 | 相同的五种 |
| 核心结构 | 一致 | 一致 |

核心解析逻辑对两个版本均兼容。
