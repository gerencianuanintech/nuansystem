# NuanSystem Engineering Constitution

## 1. Purpose

This Constitution is the highest engineering authority for work performed in NuanSystem by Codex or by a developer following Codex guidance. It protects architectural integrity, framework reuse, predictable user experience, operational safety, and verifiable delivery.

It applies to analysis, plans, code, database scripts, integrations, documentation, refactors, reviews, and generated artifacts.

## 2. Authority hierarchy

When instructions conflict, resolve them in this order:

1. This Constitution.
2. `.codex/ENGINEERING-KERNEL.md`.
3. `.codex/FRAMEWORK-CATALOG.md`, `.codex/PATTERN-CATALOG.md`, and `.codex/KNOWLEDGE-GRAPH.md`.
4. Applicable skills under `.codex/skills/`.
5. The closest approved implementation in the repository.
6. The local implementation decision.

A lower level may specialize a higher level but must not contradict it. User requirements define the desired outcome; they do not silently authorize breaking these engineering constraints. If a real conflict remains, report it with evidence and request a decision.

## 3. Non-negotiable principles

### C-01 — Understand before changing

Identify the requested outcome, domain, work type, affected layers, constraints, and acceptance criteria before implementation.

### C-02 — Discover before designing

Search the repository for existing framework components, patterns, adjacent implementations, tests, documentation, and conventions before proposing new infrastructure.

### C-03 — Reuse before extension; extension before creation

Use this decision order:

1. Reuse an exact approved component or pattern.
2. Configure an existing component through its public contract.
3. Extend an existing component when the need is reusable and backward compatible.
4. Create a new local implementation only when the need is screen-specific.
5. Create new shared infrastructure only with explicit evidence and impact analysis.

### C-04 — Architecture before convenience

Do not cross layer boundaries to save time. Frontend does not access databases or SAP directly. API, Application, Domain, Persistence, workers, and integrations retain their established responsibilities.

### C-05 — Business truth stays server-side

Do not trust frontend-supplied stock, totals, prices, balances, persisted status, tenant identity, permissions, or transaction results. Re-read and validate authoritative state in the backend.

### C-06 — Operational work is not CRUD

A change that affects stock, money, pricing, purchases, cash, documents, workflow state, synchronization, or external systems must use an operational pattern with explicit transaction, concurrency, idempotency, audit, and failure behavior.

### C-07 — Corporate frontend components are the default

For standard WinForms needs, prefer the established NuanSystem framework, including:

- `BaseGridCrudListForm` for standard grid-based CRUD lists.
- `BaseEditForm` for standard edit/consult forms.
- `NuanActionButton` for standard actions.
- `NuanLookupEdit` for lookup fields that need corporate clear/create behavior.
- `NuanDataGridControl` for reusable grids with paging, search, selection, export, and column customization.
- `NuanKpiCardControl` for KPI cards.
- `BrandResources`, `AppTypography`, and `FormStyler` for brand and presentation.
- `NuanApiClient` through `INuanApiClient` for authenticated, company-aware API communication.

Direct DevExpress controls remain valid building blocks inside the corporate framework or when the catalog documents a justified gap. They are not the first choice for duplicating an existing corporate control.

### C-08 — Designer-backed UI remains designer-safe

Visual structure belongs in `.Designer.cs` when the form is designer-backed. Keep control declarations and initialization explicit and compatible with Visual Studio serialization. Runtime code may bind data and behavior; it must not secretly reconstruct the visual layout.

### C-09 — One source of truth

Do not duplicate typography, colors, endpoint contracts, DTO meanings, validation rules, permission names, SQL behavior, or framework responsibilities. Reference or extend the authoritative source.

### C-10 — Multi-company and security are cross-cutting requirements

Preserve company context, authenticated session, authorization, field/form permissions, audit identity, and data isolation in every affected flow. Hiding a frontend control is not authorization.

### C-11 — Database changes are intentional and recoverable

Use established SQL conventions, stored procedures where required, explicit constraints, idempotent deployment scripts, compatible result contracts, and safe data evolution. Never disguise destructive data behavior as a routine refactor.

### C-12 — Evidence before claims

Never state that code compiles, tests pass, the Designer opens, SQL executes, or a workflow succeeds unless that validation was actually performed. Use exactly one evidence state:

- **Validated** — executed successfully; name the command or inspection.
- **Not validated** — not executed; explain why.
- **Not applicable** — irrelevant to the change; explain briefly.
- **Blocked** — attempted but prevented; provide the observed blocker.

### C-13 — Complete vertical impact

For meaningful changes, inspect every affected layer and explicitly classify each as changed, verified unchanged, not applicable, or blocked. Do not assume a request is frontend-only or backend-only.

### C-14 — No silent placeholders

Do not finish with TODOs, empty handlers, fake data, disabled validation, swallowed exceptions, commented-out behavior, or unimplemented branches unless the user explicitly accepts a staged deliverable and the limitation is visible.

### C-15 — Preserve unrelated work

Do not overwrite, reset, rename, stage, or remove unrelated user changes. Keep the change set scoped and reviewable.

## 4. Mandatory engineering pipeline

All non-trivial work follows:

