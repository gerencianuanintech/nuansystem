---
name: nuansystem-sql-standards
description: Create, review, or evolve NuanSystem SQL Server scripts, Master and tenant objects, stored-procedure/Dapper contracts, audit and logical deletion, constraints, indexes, idempotent deployment order, CRUD procedures, operational transactions, and security seeds. Use for any database/sql script, procedure name, repository SQL contract, schema change, permission seed, audit history, migration, or SQL execution plan.
---

# NuanSystem SQL Server Standards

## Authority and mandatory reading

Follow `$nuansystem-backend-architecture`, `$nuansystem-backend-persistence`, and the appropriate CRUD/operational skill. Before editing read:

- `docs/estandar-procedimientos-almacenados.md`
- `docs/estandar-auditoria-base-datos.md`
- `docs/ARQUITECTURA-COMERCIAL.md`
- `references/sql-checklist.md`

SQL Server is the implemented production provider. `database/sql` is the current authoritative legacy script location; do not move scripts to a proposed folder during an unrelated feature.

## Scope and deployment classification

Classify every script before writing:

```text
Master/global governance, users, roles, permissions, companies, integrations?
  -> Master script
Tenant/company catalog or operation?
  -> tenant script
Shared tenant prerequisite used by later feature scripts?
  -> earlier tenant foundation script
Data repair/hotfix after an already executed script?
  -> new forward-only idempotent script; never rewrite deployment history silently
```

Use the next numeric prefix after inspecting all existing files. State target database and prerequisite scripts in the header. A Master script must not assume tenant tables; a tenant script must not query Master directly.

## Stored-procedure contract

Use `SP_NA_{VERBO_HTTP}_{ENTIDAD}{ACCION}` and preserve established plural/legacy variants when changing an existing vertical. CRUD commonly requires:

- list;
- active lookup when applicable;
- detail by id;
- existence by unique key with exclusion id;
- create returning the created id;
- update returning affected/result indicator;
- logical delete returning affected/result indicator;
- history when the maintenance exposes History.

Every repository call must use `CommandType.StoredProcedure`, parameterized `CommandDefinition`, and the caller's `CancellationToken`. Procedure names remain in Persistence.

## Schema rules

- Use explicit SQL Server types aligned with C# validators and Dapper DTOs.
- Add primary keys, foreign keys, checks, defaults, and indexes deliberately.
- Use filtered unique indexes such as `WHERE IsDeleted = 0` for reusable codes after logical deletion.
- Use UTC timestamps with `SYSUTCDATETIME()`.
- Do not silently truncate or convert invalid values.
- Defend closed code sets in both Application and SQL.
- Keep external/SAP identifiers nullable unless the domain contract requires them; never use them as local identity by convenience.

## Audit and delete

Maintenance tables use the documented create/update/delete audit columns and normally logical deletion. Detailed history belongs to the approved domain audit table and must be written in the same local SQL transaction as the change.

The validated independent-master sequence is:

- `106_tenant_catalog_audit_foundation.sql` creates `AuditCatalogChanges`;
- `107_tenant_carriers.sql` creates Carriers, CRUD/history procedures, constraints, and transactional audit;
- `108_master_carriers_security.sql` registers menu/form/permissions;
- `109_master_carriers_permission_hotfix.sql` repairs installations where security was executed before API permission grants;
- `110_tenant_carriers_concurrency_hardening.sql` is the forward tenant repair for nonblank checks, locked write decisions, affected-row verification, and stable duplicate results;
- `111_master_carriers_operations_hardening.sql` is the forward Master repair that restricts automatic grants to the real corporate CRUD/grid actions without overwriting intentionally modified permissions.

Fresh installations receive the hardened definitions from `107`/`108`; existing installations must apply `110`/`111` instead of editing execution history. This sequence is evidence for deployment dependencies and forward-only repair, not a generic domain template.

## Idempotency and forward evolution

Use `IF OBJECT_ID`, `COL_LENGTH`, `IF NOT EXISTS`, and `CREATE OR ALTER PROCEDURE` as applicable. Idempotency means rerunning the script reaches the intended state without duplicates or data loss.

Once a script may have been executed:

- do not rename/reorder it as the only fix;
- add a later repair/migration script;
- make data backfill explicit and bounded;
- preserve compatibility until code and schema rollout order is safe;
- describe rollback/recovery for destructive or irreversible changes.

### SQL Server batch compilation gate

SQL Server compiles a batch before executing it. When a migration adds a
column, never reference that column statically later in the same batch, even
when the `ALTER TABLE` is guarded by `COL_LENGTH` or appears earlier inside a
transaction. Use separate batches in this order:

1. add columns as nullable;
2. `GO`;
3. backfill and validate;
4. make columns required and add defaults/checks/indexes;
5. `GO` before procedures that consume the evolved schema.

Before deployment, run:

```powershell
& .codex/skills/nuansystem-sql-standards/scripts/Test-SqlMigrationBatches.ps1 `
  -Path database/sql/<migration>.sql
```

The validator must pass for the real migration and fail for a negative fixture
that adds and consumes a column in one batch. Dynamic SQL is not reported
because it compiles when executed; use it only when a batch boundary is not
viable and document the reason.

Validate migrations against three schema states when practical: legacy before
the new columns, partially evolved, and complete/rerun. A static pass is not a
substitute for two authorized executions against a backed-up representative
database.

## Operational SQL

For stock, money, purchases, cash, documents, workflow, sync, or outbox:

- define the Application use-case transaction first;
- reload/guard authoritative state;
- prevent duplicate execution with status/version/idempotency constraints;
- commit audit and durable intent with local state;
- never hold a SQL transaction across SAP/HTTP calls;
- design retry/reversal/reconciliation explicitly.

## Security scripts

A navigable secured feature may require distinct but aligned records:

- `Modules` and `Permissions` for API policies;
- `RolePermissions` for role grants;
- `SecurityForms`, `SecurityMenus`, and form operations for UI/form authorization.

Form-operation grants do not replace API permission grants. Runtime permission validation must use a newly issued JWT when permissions are claim-based.

## Antipatterns

- Inline CRUD SQL in a Dapper repository.
- Mixed Master/tenant ownership.
- Editing an executed script without a forward repair.
- Script number chosen without scanning the directory.
- Unfiltered unique constraint that prevents valid code reuse after logical deletion when reuse is approved.
- History written outside the data-change transaction.
- C#/SQL length, nullability, parameter, result-column, or code-set drift.
- Claiming successful execution from static inspection only.

## Completion gate

- [ ] Target database, ownership, order, prerequisites, and rerun behavior are explicit.
- [ ] Table/procedures/parameters/results align with Application and Persistence.
- [ ] Audit, logical delete, constraints, indexes, UTC, and transactions are correct.
- [ ] API permission and form-operation seeds are both addressed when applicable.
- [ ] Existing-installation migration/repair and recovery are documented.
- [ ] Added columns are separated from static use by `GO`; `Test-SqlMigrationBatches.ps1` passes.
- [ ] Legacy, partial, and complete/rerun schema states were tested or explicitly reported as not validated.
- [ ] Execution status is reported as Executed, Statically validated, Not executed, or Blocked.
