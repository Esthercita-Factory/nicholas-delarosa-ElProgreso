# Diagrama de casos de uso

**Norma:** 220501095 — Evidencia 2 (Diagramas UML)

Actor principal: **Cajera** — es la única persona que opera el sistema (el asociado nunca lo toca directamente, según el enunciado del cliente). Actor secundario no humano: **API TRM**, consultada por el caso de uso "Consultar saldo en dólares". El **Gerente** consume los informes de forma indirecta, a través de la cajera.

```mermaid
flowchart LR
    Cajera(("🧑‍💼<br/>Cajera"))
    Gerente(("🧑‍💼<br/>Gerente"))
    APITRM(("🌐<br/>API TRM<br/>datos.gov.co"))

    subgraph Sistema["Sistema de ventanilla — El Progreso"]
        UC1([Registrar asociado])
        UC2([Listar asociados])
        UC3([Buscar por documento])
        UC4([Buscar por nombre])
        UC5([Actualizar asociado])
        UC6([Eliminar asociado])
        UC7([Consultar saldo en COP])
        UC8([Consultar saldo en USD])
        UC9([Registrar consignación])
        UC10([Registrar retiro])
        UC11([Ver movimientos])
        UC12([Consultar informes de gerencia])
    end

    Cajera --- UC1
    Cajera --- UC2
    Cajera --- UC3
    Cajera --- UC4
    Cajera --- UC5
    Cajera --- UC6
    Cajera --- UC7
    Cajera --- UC8
    Cajera --- UC9
    Cajera --- UC10
    Cajera --- UC11
    Cajera --- UC12

    UC8 -.->|"«include»<br/>obtiene la TRM vigente"| APITRM
    UC6 -.->|"«extend»<br/>solo si no tiene saldo/movimientos"| UC3
    UC9 -.->|"«include»<br/>ubica al asociado primero"| UC3
    UC10 -.->|"«include»<br/>ubica al asociado primero"| UC3

    Gerente -.->|consume vía la cajera| UC12
```

## Descripción de los casos de uso

| Caso de uso | Actor | Precondición | Flujo principal | Excepciones |
|---|---|---|---|---|
| Registrar asociado | Cajera | El documento no debe existir aún | Cajera ingresa documento, nombre, teléfono (opcional), dirección (opcional) → el sistema crea el asociado con saldo $0 | Documento repetido, formato de documento/nombre/teléfono inválido |
| Buscar por documento / por nombre | Cajera | — | Cajera ingresa el criterio → el sistema devuelve el/los asociado(s) coincidentes | Sin coincidencias |
| Actualizar asociado | Cajera | El asociado debe existir | Cajera modifica nombre/teléfono/dirección (el documento no se puede cambiar) | Asociado no encontrado, datos inválidos |
| Eliminar asociado | Cajera | El asociado no debe tener saldo ni movimientos | Cajera confirma la eliminación → el sistema lo remueve | Asociado con saldo o movimientos → se rechaza |
| Consultar saldo en COP | Cajera | El asociado debe existir | Se muestra el saldo actual | Asociado no encontrado |
| Consultar saldo en USD | Cajera | El asociado debe existir | El sistema consulta la TRM vigente (async) y convierte el saldo | Asociado no encontrado; TRM no disponible → se informa y no se interrumpe el sistema |
| Registrar consignación | Cajera | El asociado debe existir | Cajera ingresa el valor → se suma al saldo | Valor ≤ 0, asociado no encontrado |
| Registrar retiro | Cajera | El asociado debe existir y tener saldo suficiente | Cajera ingresa el valor → se resta del saldo, con comisión si supera $1.000.000 | Valor ≤ 0, saldo insuficiente (incluyendo comisión), asociado no encontrado |
| Ver movimientos | Cajera | El asociado debe existir | Se lista el historial completo con saldo corriente | Asociado no encontrado |
| Consultar informes de gerencia | Cajera (por el Gerente) | Debe haber datos registrados para que el informe no salga vacío | Se elige uno de los 6 informes y se muestra calculado en el momento | Rango de fechas inválido (informe por periodo) |
