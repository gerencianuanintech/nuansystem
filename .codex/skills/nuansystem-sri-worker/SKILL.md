---
name: nuansystem-sri-worker
description: Design, implement, or review the dedicated NuanSystem background worker that claims durable SRI document jobs, calls approved SRI services, handles XML, retries, dead letters, leases, and observability. Use for worker lifecycle and provider processing; do not use for capture/enqueue APIs, SAP synchronization, or Matriz-Sucursal replication.
---

# NuanSystem SRI Worker

## Authority and current state

Obey: Constitution > Kernel > catalogs/knowledge graph > framework discovery and operational-use-case skills > this skill > implementation.

Read `references/worker-contracts.md`. For the approved first pilot, also read `../../../docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md`. For service deployment, health or version changes, read `../../../docs/architecture/SRI-ITERATION-6-OPERATIONS-BLUEPRINT.md` and `../../../docs/operations/SRI-WORKER-OPERATIONS.md`. Existing workers remain lifecycle references only and must not be relabeled or extended across their ownership boundaries.

## Exclusive boundary

The SRI worker may claim persisted SRI jobs, resolve protected tenant configuration, call the approved SRI provider, validate/parse/store XML, transition state, schedule retries, and emit technical audit/metrics. It does not create commercial documents, own frontend behavior, synchronize SAP masters, or replicate Matriz-Sucursal data.

Use `$nuansystem-sri-document-queue` whenever the task changes enqueue, query, reprocess commands, queue persistence, or monitoring projections.

## Required discovery record

Before implementation, establish:

1. approved pilot direction and provider/environment;
2. queue schema and legal state transitions;
3. tenant/company/branch configuration resolution;
4. certificate and secret storage/rotation policy;
5. XML storage, privacy, retention, and size limits;
6. idempotency identity and remote correlation contract;
7. lease, timeout, retry, dead-letter, and manual-recovery policies;
8. observability and redaction requirements;
9. lifecycle patterns inspected in existing workers;
10. evidence required for local, non-production, and production validation.

Missing provider or security policy blocks provider implementation, not architecture documentation.

## Approved Phase 5.3 operating contract

1. Use only the official offline authorization lookup operation; generation, signing, submission, cancellation and portal scraping are excluded.
2. Store an authorized XML in the tenant database with authorization metadata, immutable queue identity, SHA-256, content type, byte size and producing attempt.
3. Reject responses or XML larger than 5 MiB. Do not implement automatic or manual deletion until retention is separately approved.
4. Keep the worker disabled by default. Approved defaults are batch `10`, concurrency `2`, lease `120s`, provider timeout `30s` and maximum `5` technical attempts.
5. Use persisted exponential backoff with deterministic jitter. Three no-authorization responses or expiry of the 30-minute functional window finish as `NotFound`.
6. Require strict TLS and exact HTTPS endpoints under `celcer.sri.gob.ec` for Test and `cel.sri.gob.ec` for Production. Never add certificate-validation bypasses.
7. Support both configured environments, but never perform a real SRI call or enable the worker without explicit deployment/validation authority.

## Processing pipeline

```text
read enabled tenant configuration
  -> claim eligible queue row with expiring lease
  -> reload persisted job and validate preconditions
  -> perform one approved provider action
  -> normalize result and store XML/reference atomically where possible
  -> transition queue state and append attempt/audit
  -> release/renew lease
```

The loop must be bounded, cancellation-aware, restart-safe, and safe with multiple worker instances.

## Worker rules

1. Claim work atomically; never select then mark in separate races.
2. Give every lease an owner, acquisition time, expiry, renewal rule, and stale-lock recovery path.
3. Re-read persisted state after claim. Do not trust stale payload or UI state.
4. Make every provider action idempotent or guarded by durable remote correlation.
5. Classify validation/rejection, transient transport, authentication/certificate, throttling, and permanent technical errors separately.
6. Use bounded exponential backoff with jitter and persisted `NextAttemptAt`; never spin.
7. Move exhausted work to a visible dead-letter state; manual reprocess requires permission and audit.
8. Keep remote HTTP/SRI calls outside open SQL transactions.
9. Store XML only through the approved abstraction; persist checksum, content type, size, and immutable reference.
10. Redact credentials, tokens, certificates, XML, and taxpayer data from routine logs.
11. Expose health, heartbeat, throughput, latency, failure class, queue age, lease recovery, and dead-letter metrics.
12. Treat `IgnoreSslErrors`, custom accept-all callbacks, proxy bypasses or equivalent certificate-validation omissions as forbidden.
13. Validate the returned authorization, inner document type, access key, issuer RUC and environment before persisting any XML.
14. Mask access keys in ordinary logs and use generic transport/provider messages; full XML never belongs in logs.
15. Publish `WorkerVersion` from `AssemblyInformationalVersionAttribute` before numeric assembly version, and preserve release metadata so update/rollback evidence can distinguish artifacts.
16. Start API runtime probes from the project/content-root directory that contains the approved local configuration; do not treat stale `bin` output or a temporary harness failure as a product defect.
17. When validating WinForms asynchronously, wait for the real refresh operation to complete and capture client exceptions separately from timeout or rendering failures.
18. Preserve append-only sanitized evidence and always return temporary services, accounts, heartbeats, event sources, processes and runtime directories to the approved baseline.

## Forbidden patterns

- Adding SRI processing to `NuanSystem.SyncWorker` or `NuanSystem.MasterBranchSyncWorker`.
- Polling SAP outbox or `SyncOutbox` for SRI work.
- Holding a SQL transaction during network calls.
- Infinite retry, `Task.Run` fire-and-forget, unbounded parallelism, or in-memory-only state.
- Marking authorized/downloaded before durable evidence is stored.
- Logging full XML or secrets.
- Treating provider mocks, compile success, or a single happy path as end-to-end validation.
- Making the worker enabled by default, calling a non-official host, or accepting an arbitrary provider URL from a request.
- Marking `Authorized` before the XML, checksum and attempt are committed in the same tenant transaction.

## Quality gates

Report `Validated`, `Not validated`, or `Not applicable` for:

- competing-worker claim safety and expired-lease recovery;
- cancellation, restart, timeout, and bounded concurrency;
- idempotent repeated provider response;
- transient retry and permanent/dead-letter behavior;
- certificate/secret resolution and log redaction;
- XML integrity, storage, duplicate prevention, and access control;
- tenant isolation and disabled-capability behavior;
- health/metrics/audit evidence;
- exact worker version through heartbeat, protected API and WinForms after update and rollback;
- Windows Service install/lifecycle/cleanup, singleton rejection and safe Event Log evidence;
- Visual Studio Designer opening for affected WinForms forms;
- real provider round trip in the explicitly approved environment when end-to-end success is claimed.

Never call Iteration 5 operational until queue, worker, provider, persistence, security, and end-to-end gates all have evidence.
