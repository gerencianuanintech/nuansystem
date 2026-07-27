# Fase 8.7 — Tax transaccional Matriz-Sucursal

## Decisión

`Tax` es el catálogo independiente de tasas de impuesto utilizadas por
artículos y órdenes de compra. No representa regímenes tributarios, tipos de
contribuyente, sustentos ni retenciones.

- Propietario: TaxCatalogs.
- Tabla tenant: `dbo.Taxes`.
- Identidad entre tenants: `GlobalId`.
- Identidad local: `Id`.
- Código de sincronización: `Tax`.
- Riesgo: alto, porque afecta cálculos monetarios y es dependencia de Item y
  PurchaseOrder.
- SAP y SRI: fuera de alcance.

## Decisiones aprobadas

1. Tax tendrá CRUD y formulario independientes.
2. Campos: código, nombre, descripción, porcentaje y activo.
3. El usuario verá `15%`; persistencia y payload conservarán `0.15`.
4. Se preservan `ExternalSystem` y `ExternalCode`; no se agrega `SapCode`.
5. Una colisión de código con otro `GlobalId` es terminal y no adopta.
6. El código de un registro eliminado queda reservado.
7. Desactivar siempre es posible; eliminar se bloquea si existen Items activos.
8. WinForms reutiliza `BaseGridCrudListForm` y `BaseEditForm`.
9. CRUD y `LocalOutbox` se guardan en la misma transacción tenant.
10. El piloto futuro usará DEMO como Matriz y Remigio/Cañaris como sucursales.

## Evidencia

- `database/sql/018_inventory_items_master.sql` crea `Taxes` y los seeds
  `IVA0`/`IVA15`.
- `database/sql/063_tenant_global_ids_and_external_refs.sql` incorpora
  `GlobalId`, `ExternalSystem` y `ExternalCode`.
- `database/sql/100_tenant_purchase_reference_catalog_sync.sql` habilita Full.
- `SyncMasterBranchEntityCatalog` declara `Tax` antes de Item y PurchaseOrder.
- `TaxFullEntitySource` ya produce páginas Full.
- `ReferenceCatalogSyncApplyRepository` aplica Tax, pero reconcilia por código.
- `Items.PurchaseTaxId` y `Items.SalesTaxId` referencian `Taxes`.
- PurchaseOrder conserva la tasa aplicada en cada línea.
- No existe endpoint ni formulario administrativo específico para `Taxes`.

## Alcance

Incluye:

- CRUD tenant completo;
- permisos, menú y operaciones;
- cliente, ViewModel, listado, edición e historial corporativo;
- LocalOutbox transaccional;
- payload Tax dedicado;
- fuente Full existente adaptada al payload;
- aplicador dedicado, Inbox y conflicto terminal;
- reserva de tombstone;
- scripts idempotentes y pruebas.

Excluye:

- modificar documentos ya registrados;
- importación o consulta SAP;
- retenciones y régimen tributario;
- cambios SRI;
- activación permanente de perfiles o workers.

## Contrato funcional

| Campo | Regla |
|---|---|
| `Code` | requerido, mayúsculas, máximo 50, reservado incluso eliminado |
| `Name` | requerido, máximo 150 |
| `Description` | opcional, máximo 500 |
| `Rate` | fracción canónica entre 0 y 1 |
| `RatePercent` | representación UI entre 0 y 100 |
| `IsActive` | independiente de eliminación lógica |
| `ExternalSystem` | preservado, no editable en el CRUD inicial |
| `ExternalCode` | preservado, no editable en el CRUD inicial |

Antes de desplegar se verificará que ninguna fila tenga `Rate < 0` o `Rate > 1`.
Una anomalía detiene la migración; no se convierte automáticamente.

## Persistencia transaccional

```text
Create/Update/Delete Tax
  -> ITransactionRunner
     -> procedimiento Tax
     -> releer estado persistido
     -> LocalOutbox con el mismo IDbTransaction
  -> commit tenant
```

Si falla el writer, se revierten Tax y LocalOutbox.

- create: `Created`;
- update activo: `Updated`;
- update inactivo: `Disabled`;
- eliminación lógica: `Deleted`.

## Payload

```text
TaxSyncPayloadV1
  GlobalId
  Code
  Name
  Description
  Rate
  IsActive
  ExternalSystem
  ExternalCode
  CreatedAt
  UpdatedAt
```

No transporta IDs locales, SAP, documentos ni información SRI.

## Aplicación en sucursal

