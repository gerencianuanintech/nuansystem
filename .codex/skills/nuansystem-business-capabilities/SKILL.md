---
name: nuansystem-business-capabilities
description: Design or modify NuanSystem company capabilities, business-type presets, configurable rules, configuration settings, feature flags, and per-company behavior for a multi-business commercial system. Use when a rule may vary by giro, such as lots, expiration dates, serials, variants, negative stock, warehouses, units, barcodes, sale by weight, cash, credit, pricing, tax, or integrations.
---

# NuanSystem Business Capabilities

## Workflow

1. Identify whether the requested behavior is universal or varies by business.
2. If it varies, define it as a company capability/parameter instead of hard-coding it.
3. Prefer neutral capability names over giro names.
4. Store capability values per company/tenant, with safe defaults.
5. Validate behavior in Application/Domain by reading the capability, not by checking a business label.
6. Seed presets as data only. Presets should configure capabilities; they should not create code branches.

## Capability Examples

- `Inventory.MultipleWarehouses`
- `Inventory.AllowNegativeStock`
- `Inventory.LotsEnabled`
- `Inventory.ExpirationDatesEnabled`
- `Inventory.SerialsEnabled`
- `Catalogs.ItemVariantsEnabled`
- `Catalogs.MultipleBarcodesEnabled`
- `Catalogs.MultipleUnitsEnabled`
- `Sales.SaleByWeightEnabled`
- `Sales.CustomerCreditEnabled`
- `Pricing.PromotionsEnabled`
- `Pricing.MaxDiscountByRoleEnabled`
- `Cash.CashShiftEnabled`
- `Integrations.ScaleEnabled`
- `Integrations.ElectronicInvoicingEnabled`

## Presets

Business presets should only apply default configuration:

- `RetailBasico`
- `Supermercado`
- `Ferreteria`
- `Distribuidora`
- `TiendaEspecializada`
- `ServiciosConInventario`

## Integration Capabilities

- `Integrations.SapEnabled`
- `Integrations.SapMode`
- `Integrations.SapServiceLayerEnabled`
- `Integrations.SapDiApiEnabled`
- `Documents.SapSyncRequired`
- `Documents.AllowManualSapRetry`
- `Documents.RequireApproval`
- `Documents.AllowDrafts`
- `Documents.AllowCancellation`
- `Security.CompanyPermissionsEnabled`
- `Security.ModulePermissionsEnabled`
- `Inventory.RequiresLotManagement`
- `Inventory.RequiresSerialManagement`
- `Inventory.RequiresExpirationDate`
- `Sales.RequiresCreditLimitValidation`
- `Purchasing.RequiresApproval`

Rules:

- Capabilities should configure behavior.
- Capabilities should not create hard-coded business-specific branches.
- Presets can enable capabilities but must not replace modular design.
- SAP capabilities must be optional by company.

## References

- Load `references/capability-checklist.md` before adding a new rule that might vary by company.
