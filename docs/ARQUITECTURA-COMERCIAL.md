# Arquitectura comercial multi-giro

Este documento define la direccion tecnica para que NuanSystem sirva como plataforma comercial configurable para distintos negocios: supermercado, ferreteria, tienda de condimentos, distribuidora, bazar, repuestos, papeleria, minimarket u otros giros con inventario, ventas, compras y caja.

## Principio rector

NuanSystem no debe codificar reglas para un solo giro. Debe implementar capacidades comerciales reutilizables y activar comportamientos por empresa mediante configuracion.

Separar claramente:

- Mantenimientos administrativos: CRUD normalizado, grillas, formularios, auditoria y permisos.
- Procesos operativos: casos de uso transaccionales con reglas de dominio, consistencia de inventario, precios, caja, compras y documentos.

No modelar ventas, cobros, inventario, promociones, compras o caja como CRUD simples.

## Modulos objetivo

- `Catalogs`: articulos, clientes, proveedores, grupos, familias, marcas, unidades, impuestos y listas base.
- `Inventory`: bodegas, stock, kardex, transferencias, ajustes, conteos, lotes, vencimientos y seriales.
- `Pricing`: listas de precios, descuentos, promociones, margenes y reglas vigentes.
- `Sales`: cotizaciones, pedidos, facturas, notas de credito, anulaciones y devoluciones.
- `Purchasing`: proveedores, ordenes de compra, recepciones, costos y cuentas por pagar iniciales.
- `Cash`: cajas, turnos, cobros, medios de pago, cierres y diferencias.
- `Documents`: documentos comerciales configurables por tipo de negocio.
- `Security`: usuarios, roles, menus, formularios, campos y operaciones.
- `Configuration`: parametros por empresa, capacidades por giro y presets.
- `Integrations`: SAP, facturacion electronica, balanzas, lectores, servicios externos y sincronizacion.

## Capacidades por empresa

Las diferencias entre negocios deben resolverse con parametros/capacidades, no con forks del codigo.

Capacidades recomendadas:

- Maneja multiples bodegas.
- Permite stock negativo.
- Maneja lotes.
- Maneja vencimientos.
- Maneja seriales.
- Maneja tallas, colores o variantes.
- Maneja multiples codigos de barra.
- Maneja multiples unidades de medida.
- Maneja venta por peso.
- Maneja listas de precio.
- Maneja promociones.
- Maneja descuento maximo por usuario/rol.
- Maneja caja rapida.
- Maneja credito de clientes.
- Maneja compras y recepcion.
- Maneja facturacion electronica.
- Maneja integracion con balanza.

Los presets de giro deben ser datos iniciales, no ramas de codigo:

- `RetailBasico`
- `Supermercado`
- `Ferreteria`
- `Distribuidora`
- `TiendaEspecializada`
- `ServiciosConInventario`

## Backend

### API

Separar endpoints por modulo en `src/Backend/NuanSystem.Api/Endpoints`.

Formato recomendado:

```text
MapCatalogEndpoints()
MapInventoryEndpoints()
MapPricingEndpoints()
MapSalesEndpoints()
MapPurchasingEndpoints()
MapCashEndpoints()
MapDocumentEndpoints()
MapSecurityEndpoints()
MapConfigurationEndpoints()
```

`Program.cs` debe quedar como composicion de middleware, inicializacion y llamadas `Map...Endpoints()`.

### Application

Usar CQRS por caso de uso.

CRUD administrativo:

```text
Catalogs/CreateBrand
Catalogs/UpdateBrand
Catalogs/DeleteBrand
```

Procesos operativos:

```text
Inventory/RegisterStockMovement
Inventory/TransferStock
Pricing/CalculateSalePrice
Sales/RegisterSale
Sales/CancelDocument
Purchasing/RegisterGoodsReceipt
Cash/OpenShift
Cash/RegisterPayment
Cash/CloseShift
```

### Domain

Mover reglas de negocio puras a `NuanSystem.Domain`:

- Calculo de totales, impuestos y redondeos.
- Politicas de stock negativo.
- Validacion de promociones vigentes.
- Reglas de anulacion/devolucion.
- Estados validos de documentos.
- Reglas de cierre de caja.
- Validacion de lote, vencimiento o serial cuando la empresa active esas capacidades.

### Persistence

Mantener procedimientos almacenados para persistencia, pero evitar esconder reglas de negocio principales en SQL cuando pertenezcan al dominio.

Usar `ITransactionRunner` para operaciones criticas:

- Venta + lineas + pagos + stock + auditoria.
- Recepcion de compra + stock + costo.
- Transferencia entre bodegas.
- Ajuste de inventario.
- Cierre de caja.

## Frontend

Separar pantallas administrativas de pantallas operativas.

- Administrativas: `BaseGridCrudListForm` y `BaseEditForm`.
- Operativas: formularios dedicados al flujo, velocidad y continuidad de trabajo.

Ejemplos de pantallas operativas:

- Venta/caja.
- Recepcion de mercaderia.
- Transferencias.
- Conteo fisico.
- Consulta rapida de precio/stock.
- Cobros y cierre de turno.

## Lookups

Todo lookup vinculado a otra tabla debe exponer codigo y nombre.

DTO recomendado para lookups administrativos:

```csharp
LookupOptionDto(int Id, string Code, string Name, bool IsActive)
```

Para operaciones rapidas, usar DTOs especificos optimizados. Ejemplo:

```text
SaleItemLookupDto: Id, Code, Barcode, Name, Price, Tax, Stock, Unit
```

No reutilizar DTOs de detalle como lookups.

## Auditoria

Mantener auditoria por dominio:

- `AuditSecurityChanges`
- `AuditInventoryChanges`
- `AuditSalesChanges`
- `AuditCashChanges`
- `AuditPricingChanges`
- `AuditPurchasingChanges`
- `AuditSystemChanges`

Registrar cambios sensibles dentro de la misma transaccion operativa cuando corresponda.

## Regla practica

Antes de crear un modulo nuevo, clasificarlo:

- Si es mantenimiento: usar el patron CRUD existente.
- Si afecta stock, dinero, precios, documentos, compras o caja: crear caso de uso operativo, revisar transaccion y documentar regla de dominio.
- Si una regla puede variar por giro, convertirla en capacidad/configuracion por empresa.
