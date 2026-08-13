# Revision tecnica NuanSystem

Fecha de revision: 2026-06-02  
Alcance autorizado: fase 1, analisis tecnico y documentacion.  
Restriccion aplicada: no se modifico codigo fuente, `Program.cs`, archivos `.csproj`, endpoints, formularios WinForms ni scripts SQL.

## 1. Resumen ejecutivo

NuanSystem muestra una arquitectura empresarial en evolucion con una separacion clara entre `Api`, `Application`, `Domain`, `Persistence`, `Infrastructure`, `SapIntegration`, `Shared` y frontend WinForms. El proyecto ya incorpora .NET 9, Minimal APIs, MediatR/CQRS, FluentValidation, Dapper, SQL Server, JWT, seguridad por permisos/formularios, multiempresa por `X-Company-Code`, auditoria y un frontend WinForms separado en Forms, Services y ViewModels.

El estado general es positivo para un ERP modular en construccion. La madurez estimada es media-alta para arquitectura base y media para procesos operativos criticos. Las bases existen, pero todavia hay deuda en modularizacion de endpoints, sincronizacion SAP asincrona, pruebas automatizadas, consistencia fina de permisos y algunos formularios WinForms muy grandes.

Riesgos principales:

- `Program.cs` todavia concentra endpoints de autenticacion, empresas, configuracion, seguridad, SAP, settings y auditoria.
- Existe SQL directo en `Program.cs` para cambio de clave, fuera de `Application`/`Persistence`.
- Las referencias DevExpress combinan `HintPath` local a `C:\Program Files\DevExpress 25.2...` con `PackageReference` en WinForms Forms.
- Orden de Compra marca sincronizacion SAP como pendiente, pero no tiene cola/worker productivo completo para envio, reintento e idempotencia.
- No se detecto carpeta/proyectos `tests`.
- Maestro de Items es funcionalmente amplio, pero parte del perfil maestro se persiste como JSON versionado, lo que requiere gobernanza para reportes, busquedas y auditoria granular.

Prioridad recomendada:

1. Documentar y modularizar endpoints sin cambiar rutas.
2. Crear plan de pruebas minimo antes de refactors de comportamiento.
3. Normalizar estrategia SAP: solicitud local, cola, worker, reintentos, trazabilidad e idempotencia.
4. Revisar permisos por formulario contra `PermissionCodes` y scripts seed.
5. Revisar DevExpress para portabilidad de equipo/CI sin romper compilacion local.

## 2. Hallazgos criticos

### HC-01: SQL directo en `Program.cs`

`Program.cs` contiene logica de cambio de clave con comandos SQL directos (`SELECT PasswordHash...`, `UPDATE dbo.Users...`). Esto rompe parcialmente la regla de endpoints delgados y acceso a datos dentro de Persistence.

Impacto:

- Dificulta pruebas unitarias de autenticacion/cambio de clave.
- Mezcla infraestructura SQL en la capa Api.
- Duplica responsabilidad frente a `IAuthService`, `IPasswordHasher` y repositorios de seguridad.

Recomendacion:

- Mover el flujo a `Application.Features.Auth.Commands.ChangePasswordCommand`.
- Implementar acceso en `Persistence` mediante repositorio/servicio de seguridad.
- Mantener ruta `/api/auth/change-password` intacta.

No corregir todavia en fase 1.

### HC-02: Sincronizacion SAP de Orden de Compra no ejecuta envio productivo

El endpoint `/api/purchase-orders/{id:int}/sync-sap` invoca `SyncPurchaseOrderSapCommandHandler`, valida estado y registra log `Pending`, pero no usa `ISapDocumentSender`, no escribe `DocEntry`/`DocNum` y no hay cola/worker para reintentos.

Impacto:

- Puede dar al usuario la impresion de "sincronizar" cuando realmente solo marca pendiente.
- Riesgo de duplicidad si luego se implementa envio directo sin idempotencia.
- Trazabilidad incompleta para intentos, payload, respuesta, errores tecnicos y usuario solicitante.

Recomendacion:

- Separar "solicitar sincronizacion" de "enviar a SAP".
- Agregar cola de sincronizacion con estados `Pending`, `Processing`, `Success`, `Failed`, `RetryPending`.
- Mantener endpoint actual como solicitud de sincronizacion inicialmente.

No implementar worker en esta fase.

### HC-03: No se detectan proyectos de prueba

No se encontro carpeta `tests` ni proyectos `*.Tests`.

Impacto:

- Alto riesgo al refactorizar endpoints, validadores, permisos y SAP.
- No hay red de seguridad para Orden de Compra, login, empresa activa ni permisos.

Recomendacion:

- Crear plan y luego proyectos `tests/NuanSystem.Application.Tests`, `tests/NuanSystem.Persistence.Tests`, `tests/NuanSystem.Api.Tests`.
- Priorizar validadores y flujos de Orden de Compra.

## 3. Hallazgos importantes

### HI-01: `Program.cs` sigue siendo demasiado grande

Ya existen endpoints modularizados:

- `AccountingEndpoints.cs`
- `BusinessPartnerEndpoints.cs`
- `FinancialCatalogEndpoints.cs`
- `GeneralSupplierEndpoints.cs`
- `GeographyEndpoints.cs`
- `InventoryCatalogEndpoints.cs`
- `PurchaseOrderEndpoints.cs`
- `TaxCatalogEndpoints.cs`

Pero `Program.cs` mantiene endpoints de:

- Auth.
- Companies.
- Configuration companies.
- Configuration settings.
- Tenancy.
- SAP sync logs/supplier import.
- Settings.
- Users.
- Roles.
- Security operations.
- Security menus.
- Security forms.
- Security fields.
- Security access.
- Grid columns.
- Audit.

