# tests/ — Testing Strategy & Conventions

```
tests/
├── Unit/     ← xUnit-Tests, eine Komponente isoliert
└── Specs/    ← ReqNRoll-Tests, ein Feature end-to-end im Gherkin-Format
```

## Tooling — verbindlich

| Zweck                 | Library                                  | Lizenz     |
| --------------------- | ---------------------------------------- | ---------- |
| Test-Framework        | xUnit                                    | Apache 2.0 |
| Mocking               | FakeItEasy                               | MIT        |
| Test-Datengenerierung | AutoFixture + AutoFixture.AutoFakeItEasy | MIT        |
| Assertions            | Shouldly                                 | BSD        |
| Specification-Tests   | ReqNRoll                                 | BSD        |
| Integrations-DB       | Testcontainers                           | MIT        |
| API-Tests             | `WebApplicationFactory<T>`               | (BCL)      |

**Nicht verwenden:** Moq, NSubstitute, FluentAssertions.
FluentAssertions ist ab v8 kommerziell lizenziert und damit ausgeschlossen (siehe ADR).
Wenn diese Libraries in bestehenden Tests auftauchen: nicht stillschweigend mischen —
als separaten Cleanup-Task melden.

---

## Unit/ — Komponenten-Tests

### Abdeckung

| Schicht                 | Test-Typ                               |
| ----------------------- | -------------------------------------- |
| Slice Handler (kein DB) | Unit                                   |
| Slice Handler (mit DB)  | Integration via Testcontainers         |
| MediatR Behaviors       | Unit                                   |
| Validators              | Unit                                   |
| Consumer                | Unit (Mediator per FakeItEasy gemockt) |

- Jeder neue Command-Handler **muss** mindestens einen Unit-Test für den Happy Path haben.
- Jeder Validator **muss** Tests für Inputs haben, die rejected werden sollen.
- Bug-Fixes **müssen** einen Regressionstest enthalten, der den ursprünglichen Fehler reproduziert.

### Struktur — AAA

```csharp
// Arrange
var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
var dbContext = fixture.Freeze<IAppDbContext>();
var handler = fixture.Create<PlaceOrderCommandHandler>();
var command = fixture.Create<PlaceOrderCommand>();

// Act
var result = await handler.Handle(command, CancellationToken.None);

// Assert
result.IsSuccess.ShouldBeTrue();
```

- Eine logische Assertion pro Test.
- AutoFixture für alle Testdaten — keine manuell konstruierten Objekte mit Magic Values,
  es sei denn der Wert ist für den Test inhaltlich relevant.
- Relevante Werte explizit setzen und per `fixture.Freeze<T>()` injecten.

### FakeItEasy-Konventionen

```csharp
var repo = A.Fake<IOrderRepository>();

A.CallTo(() => repo.GetByIdAsync(command.OrderId, A<CancellationToken>._))
 .Returns(order);

A.CallTo(() => repo.SaveAsync(A<Order>._, A<CancellationToken>._))
 .MustHaveHappenedOnceExactly();
```

- `A<CancellationToken>._` immer — nie einen konkreten Token in Fake-Setups.
- `MustHaveHappenedOnceExactly()` wenn die Aufrufzahl relevant ist.
- Keine Setups für Methoden die im Test nicht relevant sind.

### Shouldly-Konventionen

```csharp
result.IsSuccess.ShouldBeTrue();
result.Value.OrderId.ShouldBe(command.OrderId);
result.Value.Items.ShouldNotBeEmpty();
result.Value.Items.ShouldContain(i => i.ProductId == productId);
Should.Throw<NotFoundException>(() => handler.Handle(invalidCommand, ct));
```

Kein `.Should().Be()` — das ist FluentAssertions-Syntax, hier nicht verwenden.

### Ordnerstruktur spiegelt `src/Features/`

```
tests/Unit/
└── Features/
    └── Orders/
        ├── PlaceOrderCommandHandlerTests.cs
        └── PlaceOrderValidatorTests.cs
```

---

## Specs/ — Specification-Tests (ReqNRoll)

Specs testen ein vollständiges Feature aus Nutzerperspektive — von der API-Grenze durch alle Schichten.
Kein Ersatz für Unit-Tests; sie dokumentieren das **erwartete Verhalten** als lebende Spezifikation.

### Grundregeln

- Pro Feature-Slice **eine** `.feature`-Datei.
- Feature-Dateien auf Englisch (Gherkin-Standard).
- Jedes Scenario beschreibt einen konkreten Nutzungsfall — keine rein technischen Szenarien.
- Step Definitions liegen **neben der `.feature`-Datei** im gleichen Ordner.

