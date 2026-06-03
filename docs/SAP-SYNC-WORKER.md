# SAP Sync Worker

## Alcance fase 2

La fase 2 implementa sincronizacion funcional SAP -> ERP para proveedores mediante `NuanSystem.SyncWorker`.

Incluye:

- Lectura de proveedores desde SAP HANA por `ISapSupplierReader`.
- Importacion reutilizable por `ISapSupplierImportService`.
- Procesamiento automatico por `SapSupplierSyncHandler`.
- Cola tecnica `SapSyncInbox` para trazabilidad por proveedor SAP.
- Marca incremental por `SapSyncWatermark`.
- Bloqueo por empresa/entidad en `SapSyncLock`.
- Log tecnico en `SapSyncTechnicalLog`.
- Heartbeat del worker en `WorkerHeartbeat`.
- Reintento controlado para registros en estado retry.

No incluye todavia:

- Sincronizacion de articulos.
- Ordenes de compra.
- Envio ERP -> SAP.
- Cliente productivo Service Layer para documentos.

## Flujo proveedores SAP -> ERP

1. `SapSyncWorker` busca empresas activas con integracion SAP.
2. Por empresa resuelve el contexto tenant con `ICompanyResolver` e `ICompanyContext`.
3. Lee entidades activas desde `SapSyncEntitySettings`.
4. Ejecuta `ISapSyncOrchestrator`, que toma lock tecnico y llama al handler.
5. `SapSupplierSyncHandler` usa `ISapSupplierImportService`.
6. La importacion consulta proveedores cambiados desde el ultimo watermark.
7. Cada proveedor se registra/actualiza en `SapSyncInbox`.
8. Se crea o actualiza el proveedor local mediante `IBusinessPartnerRepository`.
9. Al terminar sin fallos se actualiza `SapSyncWatermark`.

## Reglas de matching

El importador reutiliza las mismas reglas para endpoint manual y Worker:

- Primero intenta match por `SapCardCode`.
- Luego por codigo local igual al `CardCode` de SAP.
- Si encuentra identificacion tributaria usada por otro proveedor, marca conflicto.
- Si no existe proveedor local, crea uno nuevo.

Estados funcionales por registro:

- `Created`
- `Updated`
- `Unchanged`
- `Conflict`
- `Failed`
- `Skipped`

## Tablas tecnicas

Scripts agregados:

- `database/sql/049_master_sap_sync_worker.sql`
- `database/sql/050_tenant_sap_sync_worker.sql`

Tablas principales:

- `SapSyncEntitySettings`
- `WorkerHeartbeat`
- `SapSyncWatermark`
- `SapSyncInbox`
- `SapSyncOutbox`
- `SapSyncLock`
- `SapSyncConflict`
- `SapSyncTechnicalLog`

`SapSyncLog` se conserva para los endpoints y logs publicos existentes.

## Incremental SAP

La lectura incremental de proveedores usa `CreateDate` y `UpdateDate` de `OCRD`.

Limitacion conocida: SAP HANA expone fechas sin hora en algunos ambientes. Por eso el filtro usa la fecha UTC convertida a dia, lo que puede releer registros del mismo dia. La importacion es idempotente por `CardCode`, por lo que el reprocesamiento no duplica proveedores.

## Ejecucion local

Configurar `appsettings.Local.json` o variables de entorno:

- `ConnectionStrings:SqlServerAdmin`
- `Security:EncryptionKey`
- `MasterDatabase:DatabaseName`
- Configuracion SAP por empresa en master.

Comando:

```powershell
dotnet run --project src\Backend\NuanSystem.SyncWorker\NuanSystem.SyncWorker.csproj
```

Endpoints manuales existentes:

- `GET /api/sap/suppliers/preview`
- `POST /api/sap/suppliers/import`

Ambos siguen funcionando y ahora comparten `ISapSupplierImportService`.

## Siguiente fase recomendada

La fase 3 deberia implementar una de estas rutas:

- SAP -> ERP articulos con unidades, grupos, familias y reglas de stock.
- SAP -> ERP ordenes de compra.
- ERP -> SAP documentos mediante Service Layer productivo e idempotente.
