# Integración bidireccional SAP Business One–NuanSystem

## Estado, autoridad y propósito

- **Fecha:** 2026-08-28.
- **Estado:** bloques 1 y 2 completos en código y verificación automatizada; gate runtime/operativo pendiente. Los bloques 3 a 7 no están implementados.
- **Alcance de este documento:** arquitectura transversal, propiedad de datos, identidad de socios, flujo Sucursal–Master–SAP, colas, conflictos, migración y orden de implementación.
- **Autoridad:** `AGENTS.md`, Constitución, Kernel, arquitectura general de NuanSystem, este documento y, finalmente, los planes de implementación aprobados por fase.

Este documento define cómo integrar SAP Business One en ambas direcciones sin convertirlo en dependencia del producto. NuanSystem debe continuar operando como ERP independiente cuando `SapIntegrationMode = None` o cuando SAP esté temporalmente indisponible.

La primera entrega funcional será el flujo de clientes y proveedores. Entradas de mercancía y ofertas de venta conservan las decisiones funcionales registradas aquí, pero tendrán especificaciones y planes independientes antes de su implementación.

## Terminología de topología

Para evitar ambigüedad, se usan estos nombres:

- **Master de gobierno:** base `NuanSystem_Master`. Gobierna empresas, jerarquías, conexiones, capacidades, perfiles, configuración SAP, routing y monitoreo central. No contiene la operación comercial de los socios.
- **Tenant central:** base operativa de la empresa marcada como `IsMaster = true`. Contiene la versión canónica de clientes, proveedores y demás datos comerciales de la empresa.
- **Tenant sucursal:** base operativa de una empresa hija marcada como `IsMaster = false`. Captura operación local y no contiene credenciales SAP.
- **SAP:** SAP Business One asociado opcionalmente a la empresa central.

## Decisiones no negociables

1. WinForms solo consume la API REST.
2. Ninguna sucursal llama directamente a SAP ni conoce sus credenciales.
3. Ninguna transacción tenant mantiene abierta una conexión a Master de gobierno o a SAP.
4. La replicación interna usa `LocalOutbox`, `SyncOutbox`, Inbox, idempotencia y auditoría.
5. La entrega a SAP usa `SapSyncOutbox` y `NuanSystem.SyncWorker`; nunca reutiliza `SyncOutbox` ni `NuanSystem.MasterBranchSyncWorker`.
6. `Domain` permanece libre de tipos, DTOs, estados y dependencias SAP.
7. Los workers, perfiles y capacidades nuevas permanecen deshabilitados por defecto hasta aprobar cada piloto.
8. Un reintento nunca crea un segundo socio o documento.
9. Un conflicto nunca se resuelve mediante sobrescritura silenciosa.
10. La desactivación o caída de SAP no invalida una operación local ya confirmada; la intención pendiente permanece durable y visible.

## Enfoques evaluados

### Enfoque seleccionado: base primero y entregas verticales

Primero se corrige identidad, versionamiento, Sucursal → tenant central y la cola SAP. Después se completa un vertical de socios de extremo a extremo y solo entonces se incorporan documentos.

Ventajas:

- evita consolidar el modelo incompatible actual;
- permite probar idempotencia y conflictos con un agregado conocido;
- mantiene dos pipelines observables y recuperables;
- produce entregas pequeñas que pueden activarse por empresa.

### Alternativa rechazada: completar primero los imports existentes

Agregar clientes, precios y agendas antes de corregir socios daría resultados visibles más rápido, pero conservaría `Code = CardCode`, la unicidad incorrecta de identificación y la conversión automática a `Both`.

### Alternativa rechazada: implementar todos los flujos en una sola liberación

Mezclar socios, órdenes, recepciones, lotes, series, precios, impuestos y ofertas en un solo cambio impediría validar los límites transaccionales por separado y elevaría el riesgo de duplicados o movimientos contables no reversibles.

## Matriz de fuente y propiedad

