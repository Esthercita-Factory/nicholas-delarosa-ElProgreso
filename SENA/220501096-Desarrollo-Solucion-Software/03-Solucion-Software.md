# Solución de software

**Norma:** 220501096 — Evidencia 3

## Repositorio

El código fuente completo, funcional e íntegro está en este mismo repositorio:

```
https://github.com/Esthercita-Factory/nicholas-delarosa-ElProgreso.git
```

(ver [`repo.txt`](../../repo.txt) en la raíz del proyecto).

## Implementación del código fuente

Todo el código fuente vive en [`ElProgreso/`](../../ElProgreso/), estructurado en capas (`Models`, `Repositories`, `Services`, `UI`) como se detalla en el [documento técnico de código fuente](01-Documento-Tecnico-Codigo-Fuente.md).

## Cumplimiento de requisitos del sistema

Las 12 funcionalidades pedidas en el enunciado (registrar, listar, buscar por documento, buscar por nombre, actualizar, eliminar, consultar saldo COP, consultar saldo USD, consignar, retirar, ver movimientos, informes de gerencia) están implementadas y verificadas funcionando en el [manual de usuario](02-Manual-Usuario.md), con una sesión real de consola como evidencia — incluyendo el caso de error (retiro rechazado por saldo insuficiente) y el caso de éxito de cada operación.

## Funcionamiento correcto de las funcionalidades

Verificado con:

```bash
dotnet build ElProgreso.slnx   # compila sin advertencias ni errores
dotnet run --project ElProgreso
```

La sesión de prueba documentada en el manual de usuario cubrió: alta de un asociado, consignación, consulta de saldo, un retiro rechazado por saldo insuficiente (demuestra que la regla de "nunca dejar el saldo en negativo" se cumple), un retiro aceptado sin comisión (por ser menor a $1.000.000), consulta del historial de movimientos, consulta de saldo en dólares con la TRM real obtenida en vivo, y un informe de gerencia.

## Integración con base de datos

**No aplica en esta entrega.** El sistema persiste en memoria por diseño (ver justificación en el [README](../../README.md) del proyecto y en el [documento de diseño](../220501095-Diseno-Solucion-Software/01-Documento-Diseno-Software.md), sección 9). El modelo relacional que respaldaría una integración real con base de datos, junto con su script SQL ejecutable, ya está diseñado en la [evidencia 4 de la norma 220501095](../220501095-Diseno-Solucion-Software/04-Modelo-Base-Datos.md) — la arquitectura en capas (`IMemberRepository` como interfaz) permite conectarlo sin modificar `Services` ni `UI`.

## Buenas prácticas de programación

Detalladas con fragmentos de código en la sección 4 del [documento técnico de código fuente](01-Documento-Tecnico-Codigo-Fuente.md): nomenclatura en inglés, inmutabilidad donde corresponde, encapsulación estricta del saldo, manejo de errores por excepciones tipadas, y ausencia de duplicación de reglas de negocio.

## Entregable adicional: proyecto en .zip

Para el formulario de entrega de evidencias, comprimir la raíz del repositorio (excluyendo `bin/` y `obj/`, ya ignorados por [`.gitignore`](../../.gitignore)):

```bash
git archive -o ElProgreso-entrega.zip HEAD
```

Este comando empaqueta exactamente lo que está versionado en git (código fuente, `README.md`, `docs/`, y esta carpeta `SENA/`), sin artefactos de compilación.
