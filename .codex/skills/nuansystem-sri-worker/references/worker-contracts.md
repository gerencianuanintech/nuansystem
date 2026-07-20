# SRI worker contracts and evidence

## Reusable lifecycle evidence

- `NuanSystem.SyncWorker` demonstrates .NET Worker/Windows Service hosting and SAP-specific scheduled processing.
- `NuanSystem.MasterBranchSyncWorker` demonstrates bounded loops, durable claims/locks, audit, and recovery for internal replication.
- Their hosting and reliability techniques may be studied, but their queues, handlers, ownership, and integration semantics are not reusable SRI contracts.

## Implemented Phase 5.3 baseline

- `src/Backend/NuanSystem.SriWorker` owns the bounded BackgroundService and official offline authorization SOAP adapter.
- `SriWorkerRepository` resolves enabled companies from Master and all queue mutations through tenant stored procedures.
- `117_tenant_sri_worker_and_document_store.sql` defines atomic claims, expiring-lease recovery, attempt completion and the immutable tenant XML store.
- The implementation supports Test and Production endpoints with strict TLS, remains disabled by default and has not made a real SRI call during development.
- Script `117` must be deployed and verified in each pilot tenant before the worker can be enabled.

## Approved first pilot

The worker will eventually query and download previously authorized documents by access key. It will not generate, sign, submit, or cancel electronic documents. The approved state machine and evidence gates are in `docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md`.

The approved provider is the SRI offline authorization service. XML is stored in the tenant database, capped at 5 MiB, with no automatic deletion. Defaults are batch 10, concurrency 2, 120-second lease, 30-second timeout, five technical attempts and three no-authorization responses within a 30-minute window.

## Validation hierarchy

1. static contract review;
2. build and unit/integration tests;
3. database concurrency and recovery tests;
4. worker lifecycle tests;
5. authorized non-production SRI round trip;
6. deployment-specific smoke test.

Do not promote evidence from a lower level as proof of a higher one.

Current evidence reaches level 2. Database concurrency/recovery, worker lifecycle against a deployed tenant and official non-production round trip remain pending.
