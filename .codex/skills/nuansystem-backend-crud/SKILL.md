---
name: nuansystem-backend-crud
description: Build or review complete NuanSystem administrative CRUD backend verticals across Application, Persistence, SQL, API, permissions, tenant scope, audit, lookups, and tests. Use for maintained masters and configuration; use the operational skill for stock, money, documents, workflow, synchronization, or external state.
---

# NuanSystem Backend CRUD

## Authority and activation

Follow `$nuansystem-backend-architecture` and run `$nuansystem-framework-discovery` first. Activate endpoint, validation, persistence, tenant-security, SQL, and testing skills for the affected layers.

```text
Changes stock, money, prices, cash, documents, workflow, sync, or external state?
  -> stop CRUD routing and use $nuansystem-operational-usecase
Otherwise administers an independent master/configuration?
  -> continue with this skill
```

## Preserve ownership

An existing feature with similar fields is a lifecycle reference, not permission to reuse its entity, repository, API, SQL, sync, or forms. Preserve explicit independent masters as independent vertical contracts.

## Required vertical

```text
Domain/entity or explicit contract when behavior requires it
  -> Application DTOs
  -> list/detail/lookup queries and handlers
  -> create/update/delete commands and handlers
  -> FluentValidation
  -> repository interface and save data records
  -> Dapper Persistence repository
  -> tenant/master SQL table and procedures
  -> Minimal API route group and authorization
  -> permission/menu/form-operation registration
  -> typed frontend consumer
  -> tests
  -> sync/integration impact verification
```

Do not create Domain types mechanically when the repository's approved pattern keeps a simple catalog contract in Application. Do create/extend Domain when the master owns meaningful pure behavior or invariants.

## Application contract

- Use feature folders under `Application/Features/{Owner}` with the nearby naming convention.
- Use explicit DTOs for list, detail, lookup, history, and save needs; do not duplicate solely to rename fields.
- Commands/queries implement the existing messaging abstractions and handlers return `Result<T>`.
- Normalize values before existence checks and persistence.
- Use stable `ApiError` codes for duplicate, not-found, invalid relationship, or forbidden state.
- Pass `CancellationToken` end to end.
- Keep SQL, HTTP, claims, WinForms, and provider types out of Application.

## CRUD behavior

### Create

Validate shape, normalize, verify authoritative uniqueness/relationships, persist, reload the authoritative DTO when required, publish only approved sync events, and return the created contract.

### Update

Validate id/input, reload or verify existence, normalize, check uniqueness excluding the current id, persist with audit identity, handle zero affected rows, and return the updated authoritative contract.

### Delete

Define physical versus logical semantics explicitly. Maintenance masters normally use logical deletion with delete audit fields. Verify dependencies or approved delete rules; do not silently delete another aggregate through a specialized view.

### List/detail/lookup

- Lists and lookups exclude logically deleted data and respect active/company rules.
- Detail returns a not-found failure through the handler.
- Lookup contracts are bounded and owned by the independent master.
- Pagination/filtering must be server-side when volume or security semantics require it.

## Auxiliary masters

Each administrable auxiliary master owns its own Application feature, DTOs, handlers, validator, repository, table/procedures, routes, permissions, lookup, frontend maintenance, and tests. A parent consumes stable Id/code values; it does not own the catalog CRUD.

Fixed closed enumerations may remain code sets when they are non-administrable and do not vary by company. Persist stable codes and defend the set in Application and SQL.

## Audit, tenant, permissions, and sync

- Obtain audit identity from authenticated claims at the endpoint.
- Resolve tenant data through trusted company context and `ITenantConnectionFactory`.
- Enforce read/manage or form-operation authorization in backend.
- Keep FormKey/action/permission constants and Master security scripts aligned.
- Search sync publishers, apply handlers, outbox/inbox, full-entity sources, and integration mappings before classifying sync as unchanged.
- Do not enroll a new master into synchronization without explicit source-of-truth and distribution requirements.

## Representative references

- Validated full independent master: `Application/Features/Carriers`, `ICarrierRepository`, `CarrierRepository`, `CarrierEndpoints`, tenant scripts `106`/`107`/`110`, Master scripts `108`/`109`/`111`, and Carrier tests. Its write contract distinguishes not-found from a database-detected concurrent duplicate so handlers preserve stable `ApiError` codes. Reuse lifecycle only; Transportistas remains its own owner.
- Simple CRUD: `Application/Features/Geography`, `GeographyRepository.cs`, `GeographyEndpoints.cs`.
- Descriptor/configurable catalogs: TaxCatalogs, after confirming descriptor ownership fits.
- Complex master: BusinessPartners, as aggregate evidence only.

## Antipatterns

- Endpoint-first or table-first implementation without vertical discovery.
- Generic catalog endpoint/form for unrelated domain owners.
- Inline SQL or stored-procedure names in Application.
- UI-only uniqueness/authorization.
- Audit user or company accepted from the request.
- Copying another master's sync behavior automatically.
- Creating only list/create while leaving edit/delete/history/contracts partial.

## Completion checklist

- [ ] Ownership, fields, uniqueness, delete, scope, permissions, audit, and sync decisions are explicit.
- [ ] Full vertical contracts align.
- [ ] Validation and SQL constraints match.
- [ ] All endpoints are authorized.
- [ ] Tenant isolation and audit identity are trusted.
- [ ] Create/update/delete/list/detail/lookup and failure tests exist.
- [ ] Relevant builds/tests and review gates are reported.
