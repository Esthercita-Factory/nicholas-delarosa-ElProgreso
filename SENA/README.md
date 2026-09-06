# Evidencias SENA — Cooperativa Financiera El Progreso

Esta carpeta contiene las evidencias de producto pedidas para las dos normas de competencia, aplicadas al proyecto que ya existe en este repositorio (`ElProgreso/`).

## Norma 220501095 — Diseñar la solución de software

Carpeta: [`220501095-Diseno-Solucion-Software/`](220501095-Diseno-Solucion-Software/)

| # | Evidencia pedida | Carpeta |
|---|---|---|
| 1 | Documento de diseño de software | [1. Documento de diseño de software](220501095-Diseno-Solucion-Software/1.%20Documento%20de%20diseño%20de%20software) |
| 2.1 | Diagrama de casos de uso | [Diagrama de casos de uso](220501095-Diseno-Solucion-Software/2.%20Diagramas%20de%20Lenguaje%20Unificado%20de%20Modelado%20(UML)/Diagrama%20de%20casos%20de%20uso) |
| 2.2 | Diagrama de clases | [Diagrama de clases](220501095-Diseno-Solucion-Software/2.%20Diagramas%20de%20Lenguaje%20Unificado%20de%20Modelado%20(UML)/Diagrama%20de%20clases) |
| 2.3 | Diagrama de secuencia | [Diagrama de secuencia o actividad](220501095-Diseno-Solucion-Software/2.%20Diagramas%20de%20Lenguaje%20Unificado%20de%20Modelado%20(UML)/Diagrama%20de%20secuencia%20o%20actividad) |
| 3 | Prototipo / wireframes | [3. Prototipo de solución de software](220501095-Diseno-Solucion-Software/3.%20Prototipo%20de%20solución%20de%20software) |
| 4 | Modelo de base de datos | [4. Modelo de base de datos](220501095-Diseno-Solucion-Software/4.%20Modelo%20de%20base%20de%20datos)|

## Norma 220501096 — Desarrollar la solución de software

Carpeta: [`220501096-Desarrollo-Solucion-Software/`](220501096-Desarrollo-Solucion-Software/)

| # | Evidencia pedida | Carpeta |
|---|---|---|
| 1 | Documento técnico de código fuente | [1. Documento técnico de código fuente](220501096-Desarrollo-Solucion-Software/1.%20Documento%20técnico%20de%20código%20fuente) |
| 2 | Instructivo de uso (manual de usuario) | [2. Instructivo de uso de la solución de software](220501096-Desarrollo-Solucion-Software/2.%20Instructivo%20de%20uso%20de%20la%20solución%20de%20software) |
| 3 | Solución de software funcionando | [3. Solución de software](220501096-Desarrollo-Solucion-Software/3.%20Solución%20de%20software) |

En cada carpeta están los respectivos archivos requeridos, sólo que hay varias versiones de ellos. Los módelos hechos en draw.io tienen 3 versiones, una en .drawio, otra en .png y una última en .pdf. Y los archivos que son documentación están en .docx y .pdf.

## Nota sobre el modelo de base de datos

El sistema entregado guarda los datos **en memoria** (`InMemoryMemberRepository`), como se explica y justifica en el [README](../README.md) del proyecto: el ejercicio no pedía persistencia real y el patrón Repository (`IMemberRepository`) deja la puerta abierta a enchufar una base de datos después sin tocar `Services` ni `UI`. Por eso el modelo de base de datos de este anexo (evidencia 4 de la norma 095) es el **diseño relacional que respaldaría esa misma solución** el día que `InMemoryMemberRepository` se reemplace por una implementación con base de datos — no algo que el código ejecute hoy. El `schema.sql` que lo acompaña es ejecutable contra PostgreSQL/MySQL/SQL Server con ajustes mínimos, como evidencia de que el diseño es real y no solo un diagrama.
