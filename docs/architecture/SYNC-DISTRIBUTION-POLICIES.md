# Politicas de distribucion Master-Sucursal

## Estado

Decision aprobada para el piloto SAP B1 de la empresa `DEMO`. La activacion de
destinos reales queda condicionada a la existencia de las empresas sucursales
Remigio y Paseo de los Canaris en Master.

## Objetivo

SAP y cualquier otro origen integrado importan primero el universo completo en
la base Master. La distribucion posterior a cada base de sucursal se decide por
entidad y sucursal, sin que SAP conozca la topologia interna de NuanSystem.

## Modos

Cada celda `Perfil x Entidad x Sucursal` tiene un modo de distribucion:

- `None`: no crea destino para la sucursal.
- `All`: distribuye todos los registros de la entidad.
- `Selected`: distribuye solo los `GlobalId` seleccionados.
- `Rule`: evalua una expresion declarativa sobre hechos autorizados de la entidad.

`KeepInMaster` es la unica accion inicial cuando una seleccion o regla no
coincide. El evento queda auditado en Master y no se crea `SyncOutboxTarget`.

## Reglas seguras

No se admiten SQL, scripts, nombres de tablas, reflection ni rutas arbitrarias.
La expresion usa JSON versionado, combinacion `All` o `Any`, campos publicados
en un catalogo por entidad y operadores permitidos (`Equals`, `NotEquals`,
`In`, `IsTrue`, `IsFalse`).

Ejemplo para una bodega:

```json
{
  "match": "All",
  "conditions": [
    { "field": "code", "operator": "Equals", "value": "20" }
  ]
}
```

Las selecciones se guardan por `GlobalId`; el codigo externo se conserva solo
como referencia legible.

## Flujo

1. El dato se crea o actualiza en Master.
2. Se publica un evento Outbox idempotente.
3. El perfil obtiene las sucursales candidatas.
4. La politica evalua cada candidata.
5. Solo las coincidencias generan `SyncOutboxTargets`.
6. Cada decision se registra con modo, version y motivo.
7. El worker entrega mediante Inbox idempotente a cada tenant.

## Compatibilidad

Las celdas existentes habilitadas migran a `All`; las deshabilitadas migran a
`None`. `TargetBranchCode` se conserva como filtro de compatibilidad para
productores que aun lo requieran. Bodegas deja de usarlo: su distribucion queda
gobernada por la politica para permitir tanto todos como seleccionados.

## Piloto de bodegas

- Master recibe todas las bodegas SAP activas.
- Remigio usara `Selected` con la bodega SAP `20`.
- Paseo de los Canaris usara `Selected` con la bodega SAP `11`.
- La politica no se activa hasta crear y validar ambos tenants.

## Evolucion

Items reutilizara los cuatro modos. Las ordenes de compra se incorporaran como
caso operativo y agregaran hechos autorizados como `warehouseCode`, manteniendo
Master como receptor completo antes de cualquier distribucion.