El aplicador Tax será dedicado:

1. bloquear `SyncInbox` por `EventId`;
2. devolver idempotencia si ya está Applied;
3. localizar Tax exclusivamente por `GlobalId`;
4. si el código pertenece a otro `GlobalId`, registrar DeadLetter terminal;
5. insertar o actualizar estado y referencias;
6. conservar el tombstone en `Deleted`;
7. marcar Inbox Applied en la misma transacción.

No se reutiliza la reconciliación por código del aplicador genérico.

## Eliminación y referencias

- Un Item activo con `PurchaseTaxId` o `SalesTaxId` bloquea la eliminación.
- La desactivación permanece permitida.
- Las líneas históricas de PurchaseOrder no bloquean la eliminación lógica.
- Un Tax eliminado continúa resolviéndose por `GlobalId` para idempotencia, pero
  no aparece en lookups activos.

## Frontend

### Listado

`TaxesForm : BaseGridCrudListForm`

Columnas: código, nombre, porcentaje, activo, sistema externo y código externo.

### Edición

`TaxEditForm : BaseEditForm`

- `TextEdit` para código y nombre;
- `MemoEdit` para descripción;
- `SpinEdit` para porcentaje 0–100;
- `CheckEdit` para activo;
- separación vertical corporativa;
- estructura completa en `.Designer.cs`;
- historial mediante el formulario corporativo existente.

Los valores externos no podrán sobrescribirse desde el CRUD.

## Seguridad

- `TAX.RATES.READ`
- `TAX.RATES.MANAGE`
- `FormKey`: `taxes`
- menú: Catálogos tributarios > Impuestos

El script Master no concede acceso automáticamente a roles existentes.

## Scripts reservados

- `144_tenant_tax_transactional_outbox.sql`
- `145_master_tax_transactional_registration.sql`

Se omiten 142/143 porque pertenecen al trabajo paralelo SRI TXT Import.

El script tenant deberá validar tasas, reservar códigos eliminados, crear CRUD,
consulta de referencias y apply, y ser idempotente.

El script Master deberá registrar Tax, permisos, formulario, menú y operaciones,
manteniendo perfiles y rutas deshabilitados por defecto.

## Validación

### Código

- rollback conjunto Tax/LocalOutbox;
- tasa entre 0 y 1;
- bloqueo por Item activo;
- payload saneado;
- aplicación por GlobalId;
- colisión terminal;
- idempotencia;
- tombstone.

### SQL real, con autorización separada

- respaldos verificados;
- dos pases 144/145;
- una versión por base;
- datos intactos;
- cero activaciones automáticas.

### Runtime, con autorización separada

- DEMO -> Remigio/Cañaris;
- create, update, disable y delete;
- rollback y promoción repetida;
- aplicación por GlobalId;
- colisión terminal y tombstone;
- limpieza exacta de fixtures y configuración temporal.

## Riesgos

1. Datos históricos podrían contener tasas fuera del rango fraccional.
2. El aplicador genérico actual adopta por código.
3. Índices históricos filtrados permiten reutilizar tombstones.
4. Item depende de Tax y puede impedir eliminación.
5. El contrato SAP de líneas usa otra representación; no se mezcla en esta fase.
6. El historial depende del contrato corporativo de auditoría de inventario y
   debe comprobarse en runtime con un registro real.

## Implementación estática

La implementación quedó completada en cuatro commits lineales:

- `723212f1` — discovery y decisiones;
- `1d470bbf` — CRUD transaccional, LocalOutbox y aplicador dedicado;
- `bf0de76f` — scripts tenant 144 y Master 145;
- `a43a9517` — WinForms independiente y pruebas.

La compilación completa finalizó con cero errores y cero advertencias. Las
pruebas focalizadas Tax aprobaron 7/7 y la suite completa 546/546 ejecutadas,
con cinco diagnósticas omitidas por infraestructura.

No se ejecutó SQL, no se activaron perfiles, rutas o workers y no se llamó a
SAP ni SRI. La siguiente puerta es el despliegue idempotente de 144/145 con
respaldos verificados y autorización separada.

## Criterio de cierre

La Fase 8.7 solo se aprobará cuando CRUD/LocalOutbox sean atómicos, Full e
incremental compartan payload, las sucursales resuelvan por GlobalId, ninguna
colisión adopte códigos, el tombstone permanezca reservado, frontend y Designer
aprueben, se restaure toda configuración y no queden fixtures o eventos.
