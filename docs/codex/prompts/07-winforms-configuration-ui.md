# Prompt 07 - UI WinForms de configuracion

Actua como arquitecto senior .NET y DevExpress WinForms de NuanSystem.

Objetivo: crear pantallas de configuracion para empresa, sucursal, capacidades, sincronizacion, SAP y SRI.

Lee antes de modificar:

- `AGENTS.md`
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`
- `docs/architecture/SRI-DOCUMENTS-WORKER.md`
- `docs/FASE-11-FRONTEND-WINFORMS.md`
- `docs/FASE-14-CONFIGURACION-PARAMETROS.md`

Reglas:

- WinForms consume API mediante cliente HTTP centralizado.
- No SQL directo, no SAP directo, no SRI directo.
- Formularios sin reglas de negocio.
- Validaciones decisivas en backend.
- Usar permisos, `ApiSession` y `X-Company-Code`.
- Mantener compatibilidad con Visual Studio Designer.

Entrega esperada:

- Servicios frontend HTTP.
- Modelos/ViewModels si aplica el patron local.
- Formularios DevExpress concretos.
- Integracion con menu/permisos.
- Build de frontend o solucion completa.