Recomendacion:

- Moverlos a extensiones `MapXEndpoints`.
- Mantener todas las rutas, permisos, request/response y comportamiento.

### HI-02: DevExpress local y NuGet conviven

Los proyectos WinForms usan referencias absolutas:

- `C:\Program Files\DevExpress 25.2\Components\Bin\NetCore\...`
- Analyzer local de DevExpress.

Y `NuanSystem.WinForms.Forms.csproj` tambien tiene `PackageReference Include="DevExpress.Win" Version="25.2.6"`.

Impacto:

- Riesgo para otros equipos, CI/CD y maquinas sin instalacion local exacta.
- Posible duplicidad entre assemblies de `HintPath` y paquete.

Recomendacion:

- No eliminar referencias sin probar compilacion.
- Definir estrategia: feed NuGet privado/offline de DevExpress o propiedad centralizada `$(DevExpressRoot)`.
- Evaluar `Directory.Build.props` para version/ruta, con fallback local documentado.

### HI-03: Permisos especificos no siempre se usan como autorizacion directa

Existen permisos especificos:

- `PURCHASING.PURCHASEORDERS.READ`
- `PURCHASING.PURCHASEORDERS.MANAGE`
- `PURCHASING.PURCHASEORDERS.APPROVE`
- `PURCHASING.PURCHASEORDERS.SYNC_SAP`
- permisos granulares de Items como `CATALOG.ITEMS.CREATE`, `CONFIGURE_SAP`, `MANAGE_ATTACHMENTS`.

Pero Orden de Compra se protege por `RequireFormOperation("purchase-orders", "...")`. Items usa `PermissionCodes.ItemsRead/ItemsManage` en endpoints, aunque hay permisos granulares definidos.

Impacto:

- La autorizacion por formulario es flexible, pero puede quedar desalineada con `PermissionCodes`.
- Dificulta auditoria de permisos efectivos por accion.

Recomendacion:

- Mantener compatibilidad actual.
- Documentar una ruta canonica: operaciones de UI via `SecurityRoleFormOperations`; permisos constantes para API global/capacidades especiales.
- Mapear acciones especiales (`approve`, `syncsap`) contra permisos equivalentes.

### HI-04: Orden de Compra reemplaza hijos completos en update

`SP_NA_INTERNAL_PURCHASEORDERS_REPLACE_CHILDREN` elimina y reinserta lineas, direcciones, documentos relacionados y anexos.

Impacto:

- Simple para persistir maestro-detalle, pero reduce trazabilidad granular.
- Riesgo de perdida de metadata en anexos/documentos relacionados si no se envia todo el estado completo.
- Dificulta concurrencia y auditoria por linea.

Recomendacion:

- Mantener por ahora.
- Para produccion, agregar control de version/concurrencia y endpoints especificos para colecciones criticas si el flujo operativo lo requiere.

## 4. Hallazgos menores

- En `ItemEditForm` se detecta al menos un control con nombre generico (`lookUpEdit1`), lo que afecta mantenibilidad.
- Algunos scripts SQL legacy redefinen procedimientos previamente definidos en scripts posteriores, por ejemplo areas de Items y BusinessPartners. Requiere orden de ejecucion muy claro.
- `database/sql` sigue siendo carpeta legacy; el estandar recomendado del skill indica migrar progresivamente a `database/sqlserver` cuando se haga una normalizacion.
- `GeographyEndpoints` usa rutas cortas como `/countries`, `/provinces`, `/cities`, mientras otros modulos usan prefijo `/api/...`. Si son rutas existentes, deben conservarse y documentarse como legacy/canonicas.
- `Program.cs` tiene endpoints duplicados funcionalmente como `/api/users` y `/api/security/users`, y `/api/roles` y `/api/security/roles`. Esto puede ser valido por compatibilidad, pero debe quedar documentado.

## 5. Arquitectura actual

Capas detectadas:

- `NuanSystem.Api`: Minimal APIs, middleware, filtros, Swagger, composicion.
- `NuanSystem.Application`: commands, queries, handlers, validators, DTOs, contratos.
- `NuanSystem.Domain`: modelos puros de dominio/tenancy/security/catalogos.
- `NuanSystem.Infrastructure`: JWT, hashing, cifrado y servicios tecnicos.
- `NuanSystem.Persistence`: Dapper, SQL Server, connection factories, repositorios.
- `NuanSystem.SapIntegration`: Service Layer, DI API placeholder, HANA supplier reader, document payloads.
- `NuanSystem.Shared`: respuestas, constantes, contratos compartidos.
- `NuanSystem.WinForms`: aplicacion ejecutable.
- `NuanSystem.WinForms.Services`: clientes HTTP.
- `NuanSystem.WinForms.ViewModels`: estado de presentacion.
- `NuanSystem.WinForms.Forms`: UI DevExpress.
- `NuanSystem.WinForms.Controls`: controles reutilizables.

Cumplimiento general:

- Application depende de Domain y Shared.
- Persistence depende de Application/Domain/Shared.
- Infrastructure depende de Application/Domain/Shared.
- SapIntegration depende de Application/Shared.
- Api compone Application, Infrastructure, Persistence, SapIntegration y Shared.
- Frontend consume Shared y Services/ViewModels, y usa `NuanApiClient`.

Posibles violaciones:

- SQL directo en `Program.cs`.
- Endpoints con logica operativa simple pero no totalmente delgada.
- `Domain.Tenancy.SapIntegrationMode` contiene concepto SAP. No es una referencia a `SapIntegration`, pero conviene revisar si el modo SAP pertenece a Domain o a configuracion de infraestructura/tenancy.

## 6. Dependencias entre proyectos

Backend:

