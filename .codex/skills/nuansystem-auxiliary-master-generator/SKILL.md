---
name: nuansystem-auxiliary-master-generator
description: Propose, approve, scaffold, and validate complete NuanSystem auxiliary administrative masters from deterministic manifests, with independent physical/API/menu routes, SQL, security, WinForms, history, columns, and tests. Use for basic, classified, or dependent CRUD catalogs; never for operational, SAP, SRI, or synchronization execution.
---

# NuanSystem Auxiliary Master Generator

## Authority

Follow `$nuansystem-project-rules` and run `$nuansystem-framework-discovery` before generating. Activate specialized backend, SQL, sync, frontend, Designer, layout, lookup, navigation-security, and testing skills for selected layers.

Treat generated output as a reviewable starting point, not proof that a feature is complete.

## Scope decision

Use this skill only for an independent administrative catalog. Stop and use the applicable operational or integration skill if the request changes stock, money, prices, documents, workflow, sync state, SAP/SRI state, or another external system.

Choose one archetype:

- `basic`: common fields without an administrable parent; reference ItemLines.
- `classified`: common fields plus one closed stable code; reference ProductTypes.
- `dependent`: common fields plus one required parent and scoped uniqueness; reference ItemFamilies.

Read [archetypes.md](references/archetypes.md). Do not infer relationships, SAP fields, accounts, `IsSystem`, sync dependencies, or consumer bindings from a reference.

## Workflow

### 1. Discover and record decisions

Inspect branch, working tree, processes, next migration number, exact symbols, legacy owner, representative verticals, consumers, tests, security rows, and sync registration. Use [discovery-record.md](references/discovery-record.md). Stop when identity, uniqueness, deletion, ownership, parent semantics, or sync source of truth is unresolved.

### 2. Author a proposal manifest

Copy the nearest example from `assets/manifests/`. Read [manifest-schema.md](references/manifest-schema.md) and [validation-rules.md](references/validation-rules.md). Keep entity names, SQL table, API route, FormKey, permissions, menu identity, procedure prefix, and optional sync identity aligned. Never include secrets or real personal data.

Choose `placement.featurePath`, `api.route`, and `navigation.path` independently. The physical path is an exact PascalCase feature path shared by generated layers; the API route is an absolute kebab-case `/api/...` path; navigation controls only the menu. Do not derive one from another.

Use `schemaVersion: 1.3` for new manifests and set `migrations.versionDate` explicitly as `yyyyMMdd`; never derive persisted migration versions from the runtime clock.

Leave `designApproval` absent or pending while preparing a new proposal. Do not reuse an approval from another master.

### 3. Present the table and full-color mockup

```powershell
& .codex/skills/nuansystem-auxiliary-master-generator/scripts/New-NuanAuxiliaryMaster.ps1 -Manifest <manifest.json> -Mode Validate
& .codex/skills/nuansystem-auxiliary-master-generator/scripts/New-NuanAuxiliaryMaster.ps1 -Manifest <manifest.json> -Mode Propose
```

Before generating any code:

1. Show the user the complete proposed SQL table as a readable table: column, SQL type, nullability, default, purpose, primary/unique keys, checks, foreign keys, audit, and logical deletion.
2. Use `$imagegen` with the emitted `mockupBrief` to generate a full-color mockup. Show the image in the conversation. A textual wireframe, monochrome sketch, or description alone does not satisfy this gate.
3. State the exact fields and relevant behaviors visible in the mockup. Do not invent controls, panels, tabs, legends, dividers, integration fields, or relationships absent from the proposal.
4. Stop and wait for the user's explicit approval of both the table structure and the displayed mockup.

Do not run `Preview`, `Diff`, or `Scaffold` before that approval. If the user requests a change, update the manifest and repeat the table plus full-color mockup. The proposal hash changes automatically, invalidating any previous approval.

### 4. Record approval, then preview without writing

After explicit approval, set `designApproval.status = approved`, copy the exact `proposalHash` returned by `Propose`, set both approval flags to `true`, and record the displayed image in `mockupReference` plus concise evidence. Then run:

```powershell
& .codex/skills/nuansystem-auxiliary-master-generator/scripts/New-NuanAuxiliaryMaster.ps1 -Manifest <manifest.json> -Mode Preview
& .codex/skills/nuansystem-auxiliary-master-generator/scripts/New-NuanAuxiliaryMaster.ps1 -Manifest <manifest.json> -Mode Diff
```

The generator must fail closed when approval is absent, incomplete, or stale. Resolve every collision or validation failure. Never weaken a rule to make an unsafe manifest pass.

### 5. Scaffold into staging

```powershell
& .codex/skills/nuansystem-auxiliary-master-generator/scripts/New-NuanAuxiliaryMaster.ps1 -Manifest <manifest.json> -Mode Scaffold -OutputPath <staging-directory>
```

The generator must refuse existing output and direct writes into production directories. Recheck candidate migration numbers immediately before integration.

### 6. Validate generated artifacts

```powershell
& .codex/skills/nuansystem-auxiliary-master-generator/scripts/Test-NuanAuxiliaryMaster.ps1 -Manifest <manifest.json> -GeneratedPath <staging-directory>
```

