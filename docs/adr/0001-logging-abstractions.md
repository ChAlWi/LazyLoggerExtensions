# ADR-0001: Dependency on Microsoft.Extensions.Logging.Abstractions

- Status: **Proposed** (awaiting approval)
- Affects: `src/LazyLoggerExtensions.csproj`

## Context

The library extends `ILogger` with lazy overloads. This requires `ILogger`, `LogLevel`, `EventId` and the framework
extension `LoggerExtensions.Log(...)`, which formats message templates and produces structured logging properties.

## Alternatives

| Option | License | Commercial use | Assessment |
| --- | --- | --- | --- |
| **A: Microsoft.Extensions.Logging.Abstractions 10.0.14** | MIT | yes | Standard abstraction; `ILogger` is not available without the package |
| B: No package / implement ourselves | – | – | Not possible: a custom `ILogger` would be compatible with no logging provider |
| C: Serilog (`ILogger`/`Log.Logger`) | Apache 2.0 | yes | Ties consumers to Serilog instead of the framework abstraction |

## License review

Verified against the `.nuspec` in the local NuGet cache (`license type="expression"`: MIT, repository
`github.com/dotnet/dotnet`). The LICENSE file in the GitHub repository and the NuGet page still have to be
cross-checked by the developer (not verifiable offline).

## Decision (proposal)

Option A, version **10.0.14**, pinned via `Directory.Packages.props`.
