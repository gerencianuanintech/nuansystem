# Manifest and scaffold validation rules

## Contents

1. [Validation phases](#validation-phases)
2. [Manifest rules](#manifest-rules)
3. [Staging safety](#staging-safety)
4. [Staged-output checks](#staged-output-checks)
5. [Post-integration evidence](#post-integration-evidence)

## Validation phases

Run validation in this order and stop on the first failing phase:

1. parse JSON and validate the manifest contract;
2. validate cross-property and archetype invariants;
3. emit and present the proposed SQL table structure;
4. generate and display the full-color mockup;
5. wait for explicit user approval and bind it to the proposal hash;
6. search the repository for identity and migration collisions;
7. generate only into an empty staging directory;
8. validate the staged output structurally;
9. compare the staged output with the repository;
10. apply reviewed changes separately;
11. build and test after integration.

Validation never executes SQL, activates workers, calls SAP/SRI, commits, pushes, or integrates branches.

## Manifest rules

### Identity and names

- Require `schemaVersion` `1.3` for new manifests; accept `1.1`/`1.2` only for backward compatibility; require a supported archetype.
- Require `entity.singular` and `entity.plural` to match `^[A-Z][A-Za-z0-9]*$` and differ.
- Require `entity.table` to match `^dbo\.[A-Z][A-Za-z0-9]*$`.
- Require `entity.entityCode` to be a stable PascalCase identifier.
- Require `api.route` to match `^/api/[a-z0-9]+(?:-[a-z0-9]+)*(?:/[a-z0-9]+(?:-[a-z0-9]+)*)*$`.
- Require `api.formKey` to match `^[a-z0-9]+(?:-[a-z0-9]+)*$` and equal the final route segment unless an approved legacy migration is explicitly documented outside the manifest.
- In versions `1.2`/`1.3`, require a nonempty `placement.featurePath` whose PascalCase segments match `^[A-Z][A-Za-z0-9]*$` and whose final segment equals `entity.plural`.
- Treat `placement.featurePath`, `api.route`, and `navigation.path` as independent contracts; never derive or force a common prefix between them.
- Require the fixed navigation path and a positive `menuOrder`.
- Require `menuCode` to begin with `MENU.DEFINITIONS.INVENTORY.`.
- Reject duplicate entity, table, route, FormKey, form/menu code, permission member, or generated class/file identities found in the repository.

### Fields

- Require field names to be unique case-insensitively and PascalCase.
- Allow only the declared version `1.1` field types and roles.
- Require exactly one `code`, `name`, `description`, `sortOrder`, and `active` role.
- Require code/name to be nonnullable, required strings with positive `stringLength`.
- Require description to be a nullable, optional string with positive `stringLength`.
- Require sort order to be a nonnullable required `int` with `minimum >= 0` and a compatible default.
- Require active to be a nonnullable required `bool`.
- Reject `nullable: true` combined with `required: true`.
- Require `stringLength` only for strings and `minimum` only for numeric fields.
- Require every default to match its field type and bounds.
- Emit every declared scalar default as an idempotent named SQL default constraint.
- Emit unfiltered unique indexes for `GlobalId` and every `unique: true` field so logical deletion cannot silently recycle identities.
- Emit SQL `CHECK` constraints for required nonblank strings and every declared numeric minimum; Application validation alone is insufficient.
- Reject technical audit/delete fields in `fields` because their owning sections generate them.
- Reject field names beginning with `Sap` or `External` in MVP manifests. Integration fields require an approved schema extension.

### Archetypes

For `basic`:

- reject `classification` and `dependency`;
- reject `classification`, `parentId`, and `system` field roles;
- require `ui.layout = single-section`.

For `classified`:

- require one `classification` field role and an exactly matching `classification.field`;
- require at least two allowed values with unique nonblank codes and labels;
- require every protected field to exist;
- require exactly one server-managed `system` role when `systemProtectedFields` is nonempty;
- reject `dependency` and `parentId`;
- require `ui.layout = classified-section`.

For `dependent`:

- require one `parentId` role and `dependency.field` to match it;
- require PascalCase parent names, canonical parent route, and kebab-case parent FormKey;
- require `dependency.required = true` in MVP;
- require `uniquenessScope` to contain the parent id and code fields;
- reject `classification`, `classification` role, and `system` role;
- require `ui.layout = dependent-section`;
- require the parent entity code in `synchronization.dependencies` when sync mode is enabled.

### Security and UI

- Require nonblank PascalCase permission-member names for read/manage and every lookup consumer.
- Require distinct read and manage permission members.
- Allow `Validate` and `Propose` before design approval; neither may write files.
- Require the complete proposed SQL table structure to be shown before implementation.
- Require a full-color mockup generated with `$imagegen` to be displayed before implementation; text-only or monochrome substitutes fail the gate.
- Require `designApproval.status = approved`, exact current `proposalHash`, both approval flags, a mockup reference, and explicit-approval evidence for Preview/Diff/Scaffold.
- Invalidate approval whenever a table, field, constraint, dependency, classification, audit/delete, or UI choice changes.
- Treat `showInfoLegends` and `showFooterDivider` as literal approved design choices.
- Generate concrete list/edit forms using the corporate bases and explicit Designer-owned controls.
- Keep the parameterless list constructor safe for Visual Studio Designer by returning from permission wiring when `ApiSession` is null.
- Generate the standard operations `refresh`, `consult`, `history`, `create`, `update`, and `delete`; copy remains a frontend lifecycle action governed by create access.
- Register the twelve canonical CRUD/grid operations in `SecurityFormOperations` so the Accesos maintenance can enumerate them: refresh, consult, create, update, delete, copy, history, customize columns, and the four export formats. Grant only the approved default role separately in `SecurityRoleFormOperations`.
- Generate a permission-controlled parent `NuanLookupEdit` only for `dependent`.

### Mandatory compact editor geometry

- Use a fixed-dialog editor with client width 870 px and minimum outer width 886 px.
- Put the maintenance title only in the native form caption through `Form.Text`; do not repeat it as an in-content title or section line.
- Start the first editor row at `Y = 26`, following the approved compact `CarrierEditForm` rhythm.
- Use X=32/154 for the main label/editor column. Código uses width 180; Nombre, parent/classification, custom fields, and Descripción use width 436.
- Use the right rail for Orden (label X=632, editor X=680, width 150, first row) and Activo (label X=632, editor X=684, width 120, second row).
- Give standard single-line editors exactly 22 px height; `NuanToggleSwitch` uses 20 px inside the same 22 px row slot.
- Align each field label at `editorY + 3`.
- Allow multiple controls on one row only when they share the exact editor Y.
- Advance distinct single-line row origins by exactly 28 px, producing 6 px of visible separation after a 22 px editor.
- Give the generated description `MemoEdit` 64 px height. The next row, when present, starts 12 px after its bottom (`nextY = memoY + 76`).
- Put the inherited Cancelar and Guardar actions 10 px below the description and align them at X=628 and X=734. Do not declare `btnGeneratedPrimary` or another local action.
- Reject arbitrary vertical steps such as 30, 44, or 56 px unless a future manifest version declares a separate approved section boundary.
- Run `Test-NuanAuxiliaryMasterDesignerSpacing.ps1` against both staged and final integrated Designer files. Build success and visual resemblance do not replace this gate.

### Audit, deletion, and synchronization

- Require `audit.enabled = true` and `softDelete.enabled = true` in version `1.1`.
- Require authenticated API claims for audit identity; never accept user identity as an untrusted form field.
- Require the generated table and CRUD procedures to persist creation, update, and deletion user id/name audit fields consistently with the proposed technical columns.
- Capture logical-delete `@@ROWCOUNT` before inserting audit, return the captured value, and keep delete plus audit in one owned-or-ambient transaction.
- On Master rerun, recover soft-deleted form/menu/role-menu rows by stable keys and reactivate them before guarded inserts.
- Require `synchronization.enabledByDefault = false` in all modes.
- For `mode = none`, require null execution order and no dependencies.
- Reject `mode = full-source-local-outbox` until the generator produces and validates the transactional producer, FullSource, applier, runtime registrations, dependencies, retries, and tests as one complete contract.
- Require parent global identity in dependent payloads and parent-first dependency ordering.
- Never infer SAP, external integration, runtime enablement, distribution targets, or company configuration.

### Migrations

- Require `migrations.versionDate` in exact `yyyyMMdd` form for schema 1.3 and use it verbatim in tenant/Master history versions.
- Accept only positive integers or `null` for migration values.
- Require nonnull values to be distinct within the manifest.
- Rescan `database/sql/*.sql` for every requested number immediately before staging.
- Reject textual placeholders and collisions; do not silently choose a different number.
- Keep tenant, Master navigation, and Master synchronization purposes separate even when some values are null.
- Run the approved static SQL batch validator against staged SQL when SQL files are generated.
- Never deploy or execute SQL from this skill.

## Staging safety

- Require an explicit staging directory outside the repository production paths.
- Resolve the absolute staging path and reject the repository root, `src`, `database`, `.git`, `tools/node_modules`, and any existing nonempty directory.
- Refuse to overwrite files in staging unless the caller explicitly removes/recreates that bounded staging directory.
- Produce deterministic UTF-8 files with stable ordering and line endings.
- Produce an inventory of proposed new files and central-file insertions.
- Do not edit central registrations during generation. Apply reviewed insertions separately with a scoped patch.
- Run the same manifest twice in two empty staging directories and require byte-identical relative file sets and hashes.

## Staged-output checks

Check that the staged vertical contains the applicable contracts:

- Application DTOs, commands, queries, handlers, validators, repository contract, stable errors, cancellation;
- Persistence stored-procedure repository and registrations;
- thin Minimal API endpoints under the canonical route with permissions and FormKey operations;
- typed WinForms client over `INuanApiClient`, models, ViewModel, concrete list form, concrete edit form, and Designer resources;
- `BaseGridCrudListForm`, `BaseEditForm`, `NuanActionButton`, `NuanToggleSwitch`, and `NuanLookupEdit` where applicable;
- Master navigation/security proposal and tenant SQL proposal;
- audit history and logical deletion;
- sync producer/full source/applier proposals only when synchronization is selected;
- backend contract tests and deterministic generator fixtures.

Reject generated code that:

- places business logic, SQL, SAP, or HTTP mechanics in WinForms;
- creates a generic user-facing maintenance owner;
- omits tenant context or backend authorization;
- persists captions or branch-local parent ids as cross-branch identities;
- enables synchronization or a worker by default;
- contains credentials, connection strings, TODOs, fake success, or silent placeholders.

## Post-integration evidence

After a reviewed patch is applied, validate proportionally:

- targeted backend contract/unit tests;
- API, worker, and WinForms builds using isolated output when required;
- Designer structure and interactive Designer opening when available;
- canonical route, FormKey, permissions, navigation, and lookup behavior;
- the exact FormKey is registered both in `Program.CreateGeneralInventoryCatalogForm` and in `MainForm.CreateModuleForm`; validating only one factory is incomplete;
- SQL static batch validation; runtime SQL only under separate explicit authorization;
- final diff limited to the approved master and skill resources.

Label every result `Validated`, `Not validated`, `Not applicable`, or `Blocked`. A staged file is not evidence that the integrated application builds or that a migration runs.
