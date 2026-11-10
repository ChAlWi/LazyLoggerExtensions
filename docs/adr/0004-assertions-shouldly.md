# ADR-0004: Assertions mit Shouldly

- Status: **Vorgeschlagen** (wartet auf Freigabe, nichts installiert)
- Betrifft: `tests/Unit/LazyLoggerExtensions.Tests.csproj`

## Alternativen

| Option | Lizenz | Kommerziell nutzbar | Bewertung |
| --- | --- | --- | --- |
| **A: Shouldly 4.3.0** | BSD-3-Clause | ja | Laut `tests/CLAUDE.md` verbindlich, Ersatz für FluentAssertions |
| B: Kein Paket / `Xunit.Assert` | Apache-2.0 (Teil von xUnit) | ja | Kein zusätzliches Paket, aber widerspricht dem verbindlichen Tooling |
| C: FluentAssertions ab v8 | kommerziell | nein | Sofortiger Ausschluss (Lizenzfilter) |
| D: AwesomeAssertions | Apache-2.0 | ja | Fork von FluentAssertions, nicht Projektstandard |

## Lizenzprüfung

Verifiziert an der `.nuspec` im lokalen NuGet-Cache (BSD-3-Clause, Repository `github.com/shouldly/shouldly`).
Transitive Abhängigkeiten sind vor Freigabe zu prüfen. GitHub-LICENSE und NuGet-Seite sind noch vom
Entwickler gegenzuprüfen.

## Entscheidung (Vorschlag)

Option A, Version **4.3.0** gepinnt.

## Nicht Teil dieses Vorschlags

AutoFixture (`tests/CLAUDE.md`) wird vorerst nicht eingeführt: Alle Testwerte sind hier inhaltlich relevant
(Message-Templates, Level, Overload-Variante). Bei Bedarf eigener ADR.
