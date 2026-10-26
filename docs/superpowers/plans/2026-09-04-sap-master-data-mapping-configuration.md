# SAP Master Data Mapping Configuration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a safe, auditable configuration flow for SAP-to-NuanSystem master-data synchronization, starting with Suppliers, Customers, and Items, including query selection, field mapping, validation, preview, and worker execution.

**Architecture:** SAP configuration remains governed by the Master database. Approved query definitions and field mappings are versioned in Master; tenant execution stores immutable snapshots, locks, watermarks, details, and logs. SAP transport stays inside `SapIntegration` and `NuanSystem.SyncWorker`; WinForms uses only the REST API. Master/Branch replication remains a separate pipeline and is not reused as SAP outbox.

**Tech Stack:** .NET 10, ASP.NET Core Minimal APIs, MediatR, FluentValidation, Dapper, SQL Server, SAP Business One Service Layer/HANA adapters, Windows Worker Services, WinForms/DevExpress, xUnit/FluentAssertions.

**Spec:** `docs/architecture/SAP-NUANSYSTEM-BIDIRECTIONAL-FOUNDATION.md`, `docs/architecture/SAP-SYNC-PROFILES-BLUEPRINT.md`, `docs/architecture/MASTER-BRANCH-STANDALONE-SAP.md`.

## Global Constraints

- NuanSystem must operate without SAP; SAP is optional per company.
- Domain must not depend on SAP, SQL Server, Dapper, WinForms, or external services.
- All business operations go through the REST API; WinForms never calls SAP or SQL directly.
- Master governs companies, SAP credentials, profiles, query definitions, field mappings, schedules, and integration activation.
- SAP sessions are isolated per company; credentials, cookies, tokens, and passwords never enter logs or snapshots.
- `SapSyncOutbox` is independent from Master/Branch `SyncOutbox`.
- `Both` is not enabled unless import and export are both executable and tested.
- Every execution must define entity, direction, company, lock, idempotency key, retry policy, audit, and recovery behavior.

---

### Task 1: Freeze the supplier/customer/item configuration contract

**Files:**
- Create: `docs/architecture/SAP-MASTER-DATA-MAPPING-CONTRACT.md`
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Mapping/SapMasterDataMappingContract.cs`
- Modify: `docs/architecture/SAP-NUANSYSTEM-BIDIRECTIONAL-FOUNDATION.md` (reference the approved contract)
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapMasterDataMappingContractTests.cs`

**Interfaces:**
- Produces the approved field names, directions, transformations, ownership rules, and status vocabulary consumed by Tasks 2–8.

- [x] **Step 1: Write contract tests** for required identity fields, allowed directions (`SapToErp`, `ErpToSap`), and rejection of `Both` for non-bidirectional capabilities.
- [x] **Step 2: Run the focused tests** and verify they fail until the contract fixture exists.
- [x] **Step 3: Document the first approved mappings**:
  - Supplier: `CardCode → SapCardCode`, generated internal `Code`, `CardName → Name`, `LicTradNum → IdentificationNumber`, `Phone1 → Phone`, `E_Mail → Email`, `Currency → PreferredCurrencyCode`, `ValidFor → IsActive`.
  - Customer: same identity separation, with customer role and prefix policy.
  - Item: `ItemCode → SapCode/ExternalCode`, `ItemName → Name`, group, units, barcode, tax groups, inventory/sales/purchase flags, serial/batch flags, type, active state.
- [x] **Step 4: Define transformation keys** (`Trim`, `UpperInvariant`, `TaxIdNormalize`, `SapBooleanYN`, `CatalogLookup`, `PrefixByTaxpayerType`) and validation outcomes (`Created`, `Updated`, `Unchanged`, `Conflict`, `Skipped`, `Retry`, `Failed`).
- [ ] **Step 5: Run tests and commit** `docs: define SAP master-data mapping contract` (blocked by Git index permissions in this checkout).

### Task 2: Add Master SQL for query definitions and field mappings

