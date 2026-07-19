---
name: nuansystem-database-provider-compatibility
description: Preserve NuanSystem SQL Server-first persistence boundaries while keeping future database providers isolated behind Application contracts. Use when changing DatabaseEngine, company connection resolution, ITenantConnectionFactory, Dapper repositories, provider-specific SQL, database scripts, or proposing MySQL support.
---

# NuanSystem Database Provider Compatibility

## Current truth

- `Domain/Tenancy/DatabaseEngine.cs` declares `SqlServer` and `MySql`.
- `Persistence/Connections/TenantConnectionFactory.cs` implements SQL Server through `Microsoft.Data.SqlClient`.
- MySQL currently throws `NotSupportedException`; it is an architectural placeholder, not a supported provider.
- Production SQL scripts currently live under legacy `database/sql` and use SQL Server stored procedures.

Never document or generate a working MySQL repository, dialect, package, connection class, or deployment script as if it already exists.

## Boundaries

- Domain may name the provider enum but must not know Dapper, connections, procedures, or SQL syntax.
- Application defines repository and transaction contracts without provider types.
- Persistence selects provider implementations and owns connection/procedure behavior.
- SQL Server scripts may use `CREATE OR ALTER`, `DATETIME2`, `SYSUTCDATETIME`, `SCOPE_IDENTITY`, filtered indexes, and other SQL Server features.
- Do not reduce SQL Server correctness to a lowest-common-denominator dialect for hypothetical portability.

## Decision tree

```text
Ordinary feature on current production platform?
  -> implement SQL Server contract under current repository/script conventions
Explicit approved MySQL implementation?
  -> define parity requirements, provider packages, connection factory behavior,
     provider-specific repositories/scripts, migration/tests, and deployment evidence
No explicit MySQL requirement?
  -> keep MySQL unsupported and do not add speculative abstractions
```

## Provider change gate

Before enabling a new provider, document and validate:

- supported features and excluded modules;
- Master and tenant connection resolution;
- secret protection and connection testing;
- repository/procedure parity;
- transaction/isolation semantics;
- types, precision, UTC timestamps, identity retrieval, filtered uniqueness, and error mapping;
- schema initialization/migration order;
- integration and rollback tests.

Keep Application contracts stable only when semantics truly match. Separate implementations are preferred when SQL differs materially.

## Antipatterns

- Invented `CompanyConnectionFactory` or `DatabaseProvider` replacing actual contracts in documentation only.
- Inline provider switches in handlers/endpoints.
- SQL Server and MySQL syntax in one script.
- Claiming provider support because an enum value exists.
- Adding dialect interfaces without two real consumers and tests.
- Passing raw connection strings through commands or frontend requests.

## Completion gate

- [ ] Current provider support is stated truthfully.
- [ ] Provider-specific details remain in Persistence/scripts.
- [ ] Master/tenant connection and secret boundaries are preserved.
- [ ] SQL Server behavior is not weakened.
- [ ] Any new provider has executable parity and isolation evidence.
