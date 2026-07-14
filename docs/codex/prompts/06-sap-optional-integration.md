# Prompt 06 - SAP Business One opcional

Actua como arquitecto senior .NET de NuanSystem.

Objetivo: reforzar SAP Business One como integracion opcional por empresa sin contaminar el dominio.

Lee antes de modificar:

- `AGENTS.md`
- `docs/FASE-10-SAP.md`
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`

Reglas:

- NuanSystem funciona sin SAP.
- SAP se activa por empresa desde Master.
- `Domain` no referencia SAP.
- WinForms no conecta a SAP.
- Application usa contratos; `NuanSystem.SapIntegration` implementa clientes, mapeos y sync.
- Registrar intentos y errores en `SapSyncLog`.

Entrega esperada:

- Configuracion SAP por empresa.
- Factory por modo `None`, `ServiceLayer`, `DiApi`.
- Validaciones y errores estandarizados.
- Reintentos controlados.
- Build de solucion.