- `NuanSystem.Api` referencia `Application`, `Infrastructure`, `Persistence`, `SapIntegration`, `Shared`.
- `NuanSystem.Application` referencia `Domain`, `Shared`.
- `NuanSystem.Persistence` referencia `Application`, `Domain`, `Shared`.
- `NuanSystem.Infrastructure` referencia `Application`, `Domain`, `Shared`.
- `NuanSystem.SapIntegration` referencia `Application`, `Shared`.
- `NuanSystem.Domain` no referencia otros proyectos de infraestructura.

Frontend:

- `NuanSystem.WinForms` referencia `Services`, `ViewModels`, `Forms`, `Controls`, `Shared`.
- `NuanSystem.WinForms.Forms` referencia `ViewModels`, `Controls`, `Shared`.
- `NuanSystem.WinForms.ViewModels` referencia `Services`, `Shared`.
- `NuanSystem.WinForms.Services` referencia `Shared`.
- `NuanSystem.WinForms.Controls` referencia `Shared`.

Evaluacion:

- La direccion de dependencias es mayormente correcta.
- El frontend no muestra referencias directas a Persistence ni SQL Server.
- No se detecto uso directo de `SqlConnection` o `Sap.Data` en WinForms.
- `NuanSystem.WinForms.Program.cs` crea `HttpClient` durante composicion, aceptable si se mantiene como setup central y no dentro de formularios.

## 7. API y endpoints

Endpoints en `Program.cs`:

- `/`
- `/api/auth/login`
- `/api/auth/change-password`
- `/api/companies/my-companies`
- `/api/companies`
- `/api/companies/validate-connection`
- `/api/companies/assign-user`
- `/api/configuration/companies`
- `/api/configuration/settings`
- `/api/tenancy/current`
- `/api/tenancy/initialize-database`
- `/api/sap/sync-logs`
- `/api/sap/suppliers/preview`
- `/api/sap/suppliers/import`
- `/api/settings/parameters`
- `/api/users`
- `/api/security/users`
- `/api/roles`
- `/api/security/roles`
- `/api/security/operations`
- `/api/security/menus`
- `/api/security/forms`
- `/api/security/fields`
- `/api/security/navigation/me`
- `/api/security/forms/{formKey}/operations/me`
- `/api/security/roles/{roleId:int}/access`
- `/api/security/grid-columns/{formKey}/{gridName}/me`
- `/api/audit/logs`
- `/api/audit/security-changes`
- `/api/audit/inventory-changes`
- `/api/audit/error-logs`

Endpoints modularizados:

- Accounting: `/api/accounting/chart-of-accounts`.
- Inventory: `/api/items`, `/api/definitions/inventory/item-groups`, `/api/definitions/inventory/item-families`, catalogos auxiliares.
- Geography: `/countries`, `/provinces`, `/cities`, `/reverse-geocode`, `/static-map`.
- Financial catalogs: rutas por catalogo financiero.
- General supplier: rutas por catalogo proveedor general.
- Tax catalogs: rutas por catalogos tributarios.
- Business partners: `/api/commercial/business-partners`, `/customers`, `/suppliers`.
- Purchase orders: `/api/purchase-orders`.

Duplicidades o rutas paralelas:

- `/api/users` y `/api/security/users`.
- `/api/users/roles` y `/api/security/users/roles`.
- `/api/roles` y `/api/security/roles`.
- Business partners tiene rutas genericas y especializadas para customers/suppliers.

No se detectaron rutas exactamente duplicadas por metodo en la extraccion automatica.

Convencion recomendada:

- Mantener rutas existentes.
- Definir rutas canonicas documentadas:
  - Seguridad: `/api/security/...`.
  - Usuarios legacy: `/api/users` como compatibilidad.
  - Roles legacy: `/api/roles` como compatibilidad.
  - Geography: evaluar prefijo `/api/geography/...` futuro, manteniendo rutas cortas como legacy.
- Cada archivo endpoint en namespace `NuanSystem.Api.Endpoints`.
- Cada clase static con `public static IEndpointRouteBuilder MapXEndpoints(this IEndpointRouteBuilder app)`.

## 8. Multiempresa

Arquitectura detectada:

- Login retorna empresas autorizadas.
- Frontend envia `X-Company-Code` desde `NuanApiClient`.
- Middleware `UseCompanyContext` resuelve empresa activa.
- Persistence usa `ITenantConnectionFactory` y `IMasterConnectionFactory`.
- `CompanyContext` existe como servicio scoped.

Fortalezas:

- La empresa activa no se resuelve desde formularios.
- Los clientes HTTP centralizan token y company code.
- Repositorios tenant usan `ITenantConnectionFactory`.

Riesgos:

- Endpoints globales y tenant deben distinguir claramente si requieren `X-Company-Code`.
- `/api/tenancy/current` y `/api/tenancy/initialize-database` usan permisos no especificos (`BusinessPartnersRead`), lo que podria ser confuso.

Recomendacion:

- Crear matriz de endpoints company-scoped vs master-scoped.
- Usar permisos especificos para tenancy/configuracion cuando existan.
- Agregar pruebas de resolucion de empresa activa y acceso cruzado.

## 9. Seguridad

Elementos detectados:

- JWT con validacion de issuer, audience, lifetime, signing key.
- `SecurityStamp` validado en `OnTokenValidated`.
- Rate limiting para login.
- `PermissionCodes.All` crea policies de autorizacion.
- `RequirePermission` usa policies por permiso.
- `RequireFormOperation` consulta operaciones permitidas por usuario/formulario.
- Bypass administrativo por `SECURITY.ACCESS.BYPASS`.
- `RequiredPasswordChangeMiddleware`.
- Auditoria de logs y errores.

Riesgos:

