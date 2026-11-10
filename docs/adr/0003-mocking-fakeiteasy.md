# ADR-0003: Mocking mit FakeItEasy

- Status: **Vorgeschlagen** (wartet auf Freigabe, nichts installiert)
- Betrifft: `tests/Unit/LazyLoggerExtensions.Tests.csproj`

## Kontext

`ILogger` muss in Tests ersetzt werden, um Aufrufe (Level, EventId, Exception, State, formatierte Message)
zu beobachten.

## Alternativen

| Option | Lizenz | Kommerziell nutzbar | Bewertung |
| --- | --- | --- | --- |
| **A: FakeItEasy 9.0.1** | MIT | ja | Laut `tests/CLAUDE.md` verbindlich |
| B: Kein Paket / selbst implementieren | – | – | Handgeschriebener `TestLogger : ILogger` möglich, aber mehr Wartung und ohne Fake-Konventionen des Projekts |
| C: Moq | BSD-3-Clause | ja | Laut `tests/CLAUDE.md` ausgeschlossen |
| D: NSubstitute | BSD-2-Clause | ja | Laut `tests/CLAUDE.md` ausgeschlossen |

## Lizenzprüfung

Verifiziert an der `.nuspec` im lokalen NuGet-Cache (MIT, Repository `github.com/FakeItEasy/FakeItEasy`).
Transitive Abhängigkeiten (u. a. Castle.Core) sind vor Freigabe zu prüfen. GitHub-LICENSE und NuGet-Seite
sind noch vom Entwickler gegenzuprüfen.

## Entscheidung (Vorschlag)

Option A, Version **9.0.1** gepinnt.
