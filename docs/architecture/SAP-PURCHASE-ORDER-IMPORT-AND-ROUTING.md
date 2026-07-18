# Importacion y enrutamiento de ordenes de compra SAP

## Decision

Las ordenes SAP Business One se importan como un caso de uso operativo en el backend. `Domain` no conoce Service Layer y WinForms solo consume API REST. La integracion sigue siendo opcional por empresa.

## Identidad y version

- `SapDocEntry` es la identidad externa estable y unica dentro de la empresa SAP.
- `SapDocNum` es el numero visible y no se usa como clave de idempotencia.
- `GlobalId` identifica el documento NuanSystem durante la distribucion Master/Sucursal.
- `SapUpdatedAt` y `SapVersion` impiden que una lectura antigua sobrescriba una version mas nueva.

## Mapeo

- Cabecera: fechas de contabilizacion y entrega, proveedor, moneda, tipo de cambio, comentarios, subtotal, descuento, impuestos, total, estado, cancelacion y cierre.
- Lineas: `LineNum`, articulo, cantidad abierta/total, precio, descuento, impuesto, unidad, bodega y fecha de entrega.
- Proveedor, articulo, moneda, impuesto, unidad y bodega se resuelven por referencia externa SAP y luego por codigo.

## Politica de actualizacion

- La cabecera y todas sus lineas se aplican en una unica transaccion.
- Repetir la misma version produce `Unchanged`.
- Una version posterior reemplaza los datos importables y las lineas.
- Una version anterior se ignora y queda auditada.
- SAP cerrado se conserva como `Closed`; SAP cancelado como `Cancelled`. No se elimina fisicamente el documento.

## Enrutamiento

- Una orden cuyas lineas pertenecen a una sola sucursal se enruta a esa sucursal.
- Una orden que mezcla bodegas de distintas sucursales queda `NeedsApproval`; no se divide.
- La aprobacion manual conserva usuario, fecha, destino y motivo.
- La distribucion usa Outbox/Inbox e idempotencia por `GlobalId`.
