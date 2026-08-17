---
name: nuansystem-backend-architecture
description: Orchestrate non-trivial NuanSystem backend work across Domain, Application, Persistence, API, SQL, security, audit, integrations, and tests. Use for backend features, CRUD or operational use cases, repositories, transactions, authorization, and refactors.
---

# NuanSystem Backend Architecture

## Authority

Run `$nuansystem-framework-discovery` first and reuse its engineering-core
record. Do not reload the complete core or catalogs from this orchestrator.
Then apply this orchestrator, only the affected backend specialists, and a
repository-backed reference implementation.

Never let a reference redefine explicit domain ownership.

## Classification

```text
Administrative master/configuration?
  -> $nuansystem-backend-crud
Stock, money, prices, cash, documents, workflow, sync, or external state?
  -> $nuansystem-operational-usecase
Shared/backend infrastructure?
  -> inspect all consumers and use the framework-evolution process
```

Activate as applicable:

- `$nuansystem-mediatr-cqrs`
- `$nuansystem-backend-endpoints`
- `$nuansystem-backend-validation`
- `$nuansystem-backend-persistence`
- `$nuansystem-backend-multitenancy-security`
- `$nuansystem-backend-testing`
- `$nuansystem-api-error-logging`
- `$nuansystem-security-auth`
- `$nuansystem-sql-standards`
- `$nuansystem-database-provider-compatibility`
- `$nuansystem-business-capabilities`
- `$nuansystem-sap-business-one`

## Mandatory discovery order

Inspect the owning vertical end to end:

```text
Domain/entity or contract
  -> Application DTOs
  -> commands/queries and handlers
  -> validators
  -> repository interfaces/data records
  -> Persistence repository and transaction services
  -> SQL scripts/procedures
  -> Minimal API endpoints and authorization
  -> frontend client/consumer
  -> tests
  -> sync/integration consumers
```

Representative evidence:

- Validated independent CRUD: `Features/Carriers`, `CarrierRepository.cs`, `CarrierEndpoints.cs`, and SQL scripts `106` through `109`.
- CRUD lifecycle: `Features/Geography` and `GeographyEndpoints.cs`.
- Configurable catalog lifecycle: `Features/TaxCatalogs` and `TaxCatalogRepository.cs`.
- Complex aggregate: `Features/BusinessPartners`.
- Operational transaction: `Features/Purchasing/PurchaseOrders`.
- Tenant connection: `Connections/TenantConnectionFactory.cs`.
- Transaction boundary: `Transactions/SqlTransactionRunner.cs`.

Use repository-relative paths rooted at `src/Backend` unless a full path is needed for delivery.

## Layer ownership

### Domain

Own pure invariants, value semantics, state transitions, and calculations without Dapper, HTTP, claims, tenant headers, or SAP clients. Do not create a Domain entity merely to satisfy a checklist; confirm the owning vertical's current pattern.

### Application

Own use cases, orchestration, repository/service abstractions, authoritative validation, normalization, stable business errors, and transaction intent. Use `ICommand<T>`, `IQuery<T>`, handlers, `Result<T>`, and `CancellationToken` following `Application/Abstractions/Messaging`.

### Persistence

Own Dapper, connections, stored-procedure names, result mapping, SQL-provider behavior, and transaction implementation. Tenant data uses `ITenantConnectionFactory`.

### API

Own HTTP transport only: bind input, obtain trusted audit identity, send the use case, map `Result<T>`, and apply backend authorization.

### Integration

Own SAP/external adapters, mappings, retries, and workers. Application depends on interfaces, not adapters.

## Vertical contract record

Before implementation, record:

```text
Outcome:
Work type:
Domain owner and exclusions:
Affected layers:
Commands/queries:
Repository contracts:
Transaction boundary:
Tenant scope:
Authorization:
Audit identity:
SQL contract:
Integration/sync impact:
Tests:
Reference implementation:
Permitted reuse boundary:
```

## Dependency rules

```text
Api -> Application -> Domain
Persistence -> Application contracts (+ Domain when required)
Infrastructure/Integrations -> Application contracts
```

- Keep `Program.cs` as composition.
- Do not place SQL procedure names in Application.
- Do not inject Persistence implementations into handlers.
- Do not return Dapper rows, exceptions, or claims objects as domain contracts.
- Do not let endpoints implement uniqueness, authorization, calculations, or state transitions.
- Do not create a generic repository, generic endpoint, or descriptor merely to reduce file count without proving a stable shared contract.
- Do not describe proposed types or provider support as implemented; verify exact symbols and executable paths first.

## Completion gate

- [ ] Discovery and explicit domain ownership are recorded.
- [ ] CRUD versus operational classification is correct.
- [ ] Every affected layer is changed or verified explicitly.
- [ ] Tenant, authorization, audit, errors, cancellation, and transactions are preserved.
- [ ] SQL and C# contracts agree.
- [ ] Integration/synchronization consumers were inspected.
- [ ] Targeted tests and build evidence are reported truthfully.
- [ ] `.codex/REVIEW-CHECKLIST.md` was executed.
