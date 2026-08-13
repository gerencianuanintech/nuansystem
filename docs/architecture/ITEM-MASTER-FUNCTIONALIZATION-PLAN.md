# Plan de funcionalización del maestro de ítems

## Objetivo

Convertir el diseño actual de `ItemEditForm` en un maestro ERP funcional, sin
acoplar el dominio a SAP y sin tratar como texto libre las relaciones con
catálogos corporativos. Este documento clasifica los campos y define cuáles
deben consumir maestros auxiliares, cuáles son valores cerrados, cuáles son
colecciones propias del ítem y cuáles son resultados operativos de solo
lectura.

## Estado descubierto

- La entidad principal se guarda en `dbo.Items` y ya contiene identidad,
  clasificación básica, unidades, impuestos, trazabilidad básica, precios de
  referencia y auditoría.
- El resto del diseño se serializa actualmente en
  `dbo.ItemMasterProfiles.MasterDataJson`.
- El formulario ya carga varios catálogos auxiliares, pero no todos los campos
  visuales están enlazados a ellos.
- Existen mantenimientos para grupos, familias, unidades, bodegas, impuestos,
  cuentas y trece catálogos auxiliares de inventario.
- Los KPI, históricos, existencias, comprometido, pedido, costos calculados y
  estados de sincronización no deben ser editables ni almacenarse como verdad
  manual del maestro.
- SAP es opcional. La participación y correspondencia del ítem pueden guardarse
  localmente, pero empresa, perfil, mapeos y ejecución pertenecen a la
  configuración central de integración.

## Clasificación adoptada

| Tipo | Regla |
|---|---|
| Maestro auxiliar | El usuario administra códigos y descripciones y más de un proceso puede consumirlos. Se referencia por `Id` o identidad estable, nunca por el texto visible. |
| Valor cerrado | Conjunto pequeño que cambia reglas del sistema. Se persiste mediante código estable y no permite crear valores desde el ítem. |
| Dato propio | Valor descriptivo o parámetro que pertenece exclusivamente al ítem. |
| Colección hija | Varias filas con ciclo de vida dependiente del ítem; no es un maestro independiente. |
| Operacional/calculado | Proviene de movimientos, documentos o cálculos. Es de solo lectura. |
| Configuración/capacidad | Se gobierna por empresa o perfil y no debe repetirse libremente por ítem. |

## Campos que usarán maestros auxiliares

### Identificación y clasificación

| Campo del formulario | Maestro propietario | Estado | Decisión |
|---|---|---|---|
| Grupo | Grupos de ítems | Existe | Relación obligatoria según política de empresa. |
| Familia | Familias de ítems | Existe | Relación dependiente de Grupo; filtrar por `ItemGroupId`. |
| Marca | Marcas de artículos | Existe | Persistir `BrandId`; actualmente solo vive en el estado visual/JSON. |
| Tipo de producto | Tipos de producto | Existe | Persistir `ProductTypeId`; clasificación comercial administrable. |
| Línea | Líneas de artículos | Existe | Persistir `ItemLineId`. |
| Subgrupo | Subgrupos de artículos | Existe | Persistir `ItemSubgroupId`; conviene agregar dependencia con Grupo cuando se defina su jerarquía. |
| Unidad base/inventario/compra/venta | Unidades de medida | Existe | Referenciar la misma entidad con roles diferentes. |
| Unidad de peso/volumen | Unidades de medida | Existe | Filtrar por dimensión cuando el maestro de unidades incorpore tipo de magnitud. |

`Tipo de ítem` será un maestro administrable con identidad propia, nombre,
valores predeterminados y orden. Su `BehaviorCode` sí permanece como conjunto
semántico cerrado (`Product`, `Service`, `Supply`, `Asset` y `Kit`) porque activa
reglas internas del ERP. Varios tipos comerciales pueden compartir un mismo
comportamiento sin permitir comportamientos arbitrarios. `Tipo de producto`
continúa cubriendo una clasificación comercial diferente.

### Unidades y códigos

| Campo | Maestro propietario | Decisión |
|---|---|---|
| Unidad de cada presentación | Unidades de medida | Relación por `UnitOfMeasureId`. |
| Presentaciones | Ninguno | Colección hija del ítem. |
| Códigos de barras | Ninguno | Colección hija del ítem/presentación con unicidad tenant. |
| Origen del código | Ninguno | Valor cerrado: manual, proveedor, balanza, interno o integración. |

### Inventario

| Campo | Maestro propietario | Estado | Decisión |
|---|---|---|---|
| Bodega principal | Bodegas | Existe | Referencia por `WarehouseId`. |
| Bodegas habilitadas | Bodegas | Existe | Colección hija `ItemWarehouse`; no incluye el stock como dato manual. |
| Ubicación predeterminada | Ubicaciones de bodega | Existe | Debe depender de la bodega seleccionada. |
| Zona de almacenamiento | Zonas de almacenamiento | Existe | Persistir referencia estable, no nombre. |
| Condición de almacenamiento | Condiciones de almacenamiento | Existe | Persistir referencia estable. |
| Método de reposición | Métodos de reposición | Existe | Catálogo administrable por empresa. |

