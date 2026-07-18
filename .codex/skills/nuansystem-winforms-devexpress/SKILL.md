---
name: nuansystem-winforms-devexpress
description: Orchestrate NuanSystem WinForms and DevExpress frontend work across forms, controls, Designer files, layout, grids, lookups, typed API clients, ViewModels, navigation, FormKey, permissions, menus, typography, and operational UI. Use for any task under src/Frontend or any backend/security/SQL change required to deliver a desktop screen.
---

# NuanSystem WinForms DevExpress Orchestrator

## Purpose

Route frontend work through the NuanSystem engineering core and the smallest relevant set of specialized skills. This skill owns classification and coordination; specialized skills own detailed implementation rules.

Do not copy specialized rules back into this file.

## Mandatory authority

Read and obey:

1. `.codex/ENGINEERING-CONSTITUTION.md`
2. `.codex/ENGINEERING-KERNEL.md`
3. `.codex/FRAMEWORK-CATALOG.md`
4. `.codex/PATTERN-CATALOG.md`
5. `.codex/KNOWLEDGE-GRAPH.md`
6. `$nuansystem-framework-discovery`
7. Relevant frontend skills below
8. `.codex/REVIEW-CHECKLIST.md`

## Orchestration workflow

1. Understand the requested user outcome.
2. Classify the screen lifecycle and affected vertical layers.
3. Run Framework Discovery before designing or editing.
4. Select repository-backed reference forms and corporate components.
5. Load only the specialized skills required by the change.
6. Build a vertical plan including API, persistence, SQL, security/menu, tests, and documentation when affected.
7. Implement through established frontend boundaries.
8. Validate build, tests, Designer, runtime behavior, and evidence in proportion to risk.
9. Execute the Review Checklist and deliver truthful results.

## Skill router

| Work detected | Load |
|---|---|
| Any new/changed screen lifecycle | `$nuansystem-winforms-forms` |
| Shared/base/custom control selection or evolution | `$nuansystem-winforms-controls` |
| Any `.Designer.cs` or designer-backed control structure | `$nuansystem-winforms-designer` |
| Position, size, grouping, hierarchy, density, resizing | `$nuansystem-winforms-layout` |
| Grid, columns, paging, selection, export, personalization | `$nuansystem-winforms-grids` |
| Lookup, selector, related creation, dependent catalogs | `$nuansystem-winforms-lookups` |
| FormKey, menus, operations, roles, shell routing, company security | `$nuansystem-winforms-navigation-security` |
| Typed frontend HTTP client or transport model | `references/service-clients.md` |
| Operational workflow/document UI | `references/operational-forms.md` and operational backend skill |
| Typography/font/grid appearance | `references/enterprise-typography.md` |
| New screen final review | `references/ui-checklist.md` |

Multiple skills may apply. Load the smallest complete set; a new navigable CRUD with a lookup normally needs Forms, Designer, Layout, Grids, Lookups, Navigation/Security, and Controls.

## Classification

```text
Administrative master/configuration?
  -> CRUD/catalog.
Stock, money, pricing, cash, document, workflow, sync, or external state?
  -> operational/document/integration.
Summary and inspection?
  -> dashboard/monitor.
Bounded choice?
  -> dialog/selector.
Real sequential steps?
  -> wizard.
Shared UI behavior?
  -> framework evolution.
```

Do not classify by the presence of a grid or Save button.

## Frontend invariants

- Reuse corporate base forms and controls before direct DevExpress equivalents.
- Keep visual structure explicit and Designer-compatible.
- Keep business truth, authorization, tenant isolation, persistence, SAP/SRI, and synchronization behind backend/API boundaries.
- Forms do not create `HttpClient`, SQL/HANA/SAP connections, or auth/company headers.
- Typed feature clients depend on `INuanApiClient`.
- Frontend permissions improve UX; backend permissions remain authoritative.
- Use `BrandResources`, `AppTypography`, and established layout rather than local brand constants.
- Preserve UTF-8 Spanish text; scan for mojibake after broad rewrites.

## Pattern discovery

Before implementation, identify:

```text
Screen type:
Domain/folder:
Reference form(s):
Base form:
Corporate controls:
Service/ViewModel pattern:
FormKey/menu/operations:
Affected backend/SQL/integration layers:
Risk:
Validation:
```

Use repository-relative paths. If no reliable pattern exists, report the gap. Low-confidence shared/high-risk design blocks implementation.

## New screen decisions

Infer safely from the closest module when conventions are clear. Stop only when a missing decision materially changes product behavior or architecture, including:

- entity identity or ownership;
- destructive/delete semantics;
- operational state transitions;
- new top-level menu/module;
- default role grants with no policy evidence;
- transport-specific fields or external integration behavior.

Do not stop merely because routine names, folders, captions, ordering, or standard CRUD operations can be inferred from established patterns.

## Vertical completeness

A frontend feature may require:

- Application commands/queries/validation;
- repository and SQL contract;
- API endpoints and form-operation authorization;
- typed frontend service/models;
- ViewModel;
- form and Designer;
- FormKey/menu/operations/DI/shell routing;
- audit/history and synchronization verification;
- tests and documentation.

Classify every layer as Change, Verify unchanged, Not applicable, or Blocked.

## Reference files

These existing references remain detailed sources and must not be duplicated into specialized skills:

- `references/ui-checklist.md`
- `references/enterprise-typography.md`
- `references/designer-compatibility.md`
- `references/service-clients.md`
- `references/operational-forms.md`
- `references/lookup-controls.md`
- `references/menu-integration.md`

When a specialized skill and an older reference differ, apply the engineering hierarchy and update the stale reference as part of the task when necessary.

## Validation

For applicable work:

- build touched frontend projects/solution;
- run targeted tests;
- statically inspect Designer initialization and resource ownership;
- open Visual Studio Designer when available;
- validate allowed/denied/read-only/busy/error/empty states;
- validate navigation and company context;
- inspect final diff and mojibake;
- label each check Validated, Not validated, Not applicable, or Blocked.

Build success does not prove Designer or runtime UI success.

## Completion gate

- [ ] Engineering core and Framework Discovery were followed.
- [ ] Screen lifecycle and reference evidence are explicit.
- [ ] Correct specialized skills were loaded.
- [ ] Corporate framework was reused without parallel infrastructure.
- [ ] Vertical layers, security, company context, and integration effects were classified.
- [ ] Designer/layout/controls/grids/lookups/navigation gates pass where applicable.
- [ ] Validation evidence is truthful and remaining risks are explicit.