Require no unresolved tokens or secrets, deterministic output, contract alignment, explicit Designer layout, applicable CRUD/history/lookup/security/audit artifacts, and safe SQL. Master navigation must register the twelve canonical CRUD/grid operations in `SecurityFormOperations` and grant the approved default role separately; role grants alone do not make operations visible in Accesos. Reject sync generation until all producer, FullSource, applier, runtime registration, retry, dependency, and test artifacts are complete. Treat the compact editor geometry as a hard gate: 870 px client width; maintenance title only in the native form caption; no repeated in-content title or heading line; first row at Y=26; main column at X=154; right rail for Orden/Activo at X=680/684; standard editors 22 px high; row origins advance exactly 28 px; labels use `editorY + 3`; and Description is a 436x64 memo. Use only inherited Cancelar/Guardar actions at the lower right. Same-row controls share the exact Y coordinate; arbitrary 30/44/56 px gaps fail validation.

Exigir una prueba contractual generada que lea el SQL de navegación y compruebe `SecurityFormOperations`, `SecurityRoleFormOperations` y los doce códigos canónicos. Validar por separado aplicabilidad y concesión: nunca aceptar como completo un maestro que solo inserte permisos del rol. Si el script defectuoso ya fue desplegado, corregir la fuente para instalaciones nuevas y crear una migración forward-only idempotente para los ambientes existentes.

For every generated list, require the column-personalization dialog to offer every persisted table column: declared business fields, relationship identities, `Id`, `GlobalId`, creation/update/delete audit fields, and `IsDeleted`. Keep technical and audit columns hidden by default, but create them explicitly with `GridView.Columns.AddField` so they remain selectable when the data source is empty. Do not expose computed display-only properties as persisted columns.

Exigir que la prueba contractual compare la lista completa de columnas persistidas contra el DTO de Application, el modelo frontend y las columnas explícitas del formulario. Validar el selector con datos y sin filas antes de dar por integrado el mantenimiento.

Require the parameterless list-form constructor to remain Designer-safe: permission wiring must return when `ApiSession` is null. Require tenant delete procedures to preserve the update row count before audit and keep delete plus audit in one owned-or-ambient transaction. Require Master navigation reruns to reactivate existing permissions, forms, menus, role-menu access, applicable operations, and approved grants instead of inserting duplicates around soft-deleted rows.

### 7. Review and integrate

Use `-Mode Diff` to classify destinations as new, identical, or colliding. Review every file against the chosen reference. Integrate with explicit patches only; never replace user changes or blindly rewrite shared DI, endpoint, initializer, menu, or consumer files.

### 8. Validate the integrated vertical

Execute the generated builds/tests and open the Designer when possible. Before calling the integration complete, run the spacing gate against the final integrated `.Designer.cs`, not only against staging:

```powershell
& .codex/skills/nuansystem-auxiliary-master-generator/scripts/Test-NuanAuxiliaryMasterDesignerSpacing.ps1 `
  -Manifest <staging>/manifest.normalized.json `
  -DesignerPath src/Frontend/.../<Entity>EditForm.Designer.cs
```

If manual integration groups several controls in one row, give them the same Y. Recalculate every later row from the previous row origin; do not preserve empty vertical space merely because the mockup is taller. SQL deployment, renewed-token authorization, runtime sync, SAP/SRI calls, worker activation, commit, push, and merge each require separate authority.

For every navigable generated maintenance, verify the exact `FormKey` in both routing stages: `Program.CreateGeneralInventoryCatalogForm` must construct the form and `MainForm.CreateModuleForm` must delegate that key to the catalog factory. The generated contract test must fail if either registration or the `ShellViewModel` entry is missing.

## Invariants

- Preserve an independent vertical per administrable master.
- Keep physical placement, public API route, and menu navigation as separate manifest decisions.
- Never scaffold before the user has approved the current table proposal and displayed full-color mockup.
- Reuse corporate CRUD, API, CQRS, Dapper, tenant, audit, security, Designer, and sync infrastructure.
- Keep layout explicit in `.Designer.cs`; never generate runtime form builders.
- Make every persisted master column available to grid personalization even with zero rows; keep technical/audit columns hidden by default.
- Generate compact fixed-dialog editors following the approved ItemLines/ItemSubgroups/ItemOrigins organization. Do not generate a local primary action, 1200 px maintenance surfaces, tabs, panels, info legends, or footer dividers unless a later explicitly approved schema version requires them.
- Never adopt replicated rows by matching code.
- Keep SAP optional and isolated.
- Never activate sync profiles or workers from scaffolding.
- Never claim validation that was not executed.

## Resources

- [Manifest schema](references/manifest-schema.md)
- [Archetypes](references/archetypes.md)
- [Validation rules](references/validation-rules.md)
- [Discovery record](references/discovery-record.md)
- `scripts/New-NuanAuxiliaryMaster.ps1`
- `scripts/Test-NuanAuxiliaryMaster.ps1`
- `scripts/Test-NuanAuxiliaryMasterDesignerSpacing.ps1`
- `assets/manifests/`
- `assets/templates/`