- Los permisos especificos de Items/PurchaseOrders no siempre estan alineados con `RequireFormOperation`.
- Cambio de clave en Api tiene SQL directo.
- Swagger esta habilitado solo en Development, correcto, pero se debe confirmar configuracion de ambiente en produccion.

Recomendaciones:

- Mantener autorizacion backend como fuente de verdad.
- Documentar equivalencia entre operaciones UI y permisos constantes.
- Refactorizar cambio de clave a Application/Persistence.
- Agregar tests para login invalido, security stamp y permisos.

## 10. Persistencia

Repositorios detectados:

- `BusinessPartnerRepository`
- `FinancialCatalogRepository`
- `GeographyRepository`
- `GeneralInventoryCatalogRepository`
- `GeneralSupplierCatalogRepository`
- `PurchaseOrderRepository`
- `TaxCatalogRepository`
- `CompanyAdminRepository`
- `ConfigurationCompanyRepository`
- `ItemRepository`
- `ItemGroupRepository`
- `ItemFamilyRepository`
- `ChartOfAccountRepository`
- `SapCompanySettingsRepository`
- `SapSyncLogRepository`
- `CompanyParameterRepository`
- `ConfigurationSettingRepository`
- `UserAdminRepository`
- `RoleAdminRepository`
- repositorios de seguridad, grid columns, auditoria e inventario

Uso de Dapper:

- Se usa `CommandDefinition` con `CommandType.StoredProcedure`.
- Se usan parametros anonimos y JSON para colecciones.
- Se usan factories master/tenant.

Transacciones:

- En SQL existen transacciones para Items, BusinessPartners, seguridad y perfil maestro.
- `ITransactionRunner` esta registrado, pero no se observo uso en Orden de Compra; el control transaccional principal vive en SPs.

Riesgos:

- SQL dinamico existe en scripts para generar familias de procedimientos/catalogos. Es aceptable en scripts de provisionamiento si los tokens estan controlados, pero requiere revision de entradas.
- Reemplazo completo de hijos con JSON puede complicar auditoria y concurrencia.
- Scripts legacy redefinen procedimientos, por lo que el orden de ejecucion es critico.

Recomendacion:

- Mantener SPs para CRUD segun estandar.
- Documentar orden de scripts y versionado.
- Para operaciones criticas, agregar control de concurrencia y transacciones explicitas por use case.

## 11. SAP Integration

Servicios SAP encontrados:

- `ISapDocumentSender`
- `ISapSupplierReader`
- `ISapSyncLogRepository`
- `ISapCompanySettingsRepository`
- `SapServiceLayerClient`
- `SapDiApiClient`
- `SapClientFactory`
- `SapHanaConnectionFactory`
- `SapHanaQueryClient`
- `SapSupplierReader`

Como se sincronizan documentos:

- Existe `SendDocumentToSapCommandHandler` generico en `Application.Features.SapSync`.
- Orden de Compra no usa todavia ese sender; marca estado `SapPending` y registra log local.
- Proveedores SAP tienen preview/import desde HANA/BusinessPartners con log.

Evaluacion:

- SAP esta aislado en proyecto dedicado.
- Application usa abstracciones.
- Domain no referencia `NuanSystem.SapIntegration`.
- DI API esta como placeholder, no implementado.

Riesgos:

- Timeout si se envia directo desde request HTTP.
- Duplicidad si no hay idempotency key por documento/operacion.
- Falta de estados de cola productivos.
- Falta de payload auditado y respuesta SAP completa para Orden de Compra.
- Falta de reintentos controlados.

Modelo recomendado para produccion:

- Tabla `SapSyncQueue` o evolucion de `SapSyncLog`.
- Campos: `Id`, `CompanyId`, `EntityName`, `EntityId`, `Operation`, `Status`, `RequestJson`, `ResponseJson`, `ErrorMessage`, `SapDocEntry`, `SapDocNum`, `RetryCount`, `MaxRetryCount`, `LastAttemptAt`, `NextRetryAt`, `RequestedByUserId`, `RequestedByUserName`, `TraceId`, `CreatedAt`, `UpdatedAt`.
- Estados: `Pending`, `Processing`, `Success`, `Failed`, `RetryPending`, `Cancelled`.
- Worker futuro separado de API.
- Endpoint solo solicita sincronizacion.
- El worker valida estado local antes de enviar.
- Guardar `DocEntry`, `DocNum`, fecha y usuario solo tras confirmacion SAP.
- No reenviar documentos ya sincronizados salvo retry forzado auditado.

## 12. WinForms DevExpress

Formularios principales detectados:

- Seguridad: usuarios, roles, operaciones, menus, formularios, campos, accesos.
- Configuracion: empresas, parametros.
- Catalogos: contabilidad, geografica, financieros, tributarios, inventario general, proveedor general.
- BusinessPartners: clientes/proveedores.
- InventoryItems: lista, edicion, dialogos de bodegas, codigos de barra, presentaciones, anexos, SAP mappings, alertas.
- Purchasing/PurchaseOrders: lista y edicion.
- Audit: logs e historial.

Uso de Services/ViewModels:

- `NuanApiClient` centraliza token JWT y `X-Company-Code`.
- PurchaseOrders usa `PurchaseOrderClient` y `PurchaseOrdersViewModel`.
- No se detectaron conexiones SQL directas ni SAP directas en WinForms.

Dependencias locales DevExpress:

- `NuanSystem.WinForms.csproj` y `NuanSystem.WinForms.Forms.csproj` usan `HintPath` absoluto a `C:\Program Files\DevExpress 25.2...`.
- `NuanSystem.WinForms.Forms.csproj` incluye ademas `PackageReference DevExpress.Win 25.2.6`.

Riesgos UX/mantenibilidad:

