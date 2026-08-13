---
name: nuansystem-auxiliary-master-generator
description: Propose, approve, scaffold, and validate complete NuanSystem auxiliary administrative master verticals from deterministic manifests. Use when creating or planning a repeated Definitions/Inventory maintenance with a mandatory proposed SQL table structure and full-color mockup approval gate, typed backend, SQL, security/navigation, optional Master-Branch sync, typed WinForms client/ViewModel, BaseGridCrudListForm, BaseEditForm Designer layout, history, and tests. Supports basic, classified, and dependent archetypes; never use for stock, money, documents, workflow, SAP execution, SRI, or other operational use cases.
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

Require no unresolved tokens or secrets, deterministic output, contract alignment, explicit Designer layout, 22 px editors with 28 px row cadence, applicable CRUD/history/lookup/security/audit artifacts, safe SQL, and disabled-by-default sync.

### 7. Review and integrate

Use `-Mode Diff` to classify destinations as new, identical, or colliding. Review every file against the chosen reference. Integrate with explicit patches only; never replace user changes or blindly rewrite shared DI, endpoint, initializer, menu, or consumer files.

### 8. Validate the integrated vertical

Execute the generated builds/tests and open the Designer when possible. SQL deployment, renewed-token authorization, runtime sync, SAP/SRI calls, worker activation, commit, push, and merge each require separate authority.

## Invariants

- Preserve an independent vertical per administrable master.
- Never scaffold before the user has approved the current table proposal and displayed full-color mockup.
- Reuse corporate CRUD, API, CQRS, Dapper, tenant, audit, security, Designer, and sync infrastructure.
- Keep layout explicit in `.Designer.cs`; never generate runtime form builders.
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
- `assets/manifests/`
- `assets/templates/`
