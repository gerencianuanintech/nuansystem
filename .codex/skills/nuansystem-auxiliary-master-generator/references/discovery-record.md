# Discovery Record

Outcome:
- Create a project-local skill that deterministically scaffolds independently administrable NuanSystem auxiliary masters into staging for review.

Work type:
- Shared framework component and development tooling for Standard CRUD/Simple Catalog verticals.

Domain:
- Cross-cutting NuanSystem engineering framework; first supported domain is Definitions/Inventory.

Explicit domain decisions and exclusions:
- Generate concrete verticals; do not create one generic runtime form, endpoint, repository, or aggregate owner.
- Support only `basic`, `classified`, and `dependent` administrative masters in the MVP.
- Do not infer SAP/external fields, accounting behavior, operational behavior, secrets, deployment, or runtime enablement.
- Generate only into staging. SQL execution, worker activation, commit, push, and branch integration remain separate authorized actions.
- Before any scaffold, show the complete proposed table and a full-color mockup, obtain explicit approval, and bind it to a deterministic proposal hash.

Affected layers:
- The generator may propose Application, Persistence, API, database, frontend services/ViewModels/forms/Designer, security/menu, synchronization, tests, and registration changes.
- This declarative-resource change affects only the new skill's references and manifest fixtures.

Risk:
- High: this is reusable generation infrastructure whose output can span tenant security, SQL contracts, Designer code, and synchronization.

Repository state:
- Branch inspected: `codex/auxiliary-master-generator`.
- Baseline inspected: `8eb94115`.
- Unrelated generated changes under `tools/node_modules` and `.codex-tmp` were present and excluded.
- Remote freshness: Not validated; no fetch was required for the local implementation request.

Evidence inspected:
- `.codex/ENGINEERING-CONSTITUTION.md` — requires discovery, reuse, Designer safety, server-side truth, vertical impact, and preservation of unrelated work.
- `.codex/ENGINEERING-KERNEL.md` — defines the high-risk shared-framework pipeline and evidence states.
- `.codex/FRAMEWORK-CATALOG.md` — identifies `BaseGridCrudListForm`, `BaseEditForm`, `NuanActionButton`, `NuanLookupEdit`, `NuanToggleSwitch`, typed API transport, and audit history as corporate contracts.
- `.codex/PATTERN-CATALOG.md` — selects Standard CRUD and Simple auxiliary catalog, not an operational lifecycle.
- `.codex/KNOWLEDGE-GRAPH.md` — maps CRUD, security, tenant, and synchronization consumers that generated proposals must preserve.
- `src/Backend/NuanSystem.Application/Features/Definitions/Inventory/ItemLines/` — proves the basic business contract, normalization, validation, audit DTOs, sync payload, and transactional LocalOutbox behavior.
- `src/Backend/NuanSystem.Persistence/Repositories/Definitions/Inventory/ItemLineRepository.cs` — proves the stored-procedure repository boundary.
- `src/Backend/NuanSystem.Api/Endpoints/Definitions/Inventory/ItemLines/ItemLineEndpoints.cs` — proves canonical route, FormKey operations, permissions, and lookup policy.
- `src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/ItemLines/` — proves concrete list/edit forms using corporate base forms.
- `database/sql/201_tenant_item_lines_master.sql` — proves GlobalId, audit, soft delete, constraints, CRUD procedures, full source, and apply-by-GlobalId.
- `src/Backend/NuanSystem.Application/Features/Definitions/Inventory/ProductTypes/` — proves closed `NatureCode`, system-row semantics, and stable validation.
- `src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/ProductTypes/ProductTypeEditForm.cs` — proves a closed lookup that persists stable codes.
- `database/sql/198_tenant_product_types_master.sql` — proves SQL closed-set defense and protected system outcomes.
- `src/Backend/NuanSystem.Application/Features/Definitions/Inventory/ItemFamilies/` — proves a child with an independently owned parent and parent global identity.
- `src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/ItemFamilies/ItemFamilyEditForm.cs` — proves permission-aware `NuanLookupEdit` create/edit lifecycle.
- `database/sql/188_tenant_item_families_master.sql` — proves active-parent validation, scoped uniqueness, in-use protection, and dependency resolution by GlobalId.
- `tests/NuanSystem.Application.Tests/Features/Definitions/Inventory/ItemLines/ItemLineBackendContractTests.cs` — proves independence from generic catalog ownership and SAP fields.
- `tests/NuanSystem.Application.Tests/Features/Definitions/Inventory/ProductTypes/ProductTypeBackendContractTests.cs` — proves closed classification, protected outcomes, and canonical vertical contracts.
- `tests/NuanSystem.Application.Tests/Features/Definitions/Inventory/ItemFamilies/ItemFamilyBackendContractTests.cs` — proves parent validation, canonical routing, and lookup behavior.
- `tests/NuanSystem.Application.Tests/Features/Sync/ItemFamilySyncEventApplierTests.cs` — proves parent-first dependency and disabled-by-default synchronization.

