# Subgrupos de articulos: maestro y sincronizacion

## Alcance

`ItemSubgroups` es un CRUD administrativo tenant bajo
`Definitions/Inventory/ItemSubgroups`. Cada subgrupo pertenece obligatoriamente
a una familia de articulos y su codigo es unico dentro de esa familia.

El maestro no depende de SAP. NuanSystem puede crearlo y utilizarlo localmente
sin configurar una integracion externa.

## Identidad y dependencia

- `Id` identifica la fila dentro de cada base tenant.
- `GlobalId` conserva la identidad entre Matriz y sucursales.
- `ItemFamilyId` es la referencia local.
- `ItemFamilyGlobalId` resuelve la dependencia durante la sincronizacion.
- La clave funcional es `(ItemFamilyId, Code)`, incluyendo tombstones.
- Nunca se adopta una fila por coincidencia de codigo.

La secuencia de distribucion es `ItemFamilies -> ItemSubgroups`. Un evento de
subgrupo se reintenta mientras su familia aun no exista en la sucursal.

## Publicacion y aplicacion

El CRUD escribe el cambio y `LocalOutbox` dentro de la misma transaccion. El
aplicador usa `SyncInbox` para idempotencia y clasifica la colision de codigo
como conflicto terminal. La fuente Full pagina por `Id` tecnico y transporta
`GlobalId`, familia global, estado activo y baja logica.

La migracion Master registra la entidad deshabilitada por defecto. No activa
perfiles, ownership, workers ni distribuciones.

## Compatibilidad historica

La migracion tenant obtiene la familia solamente de articulos que tengan a la
vez `ItemFamilyId` y el codigo histórico en `ItemMasterProfiles`. Si un codigo
se uso en varias familias, conserva la fila original en una y crea identidades
globales nuevas para las restantes.

El seed tecnico `GENERAL` se replica por familia activa cuando no tiene uso. En
una base nueva sin familias y sin consumidores se elimina; cualquier otra fila
sin relacion inequívoca detiene el despliegue para exigir un mapeo explícito.

## Seguridad y UI

- Ruta API: `/api/definitions/inventory/item-subgroups`.
- FormKey: `item-subgroups`.
- Menu: Configuracion > Definiciones > Inventario > Subgrupos de articulos.
- Permisos existentes: `GENERALINVENTORY.ITEMSUBGROUPS.READ/MANAGE`.
- Las operaciones visibles respetan los accesos efectivos del formulario.
- El selector de familia utiliza `NuanLookupEdit` y conserva valores históricos
  inactivos sin ofrecerlos para nuevas selecciones.

## Fuera de alcance

- Activar sincronizacion o workers.
- Ejecutar llamadas SAP/SRI.
- Crear equivalencias SAP para subgrupos.
- Reescribir documentos historicos.
