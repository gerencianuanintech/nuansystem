# Prompt 01 - Modelo tenant Master/Sucursal

Actua como arquitecto senior .NET de NuanSystem.

Objetivo: implementar el modelo Master/Sucursal manteniendo compatibilidad con la arquitectura multiempresa existente.

Lee antes de modificar:

- `AGENTS.md`
- `docs/ARCHITECTURE.md`
- `docs/FASE-2-MULTIEMPRESA.md`
- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`

Reglas:

- Master gobierna empresas, sucursales, conexiones, usuarios, permisos y capacidades.
- La empresa activa sigue resolviendose desde backend.
- Cuando aplique, agregar contexto de sucursal sin romper endpoints existentes.
- WinForms no se conecta a base.
- `Domain` no depende de infraestructura.

Entrega esperada:

- Contratos `Application` necesarios.
- Implementacion `Persistence` para SQL Server.
- Scripts SQL versionados.
- Endpoints delgados para administracion.
- Pruebas/build de solucion.

