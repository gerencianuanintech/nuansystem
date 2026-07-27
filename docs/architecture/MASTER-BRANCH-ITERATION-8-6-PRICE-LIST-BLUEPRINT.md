# Fase 8.6 — PriceList transaccional Matriz-Sucursal

## 1. Estado

- **Tipo:** contrato aprobado e implementación en curso.
- **Rama:** `refactor/codex-skills-v8-6-price-list`.
- **Base:** `master` en `a5ce921ff478f594dc35f6d960ac48e843f4cee6`.
- **Dominio:** Pricing / FinancialCatalogs / PriceList.
- **Riesgo:** alto. PriceList afecta precios y es referencia de socios de negocio,
  artículos y documentos de compra.
- **SQL ejecutado:** no.
- **Workers o relay activados:** no.
- **Llamadas SAP/SRI:** no.
- **Aptitud para implementación:** aprobada con las nueve decisiones del
  apartado 16. El despliegue SQL y la validación runtime conservan gates de
  autorización independientes.

## 2. Resultado del discovery

PriceList ya tiene tabla, CRUD genérico, formulario WinForms, permisos, fuente
Full y aplicador Matriz-Sucursal. Sin embargo, el recorrido no cumple todavía
el límite transaccional de la Iteración 8 ni el contrato de dependencia por
`GlobalId`.

Los hallazgos principales son:

1. El CRUD actual guarda la entidad tenant sin `LocalOutbox`.
2. El frontend y el contrato HTTP solo exponen `Code`, `Name`, `Description` e
   `IsActive`.
3. Los campos persistidos `CurrencyCode`, `AppliesTo` e `IsDefault` no se
   muestran ni se envían. Al crear se usan los valores predeterminados
   `USD`/`Both`/`false`; al editar se vuelven a aplicar esos valores
   predeterminados, incluso si la fila tenía otros valores.
4. La fuente Full publica `CurrencyCode`, pero no la identidad global de la
   moneda.
5. El aplicador localiza primero por `GlobalId` y, si no encuentra, adopta una
   fila por `Code`. Este comportamiento contradice el contrato aprobado de
   conflicto terminal sin adopción automática.
6. El índice de código de `PriceLists` es filtrado por `IsDeleted = 0`, por lo
   que permite reutilizar el código de un tombstone.
7. El catálogo C# declara únicamente `PriceList -> Currencies`, mientras la
   migración Master `099` también registra `PriceList -> Item`. La entidad
   actual representa la cabecera de una lista, no precios por artículo, por lo
   que esa dependencia adicional no tiene respaldo en el payload actual.
8. La pantalla de edición hereda `BaseEditForm`, pero declara botones
   `SimpleButton` propios en lugar de reutilizar los `NuanActionButton`
   corporativos heredados.
9. No existen pruebas específicas del productor transaccional ni de las reglas
   de aplicación de PriceList. Las pruebas actuales solo acreditan registro
   general en el catálogo y uso del aplicador genérico.

## 3. Discovery Record

### Outcome

Preparar PriceList como catálogo administrativo de Pricing con persistencia
tenant y publicación transaccional, dependencia Currency resuelta por
`GlobalId`, aplicación idempotente en sucursales y conflicto terminal por
código.

### Work type

- CRUD administrativo para mantenimiento.
- Proceso operacional para publicación y réplica Matriz-Sucursal.

La parte de sincronización no se tratará como CRUD genérico.

### Explicit domain decisions and exclusions

- NuanSystem sigue siendo la fuente local; SAP no forma parte de 8.6.
- SRI no tiene relación con PriceList.
- No se incluyen precios por artículo, costos, descuentos, promociones,
  vigencias ni escalas.
- No se modifica PurchaseOrder, BusinessPartner ni Item en esta fase.
- La identidad entre tenants es `GlobalId`; `PriceListId` continúa siendo local.

### Affected layers

Application, Persistence, API, SQL tenant, SQL Master, WinForms services,
WinForms forms/Designer, sincronización, pruebas y documentación.

### Evidence inspected