Los siguientes son valores cerrados y no maestros auxiliares: método de
valoración, política de stock negativo, método de abastecimiento
(`Purchase`, `Produce`, `Transfer`, `Consignment`) y clasificación ABC. Las
capacidades de múltiples bodegas, ubicaciones, lotes, series, vencimiento,
fracciones y stock negativo se gobiernan también por parámetros de empresa.

### Comercial - Compras

| Campo | Maestro propietario | Estado | Decisión |
|---|---|---|---|
| Proveedor principal/alterno | Socios de negocio - proveedores | Existe | Referenciar `BusinessPartnerId`; no guardar solamente el código escrito. |
| Unidad de compra | Unidades de medida | Existe | Referencia por `UnitOfMeasureId`. |
| Moneda preferida | Monedas | Existe | Referencia por `CurrencyId`. |
| Impuesto de compra | Impuestos | Existe | Referencia por `TaxId`. |
| Retención de compra | Conceptos/tipos de retención | Existe | Elegir el catálogo fiscal apropiado según el contrato tributario. |
| Cuenta de gasto | Plan de cuentas | Existe | Referencia estable a cuenta imputable. |
| Comprador asignado | Usuarios/empleados | Parcial | Reutilizar identidad corporativa; no crear un catálogo duplicado de compradores. |

Presentación preferida es una referencia a una presentación hija del mismo
ítem. Política de compra, recepción y devolución son datos propios o políticas
de empresa, no nuevos maestros en esta fase.

### Comercial - Ventas

| Campo | Maestro propietario | Estado | Decisión |
|---|---|---|---|
| Unidad de venta | Unidades de medida | Existe | Referencia por `UnitOfMeasureId`. |
| Lista principal/mínima | Listas de precios | Existe | Referencia por `PriceListId`. |
| Moneda | Monedas | Existe | Debe derivarse de la lista o referenciar `CurrencyId`. |
| Canal principal | Canales de venta | Existe | Persistir `SalesChannelId`. |
| Segmento | Segmentos comerciales de ítems | Falta | Crear maestro solo si clasifica artículos de forma reutilizable; no reutilizar segmentos de clientes. |
| Impuesto de venta | Impuestos | Existe | Referencia por `TaxId`. |

Los precios por lista son datos operativos de `PriceListItems`, no campos JSON
del ítem. Descuentos, mínimo de venta, múltiplo, comisión y observación comercial
son parámetros propios del artículo, sujetos a las capacidades de precios y
promociones de la empresa.

### Finanzas - Costos, contabilidad e impuestos

| Campo | Maestro propietario | Estado | Decisión |
|---|---|---|---|
| Moneda de costo | Monedas | Existe | Referencia por `CurrencyId`. |
| Cuentas contables | Plan de cuentas | Existe | Referencias por cuenta; solo cuentas imputables y activas. |
| Sucursal contable | Sucursales financieras | Existe | Referencia estable. |
| Centro de costo | Centros de costo | Existe | Referencia estable. |
| Proyecto | Proyectos | Existe | Referencia estable. |
| Línea de negocio | Líneas de negocio financieras | Existe | Referencia estable. |
| Departamento | Departamentos | Existe | Referencia estable. |
| IVA compra/venta | Impuestos | Existe | Referencias por `TaxId`. |
| ICE/impuesto especial | Impuestos | Existe | Usar impuesto tipificado; no lista libre. |
| Retención sugerida | Conceptos de retención | Existe | Referencia según compra/venta. |
| Sustento tributario | Sustentos tributarios | Existe | Referencia por código fiscal estable. |
| País fiscal/origen | Países | Existe | Referencia por `CountryId`; no texto libre. |
| Partida arancelaria | Partidas arancelarias | Falta/condicional | Crear solo al habilitar comercio exterior; debe incluir código, descripción y vigencia. |
| Clasificación aduanera | Clasificaciones aduaneras | Falta/condicional | Puede integrarse al maestro de partidas si no tiene ciclo de vida independiente. |

Costo promedio, último costo, margen, rentabilidad, fechas de actualización e
historial son valores calculados a partir de movimientos y precios. Costo
estándar y costo de reposición pueden ser configurables mediante un caso de uso
de costos con auditoría, no mediante guardado genérico del formulario.

### Trazabilidad y variantes

| Campo | Maestro propietario | Decisión |
|---|---|---|
| Atributos de variante | Atributos de variantes | Existe | Se consumen mediante una colección hija con valor por atributo. |
| Variante base | Ítems/variantes | No es auxiliar | Relación a otro ítem o a la entidad futura `ItemVariant`. |

Control por lote/serie/ninguno, FEFO/FIFO, método de numeración y las reglas de
expiración son valores cerrados o capacidades. Lotes, series y variantes
generadas son entidades operativas/hijas, no maestros auxiliares editables desde
un combo.

### Documentos y observaciones

