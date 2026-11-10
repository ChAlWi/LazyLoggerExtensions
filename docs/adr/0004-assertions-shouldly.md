# ADR-0004: Assertions with Shouldly

- Status: **Proposed** (awaiting approval)
- Affects: `tests/Unit/LazyLoggerExtensions.Tests.csproj`

## Alternatives

| Option | License | Commercial use | Assessment |
| --- | --- | --- | --- |
| **A: Shouldly 4.3.0** | BSD-3-Clause | yes | Mandatory per `tests/CLAUDE.md`, replacement for FluentAssertions |
| B: No package / `Xunit.Assert` | Apache-2.0 (part of xUnit) | yes | No additional package, but contradicts the mandatory tooling |
| C: FluentAssertions v8 and later | commercial | no | Immediate exclusion (license filter) |
| D: AwesomeAssertions | Apache-2.0 | yes | Fork of FluentAssertions, not the project standard |

## License review

Verified against the `.nuspec` in the local NuGet cache (BSD-3-Clause, repository `github.com/shouldly/shouldly`).
Transitive dependencies must be reviewed before approval. The GitHub LICENSE file and the NuGet page still have to be
cross-checked by the developer.

## Decision (proposal)

Option A, version **4.3.0**, pinned.

## Not part of this proposal

AutoFixture (`tests/CLAUDE.md`) is not introduced for now: all test values are relevant to the test content
(message templates, level, overload variant). A separate ADR if needed.
