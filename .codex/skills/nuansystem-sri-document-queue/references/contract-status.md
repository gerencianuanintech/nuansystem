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

## Implemented after Phase 5.2

- Phase 5.3 added the worker, official provider and immutable XML store; Phase 5.4 validated one authorized Production round trip in `NuanSystem_DEMO`.
- Phase 5.5 implements safe monitor projections, protected byte download and per-access audit in tenant script `118`, plus Master navigation/security in script `119`.
- `SriDocumentMonitorForm` uses the typed API client and corporate monitor controls without direct SQL or SRI access.
- Phase 5.5 is deployed in Master and the three authorized tenant schemas. Idempotent SQL, renewed-token permission profiles, tenant isolation, protected API download, byte length/SHA-256 integrity, per-access audit, repeated-download behavior and Visual Studio Designer review were validated without starting the worker or calling the SRI.
- Forward repair `123` aligns every monitor-summary aggregate with the public `long` DTO by returning non-null SQL `bigint` values. It was executed twice in `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO`, and `NuanSystem_DEMO_CANARIS`; each tenant retained one schema-history version and one procedure with five `bigint` result columns. Real Dapper materialization returned `4/0/0/1/0` in DEMO and `0/0/0/0/0` in both branches, with protected counts unchanged. The DEMO monitor was opened and refreshed visually; branch validation was persistence-only. No worker or SRI call was started, and Master was not changed by this repair.
- The detailed sanitized evidence is `docs/operations/SRI-DOCUMENT-MONITOR.md`; retain only this summary in the skill reference.

## Implemented and runtime-validated for SRI TXT Import core

- `Application/Features/SriTxtImports` implements bounded streaming parsing for strict UTF-8 and Windows-1252, normalized 12-column rows, SHA-256 identities, masking, validation and explicit enqueue commands.
- Tenant script `138_tenant_sri_txt_import.sql` defines import header/detail/audit, file-hash idempotency, TVP persistence, queue linking and the forward-only `Staged` state.
- Valid upload rows create or link one `SriDocumentQueue`; invalid rows never create queue records. The complete access key is transient in Application/TVP and persists only in `SriDocumentQueue`.
- `Staged -> Pending` is the only newly approved transition. It is atomic, concurrency-controlled and audited. Repeated enqueue does not create a queue, attempt or duplicate event.
- The worker claim procedure in script `117` remains unchanged and continues selecting only `Pending` and `RetryScheduled`.
- `SriTxtImportEndpoints` exposes multipart upload and explicit enqueue through independent permissions.
- Master script `139_master_sri_txt_import_security.sql` registers the approved aliases without grants to existing roles and without WinForms/menu records.
- Scripts `138`/`139`, API multipart, SQL concurrency, idempotent enqueue, rollback, claim exclusion,
  permissions and isolation were runtime-validated under the approved gate; do not repeat that
  evidence when validating the CRUD extension.
- The authorized CRUD extension adds server-paged list/detail/row projections, safe `QueueId`
  navigation and an independent corporate WinForms monitor. It must never project the complete
  access key, raw TXT/header line, XML, JWT, connection or secret.
- Forward SRI scripts `142`/`143` are implementation artifacts only until separately authorized and
  must not auto-grant existing roles. Scripts `140`/`141` are owned exclusively by PriceList 8.6.
  Query, upload and enqueue permissions remain independent.
- CRUD implementation evidence after the PriceList merge: full solution build has zero
  errors/warnings; 30 focused TXT tests, 89 SRI tests and 569 full-suite tests pass, with 5
  infrastructure tests explicitly skipped.
- SAP remains outside this implementation.

## Approved pilot direction

The first pilot is query and download by access key for previously authorized documents. Emission, signing, submission, cancellation, and portal scraping are excluded. Its functional contract, identity, states, security rules, and unresolved infrastructure decisions live in `docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md`.

The queue, worker baseline, immutable tenant XML storage and protected monitor/download are implemented and validated through their approved gates. Automatic retention/deletion and any broader provider operation remain outside the approved pilot.

## Evidence rule

Search the current repository before every task. If implementation advances, update this reference in the same change. A document or test double is not proof that a production path exists.
