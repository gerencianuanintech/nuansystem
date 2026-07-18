# Matriz de equivalencias de catalogos SAP

## Decision

Las equivalencias entre codigos de SAP Business One y NuanSystem se configuran por empresa en `NuanSystem_Master`. No se almacenan identificadores locales de las bases tenant porque esos identificadores pueden cambiar entre Master y sucursales.

La clave funcional es:

`CompanyId + MappingType + SapCode -> NuanCode`

Tipos iniciales:

- `ItemGroup`: grupo de articulo SAP hacia codigo de grupo NuanSystem.
- `UnitOfMeasure`: unidad SAP hacia codigo de unidad NuanSystem.
- `Tax`: impuesto SAP hacia codigo de impuesto NuanSystem.

## Reglas

- La matriz pertenece a la configuracion SAP de la empresa y se guarda en Master.
- Application define el contrato; Persistence implementa SQL Server mediante procedimientos almacenados.
- Durante la importacion, el codigo Nuan se resuelve contra los catalogos de la base tenant activa.
- Si no existe equivalencia, se permite importar el maestro con la referencia nullable y se informa el faltante; no se inventa una equivalencia.
- Si los codigos SAP y Nuan coinciden, la coincidencia directa se conserva como compatibilidad.
- La sustitucion de la matriz es atomica, auditada y elimina logicamente las filas que ya no forman parte de la configuracion.
- SAP sigue siendo opcional y ningun tipo de SAP entra en Domain.

## Alcance inicial

La primera aplicacion es la importacion SAP de articulos. La estructura es extensible a otros catalogos, pero cada nuevo `MappingType` debe incorporarse de forma explicita y validada.

La distribucion Master/Sucursal sigue usando Outbox/Inbox y `GlobalId`; esta matriz solo resuelve referencias de catalogo durante el ingreso desde SAP a la empresa Master. `ItemGroups` debe distribuirse antes de `Item` para que cada sucursal pueda resolver localmente `ItemGroupCode` sin transportar identificadores locales.
