# ADR-0001: Abhängigkeit auf Microsoft.Extensions.Logging.Abstractions

- Status: **Vorgeschlagen** (wartet auf Freigabe, nichts installiert)
- Betrifft: `src/LazyLoggerExtensions.csproj`

## Kontext

Die Library erweitert `ILogger` um Lazy-Overloads. Dafür werden `ILogger`, `LogLevel`, `EventId` und die
Framework-Extension `LoggerExtensions.Log(...)` benötigt, die Message-Templates formatiert und
Structured-Logging-Properties erzeugt.

## Alternativen

| Option | Lizenz | Kommerziell nutzbar | Bewertung |
| --- | --- | --- | --- |
| **A: Microsoft.Extensions.Logging.Abstractions 10.0.14** | MIT | ja | Standard-Abstraktion; `ILogger` ist ohne das Paket nicht verfügbar |
| B: Kein Paket / selbst implementieren | – | – | Nicht möglich: eigenes `ILogger` wäre mit keinem Logging-Provider kompatibel |
| C: Serilog (`ILogger`/`Log.Logger`) | Apache 2.0 | ja | Bindet Consumer an Serilog statt an die Framework-Abstraktion |

## Lizenzprüfung

Verifiziert an der `.nuspec` im lokalen NuGet-Cache (`license type="expression"`: MIT, Repository
`github.com/dotnet/dotnet`). Die LICENSE-Datei im GitHub-Repository und die NuGet-Seite sind noch vom Entwickler
gegenzuprüfen (offline nicht verifizierbar).

## Entscheidung (Vorschlag)

Option A, Version **10.0.14** gepinnt über `Directory.Packages.props`.