- `ItemEditForm.Designer.cs` es muy grande.
- Nombres genericos como `lookUpEdit1`.
- Muchas secciones en una sola pantalla pueden afectar mantenimiento, aunque el diseño ERP lo justifica parcialmente.

Recomendaciones:

- No eliminar referencias DevExpress sin prueba de compilacion.
- Centralizar version/ruta DevExpress.
- Mantener `NuanApiClient`.
- Revisar `ItemEditForm` por nombres tecnicos y dividir comportamiento en metodos/servicios/ViewModel sin tocar designer en fase inicial.

## 13. Orden de Compra

Endpoints existentes:

- `GET /api/purchase-orders`
- `GET /api/purchase-orders/lookups`
- `GET /api/purchase-orders/{id:int}`
- `POST /api/purchase-orders`
- `POST /api/purchase-orders/{id:int}/save`
- `PUT /api/purchase-orders/{id:int}`
- `DELETE /api/purchase-orders/{id:int}`
- `POST /api/purchase-orders/{id:int}/send-to-approval`
- `POST /api/purchase-orders/{id:int}/approve`
- `POST /api/purchase-orders/{id:int}/reject`
- `POST /api/purchase-orders/{id:int}/sync-sap`
- `GET /api/purchase-orders/{id:int}/sap-status`
- `GET/POST/DELETE related-documents`
- `GET/POST/DELETE attachments`

Estados detectados:

| Estado | Existe | Comentario |
|---|---:|---|
| Draft | Si | Estado inicial al crear. |
| PendingApproval | Si | Envio a aprobacion. |
| Approved | Si | Aprobacion. |
| Rejected | Si | Rechazo. |
| SapPending | Si | Marcado pendiente SAP. |
| SapSynced | Si | Previsto, pero no se actualiza desde envio real en OC actual. |
| SapError | Si | Previsto. |
| Closed | Si | Bloquea modificaciones. |
| Cancelled | Si | Usado en delete/anulacion logica. |

Matriz recomendada de estados:

| Accion | Draft | PendingApproval | Approved | Rejected | SapPending | SapSynced | SapError | Cancelled |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Editar | Si | Revisar | Revisar | Si | No | No | Revisar | No |
| Eliminar/anular | Si | No | No | Si | No | No | Revisar | No |
| Enviar aprobacion | Si | No | No | Revisar | No | No | No | No |
| Aprobar | No | Si | No | No | No | No | No | No |
| Rechazar | No | Si | No | No | No | No | No | No |
| Solicitar SAP | No | No | Si | No | Si | No | Retry controlado | No |

Matriz de permisos por accion:

| Accion | Operacion actual | Permiso recomendado |
|---|---|---|
| Listar/refrescar | `refresh` | `PURCHASING.PURCHASEORDERS.READ` |
| Consultar | `consult` | `PURCHASING.PURCHASEORDERS.READ` |
| Crear | `create` | `PURCHASING.PURCHASEORDERS.MANAGE` |
| Guardar/editar | `update` | `PURCHASING.PURCHASEORDERS.MANAGE` |
| Eliminar/anular | `delete` | `PURCHASING.PURCHASEORDERS.MANAGE` |
| Enviar a aprobacion | `approve` | `PURCHASING.PURCHASEORDERS.APPROVE` |
| Aprobar/rechazar | `approve` | `PURCHASING.PURCHASEORDERS.APPROVE` |
| Solicitar SAP | `syncsap` | `PURCHASING.PURCHASEORDERS.SYNC_SAP` |

Validaciones existentes:

- Proveedor obligatorio.
- Serie y numero obligatorios.
- Fechas obligatorias.
- Moneda obligatoria.
- Condicion de pago, comprador y bodega principal obligatorios.
- Descuento 0-100.
- Al menos una linea.
- Item, cantidad, precio, unidad, impuesto, bodega y fecha por linea.
- Direccion delivery y billing.

Validaciones faltantes/recomendadas:

- Validar transiciones de estado permitidas, no solo asignar estado.
- No aprobar si no esta `PendingApproval`.
- No rechazar si no esta `PendingApproval`.
- No enviar a aprobacion si no esta completo y `Draft`.
- No sincronizar si hay lineas invalidas, sin proveedor SAP requerido o sin configuracion SAP activa.
- Control de concurrencia/version.
- Idempotencia de SAP por `PurchaseOrderId` + `Operation`.

Riesgos funcionales:

- `SendPurchaseOrderToApproval`, `Approve`, `Reject` usan cambio de estado generico sin matriz explicita.
- `SyncPurchaseOrderSap` no envia a SAP; solo marca pendiente.
- `PurchaseOrderApprovals` existe, pero los handlers actuales no parecen crear/respondar registros de aprobacion por nivel.
- Campos `SapDocEntry`, `SapDocNum`, `SapSyncDate`, `SapMessage` existen, pero no se completan desde el flujo actual.

Flujo final recomendado:

1. Crear en SQL Server como `Draft`.
2. Guardar borrador.
3. Validar completitud.
4. Si requiere autorizacion, crear solicitud en `PurchaseOrderApprovals` y pasar a `PendingApproval`.
5. Aprobar/rechazar solo desde `PendingApproval`.
6. Si `Approved`, permitir solicitar sincronizacion SAP.
7. Crear registro `SapSyncQueue Pending`.
8. Worker procesa, envia a SAP, guarda `DocEntry`, `DocNum`, fecha, respuesta y estado.
9. Evitar reenvio salvo retry controlado y auditado.

## 14. Maestro de Items

Formularios encontrados:

