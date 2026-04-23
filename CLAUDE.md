# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

VectorCdd — a C# (.NET 10.0) library for parsing Vector CANoe `.cdd` (CANdela Diagnostic Description) files. The `.cdd` format is XML based on the CANDELA schema. The library deserializes `.cdd` XML into a rich object model and converts raw UDS diagnostic byte data into human-readable values.

## Build & Test

```bash
cd src/VectorCdd
dotnet build
dotnet test
dotnet test --filter "FullyQualifiedName~TestClassName"  # run specific test class
dotnet pack   # produces NuGet package (GeneratePackageOnBuild=true)
```

Solution file: `src/VectorCdd.sln`

## Architecture

### Inheritance Hierarchy

All CDD model objects derive from a common base:
`CanDelaBase` → `General` → domain classes (data types, ECU elements, protocol services, etc.)

`CanDelaBase` holds a reference to the root `CanDela` document. `General` adds common XML attributes: `id`, `oid`, `temploid`, multi-language `NAME`/`DESC`, and `dtref` (data type reference).

### XML Document Model

- `Common/CanDela.cs` — Root `<CANDELA>` element, contains `EcuDocument` + version
- `Common/EcuDocument.cs` — Top-level `<ECUDOC>` container holding DataTypes, DataIdentities, ProtocolServices, ECU variants, DiagnosticClassTemplates, RecordDataTablePool
- `XmlParser/Common.cs` — `DeserializeXmlToObject<T>()` for XML deserialization; `SerializeObjectToXml<T>()` for serialization; custom `XmlValueAttribute` for enum-to-XML mapping

### Conversion Engine (DataTypes/)

The core decoding logic. `IConvert` interface defines `Convert(byte[] rawData)` for any object that can decode raw diagnostic bytes.

- `BaseDataType.cs` — Base for all data types; handles bit masking, byte interception, code/physical value conversion
- `ValueType.cs` — Low-level binary interpretation: encoding types (Unsigned, Signed, BCD, Float, ASCII, UNICODE), byte order (Intel `bo='12'` / Motorola `bo='21'`), display formats (Dec, Hex, Oct, Bin)
- `Identity.cs` — Pass-through (no transformation)
- `Linear.cs` — `physical = (code × factor / divisor) + offset` with valid range checking
- `TextTable.cs` — Maps numeric values to multi-language text strings
- `Packet.cs` — Compound data type; child data objects decoded sequentially
- `BitsField.cs` — Bit-level structured data (extends Packet)
- `Multiplexer.cs` — Switch-case decoding based on multiplexer selector value

### ECU Model

- `ECU/ECU.cs` → Variants → DiagnosticClasses → DiagnosticInstances → Services
- `DiagnosticInstance.cs` — implements `IConvert`; contains `SimpleComponentCont` (links to DataIdentity via `DidDataRef`)
- `DataIdentities/DataIdentity.cs` — DID with numeric ID and Packet data type; implements `IConvert`

### Protocol Services

- `ProtocolServices/ProtocolService.cs` — Request, PositiveResponse, NegativeResponse
- `RequestResponseBase.cs` — Components (ConstComponent, StaticComponent, ContentComponent); extracts RequestId/RequestData from ConstComponents
- `ProtocolServices/BaseComponent.cs` — `ComponentBitLength` attribute maps to XML `bl` (renamed from `BitLength` to avoid shadowing `ValueType.BitLength`)

### Test Project

- MSTest framework with `[DataRow]` parameterized tests
- `TestHelpers/CddTestData.cs` — centralized .cdd file loading with Lazy caching
- Test project: `src/VectorCdd.Tests/`
- Tests organized by source structure: `DataTypes/`, `DataIdentities/`, `Integration/`, `XmlParser/`
- Integration tests parse real `.cdd` files from `Resource/` directory

## Usage Pattern

```csharp
var xml = File.ReadAllText("sample.cdd");
var candela = xml.DeserializeXmlToObject<CanDela>();
candela.SetCanDelaReference();  // wire up cross-references
// Navigate: candela.EcuDocument.Ecu.Variants[0].DiagnosticInstances...
// Decode: dataIdentity.Convert(rawUdsBytes);
```

## Key Extension Methods

`Extend.cs` provides hex/binary/octal string conversions, BCD decoding, and bit manipulation utilities used throughout the library.
