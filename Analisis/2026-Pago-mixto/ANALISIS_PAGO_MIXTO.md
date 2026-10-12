# Pago Mixto (Efectivo + Tarjeta) — Análisis funcional

**Sistema:** lluviaBackEnd (back-office / punto de venta de Comercializadora Lluvia)
**Fecha del análisis:** 2026-10-09
**Estado:** Borrador actualizado con las respuestas del cliente (2026-10-11), para validación final
**Pantallas afectadas:** Cobro de Ventas y Confirmación de Pedidos Especiales

---

## 1. Resumen

Hoy, al cobrar, el sistema solo permite **una** forma de pago por venta. Se solicita poder cobrar **una parte con tarjeta y el resto en efectivo** en la misma venta.

**Lo solicitado, en una frase:** al elegir la forma de pago **Mixto** aparecen dos recuadros, *Tarjeta* y *Efectivo*. El cajero captura una de las dos cantidades y el sistema calcula la otra.

> **Ejemplo solicitado:** la cuenta es de $1,000 y el cliente paga $800 con tarjeta. El cajero captura $800 en *Tarjeta* y el sistema muestra $200 en *Efectivo*.

### Decisiones ya tomadas

| # | Decisión |
|---|----------|
| 1 | El pago Mixto aplica en **Ventas** y en **Pedidos Especiales** (cobro al confirmar el pedido). |
| 2 | **No aplica** en Cuentas por Cobrar ni en abonos. |
| 3 | Las ventas con pago Mixto **no se podrán facturar**. |
| 4 | El **ticket** mostrará el monto pagado con tarjeta y el monto pagado en efectivo. |
| 5 | La **comisión bancaria no interviene**: está desactivada en el sistema (no hay ventas con comisión desde agosto de 2024). |
| 6 | **Cancelación y devolución:** todo se devuelve en **efectivo** (la devolución a tarjeta tarda de 8 a 15 días en la terminal). |
| 7 | **Cambio:** el cajero captura el efectivo entregado y el sistema calcula el cambio. |
| 8 | Basta registrar **"Tarjeta"**; no se distingue crédito de débito. |
| 9 | **No aplica** en pedidos especiales a crédito o con abono, ni en pedidos en ruta. El cobro adicional al agregar productos a una venta pagada usa **una sola** forma de pago. |
| 10 | Los reportes de ventas mostrarán las columnas **Tarjeta** y **Efectivo** donde sea necesario (costo adicional). |

### Puntos que requieren decisión del cliente

