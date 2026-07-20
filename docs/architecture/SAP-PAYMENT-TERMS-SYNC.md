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
- Registro Master: script `113`.

## Quality gates

- La lectura SAP y el handler deben resolverse por DI solo cuando SAP está habilitado.
- La importación debe ser repetible y preservar `GlobalId`.
- Los conflictos de representación y de identidad deben quedar visibles en el resultado.
- El payload debe conservar `Days`, `IsCredit`, `ExternalSystem` y `ExternalCode`.
- El aplicador debe operar por `GlobalId` y mantener idempotencia Inbox/Audit.
- No se considera validado en runtime hasta ejecutar `112` en la Matriz tenant, `113` en `NuanSystem_Master`, una lectura real de SAP y una entrega real a una sucursal.
- El endurecimiento genérico de reintentos SAP queda fuera de este alcance y requiere una decisión transversal posterior.
