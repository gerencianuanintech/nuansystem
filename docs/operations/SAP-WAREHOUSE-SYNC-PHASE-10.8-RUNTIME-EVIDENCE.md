# Fase 10.8 — Validación integral Bodegas SAP → DEMO

## Resultado

- Fecha: 2026-08-03.
- Rama: `refactor/codex-skills-v10-8-sap-demo-validation`.
- Alcance: SAP Business One Service Layer → `NuanSystem.SyncWorker` → `NuanSystem_DEMO`.
- Estado: **Validada para el piloto controlado DEMO**.
- Dirección: `SapToErp`; no se envió información hacia SAP.
- Remigio, Cañaris, Matriz–Sucursal y SRI: fuera de alcance y sin procesos activos.

La corrida confirmó el recorrido real de Bodegas con un perfil, una entidad y una agenda temporales. La configuración operativa quedó nuevamente inactiva y todos los fixtures fueron retirados al finalizar.

## Seguridad y respaldos

Se crearon y verificaron mediante `RESTORE VERIFYONLY WITH CHECKSUM` los respaldos:

- `NuanSystem_Master_Phase108_20260803_123820.bak`.
- `NuanSystem_DEMO_Phase108_20260803_123820.bak`.

Las conexiones, `Security:EncryptionKey` y credenciales SAP se utilizaron únicamente desde configuración local ignorada y en memoria. SQL Server mantuvo cifrado obligatorio y validación del certificado. La evidencia no contiene conexiones, credenciales, cookies, claves, payloads de login ni snapshots funcionales completos.

## Configuración temporal

| Propiedad | Valor saneado |
|---|---|
| Empresa | `DEMO` |
| Entidad | `Warehouses` |
| Dirección | `SapToErp` |
| Modo | `Full` |
| `BatchSize` | 5 |
| Máximo de intentos | 3 |
| Continuar ante error por registro | Sí |
| Agenda | Intervalo temporal de un minuto |
| Concurrencia | Impedida por agenda y lease |
| Identificador de prueba | Prefijo `ITER108-WH-*` |

El perfil fue creado inactivo mediante los procedimientos oficiales, activado únicamente durante la ventana y desactivado antes de detener el worker.

## Lectura SAP y ejecuciones

| Evidencia | Resultado |
|---|---|
| Bodegas devueltas por SAP | 24 |
| Bodegas locales en DEMO | 35 antes y después |
| Bodegas con identidad SAP | 24 |
| Ejecuciones completadas | 3 |
| Registros por ejecución | 24 |
| Detalles totales | 72 |
| Resultado por detalle | 72 `Unchanged` / `NoChange` |
| Creaciones o actualizaciones | 0 |
| Aprobaciones, conflictos o errores | 0 |
| Snapshot aprobado | `WarehouseV1` |
| Hash de snapshot | 32 bytes en los 72 detalles |
| Locks finales | 0 |
| Eventos `LocalOutbox` de Warehouse | 0 |

El alcance exigía dos ciclos Full. Se observaron esos dos ciclos y un tercero antes de la desactivación, debido al intervalo temporal de un minuto. El ciclo adicional fue igualmente idempotente: no produjo mutaciones, conflictos, eventos downstream ni llamadas de escritura a SAP.

La lectura real empleó el contrato paginado del Service Layer. El historial conservó el `BatchSize=5`, la identidad del perfil, la dirección, los tiempos y los resultados saneados por registro durante la ventana de evidencia.

## Locks, heartbeat y cierre

- La agenda impidió ejecuciones simultáneas.
- El lease fue adquirido y liberado; no quedaron locks activos ni vencidos.
- El heartbeat utilizó una identidad temporal estable y reflejó la actividad del worker.
- Los dos heartbeats de la instancia temporal fueron eliminados durante la limpieza.
- No se alteraron los dos heartbeats SAP históricos anteriores a la prueba.
- El perfil, la entidad, la agenda, las tres cabeceras, los 72 detalles y las 81 auditorías temporales fueron eliminados de forma controlada.

## API, permisos y WinForms

| Gate | Resultado |
|---|---|
| API sin autenticación | HTTP 401 con empresa `DEMO` informada |
| Sesión `ADMIN` renovada | Acceso correcto a Perfiles SAP y Ejecuciones SAP |
| Formulario Perfiles SAP | Perfil temporal visible e inactivo al cierre |
| Ribbon del perfil | Consultar, activar, desactivar, ver ejecuciones, validar, actualizar, nuevo, editar, eliminar, columnas y filtro visibles |
| Formulario Ejecuciones SAP | Tres ejecuciones `Completed`, 24 correctos por ejecución |
| Detalle de ejecución | 24 registros, todos `Unchanged`, sin novedades |
| Acciones incompatibles con `Completed` | Reintentar, cancelar y liberar lock deshabilitadas |
| Layout | Sin recortes observados en la ventana normal de validación |

La autenticación fue realizada manualmente por el usuario; ninguna credencial fue automatizada ni registrada.

## Preservación y limpieza final

| Evidencia final | Resultado |
|---|---|
| Perfiles SAP | 1 perfil legado; 0 activos |
| Entidades SAP activas | 0 |
| Agendas SAP activas | 0 |
| Fixtures `ITER108` en Master | 0 |
| Fixtures `ITER108` en DEMO | 0 |
| Bodegas DEMO | 35 |
| Locks Warehouse | 0 |
| Eventos Warehouse nuevos | 0 |
| Procesos `NuanSystem.*` | 0 |
| Archivos temporales `phase108*` | 0 |

Remigio y Cañaris no fueron consultados ni modificados. No se inició `NuanSystem.MasterBranchSyncWorker` ni `NuanSystem.SriWorker` y no hubo llamadas SRI.

## Compilación y pruebas

| Validación | Resultado |
|---|---|
| Build completo | 0 errores, 0 advertencias |
| Suite completa | 752 aprobadas, 5 diagnósticas omitidas, 0 fallidas |
| Total | 757 pruebas |

Las cinco pruebas omitidas requieren infraestructura explícita y no representan fallos del producto.

## Conclusión

La Fase 10.8 queda validada para el piloto real SAP Business One → DEMO de Bodegas. El flujo leyó SAP, produjo historial y snapshots íntegros, respetó identidad, lotes, locks y permisos, y resultó idempotente. La configuración final permanece deshabilitada; esta evidencia no autoriza operación permanente, otros tenants ni escritura hacia SAP.
