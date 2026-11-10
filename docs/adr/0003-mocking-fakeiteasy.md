# ADR-0003: Mocking with FakeItEasy

- Status: **Proposed** (awaiting approval)
- Affects: `tests/Unit/LazyLoggerExtensions.Tests.csproj`

## Context

`ILogger` must be replaced in tests in order to observe calls (level, EventId, exception, state, formatted
message).

## Alternatives

| Option | License | Commercial use | Assessment |
| --- | --- | --- | --- |
| **A: FakeItEasy 9.0.1** | MIT | yes | Mandatory per `tests/CLAUDE.md` |
| B: No package / implement ourselves | – | – | A hand-written `TestLogger : ILogger` is possible, but needs more maintenance and does not follow the project's fake conventions |
| C: Moq | BSD-3-Clause | yes | Excluded per `tests/CLAUDE.md` |
| D: NSubstitute | BSD-2-Clause | yes | Excluded per `tests/CLAUDE.md` |

## License review

Verified against the `.nuspec` in the local NuGet cache (MIT, repository `github.com/FakeItEasy/FakeItEasy`).
Transitive dependencies (e.g. Castle.Core) must be reviewed before approval. The GitHub LICENSE file and the NuGet
page still have to be cross-checked by the developer.

## Decision (proposal)

Option A, version **9.0.1**, pinned.
