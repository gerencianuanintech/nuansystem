# Implemented Matriz-Sucursal evidence

The runtime catalog is `Application/Features/Sync/Configuration/SyncMasterBranchEntityCatalog.cs`.

## Operative examples

Countries -> Provinces -> Cities; Currencies; Taxes; UnitOfMeasures; PriceLists; limited BusinessPartner; ItemGroups -> Item; Warehouse; and PurchaseOrder with reference dependencies/branch routing.

## Declared but non-operative examples

Payment terms, supplier groups/classes, economic activities, zones, and supply methods have no operative producer/applier. Configuration alone cannot activate them.

## Runtime paths

- publisher/routing/policies: `Application/Features/Sync/Services`
- profile execution/dependencies: `Application/Features/Sync/Execution`
- monitor/manual actions: `Application/Features/Sync/Queries` and `Commands`
- worker/appliers: `NuanSystem.MasterBranchSyncWorker`
- API: `SyncEndpoints.cs`, `SyncConfigurationEndpoints.cs`, `SyncEntityDefinitionEndpoints.cs`
- SQL: `064` through `105`; inspect installers `074`/`075` and verifier `076`.
