# Auxiliary master manifest schema

## Contents

1. [Purpose](#purpose)
2. [Root contract](#root-contract)
3. [Sections](#sections)
4. [Field contract](#field-contract)
5. [Technical fields](#technical-fields)
6. [Design approval gate](#design-approval-gate)
7. [Canonical example](#canonical-example)

## Purpose

Use one JSON manifest as the source of truth for a generated auxiliary-master vertical. Keep the manifest declarative: describe domain identity, fields, authorization, navigation, UI shape, audit, soft deletion, synchronization, and migration reservations. Do not embed C#, SQL, PowerShell, credentials, connection strings, or executable expressions.

The current schema version is `1.3`. Versions `1.1` and `1.2` remain accepted for deterministic backward compatibility.

## Root contract

| Property | Type | Required | Contract |
|---|---:|---:|---|
| `schemaVersion` | string | yes | `1.3` for new manifests; legacy `1.1`/`1.2` remain supported. |
| `archetype` | string | yes | `basic`, `classified`, or `dependent`. |
| `entity` | object | yes | Concrete entity and table identity. |
| `placement` | object | yes in 1.2/1.3 | Exact physical feature placement, independent from API and navigation. |
| `api` | object | yes | Canonical route and `FormKey`. |
| `navigation` | object | yes | Corporate menu location and order. |
| `permissions` | object | yes | Backend read/manage permissions and lookup consumers. |
| `fields` | array | yes | Ordered business-field definitions. |
| `classification` | object | classified only | Closed-code-set contract. |
| `dependency` | object | dependent only | Parent lookup and identity contract. |
| `ui` | object | yes | Approved editor layout choices. |
| `designApproval` | object | before Preview/Diff/Scaffold | Evidence that the current table proposal and displayed full-color mockup were explicitly approved. |
| `audit` | object | yes | Audit-history generation choice. |
| `softDelete` | object | yes | Logical-deletion choice. |
| `synchronization` | object | yes | Matriz-Sucursal generation intent. |
| `migrations` | object | yes | Reserved migration numbers or `null`. |

Reject unknown root properties. Extend the schema deliberately rather than silently accepting misspellings.

## Sections

### `entity`

| Property | Type | Contract |
|---|---:|---|
| `singular` | string | Singular PascalCase C# name, for example `ItemLine`. |
| `plural` | string | Plural PascalCase feature name, for example `ItemLines`. |
| `title` | string | Spanish plural user-facing title. |
| `table` | string | Schema-qualified table, for example `dbo.ItemLines`. |
| `entityCode` | string | Stable Matriz-Sucursal/audit code; never infer from a caption. |

### `api`

| Property | Type | Contract |
|---|---:|---|
| `route` | string | Absolute canonical kebab-case route below `/api`, without trailing slash; for example `/api/definitions/item-commercial-segments`. |
| `formKey` | string | Stable kebab-case key used by API authorization and WinForms. |

The `formKey` must equal the final route segment. The route does not determine physical folders or menu placement.

### `placement`

| Property | Type | Contract |
|---|---:|---|
| `featurePath` | string[] | Exact PascalCase path shared by Application features, API endpoints, Persistence repositories, frontend services/ViewModels/forms, and tests. Its final segment must equal `entity.plural`. |

Example: `['Definitions', 'Inventory', 'ItemCommercialSegments']`. This produces namespaces ending in `Definitions.Inventory.ItemCommercialSegments` while `api.route` may independently be `/api/definitions/item-commercial-segments`. Each segment must be a safe C# identifier; separators, drive names, `.` and `..` are forbidden.

Legacy manifests with `schemaVersion: 1.1` and no `placement` retain their historical layout: feature artifacts under `Definitions/Inventory/<Plural>` and repositories under `Definitions/Inventory`. New manifests must use version `1.3` and declare `placement.featurePath`.

### `navigation`

| Property | Type | Contract |
|---|---:|---|
| `path` | string[] | Must be `['Configuration', 'Definitions', 'Inventory']` for the current `1.3` generator contract. |
| `menuOrder` | integer | Positive order within Inventory definitions. |
| `menuCode` | string | Stable uppercase dotted code beginning `MENU.DEFINITIONS.INVENTORY.`. |

### `permissions`

| Property | Type | Contract |
|---|---:|---|
| `read` | string | Existing or proposed `PermissionCodes` member for list/detail/history. |
| `manage` | string | Existing or proposed member for create/update/delete. |
| `lookupConsumers` | string[] | Permission members allowed to consume the active lookup. Empty is valid. |

The generated lookup policy may accept read, manage, or explicitly declared consumer permissions. Hiding frontend controls is never authorization.

### `classification`

| Property | Type | Contract |
|---|---:|---|
| `field` | string | Name of the field with role `classification`. |
| `label` | string | Spanish UI label. |
| `allowedValues` | object[] | Ordered `{ "code": "StableCode", "label": "Etiqueta" }` values. |
| `systemProtectedFields` | string[] | Fields immutable when the row has role `system` enabled. |

Persist the stable code, never the label or selected index. Generate the same closed set in Application validation and SQL defense-in-depth.

### `dependency`

| Property | Type | Contract |
|---|---:|---|
| `field` | string | Child field with role `parentId`. |
| `parentEntity` | string | Singular PascalCase parent entity. |
| `parentPlural` | string | Plural PascalCase parent feature. |
| `parentIdField` | string | Local integer parent key, normally `<Parent>Id`. |
| `parentGlobalIdField` | string | Parent global identity required in sync payloads. |
| `lookupRoute` | string | Canonical parent lookup route. |
| `parentFormKey` | string | Parent maintenance `FormKey`. |
| `required` | boolean | Whether the child requires an active parent. |
| `allowCreate` | boolean | Whether the lookup exposes permission-controlled Create. |
| `allowEdit` | boolean | Whether it exposes permission-controlled Edit. |
| `uniquenessScope` | string[] | Ordered uniqueness scope, for example `['ItemGroupId', 'Code']`. |

Use `NuanLookupEdit` for a create/edit-capable dependency. Resolve Matriz-Sucursal dependencies by `GlobalId`, never by the branch-local integer id.

### `ui`

| Property | Type | Contract |
|---|---:|---|
| `layout` | string | `single-section`, `classified-section`, or `dependent-section`; must match the archetype. |
| `showInfoLegends` | boolean | Follow the approved mockup; do not infer informational legends. |
| `showFooterDivider` | boolean | Follow the approved mockup; do not infer a divider. |

The layout remains explicit in each concrete `.Designer.cs`. This setting selects a template; it does not create a generic user-facing maintenance form.

All version 1.1, 1.2, and 1.3 layouts inherit the compact corporate geometry established by the approved `CarrierEditForm`: `ClientSize.Width = 870`, the maintenance title appears only in the native form caption through `Form.Text`, main labels/editors use X=32/154, and the right rail uses X=632 and X=680/684. No title or section line is repeated inside the content. Código and Nombre stay in the main column; Orden shares the first row on the right; Activo shares the second available row on the right; parent/classification rows remain in the main column; Descripción is the final 436x64 memo. The first row is Y=26, distinct rows use a 28 px cadence, and labels sit at editor Y+3. Only inherited Cancelar/Guardar buttons are permitted at the lower right. These values are validated in staging and after integration; they are not optional hints or separately configurable in the manifest.

### `designApproval`

| Property | Type | Contract |
|---|---:|---|
| `status` | string | Must be `approved` before Preview, Diff, or Scaffold. |
| `proposalHash` | string | Exact SHA-256 emitted by `-Mode Propose` for the current table and UI contract. |
| `tableApproved` | boolean | Must be `true` after the user sees and approves the complete proposed table structure. |
| `mockupApproved` | boolean | Must be `true` after the user sees and approves the full-color mockup. |
| `mockupReference` | string | Stable reference to the displayed image or generated artifact. |
| `evidence` | string | Concise record of the user's explicit approval; never include personal data or secrets. |

Do not hand-author or guess `proposalHash`. Run `-Mode Propose`, present its table structure, generate the full-color mockup from `mockupBrief`, and wait. Any change to fields, types, constraints, dependency, classification, deletion, audit, or UI choices changes the hash and invalidates the approval.

### `audit` and `softDelete`

Both objects contain one Boolean property, `enabled`. Version `1.1` requires both to be `true` for independently administrable masters.

### `synchronization`

| Property | Type | Contract |
|---|---:|---|
| `mode` | string | Use `none`. `full-source-local-outbox` is reserved and fails closed until complete runtime generation is implemented. |
| `enabledByDefault` | boolean | Must always be `false`. |
| `executionOrder` | integer/null | Positive when synchronized; otherwise `null`. |
| `dependencies` | string[] | Stable parent entity codes that must apply first. |

Synchronization describes code generation only. It never authorizes SQL deployment, relay/worker activation, fixtures, or a branch event.

### `migrations`

| Property | Type | Contract |
|---|---:|---|
| `versionDate` | string | Required in 1.3. Exact `yyyyMMdd` prefix stored in `SchemaHistory`/`MasterSchemaHistory`; it makes SQL output deterministic and must match the intended migration date. |
| `tenant` | integer/null | Tenant table/CRUD/sync-contract migration. |
| `masterNavigation` | integer/null | Master form/menu/operation migration. |
| `masterSync` | integer/null | Master entity/profile registration migration. |

Use positive integers or `null`; never use textual placeholders such as `NNN`. Use schema 1.3 and set `versionDate` explicitly instead of deriving it from the computer clock. A generator must recheck collisions immediately before producing filenames. It must not execute the migrations.

## Field contract

Each ordered `fields` entry has:

| Property | Type | Required | Contract |
|---|---:|---:|---|
| `name` | string | yes | PascalCase C#/SQL contract name. |
| `type` | string | yes | `string`, `int`, `long`, `bool`, `guid`, `decimal`, `date`, or `datetime`. |
| `nullable` | boolean | yes | Storage and DTO nullability. |
| `required` | boolean | yes | Input validation; cannot conflict with `nullable`. |
| `unique` | boolean | yes | Single-field uniqueness. Composite uniqueness belongs to `dependency.uniquenessScope`. |
| `stringLength` | integer | string only | Maximum persisted length. |
| `minimum` | number | numeric only | Inclusive lower bound. |
| `default` | scalar/null | optional | Deterministic default compatible with the field type. |
| `role` | string | yes | `code`, `name`, `description`, `sortOrder`, `active`, `system`, `classification`, `parentId`, or `custom`. |

Versions `1.1`, `1.2`, and `1.3` require exactly one field for each base role: `code`, `name`, `description`, `sortOrder`, and `active`. The `description` field remains optional/nullable. Role `system` is server-managed and read-only in the editor. Role `classification` is mandatory only for the classified archetype. Role `parentId` is mandatory only for the dependent archetype.

Do not include SAP or external-integration fields in the MVP manifest. They require an explicit integration design and a future schema extension; no archetype infers them.

## Technical fields

Do not declare these in `fields`; generate them from the cross-cutting sections:

- local `Id`;
- `GlobalId` when the approved persistence/sync contract requires global identity;
- `CreatedByUserId`, `CreatedByUserName`, `CreatedAt`;
- `UpdatedByUserId`, `UpdatedByUserName`, `UpdatedAt`;
- `DeletedByUserId`, `DeletedByUserName`, `DeletedAt`, and `IsDeleted`.

Audit identity comes from authenticated API claims. Company scope comes from the trusted tenant context. Neither is manifest input.

## Design approval gate

`Validate` and `Propose` are pre-approval modes and never write files. `Propose` emits the SQL table contract, constraints, mockup brief, and proposal hash. `Preview`, `Diff`, and `Scaffold` are post-approval modes and must fail closed unless `designApproval` matches the current hash.

The conversation workflow is part of the contract: show the table, generate and display the full-color image with `$imagegen`, request explicit approval, then record it. A Boolean alone is insufficient because it cannot detect design drift.

## Canonical example

See the deterministic examples in `assets/manifests/`:

- `basic-item-line.json`;
- `classified-product-type.json`;
- `dependent-item-family.json`.

They are reference fixtures. Generating them must target staging and must not overwrite the existing production verticals.