| Información | Origen operativo | Propietario canónico | Destinos |
|---|---|---|---|
| Alta o edición de cliente/proveedor | Sucursal o tenant central | Tenant central | SAP y todas las sucursales |
| Cambio entrante de socio realizado en SAP | SAP | Tenant central después de reconciliación | Todas las sucursales; SAP si queda una corrección pendiente |
| Bodegas | SAP | SAP | Tenant central y sucursal asignada |
| Artículos | SAP | SAP | Tenant central y sucursales |
| Condiciones de pago, listas de precios e impuestos usados por SAP | SAP | SAP | Tenant central y sucursales |
| Órdenes de compra | SAP | SAP | Tenant central y sucursal enrutada por bodega |
| Entrada de mercancía de compra | Sucursal | NuanSystem hasta entrega; SAP confirma identidad final | Tenant central, SAP y sucursales autorizadas |
| Oferta de venta | Sucursal | NuanSystem hasta entrega; SAP confirma identidad final | Tenant central y SAP; visibilidad local según autorización |

`Id` es siempre local. `GlobalId` conserva identidad NuanSystem entre bases. Los identificadores SAP son referencias externas y no sustituyen ninguna de esas identidades.

## Identidad de clientes y proveedores

### Identidades separadas

Cada socio tiene:

- `Id`: clave técnica local del tenant.
- `GlobalId`: identidad global NuanSystem, generada en el punto de creación y conservada en toda réplica.
- `Code`: código interno NuanSystem no editable y no semántico.
- identificación original: valor mostrado y auditado tal como lo ingresó el usuario.
- identificación normalizada: valor utilizado para validación y generación de código SAP.
- `PartnerType`: exactamente `Customer` o `Supplier` para registros nuevos.
- `SapCardCode`: referencia externa separada y nullable mientras SAP no haya aceptado el socio.

Para registros nuevos, `Code` se deriva del `GlobalId` con el formato `BP-` seguido del GUID en formato `N` y mayúsculas. Así puede generarse sin conexión desde una sucursal, es globalmente único y no cambia cuando SAP asigna o confirma un `CardCode`.

Los códigos históricos de NuanSystem se preservan. La migración no los reemplaza masivamente.

### Unicidad por rol

La clave de negocio activa será:

```text
IdentificationTypeId + NormalizedIdentification + PartnerType
```

Consecuencias:

- la misma identificación puede tener un cliente y un proveedor separados;
- dos clientes activos con la misma identificación se bloquean;
- dos proveedores activos con la misma identificación se bloquean;
- crear un cliente no crea automáticamente un proveedor y viceversa;
- `Both` no se admite en nuevas altas.

Los registros históricos `Both` no se dividen automáticamente. Permanecen identificados como legado y bloqueados para sincronización de salida hasta que una revisión humana decida conservar un rol o crear explícitamente el segundo registro con otro `GlobalId`, `Code` y `SapCardCode`.

### Normalización

La normalización para unicidad y `CardCode`:

1. recorta extremos;
2. convierte letras a mayúsculas invariantes;
3. elimina espacios, puntos y guiones;
4. conserva el valor original en el registro y la auditoría;
5. nunca trunca para cumplir el límite SAP.

Si el resultado con prefijo excede el máximo admitido por la compañía SAP configurada, la solicitud queda rechazada con error estable y no se envía a SAP.

### Prefijos configurables por empresa

La política vive en Master de gobierno y se versiona por empresa central. Tiene dos modalidades:

| Modalidad | Cliente nacional | Cliente extranjero | Proveedor nacional | Proveedor extranjero |
|---|---|---|---|---|
| `NationalForeign` | `CN` | `CE` | `PL` | `PE` |
| `RoleOnly` | `C` | `C` | `P` | `P` |

Una identificación se considera extranjera únicamente cuando el tipo de identificación corresponde a pasaporte. Los demás tipos se consideran nacionales para esta regla.

El `SapCardCode` se forma como `Prefijo + NormalizedIdentification`. Antes de aceptar una configuración se valida que los cuatro resultados posibles sean representables por SAP, que no existan prefijos vacíos y que la combinación no produzca colisiones dentro de un rol.

La sucursal no genera ni reserva `SapCardCode`. El tenant central lo calcula al aceptar la propuesta, después de validar rol, identificación normalizada, política vigente y ausencia de colisión.

Un `SapCardCode` existente y confirmado no se regenera por un cambio posterior de configuración. La modificación de códigos históricos es una operación extraordinaria, humana y fuera del CRUD normal.