- `ItemsForm`
- `ItemEditForm`
- `ItemWarehouseEditDialog`
- `ItemSapFieldMappingEditDialog`
- `ItemPresentationEditDialog`
- `ItemOperationalAlertEditDialog`
- `ItemBarcodeEditDialog`
- `ItemBarcodesDialog`
- `ItemAttachmentEditDialog`

Secciones detectadas en DTO maestro:

- Datos generales.
- Unidades.
- Inventario.
- Compra.
- Venta.
- Costos.
- Contabilidad.
- Impuestos.
- Trazabilidad.
- Variantes.
- SAP.
- Anexos.
- Observaciones/alertas.

Campos criticos presentes o considerados:

- Codigo, nombre, descripcion.
- Grupo/familia.
- Tipo de articulo.
- Unidades de inventario, compra y venta.
- Factores de compra/venta.
- Manejo inventario, lotes, series, perecibles, vencimiento.
- Bodegas y ubicaciones.
- Impuestos compra/venta.
- Costos y precios.
- Presentaciones.
- Codigos de barra.
- Anexos.
- SAP field mappings.

Riesgos/dudas:

- El perfil maestro completo se guarda en `ItemMasterProfiles.MasterDataJson`; esto da flexibilidad, pero puede dificultar consultas SQL, reporting, BI e integridad referencial de campos internos.
- Algunos catalogos parecen mezclarse entre tablas base y JSON del perfil.
- El formulario es muy grande; requiere disciplina de nombres y responsabilidades.
- No se debe exponer auditoria como campos editables.

Diccionario recomendado de campos:

| Campo tecnico | Caption visible | Control DevExpress | Pestana | Obligatorio | Editable | Origen lookup | Persistencia backend | Comentario funcional | SAP |
|---|---|---|---|---|---|---|---|---|---|
| Code | Codigo | TextEdit | General | Si | Si | N/A | Items.Code | Codigo unico por tenant. | Mapear a ItemCode si aplica. |
| Name | Nombre | TextEdit | General | Si | Si | N/A | Items.Name | Nombre comercial/base. | ItemName. |
| Description | Descripcion | MemoEdit/TextEdit | General | No | Si | N/A | Items.Description | Descripcion corta. | User field si aplica. |
| ItemGroupId | Grupo | LookUpEdit | General | Recomendado | Si | ItemGroups | Items.ItemGroupId | Catalogo administrable. | ItemGroups. |
| ItemFamilyId | Familia/linea | LookUpEdit | General | Recomendado | Si | ItemFamilies | Items/extension | Debe depender de grupo si aplica. | UDF o grupo alterno. |
| ItemType | Tipo | LookUpEdit | General | Si | Si | Fijo/catalogo | Items.ItemType | Product/Service/Supply/Asset. | ItemType. |
| InventoryUnitOfMeasureId | Unidad inventario | LookUpEdit | Unidades | Si si inventario | Si | UnitMeasures | Items.InventoryUnitOfMeasureId | Unidad base de stock. | InventoryUoM. |
| PurchaseUnitOfMeasureId | Unidad compra | LookUpEdit | Compra/Unidades | Si si compra | Si | UnitMeasures | Items.PurchaseUnitOfMeasureId | Factor compra. | PurchaseUnit. |
| SalesUnitOfMeasureId | Unidad venta | LookUpEdit | Venta/Unidades | Si si venta | Si | UnitMeasures | Items.SalesUnitOfMeasureId | Factor venta. | SalesUnit. |
| PurchaseFactor | Factor compra | SpinEdit | Unidades | Si | Si | N/A | Items.PurchaseFactor | Mayor que cero. | UoM conversion. |
| SalesFactor | Factor venta | SpinEdit | Unidades | Si | Si | N/A | Items.SalesFactor | Mayor que cero. | UoM conversion. |
| IsPurchaseItem | Activo compra | CheckEdit | Compra | Si | Si | N/A | Items.IsPurchaseItem | Habilita compras. | PurchaseItem flag. |
| IsSalesItem | Activo venta | CheckEdit | Venta | Si | Si | N/A | Items.IsSalesItem | Habilita ventas. | SalesItem flag. |
| IsInventoryItem | Maneja inventario | CheckEdit | Inventario | Si | Si | N/A | Items.IsInventoryItem | Condiciona stock/lotes. | InventoryItem flag. |
| PurchaseTaxId | Impuesto compra | LookUpEdit | Impuestos/Compra | Si si compra | Si | Taxes | Items.PurchaseTaxId | IVA compra. | TaxCode AP. |
| SalesTaxId | Impuesto venta | LookUpEdit | Impuestos/Venta | Si si venta | Si | Taxes | Items.SalesTaxId | IVA venta. | TaxCode AR. |
| ValuationMethod | Metodo valoracion | LookUpEdit | Inventario/Costos | Si | Si controlado | Fijo | Items.ValuationMethod | MovingAverage/Standard/FIFO/SerialBatch. | ValuationMethod. |
| ManagedBy | Manejado por | LookUpEdit/Toggle | Trazabilidad | Si | Si | Fijo | Items.ManagedBy | None/Batch/Serial. | ManageBatch/Serial. |
| BatchSerialManagementMethod | Metodo lote/serie | LookUpEdit | Trazabilidad | Si | Si | Fijo | Items.BatchSerialManagementMethod | EveryTransaction/IssueOnly. | Gestion SAP equivalente. |
| BaseSalesPrice | Precio base | SpinEdit | Venta/Costos | Si | Si | N/A | Items.BaseSalesPrice | Precio referencial. | PriceList. |
| ReferenceCost | Costo referencia | SpinEdit | Costos | Si | Segun permiso | N/A | Items.ReferenceCost | Costo base. | AvgPrice/StdCost. |
| PreferredVendorCode | Proveedor preferido | SearchLookUpEdit | Compra | No | Si | Suppliers | Items.PreferredVendorCode | Compra. | PreferredVendor. |
| VendorCatalogCode | Codigo proveedor | TextEdit | Compra | No | Si | N/A | Items.VendorCatalogCode | SKU proveedor. | SupplierCatalogNo. |
| AllowDiscount | Permite descuento | CheckEdit | Venta | Si | Si | N/A | Items.AllowDiscount | Politica comercial. | UDF si aplica. |
| AllowSaleWithoutStock | Venta sin stock | CheckEdit | Inventario/Venta | Si | Segun capacidad | N/A | Items.AllowSaleWithoutStock | Depende de giro. | No directo. |
| Barcodes | Codigos de barra | GridControl | Unidades | No | Si | UnitMeasures | ItemBarcodes + JSON perfil | Un principal activo maximo. | BarCodes. |
| Warehouses | Bodegas | GridControl | Inventario | No | Si | Warehouses | ItemWarehouses + JSON perfil | Una bodega default activa. | ItemWarehouseInfo. |
| Presentations | Presentaciones | GridControl | Unidades | No | Si | UnitMeasures | ItemMasterProfiles JSON | Factores > 0. | UoM packages si aplica. |
| Attachments | Anexos | GridControl | Anexos | No | Si | Attachment catalogs | ItemMasterProfiles JSON | No auditoria editable. | Attachments/UDF externo. |
| Sap.IsSynchronized | Sincronizado SAP | LookUp/Text | SAP | No | No | N/A | ItemMasterProfiles JSON | Solo informativo. | Estado local. |
| Sap.SapItemCode | Codigo SAP | TextEdit | SAP | No | Controlado | SAP/local | ItemMasterProfiles JSON | Referencia remota. | ItemCode. |
| Sap.FieldMappings | Mapeo campos SAP | GridControl | SAP | No | Segun permiso | N/A | ItemMasterProfiles JSON | Mapeo aislado, no Domain. | Mappings. |
| Remarks.GeneralRemarks | Observaciones | MemoEdit | Observaciones | No | Si | N/A | ItemMasterProfiles JSON | Texto funcional. | UDF si aplica. |