- `database/sql/031_tenant_commercial_pricing_catalogs.sql` — tabla, seeds y
  procedimientos CRUD actuales.
- `database/sql/063_tenant_global_ids_and_external_refs.sql` — `GlobalId`,
  referencias externas e índices de PriceList.
- `database/sql/099_master_sync_dependency_engine.sql` — dependencias
  registradas en Master.
- `database/sql/100_tenant_purchase_reference_catalog_sync.sql` — contrato
  alterno de creación de la tabla y fuente de diferencias de longitud/default.
- `database/sql/104_master_demo_purchase_order_pilot_profile.sql` y
  `105_master_activate_reference_and_purchase_order_sync.sql` — perfil piloto y
  activación histórica.
- `database/sql/137_master_currency_transactional_registration.sql` —
  preservación reciente de `PriceList -> Currencies`.
- `Application/Features/FinancialCatalogs/Catalogs` — CRUD genérico y límite
  transaccional exclusivo de Currency.
- `Persistence/Repositories/FinancialCatalogs/FinancialCatalogRepository.cs` —
  procedimientos Dapper y overloads transaccionales existentes.
- `Application/Features/Sync/Configuration/SyncMasterBranchEntityCatalog.cs` —
  PriceList operativa, orden 230 y dependencia Currency.
- `Persistence/Repositories/Sync/ReferenceCatalogFullEntitySources.cs` — fuente
  Full actual por código.
- `Persistence/Repositories/Sync/ReferenceCatalogSyncApplyRepository.cs` —
  aplicación genérica, adopción por código e Inbox.
- `MasterBranchSyncWorker/Services/ReferenceCatalogSyncEventApplier.cs` —
  dispatcher/aplicador registrado.
- `Api/Endpoints/FinancialCatalogEndpoints.cs` — rutas y permisos actuales.
- `WinForms.Forms/FinancialCatalogs/PriceLists` — listado y edición existentes.
- `WinForms.Services/FinancialCatalogs` — cliente y modelos genéricos.
- `database/sql/033_master_financial_catalogs_security.sql` y
  `PermissionCodes.cs` — menú, formulario y permisos existentes.
- consumidores `BusinessPartners`, `Items` y `PurchaseOrders` — impacto de
  desactivación/eliminación y preservación histórica.

### Selected pattern

- Transactional Outbox de Iteración 8.
- Currency 8.5 como referencia inmediata para un catálogo financiero.
- UnitOfMeasure, ItemGroup y Warehouse como referencia de colisión terminal y
  reserva de tombstone.
- PriceList conserva su formulario dedicado, pero deja de usar contratos de
  datos genéricos que ocultan campos de negocio.

### Permitted reuse boundary

Se reutilizan:

- `ITransactionRunner`;
- `LocalOutbox` y relay idempotente;
- fuente Full y pipeline SyncOutbox/SyncInbox;
- `BaseGridCrudListForm`;
- `BaseEditForm`;
- `NuanActionButton`;
- `NuanLookupEdit` para Currency;
- `INuanApiClient`;
- permisos, menú y FormKey existentes.

No se reutilizan como verdad de PriceList:

- DTO genérico que omite moneda, aplicación y valor predeterminado;
- adopción genérica por código;
- identidad local de Currency;
- contratos SAP o SRI.

### Confidence

Alta para el patrón transaccional, el límite de integración y las reglas de
negocio aprobadas.

## 4. Contrato funcional actual

La tabla contiene:

| Campo | Estado actual |
|---|---|
| `PriceListId` | identidad local |
| `GlobalId` | identidad entre tenants |
| `Code` | código administrable |
| `Name` | nombre |
| `Description` | descripción |
| `CurrencyCode` | moneda por código local |
| `AppliesTo` | `Sales`, `Purchasing` o `Both` |
| `IsDefault` | indicador sin regla de unicidad |
| `IsActive` / `IsDeleted` | estado y eliminación lógica |
| `ExternalSystem` / `ExternalCode` / `SapCode` | referencias externas |
| auditoría | creación, actualización y eliminación |