## Campos de socios en el primer vertical

### Editables al crear y actualizar

- nombre o razón social;
- nombre comercial;
- teléfono principal;
- correo electrónico;
- direcciones;
- contactos y sus medios de contacto.

Direcciones y contactos requieren `GlobalId` propio para conservar identidad entre tenants y permitir reconciliación sin depender de IDs locales.

### Definidos al crear y luego no editables desde la sucursal

- rol `Customer` o `Supplier`;
- tipo y número de identificación.

### Solo lectura en la primera versión

- `Code` interno;
- `SapCardCode`;
- condiciones de pago;
- lista de precios;
- límite de crédito;
- estado SAP;
- datos contables, bancarios, retenciones, cuentas y configuración de crédito no incluida expresamente como editable.

Los campos de solo lectura se actualizan desde SAP o mediante procesos administrativos posteriores, nunca desde el formulario inicial de creación/edición de sucursal.

## Versionamiento y reconciliación

Cada socio canónico usa dos controles:

- `RowVersion`: token opaco de concurrencia local usado por API y SQL para impedir actualizaciones perdidas.
- `CanonicalVersion`: entero creciente asignado por el tenant central y distribuido a todas las sucursales.

Una mutación de sucursal incluye `BaseCanonicalVersion`, campos modificados y snapshot base. Master aplica una reconciliación de tres vías:

```text
Base = último snapshot canónico conocido por el origen
Local = estado canónico actual en el tenant central
Propuesto = cambio de sucursal o estado leído desde SAP
```

Reglas por campo:

- cambió solo Propuesto: aceptar;
- cambió solo Local: conservar Local;
- ambos cambiaron al mismo valor: aceptar sin conflicto;
- ambos cambiaron a valores distintos: crear conflicto humano;
- campos no autorizados en la propuesta: rechazar sin mutarlos.

La reconciliación SAP usa como Base el último snapshot confirmado por SAP. Un cambio remoto y uno local sobre campos diferentes pueden fusionarse; cambios distintos sobre el mismo campo quedan en conflicto.

Un conflicto conserva ambos valores, versiones, empresa/sucursal origen, usuario, correlación, fechas y resolución. Resolver exige permiso, motivo y auditoría. No existe `last-write-wins` silencioso.

## Flujo Sucursal → tenant central → sucursales

```text
Usuario en sucursal
  -> API con contexto de empresa
  -> transacción tenant sucursal
       -> persistir propuesta local
       -> LocalOutbox con EventId estable
  -> COMMIT
  -> responder estado PendingMaster

NuanSystem.MasterBranchSyncWorker / relay interno
  -> claim LocalOutbox con lease
  -> promover el mismo EventId a Master de gobierno
  -> target único: tenant central padre
  -> aplicar propuesta idempotente
  -> validar identidad, rol, versión y campos
  -> aceptar, rechazar o crear conflicto

Si Master acepta
  -> transacción tenant central
       -> persistir socio canónico y CanonicalVersion
       -> crear intención de distribución interna
       -> crear SapSyncOutbox si SAP está habilitado
  -> COMMIT
  -> distribuir snapshot canónico a todas las sucursales, incluida la de origen
```

La aplicación de una réplica debe usar un contexto técnico que impida generar una nueva propuesta de sucursal. Así se evita el bucle Sucursal → Master → Sucursal → Master.

Una indisponibilidad de Master no revierte el commit local. El usuario ve el socio con estado `PendingMaster`. Un rechazo o conflicto conserva el registro local con estado visible y permite corregirlo; no se distribuye como canónico.

## Flujo tenant central → SAP

La aceptación canónica y la intención SAP se confirman en la misma transacción del tenant central. Se reutiliza y completa la infraestructura SAP existente; no se crea una cola paralela adicional.

```text
Tenant central
  -> SapSyncOutbox Pending
  -> SapOutboxWorker en NuanSystem.SyncWorker
  -> resolver empresa, perfil y SapIntegrationMode
  -> adquirir lock empresa/entidad/dirección
  -> Service Layer por sesión aislada de empresa
  -> crear o actualizar BusinessPartner
  -> persistir CardCode, respuesta segura y estado
  -> publicar resultado canónico a sucursales
```

