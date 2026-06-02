# Revision y formalizacion del flujo de Orden de Compra

Fecha: 2026-06-02  
Alcance: preparacion del Commit 4  
Estado: analisis tecnico, sin cambios de codigo

## Objetivo

Analizar el modulo de Orden de Compra para formalizar estados, transiciones, validaciones, permisos, aprobacion y riesgos antes de tocar SAP o implementar un worker de sincronizacion.

Este documento no aplica cambios funcionales. Solo registra diagnostico, riesgos y plan recomendado.

## Archivos revisados

### Backend API

- `src/Backend/NuanSystem.Api/Endpoints/PurchaseOrderEndpoints.cs`

### Application

- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/PurchaseOrderCalculator.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/CreatePurchaseOrderCommand.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/UpdatePurchaseOrderCommand.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandHandlers.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandValidators.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderWorkflowCommands.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Dtos/PurchaseOrderDtos.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Queries/PurchaseOrderQueries.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Queries/PurchaseOrderQueryHandlers.cs`

### Abstracciones y persistencia

- `src/Backend/NuanSystem.Application/Abstractions/Data/IPurchaseOrderRepository.cs`
- `src/Backend/NuanSystem.Persistence/Repositories/Purchasing/PurchaseOrderRepository.cs`

### SQL

- `database/sql/032_tenant_purchasing_agents_catalog.sql`
- `database/sql/046_tenant_purchase_orders.sql`
- `database/sql/047_master_purchase_orders_security.sql`

### Frontend WinForms

- `src/Frontend/NuanSystem.WinForms.Forms/Purchasing/PurchaseOrders/PurchaseOrdersForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Purchasing/PurchaseOrders/FrmPurchaseOrderEdit.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Purchasing/PurchaseOrders/IPurchaseOrderClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Purchasing/PurchaseOrders/PurchaseOrderClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Purchasing/PurchaseOrders/Models/PurchaseOrderModels.cs`
- `src/Frontend/NuanSystem.WinForms.ViewModels/Purchasing/PurchaseOrders/PurchaseOrdersViewModel.cs`

## Endpoints actuales

| Accion | Endpoint | Metodo | Operacion actual |
|---|---|---|---|
| Listar | `/api/purchase-orders` | GET | `refresh` |
| Lookups | `/api/purchase-orders/lookups` | GET | `refresh` |
| Consultar | `/api/purchase-orders/{id:int}` | GET | `consult` |
| Crear | `/api/purchase-orders` | POST | `create` |
| Guardar legacy | `/api/purchase-orders/{id:int}/save` | POST | `update` |
| Actualizar | `/api/purchase-orders/{id:int}` | PUT | `update` |
| Eliminar/anular | `/api/purchase-orders/{id:int}` | DELETE | `delete` |
| Enviar a aprobacion | `/api/purchase-orders/{id:int}/send-to-approval` | POST | `approve` |
| Aprobar | `/api/purchase-orders/{id:int}/approve` | POST | `approve` |
| Rechazar | `/api/purchase-orders/{id:int}/reject` | POST | `approve` |
| Solicitar sincronizacion SAP | `/api/purchase-orders/{id:int}/sync-sap` | POST | `syncsap` |
| Estado SAP | `/api/purchase-orders/{id:int}/sap-status` | GET | `consult` |
| Consultar documentos relacionados | `/api/purchase-orders/{id:int}/related-documents` | GET | `consult` |
| Agregar documento relacionado | `/api/purchase-orders/{id:int}/related-documents` | POST | `update` |
| Eliminar documento relacionado | `/api/purchase-orders/{id:int}/related-documents/{relatedId:int}` | DELETE | `update` |
| Consultar anexos | `/api/purchase-orders/{id:int}/attachments` | GET | `consult` |
| Agregar anexo | `/api/purchase-orders/{id:int}/attachments` | POST | `update` |
| Eliminar anexo | `/api/purchase-orders/{id:int}/attachments/{attachmentId:int}` | DELETE | `update` |

## Commands actuales

- `CreatePurchaseOrderCommand`
- `UpdatePurchaseOrderCommand`
- `DeletePurchaseOrderCommand`
- `SendPurchaseOrderToApprovalCommand`
- `ApprovePurchaseOrderCommand`
- `RejectPurchaseOrderCommand`
- `SyncPurchaseOrderSapCommand`
- `AddPurchaseOrderRelatedDocumentCommand`
- `DeletePurchaseOrderRelatedDocumentCommand`
- `AddPurchaseOrderAttachmentCommand`
- `DeletePurchaseOrderAttachmentCommand`

## Queries actuales

- `GetPurchaseOrdersQuery`
- `GetPurchaseOrderByIdQuery`
- `GetPurchaseOrderLookupsQuery`

## DTOs actuales

- `PurchaseOrderDto`
- `PurchaseOrderSummaryDto`
- `PurchaseOrderLineDto`
- `PurchaseOrderAddressDto`
- `PurchaseOrderApprovalDto`
- `PurchaseOrderRelatedDocumentDto`
- `PurchaseOrderAttachmentDto`
- `PurchaseOrderSapSyncLogDto`
- `PurchaseOrderSaveRequest`
- `PurchaseOrderLineSaveRequest`
- `PurchaseOrderAddressSaveRequest`
- `PurchaseOrderRelatedDocumentSaveRequest`
- `PurchaseOrderAttachmentSaveRequest`
- `PurchaseOrderLookupsDto`

## Validators actuales

Existen:

- `CreatePurchaseOrderCommandValidator`
- `UpdatePurchaseOrderCommandValidator`
- `PurchaseOrderSaveValidator<TCommand>`

Cubren validaciones de captura y lineas, pero no formalizan validaciones de workflow. No hay validadores especificos para aprobar, rechazar, enviar a aprobacion, solicitar SAP, anexos o documentos relacionados.

## Repository actual

`IPurchaseOrderRepository` expone:

- `GetAllAsync`
- `GetByIdAsync`
- `GetLookupsAsync`
- `CreateAsync`
- `UpdateAsync`
- `DeleteAsync`
- `UpdateStatusAsync`
- `AddSapLogAsync`

La implementacion `PurchaseOrderRepository` usa Dapper, `ITenantConnectionFactory`, JSON para hijos y stored procedures SQL Server.

## Tablas y stored procedures relacionados

### Tablas

- `PurchaseTypes`
- `DocumentSeries`
- `PurchaseOrderHeaders`
- `PurchaseOrderLines`
- `PurchaseOrderAddresses`
- `PurchaseOrderApprovals`
- `PurchaseOrderRelatedDocuments`
- `PurchaseOrderAttachments`
- `PurchaseOrderSapSyncLogs`

### Stored procedures

- `SP_NA_GET_PURCHASEORDERS_LISTAR`
- `SP_NA_GET_PURCHASEORDERS_BUSCARPORID`
- `SP_NA_GET_PURCHASEORDERS_LOOKUPS`
- `SP_NA_POST_PURCHASEORDERS_CREAR`
- `SP_NA_PUT_PURCHASEORDERS_ACTUALIZAR`
- `SP_NA_INTERNAL_PURCHASEORDERS_REPLACE_CHILDREN`
- `SP_NA_DELETE_PURCHASEORDERS_ELIMINAR`
- `SP_NA_PATCH_PURCHASEORDERS_ESTADO`
- `SP_NA_POST_PURCHASEORDERS_SAPLOG`

## Estados actuales detectados

Estados de cabecera:

- `Draft`
- `PendingApproval`
- `Approved`
- `Rejected`
- `SapPending`
- `SapSynced`
- `SapError`
- `Closed`
- `Cancelled`

Estados adicionales:

- Lineas: `Open`, `Closed`, `Cancelled`
- Aprobacion: `Pending`, `InProgress`, `Approved`, `Rejected`, `Cancelled`
- SAP: `Pending`, `Synced`, `Error`, `Cancelled`

## Transiciones actuales detectadas

- Crear genera `Draft`.
- Actualizar bloquea solo `Closed`, `Cancelled` y `SapSynced`.
- Eliminar permite solo `Draft` y `Rejected`; el stored procedure marca `Cancelled`.
- Enviar a aprobacion cambia a `PendingApproval` sin validar estado origen.
- Aprobar cambia a `Approved` sin validar que venga de `PendingApproval`.
- Rechazar cambia a `Rejected` sin validar que venga de `PendingApproval`.
- Solicitar SAP permite `Approved` o `SapPending`, registra log y marca `SapPending`.
- Si `SapStatus` es `Synced`, se bloquea la sincronizacion.
- Anexos y documentos relacionados bloquean solo `Closed`, `Cancelled` y `SapSynced`.

## Matriz recomendada de estados

| Estado | Editar | Eliminar/anular | Enviar aprobacion | Aprobar | Rechazar | Solicitar SAP | Reintentar SAP | Consultar docs | Agregar anexos | Modificar anexos |
|---|---|---|---|---|---|---|---|---|---|---|
| Draft | Si | Si | Si | No | No | No | No | Si | Si | Si |
| PendingApproval | No | No | No | Si | Si | No | No | Si | Si | Si |
| Approved | No | No | No | No | No | Si | No | Si | Si | Si |
| Rejected | Si | Si | Si | No | No | No | No | Si | Si | Si |
| SapPending | No | No | No | No | No | No | No | Si | No | No |
| SapSynced | No | No | No | No | No | No | No | Si | No | No |
| SapError | Si, controlado | No | No | No | No | No | Si | Si | Si | Si |
| Closed | No | No | No | No | No | No | No | Si | No | No |
| Cancelled | No | No | No | No | No | No | No | Si | No | No |

## Matriz de permisos

| Accion | Endpoint | Metodo | Operacion actual | PermissionCode recomendado | Desalineacion | Recomendacion |
|---|---|---|---|---|---|---|
| Listar | `/api/purchase-orders` | GET | `refresh` | `PurchaseOrdersRead` | Baja | Mantener o mapear explicitamente a lectura |
| Lookups | `/api/purchase-orders/lookups` | GET | `refresh` | `PurchaseOrdersRead` | Baja | Correcto |
| Consultar | `/api/purchase-orders/{id:int}` | GET | `consult` | `PurchaseOrdersRead` | Baja | Correcto |
| Crear | `/api/purchase-orders` | POST | `create` | `PurchaseOrdersManage` | Baja | Correcto |
| Guardar legacy | `/api/purchase-orders/{id:int}/save` | POST | `update` | `PurchaseOrdersManage` | Baja | Mantener por compatibilidad |
| Actualizar | `/api/purchase-orders/{id:int}` | PUT | `update` | `PurchaseOrdersManage` | Baja | Correcto |
| Eliminar/anular | `/api/purchase-orders/{id:int}` | DELETE | `delete` | `PurchaseOrdersManage` | Media | Evaluar operacion futura `cancel` |
| Enviar a aprobacion | `/api/purchase-orders/{id:int}/send-to-approval` | POST | `approve` | `PurchaseOrdersApprove` o `PurchaseOrdersManage` | Media | Definir si enviar pertenece a gestion o aprobacion |
| Aprobar | `/api/purchase-orders/{id:int}/approve` | POST | `approve` | `PurchaseOrdersApprove` | Baja | Correcto |
| Rechazar | `/api/purchase-orders/{id:int}/reject` | POST | `approve` | `PurchaseOrdersApprove` | Media | Puede mantenerse como workflow, pero conviene operacion diferenciada futura |
| Solicitar SAP | `/api/purchase-orders/{id:int}/sync-sap` | POST | `syncsap` | `PurchaseOrdersSyncSap` | Baja | Correcto |
| Estado SAP | `/api/purchase-orders/{id:int}/sap-status` | GET | `consult` | `PurchaseOrdersRead` | Baja | Correcto |
| Docs relacionados | `/api/purchase-orders/{id:int}/related-documents` | GET | `consult` | `PurchaseOrdersRead` | Baja | Correcto |
| Agregar doc relacionado | `/api/purchase-orders/{id:int}/related-documents` | POST | `update` | `PurchaseOrdersManage` | Media | Revisar si aplica despues de aprobacion |
| Eliminar doc relacionado | `/api/purchase-orders/{id:int}/related-documents/{relatedId:int}` | DELETE | `update` | `PurchaseOrdersManage` | Media | Revisar si aplica despues de aprobacion |
| Anexos | `/api/purchase-orders/{id:int}/attachments` | GET | `consult` | `PurchaseOrdersRead` | Baja | Correcto |
| Agregar anexo | `/api/purchase-orders/{id:int}/attachments` | POST | `update` | `PurchaseOrdersManage` | Media | Evaluar permiso especifico si anexos son auditables |
| Eliminar anexo | `/api/purchase-orders/{id:int}/attachments/{attachmentId:int}` | DELETE | `update` | `PurchaseOrdersManage` | Media | Evaluar permiso especifico si anexos son auditables |

## Validaciones revisadas

| Validacion | Estado actual |
|---|---|
| Proveedor obligatorio | Cubierta |
| Serie obligatoria | Cubierta |
| Numero obligatorio | Cubierta |
| Fechas obligatorias | Cubierta |
| Moneda obligatoria | Cubierta |
| Condicion de pago | Cubierta |
| Comprador | Cubierta |
| Bodega principal | Cubierta |
| Direccion de entrega | Cubierta por tipo, falta revisar contenido |
| Direccion de facturacion | Cubierta por tipo, falta revisar contenido |
| Al menos una linea | Cubierta |
| Item por linea | Cubierta |
| Cantidad mayor a cero | Cubierta |
| Precio mayor o igual a cero | Cubierta |
| Impuesto por linea | Cubierta |
| Bodega por linea | Cubierta |
| Fecha requerida por linea | Cubierta |
| Descuento entre 0 y 100 | Cubierta |
| Total calculado | Cubierto por calculadora |
| Estado permitido para cada accion | Parcial |
| No sincronizar si ya esta sincronizada | Cubierta |
| No sincronizar si esta rechazada/anulada/cerrada | Cubierta indirectamente |
| No aprobar si no esta pendiente | No cubierta |
| No rechazar si no esta pendiente | No cubierta |

## Acciones de UI relacionadas

- La lista permite crear, editar, consultar y eliminar segun permisos CRUD.
- La accion SAP existe en el cliente y en el endpoint.
- En la lista, la sincronizacion SAP aparece reutilizando `HistoryAsync`, no como accion operativa independiente.
- El cliente HTTP tiene metodos para enviar a aprobacion, aprobar, rechazar, sincronizar SAP, anexos y documentos relacionados.
- El ViewModel no expone todavia todo el flujo de aprobacion, rechazo, documentos relacionados y anexos.
- En el formulario de edicion existen controles de SAP, anexos y documentos relacionados, pero parte del comportamiento parece informativo o incompleto.

## Diagnostico actual

El modulo tiene una base amplia y bien separada por capas: endpoints delgados, Application con commands/queries, Persistence con repositorio y SQL, y cliente WinForms centralizado.

El problema principal no es la existencia del CRUD, sino la falta de una maquina de estados formal. Hoy las transiciones criticas dependen de validaciones parciales en handlers, y varios cambios de estado pueden ejecutarse desde estados no permitidos.

## Problemas detectados

1. No existe una politica central de estados y transiciones.
2. `Approve` y `Reject` no validan que la orden este en `PendingApproval`.
3. `SendToApproval` no valida estado origen.
4. `PendingApproval`, `Approved`, `SapPending` y `SapError` pueden permitir ediciones no deseadas.
5. La tabla `PurchaseOrderApprovals` existe, pero el flujo no registra aprobaciones reales.
6. La observacion de aprobacion o rechazo no se persiste.
7. SAP solo queda marcado como pendiente; no hay worker ni idempotencia.
8. No hay proteccion fuerte contra doble solicitud SAP concurrente.
9. No hay control de concurrencia optimista.
10. La numeracion de documentos requiere formalizacion atomica.
11. La UI no expone claramente todas las acciones de workflow.
12. Los permisos existen como `PermissionCodes`, pero los endpoints usan `RequireFormOperation`; conviene formalizar el mapeo.

## Riesgos funcionales

- Aprobaciones desde estados incorrectos.
- Rechazos desde estados incorrectos.
- Edicion posterior a aprobacion.
- Cambios durante sincronizacion SAP pendiente.
- Eliminacion conceptual mezclada con anulacion.
- Documentos relacionados y anexos modificables en estados sensibles.
- Totales recalculados correctamente, pero sin proteccion de version contra ediciones simultaneas.

## Riesgos SAP

- La orden puede quedar en `SapPending` indefinidamente.
- Un futuro worker podria duplicar documentos si no hay idempotencia.
- Falta una llave de correlacion SAP o hash del payload.
- Falta una estrategia para timeout: SAP crea documento, pero NuanSystem no recibe confirmacion.
- `AttemptNumber = MAX + 1` puede ser inseguro bajo concurrencia.

## Riesgos de duplicidad de documento SAP

El riesgo es alto si se implementa sincronizacion real sin formalizar antes:

- bloqueo transaccional de solicitud SAP,
- idempotency key,
- persistencia de `SapDocEntry`/`SapDocNum`,
- correlacion entre intento y payload,
- guardas atomicas para no reenviar una orden ya procesada.

## Riesgos de concurrencia

- No se detectan cambios simultaneos por dos usuarios.
- `SP_NA_PATCH_PURCHASEORDERS_ESTADO` no valida estado origen.
- `SP_NA_PUT_PURCHASEORDERS_ACTUALIZAR` no aplica guardas de estado.
- El reemplazo de hijos borra e inserta; debe operar siempre bajo transaccion.
- El log SAP calcula numero de intento con `MAX + 1`.

## Riesgos de aprobacion

- No hay flujo formal por niveles.
- No se persiste aprobador, fecha ni motivo desde los commands actuales.
- `PendingApproval` no bloquea de forma suficiente la edicion.
- Rechazo no conserva motivo funcional.
- La UI no presenta claramente acciones de aprobar/rechazar.

## Campos faltantes potenciales

- `RowVersion` o token de concurrencia.
- `SubmittedAt`, `SubmittedBy`.
- `ApprovedAt`, `ApprovedBy`.
- `RejectedAt`, `RejectedBy`, `RejectionReason`.
- `CancelledAt`, `CancelledBy`, `CancellationReason`.
- `ClosedAt`, `ClosedBy`.
- `SapRequestedAt`, `SapSyncedAt`, `SapErrorAt`.
- `SapCorrelationId` o idempotency key.
- `SapPayloadHash`.
- `SapDocEntry` y `SapDocNum`, si no estan previstos en integracion final.
- Estado de worker o bloqueo de sincronizacion.

## Cambios recomendados para un futuro commit

1. Crear una politica central tipo `PurchaseOrderWorkflowPolicy` o `PurchaseOrderStateMachine`.
2. Validar estado origen y destino en Application.
3. Hacer que aprobar y rechazar solo apliquen desde `PendingApproval`.
4. Hacer que enviar a aprobacion solo aplique desde `Draft` o `Rejected`.
5. Bloquear edicion de estados sensibles segun matriz formal.
6. Registrar decisiones en `PurchaseOrderApprovals`.
7. Persistir observaciones de aprobacion y rechazo.
8. Preparar idempotencia SAP antes de implementar worker.
9. Agregar pruebas unitarias de transiciones.
10. Alinear ViewModel y UI con acciones reales por estado.

## Archivos que se tocarian en un futuro commit

- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandHandlers.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderWorkflowCommands.cs`
- `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandValidators.cs`
- nuevo archivo de politica de estados en `src/Backend/NuanSystem.Application/Features/Purchasing/PurchaseOrders/`
- `src/Backend/NuanSystem.Application/Abstractions/Data/IPurchaseOrderRepository.cs`
- `src/Backend/NuanSystem.Persistence/Repositories/Purchasing/PurchaseOrderRepository.cs`
- `database/sql/046_tenant_purchase_orders.sql`
- `database/sql/047_master_purchase_orders_security.sql`, solo si se formalizan nuevas operaciones
- `src/Frontend/NuanSystem.WinForms.ViewModels/Purchasing/PurchaseOrders/PurchaseOrdersViewModel.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Purchasing/PurchaseOrders/PurchaseOrdersForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Purchasing/PurchaseOrders/FrmPurchaseOrderEdit.cs`
- proyecto de pruebas de Application, si se agregan pruebas de workflow

