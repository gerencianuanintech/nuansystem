# SRI worker contracts and evidence

## Reusable lifecycle evidence

- `NuanSystem.SyncWorker` demonstrates .NET Worker/Windows Service hosting and SAP-specific scheduled processing.
- `NuanSystem.MasterBranchSyncWorker` demonstrates bounded loops, durable claims/locks, audit, and recovery for internal replication.
- Their hosting and reliability techniques may be studied, but their queues, handlers, ownership, and integration semantics are not reusable SRI contracts.

## Current implementation gap

- No `NuanSystem.SriWorker` project or production SRI client exists.
- No approved provider endpoint/certificate workflow, queue lease procedure, XML store, or SRI attempt history exists.
- The architecture in `docs/architecture/SRI-DOCUMENTS-WORKER.md` and `SRI-ITERATION-5-BLUEPRINT.md` is the design authority until code is introduced.

## Approved first pilot

The worker will eventually query and download previously authorized documents by access key. It will not generate, sign, submit, or cancel electronic documents. The approved state machine and evidence gates are in `docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md`.

The provider/environment, physical XML storage, retention, and operational retry/lease limits remain unresolved and block implementation, not contract documentation.

## Validation hierarchy

1. static contract review;
2. build and unit/integration tests;
3. database concurrency and recovery tests;
4. worker lifecycle tests;
5. authorized non-production SRI round trip;
6. deployment-specific smoke test.

Do not promote evidence from a lower level as proof of a higher one.
