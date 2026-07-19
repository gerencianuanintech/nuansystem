---
name: nuansystem-backend-testing
description: Plan, create, run, or review NuanSystem backend unit, contract, integration, authorization, tenant-isolation, persistence, synchronization, and regression tests. Use whenever a backend feature, command/query handler, validator, endpoint, repository, SQL contract, transaction, permission, or integration behavior changes.
---

# NuanSystem Backend Testing

## Test from risk and contracts

Start with the vertical contract and failure modes. Do not treat a successful build or one happy-path handler test as complete validation.

## Required test map

### Application

- Valid command/query behavior.
- Validation boundaries and closed sets.
- Normalization and stable error codes.
- Not-found and uniqueness/conflict paths.
- Current-state/transition/concurrency behavior for operations.
- Cancellation propagation where meaningful.

### Persistence/SQL

- Parameters and returned columns map to C# contracts.
- Create/list/detail/update/delete round trip where integration infrastructure exists.
- Constraints, logical delete, audit fields, affected rows, and idempotent scripts.
- Transaction rollback for multi-write failures.

### API/security

- Route/request/response contract.
- Authentication and company requirement.
- Allowed and denied permission/form-operation cases.
- Safe 400/401/403/404/409/500 behavior as applicable.

### Tenant

- Two companies do not see/change each other's data.
- Missing/invalid/unauthorized company is rejected.
- Company change does not reuse stale ids or cached data.

### Integration/sync

- Publishing/apply behavior remains correct when the entity participates.
- Idempotency, duplicate event, retry, terminal failure, and source-of-truth behavior.
- Explicit negative test when a new independent entity must not enter BusinessPartners/SAP/sync.

## Repository references

- Handler/auth examples: `tests/NuanSystem.Application.Tests/Features/Auth`.
- Operational policy/handler tests: `Features/Purchasing/PurchaseOrders`.
- Publish regression: Geography, BusinessPartners, Warehouses, FinancialCatalogs tests.
- Sync contract tests: `Features/Sync`.
- SQL Server conditional infrastructure: `Infrastructure/SqlServerIntegrationFactAttribute.cs`.

Use these as testing techniques, not domain templates.

## Test quality

- Name tests by behavior and expected outcome.
- Assert returned value/error code and important repository interactions.
- Keep domain decisions visible in fixtures; avoid unexplained magic values.
- Use fakes/mocks for Application isolation; use real SQL only in explicit integration tests.
- Do not make tests pass by weakening production validation or skipping failures silently.
- Add a regression test before or with a defect fix when practical.
- Keep environment-dependent tests clearly marked and report when they were not executed.

## Validation execution

Run the narrowest meaningful tests first, then the affected project/solution build and broader tests proportional to risk. Report exact commands and outcomes using Validated, Not validated, Not applicable, or Blocked.

## Antipatterns

- Tests that only assert no exception.
- Mocking the behavior under test.
- One test covering multiple unrelated outcomes.
- Ignoring tenant/authorization because UI hides the action.
- Claiming SQL validation from static C# tests.
- Snapshotting unstable implementation details instead of contracts.

## Completion checklist

- [ ] Happy, invalid, conflict/not-found, and permission paths are covered.
- [ ] Tenant isolation is covered for company-scoped data.
- [ ] Persistence/SQL contract is validated at the appropriate level.
- [ ] Operational transaction/failure behavior is covered.
- [ ] Integration/sync impact is tested or explicitly not applicable.
- [ ] Commands and outcomes are reported truthfully.
