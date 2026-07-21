---
name: nuansystem-sri-document-queue
description: Design, implement, or review the tenant-scoped durable queue used to capture SRI electronic-document work in NuanSystem. Use for SRI enqueue APIs, queue tables, idempotency, document status, retries, permissions, capturers, XML references, or monitoring; do not use for SAP synchronization or the worker's remote SRI processing loop.
---

# NuanSystem SRI Document Queue

## Authority and current state

Obey this order: `ENGINEERING-CONSTITUTION.md` > `ENGINEERING-KERNEL.md` > catalogs and knowledge graph > framework discovery and operational-use-case skills > this skill > local implementation.

Read `references/contract-status.md` before proposing changes. For the approved first pilot, also read `../../../docs/architecture/SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md`. Phase 5.2 queue behavior and Phase 5.5 monitor/download are deployed and runtime-validated; provider, worker, XML storage and remote SRI processing remain separate evidence gates. Never infer remote processing from queue or download evidence.

## Boundary

- Master owns tenant capability and protected integration configuration.
- Each tenant database owns its queue, attempts, document metadata, XML reference, and audit trail.
- API/Application own validation and enqueue/query/reprocess use cases.
- Capturers (NuanSystem documents, TXT, SAP AddOn, forms, authorized APIs) stop after durable enqueue.
- `$nuansystem-sri-worker` exclusively owns claims, remote SRI calls, XML processing, retry execution, and terminal transitions.
- SAP synchronization and Matriz-Sucursal replication are separate pipelines. Never reuse `SyncOutbox`, SAP handlers, or either worker as the SRI queue.

## Required discovery record

Before writing code, record:

1. request type and document direction;
2. tenant/company/branch ownership;
3. originating document and stable source reference;
4. existing SRI feature and integration configuration;
5. queue/idempotency alternatives inspected;
6. affected Application, Persistence, API, SQL, security, UI, and worker contracts;
7. unresolved provider, XML storage, retention, and access-key policies;
8. validation evidence that will be produced.

If a required business policy is unresolved, finish a blueprint or contract only. Do not invent a production rule.

## Decision tree

```text
Does the request capture or track SRI work?
  No  -> use the relevant commercial/integration skill.
  Yes -> Is it remote processing, XML parsing, or retry execution?
           Yes -> use $nuansystem-sri-worker.
           No  -> Is durable enqueue/query/reprocess required?
                    Yes -> use this skill.
                    No  -> keep the concern in its existing owner.
```

## Queue rules

1. Persist intent before any remote SRI operation.
2. Scope every row and query to the resolved tenant; preserve branch context when relevant.
3. For the approved pilot, enforce tenant-local uniqueness on `(Environment, AccessKey)` in the database.
4. Store source type and immutable source reference so retries cannot create a second logical job.
5. Treat payload, XML, credentials, authorization messages, and logs according to explicit privacy and retention rules.
6. Define transitions as a state machine; reject illegal transitions atomically.
7. Separate business rejection from transient technical failure.
8. Use optimistic concurrency or an equivalent persisted version for commands exposed to users.
9. Require backend permissions for enqueue, cancel, reprocess, payload/XML view, and download.
10. Return stable conflict/not-found/validation outcomes; never leak provider or SQL exceptions.
11. SQL scripts must be versioned, idempotent, forward-safe, indexed for claim/query paths, and compatible with repository/Dapper contracts.
12. Monitoring UI reads API projections only; it never calls SQL or SRI directly.

## Approved Phase 5.2 state contract

The queue uses `Pending`, `Querying`, `RetryScheduled`, `Authorized`, `NotFound`, `Failed`, `DeadLetter`, and `Cancelled`. Phase 5.2 exposes only `Pending|RetryScheduled -> Cancelled` and `Failed|DeadLetter -> Pending`; worker-owned transitions are reserved for Phase 5.3. Do not add arbitrary status editing.

## Forbidden patterns

- Calling SRI before the enqueue transaction commits.
- Treating a frontend request or in-memory channel as the durable queue.
- Sharing SAP or Matriz-Sucursal outbox tables.
- Trusting tenant, company, status, retry count, authorization, or totals supplied by the client.
- Storing secrets or full sensitive XML in ordinary logs.
- Allowing manual status edits that bypass transition rules.
- Claiming end-to-end success from compilation or unit tests alone.
- Creating a `NuanSystem.SriWorker` placeholder and reporting the queue complete.

## Completion gate

Report each item as `Validated`, `Not validated`, or `Not applicable`:

- ownership and tenant isolation;
- database uniqueness and concurrent enqueue;
- legal/illegal transitions and optimistic concurrency;
- permission aliases and renewed JWT behavior;
- claim indexes and retry scheduling contracts;
- sensitive-data redaction and XML access controls;
- build/tests/scripts actually executed;
- provider and worker end-to-end evidence, if claimed.

Queue completion never proves remote SRI processing. State that boundary in the handoff.