## Pruebas necesarias

- Crear orden y verificar estado `Draft`.
- Enviar `Draft` a `PendingApproval`.
- Enviar `Rejected` a `PendingApproval`.
- Aprobar solo desde `PendingApproval`.
- Rechazar solo desde `PendingApproval`.
- Fallar aprobacion desde `Draft`, `Approved`, `Rejected`, `SapPending`, `SapSynced`, `Closed`, `Cancelled`.
- Fallar rechazo desde estados distintos a `PendingApproval`.
- Solicitar SAP solo desde estado permitido.
- No sincronizar si ya esta `SapSynced`.
- No editar estados bloqueados.
- No eliminar fuera de `Draft` o `Rejected`.
- No duplicar solicitud SAP concurrente.
- Verificar persistencia de aprobacion, rechazo y observacion cuando se implemente.

## Orden de implementacion recomendado

1. Formalizar matriz de estados en Application.
2. Agregar pruebas unitarias de transiciones.
3. Ajustar handlers para usar la politica.
4. Ajustar persistencia con guardas atomicas.
5. Persistir aprobacion, rechazo y observaciones.
6. Alinear UI con estados y permisos.
7. Preparar idempotencia SAP.
8. Disenar e implementar worker SAP en una fase posterior.

## Que no se debe tocar todavia

- Integracion real con SAP.
- Worker de sincronizacion.
- Redisenio amplio del frontend.
- Rutas existentes.
- Permisos existentes sin decision formal.
- Scripts SQL productivos sin migracion planificada.
- Formularios DevExpress fuera del alcance de Orden de Compra.
- Modulos no relacionados.

## Confirmacion

Este documento es solo preparacion tecnica. No cambia codigo backend, frontend, endpoints, permisos, estados, handlers, repositorios, formularios, archivos `.csproj`, scripts SQL ni SAP Integration.