| Campo | Maestro propietario | Estado | Decisión |
|---|---|---|---|
| Tipo de documento/anexo | Tipos de documento de anexos | Existe | Persistir referencia estable. |
| Categoría de anexo | Categorías de anexos | Existe | Persistir referencia estable. |
| Tipo de alerta operativa | Tipos de alerta de ítems | Falta | Crear maestro si la empresa podrá definir alertas propias y reutilizarlas. |
| Proceso de alerta | Módulos/procesos del sistema | No crear libre | Conjunto controlado para que una alerta pueda ejecutarse de forma segura. |

Estado del archivo, prioridad, visibilidad y confirmación son valores cerrados.
Archivos, imágenes, observaciones y alertas son colecciones/datos hijos del ítem,
no catálogos independientes.

### Integración

No se crearán maestros auxiliares dentro del ítem para empresa SAP, base de
destino, perfil, grupo SAP, grupo de unidades SAP, planificación, abastecimiento
o valoración SAP. Esos valores se resuelven desde:

- empresa y capacidades configuradas en Master;
- perfil de sincronización;
- mapeos de catálogos NuanSystem-SAP;
- historial técnico de ejecuciones.

El ítem solo conserva participación (`SynchronizeItem`) y correspondencia
externa (`ExternalSystem`, `ExternalCode`, `SapCode`) cuando aplique. Los mapeos
de campos no se administran por artículo.

## Nuevos maestros recomendados

No se necesita crear una tabla por cada selector. Después de reutilizar los
maestros existentes, solo quedan como candidatos reales:

1. **Segmentos comerciales de ítems**, si el negocio necesita clasificación de
   mercado independiente de línea, familia y canal.
2. **Tipos de alerta de ítems**, si las alertas serán configurables y
   reutilizables entre artículos.
3. **Partidas arancelarias**, únicamente cuando se active la capacidad de
   comercio exterior.

Antes de crear los dos primeros se debe confirmar un consumidor adicional al
maestro de ítems. Si no existe, pueden iniciar como valores cerrados y promoverse
después sin afectar el núcleo.

## Inconsistencias que deben resolverse

1. `ItemTypes.Code` usa códigos comerciales históricos como `PRODUCTO`, mientras
   `Items.ItemType` guarda comportamientos como `Product`. El maestro independiente
   conserva ambos conceptos mediante `Code` y `BehaviorCode`; la cabecera no se
   vinculará hasta agregar `Items.ItemTypeId` y migrar las referencias de forma
   explícita.
2. El formulario carga `Brands`, pero `BrandId` no forma parte del contrato de
   `ItemMasterData` ni de `dbo.Items`.
3. `ProductType`, `Line`, `SubGroup`, `ReplenishmentMethod`, `SalesChannel` y
   `WarehouseLocation` se guardan hoy como códigos/texto dentro de JSON aunque
   ya tienen tablas con `Id`.
4. Monedas y listas de precio se muestran con valores locales de ejemplo pese a
   que existen maestros funcionales.
5. Proveedores, dimensiones contables y catálogos tributarios todavía se
   muestran como texto o listas sin enlazar a sus módulos propietarios.
6. `MasterDataJson` mezcla configuración, relaciones, cálculos, históricos y
   datos operativos. No debe convertirse en la fuente definitiva del ERP.

## Modelo de persistencia objetivo

- `Items`: identidad y configuración universal del maestro.
- Relaciones mediante claves foráneas para los maestros auxiliares estables.
- Tablas hijas para presentaciones, códigos de barras, configuración por bodega,
  atributos/variantes, anexos, observaciones y alertas.
- Tablas operativas existentes o futuras para stock, costos, precios, compras,
  ventas, lotes, series e históricos.
- `ItemMasterProfiles.MasterDataJson` se mantiene temporalmente como puente de
  compatibilidad y se retira gradualmente después de migrar cada bloque.

## Orden de ejecución recomendado

1. Implementar el maestro independiente `Tipos de ítem` y después vincularlo a
   `Items` mediante `ItemTypeId`, manteniendo compatibilidad temporal con
   `Items.ItemType` como comportamiento.
2. Funcionalizar General: marca, tipo de producto, línea y subgrupo.
3. Funcionalizar Unidades y códigos con tablas hijas y validación de unicidad.
4. Funcionalizar Inventario sin mezclar existencias ni costos manuales.
5. Conectar Compras/Ventas a proveedores, monedas, listas de precio e impuestos.
6. Conectar Finanzas a cuentas, dimensiones y catálogos tributarios.
7. Implementar Trazabilidad/Variantes según capacidades por empresa.
8. Implementar Documentos/Alertas con almacenamiento de archivos aprobado.
9. Integración: solo participación/correspondencia; configuración y mapeos fuera
   del ítem.
10. Migrar datos útiles desde JSON, validar compatibilidad de sincronización de
    maestros y finalmente reducir el perfil JSON.

Cada bloque requiere contrato Application, validación, repositorio Dapper,
procedimientos SQL versionados, endpoints autorizados, cliente tipado, lookups
corporativos, pruebas y revisión del payload de sincronización. Ninguna
migración se despliega ni ningún worker se activa como parte de este documento.
