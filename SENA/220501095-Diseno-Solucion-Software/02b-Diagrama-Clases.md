# Diagrama de clases

**Norma:** 220501095 — Evidencia 2 (Diagramas UML)

Este diagrama es la versión Mermaid (texto versionable) del diagrama de clases del modelo implementado. La versión oficial hecha en Draw.io, con el mismo contenido, está en [`docs/ElProgreso-Class-Diagram.drawio`](../../docs/ElProgreso-Class-Diagram.drawio) y su export en [`docs/ElProgreso-Class-Diagram.drawio.pdf`](../../docs/ElProgreso-Class-Diagram.drawio.pdf).

```mermaid
classDiagram
    direction LR

    class MovementType {
        <<enumeration>>
        Deposit
        Withdrawal
    }

    class Movement {
        <<record>>
        +MovementType Type
        +decimal Amount
        +decimal Fee
        +DateTime OccurredAt
        +decimal SignedEffect
    }

    class Member {
        -List~Movement~ _movements
        +string DocumentNumber
        +string FullName
        +string? PhoneNumber
        +string? Address
        +DateTime JoinedAt
        +IReadOnlyList~Movement~ Movements
        +decimal Balance
        +bool HasMovements
        +Member(documentNumber, fullName, phoneNumber, address)
        +UpdateContactInfo(fullName, phoneNumber, address) void
        +RegisterDeposit(amount) Movement
        +RegisterWithdrawal(amount) Movement
    }

    class IMemberRepository {
        <<interface>>
        +Add(member) void
        +GetByDocumentNumber(documentNumber) Member?
        +GetAll() List~Member~
        +SearchByName(namePart) List~Member~
        +ExistsByDocumentNumber(documentNumber) bool
        +Remove(member) void
    }

    class InMemoryMemberRepository {
        -List~Member~ _members
        +Add(member) void
        +GetByDocumentNumber(documentNumber) Member?
        +GetAll() List~Member~
        +SearchByName(namePart) List~Member~
        +ExistsByDocumentNumber(documentNumber) bool
        +Remove(member) void
    }

    class MemberService {
        -IMemberRepository memberRepository
        -ExchangeRateService exchangeRateService
        +Register(...) Member
        +GetAll() List~Member~
        +GetByDocument(documentNumber) Member
        +SearchByName(namePart) List~Member~
        +Update(...) Member
        +Delete(documentNumber) void
        +GetBalanceInUsdAsync(documentNumber) Task~BalanceInUsd~
    }

    class MovementService {
        -IMemberRepository memberRepository
        +RegisterDeposit(documentNumber, amount) Movement
        +RegisterWithdrawal(documentNumber, amount) Movement
        +GetMovements(documentNumber) IReadOnlyList~Movement~
    }

    class ReportService {
        -IMemberRepository memberRepository
        +GetCooperativeSummary() CooperativeSummary
        +GetTopMembers(count) List~TopMember~
        +GetDormantMembers() List~DormantMember~
        +GetPeriodReport(from, to) PeriodReport
        +GetLargestMovements(count) List~LargestMovement~
        +GetMemberActivity() List~MemberActivity~
    }

    class ExchangeRateService {
        -HttpClient httpClient
        +GetLastestRateAsync() Task~ExchangeRate?~
    }

    class ExchangeRate {
        <<record>>
        +decimal Value
        +DateTime ValidFrom
        +DateTime ValidUntil
    }

    class BalanceInUsd {
        <<record>>
        +decimal BalanceCop
        +decimal ExchangeRate
        +decimal BalanceUsd
        +DateTime RateValidFrom
        +DateTime RateValidUntil
    }

    class TellerConsole {
        -MemberService memberService
        -MovementService movementService
        -ReportService reportService
        +RunAsync() Task
    }

    class ManagementReportsMenu {
        <<static>>
        +Run(reportService) void
    }

    IMemberRepository <|.. InMemoryMemberRepository : implements
    Member "1" *-- "0..*" Movement : contiene
    Movement --> MovementType : usa
    Member ..> Movement : crea vía\nRegisterDeposit/RegisterWithdrawal

    MemberService --> IMemberRepository : depende de
    MemberService --> ExchangeRateService : depende de
    MemberService ..> BalanceInUsd : produce
    MovementService --> IMemberRepository : depende de
    ReportService --> IMemberRepository : depende de
    ExchangeRateService ..> ExchangeRate : produce

    TellerConsole --> MemberService : usa
    TellerConsole --> MovementService : usa
    TellerConsole --> ReportService : usa
    TellerConsole --> ManagementReportsMenu : delega informes
    ManagementReportsMenu --> ReportService : usa
```

## Multiplicidades y relaciones clave

- **`Member` 1 — 0..\* `Movement`**: composición. Un asociado tiene cero o muchos movimientos; un movimiento no existe sin un asociado dueño, y solo `Member` puede crearlos (`_movements` es una lista privada). Si se elimina el `Member`, sus `Movement` dejan de existir con él — coherente con la regla de negocio "no se puede eliminar un asociado con movimientos" (nunca hay `Movement` huérfanos que borrar).
- **`IMemberRepository` ◁┄┄ `InMemoryMemberRepository`**: realización de interfaz. Es la relación que permite inversión de dependencias: `MemberService`, `MovementService` y `ReportService` dependen de la interfaz, no de la implementación en memoria.
- **`MemberService` → `ExchangeRateService`**: dependencia de uso (inyectada por constructor), no de composición — `ExchangeRateService` se instancia una sola vez en `Program.cs` y se comparte.
- **`Movement` → `MovementType`**: dependencia simple; el `enum` solo distingue el tipo de movimiento y determina el signo de `SignedEffect`.
