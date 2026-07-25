---
name: nuansystem-master-branch-sync
description: Design, implement, review, or diagnose NuanSystem Matriz-Sucursal replication across entity definitions, profiles, dependencies, distribution policies, SyncOutbox targets, event appliers, retries, audit, monitoring, and NuanSystem.MasterBranchSyncWorker. Use for MasterToBranch distribution or branch routing; do not use as SAP synchronization.
---

# NuanSystem Master-Branch Sync

## Authority and boundary

Follow Constitution > Kernel > Catalogs/Graph > `$nuansystem-framework-discovery` > this skill > source.

This is internal NuanSystem replication. It does not authenticate to SAP, choose Service Layer/DI API, or replace `$nuansystem-sap-sync-orchestration`.

## Implemented flow

```text
authoritative local change/full execution
  -> entity metadata/profile validation
  -> SyncEventPublisher -> durable SyncOutbox
  -> routing/distribution decisions
  -> one target per selected branch
  -> worker claim with expiring lock
  -> enabled entity applier -> idempotent branch apply
  -> target status + SyncAudit
  -> aggregate status -> retry or DeadLetter/manual action
```

Read `references/implemented-entities.md` before proposing an entity.

## Decision tree

```text
Source is SAP? -> SAP skill first; Matriz-Sucursal may be downstream
Master-owned NuanSystem entity distributed to branches? -> this skill
Catalog entry has producer/applier false? -> not operative; implement missing contract first
Document routed to one branch? -> operational pattern + routing + transactional applier
```

## Design record

```text
Entity/source/GlobalId/version:
Operations and producer mode:
Dependencies/order:
Profile/direction/sync mode:
Routing/OnNoMatch/distribution:
Targets/payload version:
Applier/idempotency/transaction:
Attempts/backoff/lock:
Audit/monitor/manual recovery:
Security/SQL migration:
```

## Rules

- Require stable `GlobalId` and metadata before publication.
- For CRUD events whose business state is persisted in a tenant database, use
  the Iteration 8 boundary: entity mutation plus `LocalOutbox` in one tenant
  transaction, followed by idempotent promotion to Master with the same
  `EventId`. Do not use a direct tenant/Master dual write.
- Publish only from enabled Master context and allowed direction.
- Persist outbox before delivery; route through `ISyncRoutingService` and record policy decisions.
- Deduplicate targets by branch and keep target states independent until aggregate closure.
- Respect catalog dependencies/execution order.
- Apply idempotently at the branch using the established transaction/inbox/audit boundary.
- Distinguish retryable dependency/technical failures, ignored routing, and terminal `DeadLetter`.
- Require permission and reason for retry, dead-letter reset, or expired-lock release.
- Reuse exact permission constants and values: `SyncOutboxView` (`SYNC.OUTBOX.VIEW`), `SyncOutboxRetry` (`SYNC.OUTBOX.RETRY`), `SyncOutboxRetryDeadLetter` (`SYNC.OUTBOX.RETRY_DEADLETTER`), and `SyncOutboxReleaseLock` (`SYNC.OUTBOX.RELEASE_LOCK`).
- `SkeletonMode.ObserveOnly` does not claim; other skeleton modes are dry-run/ignore, not real application.
- An entity is operative only when producer and applier exist and are enabled/configured.
- Update catalog, dependency planner, producer, payload, dispatcher/applier, SQL, profiles/security, tests, and graph together.

## Iteration 8 transactional boundary

Read
`docs/architecture/MASTER-BRANCH-ITERATION-8-TRANSACTIONAL-OUTBOX-BLUEPRINT.md`
and
`docs/operations/MASTER-BRANCH-ITERATION-8-VALIDATION-PLAN.md`
before changing CRUD publication.

The first pilot is `BusinessPartner` only. `Item`, `Warehouse` and other
entities remain independent promotion decisions. The relay belongs to
`NuanSystem.MasterBranchSyncWorker`, stays disabled by default and never reuses
SAP or SRI infrastructure.

## Antipatterns

- Enabling a catalog row with producer/applier false.
- Publishing without `GlobalId`, ownership/profile checks, or durable outbox.
- Applying by display code alone when `GlobalId` is identity.
- Marking aggregate applied while a target remains pending/error/dead-letter.
- Infinite retry, silent lock takeover, automatic dead-letter reset, or SAP calls from this worker.
- Treating demo/pilot seeds as universal production configuration.

## Completion gate

- [ ] Source, identity/version, dependencies, profile, routing, and distribution are explicit.
- [ ] Producer and applier are executable/registered before activation.
- [ ] State transitions, locks, attempts, audit, and recovery are tested.
- [ ] Skeleton/disabled configuration cannot mutate branch data.
- [ ] API permissions and monitor actions match backend behavior.
- [ ] Master/tenant scripts are idempotent and ordered.
- [ ] SAP and Matriz-Sucursal stages remain separate.
