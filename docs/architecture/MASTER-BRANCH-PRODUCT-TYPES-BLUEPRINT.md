# ProductType — maestro tenant y sincronización Matriz-Sucursal

## Alcance

`ProductType` es un maestro independiente de inventario. Clasifica el papel
comercial o productivo del artículo mediante `NatureCode`; no reemplaza
`ItemType`, no contiene cuentas contables y no configura SAP.

La fase inicial preserva los códigos existentes, no agrega `ProductTypeId` a
`Items` y no crea semillas adicionales.

## Contrato de datos

- Identidad distribuida: `GlobalId`.
- Identidad local: `Id`.
- Código de negocio: `Code`, único incluso después de eliminación lógica.
- Naturalezas cerradas: `Merchandise`, `FinishedGood`, `RawMaterial`,
  `SemiFinished`, `Supply`, `Packaging`, `ByProduct`, `Other`.
- Semillas migradas: `MERCADERIA`, `PROD_TERM`, `MAT_PRIMA`; conservan sus
  registros y se marcan `IsSystem=1`.
- Un tipo del sistema no permite cambiar `Code` ni `NatureCode`, ni eliminarse.

## Escritura y publicación

```text
comando autenticado
  -> procedimiento CRUD ProductTypes
  -> auditoría AuditCatalogChanges
  -> ProductTypeLocalOutboxWriter
  -> LocalOutbox ProductType en la misma conexión/transacción tenant
  -> commit
```

El evento usa `EntityName=ProductType`. No existe escritura directa tenant a
Master. El relay conserva el `EventId` al promoverlo.

## Full y aplicación

El origen Full pagina técnicamente por el `Id` local (`AfterId`) y publica
identidad y estado del maestro, incluidos tombstones. El cursor técnico no
cambia que `GlobalId` sea la identidad distribuida del evento y del apply.
El aplicador busca exclusivamente por `GlobalId`:

- si `GlobalId` existe, actualiza esa fila;
- si no existe y `Code` está libre, crea una fila con el `GlobalId` recibido;
- si el mismo `Code` pertenece a otro `GlobalId`, devuelve conflicto terminal;
- nunca adopta automáticamente una fila por `Code`.

La aplicación registra auditoría local en la misma transacción. No genera un
nuevo `LocalOutbox` en la sucursal receptora.

## Dependencias y activación

`ProductType` no depende de otro maestro. Se registra antes de `Item` y la
dependencia `Item -> ProductType` queda preparada, aunque el payload actual de
Item aún no incluya `ProductTypeId`.

Las nuevas filas de `SyncEntityConfigurations` y
`EntityOwnershipConfigurations` nacen deshabilitadas. La migración no activa
perfiles, rutas, relay ni workers.

## Seguridad y recuperación

El formulario canónico es `product-types` y conserva las identidades legacy
`inventory-product-types`. La migración mantiene grants y denegaciones
existentes; solo completa el rol `ADMIN`.

Ante una colisión de código, el evento debe permanecer recuperable como error
terminal/dead letter para resolución manual. No se modifica la identidad local
ni se consulta SAP como mecanismo de reparación.

## Despliegue

1. Tenant `198_tenant_product_types_master.sql`.
2. Master `199_master_definitions_inventory_product_types_navigation.sql`.
3. Backend con productor/aplicador habilitable en código.
4. Master `200_master_product_types_sync_registration.sql`.
5. Mantener toda configuración deshabilitada hasta una validación autorizada.

Los scripts son forward-only e idempotentes. No se ejecutan como parte de esta
implementación sin autorización separada de despliegue.
