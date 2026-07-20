---
name: nuansystem-sap-sync-orchestration
description: Design, implement, review, or diagnose NuanSystem SAP synchronization across entity handlers, locks, logs, watermarks, inbox/outbox records, retries, heartbeat, and NuanSystem.SyncWorker. Use for scheduled SAP imports/exports, worker reliability, retry policy, or SAP sync monitoring.
---

# NuanSystem SAP Sync Orchestration

## Authority and boundary

Follow Constitution > Kernel > Catalogs/Graph > `$nuansystem-framework-discovery` > `$nuansystem-sap-business-one` > this skill > source.

This governs SAP synchronization. `Application/Features/Sync`, `SyncOutbox`, profiles, branch targets, and `MasterBranchSyncWorker` belong to `$nuansystem-master-branch-sync`.

## Implemented pipeline

```text
active SAP companies
  -> resolve tenant context
  -> enabled entity settings ordered by ExecutionOrder
  -> acquire company/entity/direction lock
  -> resolve ISapSyncEntityHandler by EntityCode
  -> import/export handler
  -> persistence/watermark as implemented
  -> SapSyncLog
  -> release lock
  -> heartbeat/idle
```

Entity constants include `Suppliers`, `Items`, and `PurchaseOrders`; warehouse import also exists through commands/services. Verify current registration before adding an entity.

## Worker truth

- `SapSyncWorker`: scheduled SAP-to-ERP discovery/orchestration.
- `SapRetryWorker`: due inbox failures evaluated by `ISapSyncRetryPolicy`.
- `SapOutboxWorker`: ERP-to-SAP surface; current code reports not implemented.
- `SapSyncJobRunner.RunOutboxAsync`: current code returns `NotImplemented`.

Never present ERP-to-SAP outbox delivery as complete until executable code and tests replace those markers.

The generic SAP inbox retry path is also incomplete. A new SAP-to-ERP Full import may initially use an idempotent, safely rerunnable command plus logs/reconciliation; rebuilding generic payload replay and manual SAP recovery is a separate hardening scope unless the approved use case explicitly requires durable per-record retry.

## Design record

```text
Entity/direction/trigger:
Company resolution:
Handler/reader/import service:
Lock key/timeout:
Idempotency/external version:
Watermark rule:
Batch/order:
Retryable vs terminal errors:
Maximum attempts/backoff:
Heartbeat/log/manual recovery:
Cancellation/shutdown:
Current gaps:
```

## Rules

- Resolve/set tenant context before tenant repository work.
- Acquire and always release the sync lock; overlap skips safely.
- Add entity code, handler, DI registration, settings, SQL, and tests together.
- Preserve dependency and source order.
- Advance watermark only after durable local success.
- Retry only classified transient errors with bounded exponential backoff and max attempts.
- Expose terminal failure for reconciliation/manual recovery.
- Do not silently expand a new entity import into a rewrite of generic retry infrastructure. Record the limitation and choose rerunnable Full reconciliation or an explicitly approved durable-retry scope.
- Emit safe structured logs and heartbeat; caught failure cannot appear successful.
- Honor cancellation in loops, calls, delays, and shutdown.
- Do not claim actual parallelism from option names; verify execution code.

## Antipatterns

- Remote SAP calls inside tenant SQL transactions.
- Retrying every exception or advancing watermark before persistence.
- Treating `Both` as proof of a complete two-way pipeline.
- Using Matriz-Sucursal `SyncOutbox` as SAP outbox.
- Logging credentials, cookies, or sensitive full payloads.

## Completion gate

- [ ] Entity, direction, tenant, lock, handler, idempotency, and watermark are explicit.
- [ ] Retry/terminal behavior, logs, heartbeat, recovery, cancellation, and shutdown are testable.
- [ ] SQL `049`/`050` and entity scripts were inspected where applicable.
- [ ] Incomplete export/outbox paths are not represented as complete.
- [ ] Worker/handler tests and build evidence are truthful.
