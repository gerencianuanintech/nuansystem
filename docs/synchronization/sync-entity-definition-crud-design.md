# CRUD de definiciones de entidades de sincronizacion

## Estado

- Fase 0: diseno tecnico completado.
- Fase 1: persistencia Master completada.
- Fase 2: contratos Application, CQRS, validadores, catalogo tecnico y repositorio Dapper completados.
- Fase 3: API, permisos y registro de seguridad del mantenimiento completados.
- Fase 4: cliente HTTP, modelos y ViewModels WinForms completados.
- Fase 5: formularios WinForms, historial, navegacion y menu dinamico completados en codigo.
- Script `082_sync_entity_definition_winforms.sql` aplicado y verificado en `NuanSystem_Master`.
- Persistencia asociada: `database/sql/080_sync_entity_definitions.sql`.
- Base propietaria: `NuanSystem_Master`.
- Direccion soportada en la primera version: `MasterToBranch`.

## Problema

El selector `Codigo entidad` del mantenimiento de perfiles obtiene actualmente sus opciones desde
`SyncMasterBranchEntityCodes.InitialCatalog`, un catalogo definido en Application. Las entidades
seleccionadas se guardan en `SyncProfileEntities`, mientras que `SyncEntityConfigurations` conserva
configuracion operativa por empresa. Ninguna de esas dos tablas representa un catalogo global
administrable de definiciones.

## Separacion de responsabilidades

| Concepto | Propietario | Responsabilidad |
|---|---|---|
| Definicion de entidad | `SyncEntityDefinitions` | Catalogo global, nombre, descripcion, valores predeterminados y capacidades configurables. |
| Dependencias | `SyncEntityDefinitionDependencies` | Orden logico entre definiciones, sin ciclos ni autorreferencias. |
| Configuracion por empresa | `SyncEntityConfigurations` | Habilitacion operativa, direccion, conflicto, batch y reintentos por empresa. |
| Entidad de perfil | `SyncProfileEntities` | Copia de la configuracion elegida para un perfil concreto. |
| Matriz entidad-sucursal | `SyncProfileEntityBranches` | Distribucion habilitada y batch especifico por destino. |

`SyncEntityConfigurations` no se reutilizara como catalogo. Su granularidad es por empresa y su
responsabilidad pertenece al publicador/routing, no al mantenimiento global.

## Modelo aprobado

### SyncEntityDefinitions

- `Id`: identificador interno.
- `Code`: codigo tecnico unico e inmutable.
- `Name`: nombre visible.
- `Description`: descripcion funcional.
- `DefaultExecutionOrder`: orden sugerido al agregarla a un perfil.
- `SupportsIncremental`: permite seleccionar modo incremental.
- `SupportsInsert`, `SupportsUpdate`, `SupportsDeactivate`: capacidades configurables.
- `DefaultKeyField`, `DefaultModifiedAtField`: valores tecnicos predeterminados.
- `IsSystem`: identifica definiciones sembradas y evita su eliminacion.
- `IsActive`: controla si aparece como opcion nueva en lookups.
- Auditoria completa y eliminacion logica.

### SyncEntityDefinitionDependencies

- Relaciona una definicion con otra definicion requerida.
- No permite autorreferencias.
- No permite duplicados activos.
- El CRUD rechazara configuraciones que formen ciclos.
- Usa eliminacion logica y auditoria basica.

## Reglas de negocio

1. `Code` se asigna al crear y no se modifica posteriormente.
2. Una definicion de sistema no puede eliminarse; puede desactivarse si no se desea ofrecerla en nuevos perfiles.
3. Una definicion referenciada por `SyncProfileEntities` no puede eliminarse.
4. Una definicion usada como dependencia de otra no puede eliminarse.
5. Una definicion inactiva no puede agregarse como entidad activa a un perfil.
6. La base valida existencia y estado; Application validara ademas capacidades y mensajes funcionales.
7. `HasProducer` y `HasApplier` no son campos administrables. Representan implementaciones reales y se resolveran mediante un registro tecnico en Application.
8. Crear una definicion no crea productor, aplicador, SQL, scripts ni mapeos personalizados.
9. No se admite SQL ingresado por el usuario.
10. Las dependencias deben existir, estar activas y no formar ciclos.

## Capacidades tecnicas

El futuro servicio de catalogo combinara:

```text
SyncEntityDefinitions (metadatos administrables)
    +
registro tecnico de productores/aplicadores (codigo)
    =
catalogo expuesto a perfiles
```

Una definicion creada por un administrador podra mantenerse como borrador, pero no sera ejecutable
hasta que exista soporte tecnico registrado. Los indicadores de productor y aplicador se mostraran
como solo lectura.

## Persistencia y compatibilidad

- `SyncEntityDefinitions.Code` sera clave candidata para la relacion con `SyncProfileEntities.EntityCode`.
- Las 13 entidades actuales se sembraran como registros de sistema.
- El `CHECK` enumerado de `SyncProfileEntities.EntityCode` sera sustituido por una clave foranea.
- `SP_NA_PUT_SYNCPROFILEACTUALIZAR` validara contra el catalogo en lugar de una lista literal.
- Los procedimientos SQL Server permaneceran aislados en Persistence y `database/sql`.
- Una implementacion MySQL futura conservara los mismos contratos de Application con scripts propios.

