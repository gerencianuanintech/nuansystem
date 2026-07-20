# SRI queue contract status

## Implemented evidence

- `TenantFeatureCodes.SriDocuments` defines capability `SRI_DOCUMENTS`.
- `TenantIntegrationCodes.Sri` defines integration code `SRI`.
- `database/sql/062_master_tenant_configuration.sql` seeds both disabled by default.
- `TenantIntegrationService` protects sensitive configuration values in reads.
- Existing tests verify that the SRI capability is disabled by default.
- `docs/architecture/SRI-DOCUMENTS-WORKER.md` describes the target separation between capturers, queue, worker, and storage.

## Implemented in Phase 5.2

- Application commands/queries validate and manage enqueue, listing, detail, attempts, cancellation, and manual reprocess.
- `SriDocumentQueueRepository` uses versioned stored procedures through Dapper.
- `115_tenant_sri_document_queue.sql` defines the tenant queue, immutable attempts, audit, database idempotency, legal manual transitions, indexes, and `rowversion` concurrency.
- `116_master_sri_document_queue_security.sql` registers the six approved permission aliases without creating a UI/menu.
- `/api/sri/documents` endpoints use the granular view/enqueue/cancel/reprocess permissions.
- The initial evidence-backed types are invoice `01`, credit note `04`, and withholding document `07`; real samples are not committed.

## Runtime validation evidence

- Script `116` is installed idempotently in `NuanSystem_Master`; script `115` is installed idempotently in `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO`, and `NuanSystem_DEMO_CANARIS`.
- Schema histories, all queue objects, the unique `(Environment, AccessKey)` index, six procedures, six permissions, and ADMIN grants were verified without duplicates.
- A renewed ADMIN JWT contained all SRI permissions; a real user without them received HTTP 403.
- Two simultaneous enqueue calls returned the same queue Id and one database row.
- Pending cancellation, stale `rowversion`, invalid repeated cancellation, and reprocess rules for `Failed`/`DeadLetter` were verified through the API.
- Audit recorded enqueue, cancel, and reprocess actions; no attempts or XML were created without a worker.
- Validation passed 17 SRI tests and the full 411-test suite; the API and ephemeral users were removed afterward.
- `NuanSystem_DEMO` is enabled for the Production pilot. Remigio and Canaris remain disabled.

## Still not implemented

- No XML/document store, provider, monitor form, or `NuanSystem.SriWorker` project exists.
- No production SRI provider/client, certificate flow, environment selection, or end-to-end authorization/download flow exists.
- Payload/XML permissions are reserved but have no endpoint in Phase 5.2.

## Approved pilot direction

The first pilot is query and download by access key for previously authorized documents. Emission, signing, submission, cancellation, and portal scraping are excluded. Its functional contract, identity, states, security rules, and unresolved infrastructure decisions live in `docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md`.

The queue contract is now implemented in code. XML storage, retention, official validation environment, and operational limits remain pending before worker/provider phases.

## Evidence rule

Search the current repository before every task. If implementation advances, update this reference in the same change. A document or test double is not proof that a production path exists.
