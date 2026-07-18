# NuanSystem Engineering Kernel

## 1. Purpose

This Kernel turns the rules in `ENGINEERING-CONSTITUTION.md` into an executable engineering workflow. It defines what Codex must know, produce, and verify at every stage of a non-trivial task.

The Kernel does not replace specialized skills. It governs when they are loaded and the evidence they must use.

## 2. Required control record

Before implementation, maintain a compact task record:

```text
Outcome:
Work type:
Domain:
Affected layers:
Risk:
Reference pattern:
Framework components:
Known differences:
Files expected:
Validation plan:
Open decisions:
```

For small changes, this record may be implicit but the same decisions still apply. For medium/high-risk work, make it explicit in the plan or progress update.

## 3. Stage K1 — UNDERSTAND

### Inputs

- User request.
- Repository and branch state.
- Existing changes and constraints.
- Acceptance criteria stated or inferred safely.

### Actions

1. Restate the concrete outcome.
2. Separate required behavior from optional improvement.
3. Identify destructive, external, financial, security, or migration implications.
4. Discover repository instructions and applicable skills.
5. Confirm that the target branch and worktree are safe.

### Output

A bounded outcome, explicit exclusions, and any blocking decision.

### Stop conditions

Stop before mutation when the target is ambiguous, authority is missing, destructive scope is unresolved, or a meaningful product decision cannot be inferred safely.

## 4. Stage K2 — CLASSIFY

Choose one primary work type and any secondary types:

- CRUD or administrative catalog.
- Operational use case.
- Document/master-detail workflow.
- Dashboard or monitor.
- Report/export.
- Wizard/dialog/selector.
- Configuration/capability.
- API/client contract.
- Integration/synchronization/worker.
- Security/authorization.
- Database/migration.
- Shared framework component.
- Refactor/bug fix/documentation.

### Operational trigger

If the work changes stock, money, prices, cash, purchasing, document state, external-system state, synchronization state, or auditable workflow state, classify it as operational even if it includes CRUD-shaped screens.

### Output

Primary type, domain, affected layers, and initial risk.

## 5. Stage K3 — DISCOVER

Load `.codex/skills/nuansystem-framework-discovery/SKILL.md` and execute it before designing.

### Minimum searches

1. Exact symbol/name.
2. Synonyms and domain terms.
3. Shared/base/common components.
4. Two representative implementations of the same work type where available.
5. Tests and documentation.
6. Cross-layer siblings for the same entity or flow.

### Frontend-specific search order

1. `src/Frontend/NuanSystem.WinForms.Controls`
2. `src/Frontend/NuanSystem.WinForms.Forms/Common`
3. `src/Frontend/NuanSystem.WinForms.Services`
4. Same-domain forms.
5. Approved cross-domain examples.
6. Designer files and tests.

### Output

An evidence table:

| Need | Candidate | Repository evidence | Decision |
|---|---|---|---|
| Standard CRUD list | `BaseGridCrudListForm` | class and representative derived form | Reuse |
| Standard edit form | `BaseEditForm` | class and representative derived form | Reuse |
| Uncovered behavior | None found | searches performed | Local or architectural decision |

### Stop conditions

Do not create shared infrastructure when discovery is incomplete, evidence conflicts, or confidence is low.

## 6. Stage K4 — SELECT EVIDENCE

Rank candidates by:

1. Same domain and same work type.
2. Same architectural layer and lifecycle.
3. Current corporate component versus legacy/direct component.
4. Active tests and documentation.
5. Recency only as a tie-breaker; newer is not automatically authoritative.

Record:

- selected reference;
- alternatives rejected;
- meaningful differences;
- compatibility constraints;
- confidence: high, medium, or low.

Do not invent numeric similarity scores.

## 7. Stage K5 — PLAN

Build a vertical change map. For each layer, use one state:

- **Change**
- **Verify unchanged**
- **Not applicable**
- **Blocked**

Typical layers:

```text
Domain
Application
Persistence
API
Database
Frontend services
Frontend view models
Frontend forms/designer
Security/menu
Integration/synchronization
Tests
Documentation/catalogs
```