### Ordnerstruktur

```
tests/Specs/
└── Features/
    └── Orders/
        ├── PlaceOrder.feature
        └── PlaceOrderSteps.cs
```

### Feature-Datei — Format

```gherkin
Feature: Place Order
  As a customer
  I want to place an order for available products
  So that I can receive them at my specified address

  Scenario: Successfully placing an order with valid products
    Given a customer with a verified account
    And the following products are available in stock
      | ProductId | Quantity |
      | prod-1    | 10       |
    When the customer places an order for 2 units of "prod-1"
    Then the order is confirmed
    And the customer receives an order confirmation

  Scenario: Placing an order for an out-of-stock product
    Given a customer with a verified account
    And product "prod-1" is out of stock
    When the customer places an order for 1 unit of "prod-1"
    Then the order is rejected with error "Product not available"
```

### Step Definitions — Regeln

- Eine `Steps.cs` pro Feature-Ordner. Keine geteilten globalen Step-Klassen.
- Steps injizieren `WebApplicationFactory<T>` — kein Mocking auf dieser Ebene.
- DB-Zustand per `Background`-Step oder `[BeforeScenario]`-Hook aufbauen,
  nach jedem Scenario zurücksetzen (Testcontainers).
- Keine Assertions in `Given`- oder `When`-Steps — ausschließlich in `Then`, mit Shouldly.
- Step-Text domänennah: `"the customer places an order"`, nicht `"POST /api/orders is called"`.

### Was gehört wohin

| Fragestellung                               | Unit/ | Specs/ |
| ------------------------------------------- | ----- | ------ |
| Validierungsregel für ein einzelnes Feld    | ✓     |        |
| Business-Regel die mehrere Schritte umfasst |       | ✓      |
| Fehlerfall den ein Nutzer erlebt            |       | ✓      |
| Internes Verhalten eines Handlers           | ✓     |        |
| Acceptance Criterion aus einem Ticket       |       | ✓      |

---

## Test Red Flags — sofort korrigieren, nicht einchecken

Wenn du einen dieser Punkte in einem Test siehst den du **gerade schreibst**: neu schreiben.
Wenn du ihn in einem **bestehenden** Test siehst: separat melden, nicht anfassen.

### Interna mocken

Ein Test der interne Implementierungsdetails mockt (konkrete Klassen statt Interfaces,
interne Methoden via Reflection) testet nicht das Verhalten — er zementiert die Implementierung.

```csharp
// ❌ konkrete Klasse gemockt statt Interface
var repo = A.Fake<SqlOrderRepository>();

// ✅ Interface gemockt
var repo = A.Fake<IOrderRepository>();
```

Wenn etwas nur testbar ist indem man Interna mockt, ist das ein Designproblem im Produktivcode —
als separaten Hinweis melden, nicht im Test umgehen.

### Private Methoden testen

Private Methoden sind Implementierungsdetail. Wer sie direkt testet (via Reflection oder
`InternalsVisibleTo` nur für Tests) testet das Wie statt das Was.

```csharp
// ❌ private Methode via Reflection
var method = typeof(OrderService).GetMethod("CalculateDiscount",
    BindingFlags.NonPublic | BindingFlags.Instance);

// ✅ das öffentliche Verhalten testen das die private Methode antreibt
var result = await handler.Handle(new PlaceOrderCommand { ... }, ct);
result.Value.DiscountedTotal.ShouldBe(expectedTotal);
```

Wenn eine private Methode so komplex ist dass man sie direkt testen möchte,
gehört sie in eine eigene Klasse mit einem öffentlichen Interface.

### Testname beschreibt Wie, nicht Was

Der Testname ist die Spezifikation. Er beschreibt was das System unter welcher Bedingung tut —
nicht welche Methode aufgerufen wird.

```csharp
// ❌ beschreibt die Implementierung
Handle_CallsRepository_WhenOrderIsValid()
PlaceOrderCommandHandler_Execute_ReturnsResult()

// ✅ beschreibt das beobachtbare Verhalten
PlaceOrder_WithValidProducts_ConfirmsOrder()
PlaceOrder_WhenProductOutOfStock_RejectsWithError()
PlaceOrder_WithExpiredCustomerAccount_ReturnsForbidden()
```

Schema: `<Aktion>_<Bedingung>_<ErwartetesErgebnis>`
