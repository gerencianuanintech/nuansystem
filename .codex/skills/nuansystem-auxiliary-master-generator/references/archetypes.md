# Auxiliary master archetypes

## Contents

1. [Selection](#selection)
2. [Basic](#basic)
3. [Classified](#classified)
4. [Dependent](#dependent)
5. [Explicit exclusions](#explicit-exclusions)

## Selection

```text
Independent catalog with only standard fields
  -> basic
Closed functional code set or protected system rows
  -> classified
Required parent maintained through a corporate lookup
  -> dependent
Accounting, stock, pricing, documents, SAP, or external state
  -> outside MVP; classify and design explicitly
```

All archetypes generate a concrete Standard CRUD vertical. They reuse `BaseGridCrudListForm`, `BaseEditForm`, the inherited Cancelar/Guardar actions, `NuanToggleSwitch`, corporate typography, typed API transport, and audit history. They must not produce one generic runtime maintenance form.

All three editor variants use the compact 870 px shell approved for ItemLines, ItemSubgroups, and ItemOrigins. The main column owns Código, Nombre, parent/classification, custom fields, and Descripción; the right rail owns Orden and Activo. The dependent parent occupies the first main row, moving Código and Nombre down one cadence. Classified fields are inserted in the main column before Descripción, with a server-managed system toggle on the same right-hand row when applicable. The form height grows only by the required 28 px rows; it must not return to a wide 1200 px surface or a single vertical stack.

Every archetype first emits its complete SQL table proposal and a full-color mockup brief. The table and generated image must be shown and explicitly approved before any preview, diff, or scaffold operation.

## `basic`

Reference: `ItemLines`.

Required business roles:

- `code`: required unique string, normally 50 characters;
- `name`: required string, normally 150 characters;
- `description`: optional string, normally 500 characters;
- `sortOrder`: nonnegative integer;
- `active`: Boolean.

Expected vertical behavior:

- normalize code and name in Application;
- validate lengths and nonnegative order in Application and SQL;
- enforce unique code among the approved row scope;
- return active rows from `/lookup` ordered by order, name, and code;
- support list, detail, history, create, update, and logical delete;
- write a `LocalOutbox` intent inside the same tenant transaction when sync mode is enabled;
- apply synchronization by `GlobalId` without adopting a conflicting code.

Evidence:

- `src/Backend/NuanSystem.Application/Features/Definitions/Inventory/ItemLines/`;
- `src/Backend/NuanSystem.Persistence/Repositories/Definitions/Inventory/ItemLineRepository.cs`;
- `src/Backend/NuanSystem.Api/Endpoints/Definitions/Inventory/ItemLines/ItemLineEndpoints.cs`;
- `src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/ItemLines/`;
- `database/sql/201_tenant_item_lines_master.sql`;
- `tests/NuanSystem.Application.Tests/Features/Definitions/Inventory/ItemLines/ItemLineBackendContractTests.cs`.

## `classified`

Reference: `ProductTypes`.

Includes every `basic` rule plus:

- one required field with role `classification`;
- one nonempty ordered code set with stable codes and translated labels;
- optional server-managed Boolean field with role `system`;
- immutable fields for system rows declared in `classification.systemProtectedFields`;
- SQL check constraint and Application validator generated from the same allowed values;
- a closed `LookUpEdit` that persists the stable code, not its caption/index;
- stable errors for protected update/delete outcomes.

Do not use `classified` merely because a form contains a Boolean or a lookup. Use it only when the persisted classification is a closed business code set or system rows have protected semantics.

Evidence:

- `src/Backend/NuanSystem.Application/Features/Definitions/Inventory/ProductTypes/`;
- `src/Backend/NuanSystem.Api/Endpoints/Definitions/Inventory/ProductTypes/ProductTypeEndpoints.cs`;
- `src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/ProductTypes/ProductTypeEditForm.cs`;
- `database/sql/198_tenant_product_types_master.sql`;
- `tests/NuanSystem.Application.Tests/Features/Definitions/Inventory/ProductTypes/ProductTypeBackendContractTests.cs`.

## `dependent`

Reference: `ItemFamilies` for lifecycle and dependency behavior. The MVP does not copy its legacy/external integration fields.

Includes every `basic` rule plus:

- exactly one required field with role `parentId`;
- one `dependency` section that identifies the independently owned parent;
- backend verification that the parent exists, is active, and is not deleted;
- a lookup endpoint that can filter by the parent when the consuming contract requires it;
- composite uniqueness declared explicitly, normally parent id plus code;
- `NuanLookupEdit` with create/edit buttons controlled by the parent FormKey operations;
- refresh and selection of the created/edited parent without changing the child on cancel;
- parent `GlobalId` in the sync payload and parent-first dependency ordering;
- terminal sync failure when the parent global identity is missing; never adopt by local id or code.

The parent remains an independent master with its own API, permissions, forms, persistence, and synchronization definition. The dependent archetype does not place the parent CRUD inside the child.

Evidence:

- `src/Backend/NuanSystem.Application/Features/Definitions/Inventory/ItemFamilies/`;
- `src/Backend/NuanSystem.Api/Endpoints/Definitions/Inventory/ItemFamilies/ItemFamilyEndpoints.cs`;
- `src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/ItemFamilies/ItemFamilyEditForm.cs`;
- `database/sql/188_tenant_item_families_master.sql`;
- `tests/NuanSystem.Application.Tests/Features/Definitions/Inventory/ItemFamilies/ItemFamilyBackendContractTests.cs`;
- `tests/NuanSystem.Application.Tests/Features/Sync/ItemFamilySyncEventApplierTests.cs`.

## Explicit exclusions

Do not infer or generate through these archetypes:

- accounting-account fields or financial posting rules;
- stock, price, purchasing, document, workflow, or transaction behavior;
- SAP codes, external system identifiers, mapping profiles, or remote calls;
- runtime-enabled workers, profiles, or relay;
- secrets, connection strings, company codes, or tenant credentials;
- a generic endpoint/repository/form that owns several catalogs at runtime.

If one of these is required, stop generation and apply the relevant architecture/integration/operational skills before extending the schema.