Selected pattern:
- `basic`: ItemLines lifecycle.
- `classified`: ProductTypes lifecycle layered on the basic shape.
- `dependent`: ItemFamilies parent/lookup lifecycle layered on the basic shape.

Permitted reuse boundary:
- Reuse lifecycle structure, corporate components, normalized contract shapes, security operations, audit/soft-delete conventions, and safe synchronization techniques.
- Do not reuse the reference entities, tables, permissions, forms, external/SAP fields, migration numbers, or domain-specific rules for a new master.

Components to reuse:
- `BaseGridCrudListForm` — concrete maintenance list lifecycle.
- `BaseEditForm` — concrete edit/consult lifecycle.
- `NuanActionButton` and inherited base actions — corporate actions.
- `NuanToggleSwitch` — Boolean Sí/No fields.
- `NuanLookupEdit` — dependent parent selection with permission-aware create/edit.
- `INuanApiClient`/typed clients — authenticated company-aware transport.
- `RecordHistoryForm` — field-level history.
- MediatR, FluentValidation, `Result<T>`, tenant transaction runner, Dapper stored-procedure repositories, and LocalOutbox contracts — approved backend boundaries.

Alternatives rejected:
- Generic `InventoryCatalogEndpoints`/repository ownership — current contract tests explicitly remove ItemLines/ProductTypes from generic owners; it would violate independent-master ownership.
- One generic user-facing maintenance form — prohibited by project rules and would break concrete FormKey/menu/permission/Designer ownership.
- Copying ItemFamilies external/SAP fields into all dependent masters — SAP is optional and those fields are domain-specific, not a dependency invariant.
- Direct generation into `src` or `database` — unsafe for a new shared tool and incompatible with collision/diff review requirements.
- Automatic SQL deployment or synchronization activation — outside generation authority and prohibited without separate authorization.

Gaps/new code:
- Versioned manifest schema, archetype descriptions, validation rules, deterministic examples, a staging-only renderer, and a validator are new local skill resources.

Differences/constraints:
- Existing reference migrations include evolved/legacy contracts; examples capture approved semantics, while new output must not copy historical migration identities or compatibility overloads blindly.
- ItemFamilies demonstrates optional external integration fields, but the dependent MVP deliberately excludes them.
- Migration numbers must be supplied as integers/null and revalidated for collisions at generation time.

Confidence:
- High: three concrete, tested verticals and corporate framework contracts were inspected across Application, Persistence, API, SQL, WinForms, security, synchronization, and tests.

Validation required:
- Validate JSON fixtures and schema invariants.
- Generate twice into separate empty staging directories and compare relative paths and hashes.
- Refuse identity/migration collisions and nonempty/production output directories.
- Validate representative basic, classified, and dependent staged output.
- After reviewed integration, build affected projects and run targeted/full tests as proportionate.
- Run static SQL validation only; do not deploy SQL.

## Affected-layer map

| Layer | State for generator output | Evidence/action |
|---|---|---|
| Domain | Verify unchanged | Auxiliary CRUD rules remain in Application/SQL unless a true domain invariant is approved. |
| Application | Change | Generate feature contracts, handlers, validators, and repository abstractions. |
| Persistence | Change | Generate concrete Dapper repository and registrations. |
| API | Change | Generate thin canonical endpoints with tenant/security operations. |
| Database | Change | Stage versioned tenant/Master proposals; never execute. |
| Frontend services | Change | Generate typed client/models over `INuanApiClient`. |
| Frontend view models | Change | Generate concrete ViewModel. |
| Frontend forms/Designer | Change | Generate concrete Designer-safe list/edit forms. |
| Security/menu | Change | Stage FormKey, permissions, operations, and menu proposals. |
| Integration/sync | Change or N/A | Generate only when mode is selected; remain disabled. |
| Tests | Change | Generate contract tests and validate deterministic output. |
| Documentation/catalogs | Change | Register the skill/shared contract after validation. |
