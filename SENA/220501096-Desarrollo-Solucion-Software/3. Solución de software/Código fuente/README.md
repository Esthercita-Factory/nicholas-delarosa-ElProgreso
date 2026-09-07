# Cooperativa Financiera El Progreso

Console app for a small savings cooperative teller. Members have one savings
account each; a teller registers members, takes deposits and withdrawals, and
management pulls a few reports.

## Features

- Register, list, find (by document or by name), update and delete members.
- Check a member's balance in COP, or converted to USD using the official
  TRM (fetched asynchronously; if the API is down the app tells the teller
  and keeps working instead of crashing).
- Deposits and withdrawals, with the cooperative's account rules enforced.
- Full movement history per member.
- Six management reports: total balance/members/average, top 5 by balance,
  dormant members, a date-range summary, the 10 largest movements, and a
  per-member activity summary.

## Business rules

- A new member starts at a $0 balance; the balance is always the sum of
  that member's movements, never a value set directly.
- Document numbers are unique.
- A member can't be deleted while they have a balance or any movement.
- Withdrawals over $1,000,000 COP add an $8,000 COP cash handling fee. A
  withdrawal is rejected outright (fee included) if it would leave the
  balance negative.
- Movements of zero or negative value are rejected and never recorded.

## Project structure

```
ElProgreso/
  Models/         Member, Movement, MovementType - the account rules live
                   inside Member (RegisterDeposit/RegisterWithdrawal), so
                   there is exactly one place that can change a balance.
  Repositories/    IMemberRepository + an in-memory implementation.
  Services/        MemberService, MovementService, ReportService and
                   ExchangeRateService - the use cases the console calls.
  UI/              The teller menu (TellerConsole, ManagementReportsMenu)
                   and small console I/O helpers.
  Program.cs       Wires everything together and starts the menu.
```

Layered by folder rather than by project: `Models` has no dependency on
anything else, `Services` depends on `Models` and `Repositories`, and `UI`
is the only place that talks to `Console`.

You can see in [docs](docs/) two files: one in draw.io format and another in PDF format, both containing the class diagram.
Draw.io: [ElProgreso-Class-Diagram.drawio](docs/ElProgreso-Class-Diagram.drawio)
PDF: [ElProgreso-Class-Diagram.drawio.pdf](docs/ElProgreso-Class-Diagram.drawio.pdf)

## Technologies

- .NET 10 / C# 14
- `HttpClient` for the TRM lookup (no extra package - the framework's own
  JSON extensions were enough)

## Running it

```bash
dotnet run --project ElProgreso
```

Data lives in memory for the run - closing the app clears it. That was
enough for this exercise's scope; a real deployment would swap
`InMemoryMemberRepository` for a database-backed one behind the same
`IMemberRepository` interface.

## Notes on a few decisions

- **No database.** The brief doesn't ask for one, and an in-memory
  repository behind an interface was the fastest way to get the actual
  business rules right within the time box, while still keeping the door
  open to swap in real persistence later without touching the services.
- **Document/name/phone format checks aren't in the brief** - I added
  reasonable ones anyway (6-10 digit document, letters-only name, 7-15
  digit phone) since the brief is clearly about a real financial system,
  and an unchecked "document number" felt like an obvious gap.
- **Plain exceptions instead of a Result type.** `Member` throws
  `InvalidOperationException`/`ArgumentException` with the exact message
  the teller should see, and the console layer catches them at each menu
  option. Simple and enough for one UI.

## Author

Nicholas De la Rosa - [Mi GitHub](https://github.com/nicholas-delarosa)