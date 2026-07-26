# Implemented Matriz-Sucursal evidence

The runtime catalog is `Application/Features/Sync/Configuration/SyncMasterBranchEntityCatalog.cs`.

## Operative examples

Countries -> Provinces -> Cities; Currencies; Taxes; UnitOfMeasures; PriceLists; BusinessPartnerPaymentTerms; limited BusinessPartner; ItemGroups -> ItemFamilies -> Item; Warehouse; and PurchaseOrder with reference dependencies/branch routing.

`ItemFamilies` has a transactional `LocalOutbox` producer, Full source and
branch applier in code. Its identity is `GlobalId`; its parent is resolved by
`ItemGroupGlobalId`; `(ItemGroupId, Code)` collisions are terminal and never
adopted automatically. Scripts `127`/`128` are deployed in Master, DEMO and
Remigio as applicable, disabled by default; Cañaris remains unchanged.
Runtime application remains pending; read
`docs/architecture/MASTER-BRANCH-ITERATION-8-4B-ITEM-FAMILY-BLUEPRINT.md`.

`BusinessPartnerPaymentTerms` is operative through `PaymentTermFullEntitySource`, `ReferenceCatalogSyncEventApplier`, and `ReferenceCatalogSyncApplyRepository`. Its approved upstream source is SAP B1 `PaymentTermsTypes`; read `docs/architecture/SAP-PAYMENT-TERMS-SYNC.md` and the SAP synchronization skill before changing that pipeline. Tenant script `112` and Master scripts `113`/`114` install its contracts but do not activate profiles or workers.

## Declared but non-operative examples

Supplier groups/classes, economic activities, zones, and supply methods have no operative producer/applier. Configuration alone cannot activate them.

## Runtime paths

- publisher/routing/policies: `Application/Features/Sync/Services`
- profile execution/dependencies: `Application/Features/Sync/Execution`
- monitor/manual actions: `Application/Features/Sync/Queries` and `Commands`
- worker/appliers: `NuanSystem.MasterBranchSyncWorker`
- API: `SyncEndpoints.cs`, `SyncConfigurationEndpoints.cs`, `SyncEntityDefinitionEndpoints.cs`
- SQL: `064` through `105`, plus forward contracts such as `112`/`113`; inspect installers `074`/`075` and verifier `076`.