La implementación 8.6 debe mantener compatibilidad con consumidores que hoy
usan `PriceListId` o `PriceListCode`. No debe convertir esas referencias locales
en `GlobalId` fuera del transporte Matriz-Sucursal.

## 5. Contrato objetivo recomendado

### 5.1 DTO y API dedicados

PriceList debe tener contratos dedicados de lista, detalle y guardado:

```text
Id
GlobalId
Code
Name
Description
CurrencyCode
CurrencyName
AppliesTo
IsDefault
IsActive
ExternalSystem
ExternalCode
audit
```

El request de creación/actualización debe incluir:

```text
Code
Name
Description
CurrencyCode
AppliesTo
IsDefault
IsActive
```

No se recomienda ampliar el DTO genérico de todos los catálogos financieros,
porque introduciría campos de PriceList en bancos, monedas y catálogos no
relacionados.

### 5.2 Dependencia Currency

Recomendación:

- mantener `CurrencyCode` como representación local para compatibilidad;
- validar en create/update que exista una Currency activa y no eliminada;
- incluir `CurrencyGlobalId` en el payload de sincronización;
- en sucursal, resolver `CurrencyGlobalId` a la Currency local y persistir su
  `Code`;
- si la Currency no está aplicada todavía, devolver error de dependencia
  reintentable;
- no resolver ni adoptar Currency por código durante la aplicación de
  PriceList.

No se recomienda agregar `CurrencyId` a PriceLists dentro de 8.6, porque
obligaría a migrar consumidores históricos sin aportar identidad de transporte
adicional.

### 5.3 Payload recomendado

```text
PriceListSyncPayloadV2
  GlobalId
  Code
  Name
  Description
  CurrencyGlobalId
  CurrencyCode (solo evidencia/diagnóstico, no resolución)
  AppliesTo
  IsDefault
  IsActive
  ExternalSystem
  ExternalCode
  CreatedAt
  UpdatedAt
```

La operación se deriva del evento: `Created`, `Updated`, `Disabled` o
`Deleted`.

## 6. Límite transaccional

```text
API
  -> handler PriceList
     -> ITransactionRunner
        -> validar unicidad y Currency
        -> guardar PriceList
        -> releer estado autoritativo
        -> escribir LocalOutbox con el mismo connection/transaction
        -> commit tenant
  -> relay deshabilitado por defecto
     -> promoción idempotente por EventId a Master
  -> MasterBranchSyncWorker
     -> aplicar por GlobalId + SyncInbox
```

Un fallo del writer debe revertir PriceList. Una indisponibilidad posterior de
Master no debe revertir el commit tenant; el evento queda en LocalOutbox para
reintento.

## 7. Aplicación en sucursal

Reglas recomendadas:

1. Buscar exclusivamente por `GlobalId`.
2. Bloquear por transacción la decisión de código.
3. Si el mismo `Code` pertenece a otro `GlobalId`, crear/actualizar SyncInbox
   como `DeadLetter` terminal.
4. No adoptar seeds, filas activas ni tombstones por código.
5. Resolver Currency exclusivamente por `CurrencyGlobalId`.
6. Insertar/actualizar PriceList e Inbox en una sola transacción.
7. Una repetición del mismo `EventId` debe ser idempotente.
8. Un tombstone conserva el código reservado.

Se recomienda reemplazar la rama PriceLists del aplicador genérico por un
repositorio/procedimiento dedicado. Esto evita alterar Tax,
BusinessPartnerPaymentTerms y otros contratos compartidos.

## 8. SQL previsto, no ejecutado

La numeración `138`/`139` fue reservada por el módulo SRI TXT desarrollado en
una rama independiente. Para evitar una colisión al integrar ambas líneas,
PriceList utilizará:

- `140_tenant_price_list_transactional_outbox.sql`;
- `141_master_price_list_transactional_registration.sql`.

El script tenant deberá:

- validar contratos previos y datos inesperados;
- alinear longitudes y el conjunto cerrado de `AppliesTo`;
- reservar código después de eliminación lógica;
- validar la referencia Currency;
- crear procedimientos transaccionales para CRUD y aplicación;
- producir payload con `CurrencyGlobalId`;
- conservar referencias externas;
- no habilitar workers ni perfiles.

