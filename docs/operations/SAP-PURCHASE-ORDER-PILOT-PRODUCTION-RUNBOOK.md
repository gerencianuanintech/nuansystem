# Runbook de despliegue SAP Purchase Order Master-Sucursal

## Alcance

Despliegue controlado del flujo SAP Business One -> empresa Master -> sucursal para catalogos y ordenes de compra. El piloto aprobado usa `DEMO`, `DEMO-REMIGIO` y `DEMO-CANARIS`; reemplazar estos codigos solo mediante una configuracion formal de otra empresa.

Este runbook no activa stock, kardex, costos, recepciones, pagos, SRI ni escrituras desde sucursal hacia SAP.

## Artefactos de la version

- API REST NuanSystem.
- `NuanSystem.MasterBranchSyncWorker` como servicio separado.
- WinForms publicado con `NUANSYSTEM_API_URL` apuntando a HTTPS.
- Scripts Master y tenant hasta las versiones `20260718.105` y `20260718.103`, respectivamente.
- Perfil de sincronizacion revisado desde la consola administrativa.

## Secretos y variables

No crear `appsettings.Local.json` en el servidor. Configurar secretos en el entorno de la identidad del Application Pool o servicio Windows:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__SqlServerAdmin=<secreto>
Security__EncryptionKey=<secreto>
Jwt__SigningKey=<secreto de al menos 32 caracteres>
AllowedHosts=<host HTTPS de la API>
NUANSYSTEM_API_URL=https://<host-api>/
```

Las credenciales SAP permanecen cifradas por empresa en Master. No se copian a las bases de sucursal ni a WinForms.

## Preflight obligatorio

1. Congelar la version de binarios y scripts.
2. Ejecutar `dotnet test` y `dotnet build -c Release`.
3. Respaldar `NuanSystem_Master`, la base Master operativa y cada sucursal incluida.
4. Restaurar los respaldos en un ambiente de ensayo y ejecutar allí primero los scripts.
5. Ejecutar `database/sql/076_check_master_branch_sync_installation.sql` y la plantilla de validacion de este piloto.
6. Confirmar que solo las sucursales aprobadas estan activas en el perfil.
7. Confirmar bodega 20 -> REMIGIO y bodega 11 -> CANARIS.
8. Confirmar que eventos mixtos permanecen en `NeedsApproval`.
9. Confirmar HTTPS, certificado SQL confiable y firewall hacia SQL/SAP Service Layer.

No continuar si hay duplicados por `GlobalId`/`SapDocEntry`, locks vigentes sin responsable, errores de esquema o backups no restaurables.

## Publicacion inicial segura

1. Publicar API con `InitializeMasterOnStartup=false`.
2. Validar `GET /health/live` sin credenciales.
3. Validar `GET /health/ready` con JWT de operador; debe responder `Healthy` sin mostrar conexiones.
4. Publicar el Worker con:

```text
DOTNET_ENVIRONMENT=Production
MasterBranchSyncWorker__Enabled=true
MasterBranchSyncWorker__SkeletonMode=true
MasterBranchSyncWorker__SkeletonModeBehavior=ObserveOnly
```

5. Confirmar que ObserveOnly no reclama ni modifica `SyncOutbox`.

## Activacion por oleadas

Cada oleada requiere verificacion de `SyncOutbox`, targets, `SyncInbox`, duplicados y logs antes de avanzar.

1. Catalogos de referencia: `Countries`, `Provinces`, `Cities`, `Currencies`, `Tax`, `UnitOfMeasure`, `PriceList`.
2. Maestros: `BusinessPartner`, `ItemGroups`, `Item`.
3. Bodegas seleccionadas: `Warehouse` solamente con distribucion `Selected`.
4. Ordenes: agregar `PurchaseOrder` cuando todas sus dependencias esten `Applied`.

Para una oleada real configurar `SkeletonMode=false` y declarar solamente sus aplicadores en `EnabledEntityAppliers`. Reiniciar el servicio y observar primero un lote pequeno.

## Criterios de aborto

- Una orden aparece en mas de una sucursal.
- Una orden mixta se publica sin aprobacion.
- Faltan dependencias en la sucursal aprobada.
- Aparecen duplicados por `SapDocEntry`, `GlobalId` o `EventId`.
- El porcentaje de Error/DeadLetter del lote supera el umbral aprobado.
- Se detectan credenciales, tokens o connection strings en logs.
- Se pierde conectividad con Master o no se puede auditar el resultado.

## Rollback operativo

1. Detener `NuanSystem.MasterBranchSyncWorker`.
2. Restaurar `SkeletonMode=true`, `ObserveOnly` y vaciar `EnabledEntityAppliers`.
3. Deshabilitar el perfil o la matriz de la sucursal afectada; no borrar eventos.
4. Conservar `SyncOutbox`, targets, `SyncInbox` y auditoria.
5. No editar payloads, `GlobalId`, `SapDocEntry` ni estados con SQL manual.
6. Revertir binarios API/Worker al artefacto anterior si el defecto es de codigo.
7. Restaurar bases solamente ante dano confirmado, con aprobacion y punto de recuperacion documentado.

## Eventos historicos

Los DeadLetter anteriores al perfil definitivo se conservan como evidencia. Clasificarlos por empresa/target usando el monitor o la consulta de validacion. No reintentar eventos dirigidos a sucursales fuera del perfil actual; registrar la decision administrativa y mantenerlos fuera de los indicadores del nuevo lote mediante fecha/correlacion.

## Cierre

Registrar version de API, Worker, WinForms, scripts, backups, responsables, hora de activacion, conteos por estado, ordenes verificadas y cualquier rollback. El Worker solo queda habilitado después de la aceptación del lote por negocio y soporte.