**Files:**
- Create: `database/sql/231_master_sap_query_definitions.sql`
- Create: `database/sql/232_master_sap_field_mappings.sql`
- Create: `database/sql/manual/validate-sap-master-data-mapping.sql`
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapMasterDataMappingSqlContractTests.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Services/SqlServerMasterDatabaseInitializer.cs`

**Interfaces:**
- Produces Master tables consumed by Persistence repositories and API configuration use cases.

- [ ] **Step 1: Add SQL contract tests** for idempotent creation, unique active versions, foreign keys, direction checks, and safe deletion rules.
- [ ] **Step 2: Create `SapQueryDefinitions`** with `Id`, `EntityCode`, `Transport`, `QueryText`, `ParameterSchemaJson`, `WatermarkField`, `Version`, `IsApproved`, `IsActive`, audit fields, and soft-delete fields.
- [ ] **Step 3: Create `SapFieldMappings`** with `ProfileEntityId`, `SapField`, `NuanField`, `Direction`, `DataType`, `IsRequired`, `IsReadOnly`, `TransformationKey`, `DefaultValue`, `ValidationRuleKey`, `ExecutionOrder`, `IsActive`, and audit fields.
- [ ] **Step 4: Add constraints** preventing empty source/destination fields, invalid directions, duplicate active mappings, and activation of unapproved query versions.
- [ ] **Step 5: Add validation SQL** that reports missing identity mappings, duplicate `SapCardCode`, `Both` rows without capabilities, and active profiles without query/mapping definitions.
- [ ] **Step 6: Run SQL contract tests and commit** `feat(sql): add SAP master-data mapping configuration`.

### Task 3: Implement Application contracts, validation, and repositories

**Files:**
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Mapping/SapQueryDefinitionContracts.cs`
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Mapping/SapFieldMappingContracts.cs`
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Mapping/SapMappingValidationService.cs`
- Create: `src/Backend/NuanSystem.Application/Abstractions/SapSync/ISapMasterDataMappingRepository.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/SapSync/SapSyncProfileRepository.cs` only where profile/entity joins are required
- Create: `src/Backend/NuanSystem.Persistence/Repositories/SapSync/SapMasterDataMappingRepository.cs`
- Create: `database/sql/233_master_sap_mapping_procedures.sql`
- Modify: `src/Backend/NuanSystem.Persistence/Services/SqlServerMasterDatabaseInitializer.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapMasterDataMappingApplicationTests.cs`

**Interfaces:**
- `GetQueryDefinitionsAsync(entityCode, transport)` returns approved query definitions.
- `GetFieldMappingsAsync(profileEntityId, direction)` returns ordered active mappings.
- `ValidateActivationAsync(profileId)` returns deterministic validation errors.
- `CreateVersionAsync(SapQueryDefinitionWriteModel model, CancellationToken ct)` creates the next immutable query version.
- `ReplaceMappingsAsync(int profileEntityId, IReadOnlyCollection<SapFieldMappingWriteModel> mappings, CancellationToken ct)` replaces the draft mapping set with audit history.
- `DeactivateVersionAsync(int queryDefinitionId, string reason, CancellationToken ct)` deactivates a version without deleting history.

- [ ] **Step 1: Write failing tests** for authorization-independent validation, identity mapping requirements, duplicate destination fields, and invalid transformation keys.
- [ ] **Step 2: Implement immutable records/DTOs and repository interfaces** in Application.
- [ ] **Step 3: Implement Dapper queries** against the two new Master tables with tenant/company scope obtained from the established context.
- [ ] **Step 4: Implement activation validation** requiring an approved active query, identity mapping, valid direction, and an executable reader/handler capability.
- [ ] **Step 5: Run focused tests and commit** `feat(application): validate SAP mapping configuration`.

### Task 4: Add safe API endpoints for configuration and preview

