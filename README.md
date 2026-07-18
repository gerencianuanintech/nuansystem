# NuanSystem

Solucion empresarial modular para API REST .NET, frontend Windows Forms con DevExpress, soporte multiempresa/multibase e integracion opcional con SAP Business One.

## Estructura principal

- `src/Backend/NuanSystem.Api`: entrada HTTP, endpoints, middleware, filtros, Swagger y composicion de dependencias.
- `src/Backend/NuanSystem.Application`: casos de uso con MediatR, DTOs, validaciones, contratos de servicios y reglas de aplicacion.
- `src/Backend/NuanSystem.Domain`: entidades, value objects, reglas de dominio y contratos puros.
- `src/Backend/NuanSystem.Infrastructure`: servicios tecnicos como JWT, cifrado, logging, reloj del sistema y proveedores externos no SAP.
- `src/Backend/NuanSystem.Persistence`: DbContexts, conexiones dinamicas, repositorios, migraciones y scripts SQL.
- `src/Backend/NuanSystem.SapIntegration`: clientes SAP Service Layer/DI API, envio de documentos y manejo de sincronizacion.
- `src/Backend/NuanSystem.Shared`: contratos compartidos, respuestas estandarizadas, constantes y enums.
- `src/Frontend/NuanSystem.WinForms`: aplicacion ejecutable, composicion, sesion, tema y arranque.
- `src/Frontend/NuanSystem.WinForms.Services`: servicios HTTP reutilizables para consumir la API.
- `src/Frontend/NuanSystem.WinForms.ViewModels`: estado y logica de presentacion sin reglas de negocio.
- `src/Frontend/NuanSystem.WinForms.Forms`: formularios XtraForm por modulo.
- `src/Frontend/NuanSystem.WinForms.Controls`: controles reutilizables, grillas, editores y navegacion.
- `database/sql`: scripts SQL iniciales, master database y objetos por tenant.
- `docs`: documentacion tecnica de arquitectura, despliegue y decisiones.

## Documentacion

- `docs/ARCHITECTURE.md`: vision general de capas, flujo multiempresa y reglas de dependencia.
- `docs/ARQUITECTURA-COMERCIAL.md`: direccion tecnica para evolucionar hacia una plataforma comercial multi-giro con capacidades configurables, inventario, ventas, compras, caja, precios y transacciones.
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`: arquitectura objetivo para ERP independiente, Master central, bases por sucursal, sincronizacion Outbox/Inbox e integracion SAP opcional.
- `docs/architecture/SRI-DOCUMENTS-WORKER.md`: arquitectura objetivo para modulo SRI centralizado, cola de documentos y Worker Service responsable de XML.
- `docs/operations/SYNC-MASTER-BRANCH-OPERATIONS.md`: guia operativa de Sync Master/Sucursal, estados, worker, monitoreo, acciones manuales y reglas de seguridad.
- `docs/operations/SYNC-MASTER-BRANCH-DEPLOYMENT-CHECKLIST.md`: checklist de despliegue controlado para Sync Master/Sucursal.
- `docs/operations/SYNC-MASTER-BRANCH-TROUBLESHOOTING.md`: diagnostico de eventos Pending, InProcess, Error, DeadLetter, duplicados y limites actuales.
- `docs/operations/SAP-PURCHASE-ORDER-PILOT-PRODUCTION-RUNBOOK.md`: despliegue gradual, secretos, readiness, oleadas y rollback del flujo SAP Purchase Order hacia sucursales.
- `docs/FASE-1-BASE-TECNICA.md`: detalle de lo implementado en la Fase 1.
- `docs/FASE-2-MULTIEMPRESA.md`: detalle de la arquitectura multiempresa y resolucion de tenant.
- `docs/FASE-3-SEGURIDAD.md`: autenticacion JWT, roles, permisos y empresas por usuario.
- `docs/FASE-4-PERSISTENCIA.md`: Dapper, fabricas de conexion, repositorio base y esquema tenant.
- `docs/FASE-5-MEDIATR.md`: MediatR, FluentValidation, behaviors y ejemplo Customers.
- `docs/FASE-6-EMPRESAS.md`: CRUD inicial de empresas, validacion de conexion y asignacion de usuarios.
- `docs/FASE-7-CLIENTES.md`: CRUD completo de clientes por empresa activa.
- `docs/FASE-8-ARTICULOS.md`: CRUD completo de articulos por empresa activa.
- `docs/FASE-9-DOCUMENTOS.md`: documentos comerciales con cabecera, detalle y totales.
- `docs/FASE-10-SAP.md`: integracion inicial con SAP Business One, envio de documentos y logs de sincronizacion.
- `docs/FASE-11-FRONTEND-WINFORMS.md`: cliente WinForms inicial con login, empresa activa, menu y consumo HTTP.
- `docs/FASE-12-FRONTEND-MODULOS-OPERATIVOS.md`: formularios WinForms para clientes, articulos y documentos conectados a la API.
- `docs/FASE-13-ORIGINAL-MENU-PRINCIPAL.md`: menu principal dinamico por permisos, navegacion agrupada y logout formal.
- `docs/FASE-13-CREACION-DOCUMENTOS-WINFORMS.md`: pantalla maestro-detalle WinForms para crear documentos comerciales.
- `docs/FASE-14-CONFIGURACION-PARAMETROS.md`: parametros por empresa activa con API REST y modulo WinForms de configuracion.
- `docs/FASE-15-ADMINISTRACION-EMPRESAS-WINFORMS.md`: administracion de empresas desde WinForms con creacion y validacion de conexion.
- `docs/FASE-16-ADMINISTRACION-USUARIOS.md`: administracion de usuarios, roles iniciales y asignacion de empresas.
- `docs/FASE-17-ROLES-PERMISOS.md`: administracion de roles y permisos desde API y WinForms.
- `docs/FASE-18-AUTORIZACION-POR-PERMISOS.md`: autorizacion real por permisos en endpoints de la API.
- `docs/FASE-19-AUDITORIA-OPERATIVA.md`: auditoria de operaciones de escritura en API y consulta desde WinForms.

## Arquitectura objetivo

NuanSystem se dirige a operar como ERP independiente, multiempresa y multisucursal, con una base `NuanSystem_Master` para gobierno central y bases tenant/sucursal para la operacion diaria.

SAP Business One es una integracion opcional por empresa. El producto debe funcionar con `SapIntegrationMode = None`, y cualquier comunicacion SAP queda aislada en backend mediante contratos de Application y el proyecto `NuanSystem.SapIntegration`; `Domain` y WinForms no dependen de SAP.

La sincronizacion entre Master y sucursales debe implementarse con Outbox/Inbox, mensajes versionados, idempotencia, auditoria, reintentos y Dead Letter. No se permiten escrituras cruzadas directas entre bases.

El modulo SRI es centralizado e independiente de SAP. Documentos originados por NuanSystem, TXT, AddOn SAP, formularios o integraciones externas solo alimentan una cola SRI; el Worker SRI es el unico responsable de consultar, descargar, procesar y almacenar XML.
