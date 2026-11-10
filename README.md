# LazyLoggerExtensions

Lazy overloads for `Microsoft.Extensions.Logging.ILogger`: the log message is produced by a `Func<string>` and is
**only evaluated when the log level is enabled**.

```csharp
// The expensive string is built ONLY when the Debug level is enabled
logger.LogDebug(() => $"Complex object: {JsonSerializer.Serialize(bigObject)}");

// Message templates with named placeholders remain structured logging
logger.LogError(ex, () => "Error in method {Method}", nameof(MyMethod));
```

## Why?

With `logger.LogDebug($"...")` the interpolated string is always built, even when `Debug` is disabled.
The lazy overloads avoid that work. Details, notes and limitations are in the
[documentation](docs/LazyLoggerExtensions.md).

## Features

- 6 log levels (`Trace` to `Critical`) with 8 overloads each, 48 methods in total
  (with/without `EventId`, `Exception` and `params object?[] args`)
- No additional `using` needed: the class lives in the `Microsoft.Extensions.Logging` namespace
- Message templates with `{placeholders}` are formatted by the framework and captured as structured logging properties
- Argument validation for `logger`, `messageFactory` and `exception`

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Usage

The project is currently **not** published as a NuGet package. To try it out, clone the repository and reference the
project:

```bash
git clone https://github.com/ChAlWi/LazyLoggerExtensions.git
```

```xml
<ProjectReference Include="path/to/LazyLoggerExtensions/src/LazyLoggerExtensions.csproj" />
```

## Build and test

```bash
dotnet build LazyLoggerExtensions.slnx
dotnet test LazyLoggerExtensions.slnx
```

If a machine's private NuGet feed is unreachable, the warning `NU1900` appears. It is intentionally only a warning.

## Project structure

```
src/        Library
tests/Unit/ xUnit tests
docs/       Documentation and architecture decision records (ADR)
```

## Contributing

Contributions are welcome. Please open an issue first for anything beyond a small change.

1. Fork the repository and create a branch named `<type>/<short-slug>`,
   e.g. `bugfix/null-check-exception`. Allowed types: `feature`, `bugfix`, `hotfix`.
2. Implement your change and follow the conventions in [`src/CLAUDE.md`](src/CLAUDE.md) and
   [`tests/CLAUDE.md`](tests/CLAUDE.md) (4 spaces, at most 120 characters per line, nullable enabled).
3. Write tests: new features and bug fixes need tests, bug fixes additionally need a regression test.
   The test stack is xUnit, FakeItEasy and Shouldly (no Moq, NSubstitute or FluentAssertions).
   Test names follow the pattern `<Action>_<Condition>_<ExpectedResult>`.
4. Make sure `dotnet test LazyLoggerExtensions.slnx` is green.
5. Write commits following [Conventional Commits](https://www.conventionalcommits.org/):
   `<type>(<scope>): <short description>` with the types `feature`, `bugfix`, `hotfix`.
   Every commit is a buildable unit; no commented-out code, no `TODO`s without an issue.
6. Open a pull request against `main` and briefly describe what changes and how it was tested.

### New NuGet packages

New dependencies, including test dependencies, require an ADR in [`docs/adr/`](docs/adr/) beforehand, with at least two
alternatives (one of them "implement it ourselves") and a license review. Packages with commercial, trial, BSL or
restrictive dual licenses are not used. Please do not add packages without approval.

## License

Released under the [MIT License](LICENSE).
