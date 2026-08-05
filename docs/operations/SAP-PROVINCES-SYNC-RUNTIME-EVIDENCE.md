# Evidencia runtime — despliegue SQL de Provincias

## Resultado

- Fecha: 2026-08-04.
- Master: `NuanSystem_Master`.
- Tenants: `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y
  `NuanSystem_DEMO_CANARIS`.
- Estado: estructura SQL desplegada y validada; SAP y workers no ejecutados.

## Respaldos

Los siguientes respaldos `COPY_ONLY WITH CHECKSUM` aprobaron
`RESTORE VERIFYONLY WITH CHECKSUM`:

- `/var/opt/mssql/data/NuanSystem_Master_ProvinceDeploy_20260804_195937.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_ProvinceDeploy_20260804_195937.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_REMIGIO_ProvinceDeploy_20260804_195937.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_CANARIS_ProvinceDeploy_20260804_195937.bak`.

## Despliegue

- `085` se ejecutó en Remigio y Cañaris porque no tenían `dbo.Provinces`.
- `172` y `173` se ejecutaron en DEMO, Remigio y Cañaris.
- `174` se ejecutó únicamente en Master.
- La secuencia completa se repitió y cada versión quedó registrada una vez.
- Master registra `Provinces` como `SapToErp + Full`, sin Incremental ni
  ERP→SAP.
- No se crearon entidades de perfil ni agendas para Provincias.

La primera ejecución de `173` se detuvo en DEMO antes de registrar la versión:
`OBJECT_DEFINITION` devolvió el procedimiento existente con encabezado
`CREATE   PROCEDURE`. El script se corrigió para normalizar de forma segura el
encabezado a `CREATE OR ALTER PROCEDURE`; las pruebas del contrato aprobaron y
la ejecución se reanudó satisfactoriamente. No se restauró el respaldo porque
el estado parcial era idempotente: el check ya incluía `ProvinceV1`, el
procedimiento seguía intacto y la versión `173` continuaba ausente.

## Validación final

| Base | 172 | 173 | Países activos | Provincias activas | Vinculadas SAP |
|---|---:|---:|---:|---:|---:|
| DEMO | 1 | 1 | 250 | 3 | 0 |
| Remigio | 1 | 1 | 250 | 0 | 0 |
| Cañaris | 1 | 1 | 250 | 0 | 0 |

- `DBCC CHECKCONSTRAINTS('dbo.Provinces')` no reportó violaciones.
- Los tres tenants tienen reserva única de código incluso para tombstones,
  referencia externa única, aplicador jerárquico y snapshot `ProvinceV1`.
- Master contiene una versión `20260804.174`, una capacidad `Provinces` y cero
  perfiles/agendas asociados.
- Los tres tenants registran cero locks, cero ejecuciones SAP y cero eventos
  `LocalOutbox` de Provincias.
- No quedó ningún proceso `NuanSystem.MasterBranchSyncWorker` ni
  `NuanSystem.SyncWorker` activo. La configuración existente no fue modificada.
- No se consultó SAP, no se activaron workers, no se llamó SRI y no se hizo
  commit, push ni integración a `master`.

## Preview SAP autorizado

El preview read-only se ejecutó contra DEMO mediante el transporte y lector
paginado existentes, con TLS estricto y consulta
`States?$orderby=Country,Code`. La sesión se cerró al terminar y no se
registraron credenciales, cookies ni payloads completos.

- Registros leídos: 95.
- Países SAP distintos: 3.
- Chile: 15; Ecuador: 24; Estados Unidos: 56.
- Filas inválidas: 0.
- Identidades `COUNTRY|STATE` duplicadas: 0.
- Nuevas: 95.
- Existentes, diferentes, aprobaciones y conflictos: 0.

Las tres provincias locales de DEMO son `EC/AZU`, `EC/GYE` y `EC/PIC`, todas
activas y sin referencia SAP. Ninguna colisionó con los códigos devueltos por
SAP; por eso el preview clasificó las 95 filas como nuevas.

Después del preview se confirmaron cero `SapSyncLog`, cero ejecuciones, cero
locks y cero `LocalOutbox` de Provincias.

## Full SAP → DEMO autorizado

Antes de importar se creó y verificó con checksum el respaldo:

- `/var/opt/mssql/data/NuanSystem_DEMO_ProvinceImport_20260804_201909.bak`.

Baseline: 3 provincias locales, cero Outbox y cero logs de Provincias. Se
preservaron exactamente los `GlobalId`, estado y ausencia de referencia SAP de
`EC/AZU`, `EC/GYE` y `EC/PIC`.

Primer Full:

- 95 leídas y 95 creadas;
- 0 actualizadas, sin cambios, aprobaciones, conflictos, omitidas o fallidas.

Segundo Full idempotente:

- 95 leídas y 95 sin cambios;
- 0 creadas, actualizadas, aprobaciones, conflictos, omitidas o fallidas.

Estado posterior:

- 98 provincias activas en DEMO: 95 SAP y 3 locales;
- SAP: Chile 15, Ecuador 24 y Estados Unidos 56;
- 0 referencias externas inválidas o duplicadas;
- 95 eventos `LocalOutbox` distintos, todos `Pending`;
- 2 logs SAP `Succeeded`;
- 0 ejecuciones programadas y 0 locks de Provincias;
- Remigio y Cañaris permanecen con 0 provincias;
- `DBCC CHECKCONSTRAINTS` de `Provinces` y `LocalOutbox` sin violaciones.

Master conserva tres eventos históricos `EC/AZU`, `EC/GYE` y `EC/PIC`, creados
el 2026-07-16 por `E2E-GEO-20260716181024` y ya aplicados. No proceden de esta
importación: los 95 eventos nuevos continúan exclusivamente en LocalOutbox.

## Distribución DEMO → Remigio/Cañaris autorizada

Antes de iniciar se crearon y verificaron con checksum respaldos coordinados:

- `/var/opt/mssql/data/NuanSystem_Master_ProvinceDistribution_20260804_202816.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_ProvinceDistribution_20260804_202816.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_REMIGIO_ProvinceDistribution_20260804_202816.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_CANARIS_ProvinceDistribution_20260804_202816.bak`.

Se creó mediante Application el perfil temporal `5004`,
`PROVINCES-DISTRIBUTION-20260804`, con Countries como dependencia y Provinces
como objetivo Incremental para Remigio `1002` y Cañaris `1003`. El worker se
ejecutó una sola vez con configuración de proceso, relay habilitado y allowlist
exclusiva `Provinces`; SAP y SRI permanecieron fuera.

Resultado:

- 95 `LocalOutbox` promovidos y cerrados `Applied`;
- 95 `SyncOutbox` de origen `LocalOutbox`, todos `Applied`;
- Remigio: 95 targets e Inbox `Applied`;
- Cañaris: 95 targets e Inbox `Applied`;
- 0 errores, DeadLetter o locks ajenos;
- 0 faltantes, diferencias o extras al comparar `GlobalId`,
  `CountryGlobalId`, código, nombre, estado y referencia externa;
- Remigio y Cañaris terminaron con 95 provincias y 95 `GlobalId` distintos;
- `DBCC CHECKCONSTRAINTS` de Provinces e Inbox sin violaciones en ambas;
- perfil `5004` desactivado y worker detenido;
- el log temporal del worker registró 0 líneas de error.

Las tres provincias locales de DEMO no se distribuyeron en esta oleada porque
no tenían eventos nuevos en LocalOutbox. Los tres eventos históricos de julio
en Master permanecieron sin cambios.

## Próximo gate

La cadena Países → Provincias queda cerrada. El próximo alcance independiente
es diseñar e implementar Ciudades con dependencia `ProvinceGlobalId`.