La primera implementación de salida soporta Service Layer. `DiApi` continúa bloqueado para esta capacidad mientras su adaptador siga marcado como no implementado.

Para socios, `SapCardCode` es la clave externa idempotente. Antes de crear, el worker vuelve a consultar por `CardCode`; una coincidencia compatible se trata como recuperación idempotente, y una coincidencia con otra identificación/rol es conflicto terminal.

## Replicación SAP → NuanSystem

El objetivo por empresa activa es:

- ciclo de cambios cada cinco minutos;
- reconciliación Full nocturna;
- lock por empresa, entidad y dirección;
- watermark solo después de persistencia durable exitosa;
- procesamiento paginado y cancelable;
- ausencia en un Full no implica eliminación automática.

Si SAP no ofrece una marca de modificación confiable para una entidad, el ciclo de cinco minutos usa lectura paginada y comparación de hash/snapshot persistido. No se simula un watermark inseguro.

Orden mínimo de dependencias:

1. geografía, monedas, unidades e impuestos;
2. condiciones de pago y listas de precios;
3. bodegas;
4. artículos;
5. clientes y proveedores;
6. órdenes de compra.

Las bodegas y artículos son de lectura en NuanSystem. Cada bodega SAP pertenece a una sola sucursal y una sucursal puede recibir varias bodegas. Las órdenes de compra se enrutan por las bodegas de sus líneas; una orden con bodegas de varias sucursales queda `NeedsApproval` y no se divide automáticamente.

## Estados y errores

### Estado de propuesta interna

```text
PendingMaster -> Accepted | Rejected | Conflict
Conflict -> Accepted | Rejected
```

### Estado de entrega SAP

```text
Pending -> InProcess -> Synced
                    -> Retry -> InProcess
                    -> Conflict
                    -> DeadLetter
```

Clasificación:

- **Retry:** timeout, red, Service Layer temporalmente indisponible, sesión expirada, HTTP 429/5xx y lock técnico recuperable.
- **Conflict:** identidad externa ocupada, edición concurrente del mismo campo o respuesta SAP incompatible con el snapshot base.
- **DeadLetter:** validación estable, configuración faltante, mapeo obligatorio inexistente, longitud no representable o agotamiento de intentos.
- **Rejected:** regla de negocio de Master, duplicado dentro del mismo rol o campo no autorizado.

El valor inicial de `MaxAttempts` es cinco. El backoff predeterminado es 1, 5, 15 y 60 minutos, y permanece en 60 minutos para el último intento. Los perfiles pueden reducir intentos, pero no superar el máximo global autorizado de veinte.

Toda acción manual de retry, resolución o liberación de lock exige permiso y motivo. Ningún operador edita payload, `EventId`, `GlobalId`, identidad externa o correlación.

## Seguridad y aislamiento

- Master de gobierno conserva URLs, usuario, contraseña y configuración cifrada SAP.
- El tenant central y las sucursales no reciben secretos SAP.
- La API deriva la empresa autenticada; no confía en un `CompanyId` arbitrario enviado en el cuerpo.
- La propuesta BranchToMaster solo puede apuntar al `ParentCompanyId` configurado.
- La distribución posterior solo alcanza sucursales activas hijas de esa empresa central.
- Los campos contables, crédito, precio y estado no se aceptan en comandos de edición inicial aunque la UI fuera manipulada.
- Logs, auditoría y errores excluyen contraseñas, cookies, tokens, conexiones y payloads SAP completos.
- SAP, MasterBranch y SRI conservan permisos, colas y workers separados.

## Observabilidad y operación

Cada etapa registra de forma segura:

- `CorrelationId`, `EventId` y `CausationEventId`;
- empresa central y sucursal origen/destino;
- entidad, rol, `GlobalId` y referencia SAP no sensible;
- versión base y versión canónica;
- estado, intentos, próxima ejecución, lock owner y expiración;
- clasificación de error y mensaje saneado;
- usuario o worker responsable;
- conteos de aceptados, rechazados, conflictos, retry y dead-letter.

Los monitores deben permitir seguir una operación completa desde la sucursal hasta SAP sin fusionar las colas. La correlación une la trazabilidad; no cambia la propiedad de cada pipeline.

## Migración de datos existentes