Recomendacion SAP para Items:

- Mantener configuracion SAP en seccion dedicada.
- No enviar desde WinForms.
- Backend debe validar item completo antes de sincronizar.
- Usar cola SAP si se sincronizan maestros.
- No guardar credenciales SAP en formulario.

## 15. Pruebas recomendadas

Estado actual:

- No se detectaron proyectos de tests.

Propuesta minima:

Application:

- `CreatePurchaseOrderCommandValidator` requiere proveedor, fechas, moneda, lineas, direccion delivery/billing.
- `UpdatePurchaseOrderCommandValidator` requiere Id > 0.
- `ApprovePurchaseOrderCommandHandler` solo permite transicion desde `PendingApproval`.
- `RejectPurchaseOrderCommandHandler` solo permite transicion desde `PendingApproval`.
- `SyncPurchaseOrderSapCommandHandler` rechaza no aprobadas y ya sincronizadas.
- `CreateItemCommandValidator` valida lotes/series, factores, codigos de barra, bodega default.

Persistence:

- Repositorios invocan nombres de SP esperados.
- Serializacion JSON de lineas/anexos no pierde propiedades obligatorias.
- Tenant connection factory usa company context activo.

Api:

- Login con credenciales invalidas retorna 401/response esperado.
- Endpoint protegido sin JWT retorna 401.
- Permiso faltante retorna 403.
- `X-Company-Code` faltante en tenant endpoints retorna error controlado.

SAP con mocks:

- `SendDocumentToSapCommandHandler` registra log failed/success.
- Worker futuro no reenvia documento ya sincronizado.
- Retry incrementa contador y conserva payload/respuesta.

Herramientas recomendadas:

- xUnit.
- FluentAssertions.
- Moq o NSubstitute.
- WebApplicationFactory para API si se habilita `NuanSystem.Api.Tests`.

## 16. Plan de cambios por commits

### Commit 1: Modularizacion de endpoints

Objetivo:

- Reducir `Program.cs` a composicion de pipeline y llamadas `app.MapXEndpoints()`.

Archivos a modificar:

- `src/Backend/NuanSystem.Api/Program.cs`
- Nuevos endpoints bajo `src/Backend/NuanSystem.Api/Endpoints/`

Archivos nuevos sugeridos:

- `AuthEndpoints.cs`
- `CompanyEndpoints.cs`
- `ConfigurationCompanyEndpoints.cs`
- `ConfigurationSettingEndpoints.cs`
- `UserEndpoints.cs`
- `RoleEndpoints.cs`
- `SecurityOperationEndpoints.cs`
- `SecurityMenuEndpoints.cs`
- `SecurityFormEndpoints.cs`
- `SecurityFieldEndpoints.cs`
- `SecurityAccessEndpoints.cs`
- `SapEndpoints.cs`
- `SettingsEndpoints.cs`
- `TenancyEndpoints.cs`
- `AuditEndpoints.cs`

Reglas:

- No cambiar rutas.
- No cambiar permisos.
- No cambiar request/response.
- Mantener rutas legacy duplicadas y documentar canonicas.

Riesgo: medio, por movimiento de codigo; requiere build y pruebas API.

### Commit 2: Refactor seguro de cambio de clave

Objetivo:

- Sacar SQL directo de `Program.cs`.

Archivos a modificar:

- `Application/Features/Auth/Commands`
- Contrato de repositorio/servicio en `Application.Abstractions`
- Implementacion en `Persistence`
- Endpoint de Auth solo llama `ISender`

Riesgo: alto si se toca seguridad; requiere tests.

### Commit 3: Limpieza DevExpress documentada

Objetivo:

- Definir estrategia portable.

Opciones:

- Mantener `HintPath` y centralizar `DevExpressRoot`.
- Migrar a NuGet/offline feed.
- Usar `Directory.Build.props` para version/ruta.

Riesgo: alto para compilacion local; no eliminar referencias sin validacion.

### Commit 4: Diseno SAP asincrono para documentos

Objetivo:

- Crear diseno tecnico y cambios minimos seguros para cola.

Archivos candidatos:

- `Application.Features.SapSync`
- `Application.Abstractions.Sap`
- `Persistence.Repositories.SapSyncLogRepository`
- SQL nuevo futuro para cola

Riesgo: alto funcional; empezar con documento/contratos, no worker completo.

### Commit 5: Estandarizacion de permisos por formulario

Objetivo:

- Matriz de permisos y operaciones.

Archivos candidatos:

- `PermissionCodes.cs`
- scripts master de seguridad
- endpoint authorization si requiere alias

Riesgo: alto para usuarios existentes; no revocar permisos sin migracion.

### Commit 6: Pruebas minimas

Objetivo:

- Crear estructura `tests`.

Archivos nuevos:

- `tests/NuanSystem.Application.Tests`
- `tests/NuanSystem.Api.Tests`
- `tests/NuanSystem.Persistence.Tests` o plan si no hay contexto suficiente.

Riesgo: bajo-medio; puede requerir paquetes NuGet y ajustes de solucion.

### Commit 7: Orden de Compra

Objetivo:

- Formalizar matriz de estados y aprobaciones.

Archivos candidatos:

- Commands/handlers/validators de PurchaseOrders.
- Repository y SQL de PurchaseOrders.
- Frontend PurchaseOrders ViewModel/Form si se alinean acciones.

Riesgo: alto operativo; requiere pruebas antes.

### Commit 8: Maestro de Items

Objetivo:

- Normalizar diccionario, nombres tecnicos y persistencia critica.

Archivos candidatos:

- `ItemEditForm`/Designer solo con extrema cautela.
- DTOs/validators si se formalizan campos.
- SQL/perfil maestro si se decide normalizar columnas.

Riesgo: alto por tamano del formulario y amplitud funcional.

## 17. Riesgos antes de produccion

- Ausencia de tests automatizados.
- SAP sin cola productiva e idempotencia completa para documentos.
- DevExpress atado a instalacion local exacta.
- `Program.cs` con demasiadas responsabilidades.
- SQL directo en API.
- Reemplazo completo de hijos en documentos operativos.
- Permisos granulares y operaciones UI no totalmente alineados.
- Scripts SQL legacy con redefiniciones y orden sensible.
- Maestro de Items con perfil JSON que puede limitar reporting y auditoria granular.
- Falta de pruebas de concurrencia y transiciones de estado.

## 18. Checklist de aceptacion

- [ ] El proyecto sigue compilando despues de cualquier cambio futuro.
- [ ] No se cambiaron rutas publicas sin justificacion.
- [ ] `Program.cs` queda mas limpio o el plan queda documentado.
- [ ] No hay logica de negocio nueva en WinForms.
- [ ] WinForms no conecta directo a SQL Server.
- [ ] WinForms no conecta directo a SAP.
- [ ] No hay referencias SAP concretas en Domain.
- [ ] No se eliminan permisos existentes sin migracion.
- [ ] No se duplican endpoints sin documentar ruta canonica.
- [ ] Las recomendaciones SAP consideran reintento, auditoria, payload, respuesta y trazabilidad.
- [ ] Maestro de Items tiene diccionario de campos.
- [ ] Orden de Compra tiene matriz de estados y permisos.
- [ ] Se documentan riesgos de DevExpress local.
- [ ] Se agregan tests minimos antes de refactors funcionales.
- [ ] Se conserva compatibilidad con endpoints existentes.
- [ ] Se documentan dudas o pendientes antes de implementar cambios riesgosos.

## Anexo A: Archivos revisados

- `README.md`
- `docs/ARCHITECTURE.md`
- `src/Backend/NuanSystem.Api/Program.cs`
- `src/Backend/NuanSystem.Api/Endpoints/*.cs`
- `src/Backend/NuanSystem.Api/Extensions/*.cs`
- `src/Backend/NuanSystem.Api/Middleware/*.cs`
- `src/Backend/NuanSystem.Application/DependencyInjection/ApplicationServiceRegistration.cs`
- `src/Backend/NuanSystem.Application/Features/*`
- `src/Backend/NuanSystem.Application/Abstractions/*`
- `src/Backend/NuanSystem.Persistence/DependencyInjection/PersistenceServiceRegistration.cs`
- `src/Backend/NuanSystem.Persistence/Repositories/*`
- `src/Backend/NuanSystem.SapIntegration/*`
- `src/Frontend/NuanSystem.WinForms.Services/*`
- `src/Frontend/NuanSystem.WinForms.ViewModels/*`
- `src/Frontend/NuanSystem.WinForms.Forms/*`
- Archivos `.csproj` de backend y frontend.
- `database/sql/*.sql`

## Anexo B: Que no se debe tocar todavia

- No cambiar rutas de API.
- No eliminar rutas legacy `/api/users`, `/api/roles`, `/countries`, `/provinces`, `/cities`.
- No modificar `.csproj` DevExpress hasta definir estrategia y validar build.
- No implementar worker SAP completo sin tabla de cola, pruebas e idempotencia.
- No dividir `ItemEditForm.Designer.cs` sin una estrategia compatible con Visual Studio Designer.
- No cambiar permisos existentes sin script de migracion y analisis de usuarios/roles.
- No normalizar JSON de `ItemMasterProfiles` a columnas sin decidir reporting, indices y compatibilidad.
