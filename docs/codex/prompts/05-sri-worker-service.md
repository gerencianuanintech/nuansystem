# Prompt 05 - Worker Service SRI

Actua como arquitecto senior .NET de NuanSystem.

Objetivo: crear el Worker Service SRI responsable de procesar cola, consultar/descargar XML y persistir resultados.

Lee antes de modificar:

- `AGENTS.md`
- `docs/architecture/SRI-DOCUMENTS-WORKER.md`
- `docs/codex/prompts/04-sri-document-queue.md`

Reglas:

- El Worker SRI es el unico que descarga/procesa XML.
- El worker debe ser idempotente por `AccessKey` y mensaje.
- Reintentos con limite, backoff y Dead Letter.
- No loguear secretos ni XML sensible completo.
- Configuracion productiva preparada para Windows Service.

Entrega esperada:

- Proyecto Worker si no existe.
- Servicios de procesamiento enfocados.
- Repositorios/contratos necesarios.
- Health checks o logs operativos.
- Build y resumen de pruebas.