La migración es forward-only, idempotente y deshabilitada por defecto. Se ejecuta en estas etapas:

1. generar un informe sin escrituras de identificaciones normalizadas duplicadas, registros `Both`, `SapCardCode` repetidos, identificaciones faltantes, códigos SAP no representables y divergencias entre tenant central y sucursales;
2. agregar columnas, tablas, índices no activados y contratos de auditoría;
3. completar `NormalizedIdentification`, `CanonicalVersion`, `RowVersion` y `GlobalId` faltantes sin cambiar códigos históricos;
4. clasificar `Both` como legado pendiente de revisión, sin crear registros ni códigos adicionales;
5. corregir datos aprobados por responsables humanos;
6. reemplazar la unicidad global de identificación por unicidad normalizada dentro del rol;
7. configurar y validar la política de prefijos por empresa;
8. habilitar primero ObserveOnly, luego un tenant central y una sucursal piloto explícita.

No se elimina, fusiona, divide ni reasigna automáticamente un socio histórico. Las decisiones destructivas o de identidad requieren respaldo, reporte de impacto, aprobación y evidencia de reconciliación.

## Decisiones conservadas para fases posteriores

### Entrada de mercancía de compra

- se origina en la sucursal contra `SapDocEntry` y `LineNum` de la orden;
- admite recepciones parciales y múltiples contra cantidad abierta;
- lotes y series dependen de configuración por empresa/artículo;
- lote/serie puede ingresarse, escanearse o generarse automáticamente;
- patrones, prefijos y secuencias se configuran por empresa;
- la confirmación local aumenta stock antes de la respuesta SAP y crea intención durable automáticamente;
- la reversión exige permiso, motivo y auditoría; después de sincronizar con SAP se modela como documento compensatorio o cancelación soportada, nunca como borrado;
- el documento SAP debe usar una clave externa idempotente configurable, con `U_NuanEventId` como alias predeterminado; si el UDF no existe y no está validado, el envío permanece deshabilitado.

### Oferta de venta

- se origina en la sucursal;
- precios, listas e impuestos provienen de SAP y se importan previamente;
- descuentos dependen de empresa y rol; exceder el límite requiere aprobación;
- estados: `Draft -> PendingApproval` cuando corresponda, `Confirmed -> PendingSAP -> Synced`, con estados de error/conflicto observables;
- el envío a SAP es automático después de confirmar o aprobar;
- es visible en la sucursal origen y para usuarios centrales autorizados;
- usa la misma regla de clave externa idempotente para documentos SAP.

Estas decisiones no autorizan todavía movimientos de stock, asientos, llamadas SAP ni creación de formularios.

## Descomposición de implementación

Cada bloque tendrá especificación enfocada, plan, pruebas y autorización independiente:

1. **Fundación de identidad y configuración:** modelo de socios, normalización, prefijos, versiones, informe de migración y feature flags.
2. **Sucursal → tenant central:** propuesta durable, intake canónico, conflictos y redistribución a todas las sucursales.
3. **Socios tenant central ↔ SAP:** readers de clientes, reconciliación, `SapSyncOutbox`, sender Service Layer y confirmación.
4. **Catálogos y órdenes SAP → NuanSystem:** agenda cinco minutos/nocturna, dependencias, automatización de OC y routing.
5. **Entrada de mercancía:** documento operativo, stock, lotes/series, recepción parcial, reversión y entrega SAP.
6. **Oferta de venta:** precios/impuestos, descuentos, aprobación, visibilidad y entrega SAP.
7. **Endurecimiento y despliegue:** reconciliación, monitores, runbooks, seguridad negativa, rendimiento, piloto y rollback.

No se crea un único plan gigante. El primer plan posterior a la aprobación de este documento cubrirá únicamente los bloques 1 y 2 hasta obtener un socio canónico distribuido, sin realizar llamadas SAP.

### Estado verificable de los bloques 1 y 2

La implementación de código de los bloques 1 y 2 incluye identidad separada, política central de prefijos, migraciones forward-only, propuesta durable desde sucursal, reconciliación/versionamiento en el tenant central, conflicto explícito y redistribución a origen y sucursales hermanas. Los doce escenarios de aceptación pasan en memoria reutilizando los servicios y políticas de producción. Los contratos SQL y el fixture SQL opt-in compilan; sin opt-in, las pruebas SQL se omiten de manera explícita.

