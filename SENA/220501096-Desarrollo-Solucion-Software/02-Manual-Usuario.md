# Manual de usuario / Instructivo de uso

**Proyecto:** Cooperativa Financiera El Progreso
**Norma:** 220501096 — Evidencia 2

Las capturas de este manual son transcripciones reales de una ejecución del sistema (`dotnet run`), no ilustraciones — al ser una aplicación de consola, el texto de la terminal **es** la pantalla que verá la cajera, así que se presenta tal cual salió, incluyendo la tasa TRM real obtenida en vivo desde `datos.gov.co` en el momento de la prueba.

## 1. Requisitos del sistema

| Requisito | Detalle |
|---|---|
| SDK | .NET 10 SDK o superior instalado (`dotnet --version`) |
| Sistema operativo | Windows, Linux o macOS (cualquiera con soporte .NET) |
| Conexión a internet | Opcional. Solo se necesita para la opción 8 (saldo en dólares). Sin internet, el resto del sistema funciona igual. |
| Base de datos | Ninguna — los datos se guardan en memoria mientras el programa está abierto. |

## 2. Pasos de instalación / ejecución

```bash
# 1. Clonar el repositorio
git clone https://github.com/Esthercita-Factory/nicholas-delarosa-ElProgreso.git
cd nicholas-delarosa-ElProgreso

# 2. Ejecutar (dotnet compila automáticamente si hace falta)
dotnet run --project ElProgreso
```

No se requiere ningún paso de configuración adicional (sin cadenas de conexión, sin variables de entorno, sin `appsettings.json`).

## 3. Descripción de las funcionalidades y ejemplos de uso

### 3.1 Registrar un asociado nuevo (opción 1)

```
Seleccione una opción: 1

Número de documento: 1234567
Nombre completo: Juan Perez
Teléfono (Enter para omitir): 3001234567
Dirección (Enter para omitir): 
Asociado registrado. Juan Perez queda con la cuenta en $ 0.
```

Todo asociado nuevo queda con saldo $0. El documento y el nombre son obligatorios; teléfono y dirección se pueden omitir presionando Enter.

### 3.2 Listar todos los asociados (opción 2)

```
Seleccione una opción: 2

1 asociado(s):
  1234567         Juan Perez                                 $ 0
```

### 3.3 Registrar una consignación (opción 9)

```
Seleccione una opción: 9

Número de documento: 1234567
Valor a consignar: 500000
Consignación registrada.
Nuevo saldo: $ 500.000
```

### 3.4 Consultar saldo (opción 7)

```
Seleccione una opción: 7

Número de documento: 1234567
Saldo actual: $ 500.000
```

### 3.5 Registrar un retiro — caso rechazado (opción 10)

```
Seleccione una opción: 10

Número de documento: 1234567
Valor a retirar: 2000000
Movimiento rechazado: Saldo insuficiente: el retiro de 2.000.000 más la comisión
de manejo de efectivo de 8.000 deja el saldo en negativo.
```

El sistema nunca permite que un retiro deje el saldo en negativo — ni siquiera por efecto de la comisión. El saldo del asociado no cambió tras este intento.

### 3.6 Registrar un retiro — caso aceptado (opción 10)

```
Seleccione una opción: 10

Número de documento: 1234567
Valor a retirar: 100000
Retiro registrado.
Nuevo saldo: $ 400.000
```

Como este retiro no supera $1.000.000, no se cobró la comisión de manejo de efectivo.

### 3.7 Ver los movimientos de un asociado (opción 11)

```
Seleccione una opción: 11

Número de documento: 1234567
2 movimiento(s) de Juan Perez:
  01/09/2026 14:31  Consignación  $ 500.000  saldo: $ 500.000
  01/09/2026 14:31  Retiro        $ 100.000  saldo: $ 400.000
```

### 3.8 Consultar saldo en dólares — TRM oficial (opción 8)

```
Seleccione una opción: 8

Número de documento: 1234567
Saldo en pesos: $ 400.000
TRM utilizada: $ 3.214 por dólar (vigente del 01/09/2026 al 01/09/2026)
Saldo equivalente: $124.46
```

Captura real de una consulta en vivo a la API de la Superintendencia Financiera durante la prueba de este manual (1 de septiembre de 2026). Si la API no responde, en vez de este resultado el sistema muestra: *"No fue posible obtener la TRM en este momento. Intente de nuevo más tarde."* y sigue funcionando con normalidad — no se cae.

### 3.9 Informes de gerencia (opción 12)

```
Seleccione una opción: 12

Informes de gerencia:
1. ¿Cuánta plata tenemos?
2. ¿Quiénes son mis mejores asociados?
3. ¿Quiénes están dormidos?
4. ¿Cómo nos fue en un periodo?
5. ¿Cuáles fueron los movimientos más grandes?
6. ¿Quién me esta moviendo la caja?
0. Volver al menu principal

Seleccione una opción: 1

Cuanta plata tenemos:
  Saldo total de la cooperativa: $ 400.000
  Cantidad de asociados: 1
  Saldo promedio por asociado: $ 400.000
```

Los otros cinco informes (mejores asociados, dormidos, periodo, movimientos más grandes, actividad por asociado) siguen el mismo patrón: se elige el número y el resultado se calcula al instante sobre los datos actuales en memoria.

### 3.10 Salir (opción 0)

```
Seleccione una opción: 0

Hasta pronto.
```

Al cerrar el programa, todos los datos capturados durante la sesión se pierden (persistencia en memoria) — es el comportamiento esperado para el alcance de este ejercicio, documentado también en el `README.md` principal.
