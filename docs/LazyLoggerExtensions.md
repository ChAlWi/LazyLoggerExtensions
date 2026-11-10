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

Kein zusätzliches `using` erforderlich – die Klasse liegt im selben Namespace:

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

---

## Unterstützte LogLevel und Overloads

Alle 6 LogLevel werden vollständig unterstützt:

| Level | Ohne Exception | Mit Exception | Mit EventId | Mit EventId + Exception |
|---|:---:|:---:|:---:|:---:|
| Trace | ✅ | ✅ | ✅ | ✅ |
| Debug | ✅ | ✅ | ✅ | ✅ |
| Information | ✅ | ✅ | ✅ | ✅ |
| Warning | ✅ | ✅ | ✅ | ✅ |
| Error | ✅ | ✅ | ✅ | ✅ |
| Critical | ✅ | ✅ | ✅ | ✅ |

Jeder Overload ist zusätzlich mit `params object?[] args` verfügbar.

---

## Technische Details

| Eigenschaft | Wert |
|---|---|
| Namespace | `Microsoft.Extensions.Logging` |
| Klassenname | `LazyLoggerExtensions` |
| Zieldatei | `LoggerExtensions.Lazy.cs` |
| Target Framework | .NET 8 / C# 12 |
| NuGet-Abhängigkeit | `Microsoft.Extensions.Logging.Abstractions` |

### Sicherheitsmerkmale

- **`ArgumentNullException`** bei `logger = null` oder `messageFactory = null`
- **`ArgumentNullException`** bei `exception = null` in Exception-Overloads
- **`InvalidOperationException`** wenn `messageFactory()` `null` zurückgibt
- **`[StackTraceHidden]`** auf allen public Methoden (saubere Stack Traces)
- **Kein doppeltes Array-Wrapping** – `ILogger.Log<TState>` wird direkt aufgerufen

---

## Tests

Datei: `LoggerExtensions.Lazy.Tests.cs`  
Framework: xUnit + Moq

Testabdeckung:
- ✅ Delegate wird **nicht** aufgerufen wenn Level deaktiviert (alle 6 Level)
- ✅ Delegate wird aufgerufen wenn Level aktiv (alle 6 Level)
- ✅ Exception-Overloads (Error, Critical, Warning)
- ✅ Null-Guard: `logger`, `messageFactory`, `exception`
- ✅ Factory gibt `null` zurück → `InvalidOperationException`

---

## QA-Status

| Bug | Beschreibung | Status |
|---|---|---|
| BUG-01 | Null-Return Factory → `InvalidOperationException` | ✅ Behoben |
| BUG-02 | Klassenname-Kollision → `LazyLoggerExtensions` | ✅ Behoben |
| BUG-03 | Exception-Overloads für Trace/Info/Warning fehlten | ✅ Behoben |
| BUG-04 | `params`-Array-Wrapping brach Structured Logging | ✅ Behoben |

**Gesamtstatus: ✅ GO – Freigegeben für Produktion**
