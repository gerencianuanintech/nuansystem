# Prueba E2E de geografia Maestro-Sucursal

## Alcance

Prueba controlada del flujo Full para `Countries`, `Provinces` y `Cities` desde
la empresa maestra `DEMO` hacia `SYNC-WH-BRANCH-TEST`.

Fecha de ejecucion: 2026-07-16.

## Configuracion utilizada

- Perfil: `TEST-MB-CATALOGS-FULL`.
- Ejecucion: `4003`.
- Correlacion: `E2E-GEO-20260716181024`.
- Base fuente: `NuanSystem_DEMO`.
- Base destino: `NuanSystem_SYNC_WH_BRANCH_TEST`.
- Entidades y orden: `Countries` (10), `Provinces` (20), `Cities` (30).
- Limite de seguridad: 100 registros por entidad.

`Currencies` y `SupplierGroups` se deshabilitaron temporalmente porque todavia
no son entidades operativas. Al finalizar, el perfil quedo inactivo y sus cinco
entidades volvieron a su estado de borrador original.

## Resultado de la publicacion

La API proceso la ejecucion administrativa mediante
`SyncProfileExecutionHostedService`:

| Entidad | Leidos | Publicados | Omitidos | Errores |
| --- | ---: | ---: | ---: | ---: |
| Countries | 3 | 3 | 0 | 0 |
| Provinces | 3 | 3 | 0 | 0 |
| Cities | 3 | 3 | 0 | 0 |
| Total | 9 | 9 | 0 | 0 |

Se crearon nueve eventos `SyncOutbox` (`30002` a `30010`) y nueve targets para
la empresa destino. No habia otros eventos reclamables durante la prueba.

## Resultado del Worker

`NuanSystem.MasterBranchSyncWorker` se ejecuto con `SkeletonMode = false` y los
aplicadores geograficos habilitados. Los nueve eventos y targets finalizaron en
estado `Applied`, sin mensajes de error.

Comparacion final:

| Entidad | Fuente | Destino | Diferencias |
| --- | ---: | ---: | ---: |
| Countries | 3 | 3 | 0 |
| Provinces | 3 | 3 | 0 |
| Cities | 3 | 3 | 0 |

La comparacion verifico `GlobalId`, datos basicos, estado y relaciones
`CountryGlobalId`/`ProvinceGlobalId`. Cada evento tiene un unico registro
`SyncInbox` aplicado en la sucursal.

## Idempotencia

El evento `30008` (`EC|AZU|CUE`) se devolvio controladamente a `Pending` y se
proceso otra vez con su Inbox ya aplicado. El resultado fue:

- Outbox y target nuevamente `Applied`.
- Tres ciudades antes y despues del reproceso.
- Un solo registro Inbox para el `EventId`.

## Estado restaurado

- Perfil `TEST-MB-CATALOGS-FULL`: inactivo.
- Entidades del perfil: cinco activas como borrador.
- Ejecuciones activas del perfil: cero.
- La ejecucion, Outbox, targets, Inbox y auditoria se conservaron como evidencia.

## Observacion de entorno

El SQL Server local no negocio el cifrado exigido por la configuracion
`SqlConnectionPolicy:Encrypt = true`. Para esta prueba se uso un override de
proceso `Encrypt = false`; no se modificaron archivos de configuracion. Antes de
usar este entorno con cifrado obligatorio se debe corregir TLS/certificado en SQL
Server, en lugar de trasladar este override a produccion.