**Files:**
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Mapping/SapMappingUseCases.cs`
- Create: `src/Backend/NuanSystem.Api/Endpoints/SapSyncMappingEndpoints.cs`
- Modify: `src/Backend/NuanSystem.Shared/Constants/PermissionCodes.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapMappingApiContractTests.cs`

**Interfaces:**
- `GET /api/sap/mapping/query-definitions?entityCode={entityCode}`
- `GET /api/sap/profiles/{profileId}/entities/{entityId}/field-mappings`
- `PUT /api/sap/profiles/{profileId}/entities/{entityId}/field-mappings`
- `POST /api/sap/profiles/{profileId}/validate`
- `POST /api/sap/profiles/{profileId}/entities/{entityId}/preview`

- [x] **Step 1: Write endpoint contract tests** for permissions, company scope, validation errors, and absence of secrets in responses.
- [x] **Step 2: Implement MediatR commands/queries** with audit identity and company-access checks.
- [x] **Step 3: Implement Minimal API routes** through the SAP mapping route group and permissions (`SAP.SYNC.PROFILES.VIEW`, `EDIT`, `VALIDATE`).
- [x] **Step 4: Implement preview** as configuration-only validation without SAP calls, local writes, watermark advancement, or outbox publication.
- [ ] **Step 5: Run tests and commit** `feat(api): expose SAP mapping configuration and preview` (commit blocked by repository index permission).

### Task 5: Build the mapping engine and supplier identity correction

**Files:**
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Mapping/SapFieldMappingEngine.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/SapSync/Services/SapSupplierImportService.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Dtos/BusinessPartnerDtos.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/BusinessPartnerRepository.cs`
- Modify: `database/sql/048_tenant_sap_supplier_import.sql`
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapSupplierImportServiceTests.cs`

**Interfaces:**
- `MapAsync(sourceRecord, mappings, direction)` returns a typed mapped snapshot and validation messages.
- Supplier import must persist `SapCardCode` independently from generated/retained NuanSystem `Code`.

- [x] **Step 1: Add failing tests** proving the identity generator preserves existing internal codes and applies national/foreign supplier prefixes.
- [x] **Step 2: Implement the mapping engine** with allowlisted transformations only; reject unknown transformation keys. Context-dependent catalog/prefix transformations remain pending integration with Master configuration.
- [x] **Step 2a: Define the identity policy contract** for customer/supplier prefixes, passport taxpayer types, and preservation of existing internal codes.
- [x] **Step 2b: Add Master persistence** for company-scoped identity policies with idempotent SQL and versioned activation history.
- [x] **Step 2c: Add Application repository contract and Dapper Master repository** for policy read/save operations.
- [x] **Step 2d: Expose authenticated API read/save routes** using active company context and SAP profile permissions.
- [x] **Step 2e: Add typed frontend client, ViewModel, and designer-backed policy form**; navigation/security registration remains pending.
- [x] **Step 2f: Register FormKey, dynamic menu, ADMIN operations, and shell factory routing** for the policy form.
- [x] **Step 3: Change supplier import DTO/procedure parameters** to carry `InternalCode` and `SapCardCode` separately.
- [ ] **Step 4: Implement prefix generation** from partner role and taxpayer type, preserving existing approved codes on updates.
- [ ] **Step 5: Include mapping version and snapshot hash** in Inbox/log detail without storing credentials or cookies.
- [ ] **Step 6: Run focused supplier tests and commit** `fix(sap): separate supplier internal and SAP identities`.

### Task 6: Extend customers and items through the same configurable pipeline

**Files:**
- Create: `src/Backend/NuanSystem.Application/Abstractions/Sap/ISapCustomerReader.cs`
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Customers/Dtos/SapCustomerRecord.cs`
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Customers/Services/SapCustomerImportService.cs`
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Customers/Handlers/SapCustomerSyncHandler.cs`
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Customers/Commands/ImportCustomersFromSapCommand.cs`
- Create: `src/Backend/NuanSystem.Application/Features/SapSync/Customers/Queries/PreviewCustomersFromSapQuery.cs`
- Create: `src/Backend/NuanSystem.SapIntegration/Customers/SapCustomerReader.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/SapSync/Constants/SapSyncEntityCode.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/SapSync/Handlers/SapItemSyncHandler.cs`
- Modify: `src/Backend/NuanSystem.Application/DependencyInjection/ApplicationServiceRegistration.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapCustomerImportServiceTests.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapItemMappingEngineTests.cs`

**Interfaces:**
- Customer reader returns a typed `SapCustomerRecord` using the configured approved query.
- Item reader keeps Service Layer pagination and applies the same mapping contract.

- [ ] **Step 1: Write customer reader/import tests** for `CardType = 'C'`, identity matching, and customer-only creation.
- [ ] **Step 2: Implement customer reader, DTO, service, handler, and registration** without reusing supplier domain rules incorrectly.
- [ ] **Step 3: Add customer query/mapping seeds** as inactive until validated.
- [ ] **Step 4: Add item mapping tests** for units, tax groups, active state, serials, and batches.
- [ ] **Step 5: Replace hard-coded item field assumptions** with approved mapping snapshots while retaining safe pagination and Service Layer session isolation.
- [ ] **Step 6: Run tests and commit** `feat(sap): configure customer and item master imports`.

### Task 7: Integrate the worker with immutable execution snapshots

**Files:**
- Modify: `src/Backend/NuanSystem.SyncWorker/Workers/SapSyncWorker.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/SapSync/Services/SapSyncOrchestrator.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/SapSync/Dtos/SapSyncExecutionContext.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/SapSync/SapSyncExecutionRepository.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapSyncWorkerMappingExecutionTests.cs`

**Interfaces:**
- Each execution receives an immutable profile-entity snapshot containing query version, mappings, batch size, retry policy, and correlation identifiers.

- [ ] **Step 1: Write failing tests** for preview/dry-run/import modes, lock overlap, cancellation, watermark-after-commit, and mapping-version traceability.
- [ ] **Step 2: Persist the effective configuration snapshot** before calling SAP.
- [ ] **Step 3: Pass the snapshot to the handler** and keep remote calls outside SQL transactions.
- [ ] **Step 4: Ensure failed executions do not advance watermarks** and retry only transient failures.
- [ ] **Step 5: Keep `SapOutboxWorker` disabled/NotImplemented** until a separate ERP→SAP implementation is approved.
- [ ] **Step 6: Run worker tests and commit** `feat(worker): execute SAP master-data profiles from snapshots`.

### Task 8: Create the WinForms configuration screens

**Files:**
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncProfileEditForm.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncProfileEditForm.Designer.cs`
- Create: `src/Frontend/NuanSystem.WinForms.Forms/Sap/SapFieldMappingForm.cs`
- Create: `src/Frontend/NuanSystem.WinForms.Forms/Sap/SapFieldMappingForm.Designer.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/Sync/Configuration/SyncConfigurationClient.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.ViewModels/Sync/Configuration/SyncProfileEditViewModel.cs`
- Test: `tests/NuanSystem.Application.Tests/Features/SapSync/SapSyncWinFormsMappingContractTests.cs`

