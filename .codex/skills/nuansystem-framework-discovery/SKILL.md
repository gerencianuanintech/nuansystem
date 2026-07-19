---
name: nuansystem-framework-discovery
description: Discover and evaluate existing NuanSystem framework components, patterns, contracts, documentation, tests, and nearby implementations before creating or modifying frontend, backend, SQL, integration, worker, security, or shared infrastructure.
---

# NuanSystem Framework Discovery

## Purpose

Prevent parallel infrastructure and unsupported design decisions. This skill is mandatory before non-trivial implementation and before creating any shared component, base class, helper, service, wrapper, repository abstraction, control, or pattern.

This skill discovers and selects evidence. It does not authorize implementation by itself.

## Authority

Read and obey, in order:

1. `.codex/ENGINEERING-CONSTITUTION.md`
2. `.codex/ENGINEERING-KERNEL.md`
3. `.codex/FRAMEWORK-CATALOG.md`
4. `.codex/PATTERN-CATALOG.md`
5. `.codex/KNOWLEDGE-GRAPH.md`
6. Applicable specialized skills
7. Closest approved implementation

Use `.codex/REVIEW-CHECKLIST.md` after implementation.

## Trigger rules

Run this skill when a task:

- adds or modifies code, SQL, UI, API, integration, worker, permissions, or shared documentation;
- creates a new feature/module;
- proposes a new shared component or abstraction;
- modifies a base form, corporate control, visual resource, transport, or shared contract;
- has uncertain architecture or no obvious reference implementation.

For a typo-only or mechanically local change, discovery may be compact but must still confirm scope and nearby convention.

## Mandatory precondition

Do not design or write implementation until the Discovery Record is complete enough for the task risk.

A new shared abstraction with no documented search is prohibited.

## Step 1 — Classify the need

Record:

```text
Outcome:
Primary work type:
Domain:
Affected layers:
Risk:
Search vocabulary:
```

Use the Pattern Catalog classification. Any effect on stock, money, prices, cash, documents, synchronization, or external state is operational.

## Step 2 — Inspect repository instructions

Before source search:

1. Read repository-level instructions.
2. Read the engineering core documents above.
3. Identify applicable skills.
4. Confirm branch/worktree state and unrelated changes.
5. If the task names a remote branch or requires its latest state, fetch that ref when permitted and compare `HEAD` with the refreshed remote-tracking commit. A local `origin/*` ref is cached evidence and does not prove live remote freshness.
6. If remote refresh cannot be performed, record `Remote freshness: Not validated` and avoid claiming that local and live GitHub state match.
7. Note read-only or generated areas.

## Step 3 — Search exact and semantic evidence

Search with fast repository tools where available.

### Required search sequence

1. Exact requested symbol/entity/field.
2. English/Spanish synonyms and domain terminology.
3. Interface and implementation names.
4. Base/shared/common/control/helper/service areas.
5. Same-domain forms, handlers, repositories, endpoints, and SQL.
6. Tests, docs, skills, and configuration.
7. Consumers of any shared component considered for change.

Do not stop at the first filename match. Open source and verify its responsibility and public contract.

## Step 4 — Frontend discovery

Inspect in this order:

1. `src/Frontend/NuanSystem.WinForms.Controls`
2. `src/Frontend/NuanSystem.WinForms.Forms/Common`
3. Same-domain forms and their `.Designer.cs`
4. `src/Frontend/NuanSystem.WinForms.Services`
5. View models/session/security/menu infrastructure
6. Tests and frontend documentation
7. Approved examples from another domain

### Mandatory corporate candidates

Evaluate these before direct DevExpress or new infrastructure:

| Need | Candidate |
|---|---|
| Standard CRUD list | `BaseGridCrudListForm` |
| Standard edit/consult | `BaseEditForm` |
| Standard action | `NuanActionButton` |
| Related catalog lookup | `NuanLookupEdit` |
| Reusable feature grid | `NuanDataGridControl` |
| KPI summary | `NuanKpiCardControl` |
| Brand colors/logo | `BrandResources` |
| Typography | `AppTypography` |
| Form presentation | `FormStyler` |
| API transport | `INuanApiClient` / `NuanApiClient` |

Read the corresponding entries in `FRAMEWORK-CATALOG.md`. The presence of a candidate does not mean it fits every lifecycle; document why it fits or does not.

### Frontend evidence minimum

For medium/high-risk UI work, inspect:

- the corporate component/base class;
- its Designer counterpart when applicable;
- one same-domain consumer;
- one additional representative consumer when shared behavior changes;
- the typed client and permission/menu path where relevant.

## Step 5 — Backend and database discovery

