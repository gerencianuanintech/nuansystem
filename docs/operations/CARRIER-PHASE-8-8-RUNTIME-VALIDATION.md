# Transportistas 8.8 — evidencia runtime Matriz–Sucursal

## Resultado

El piloto runtime de Transportistas quedó validado el 1 de agosto de 2026 con
`NuanSystem_DEMO` como Matriz y `NuanSystem_DEMO_REMIGIO` como única sucursal.
`NuanSystem_DEMO_CANARIS` permaneció en solo lectura y no recibió targets.

La ejecución utilizó la API real, `LocalOutbox`, promoción idempotente a
Master y `NuanSystem.MasterBranchSyncWorker` en modo real, limitado al
aplicador `Carrier`. SAP y SRI permanecieron fuera de alcance.

## Respaldos

Antes de crear fixtures se generaron respaldos `COPY_ONLY WITH CHECKSUM` y se
verificaron mediante `RESTORE VERIFYONLY WITH CHECKSUM`:

- `NuanSystem_Master_Phase88_CarrierRuntime_20260801_230248.bak`;
- `NuanSystem_DEMO_Phase88_CarrierRuntime_20260801_230248.bak`;
- `NuanSystem_DEMO_REMIGIO_Phase88_CarrierRuntime_20260801_230248.bak`.

## Configuración temporal

Se creó el perfil identificable `I88-CARRIER-RUNTIME` con estas condiciones:

- origen `DEMO` y destino único `DEMO-REMIGIO`;
- dirección `MasterToBranch`;
- modo `Incremental`;
- estrategia `MasterWins`;
- distribución `All`;
- entidad única `Carrier`;
- relay habilitado únicamente mediante configuración del proceso;
- `SkeletonMode=false` y `EnabledEntityAppliers=[Carrier]`.

La configuración global y el ownership de `Carrier` se habilitaron solo durante
el piloto. El perfil fue eliminado y ambos flags regresaron exactamente a
deshabilitado al finalizar.

## Gates aprobados

| Gate | Evidencia |
|---|---|
| CRUD real | Create, update, disable y delete lógico respondieron HTTP 200 mediante `/api/carriers` |
| Atomicidad | Un fallo inducido al insertar `LocalOutbox` devolvió HTTP 500 y revirtió Carrier, evento y trigger temporal |
| Desacoplamiento | El create quedó confirmado en DEMO y `LocalOutbox=Pending` mientras relay y worker estaban detenidos; Master tenía cero eventos |
| Promoción | Cinco eventos Carrier se promovieron una sola vez a cinco eventos Master |
| Routing | Cada evento creó exactamente un target hacia Remigio y ninguno hacia Cañaris |
| Aplicación | Remigio procesó cinco Inbox; las cuatro transiciones del lifecycle quedaron aplicadas |
| Identidad | La aplicación utilizó `GlobalId`; el `Id` local no se replicó |
| Tombstone | El estado final fue `IsActive=0`, `IsDeleted=1` y la recreación del código fue rechazada con HTTP 400 |
| Colisión | Un código ocupado por otro `GlobalId` terminó en `DeadLetter` sin modificar ni adoptar la fila local |
| Idempotencia | Reprogramar el evento `Updated` conservó cinco eventos, cinco targets y cinco Inbox |
| Master no disponible | Un worker con conexión Master inaccesible, solo en su proceso, dejó el evento `C88M` pendiente y sin filas en Master/Remigio; al restaurar la conexión se promovió y aplicó una sola vez |
| Eventos ajenos | 1.420 eventos y 2.209 targets no Carrier conservaron conteos y fingerprints exactos antes, durante y después |
| Limpieza | Cero fixtures `C88*`, cero perfil temporal, cero locks y cero procesos NuanSystem al cierre |

La protección de eventos ajenos depende además del endurecimiento que filtra
el claim de Master por `EnabledEntityAppliers`; el worker no reclamó los siete
eventos históricos elegibles de otras entidades.

## Conteos del lifecycle antes de limpiar

- `LocalOutbox Carrier`: 5 filas, las 5 promovidas;
- `SyncOutbox Carrier`: 5 filas;
- `SyncOutboxTargets`: 5 filas, todas para Remigio;
- lifecycle `C88L`: 4 eventos aplicados;
- colisión `C88C`: 1 evento y target en `DeadLetter`;
- `SyncInbox` Remigio: 5 filas;
- `SyncInbox` Cañaris: 0 filas.

El escenario adicional de indisponibilidad usó `C88M`. Permaneció pendiente
durante la conexión fallida y se aplicó una vez después de restablecerla; luego
se eliminó junto con toda su trazabilidad.

## Regresión

- Build completo: 0 errores y 0 advertencias.
- Suite completa: 747 aprobadas, 5 diagnósticas omitidas y 0 fallidas.
- `git diff --check`: aprobado antes del commit documental.
- API, worker y harness temporal: detenidos al finalizar.

## Estado final

- `Carrier` permanece deshabilitado por defecto en configuración y ownership.
- No existe el perfil temporal `I88-CARRIER-RUNTIME`.
- No quedan filas `C88*` en DEMO, Remigio, Cañaris, LocalOutbox, SyncOutbox,
  targets, Inbox o auditorías.
- SAP, Service Layer, SRI y sus workers no fueron iniciados.
- La evidencia aplica exclusivamente al piloto DEMO → Remigio.
