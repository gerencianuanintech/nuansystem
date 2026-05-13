# Patron de mantenimientos WinForms

Los mantenimientos del frontend siguen una estructura fija por modulo.

## Nombres

Para un catalogo plural como `Items`:

- `ItemsForm`: listado y acciones principales.
- `ItemEditForm`: creacion y edicion.
- `ItemsViewModel`: logica de pantalla.
- `IItemClient` / `ItemClient`: llamadas HTTP a la API.
- `CreateItemCommand`, `UpdateItemCommand`, `DeleteItemCommand`: casos de uso backend.
- `CreateItemCommandValidator`, `UpdateItemCommandValidator`, `DeleteItemCommandValidator`: reglas backend.

Para otro catalogo se mantiene la misma forma:

- `CustomersForm`
- `CustomerEditForm`
- `CustomersViewModel`
- `ICustomerClient` / `CustomerClient`
- `CreateCustomerCommand`, `UpdateCustomerCommand`, `DeleteCustomerCommand`

## Formularios

Los formularios de edicion heredan de `BaseEditForm<TRequest>` y solo implementan:

- `ValidateForm()`
- `BuildRequest()`

Los listados CRUD heredan de `BaseCrudListForm` cuando no necesitan grid estandar, o de `BaseGridCrudListForm` cuando usan DevExpress GridControl.

`BaseGridCrudListForm` es la plantilla recomendada para mantenimientos. Incluye:

- grid con busqueda visible
- seleccion multiple con checkbox
- paginacion local
- contador de seleccionados y total
- franja compacta de auditoria basica
- acciones ejecutadas desde el Ribbon

El formulario concreto solo debe:

- cargar datos y llamar `SetGridData(items)`
- obtener el registro actual con `SelectedGridItem<T>()`
- implementar creacion, edicion y eliminacion
- configurar permisos con `ConfigureCrudPermissions(...)`

Primeros mantenimientos con esta base:

- `OperationsForm`
- `MenusForm`

## Permisos

En WinForms se controla la experiencia deshabilitando acciones segun la sesion actual.

Para permisos granulares por operacion:

- `Read`: permite actualizar/listar.
- `Create`: permite nuevo.
- `Update`: permite editar.
- `Delete`: permite eliminar.

Mientras el backend usa permisos `READ` y `MANAGE`, las operaciones `Create`, `Update` y `Delete` se mapean a `MANAGE`.

La API sigue siendo la autoridad final: aunque un boton este visible por error, los endpoints deben validar permisos con `RequirePermission(...)`.
