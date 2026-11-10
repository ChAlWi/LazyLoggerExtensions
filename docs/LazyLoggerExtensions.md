# LazyLoggerExtensions – Dokumentation

## Übersicht

`LazyLoggerExtensions` erweitert `Microsoft.Extensions.Logging.ILogger` um Overloads,
die einen `Func<string>`-Delegate (Factory) als Message-Parameter akzeptieren.
Der Delegate wird **nur aufgerufen, wenn der entsprechende LogLevel aktiv ist**
(Lazy Evaluation / Short-Circuit).

---

## Problem, das gelöst wird

Standard-Logging evaluiert den Message-String **immer**, selbst wenn der LogLevel deaktiviert ist:

```csharp
// SCHLECHT: teurer String wird immer aufgebaut
_logger.LogDebug($"Komplexes Objekt: {JsonSerializer.Serialize(bigObject)}");
```

Mit `LazyLoggerExtensions` wird der String nur erzeugt, wenn er tatsächlich geloggt wird:

```csharp
// GUT: String wird NUR bei aktivem Debug-Level erzeugt
_logger.LogDebug(() => $"Komplexes Objekt: {JsonSerializer.Serialize(bigObject)}");
```

---

## Verwendung

Kein zusätzliches `using` erforderlich – die Klasse liegt im Namespace `Microsoft.Extensions.Logging`
(dem Namespace von `ILogger`):

```csharp
// Einfache Nachricht
logger.LogDebug(() => "Dies ist ein Test für {DebugLevel}", "DebugLevel");

// Mit Exception
logger.LogError(ex, () => "Fehler in Methode {Method}", nameof(MyMethod));

// Mit EventId
logger.LogWarning(new EventId(42, "TimeoutWarning"), () => "Timeout nach {Seconds}s", 30);

// Mit EventId + Exception
logger.LogCritical(new EventId(99), ex, () => "Kritischer Fehler: {Detail}", detail);
```

### Hinweise zur Verwendung

- **Die Message ist ein Template.** Wird die Factory zusammen mit `args` aufgerufen, parst das Framework den
  Text nach `{Platzhaltern}`. Geschweifte Klammern in interpolierten Werten (z. B. JSON) können dann als
  Platzhalter missverstanden werden. Ohne `args` wird der Text unverändert geloggt.
- **Das Lambda selbst ist nicht kostenlos.** Ein Lambda, das Variablen einfängt (Closure), allokiert auch bei
  deaktiviertem Level. Der Vorteil liegt im Vermeiden der teuren *Message-Erzeugung*, nicht in einer
  allokationsfreien Aufrufstelle. Mit `static () => ...` ohne Captures entfällt auch die Allokation.
- **Exceptions in der Factory** werden nicht abgefangen, sondern an den Aufrufer weitergereicht.

---

## Unterstützte LogLevel und Overloads

Alle 6 LogLevel (`Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`) bieten dieselben
**8 Overloads** (`Log<Level>`), insgesamt also 48 öffentliche Methoden:

| # | Parameter (nach `this ILogger logger`) |
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

## Technische Details

| Eigenschaft | Wert |
|---|---|
| Namespace | `Microsoft.Extensions.Logging` |
| Klassenname | `LazyLoggerExtensions` |
| Quelldatei | `src/LoggerExtensions.Lazy.cs` |
| Projekt | `src/LazyLoggerExtensions.csproj` |
| Target Framework | .NET 10 |
| NuGet-Abhängigkeit | `Microsoft.Extensions.Logging.Abstractions` 10.0.14 (siehe [ADR-0001](adr/0001-logging-abstractions.md)) |

Die Message ist ein **Message-Template** (`{Name}`-Platzhalter, kein `{0}`). Sie wird von der
Framework-Extension `ILogger.Log(...)` formatiert; benannte Platzhalter landen als Structured-Logging-Properties.

### Sicherheitsmerkmale

