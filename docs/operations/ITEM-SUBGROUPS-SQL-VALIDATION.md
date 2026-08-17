# Despliegue SQL del maestro Subgrupos de articulos

## Alcance

Fecha: 2026-08-13.

- Tenant escrito: `NuanSystem_DEMO`.
- Master escrito: `NuanSystem_Master`.
- `NuanSystem_DEMO_REMIGIO`, `NuanSystem_DEMO_CANARIS` y
  `NuanSystem_SYNC_WH_BRANCH_TEST` no fueron modificadas.
- SAP, SRI, API, WinForms y workers permanecieron detenidos.

## Respaldos

Antes de ejecutar migraciones se crearon respaldos `COPY_ONLY WITH CHECKSUM`
y ambos aprobaron `RESTORE VERIFYONLY WITH CHECKSUM`:

- `/var/opt/mssql/data/NuanSystem_Master_ItemSubgroups205207_20260813_150100.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_ItemSubgroups205207_20260813_150100.bak`.

## Ejecucion

| Base | Migracion | Pases | Version final |
|---|---:|---:|---:|
| `NuanSystem_DEMO` | 205 | 2 | `20260813.205` una vez |
| `NuanSystem_Master` | 206 | 2 | `20260813.206` una vez |
| `NuanSystem_Master` | 207 | 2 | `20260813.207` una vez |

El primer intento de 207 se detuvo y revirtio antes de crear objetos o version
porque el esquema real de `SyncEntityDefinitionDependencies` no contiene la
columna historica `IsRequired`. Se alineo el `INSERT` al contrato vigente
(`EntityDefinitionId`, `DependsOnEntityDefinitionId` y auditoria), el validador
de lotes SQL aprobo y luego 207 completo dos pases idempotentes.

## Evidencia tenant

- Un subgrupo `GENERAL`, relacionado con la unica familia activa.
- `GlobalId` e `ItemFamilyId` completos y unicos según su contrato.
- Diez procedimientos dedicados presentes.
- `LocalOutbox` permanecio en 357 filas antes y despues.
- La migracion no genero auditoria CRUD ni eventos de sincronizacion.

## Evidencia Master

- FormKey `item-subgroups` y menu
  `MENU.DEFINITIONS.INVENTORY.ITEMSUBGROUPS` bajo
  `MENU.DEFINITIONS.INVENTORY`, orden 70.
- Permisos `GENERALINVENTORY.ITEMSUBGROUPS.READ/MANAGE` presentes y con los
  grants previos preservados.
- Operaciones del formulario asignadas a ADMIN.
- Definicion `ItemSubgroups`, orden 209, dependiente de `ItemFamilies`.
- `SyncEntityConfigurations.IsEnabled = 0`.
- `EntityOwnershipConfigurations.IsEnabled = 0`.
- Cero asociaciones a perfiles y cero eventos `SyncOutbox`.

## Cierre

No se activo sincronizacion, no se inicio ningun worker y no se realizaron
llamadas externas. El despliegue SQL y su validacion idempotente quedan
aprobados para el alcance indicado.
