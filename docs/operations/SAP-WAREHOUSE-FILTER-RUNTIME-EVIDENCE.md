# Evidencia runtime — filtro de Bodegas SAP por perfil

Fecha: 2026-08-04  
Rama: `refactor/codex-skills-v10-sap-warehouse-filter`  
Commit funcional: `e71fe0a1` — `feat(sap): filter warehouses by profile`

## Objetivo

Limitar la importación SAP → NuanSystem_DEMO a las bodegas cuyo nombre:

- contiene `MEGA`; o
- es exactamente `FERIA LIBRE`.

Las dos condiciones se combinan con OR y pertenecen a la entidad `Warehouses`
del perfil SAP, no a una regla global codificada.

## Respaldo y despliegue

- Master: `/var/opt/mssql/data/NuanSystem_Master_WarehouseFilter_20260804_110649.bak`
- DEMO: `/var/opt/mssql/data/NuanSystem_DEMO_WarehouseFilter_20260804_110649.bak`
- Ambos respaldos se crearon con `COPY_ONLY WITH CHECKSUM` y aprobaron
  `RESTORE VERIFYONLY WITH CHECKSUM`.
- `166_master_sap_warehouse_profile_filter.sql` se ejecutó dos veces únicamente
  en `NuanSystem_Master`.
- `20260804.166` quedó registrada exactamente una vez.
- La configuración quedó asociada a `SapSyncProfileEntityId = 10057`.

## Estado antes de la limpieza

| Métrica | Conteo |
|---|---:|
| Bodegas visibles | 35 |
| Vinculadas a SAP_B1 | 24 |
| Seleccionadas por la regla | 9 |
| Excluidas por la regla | 15 |
| Bodegas locales | 11 |
| Dependencias ItemWarehouses de las excluidas | 0 |
| Eventos pendientes identificables de las excluidas | 15 |

Los 15 eventos eran exclusivamente `Warehouse / Created / Pending`.

## Limpieza autorizada

En una transacción serializable se eliminaron físicamente:

- 15 bodegas SAP excluidas;
- sus 15 eventos `LocalOutbox` pendientes e identificables.

No se modificaron las 9 bodegas seleccionadas ni las 11 bodegas locales.

## Ciclo real controlado

El primer intento controlado detectó que el fallback legado de `PaymentTerms`
se ejecutaba antes de Bodegas. Se detuvo sin crear una ejecución de Bodegas ni
modificar catálogos. El lock técnico se adquirió y liberó correctamente.

Para el segundo intento se deshabilitó temporalmente solo el fallback legado,
se activó la agenda de Bodegas y se inició `NuanSystem.SyncWorker`. Al finalizar
se restauraron exactamente el fallback, el perfil y la agenda.

Resultado de la ejecución `30022`:

| Métrica | Resultado |
|---|---:|
| Estado | Completed |
| Registros procesados | 9 |
| Creados | 0 |
| Actualizados | 0 |
| Sin cambios | 9 |
| Fallidos | 0 |
| ApprovalRequired | 0 |

Códigos procesados: `02`, `03`, `04`, `08`, `09`, `11`, `18`, `19`, `20`.

No se procesó ninguno de los 15 códigos excluidos. La interacción con SAP fue
exclusivamente de lectura; no se enviaron datos hacia SAP y no se llamó al SRI.

## Estado final

| Métrica | Conteo |
|---|---:|
| Bodegas visibles | 20 |
| Bodegas SAP seleccionadas | 9 |
| Bodegas SAP excluidas | 0 |
| Bodegas locales | 11 |
| Eventos Warehouse/Created/Pending | 9 |
| Perfiles SAP activos | 0 |
| Procesos NuanSystem activos | 0 |

El filtro permanece configurado, mientras el perfil y su agenda permanecen
deshabilitados. Remigio y Cañaris no fueron consultados ni modificados.

## Validación de código

- Pruebas dirigidas: 27 aprobadas, 0 fallidas.
- Build completo: 0 errores, 0 advertencias.
- Suite completa: 765 aprobadas, 5 omitidas, 0 fallidas.
- `git diff --check`: aprobado.

## Corrección arquitectónica posterior

El commit `24b9439` movió la aplicación del filtro desde el procesador de la
ejecución hacia la consulta tipada `SapWarehouseQuery` de Service Layer.

- El scheduler continúa transportando la configuración del perfil.
- El procesador solo solicita la lectura con esos criterios y procesa la
  respuesta recibida.
- `SapWarehouseQuery` genera el `$filter` OData con comparación normalizada,
  escape de literales y orden por `WarehouseCode`.

La ejecución `30022` conserva su valor como evidencia del resultado funcional
de nueve bodegas. Una validación posterior ejecutó directamente el lector real
contra SAP Service Layer, sin iniciar workers ni realizar escrituras.

El primer dialecto propuesto con `toupper(WarehouseName)` fue rechazado por SAP
con HTTP 400 y código 201 por parámetro de función inválido. La consulta se
corrigió a las expresiones compatibles `contains(WarehouseName, ...)` y
`WarehouseName eq ...`, manteniendo valores normalizados y literales escapados.

La consulta compatible inició sesión, leyó con paginación segura y cerró sesión.
SAP devolvió exactamente nueve códigos: `02`, `03`, `04`, `08`, `09`, `11`,
`18`, `19` y `20`. Todos sus nombres contienen `MEGA` o son exactamente
`FERIA LIBRE`; no se recibió ninguna bodega adicional.

Validación automática posterior: build completo sin errores ni advertencias,
40 pruebas de Bodegas SAP y suite completa con 768 aprobadas y 5 omitidas.