```text
UNDERSTAND
  -> CLASSIFY
  -> DISCOVER
  -> SELECT EVIDENCE
  -> PLAN
  -> IMPLEMENT
  -> VALIDATE
  -> REVIEW
  -> DELIVER
```

The detailed inputs, outputs, stop conditions, and risk model are defined in `.codex/ENGINEERING-KERNEL.md`.

## 5. Decision rules

### Reuse decision tree

```text
Does an exact approved component/pattern exist?
  Yes -> reuse it.
  No  -> does an approved component cover the need through configuration?
           Yes -> configure it.
           No  -> is the requirement reusable and backward compatible?
                    Yes -> propose an extension and inspect consumers.
                    No  -> implement locally.
Only create shared infrastructure after documenting the search and the gap.
```

### CRUD versus operational

```text
Does the action affect stock, money, price calculation, cash,
document state, synchronization, or an external system?
  Yes -> operational use case.
  No  -> is it administration of master/configuration data?
           Yes -> CRUD/catalog pattern.
           No  -> classify using PATTERN-CATALOG.md.
```

### Low-confidence decisions

Confidence is descriptive, not invented arithmetic:

- **High** — exact component and representative implementation found.
- **Medium** — close pattern found with documented differences.
- **Low** — no reliable pattern or contract found.

Low confidence blocks shared infrastructure changes and high-risk implementation. Continue searching or request a decision.

## 6. Prohibited engineering

Unless the applicable catalog or an approved architectural decision explicitly permits it, do not:

- Create a parallel base form, visual helper, HTTP wrapper, grid wrapper, lookup wrapper, permission model, result type, repository abstraction, or synchronization pipeline.
- Instantiate `HttpClient` inside a form or feature client; use the registered API client path.
- Open SQL Server, SAP HANA, DI API, or Service Layer connections from WinForms.
- Put business rules, authoritative totals, stock decisions, or authorization in forms.
- Replace `NuanActionButton`, `NuanLookupEdit`, `NuanDataGridControl`, or `NuanKpiCardControl` with direct controls merely for convenience.
- Hard-code corporate fonts, colors, logos, spacing conventions, JWT headers, company headers, permission keys, or connection strings.
- Build a designer-backed form's entire visual tree in runtime helpers.
- Add inline CRUD SQL to repositories when the established persistence contract requires stored procedures.
- Duplicate DTOs or validators solely to rename them.
- catch and ignore failures, return fabricated success, or claim unexecuted validation.
- Modify `master` directly for feature work when a task branch is in scope.

## 7. Change risk

Classify risk before implementation:

- **Low** — isolated documentation or local presentation change with no shared contract.
- **Medium** — feature change across several layers, new CRUD, permission/menu change, SQL contract update.
- **High** — shared framework component, base form, authentication, tenant isolation, money, stock, document workflow, schema migration, synchronization, SAP integration, or destructive operation.

Medium and high risk require an explicit affected-layer map and validation plan. High risk also requires consumer discovery and rollback or recovery considerations.

## 8. Quality gates

A change is not complete until every applicable gate passes or is reported as blocked:

1. **Scope gate** — requested outcome and exclusions are satisfied.
2. **Discovery gate** — framework and representative patterns were inspected.
3. **Architecture gate** — layer boundaries and dependency direction remain valid.
4. **Reuse gate** — no unjustified parallel infrastructure was created.
5. **Domain gate** — business invariants and operational classification are correct.
6. **Security/tenant gate** — authorization and company isolation are preserved.
7. **Frontend gate** — corporate controls, Designer safety, layout, typography, and permissions are correct.
8. **Persistence gate** — SQL contracts, transactions, audit, and data safety are correct.
9. **Integration gate** — idempotency, retry, mapping, and observable failure are addressed where relevant.
10. **Validation gate** — build/tests/inspection were executed in proportion to risk.
11. **Evidence gate** — every claim is labeled accurately.
12. **Completeness gate** — no silent placeholders or omitted affected layers remain.

Use `.codex/REVIEW-CHECKLIST.md` to execute these gates.

## 9. Exceptions and architectural evolution

An exception must include:

- the requirement that cannot be met by the current rule;
- repository evidence and alternatives inspected;
- affected consumers and compatibility risk;
- chosen scope and why it is minimal;
- validation and rollback strategy;
- whether a catalog, skill, ADR, or this Constitution must be updated.

A local exception does not create a new standard. Shared framework changes require an explicit architectural decision and updates to the Framework Catalog and Knowledge Graph.

## 10. Definition of done

Work is done only when:

- the requested behavior exists without unrelated changes;
- discovery and pattern selection are documented;
- affected layers are complete;
- applicable quality gates pass;
- validation evidence is truthful;
- documentation/catalogs are updated when the framework or pattern changed;
- remaining risks and pending work are explicit.

## 11. Canonical references

- Process: `.codex/ENGINEERING-KERNEL.md`
- Components: `.codex/FRAMEWORK-CATALOG.md`
- Solution patterns: `.codex/PATTERN-CATALOG.md`
- Relationships: `.codex/KNOWLEDGE-GRAPH.md`
- Review gates: `.codex/REVIEW-CHECKLIST.md`
- Mandatory discovery: `.codex/skills/nuansystem-framework-discovery/SKILL.md`
