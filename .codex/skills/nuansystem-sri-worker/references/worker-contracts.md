# SRI worker contracts and evidence

## Reusable lifecycle evidence

- `NuanSystem.SyncWorker` demonstrates .NET Worker/Windows Service hosting and SAP-specific scheduled processing.
- `NuanSystem.MasterBranchSyncWorker` demonstrates bounded loops, durable claims/locks, audit, and recovery for internal replication.
- Their hosting and reliability techniques may be studied, but their queues, handlers, ownership, and integration semantics are not reusable SRI contracts.

## Current implementation gap

- No `NuanSystem.SriWorker` project or production SRI client exists.
- No approved provider endpoint/certificate workflow, queue lease procedure, XML store, or SRI attempt history exists.
- The architecture in `docs/architecture/SRI-DOCUMENTS-WORKER.md` and `SRI-ITERATION-5-BLUEPRINT.md` is the design authority until code is introduced.

## Validation hierarchy

1. static contract review;
2. build and unit/integration tests;
3. database concurrency and recovery tests;
4. worker lifecycle tests;
5. authorized non-production SRI round trip;
6. deployment-specific smoke test.

Do not promote evidence from a lower level as proof of a higher one.
