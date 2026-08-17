---
name: nuansystem-sap-business-one
description: Design, implement, review, or diagnose NuanSystem SAP Business One transport, company configuration, Service Layer, DI API, HANA reads, catalog mappings, previews, imports, and document sends. Use whenever a task touches NuanSystem.SapIntegration, /api/sap, SAP credentials, sessions, external identifiers, or SAP DTO mapping.
---

# NuanSystem SAP Business One

## Authority

Run `$nuansystem-framework-discovery` and reuse its Constitution, Kernel,
catalog, and graph record. Then apply this skill and repository evidence.

Load `$nuansystem-sap-sync-orchestration` for locks, schedules, workers, retries, logs, watermarks, inbox, or outbox. Load `$nuansystem-master-branch-sync` only for NuanSystem Matriz-Sucursal replication; it is not SAP transport.

## Classification

```text
Read/preview SAP master data?
  -> implemented typed reader + Service Layer query client, or HANA only where evidence proves it
Send an SAP document?
  -> ISapDocumentSender / ISapClientFactory and implemented payload
Scheduled/retriable synchronization?
  -> $nuansystem-sap-sync-orchestration
Replicate NuanSystem Master data to branches?
  -> $nuansystem-master-branch-sync
```

Do not select a transport from preference. Inspect active company configuration and the closest implemented vertical.

## Implemented contracts

- `SapIntegrationMode`: `None`, `ServiceLayer`, `DiApi`.
- `ISapClientFactory.Create(mode)` selects document transport and rejects disabled/unknown modes.
- `SapIntegrationServiceRegistration` registers named client `SapServiceLayer` with cookies disabled to prevent cross-company session reuse.
- Typed readers exist for suppliers, warehouses, items, and purchase orders.
- HANA query/connection abstractions belong to Integration, never frontend or Application shortcuts.
- `/api/sap/*` dispatches Application use cases and requires `SapRead` or `SapManage`.
- Existing item and warehouse imports persist `ExternalSystem = "SAP_B1"`; reuse that exact identifier unless a migration explicitly changes the cross-vertical convention.
- Permission values are `SAP.SYNC.READ` and `SAP.SYNC.MANAGE` through `PermissionCodes.SapRead` and `PermissionCodes.SapManage`.

Read `references/implemented-contracts.md` before design.

## Mandatory design record

```text
Outcome/source of truth:
Company/tenant scope:
Direction: SAP->ERP | ERP->SAP | Query only
Transport and evidence:
External entity/object code:
Local owner and explicit exclusions:
Mapping/external identity:
Session/credential source:
Idempotency key:
Failure classification:
Audit/observability:
Recovery/reconciliation:
Implemented gaps:
Tests/SQL scripts:
```

## Rules

- Keep credentials/company configuration in established repositories; never hard-code or return secrets.
- Create sessions per company/request through registered clients. Never share cookies/authenticated state across companies.
- Map SAP payloads in Integration/Application contracts, not endpoints or WinForms.
- Preview remains read-only; import commands own local writes and authoritative validation.
- Persist stable SAP identifiers/version data required for idempotency before claiming success. Do not shorten `SAP_B1` to `SAP` in new mappings.
- Keep remote calls outside open SQL transactions. Persist durable intent when delivery must survive failure.
- Propagate cancellation and use bounded reads/timeouts.
- Log safe correlation, entity, company, status, counts, and error class; never passwords, cookies, or sensitive payloads.
- Treat DI API as Windows/COM infrastructure; do not claim portability or concurrency without executable evidence.
- Keep HANA adapter SQL separate from tenant SQL Server scripts.
- Do not infer SAP ownership for an independent master. `Transportistas` has no SAP mapping without an approved requirement.

## Antipatterns

- Direct HTTP, SQL, HANA, DI API, or Service Layer calls from forms/endpoints.
- Global SAP sessions shared among tenants.
- Retrying validation, duplicate, conflict, or mandatory-data errors.
- Claiming success before the implemented SAP result confirms it.
- Inventing readers, entity codes, tables, or completed export paths.
- Reusing BusinessPartner SAP contracts for an independent entity because fields look similar.

## Completion gate

- [ ] Transport is backed by code/configuration evidence.
- [ ] Tenant isolation, sessions, secrets, mapping, idempotency, cancellation, and transaction boundaries are explicit.
- [ ] Permissions, audit, failure classification, recovery, and observability are covered.
- [ ] `NotImplemented`, skeleton, or disabled paths remain reported as pending.
- [ ] Adapter/handler/API tests and build were run or truthfully marked not validated.
- [ ] `.codex/REVIEW-CHECKLIST.md` was applied.
