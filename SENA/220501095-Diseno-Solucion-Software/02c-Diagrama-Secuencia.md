# Diagrama de secuencia

**Norma:** 220501095 — Evidencia 2 (Diagramas UML)

Se documentan los dos flujos con más lógica de negocio y más valor para explicar en la sustentación: **registrar un retiro** (con comisión y rechazo por saldo insuficiente) y **consultar el saldo en dólares** (consumo asíncrono de la TRM, con manejo de falla).

## 1. Registrar un retiro

```mermaid
sequenceDiagram
    actor Cajera
    participant UI as TellerConsole
    participant Svc as MovementService
    participant Repo as IMemberRepository
    participant M as Member

    Cajera->>UI: Opción 10 (Registrar retiro) + documento + valor
    UI->>Svc: RegisterWithdrawal(documentNumber, amount)
    Svc->>Repo: GetByDocumentNumber(documentNumber)
    Repo-->>Svc: Member | null

    alt asociado no existe
        Svc-->>UI: throw InvalidOperationException("No se encontró...")
        UI-->>Cajera: "No se encontró ningún asociado con el documento..."
    else asociado existe
        Svc->>M: RegisterWithdrawal(amount)
        M->>M: valida amount > 0
        M->>M: fee = amount > 1.000.000 ? 8.000 : 0
        M->>M: totalDebit = amount + fee
        M->>M: valida totalDebit <= Balance

        alt saldo insuficiente
            M-->>Svc: throw InvalidOperationException("Saldo insuficiente...")
            Svc-->>UI: propaga la excepción
            UI-->>Cajera: "Movimiento rechazado: Saldo insuficiente..."<br/>(saldo queda intacto)
        else fondos suficientes
            M->>M: crea Movement(Withdrawal, amount, fee, ahora)
            M->>M: _movements.Add(movement)
            M-->>Svc: Movement registrado
            Svc-->>UI: Movement
            UI->>Svc: (vuelve a pedir el Member para leer el nuevo Balance)
            UI-->>Cajera: "Retiro registrado."<br/>"Se aplicó comisión de $X" (si aplica)<br/>"Nuevo saldo: $Y"
        end
    end
```

**Puntos a resaltar en la sustentación:** la validación de fondos ocurre **dentro de `Member`**, no en `MovementService` ni en la UI — así ningún otro punto de entrada al sistema puede saltarse la regla. Si el retiro se rechaza, el método lanza la excepción *antes* de tocar `_movements`, por lo que el saldo queda exactamente igual que antes del intento (cumple "un movimiento rechazado no se ejecuta ni se registra").

## 2. Consultar saldo en dólares (TRM)

```mermaid
sequenceDiagram
    actor Cajera
    participant UI as TellerConsole
    participant Svc as MemberService
    participant Repo as IMemberRepository
    participant Rate as ExchangeRateService
    participant API as API TRM (datos.gov.co)

    Cajera->>UI: Opción 8 (Saldo en USD) + documento
    UI->>Svc: GetBalanceInUsdAsync(documentNumber)
    Svc->>Repo: GetByDocumentNumber(documentNumber)
    Repo-->>Svc: Member | null

    alt asociado no existe
        Svc-->>UI: throw InvalidOperationException
        UI-->>Cajera: mensaje de error
    else asociado existe
        Svc->>Rate: await GetLastestRateAsync()
        Rate->>API: GET resource/32sa-8pi3.json?...limit=1 (async)

        alt API falla / timeout / JSON inválido
            API-->>Rate: excepción de red o JSON malformado
            Rate->>Rate: catch (HttpRequestException, TaskCanceledException, JsonException)
            Rate-->>Svc: null
            Svc-->>UI: throw InvalidOperationException("No fue posible obtener la TRM...")
            UI-->>Cajera: mensaje de error<br/>(el sistema sigue operando con normalidad)
        else API responde
            API-->>Rate: JSON con valor, vigenciadesde, vigenciahasta
            Rate->>Rate: parsea y valida el valor decimal
            Rate-->>Svc: ExchangeRate(Value, ValidFrom, ValidUntil)
            Svc->>Svc: balanceUsd = member.Balance / rate.Value
            Svc-->>UI: BalanceInUsd
            UI-->>Cajera: "Saldo en pesos: $X"<br/>"TRM: $Y (vigente del ... al ...)"<br/>"Saldo equivalente: US$Z"
        end
    end
```

**Puntos a resaltar en la sustentación:** `ExchangeRateService.GetLastestRateAsync` **nunca propaga una excepción** — el `try/catch` interno la absorbe y devuelve `null`. Es `MemberService` quien decide qué significa ese `null` para el negocio (no se puede convertir el saldo) y lo traduce en una excepción de dominio con el mensaje que debe leer la cajera. Esa separación es la que garantiza que una caída de la API externa jamás tumbe la aplicación completa.
