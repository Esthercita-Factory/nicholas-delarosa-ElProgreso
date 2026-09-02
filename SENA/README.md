# Evidencias SENA — Cooperativa Financiera El Progreso

Esta carpeta contiene las evidencias de producto pedidas para las dos normas de competencia, aplicadas al proyecto que ya existe en este repositorio (`ElProgreso/`).

Todos los documentos son Markdown a propósito: se ven bien en GitHub/Rider/VS Code tal cual, y cualquier editor (VS Code con la extensión "Markdown PDF", Typora, o incluso pegar en Word) los exporta a PDF o .docx en un clic si el formulario de entrega exige ese formato puntual. Los diagramas están en sintaxis [Mermaid](https://mermaid.js.org/), que se renderiza sola en GitHub y en la mayoría de visores Markdown — no se necesitó Draw.io/Figma aparte para tenerlos versionados junto al código, aunque el diagrama de clases oficial del proyecto (hecho en Draw.io) ya vivía en [`docs/`](../docs/) desde antes y se referencia, no se duplica.

## Norma 220501095 — Diseñar la solución de software

Carpeta: [`220501095-Diseno-Solucion-Software/`](220501095-Diseno-Solucion-Software/)

| # | Evidencia pedida | Archivo |
|---|---|---|
| 1 | Documento de diseño de software | [01-Documento-Diseno-Software.md](220501095-Diseno-Solucion-Software/01-Documento-Diseno-Software.md) |
| 2 | Diagrama de casos de uso | [02a-Diagrama-Casos-de-Uso.md](220501095-Diseno-Solucion-Software/02a-Diagrama-Casos-de-Uso.md) |
| 2 | Diagrama de clases | [02b-Diagrama-Clases.md](220501095-Diseno-Solucion-Software/02b-Diagrama-Clases.md) |
| 2 | Diagrama de secuencia | [02c-Diagrama-Secuencia.md](220501095-Diseno-Solucion-Software/02c-Diagrama-Secuencia.md) |
| 3 | Prototipo / wireframes | [03-Prototipo-Wireframes.md](220501095-Diseno-Solucion-Software/03-Prototipo-Wireframes.md) |
| 4 | Modelo de base de datos | [04-Modelo-Base-Datos.md](220501095-Diseno-Solucion-Software/04-Modelo-Base-Datos.md) + [schema.sql](220501095-Diseno-Solucion-Software/schema.sql) |

## Norma 220501096 — Desarrollar la solución de software

Carpeta: [`220501096-Desarrollo-Solucion-Software/`](220501096-Desarrollo-Solucion-Software/)

| # | Evidencia pedida | Archivo |
|---|---|---|
| 1 | Documento técnico de código fuente | [01-Documento-Tecnico-Codigo-Fuente.md](220501096-Desarrollo-Solucion-Software/01-Documento-Tecnico-Codigo-Fuente.md) |
| 2 | Instructivo de uso (manual de usuario) | [02-Manual-Usuario.md](220501096-Desarrollo-Solucion-Software/02-Manual-Usuario.md) |
| 3 | Solución de software funcionando | [03-Solucion-Software.md](220501096-Desarrollo-Solucion-Software/03-Solucion-Software.md) |

## Nota sobre el modelo de base de datos

El sistema entregado guarda los datos **en memoria** (`InMemoryMemberRepository`), como se explica y justifica en el [README](../README.md) del proyecto: el ejercicio no pedía persistencia real y el patrón Repository (`IMemberRepository`) deja la puerta abierta a enchufar una base de datos después sin tocar `Services` ni `UI`. Por eso el modelo de base de datos de este anexo (evidencia 4 de la norma 095) es el **diseño relacional que respaldaría esa misma solución** el día que `InMemoryMemberRepository` se reemplace por una implementación con base de datos — no algo que el código ejecute hoy. El `schema.sql` que lo acompaña es ejecutable contra PostgreSQL/MySQL/SQL Server con ajustes mínimos, como evidencia de que el diseño es real y no solo un diagrama.
