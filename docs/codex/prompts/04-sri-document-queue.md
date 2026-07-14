# Prompt 04 - Cola de documentos SRI

Actua como arquitecto senior .NET de NuanSystem.

Objetivo: crear la cola central SRI para que documentos desde NuanSystem, TXT, AddOn SAP o formularios alimenten un pipeline unico.

Lee antes de modificar:

- `AGENTS.md`
- `docs/architecture/SRI-DOCUMENTS-WORKER.md`
- `docs/ARQUITECTURA-COMERCIAL.md`

Reglas:

- SRI no depende de SAP.
- Capturadores solo encolan.
- API valida y registra; no descarga ni procesa XML.
- WinForms solo consume API.
- Usar permisos backend para encolar, consultar y reprocesar.

Entrega esperada:

- DTOs, commands, queries y validators.
- Contratos de repositorio.
- Tablas SQL Server para cola e intentos.
- Endpoints para encolar y consultar estado.
- Build de solucion.

