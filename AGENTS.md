# AGENTS.md

Guia rectora para agentes Codex que trabajen en NuanSystem.

## Lectura obligatoria

Antes de modificar arquitectura, tenancy, integraciones, SRI, sincronizacion o SAP, revisar:

- `README.md`
- `docs/ARCHITECTURE.md`
- `docs/ARQUITECTURA-COMERCIAL.md`
- `docs/FASE-2-MULTIEMPRESA.md`
- `docs/FASE-10-SAP.md`
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`
- `docs/architecture/SRI-DOCUMENTS-WORKER.md`

## Reglas no negociables

- NuanSystem debe funcionar como ERP independiente sin SAP.
- SAP Business One es una integracion opcional por empresa, no una dependencia del producto.
- `Domain` no debe depender de SAP, SRI, SQL Server, Dapper, WinForms ni servicios externos.
- WinForms no debe conectarse directo a base de datos, SAP ni SRI.
- Toda operacion de negocio pasa por la API REST.
- La base Master gobierna empresas, sucursales, capacidades, conexiones, integraciones y configuracion global.
- Las bases de sucursal contienen operacion local y no deben conocer secretos de otras sucursales.
- La sincronizacion Master/Sucursal debe usar Outbox/Inbox con idempotencia y auditoria.
- SRI no depende de SAP.
- TXT, AddOn SAP, formularios y otros capturadores solo alimentan la cola SRI.
- El Worker SRI es el unico componente que descarga, autoriza, procesa y almacena XML SRI.

## Capas esperadas

- `Api`: endpoints, middleware, autenticacion, autorizacion y composicion.
- `Application`: casos de uso, contratos, DTOs, validaciones y orquestacion.
- `Domain`: reglas puras de negocio.
- `Persistence`: acceso a Master, sucursales, Outbox/Inbox, cola SRI y repositorios.
- `Infrastructure`: servicios tecnicos transversales.
- `SapIntegration`: clientes, mapeos y logs SAP aislados.
- `Worker`: procesos en segundo plano, incluido SRI.
- `WinForms`: cliente de escritorio que consume API mediante cliente HTTP centralizado.

## Como proponer cambios

1. Clasificar si el cambio es CRUD administrativo, proceso operativo, sincronizacion, integracion o worker.
2. Ubicar contratos en `Application` y detalles externos en `Persistence`, `Infrastructure`, `SapIntegration` o `Worker`.
3. Mantener configuracion por empresa/sucursal en Master cuando afecte comportamiento o integraciones.
4. Usar Outbox/Inbox para cualquier replicacion entre Master y sucursal.
5. Documentar decisiones nuevas en `docs/architecture` antes de codificar cambios de alcance transversal.