**Interfaces:**
- WinForms consumes typed clients only; it never constructs SQL, HANA, Service Layer, or SAP sessions.

- [ ] **Step 1: Add client/ViewModel contract tests** for loading profiles, query definitions, mappings, validation, and preview.
- [ ] **Step 2: Add a “Consulta SAP” tab** with approved query selection, parameters, preview, and read-only query text for nontechnical users.
- [ ] **Step 3: Add a “Mapeo de campos” tab** using a grid with source, destination, direction, required, read-only, transformation, default, validation, and active columns.
- [ ] **Step 4: Add activation validation feedback** and prevent saving an invalid profile.
- [ ] **Step 5: Reuse corporate controls, permissions, layout, and disposal patterns.**
- [ ] **Step 6: Run frontend build/tests and commit** `feat(winforms): configure SAP master-data mappings`.

### Task 9: Add pilot scripts, runtime tests, and operational runbook

**Files:**
- Create: `docs/operations/SAP-MASTER-DATA-MAPPING-PILOT.md`
- Create: `docs/operations/templates/validate-sap-master-data-pilot.sql`
- Create: `tests/NuanSystem.Application.Tests/Features/SapSync/SapMasterDataPilotScenarioTests.cs`
- Modify: `docs/SAP-SYNC-WORKER.md`

- [ ] **Step 1: Define a nonproduction pilot** with one SAP company, Master, and two branches; all workers start disabled or in `ObserveOnly`.
- [ ] **Step 2: Add scenarios** for create, update, unchanged, duplicate, invalid data, pagination, retry, cancellation, duplicate event, and watermark recovery for each of the three entities.
- [ ] **Step 3: Add SQL evidence queries** for counts, `SapCardCode`, internal codes, Inbox, logs, watermarks, execution states, and absence of duplicate events.
- [ ] **Step 4: Execute preview, dry-run, then import** for suppliers; repeat for customers and items only after supplier gates pass.
- [ ] **Step 5: Document rollback**: deactivate profile first, stop worker, preserve logs, restore configuration version, and never delete audit history.
- [ ] **Step 6: Run full test/build gates and commit** `docs: add SAP master-data mapping pilot runbook`.

## Completion gates

- [ ] Supplier SAP query and mappings are configured from Master and no longer depend on hard-coded identity assumptions.
- [ ] Customer import exists with explicit customer role and prefix rules.
- [ ] Item import uses the approved mapping snapshot while retaining safe Service Layer pagination.
- [ ] Preview and dry-run never mutate local data, SAP, Inbox, Outbox, or watermarks.
- [ ] Worker execution is lock-safe, cancelable, idempotent, auditable, and tenant/company isolated.
- [ ] No SAP credentials, cookies, tokens, or full sensitive payloads appear in logs.
- [ ] Master/Branch synchronization remains separate from SAP synchronization.
- [ ] All automated tests and builds pass; runtime SAP tests are explicitly evidenced in a nonproduction environment.
- [ ] ERP→SAP remains marked pending until an executable outbox sender and handler tests exist.
