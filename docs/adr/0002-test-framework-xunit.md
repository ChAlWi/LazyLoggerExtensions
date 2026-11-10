# ADR-0002: Test framework xUnit incl. test SDK and runner

- Status: **Proposed** (awaiting approval)
- Affects: `tests/Unit/LazyLoggerExtensions.Tests.csproj`

## Packages

| Package | Version | License |
| --- | --- | --- |
| xunit | 2.9.3 | Apache-2.0 |
| xunit.runner.visualstudio | 3.1.5 | Apache-2.0 |
| Microsoft.NET.Test.Sdk | 18.8.1 | MIT |

## Alternatives

| Option | License | Commercial use | Assessment |
| --- | --- | --- | --- |
| **A: xUnit 2.9.3** | Apache-2.0 | yes | Mandatory per `tests/CLAUDE.md`, works directly with `dotnet test` |
| B: No package / implement ourselves | – | – | A custom test runner with `dotnet test` integration is disproportionate |
| C: NUnit | MIT | yes | Works, but contradicts the mandatory tooling |
| D: xunit.v3 | Apache-2.0 | yes | Newer generation, not the current project standard |

`Microsoft.NET.Test.Sdk` and `xunit.runner.visualstudio` are technically required for `dotnet test` with xUnit.

## License review

Verified against the `.nuspec` files in the local NuGet cache. The GitHub LICENSE files and the NuGet pages still have
to be cross-checked by the developer.

## Decision (proposal)

Option A with the pinned versions listed above.
