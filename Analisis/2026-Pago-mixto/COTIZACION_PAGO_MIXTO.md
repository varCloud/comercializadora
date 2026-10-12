# Pago Mixto (Efectivo + Tarjeta) — Estimación de horas

**Sistema:** lluviaBackEnd (Comercializadora Lluvia)
**Fecha:** 2026-10-12
**Base:** Análisis funcional del Pago Mixto (ANALISIS_PAGO_MIXTO)

---

## 1. Desglose por fase

| Fase | Qué incluye | Horas |
|------|-------------|------:|
| 1 | Preparación de la base de datos: nuevos campos y la forma de pago "Mixto", con scripts reversibles. | 2 – 3 |
| 2 | Cobro de Ventas: pantalla con recuadros Tarjeta, Efectivo y Efectivo recibido; cálculo automático, cambio y validaciones. | 8 – 10 |
| 3 | Pedidos Especiales: mismo cobro al confirmar el pedido; se rechaza en pedidos a crédito o con abono. | 6 – 8 |
| 4 | Bloqueo de facturación de ventas mixtas, en pantalla y en el servidor. | 3 – 4 |
| 5 | Cierres de caja, aviso de exceso de efectivo y retiros de efectivo con el reparto Tarjeta / Efectivo. | 8 – 12 |
| 6 | Tickets de venta y de pedido especial (impresión directa y PDF) con las líneas PAGO CON TARJETA y PAGO EN EFECTIVO. | 5 – 7 |
| 7 | Cancelaciones y devoluciones reembolsadas en efectivo, y apartado "Cancelación de venta mixta" en el cierre y en el reporte histórico de cierres. | 6 – 8 |
| 8 | Reportes: columnas Tarjeta y Efectivo en Consulta de ventas, Ventas canceladas, Reporte de ventas y Reporte de ventas de pedidos especiales, incluyendo exportaciones. | 8 – 12 |
| 9 | Pruebas de aceptación con el cliente, prueba sobre copia de la base de datos y salida a producción con respaldo previo. | 6 – 8 |

## 2. Totales

| Concepto | Horas |
|----------|------:|
| Alcance completo (fases 1 a 9) | **52 – 72** (aprox. 60) |
| Sin reportes (fases 1 a 7 y 9) | **44 – 60** (aprox. 52) |

La fase 8 es el costo adicional ya mencionado en el análisis; puede contratarse en una segunda etapa sin afectar el resto.

## 3. Supuestos

- Los tiempos incluyen desarrollo y pruebas internas de cada fase.
- Se parte de las definiciones del análisis (sección 9, respuestas del cliente del 2026-10-11).
- Los cambios se prueban primero en una copia de la base de datos.
- Las fases 5 y 7 (cierres y cancelaciones) son las de mayor cuidado, porque afectan el efectivo disponible en caja.

## 4. No incluido

- Cuentas por Cobrar y abonos.
- Pedidos especiales a crédito o con abono, y cobro de pedidos en ruta.
- Facturación de ventas mixtas.
- Distinguir crédito de débito.
- Devolución del monto a la tarjeta del cliente.
- Cambios a ventas históricas.
- Adaptación de otros sistemas que escriban ventas (por ejemplo, handheld) si los hubiera; se confirma antes de iniciar.

## 5. Condición para iniciar

El cliente aprueba el funcionamiento, el plan y los casos de prueba (sección 14 del análisis).