El script Master deberá:

- registrar el contrato transaccional deshabilitado por defecto;
- preservar únicamente dependencias aprobadas;
- no activar rutas ni ownership;
- registrar versión idempotente.

No se debe desplegar hasta auditar en lectura los datos reales, especialmente:

- códigos duplicados activos o tombstone;
- valores `AppliesTo` fuera del conjunto aprobado;
- `CurrencyCode` sin Currency correspondiente;
- más de un `IsDefault` según la regla que se apruebe;
- seeds iguales con `GlobalId` diferentes entre Matriz y sucursales.

## 9. Frontend

### Listado

Mantener `PriceListsForm : BaseGridCrudListForm` y mostrar:

- código;
- nombre;
- moneda;
- aplica a;
- predeterminada;
- activo.

### Edición

Mantener `PriceListEditForm : BaseEditForm`, pero:

- reutilizar los `NuanActionButton` heredados;
- no declarar botones `SimpleButton` paralelos;
- usar `NuanLookupEdit` para Currency con código y nombre;
- usar un selector cerrado para `AppliesTo`;
- exponer `IsDefault` e `IsActive`;
- mantener todos los controles explícitos en `.Designer.cs`;
- conservar el espaciado vertical corporativo y compatibilidad con Designer;
- no crear `HttpClient`, SQL, SAP ni reglas de sincronización en el formulario.

Currency debe cargarse mediante el cliente API tipado y el lookup debe
seleccionar el registro actual al editar.

## 10. Seguridad y navegación

Se conservan inicialmente:

- `FINANCIAL.PRICELISTS.READ`;
- `FINANCIAL.PRICELISTS.MANAGE`;
- FormKey `price-lists`;
- menú y formulario existentes.

No se propone un permiso nuevo para sincronización desde el mantenimiento.
Relay, monitor y recuperación mantienen sus permisos Sync actuales.

## 11. Dependencias y eliminación

Consumidores encontrados:

- BusinessPartner usa `PriceListCode`;
- PurchaseOrder usa `PriceListId`;
- Item contiene referencias de presentación/configuración a listas de precios;
- PurchaseOrder depende de PriceList en el planner.

Los documentos históricos no deben perder la referencia por una eliminación
lógica. La política para impedir o permitir eliminación cuando existan
referencias activas queda pendiente de aprobación.

## 12. Árbol de decisión

```text
¿Es cambio CRUD de PriceList?
  -> guardar PriceList + LocalOutbox en la misma transacción.

¿CurrencyGlobalId existe en la sucursal?
  No -> dependencia reintentable; no aplicar PriceList.
  Sí -> continuar.

¿GlobalId de PriceList ya existe?
  Sí -> actualizar el mismo registro.
  No -> ¿Code pertenece a otro GlobalId, activo o eliminado?
          Sí -> DeadLetter terminal; no adoptar.
          No -> insertar.

¿Evento ya está Applied?
  Sí -> responder idempotente sin segunda escritura.
```

## 13. Matriz de capas

| Capa | Estado previsto |
|---|---|
| Domain | verificar; crear regla pura solo si `IsDefault` lo requiere |
| Application | cambiar a contratos/handlers dedicados y LocalOutbox |
| Persistence | repositorio PriceList dedicado o extensión aislada |
| API | endpoints dedicados bajo la ruta existente |
| Database tenant | migración 140 |
| Database Master | migración 141 |
| Frontend services | modelos y cliente PriceList dedicados |
| Frontend forms/Designer | completar campos y controles corporativos |
| Security/menu | verificar sin nuevos permisos |
| Sync | payload v2, fuente, dependencia y aplicador dedicado |
| Tests | agregar unitarias, contratos SQL y runtime controlado |
| Documentation | actualizar catálogos, graph y plan de validación |

## 14. Validación futura

### Automática