- **`ArgumentNullException`** bei `logger = null` oder `messageFactory = null`
- **`ArgumentNullException`** bei `exception = null` in Exception-Overloads
- **`InvalidOperationException`** wenn `messageFactory()` `null` zurückgibt
- **`[StackTraceHidden]`** auf allen public Methoden (saubere Stack Traces)
- **Kein doppeltes Array-Wrapping** – das `args`-Array wird unverändert an `ILogger.Log(...)` übergeben

---

## Tests

Datei: `tests/Unit/LazyLoggerExtensionsTests.cs`  
Framework: xUnit + FakeItEasy + Shouldly (siehe [ADR-0002](adr/0002-test-framework-xunit.md),
[ADR-0003](adr/0003-mocking-fakeiteasy.md), [ADR-0004](adr/0004-assertions-shouldly.md))

Ausführen (aktuell 578 Tests, alle grün):

```bash
dotnet test LazyLoggerExtensions.slnx
```

Ist der private NuGet-Feed nicht erreichbar (z. B. ohne VPN), erscheint die Warnung `NU1900`
(Sicherheitsprüfung nicht möglich). Sie ist bewusst nur eine Warnung; Restore und Tests laufen mit den
Paketen aus dem lokalen Cache.

Testabdeckung (jeweils für **alle 48 Overloads**):
- ✅ Factory wird **nicht** aufgerufen und nichts geschrieben, wenn Level deaktiviert
- ✅ Factory wird genau einmal aufgerufen, wenn Level aktiv
- ✅ Level, EventId, Exception und formatierte Message im geschriebenen Eintrag
- ✅ Benannte Platzhalter und `{OriginalFormat}` als Structured-Logging-State
- ✅ Null-Guard: `logger`, `messageFactory`, `exception` (auch bei deaktiviertem Level)
- ✅ Factory gibt `null` zurück → `InvalidOperationException`

---

## Projektstruktur

```
LazyLoggerExtensions.slnx
Directory.Build.props        ← gemeinsame Einstellungen (net10.0, Nullable, Warnungen als Fehler)
Directory.Packages.props     ← zentrale, gepinnte Paketversionen
src/                         ← Library
tests/Unit/                  ← xUnit-Tests
docs/                        ← Dokumentation und ADRs
```

---

## Architekturentscheidungen (ADR)

Neue NuGet-Pakete benötigen einen ADR (siehe `src/CLAUDE.md`).

| ADR | Thema | Status |
|---|---|---|
| [0001](adr/0001-logging-abstractions.md) | Microsoft.Extensions.Logging.Abstractions | Vorgeschlagen, Pakete installiert |
| [0002](adr/0002-test-framework-xunit.md) | xUnit, Test-SDK, Runner | Vorgeschlagen, Pakete installiert |
| [0003](adr/0003-mocking-fakeiteasy.md) | FakeItEasy | Vorgeschlagen, Pakete installiert |
| [0004](adr/0004-assertions-shouldly.md) | Shouldly | Vorgeschlagen, Pakete installiert |

Offen: Gegenprüfung der Lizenzen auf NuGet-Seite und GitHub-LICENSE sowie der transitiven Abhängigkeiten.

---

## QA-Status

| Bug | Beschreibung | Status |
|---|---|---|
| BUG-01 | Null-Return Factory → `InvalidOperationException` | ✅ Behoben |
| BUG-02 | Klassenname-Kollision → `LazyLoggerExtensions` | ✅ Behoben |
| BUG-03 | Exception-Overloads für Trace/Info/Warning fehlten | ✅ Behoben |
| BUG-04 | `params`-Array-Wrapping brach Structured Logging | ✅ Behoben |
| BUG-05 | `string.Format` warf `FormatException` bei benannten Platzhaltern (`{Name}`) und verlor Structured Logging | ✅ Behoben (Regressionstests) |
| BUG-06 | `exception = null` wurde in Exception-Overloads nicht geprüft | ✅ Behoben |
