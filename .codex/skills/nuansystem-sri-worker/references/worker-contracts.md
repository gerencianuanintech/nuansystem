# SRI worker contracts and evidence

## Reusable lifecycle evidence

- `NuanSystem.SyncWorker` demonstrates .NET Worker/Windows Service hosting and SAP-specific scheduled processing.
- `NuanSystem.MasterBranchSyncWorker` demonstrates bounded loops, durable claims/locks, audit, and recovery for internal replication.
- Their hosting and reliability techniques may be studied, but their queues, handlers, ownership, and integration semantics are not reusable SRI contracts.

## Implemented Phase 5.3 baseline

- `src/Backend/NuanSystem.SriWorker` owns the bounded BackgroundService and official offline authorization SOAP adapter.
- `SriWorkerRepository` resolves enabled companies from Master and all queue mutations through tenant stored procedures.
- `117_tenant_sri_worker_and_document_store.sql` defines atomic claims, expiring-lease recovery, attempt completion and the immutable tenant XML store. It is deployed idempotently in `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` and `NuanSystem_DEMO_CANARIS`.
- The implementation supports Test and Production endpoints with strict TLS and remains disabled by default. Phase 5.4 later completed one explicitly authorized Production call in `NuanSystem_DEMO`; this evidence does not authorize another call.
- Script `117` and its SQL concurrency/integrity contract are verified in the three pilot tenants. This evidence does not authorize enabling the worker or calling the SRI.

## Approved first pilot

The worker will eventually query and download previously authorized documents by access key. It will not generate, sign, submit, or cancel electronic documents. The approved state machine and evidence gates are in `docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md`.

The approved provider is the SRI offline authorization service. XML is stored in the tenant database, capped at 5 MiB, with no automatic deletion. Defaults are batch 10, concurrency 2, 120-second lease, 30-second timeout, five technical attempts and three no-authorization responses within a 30-minute window.

## Validation hierarchy

1. static contract review;
2. build and unit/integration tests;
3. database concurrency and recovery tests;
4. worker lifecycle tests;
5. authorized SRI round trip in the explicitly approved environment;
6. deployment-specific smoke test.

Do not promote evidence from a lower level as proof of a higher one.

Current evidence reaches level 6 for the expressly approved `NuanSystem_DEMO` Production route: build/tests, deployed database concurrency, environment isolation, lease recovery, lease ownership, atomic authorization, idempotency, checksum conflict, 5 MiB rejection, controlled worker lifecycle, one official round trip and a second no-op idempotency cycle. The sanitized evidence and its limits are recorded in `docs/architecture/SRI-WORKER-DEPLOYMENT.md`; it must not be generalized to other tenants, environments or provider actions.

The disabled host lifecycle is validated. Empty enabled polling from Codex is not valid evidence because the process runs as `CodexSandboxOffline` and cannot negotiate the local SQL TLS context. Repeat that validation from a normal Windows PowerShell console with `docs/operations/templates/run-sri-worker-empty-poll-local-proye.example.ps1`; never weaken SQL TLS to make the sandbox pass.