Esto no equivale a una activación operativa. Permanecen **no validados**: la ejecución runtime SQL y de migraciones, el readiness de datos reales, el despliegue/activación de perfiles y worker, el piloto/UAT, el rollback en infraestructura y cualquier conexión o escritura SAP. No se declara listo el bloque 3 ni se habilita `SapSyncOutbox`.

El procedimiento operativo, criterios de aborto y evidencia requerida están en [`docs/operations/BUSINESS-PARTNER-BIDIRECTIONAL-PILOT.md`](../operations/BUSINESS-PARTNER-BIDIRECTIONAL-PILOT.md).

## Matriz de capas

| Capa | Estado para la implementación | Responsabilidad |
|---|---|---|
| Domain | Verificar sin dependencia externa | reglas puras de identidad, roles y transiciones aplicables |
| Application | Cambiar | comandos, validación autoritativa, snapshots, reconciliación y contratos de colas |
| Persistence | Cambiar | repositorios transaccionales tenant/Master y mapping SAP |
| API | Cambiar | contratos finos, concurrencia, permisos y estados de sincronización |
| Database | Cambiar | migraciones Master/tenant, constraints, índices, auditoría y colas |
| Frontend services | Cambiar por fase | DTOs tipados y llamadas mediante `INuanApiClient` |
| Frontend forms/Designer | Cambiar después de contratos | edición permitida, estados, conflictos y monitores |
| Security/menu | Cambiar | permisos por acción, resolución, retry y visibilidad central |
| MasterBranch worker | Extender | BranchToMaster, target central, canonicalización y distribución |
| SAP Integration/SyncWorker | Cambiar desde bloque 3 | readers, sender, outbox, locks, logs, retry y heartbeat |
| Tests | Cambiar en cada bloque | unidad, contrato, SQL, integración, seguridad, concurrencia y runtime autorizado |
| Documentation/catalogs | Cambiar | blueprints, catálogo, grafo, runbooks y evidencia saneada |

## Estrategia de pruebas y gates

### Contratos mínimos del primer incremento

1. crear un cliente nacional en una sucursal produce una sola propuesta durable;
2. repetir el mismo `EventId` no duplica propuesta, canónico ni distribución;
3. una caída de Master deja la propuesta local recuperable;
4. el tenant central acepta y distribuye el socio a todas sus sucursales;
5. la aplicación distribuida no vuelve a publicar hacia Master;
6. crear otro cliente con la misma identificación se rechaza;
7. crear explícitamente un proveedor con esa identificación se acepta como otro `GlobalId`;
8. una versión base obsoleta sobre el mismo campo produce conflicto;
9. una versión base obsoleta sobre campos diferentes se fusiona de forma determinista;
10. sucursal, API, logs y payloads no exponen secretos SAP;
11. perfiles y workers deshabilitados no reclaman ni mutan colas;
12. ninguna prueba del primer incremento llama SAP o modifica stock.

### Gates posteriores SAP

- sesión aislada por empresa;
- create/update idempotente por `SapCardCode`;
- watermark solo tras commit local;
- retry solo para errores transitorios;
- conflicto y dead-letter visibles;
- segundo ciclo sin cambios produce cero mutaciones;
- pruebas reales SAP separadas de build/unit tests y ejecutadas solo con autorización.

### Gates documentales y operativos

- scripts forward-only e idempotentes;
- respaldo y restauración ensayada antes de una migración real;
- activación por empresa, entidad y sucursal piloto;
- métricas y criterios de aborto definidos antes de runtime;
- rollback deshabilita procesamiento sin borrar eventos ni auditoría.

## Discovery Record

**Outcome:** establecer una base ejecutable y segura para clientes/proveedores bidireccionales y fijar límites obligatorios para documentos posteriores.

**Work type:** arquitectura de sincronización, integración externa y futuros casos operativos.

**Domain:** socios comerciales, replicación interna NuanSystem y SAP Business One.

**Explicit domain decisions and exclusions:** SAP opcional; tenant central canónico; sucursales capturan; roles separados; no segundo rol automático; dos colas y workers independientes; WinForms sin acceso externo; primera implementación sin stock ni documentos.

