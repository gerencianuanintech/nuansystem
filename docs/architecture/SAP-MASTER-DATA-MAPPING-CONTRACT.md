# Contrato de mapeo de datos maestros SAP

## Alcance

Este contrato define la configuración inicial de sincronización para Proveedores, Clientes e Ítems. La configuración pertenece a Master y cada ejecución toma una versión inmutable de consulta y mapeo.

## Identidad

- `GlobalId` identifica el registro dentro de NuanSystem.
- `Code` es el código interno de NuanSystem y no se sustituye por un código SAP.
- `SapCardCode` identifica al socio de negocio en SAP.
- Para ítems, `ItemCode` de SAP se conserva como `SapCode`/`ExternalCode`; el código interno se genera según las reglas de NuanSystem.
- Cliente y proveedor son roles distintos y no generan automáticamente dos registros.

## Direcciones

- `SapToErp`: SAP es el origen de los campos configurados para importación.
- `ErpToSap`: NuanSystem es el origen de los campos configurados para exportación.
- `Both` solo puede activarse cuando existen y están habilitados el lector/importador y el sender/exportador ejecutables para la entidad.
- `Generated` identifica valores calculados localmente, como el código interno.

## Proveedores

| Campo SAP | Campo NuanSystem | Dirección | Transformación | Regla |
|---|---|---|---|---|
| `CardCode` | `SapCardCode` | SAP → ERP | `Trim` | Solo lectura de identidad SAP |
| — | `Code` | Generado | `GenerateInternalCode` | No usar `CardCode` como código interno |
| `CardName` | `Name` | Ambas | `Trim` | Obligatorio |
| `LicTradNum` | `IdentificationNumber` | SAP → ERP | `TaxIdNormalize` | Obligatorio para importación |
| `Phone1` | `Phone` | Ambas | `Trim` | Opcional |
| `E_Mail` | `Email` | Ambas | `LowerInvariant` | Opcional |
| `Currency` | `PreferredCurrencyCode` | SAP → ERP | `CatalogLookup` | Validar catálogo |
| `ValidFor` | `IsActive` | SAP → ERP | `SapBooleanYN` | No borrar físicamente |

Prefijos aprobados:

- Nacional: `PL` o `P`.
- Extranjero: `PE`.
- Tipo tributario `PASAPORTE`: extranjero.

## Clientes

Usan la misma separación de identidad, pero con rol Cliente:

- Nacional: `CN` o `C`.
- Extranjero: `CE`.
- Tipo tributario `PASAPORTE`: extranjero.

La creación de un cliente no crea automáticamente un proveedor.

## Ítems

El conjunto inicial de campos incluye código, nombre, grupo, unidades, código de barras, impuestos de compra/venta, indicadores de compra/venta/inventario, gestión de series/lotes, tipo y estado. Los catálogos relacionados se resuelven por identificador estable y no por nombre únicamente.

## Resultados de ejecución

Cada registro puede terminar como `Created`, `Updated`, `Unchanged`, `Conflict`, `Skipped`, `Retry` o `Failed`. La ejecución conserva entidad, dirección, versión del mapeo, consulta, empresa, correlación, evento, conteos, watermark y auditoría sin guardar credenciales, cookies ni tokens SAP.