Inspect the vertical slice rather than starting from the endpoint:

```text
Domain/contract
  -> Application command/query/handler/validator
  -> repository contract
  -> Persistence implementation
  -> SQL procedure/script
  -> API endpoint/authorization
  -> frontend client/consumer
  -> tests
```

Search for shared result/error, tenant/company, audit, transaction, and permission infrastructure before adding alternatives.

For SQL, inspect provider-specific conventions, procedure naming, result columns, Dapper mapping, idempotent deployment, indexes/constraints, and logical deletion.

## Step 6 — Integration discovery

When SAP Business One, BEAS, synchronization, or an external service is involved, search:

- source-of-truth documentation;
- mappings and external identifiers;
- tenant/session configuration;
- outbox or durable intent;
- worker/processor;
- idempotency;
- retry/backoff and error classification;
- audit/observability;
- reconciliation/manual recovery;
- representative tests.

Frontend-to-external-system direct calls are not candidates.

## Step 7 — Select the reference pattern

Rank evidence by:

1. same domain and work type;
2. same lifecycle and layer;
3. active corporate component over legacy/direct implementation;
4. tested/documented behavior;
5. closest meaningful contract.

Recency is only a tie-breaker.

Record alternatives and differences. Never invent numeric similarity scores.

## Step 8 — Decide reuse, extension, or creation

```text
Exact approved solution exists?
  Yes -> reuse.
  No  -> existing public contract is configurable?
           Yes -> configure.
           No  -> reusable gap across multiple consumers?
                    Yes -> inspect all consumers and propose backward-compatible extension.
                    No  -> implement the smallest local solution.
No reliable evidence?
  -> continue searching or request an architectural decision.
```

### Confidence

- **High:** exact component plus representative implementation found.
- **Medium:** close pattern found and differences are explicit.
- **Low:** no reliable contract/pattern found.

Low confidence blocks shared or high-risk changes.

## Step 9 — Produce the Discovery Record

Use this exact structure for medium/high-risk work.

Evidence references must use repository-relative paths and symbols so the record remains portable across machines, worktrees, GitHub, and Codex environments. Do not emit machine-specific absolute paths such as `C:\\...` or `E:\\...` unless the user explicitly requests local clickable links for the current checkout.

```text
Discovery Record

Outcome:
Work type:
Domain:
Affected layers:
Risk:

Evidence inspected:
- <path/symbol> — <what it proves>

Selected pattern:
- <pattern and reference implementation>

Components to reuse:
- <component> — <how>

Alternatives rejected:
- <candidate> — <why>

Gaps/new code:
- <gap> — local implementation or proposed extension

Differences/constraints:
- <difference>

Confidence:
- High | Medium | Low

Validation required:
- <checks>
```

For low-risk work, a shorter table is acceptable.

## Step 10 — Build the affected-layer map

Classify each:

| Layer | State | Evidence/action |
|---|---|---|
| Domain | Change / Verify unchanged / N/A / Blocked | |
| Application | | |
| Persistence | | |
| API | | |
| Database | | |
| Frontend services | | |
| Frontend forms/Designer | | |
| Security/menu | | |
| Integration/sync | | |
| Tests | | |
| Documentation/catalogs | | |

A persisted UI field normally requires contract/API/persistence/SQL inspection. A shared component change normally requires consumer and catalog/graph updates.

## Antipatterns

Do not:

- create first and search later;
- search only by exact requested name;
- treat one match as an approved pattern without opening it;
- copy a form from another lifecycle;
- use direct DevExpress controls to duplicate a corporate control;
- create `HttpClient`, SQL, HANA, or SAP access from a form;
- create a helper/wrapper/service because its name differs;
- claim a component does not exist without recording searches;
- use arbitrary reuse percentages;
- turn the Discovery Record into implementation;
- ignore tests, Designer files, permissions, tenant context, or SQL contracts.

## Discovery quality gates

Discovery passes only when:

- classification and risk are explicit;
- applicable engineering core documents were read;
- checkout, branch, and remote-freshness claims are backed by an actual fetch or labeled Not validated;
- source contracts, not just filenames, were inspected;
- representative evidence is repository-backed and cited with portable repository-relative paths;
- corporate frontend candidates were evaluated;
- cross-layer siblings were considered;
- reuse/extension/creation decision is justified;
- low-confidence high-risk work is stopped;
- affected layers and validation plan are recorded.

## Completion output

Before implementation, report concisely:

1. selected pattern;
2. components to reuse;
3. files/areas likely affected;
4. meaningful differences and risks;
5. validation plan;
6. blocker or low-confidence decision, if any.

After implementation, the Review Checklist must verify that the discovery decision was actually followed.
