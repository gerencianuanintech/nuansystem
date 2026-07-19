---
name: nuansystem-backend-persistence
description: Create, modify, or review NuanSystem Application repository contracts, Dapper Persistence repositories, stored-procedure calls, tenant connections, mappings, transaction boundaries, cancellation, provider isolation, and dependency registration. Use for repository, SQL contract, Dapper, ITenantConnectionFactory, ITransactionRunner, or Persistence changes.
---

# NuanSystem Backend Persistence

## Authority and discovery

Follow `$nuansystem-backend-architecture` and `$nuansystem-sql-standards`. Inspect the Application abstraction, same-domain repository, called procedures, DI registration, consuming handlers, and tests before editing.

## Ownership

- Application owns repository/service interfaces and use-case-shaped data contracts.
- Persistence owns Dapper, connections, procedure names, provider-specific behavior, transactions, and mapping.
- Domain and Application must not depend on `SqlConnection`, Dapper, `CommandType`, or stored-procedure names.

## Repository rules

- Use `ITenantConnectionFactory` for tenant-scoped data and the established master connection path for master data.
- Create/dispose a connection per established repository method unless participating in an explicit transaction.
- Use `CommandDefinition` with `commandType: CommandType.StoredProcedure` and the caller's `CancellationToken` for SQL Server CRUD.
- Keep parameter names/types and result column aliases aligned with C# records/DTOs.
- Use `QuerySingleOrDefaultAsync` for optional detail, `QueryAsync` for collections, and scalar/affected-row semantics matching the procedure contract.
- Convert result sequences to stable read-only/list shapes before returning.
- Register concrete repositories in `PersistenceServiceRegistration.cs` exactly once.
- Do not expose connection strings, provider types, SQL exceptions, or procedure names above Persistence.

## Contract design

Prefer methods named for business persistence needs, not generic table mechanics. Keep independent domain owners in independent interfaces/repositories even when their fields resemble another catalog.

Do not introduce a generic repository or descriptor until multiple consumers prove an identical stable contract and the abstraction preserves routes, permissions, validation, audit, and SQL ownership.

## Transaction tree

```text
One procedure owns one atomic write?
  -> procedure transaction/atomicity may be sufficient; verify it
Multiple writes must commit together?
  -> use ITransactionRunner and transaction-aware repository methods
External call involved?
  -> do not hold a database transaction across remote calls;
     use durable intent/outbox and recovery where required
```

`SqlTransactionRunner` is the existing tenant transaction implementation. Transaction ownership belongs to the Application use case; Persistence executes it.

## SQL alignment

For every change verify:

- procedure constant/name exists;
- parameter names, nullability, lengths, precision, and code sets match;
- returned column names/types map correctly;
- logical delete filters and audit fields are consistent;
- uniqueness/check/FK constraints defend authoritative invariants;
- scripts are idempotent according to deployment conventions.

## Provider isolation

SQL Server is production authority. Keep SQL Server syntax in SQL Server scripts/Persistence. Do not weaken its design for hypothetical portability. If another provider is implemented, preserve Application contracts and create provider-specific implementations/scripts.

## Representative references

- CRUD procedures: `Repositories/Geography/GeographyRepository.cs`.
- Complex multi-result mapping: `Repositories/BusinessPartnerRepository.cs`.
- Tenant transaction: `Transactions/SqlTransactionRunner.cs`.
- Registration: `DependencyInjection/PersistenceServiceRegistration.cs`.

## Antipatterns

- Inline CRUD SQL in repositories.
- Handler opening a connection or naming a procedure.
- Company code supplied as an untrusted repository parameter when context owns it.
- Transaction per repository method for a multi-write use case.
- Returning `true` without checking affected rows.
- Schema/DTO drift hidden by manual defaults.

## Completion checklist

- [ ] Scope uses the correct master/tenant connection.
- [ ] Interface and implementation describe the same contract.
- [ ] Dapper parameters/results match SQL exactly.
- [ ] Cancellation, disposal, affected rows, and transactions are correct.
- [ ] DI registration and all consumers are updated.
- [ ] SQL tests/static checks and targeted build are reported.