## CRUD previsto

```text
GET    /api/sync/configuration/entities
GET    /api/sync/configuration/entities/lookup
GET    /api/sync/configuration/entities/{id}
POST   /api/sync/configuration/entities
PUT    /api/sync/configuration/entities/{id}
DELETE /api/sync/configuration/entities/{id}
```

Los endpoints API se incorporan en la Fase 3. Los clientes y formularios WinForms se implementaran
en fases posteriores.

## Implementacion de Fase 2

- Feature independiente: `Application/Features/Sync/EntityDefinitions`.
- Contrato: `ISyncEntityDefinitionRepository`.
- Queries: paginacion, detalle y lookup.
- Commands: crear, actualizar y eliminar logicamente.
- Validadores FluentValidation para longitudes, identificadores tecnicos, orden y dependencias.
- Persistence usa exclusivamente procedimientos almacenados mediante Dapper y la conexion Master.
- Los errores SQL funcionales se traducen a resultados tipados antes de regresar a Application.
- `ISyncEntityCatalogService` combina definiciones de Master con el registro tecnico existente.
- El catalogo de perfiles y `SyncProfileValidationService` consultan las definiciones de Master.
- Una definicion personalizada sin productor/aplicador puede guardarse en un perfil inactivo, pero no activar el perfil.

## Seguridad prevista

- FormKey: `sync-entities`.
- Tipo: mantenimiento, no transaccional.
- Operaciones: `refresh`, `consult`, `create`, `update`, `delete`, `customizecolumns`, `history`.
- El frontend no confiara en permisos locales para proteger el API.

## Implementacion de Fase 3

- Endpoints minimal API bajo `/api/sync/configuration/entities` delegados exclusivamente a MediatR.
- Grupo Swagger independiente: `Synchronization - Entity Definitions`.
- Permisos API: `SYNC.ENTITIES.VIEW`, `CREATE`, `EDIT` y `DELETE`.
- Formulario de seguridad registrado con `FormKey = sync-entities` y tipo mantenimiento.
- Operaciones estandar concedidas a `ADMIN`: refrescar, consultar, crear, actualizar, eliminar,
  personalizar columnas e historial.
- El formulario no se agrega todavia a `SecurityMenus`; se publicara cuando exista la pantalla WinForms.
- Script idempotente: `database/sql/081_sync_entity_definition_api_security.sql`.

## Frontend previsto

- `SyncEntityListForm : BaseGridCrudListForm`.
- `SyncEntityEditForm : BaseEditForm`.
- Grid heredado `NuanDataGridControl`.
- Botones heredados y `NuanActionButton` para acciones auxiliares.
- `NuanDataGridControl` editable para seleccionar dependencias con codigo, nombre y disponibilidad.
- Archivos `.Designer.cs` y `.resx` compatibles con Visual Studio Designer.

## Implementacion de Fase 4

- Cliente independiente `ISyncEntityDefinitionClient` sobre `INuanApiClient`.
- Modelos de listado, detalle, lookup, dependencias y requests separados de los contratos de persistencia.
- Paginacion y filtros enviados como query string codificado.
- `SyncEntityDefinitionsViewModel` para listado, detalle y eliminacion.
- `SyncEntityDefinitionEditViewModel` y estado de editor para crear, actualizar y seleccionar dependencias.
- El codigo se mantiene fuera del request de actualizacion porque es inmutable despues de crear.
- Las dependencias historicas no disponibles se conservan visibles en el estado del editor.
- El cliente queda preparado en `FrontendComposition`; no se publican formularios ni menus en esta fase.

## Implementacion de Fase 5

- `SyncEntityListForm` hereda de `BaseGridCrudListForm` y utiliza permisos CRUD independientes.
- `SyncEntityEditForm` hereda de `BaseEditForm`, reutiliza los botones heredados de 100 x 36 y no usa paneles.
- El editor administra datos generales, capacidades tecnicas, estado operativo y dependencias.
- El listado soporta consulta, creacion, edicion, eliminacion, auditoria y personalizacion de columnas.
- El endpoint de historial lee `AuditSyncConfigurationChanges` mediante
  `SP_NA_GET_SYNCENTITYDEFINITIONHISTORIAL`.
- `sync-entities` se integra en `MainForm`, `ShellViewModel` y `FrontendComposition`.
- `082_sync_entity_definition_winforms.sql` publica el menu bajo Integraciones y concede acceso a `ADMIN`.
- Los formularios fueron renderizados localmente para comprobar alineacion, tipografia y controles heredados.

## Exclusiones

- Productores o aplicadores creados dinamicamente.
- SQL o scripts personalizados.
- Mapeo avanzado de campos.
- Sincronizacion bidireccional.
- Cambios en Outbox/Inbox.

## Criterios de salida de Fase 1

- Tablas e indices creados de forma idempotente.
- Seed de las 13 definiciones existentes.
- CRUD por procedimientos almacenados.
- Dependencias sin duplicados, autorreferencias ni ciclos.
- Auditoria de cambios.
- Relacion de `SyncProfileEntities` con el catalogo.
- Instalador y diagnostico Master actualizados.
- Pruebas de contrato y prueba transaccional con rollback.
