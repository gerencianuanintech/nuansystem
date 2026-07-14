# Prompt 03 - Sincronizacion Outbox/Inbox

Actua como arquitecto senior .NET de NuanSystem.

Objetivo: implementar sincronizacion Master/Sucursal mediante Outbox/Inbox.

Lee antes de modificar:

- `AGENTS.md`
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`
- `docs/FASE-2-MULTIEMPRESA.md`

Reglas:

- No usar escrituras cruzadas directas entre bases.
- Todo mensaje debe ser idempotente.
- El Outbox se graba en la misma transaccion del cambio de negocio.
- El Inbox registra recepcion antes de aplicar.
- Usar estados, reintentos y Dead Letter.
- Registrar auditoria y `TraceId`.

Entrega esperada:

- Contratos `Application` para publicar y consumir mensajes.
- Tablas SQL Server Outbox/Inbox.
- Repositorios `Persistence`.
- Worker o servicio backend de sincronizacion.
- Build y verificacion.