**Affected layers:** Domain verificado, Application, Persistence, API, SQL Master/tenant, frontend posterior, seguridad, `NuanSystem.MasterBranchSyncWorker`, `NuanSystem.SapIntegration`, `NuanSystem.SyncWorker`, pruebas y documentación.

**Risk:** alto por identidad, concurrencia, múltiples bases, estado externo, inventario futuro y migración de datos existentes.

**Evidence inspected:**

- `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md` — topología, SAP opcional y separación de workers.
- `docs/architecture/MASTER-BRANCH-ITERATION-8-TRANSACTIONAL-OUTBOX-BLUEPRINT.md` — límite tenant + `LocalOutbox`, promoción idempotente y recuperación.
- `docs/architecture/SAP-SYNC-PROFILES-BLUEPRINT.md` — perfiles, agenda, historial, locks y estado real de outbox SAP.
- `docs/architecture/SAP-PURCHASE-ORDER-IMPORT-AND-ROUTING.md` — identidad, versión y routing de órdenes.
- `Application/Features/Sync/Configuration/SyncMasterBranchEntityCatalog.cs` — entidades internas operativas y dependencias.
- `Application/Features/BusinessPartners/Commands/BusinessPartnerLocalOutboxWriter.cs` — publicación actual limitada a empresa Master.
- `database/sql/024_tenant_business_partners.sql` — unicidad actual incompatible con separación por rol.
- `database/sql/048_tenant_sap_supplier_import.sql` — uso actual de `CardCode` como código local y conversión a `Both`.
- `Application/Features/SapSync/Constants/SapSyncEntityCode.cs` — catálogo SAP actual sin clientes ni documentos de salida aprobados.
- `Application/Features/SapSync/Handlers/SapPurchaseOrderSyncHandler.cs` — handler programado de OC todavía no implementado.
- `NuanSystem.SapIntegration/Documents/SapDocumentSender.cs` y `NuanSystem.SyncWorker/Workers/SapOutboxWorker.cs` — salida SAP todavía no operativa.
- pruebas de Sync, SAP y BusinessPartners — base contractual existente y ausencia de validación SAP real automática.

**Selected pattern:** transactional outbox local, canonicalización en tenant central, distribución interna idempotente y cola SAP independiente.

**Permitted reuse boundary:** `ITransactionRunner`, `LocalOutbox`, promoción por `EventId`, `SyncOutbox`/targets/auditoría, appliers por `GlobalId`, perfiles SAP, locks, ejecuciones, heartbeat, configuración cifrada y cliente Service Layer por empresa.

**Alternatives rejected:** dual-write entre bases; SAP desde sucursal; reutilizar `SyncOutbox` para SAP; conservar `Code = CardCode`; `Both` automático; sobrescritura por última escritura; lanzamiento conjunto de todos los documentos.

**Gaps/new code:** modelo de identidad por rol, normalización/prefijos, versionamiento canónico, intake BranchToMaster, conflictos, payload completo de socio, outbox sender SAP, clientes, precios/impuestos, automatización de OC y documentos operativos posteriores.

**Differences/constraints:** la infraestructura interna actual es principalmente MasterToBranch; proveedores solo importan desde SAP; OC tiene importación manual/routing pero su scheduler está bloqueado; Service Layer es el único transporte de salida viable para la primera fase.

**Confidence:** alta para límites y brechas; la validación con SAP real permanece no validada hasta un piloto autorizado.

**Validation required:** revisión humana de esta especificación; planes TDD por bloque; pruebas SQL reales con respaldo; UAT con compañía SAP de prueba; activación gradual y evidencia saneada.

## Criterio de salida de Fase 0

Fase 0 termina cuando el propietario:

1. revisa y aprueba este documento;
2. confirma que el primer plan cubra solo identidad/configuración y Sucursal → tenant central → sucursales, sin SAP real;
3. acepta que los registros `Both` y duplicados se resuelvan mediante reporte y decisión humana;
4. mantiene deshabilitados workers, perfiles y migraciones reales hasta aprobar sus gates independientes.

Después de esa aprobación se redactará el plan técnico detallado, archivo por archivo y con desarrollo guiado por pruebas, para los bloques 1 y 2.
