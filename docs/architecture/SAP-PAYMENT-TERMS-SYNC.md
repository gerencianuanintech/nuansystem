# Sincronización de Condiciones de Pago SAP B1 → Matriz → Sucursal

## Decisión

`BusinessPartnerPaymentTerms` es un catálogo local replicable cuya fuente externa aprobada es SAP Business One. El identificador técnico del origen es `SAP_B1`; `PaymentTermsTypes.GroupNumber` se conserva como `ExternalCode` y como código local. `GlobalId` es la identidad estable para la distribución Matriz-Sucursal.

El flujo se divide en dos etapas independientes:

```text
SAP Business One
  -> lectura Full de PaymentTermsTypes
  -> importación idempotente en la base tenant de la Matriz
  -> SyncOutbox / fuente Full de BusinessPartnerPaymentTerms
  -> aplicador idempotente en cada Sucursal por GlobalId
```

No existe transacción distribuida entre SAP, la base tenant y las sucursales. La repetición Full es el mecanismo de reconciliación: incluso un registro local sin cambios puede volver a publicarse para reparar una entrega anterior ausente.

## Regla de representabilidad

Una condición se importa automáticamente solo cuando puede expresarse sin pérdida en el modelo local:

- `NumberOfAdditionalDays >= 0`;
- `NumberOfAdditionalMonths = 0`;
- `NumberOfInstallments <= 1` (un único pago, sin plan de cuotas).

El mapeo aprobado es:

```text
Days = NumberOfAdditionalDays
IsCredit = Days > 0
```

Meses adicionales, varias cuotas o días negativos producen un conflicto visible y no modifican el catálogo local. No se aproximan meses o cuotas a días y no se infiere crédito con otra fórmula.

## Identidad, coexistencia y bajas

- Identidad externa: `(ExternalSystem='SAP_B1', ExternalCode=GroupNumber)`.
- Identidad de replicación: `GlobalId` generado una vez y preservado.
- Los seeds existentes, como `CONTADO` o `CREDITO30`, coexisten con SAP; no se adoptan ni enlazan automáticamente por nombre, código o días.
- Una colisión de código con un registro que no posee la misma identidad externa se registra como conflicto.
- La ausencia en una lectura Full de SAP no desactiva ni elimina automáticamente una condición local.
- La eliminación en sucursal solo puede provenir de una operación explícita de la Matriz; no se deriva del snapshot SAP.

## Contratos ejecutables

- Reader SAP: `SapServiceLayerPaymentTermReader`.
- Caso de uso: `SapPaymentTermImportService`.
- Handler programado: `SapPaymentTermSyncHandler`, entidad `PaymentTerms`, dirección `SapToErp`.
- Importación tenant: `SP_NA_POST_BUSINESSPARTNERPAYMENTTERMS_IMPORTARSAP` en script `112`.
- Publicación/lectura Full: `SyncEventPublisher` y `PaymentTermFullEntitySource`.
- Aplicación en sucursal: `ReferenceCatalogSyncEventApplier` y `ReferenceCatalogSyncApplyRepository`.
- Registro Master: scripts `113` y reparación forward-only `114`.

## Transporte TLS

El cliente nombrado `SapServiceLayer` aplica `ServiceLayer:HttpTimeoutSeconds`. La opción `ServiceLayer:IgnoreSslErrors=true` conecta explícitamente `HttpClientHandler.DangerousAcceptAnyServerCertificateValidator`, pero solo se admite cuando el host confirma ambiente `Development`.

Esta opción es exclusivamente diagnóstica para certificados internos o autofirmados. Producción debe confiar en la CA/cadena correcta del Service Layer; no se permite desactivar globalmente la validación TLS ni guardar credenciales SAP en `appsettings.json` versionado.

## Activación controlada

Los scripts registran capacidad, no habilitan distribución automáticamente:

1. Ejecutar `112` en la base tenant de la Matriz y en cada Sucursal para mantener el mismo esquema e índices de identidad externa.
2. Ejecutar `113` y después `114` en `NuanSystem_Master`.
3. Configurar un perfil activo `MasterToBranch` de modo `Incremental`, incluir `BusinessPartnerPaymentTerms`, seleccionar cada Sucursal destino y usar distribución `All`.
4. Habilitar `BusinessPartnerPaymentTerms` en `MasterBranchSyncWorker:EnabledEntityAppliers`.
5. Para una prueba real, establecer `MasterBranchSyncWorker:Enabled=true`, `SkeletonMode=false` y reiniciar el worker. Restaurar el modo seguro al terminar la ventana de validación.

`114` inserta `SyncEntityConfigurations` con `IsEnabled=0`; la activación requiere una decisión administrativa o un perfil activo. No crea perfiles ni habilita workers porque hacerlo automáticamente podría distribuir datos a sucursales no aprobadas.

## Quality gates

- La lectura SAP y el handler deben resolverse por DI solo cuando SAP está habilitado.
- La importación debe ser repetible y preservar `GlobalId`.
- Los conflictos de representación y de identidad deben quedar visibles en el resultado.
- El payload debe conservar `Days`, `IsCredit`, `ExternalSystem` y `ExternalCode`.
- El aplicador debe operar por `GlobalId` y mantener idempotencia Inbox/Audit.
- No se considera validado en runtime hasta ejecutar `112` en Matriz y sucursales, `113`/`114` en `NuanSystem_Master`, una lectura real de SAP y una entrega real a una sucursal.
- El endurecimiento genérico de reintentos SAP queda fuera de este alcance y requiere una decisión transversal posterior.
