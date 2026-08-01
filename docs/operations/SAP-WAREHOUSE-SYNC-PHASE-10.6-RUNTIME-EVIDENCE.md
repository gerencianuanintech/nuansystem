# Fase 10.6 — Evidencia runtime Bodegas SAP → DEMO

## Resultado

- Fecha: 2026-07-31.
- Rama: `refactor/codex-skills-v10-6-sap-warehouses`.
- Alcance: lectura Full de Bodegas desde SAP Business One y aplicación exclusiva en `NuanSystem_DEMO`.
- Estado: **Validada**.
- Remigio y Cañaris: no modificados.
- Matriz–Sucursal y SRI: fuera de alcance y deshabilitados.

La validación demostró el recorrido SAP Service Layer → `NuanSystem.SyncWorker` → historial tenant → mantenimiento Warehouse de DEMO. No se enviaron datos hacia SAP.

## Seguridad y respaldo

Antes del piloto se crearon y verificaron con `RESTORE VERIFYONLY WITH CHECKSUM` los respaldos:

- `NuanSystem_Master_Phase106_WarehousePilot_20260731_231950.bak`.
- `NuanSystem_DEMO_Phase106_WarehousePilot_20260731_231950.bak`.

Las conexiones y credenciales se cargaron exclusivamente desde configuración local ignorada, en memoria y con TLS estricto. No se registraron secretos, cookies, payloads de login ni cadenas de conexión.

## Lectura SAP y ciclos reales

| Evidencia | Resultado |
|---|---|
| Bodegas leídas desde SAP | 24 |
| Bodegas SAP activas | 24 |
| Identidades vinculadas en DEMO | 24 |
| Duplicados o colisiones inesperadas | 0 |
| Campos SAP administrados con diferencias | 0 |
| Primer ciclo Full | 24 `Unchanged`, ejecución `Completed` |
| Segundo ciclo Full | 24 `Unchanged`, ejecución `Completed` |
| Snapshots | 24 por ciclo, tipo `WarehouseV1`, SHA-256 de 32 bytes |
| Mutaciones Warehouse durante los dos ciclos | 0 |
| Eventos LocalOutbox creados | 0 |

El perfil, entidad y agenda temporales usaron identificadores `ITER106-WH-*`, `BatchSize=5` y una única instancia de worker. Fueron retirados al finalizar.

## Casos funcionales controlados

Los casos que requerían mutación se ejecutaron con fixtures identificables y se limpiaron después:

| Caso | Resultado aprobado |
|---|---|
| Nueva bodega SAP activa | `Created` |
| Bodega vinculada con cambios | `Updated` |
| Bodega SAP nueva inactiva | `Skipped` |
| Segundo procesamiento sin cambios | `Unchanged` |
| Colisión únicamente por código | `ApprovalRequired`, sin adopción automática |
| Bodega vinculada reportada inactiva por SAP | `ApprovalRequired`; DEMO conservó el estado activo |
| Identidad | `GlobalId` preservado |
| Campos locales | Preservados |

## Cancelación y reintento

El primer intento controlado reveló que una cancelación durante la inicialización podía dejar la cabecera en `Running`. Se corrigió el alcance del bloque protegido y, durante el gate real, se detectó además que el cierre debía respetar la transición SQL oficial `Running → Cancelling → Cancelled`.

Correcciones:

- `1fb32aeb` — `fix(sap): close cancelled warehouse initialization`.
- `31be91f` — `fix(sap): honor warehouse cancellation transitions`.

Resultado posterior contra el repositorio Dapper real de DEMO:

| Gate | Resultado |
|---|---|
| Cancelación después de persistir `Running` | `Cancelled` |
| Código seguro | `SAP_WAREHOUSE_EXECUTION_INTERRUPTED` |
| Lectura SAP después de cancelar inicialización | No ejecutada |
| Fallo transitorio simulado por registro | Cabecera y detalle `RetryScheduled` |
| Intento persistido | 1 |
| Próximo intento | Informado |
| Evidencia temporal | 2 cabeceras, 1 detalle y 8 auditorías |
| Limpieza final | 0 cabeceras, detalles y auditorías fixture |

El gate de retry empleó una lectura tipada simulada y el repositorio tenant real; no llamó SAP. El primer y segundo intento fallido del harness también ejecutaron limpieza automática y no dejaron residuos.

## Preservación y cierre

- Bodegas DEMO finales: 35, igual al baseline posterior a la limpieza del piloto.
- Ejecuciones, detalles y locks fixture: 0.
- Perfiles, entidades y agendas SAP activos: 0.
- Heartbeat temporal del piloto: eliminado.
- Procesos `NuanSystem.*` finales: 0.
- Remigio y Cañaris: no consultados ni modificados durante los gates correctivos.
- No hubo llamadas SRI ni escrituras hacia SAP.

## Compilación y pruebas finales

| Validación | Resultado |
|---|---|
| Pruebas dirigidas del procesador | 4/4 aprobadas |
| Pruebas de Bodegas SAP | 32/32 aprobadas |
| Build de `NuanSystem.sln` | 0 errores, 0 advertencias |
| Suite completa | 721 aprobadas, 5 diagnósticas omitidas, 0 fallidas |
| `git diff --check` | Aprobado |

Las cinco pruebas omitidas requieren infraestructura explícita y no representan fallos.

## Conclusión

La Fase 10.6 queda cerrada para el piloto SAP Business One → DEMO. La configuración operativa permanece deshabilitada. El siguiente alcance es la Fase 10.7 WinForms para perfiles y ejecuciones SAP; no debe activarse una operación permanente ni ampliar tenants sin una aprobación independiente.
