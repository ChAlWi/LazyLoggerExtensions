# ADR-0002: Test-Framework xUnit inkl. Test-SDK und Runner

- Status: **Vorgeschlagen** (wartet auf Freigabe, nichts installiert)
- Betrifft: `tests/Unit/LazyLoggerExtensions.Tests.csproj`

## Pakete

| Paket | Version | Lizenz |
| --- | --- | --- |
| xunit | 2.9.3 | Apache-2.0 |
| xunit.runner.visualstudio | 3.1.5 | Apache-2.0 |
| Microsoft.NET.Test.Sdk | 18.8.1 | MIT |

## Alternativen

| Option | Lizenz | Kommerziell nutzbar | Bewertung |
| --- | --- | --- | --- |
| **A: xUnit 2.9.3** | Apache-2.0 | ja | Laut `tests/CLAUDE.md` verbindlich, `dotnet test` direkt lauffähig |
| B: Kein Paket / selbst implementieren | – | – | Eigener Test-Runner mit `dotnet test`-Integration ist unverhältnismäßig |
| C: NUnit | MIT | ja | Funktioniert, widerspricht aber dem verbindlichen Tooling |
| D: xunit.v3 | Apache-2.0 | ja | Neuere Generation, nicht der aktuelle Projektstandard |

`Microsoft.NET.Test.Sdk` und `xunit.runner.visualstudio` sind für `dotnet test` mit xUnit technisch erforderlich.

## Lizenzprüfung

Verifiziert an den `.nuspec`-Dateien im lokalen NuGet-Cache. GitHub-LICENSE und NuGet-Seite sind noch vom
Entwickler gegenzuprüfen.

## Entscheidung (Vorschlag)

Option A mit den oben genannten, gepinnten Versionen.