### Plan requirements

- List files or bounded areas expected to change.
- Name framework components and patterns to reuse.
- Order work by dependency.
- Include migration/recovery when data changes.
- Include consumer inspection when shared components change.
- Specify validation for each affected layer.
- Avoid unrelated cleanup.

## 8. Stage K6 — IMPLEMENT

### Rules

1. Preserve established naming and folder structure.
2. Make the smallest coherent vertical change.
3. Reuse public contracts rather than internal implementation details.
4. Keep business rules in the appropriate backend/domain layer.
5. Keep designer layout explicit and runtime behavior separate.
6. Preserve tenant, session, permissions, audit, cancellation, and error semantics.
7. Update tests and documentation when the contract changes.
8. Do not leave silent placeholders.

### Checkpoint

After each coherent block, compare the implementation against the plan. Update the plan when evidence changes; do not drift silently.

## 9. Stage K7 — VALIDATE

Validation is proportional to risk.

### Documentation-only changes

- Verify all referenced paths/symbols exist.
- Check links and hierarchy consistency.
- Search for contradictory rules.
- Inspect the resulting diff.

### Frontend changes

- Build affected projects/solution.
- Run relevant tests.
- Inspect `.Designer.cs` structure.
- Open the Designer when an interactive environment is available.
- Validate permissions, read-only behavior, loading/error states, and corporate controls.

### Backend/database/integration changes

- Build and run targeted tests.
- Validate contracts and failure paths.
- Validate tenant/security boundaries.
- Execute or statically verify SQL as permitted.
- Exercise transaction/idempotency/retry behavior where relevant.

### Evidence record

```text
Validation:
- [Validated] <command or inspection> — <result>
- [Not validated] <check> — <reason>
- [Not applicable] <check> — <reason>
- [Blocked] <attempt> — <observed blocker>
```

## 10. Stage K8 — REVIEW

Execute `.codex/REVIEW-CHECKLIST.md`.

Review the final diff for:

- scope creep;
- duplicated infrastructure;
- architecture violations;
- missing vertical layers;
- mismatched naming/contracts;
- security or tenant regressions;
- designer/runtime layout conflicts;
- unverifiable claims;
- accidental destructive behavior.

A failed gate sends the task back to the earliest affected stage.

## 11. Stage K9 — DELIVER

Lead with the outcome. Report:

1. What changed.
2. Where it changed.
3. What was validated.
4. What was not validated or blocked.
5. Remaining risks or pending work.
6. The next action only when one is actually required.

Never represent a plan as completed work or a created file as a validated behavior.

## 12. Risk escalation matrix

| Risk | Examples | Required rigor |
|---|---|---|
| Low | Isolated docs, local label/style | Targeted discovery, diff review |
| Medium | CRUD, permissions/menu, API/SQL contract | Explicit vertical map, build/tests |
| High | Base controls/forms, auth, tenant, stock, money, documents, migrations, SAP/sync | Consumer map, failure/recovery plan, broad validation |

## 13. Decision trees

### New component

```text
Need identified
  -> exact component exists? reuse
  -> configurable component exists? configure
  -> reusable gap across consumers? inspect consumers and propose extension
  -> screen-specific gap? implement locally
  -> otherwise stop and request an architectural decision
```

### Layer impact

```text
UI field/behavior requested
  -> display-only? frontend may be sufficient after contract verification
  -> persisted? inspect DTO/API/Application/Persistence/SQL
  -> authoritative rule? backend/domain owns it
  -> external/sync field? inspect mappings, outbox/worker, and failure behavior
```

### Validation failure

```text
Check fails
  -> caused by this change? fix and rerun
  -> pre-existing? prove with evidence and report
  -> environment blocked? record attempt and blocker
  -> do not relabel failure as success
```

## 14. Kernel invariants

The Kernel is being followed only when:

- discovery precedes new design;
- references are repository-backed;
- the plan covers vertical impact;
- implementation remains scoped;
- validation evidence is truthful;
- all applicable review gates are resolved.
