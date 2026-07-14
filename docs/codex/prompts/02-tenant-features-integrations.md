# Prompt 02 - Capacidades e integraciones por empresa

Actua como arquitecto senior .NET de NuanSystem.

Objetivo: consolidar capacidades de negocio e integraciones opcionales por empresa/sucursal.

Lee antes de modificar:

- `AGENTS.md`
- `docs/ARQUITECTURA-COMERCIAL.md`
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`
- `docs/architecture/SRI-DOCUMENTS-WORKER.md`

Reglas:

- Las reglas variables por giro se configuran como capacidades.
- SAP Business One es opcional por empresa.
- SRI es independiente de SAP.
- No hard-codear comportamiento por cliente o giro.
- No exponer secretos en logs ni respuestas.

Entrega esperada:

- Modelo de parametros/capacidades.
- Contratos para lectura segura desde Application.
- Persistencia SQL Server.
- Validaciones backend.
- Documentacion de decisiones si aparece una regla nueva.

