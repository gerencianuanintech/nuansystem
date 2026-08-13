# SQL Change Checklist

## 1. Discovery and ownership

- [ ] Read the stored-procedure, audit, commercial, and tenancy rules applicable to the change.
- [ ] Scan all `database/sql` filenames and select the next valid numeric prefix.
- [ ] Classify the object as Master, tenant, shared tenant foundation, operational, or forward repair.
- [ ] Name prerequisite scripts and deployment order.
- [ ] Inspect the Application contract, Persistence repository, endpoint, frontend consumer, and tests.

## 2. Table/schema

- [ ] Primary key and identity/global identity semantics are explicit.
- [ ] Required/optional fields, SQL types, lengths, precision, and defaults match C#.
- [ ] Foreign keys and ownership stay inside the correct database boundary.
- [ ] Business uniqueness is protected; logical-delete filtering is intentional.
- [ ] Closed code sets and non-blank requirements have check constraints when authoritative.
- [ ] UTC audit fields and logical-delete fields follow the approved standard.
- [ ] Indexes support real list, lookup, uniqueness, history, and operational access paths.
- [ ] SAP/external fields remain optional and are not local identity unless explicitly required.
- [ ] Every newly added column is separated by `GO` from its first static `UPDATE`, `SELECT`, constraint, index, or procedure reference.

## 3. Procedures and Dapper

- [ ] Use `SET NOCOUNT ON`; use `SET XACT_ABORT ON` for explicit transactions.
- [ ] Use `CREATE OR ALTER PROCEDURE`.
- [ ] List/detail/existence/lookup exclude deleted rows according to the contract.
- [ ] Active lookup behavior is explicit.
- [ ] Create returns the new id; update/delete return the exact scalar/row semantics expected by the repository.
- [ ] Parameter names, nullability, sizes, and result aliases map exactly to Dapper contracts.
- [ ] Repository uses `CommandType.StoredProcedure` and propagates cancellation.
- [ ] No procedure name or SQL-provider type leaks into Application.

## 4. Audit and transactions

- [ ] Audit user comes from authenticated backend identity, not request authority.
- [ ] Detailed history uses the approved domain audit table.
- [ ] Data change and detailed audit commit or roll back together.
- [ ] Update history records only changed fields when that is the established contract.
- [ ] Logical delete records state/audit consistently.
- [ ] Multi-write operational work has one explicit transaction boundary.
- [ ] No SQL transaction remains open during SAP/HTTP/external calls.

## 5. Security and tenancy

- [ ] Tenant scripts do not query/write Master; Master scripts do not assume tenant objects.
- [ ] API `Permissions` and `RolePermissions` are created for secured endpoints.
- [ ] `SecurityForms`, menus, and form operations align when the feature is navigable.
- [ ] Permission codes, FormKey, action keys, endpoint policies, and frontend constants match.
- [ ] Runtime authorization is tested with a renewed token after permission changes.

## 6. Evolution and evidence

- [ ] Script is safely rerunnable and does not duplicate data/grants.
- [ ] `Test-SqlMigrationBatches.ps1` passes for every changed migration and rejects a same-batch negative fixture.
- [ ] Legacy-without-columns, partially evolved, and complete/rerun states were tested or explicitly marked not validated.
- [ ] Existing installations receive a forward migration/hotfix rather than silent history edits.
- [ ] Backfill, compatibility window, destructive risk, and recovery are explicit.
- [ ] Static SQL checks distinguish syntax/contract inspection from real execution.
- [ ] Exact executed scripts, target databases, commands/tools, and results are recorded.
- [ ] Build/tests cover affected handlers, repositories/contracts, permissions, and regressions.
