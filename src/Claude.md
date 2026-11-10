# src/ — Code Conventions

- C# 12 / .NET 10 minimum (check `*.csproj`).
- Nullable reference types enabled everywhere. Never suppress with `!` without a comment.
- `var` when type is obvious from the right-hand side, explicit type otherwise.
- No `region` blocks. If a class needs regions, it needs splitting.

## Naming

| Construct                    | Convention     |
| ---------------------------- | -------------- |
| Classes, records, interfaces | `PascalCase`   |
| Methods, properties          | `PascalCase`   |
| Private fields               | `_camelCase`   |
| Local variables, parameters  | `camelCase`    |
| Constants                    | `PascalCase`   |
| Async methods                | suffix `Async` |

## Formatting

- 4-space indent, no tabs. Max line length: 120 chars.
- Opening brace: new line for type/method declarations, same line for lambdas.
- One blank line between members, two between top-level type declarations.

## Language features to prefer

- Primary constructors for simple record/DTO types.
- `required` properties over constructor-enforced init where it aids readability.
- Pattern matching over chains of `is`/`as` casts.
- `IAsyncEnumerable` for streaming — never buffer large sequences into `List<T>` just to return them.
- Avoid `dynamic`; if used, explain why in a comment.

## Security

- **No secrets in code.** Flag hardcoded credentials immediately.
- **No raw SQL string concatenation.** Parameterised queries or EF Core LINQ only.
- **Validate all external input** at the slice endpoint (FluentValidation where already in use).
- **Authorization checks belong in the handler or a dedicated behavior**, not in infrastructure.
- **Never log PII** — log correlation IDs instead.

## Git

Conventional Commits:

```
<type>(<scope>): <short description>
```

Types: `feature`, `bugfix`, `hotfix`

- Each commit is a logical, buildable unit. No mixing refactoring and feature changes.
- No commented-out code, `TODO` without an issue reference, or debug output.
- Migration + model change → same commit.
- Branch naming: `<type>/<short-slug>` (e.g. `feature/order-place`).

## NuGet-Pakete — ADR-Pflicht

**Kein neues Paket ohne ADR.** Das gilt ohne Ausnahme — auch für kleine Utilities und Testhelper.

### ADR-Prozess

Vor jeder neuen Abhängigkeit muss ein ADR unter `docs/adr/` erstellt werden, das:

- mindestens **2 Alternativen** (inkl. "kein Paket / selbst implementieren") gegenüberstellt,
- Lizenz, Lizenzmodell und kommerzielle Nutzbarkeit jeder Alternative explizit prüft,
- eine begründete Entscheidung dokumentiert.

Erst nach expliziter Freigabe durch den Entwickler installieren und die Version pinnen.

### Lizenz-Filter — sofortiger Ausschluss

Pakete mit folgenden Lizenzmodellen werden **nicht** verwendet — kein ADR nötig, direkt ablehnen:

- Kommerziell kostenpflichtig (auch "free for non-commercial")
- Shareware / Trialware
- Business Source License (BSL) oder ähnliche delayed-OSS-Modelle
- Dual-License wo die OSS-Variante kommerzielle Nutzung einschränkt

Akzeptiert: MIT, Apache 2.0, BSD 2/3-Clause, MPL 2.0, LGPL (mit Vorsicht bei statischem Linking).

> **Präzedenzfall:** FluentAssertions ist ab v8 kostenpflichtig für kommerzielle Projekte → ausgeschlossen.
> Ersatz: **Shouldly** (MIT), dokumentiert per ADR.

### Wenn du ein Paket vorschlagen willst

1. Nicht installieren. Einen ADR-Entwurf vorschlagen und auf Freigabe warten.
2. Lizenz auf der NuGet-Seite, GitHub und der LICENSE-Datei im Repo verifizieren — nicht nur das Label.
3. Mindestens zwei Alternativen benennen, darunter immer "selbst implementieren".

## Project-Specific Overrides

> Add project-specific rules here — this section wins over everything above.

```
# Example:
# - EF Core provider: Npgsql
# - Message broker: MassTransit on RabbitMQ
# - Auth: Keycloak via OpenIdConnect middleware
```