Todos los puntos de la sección [9](#9-puntos-pendientes-de-definir) ya fueron respondidos. Queda por confirmar solo el detalle del tratamiento de las cancelaciones en el cierre de caja (sección 7.2).

---

## 2. Situación actual (verificada en el sistema)

| Aspecto | Hoy |
|---------|-----|
| Formas de pago por venta | Una sola (Efectivo, Tarjeta de crédito, Tarjeta de débito, Transferencia, Crédito, etc.). |
| Campo de efectivo en pantalla | Solo aparece cuando la forma de pago es Efectivo. Sirve para calcular el cambio. |
| Pago con tarjeta | El sistema asume que se pagó el total exacto. |
| Datos que se guardan | Una forma de pago y un monto pagado por venta. |
| Cierre de caja | Separa los importes por forma de pago: efectivo, tarjeta (crédito y débito juntos), transferencia y otras. |
| Facturación | La factura toma la forma de pago registrada en la venta. |

**Consecuencia:** una venta pagada mitad tarjeta y mitad efectivo hoy no se puede registrar con exactitud. Si se captura como "Efectivo", el cierre de caja marca efectivo que no existe; si se captura como "Tarjeta", falta efectivo en el cierre.

---

## 3. Funcionamiento propuesto

### 3.1 Pantalla de cobro

1. El cajero elige **Mixto** en *Forma de pago*.
2. Aparecen dos recuadros: **Tarjeta** y **Efectivo**.
3. Al capturar un monto en uno, el otro se calcula solo: `restante = total − monto capturado`.
4. El cajero puede capturar en cualquiera de los dos; el sistema recalcula el opuesto.
5. Aparece un tercer recuadro, **Efectivo recibido**, donde el cajero captura el billete o monto que entrega el cliente; el sistema muestra el **cambio** (`efectivo recibido − efectivo a cobrar`).
6. Al guardar, el sistema valida las reglas de la sección 4.

**Maqueta (referencia, no diseño final):**

```
Forma de pago:  [ Mixto            v ]

Total a cobrar:                 $1,000.00
Tarjeta:           [  800.00 ]   (capturado)
Efectivo a cobrar: [  200.00 ]   (calculado)
Efectivo recibido: [  500.00 ]   (capturado)
Cambio:                          $300.00
```

### 3.2 Ejemplos de cálculo

| Total | Captura | Tarjeta | Efectivo |
|------:|---------|--------:|---------:|
| $1,000.00 | Tarjeta = $800 | $800.00 | $200.00 |
| $1,000.00 | Efectivo = $300 | $700.00 | $300.00 |
| $350.50 | Tarjeta = $100 | $100.00 | $250.50 |

### 3.3 Dónde aplica

| Pantalla | Aplica | Nota |
|----------|:------:|------|
| Cobro de Ventas (punto de venta) | Sí | Venta normal. |
| Confirmación de productos del Pedido Especial | Sí | Cobro al confirmar el pedido. |
| Abonos / Cuentas por Cobrar | No | Decisión del cliente. |
| Pedido Especial a crédito o crédito con abono | No | Decisión del cliente. |
| Cobro de pedidos en ruta | No | Decisión del cliente. |
| Cobro adicional al agregar productos a una venta ya pagada | No | Una sola forma de pago. |

---

## 4. Reglas de negocio propuestas

| # | Regla |
|---|-------|
| R1 | Tarjeta + Efectivo debe ser igual al total de la venta. |
| R2 | Tarjeta debe ser **mayor que $0 y menor que el total**. Si fuera igual al total, la venta es de tarjeta; si fuera $0, es de efectivo. |
| R3 | No se permiten montos negativos. |
| R4 | El sistema valida las reglas **también en el servidor** (base de datos), no solo en la pantalla, para evitar registros inconsistentes. |
| R5 | Una venta Mixta **no puede facturarse** (ver sección 6). |
| R6 | La comisión bancaria no se calcula en ventas Mixtas. |

---

## 5. Ticket

El ticket de una venta mixta incluye dos líneas nuevas en el pie:

| Línea | Valor |
|-------|-------|
| **PAGO CON TARJETA** | Monto cobrado con tarjeta. |
| **PAGO EN EFECTIVO** | `Total de la venta − monto de tarjeta` (lo aplicado a la venta). |

El encabezado mostrará *Forma de Pago: Mixto*. Las ventas que no son mixtas **no cambian**: las líneas nuevas no se imprimen. Se conservan **RECIBIDO** (tarjeta + efectivo entregado) y **SU CAMBIO**.

**Ejemplo (cuenta de $1,000, tarjeta $800, el cliente entrega un billete de $500):**

```
  TOTAL:               $1,000.00
  PAGO CON TARJETA:      $800.00
  PAGO EN EFECTIVO:      $200.00
  RECIBIDO:            $1,300.00
  SU CAMBIO:             $300.00
```

**Tickets que se modifican:** venta (impresión directa y PDF) y pedido especial (impresión directa y PDF). El PDF del pedido en ruta solo se modifica si puede mostrar un pedido pagado como Mixto al confirmarlo (se verifica en la implementación). Los tickets de cancelación y devolución muestran el reembolso en efectivo.

---

## 6. Facturación

Una factura (CFDI) admite **una sola** forma de pago, por lo que una venta repartida entre dos formas no puede facturarse sin una regla fiscal adicional. Se decidió **bloquearlo**.

| Momento | Control |
|---------|---------|
| Al cobrar | Con Mixto seleccionado, la opción "Facturar esta venta" se deshabilita. |
| Al guardar | El sistema rechaza una venta Mixta marcada para facturar. |
| Facturar después (desde consulta de ventas o pedidos) | El sistema rechaza guardar los datos de facturación de una venta Mixta. **Importante:** hoy ese paso reemplaza la forma de pago de la venta, por lo que sin este control una venta mixta quedaría con datos inconsistentes. |
| Selector de forma de pago en pantallas de facturación | "Mixto" no debe ofrecerse como opción. |

**Consecuencia para el negocio:** si un cliente pide factura de una compra ya pagada de forma mixta, no será posible facturarla. La alternativa operativa sería cancelar y volver a cobrar con una sola forma de pago (sujeta a la definición del punto abierto 1).

---

## 7. Cierres de caja y reportes

Es el área de mayor cuidado: el cierre de caja separa los importes por forma de pago. Si una venta mixta no se reparte correctamente, **el efectivo disponible y el aviso de exceso de efectivo quedan mal**.

### 7.1 Cálculo esperado

| Concepto | Se suma al… |
|----------|-------------|
| Venta Mixta: monto de tarjeta | Total de **tarjeta** |
| Venta Mixta: total − tarjeta | Total de **efectivo (contado)** |

### 7.2 Procesos que se ajustan

| Proceso | Ajuste |
|---------|--------|
| Aviso de exceso de efectivo | Incluye la parte en efectivo de las ventas mixtas. |
| Cierre de caja de ventas | Reparte la venta mixta entre efectivo y tarjeta. |
| Retiro de efectivo | Mismo reparto. |
| Cierre de caja de pedidos especiales | Reparte la venta mixta entre efectivo y tarjeta. |
| Cancelación y devolución de una venta mixta | El reembolso sale **100 % en efectivo** (decisión del cliente). El cierre agrega una columna/apartado **"Cancelación de venta mixta"** con el monto reembolsado, separado de las demás cancelaciones. Así el cierre cuadra y se ve cuánto efectivo salió por ventas que se cobraron en parte con tarjeta (cuya parte de tarjeta sigue cobrada en la terminal hasta que se gestione su devolución). |
| Reporte histórico de cierres (Reportes → Cierres) | Muestra también el monto de "Cancelación de venta mixta". El reporte lee valores guardados al hacer el cierre, por lo que el monto debe **guardarse** en el registro del cierre. Es el único otro reporte donde el monto debe aparecer. |
| Tickets de cierre | Reflejan los importes ya repartidos y la nueva línea. |

**Cómo funciona hoy el cierre (verificado en los procedimientos):** las ventas canceladas **no se excluyen** de los totales de venta; el monto cancelado se **resta después** (`montoVentasCanceladas`). Por eso una venta mixta de $1,000 (tarjeta $800) cancelada el mismo día aporta $200 al contado, $800 a tarjeta y resta $1,000 por cancelación: el efectivo disponible queda en −$800, que es lo que realmente ocurrió en caja (entraron $200, salieron $1,000). La nueva línea **no cambia ese cálculo**; separa del total cancelado lo que corresponde a ventas mixtas.

**Puntos a verificar en la implementación:**
- El aviso de exceso de efectivo solo cuenta como ingreso las ventas con forma de pago Efectivo; debe sumar la parte en efectivo de las mixtas.
- Si la cancelación escribe registros en devoluciones, el cierre resta ese monto del total cancelado; comprobar que el reembolso de una venta mixta no se descuente dos veces.
- Cierre de pedidos especiales: tiene flujo propio y no usa esta lógica de cancelación de ventas; no cambia por este punto.

### 7.3 Reportes de consulta

Las ventas mixtas se mostrarán con forma de pago **"Mixto"** y el filtro por forma de pago seguirá funcionando. Además, se agregan las columnas **Tarjeta** y **Efectivo** (también en las exportaciones) en:

| Pantalla |
|----------|
| Consulta de ventas |
| Ventas canceladas |
| Reporte de ventas |
| Reporte de ventas de pedidos especiales |

En ventas no mixtas, la columna que corresponda a su forma de pago muestra el total y la otra queda en $0. La lista de pantallas puede ampliarse si durante la implementación se detecta otra donde sea necesario.

### 7.4 Datos históricos

Las ventas anteriores **no se modifican** ni se recalculan.

---

## 8. Impacto técnico (resumen)

> Sección para el equipo técnico. Verificado contra la base de datos de desarrollo y el código.

### 8.1 Base de datos

| Cambio | Detalle |
|--------|---------|
| Columna nueva `montoTarjeta` (nullable) | En `Ventas` y en `PedidosEspeciales`. Vacío = venta no mixta. No requiere reescribir los 761,560 registros existentes de `Ventas`. |
| Registro nuevo en `FactCatFormaPago` | Forma de pago "Mixto" con id propio (como ya existe el id 100 "Crédito"). |
| Columna nueva en el registro de cierres del día (`RetirosCierreDia`) | Monto de "Cancelación de venta mixta", para que el reporte histórico de cierres lo muestre. |

El efectivo no se almacena: se obtiene como `montoTotal − montoTarjeta`. `montoPagado` se mantiene como "monto recibido" (tarjeta + efectivo entregado), de modo que el cálculo actual del cambio no se altera.

Las tablas afectadas no tienen triggers y ningún procedimiento inserta en `Ventas` sin lista de columnas, por lo que agregar columnas no rompe procedimientos existentes.

### 8.2 Procedimientos almacenados

| Procedimiento | Cambio |
|---------------|--------|
| `SP_REALIZA_VENTA` | Recibe y valida `montoTarjeta`; rechaza Mixto con facturación. |
| `SP_CONFIRMAR_PRODUCTOS_PEDIDOS_ESPECIALES_V2` | Igual; rechaza además Mixto con crédito. |
| `SP_GUARDA_IVA_VENTA`, `SP_GUARDA_IVA_PEDIDO_ESPECIAL_V2` | Rechazan ventas/pedidos Mixtos. |
| `SP_CONSULTA_EXCESO_EFECTIVO` | Suma la parte en efectivo de ventas mixtas. |
| `SP_CONSULTA_INFO_CIERRE`, `SP_RETIRA_EFECTIVO` | Reparten la venta mixta entre efectivo y tarjeta; calculan y guardan el monto de "Cancelación de venta mixta". |
| `SP_CONSULTA_CIERRES_DIA` | Devuelve el monto guardado para el reporte histórico de cierres. |
| `SP_CONSULTA_CIERRE_PEDIDOS_ESPECIALES`, `SP_CONSULTA_INFO_CIERRE_PEDIDOS_ESPECIALES` | Igual, para pedidos especiales. |
| `SP_CONSULTA_TICKET`, `SP_CONSULTA_TICKET_PEDIDO_ESPECIALV2`, `SP_CONSULTA_DETALLE_TICKET_PEDIDO_ESPECIAL` | Devuelven `montoTarjeta`. |
| Procedimientos de las 4 pantallas de reportes (sección 7.3) | Devuelven columnas Tarjeta y Efectivo. Se identifican en la implementación. |
| `SP_FACTURACION_OBTENER_DATOS_FACTURA` (y variante de pedido especial) | Validación defensiva. |

### 8.3 Aplicación

| Área | Archivos principales |
|------|----------------------|
| Pantalla de ventas | `Views/Ventas/Ventas.cshtml`, `js/EvtVentas.js` |
| Pantalla de pedidos especiales | `js/EvtConfirmarProductosV2.js` |
| Controladores y DAO | `VentasController`, `PedidosEspecialesV2Controller`, `VentasDAO`, `PedidosEspecialesV2DAO` |
| Tickets | `Models/Ticket.cs`, `Utilerias/Utils.cs`, impresión en los controladores |
| Reportes | Vistas y `js` de Consulta de ventas, Ventas canceladas, Reporte de ventas y Reporte de ventas de pedidos especiales, con sus exportaciones. |
| Proyecto `PrintDocumentolluvia` | Mantiene copias de DAO, modelo y tickets; deben actualizarse igual. |

El catálogo de formas de pago alimenta varias pantallas; "Mixto" debe ocultarse donde no aplica (facturación, abonos).

---

## 9. Puntos definidos con el cliente

Respuestas recibidas el 2026-10-11.

| # | Punto | Respuesta del cliente | Efecto en el diseño |
|---|-------|-----------------------|---------------------|
| 1 | Cancelación y devolución de una venta mixta | Todo en **efectivo** (devolver a tarjeta tarda de 8 a 15 días). Devolución parcial: también en efectivo. El cierre muestra un apartado propio de **"Cancelación de venta mixta"** con el monto. | Sin reparto proporcional; ver sección 7.2. |
| 2 | Efectivo entregado y cambio | El cajero **captura** el efectivo entregado; el sistema muestra el cambio. | Tercer recuadro en pantalla; el ticket conserva RECIBIDO y SU CAMBIO. |
| 3 | Agregar productos a una venta ya pagada | **Una sola** forma de pago. | Sin cambios en ese cobro. |
| 4 | Tipo de tarjeta | Solo **"Tarjeta"**; ningún reporte necesita separar crédito de débito. | Una sola columna `montoTarjeta`. |
| 5 | Pedidos especiales a crédito | **No** se permite Mixto en pedidos a crédito o con abono. | El servidor rechaza Mixto con crédito. |
| 6 | Pedidos en ruta | **No aplica** Mixto. | Sin pantalla adicional en el alcance. |
| 7 | Desglose en reportes | **Sí**: columnas Tarjeta y Efectivo donde sea necesario. | Se agrega la sección 7.3; tiene costo adicional. |

---

## 10. Fuera de alcance

- Cuentas por Cobrar y abonos.
- Pedidos especiales a crédito o con abono, y cobro de pedidos en ruta.
- Cobro adicional al agregar productos a una venta ya pagada.
- Distinguir crédito de débito en ventas mixtas.
- Devolución del monto de tarjeta a la tarjeta del cliente (se reembolsa en efectivo).
- Facturación de ventas mixtas.
- Pagos con más de dos medios (por ejemplo efectivo + tarjeta + transferencia).
- Cambios a ventas históricas.
- Comisión bancaria.

---

## 11. Riesgos

| Riesgo | Mitigación |
|--------|------------|
| Sistema en producción con cierres de caja que dependen de la forma de pago. | Cambios probados en una copia de la base antes de producción; comparar cierres de caja con y sin ventas mixtas. |
| Una venta mixta que llegue a facturación. | Bloqueo en pantalla **y** en la base de datos (sección 6). |
| Copias de código en `PrintDocumentolluvia`. | Incluirlas en el alcance y las pruebas. |
| Otros sistemas (por ejemplo handheld) que usen los mismos procedimientos. | Confirmar con el cliente qué sistemas externos escriben ventas; desde este repositorio no es verificable. |
| No hay pruebas automatizadas en el proyecto. | Pruebas manuales con los casos de la sección 12. |

---

## 12. Casos de prueba de aceptación

| # | Caso | Resultado esperado |
|---|------|--------------------|
| P1 | Venta de $1,000, Mixto, tarjeta $800. | Efectivo calculado $200; se guarda; ticket muestra tarjeta $800 y efectivo $200. |
| P2 | Venta Mixto capturando primero el efectivo ($300 de $1,000). | Tarjeta calculada $700. |
| P3 | Mixto con tarjeta igual al total. | Se rechaza (regla R2). |
| P4 | Mixto con tarjeta en $0 o negativa. | Se rechaza. |
| P5 | Mixto con "Facturar esta venta" marcado. | La opción está deshabilitada; si se fuerza, el servidor rechaza. |
| P6 | Intentar facturar después una venta Mixta. | El sistema lo rechaza con mensaje claro. |
| P7 | Cierre de caja con una venta mixta de $1,000 (tarjeta $800). | Efectivo +$200 y tarjeta +$800. |
| P8 | Aviso de exceso de efectivo con ventas mixtas. | Considera solo la parte en efectivo. |
| P9 | Pedido especial confirmado con Mixto. | Igual que P1; cierre de pedidos especiales correcto. |
| P10 | Pedido especial a crédito o con abono intentando Mixto. | Se rechaza. |
| P11 | Venta normal en efectivo, tarjeta y transferencia. | Sin cambios respecto a hoy (pantalla, ticket y cierre). |
| P12 | Ventas históricas en reportes y cierres. | Sin cambios. |
| P13 | Cancelación completa de una venta mixta de $1,000 (tarjeta $800). | El reembolso es de $1,000 en efectivo; el cierre muestra $1,000 en "Cancelación de venta mixta"; ticket coherente. |
| P14 | Devolución parcial de $300 en una venta mixta. | El reembolso es en efectivo. |
| P15 | Mixto con tarjeta $800, efectivo a cobrar $200, el cliente entrega $500. | Cambio $300; ticket muestra RECIBIDO $1,300 y SU CAMBIO $300. |
| P16 | Consulta de ventas, Ventas canceladas, Reporte de ventas y Reporte de ventas de pedidos especiales con ventas mixtas y no mixtas. | Columnas Tarjeta y Efectivo correctas en pantalla y en exportaciones; el total coincide con el de la venta. |
| P17 | Cobro de un pedido en ruta; cobro adicional a una venta ya pagada. | "Mixto" no se ofrece. |

---

## 13. Plan de implementación propuesto

| Fase | Contenido |
|------|-----------|
| 1 | Cambios en base de datos (columnas, forma de pago Mixto) y scripts reversibles. |
| 2 | Ventas: pantalla, validaciones y procedimiento de guardado. |
| 3 | Pedidos Especiales: pantalla, validaciones y procedimiento de confirmación. |
| 4 | Bloqueo de facturación (pantalla y servidor). |
| 5 | Cierres de caja, exceso de efectivo y retiros. |
| 6 | Tickets (venta y pedido especial). |
| 7 | Cancelaciones y devoluciones (reembolso en efectivo) y apartado "Cancelación de venta mixta" en el cierre. |
| 8 | Reportes: columnas Tarjeta y Efectivo en las 4 pantallas y sus exportaciones. |
| 9 | Pruebas de aceptación con el cliente y salida a producción con respaldo previo. |

---

## 14. Aprobación

- [ ] El cliente confirma el funcionamiento de las secciones 3 a 7.
- [x] El cliente respondió los puntos de la sección 9 (2026-10-11).
- [ ] Se aprueba el plan y los casos de prueba.

| Nombre | Rol | Fecha | Firma |
|--------|-----|-------|-------|
|        |     |       |       |
