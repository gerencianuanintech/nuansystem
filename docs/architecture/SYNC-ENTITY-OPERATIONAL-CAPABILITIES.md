# Capacidades operativas de entidades de sincronizacion

## Proposito

Definir una unica regla para distinguir una definicion administrativa de una entidad realmente ejecutable en el flujo Master-Sucursal.

## Tres niveles

1. `SyncEntityDefinitions` conserva el catalogo administrable: codigo, nombre, orden, capacidades configurables y dependencias.
2. `SyncMasterBranchEntityCodes.InitialCatalog` es el manifiesto de soporte incluido en la version desplegada. Declara si existe productor y aplicador implementado.
3. `MasterBranchSyncWorker:EnabledEntityAppliers` habilita por ambiente los aplicadores ya implementados. No agrega soporte a entidades nuevas.

Una entidad es **operativa** cuando el manifiesto declara productor y aplicador. Para una ejecucion Full, ademas debe existir un `ISyncFullEntitySource` registrado. En el Worker, el aplicador debe estar registrado y habilitado por configuracion.

## Entidades operativas actuales

- `Countries`: catalogo de paises con identidad `GlobalId`; incluye codigo, nombre, ISO, prefijo telefonico y estado. No incluye provincias ni ciudades.
- `Provinces`: catalogo de provincias con identidad `GlobalId` y dependencia por `CountryGlobalId`; no incluye ciudades.
- `Cities`: catalogo de ciudades con identidad `GlobalId` y dependencias por `CountryGlobalId` y `ProvinceGlobalId`; valida que la provincia pertenezca al pais del payload.
- `Currencies`: catalogo de monedas con identidad `GlobalId`; incluye codigo, nombre, simbolo, descripcion, indicador de moneda base, estado y referencias externas opcionales.
- `BusinessPartner`: maestro limitado de socios de negocio.
- `ItemGroups`: catalogo de grupos de articulos con identidad `GlobalId`, referencias contables y SAP opcionales; es dependencia obligatoria de `Item`.
- `Item`: maestro de articulos, sin stock, precios, costos ni movimientos.
- `Warehouse`: maestro de bodegas, sin existencias, kardex, costos ni transferencias.

## Definiciones personalizadas

El CRUD permite registrar definiciones personalizadas para preparar configuracion y dependencias. Una definicion personalizada no crea productores, lectores Full ni aplicadores en tiempo de ejecucion.

Mientras no exista una implementacion registrada en codigo:

- Puede guardarse en un perfil inactivo como borrador.
- No puede activar ni ejecutar un perfil.
- Debe mostrarse como no operativa en API y WinForms.

La primera version no admite SQL escrito por usuarios, scripts personalizados ni un disenador avanzado de mapeos.

## Prevencion de divergencias

- Los codigos de fuentes Full y aplicadores deben reutilizar `SyncMasterBranchEntityCodes`.
- La ejecucion administrativa debe consultar las fuentes Full registradas, no mantener otra lista manual.
- La validacion del perfil debe usar las capacidades entregadas por `ISyncEntityCatalogService`.
- El Worker debe conservar la doble proteccion: aplicador registrado y nombre incluido en `EnabledEntityAppliers`.

## Extension futura

Agregar una entidad operativa requiere una entrega de codigo que incluya productor o fuente, aplicador idempotente, contratos de payload versionados, pruebas, documentacion de alcance y habilitacion controlada en el Worker.
