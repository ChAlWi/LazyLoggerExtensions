# LazyLoggerExtensions – Documentation

## Overview

`LazyLoggerExtensions` extends `Microsoft.Extensions.Logging.ILogger` with overloads that accept a
`Func<string>` delegate (factory) as the message parameter.
The delegate is **only invoked when the corresponding log level is enabled**
(lazy evaluation / short-circuit).

---

## Problem solved

Standard logging **always** evaluates the message string, even when the log level is disabled:

```csharp
// BAD: the expensive string is always built
_logger.LogDebug($"Complex object: {JsonSerializer.Serialize(bigObject)}");
```

With `LazyLoggerExtensions` the string is only created when it is actually logged:

```csharp
// GOOD: the string is built ONLY when the Debug level is enabled
_logger.LogDebug(() => $"Complex object: {JsonSerializer.Serialize(bigObject)}");
```

---

## Usage

No additional `using` is required – the class lives in the `Microsoft.Extensions.Logging` namespace
(the namespace of `ILogger`):

```csharp
// Simple message
logger.LogDebug(() => "This is a test for {DebugLevel}", "DebugLevel");

// With exception
logger.LogError(ex, () => "Error in method {Method}", nameof(MyMethod));

// With EventId
logger.LogWarning(new EventId(42, "TimeoutWarning"), () => "Timeout after {Seconds}s", 30);

// With EventId + exception
logger.LogCritical(new EventId(99), ex, () => "Critical error: {Detail}", detail);
```

### Usage notes

- **The message is a template.** When the factory is called together with `args`, the framework parses the text for
  `{placeholders}`. Curly braces in interpolated values (e.g. JSON) may then be mistaken for placeholders.
  Without `args` the text is logged unchanged.
- **The lambda itself is not free.** A lambda that captures variables (closure) allocates even when the level is
  disabled. The benefit is avoiding the expensive *message creation*, not an allocation-free call site.
  With `static () => ...` and no captures the allocation is avoided as well.
- **Exceptions thrown by the factory** are not caught but propagate to the caller.

---

## Supported log levels and overloads

All 6 log levels (`Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`) provide the same
**8 overloads** (`Log<Level>`), i.e. 48 public methods in total:

| # | Parameters (after `this ILogger logger`) |
|---|---|
| 1 | `Func<string> messageFactory` |
| 2 | `Func<string> messageFactory, params object?[] args` |
| 3 | `EventId eventId, Func<string> messageFactory` |
| 4 | `EventId eventId, Func<string> messageFactory, params object?[] args` |
| 5 | `Exception exception, Func<string> messageFactory` |
| 6 | `Exception exception, Func<string> messageFactory, params object?[] args` |
| 7 | `EventId eventId, Exception exception, Func<string> messageFactory` |
| 8 | `EventId eventId, Exception exception, Func<string> messageFactory, params object?[] args` |

---

## Technical details

| Property | Value |
|---|---|
| Namespace | `Microsoft.Extensions.Logging` |
| Class name | `LazyLoggerExtensions` |
| Source file | `src/LoggerExtensions.Lazy.cs` |
| Project | `src/LazyLoggerExtensions.csproj` |
| Target framework | .NET 10 (usable from .NET 10 and later) |
| NuGet package | `LazyLoggerExtensions` 1.0.0 (`dotnet pack src/LazyLoggerExtensions.csproj -c Release`) |
| NuGet dependency | `Microsoft.Extensions.Logging.Abstractions` 10.0.14 (see [ADR-0001](adr/0001-logging-abstractions.md)) |
| License | MIT (see [LICENSE](../LICENSE)) |

The message is a **message template** (`{Name}` placeholders, not `{0}`). It is formatted by the framework
extension `ILogger.Log(...)`; named placeholders end up as structured logging properties.

### Safety features

- **`ArgumentNullException`** for `logger = null` or `messageFactory = null`
- **`ArgumentNullException`** for `exception = null` in the exception overloads
- **`InvalidOperationException`** when `messageFactory()` returns `null`
- **`[StackTraceHidden]`** on all public methods (clean stack traces)
- **No double array wrapping** – the `args` array is passed unchanged to `ILogger.Log(...)`

---

## Tests

File: `tests/Unit/LazyLoggerExtensionsTests.cs`  
Framework: xUnit + FakeItEasy + Shouldly (see [ADR-0002](adr/0002-test-framework-xunit.md),
[ADR-0003](adr/0003-mocking-fakeiteasy.md), [ADR-0004](adr/0004-assertions-shouldly.md))

Run (currently 578 tests, all green):

```bash
dotnet test LazyLoggerExtensions.slnx
```

If the private NuGet feed is unreachable (e.g. without VPN), the warning `NU1900`
(security audit not possible) appears. It is intentionally only a warning; restore and tests run with the
packages from the local cache.

Test coverage (each for **all 48 overloads**):
- ✅ Factory is **not** invoked and nothing is written when the level is disabled
- ✅ Factory is invoked exactly once when the level is enabled
- ✅ Level, EventId, exception and formatted message in the written entry
- ✅ Named placeholders and `{OriginalFormat}` as structured logging state
- ✅ Null guards: `logger`, `messageFactory`, `exception` (also when the level is disabled)
- ✅ Factory returns `null` → `InvalidOperationException`

---

## Project structure

```
LazyLoggerExtensions.slnx
Directory.Build.props        ← shared settings (net10.0, nullable, warnings as errors)
Directory.Packages.props     ← central, pinned package versions
src/                         ← library
tests/Unit/                  ← xUnit tests
docs/                        ← documentation and ADRs
```

---

## Architecture decisions (ADR)

New NuGet packages require an ADR (see `src/CLAUDE.md`).

| ADR | Topic | Status |
|---|---|---|
| [0001](adr/0001-logging-abstractions.md) | Microsoft.Extensions.Logging.Abstractions | Proposed, packages installed |
| [0002](adr/0002-test-framework-xunit.md) | xUnit, test SDK, runner | Proposed, packages installed |
| [0003](adr/0003-mocking-fakeiteasy.md) | FakeItEasy | Proposed, packages installed |
| [0004](adr/0004-assertions-shouldly.md) | Shouldly | Proposed, packages installed |

Open: cross-checking the licenses on the NuGet page and the GitHub LICENSE file, as well as the transitive
dependencies.

---

## QA status

| Bug | Description | Status |
|---|---|---|
| BUG-01 | Factory returning null → `InvalidOperationException` | ✅ Fixed |
| BUG-02 | Class name collision → `LazyLoggerExtensions` | ✅ Fixed |
| BUG-03 | Exception overloads for Trace/Info/Warning were missing | ✅ Fixed |
| BUG-04 | `params` array wrapping broke structured logging | ✅ Fixed |
| BUG-05 | `string.Format` threw `FormatException` for named placeholders (`{Name}`) and lost structured logging | ✅ Fixed (regression tests) |
| BUG-06 | `exception = null` was not checked in the exception overloads | ✅ Fixed |