- create/update/disable/delete generan LocalOutbox dentro de la transacción;
- fallo del writer revierte PriceList;
- payload contiene `CurrencyGlobalId`;
- no existe publicación directa a Master;
- aplicación resuelve Currency por GlobalId;
- colisión de código y tombstone son terminales;
- repetición de EventId es idempotente;
- deserialización y formulario incluyen todos los campos;
- permisos 401/403/200;
- build, pruebas completas y Designer.

### SQL

- respaldos verificados;
- scripts ejecutados dos veces;
- una sola versión por base;
- constraints, índices y procedimientos esperados;
- cero cambios con workers deshabilitados;
- ninguna adopción automática de seeds.

### Runtime

Con fixtures identificables:

- create;
- update;
- disable;
- delete lógico;
- rollback atómico;
- promoción idempotente;
- aplicación en cada sucursal piloto aprobada;
- dependencia Currency ausente/retrasada;
- tombstone;
- colisión terminal;
- restauración exacta de configuración;
- limpieza de fixtures y procesos.

## 15. Riesgos

1. Editar una lista actual desde la UI puede hoy sobrescribir silenciosamente
   Currency/AppliesTo/IsDefault con defaults.
2. Seeds `LP1`/`LP2` pueden compartir código y tener distintos `GlobalId` entre
   tenants; el contrato correcto los tratará como conflicto, no como adopción.
3. Cambiar el aplicador genérico puede afectar Tax, UnitOfMeasure o PaymentTerm;
   por eso se recomienda aislar PriceList.
4. La dependencia `PriceList -> Item` de `099` contradice el modelo actual.
5. `IsDefault` carece de una regla transaccional de unicidad.
6. No existe FK entre PriceList y Currency; hay riesgo de códigos huérfanos.
7. La eliminación afecta selecciones futuras de BusinessPartner y
   PurchaseOrder.
8. Los scripts `031` y `100` difieren en longitudes y en el default
   `Both`/`All`; la migración debe abortar ante valores inesperados.
9. La pantalla actual duplica botones del formulario base y no usa el lookup
   corporativo de Currency.

## 16. Decisiones aprobadas por el propietario

1. **Campos CRUD:** el formulario administra `Code`, `Name`, `Description`,
   `Currency`, `AppliesTo`, `IsDefault` e `IsActive`.
2. **Regla de predeterminada:** existe una sola lista predeterminada efectiva
   para Sales y una para Purchasing. Una lista `Both` ocupa ambos ámbitos.
3. **Moneda:** se mantiene `CurrencyCode` local, se transporta
   `CurrencyGlobalId` y no se agrega `CurrencyId`.
4. **Dependencia Item:** se retira mediante migración forward la dependencia
   histórica `PriceList -> Item`; se conserva exclusivamente
   `PriceList -> Currencies`.
5. **Colisión:** un código activo o tombstone perteneciente a otro `GlobalId`
   produce conflicto terminal sin adopción automática.
6. **Eliminación:** se bloquea cuando existen referencias operativas activas;
   en caso contrario se ejecuta eliminación lógica y se preservan referencias
   históricas.
7. **Referencias externas:** se preservan
   `ExternalSystem`/`ExternalCode`/`SapCode`, no se editan en UI y no se llama
   SAP.
8. **Pilotos:** DEMO es Matriz; Remigio y Cañaris son sucursales piloto.
9. **Modo:** Incremental para CRUD y Full para reconciliación manual; ambos
   permanecen deshabilitados por defecto.

## 17. Plan de implementación propuesto

1. `docs(sync): define PriceList transactional discovery`
2. `feat(price-list): add complete CRUD contracts and corporate UI`
3. `feat(price-list): write CRUD and local outbox atomically`
4. `fix(price-list): resolve Currency by GlobalId and reject code adoption`
5. `feat(sql): add PriceList transactional sync contracts`
6. `test(price-list): verify atomic outbox and terminal conflicts`
7. `docs(sync): record PriceList SQL deployment`
8. `docs(sync): record PriceList runtime validation`

La implementación, despliegue SQL, activación temporal y piloto runtime requieren
autorizaciones posteriores y separadas.
