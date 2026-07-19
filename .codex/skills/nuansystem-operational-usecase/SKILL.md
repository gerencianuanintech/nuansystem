---
name: nuansystem-operational-usecase
description: Design, implement, or review NuanSystem operational backend use cases affecting stock, money, prices, purchasing, cash, documents, workflow, synchronization, or external systems. Use for posting, receiving, transferring, adjusting, paying, approving, canceling, reversing, closing, retrying, or any transaction requiring authoritative state, concurrency, idempotency, audit, and recovery.
---

# NuanSystem Operational Use Case

## Authority

Follow `$nuansystem-backend-architecture`, `$nuansystem-backend-validation`, `$nuansystem-backend-persistence`, `$nuansystem-backend-multitenancy-security`, and `$nuansystem-backend-testing`. Use `$nuansystem-business-capabilities`, `$nuansystem-sql-standards`, and integration skills when applicable.

Do not force an operational action through generic CRUD merely because the screen has Save/Delete buttons.

## Required use-case record

```text
Business action and actor:
Inputs/intent:
Domain owner:
Current and target states:
Authoritative reads:
Invariants/calculations:
Stock impact:
Money/cash impact:
Document impact:
Transaction boundary:
Concurrency/version guard:
Idempotency/duplicate submission:
Audit events:
Capabilities/configuration:
External/sync effects:
Failure categories:
Retry/reversal/recovery:
Authorization:
Tests:
```

## Execution model

```text
authorize actor/company/action
  -> load current authoritative state
  -> validate capability and allowed transition
  -> recalculate sensitive values
  -> execute atomic local writes
  -> persist audit and durable external intent
  -> commit
  -> process external intent with idempotency/retry
  -> return authoritative status/result
```

## Domain and Application

- Name commands as business actions: `PostPurchaseOrder`, `ReceiveGoods`, `TransferStock`, `CloseCashShift`, `CancelDocument`.
- Put pure calculations/transitions in Domain policies/entities when they own stable behavior.
- Orchestrate repositories, capability services, transactions, clocks, authorization context, and durable intent in Application.
- Return purpose-built operation results; do not reuse a CRUD detail DTO when the workflow requires status, totals, warnings, or recovery information.

## Authoritative state

- Never trust frontend totals, stock, prices, discounts, taxes, balances, cash amounts, document status, approval status, or sync success.
- Reload current records inside the use-case/transaction boundary.
- Validate allowed source state and recheck it at persistence when races are possible.
- Normalize and validate external identifiers before using them for idempotency.

## Transactions and concurrency

- Use `ITransactionRunner` for multiple local writes that must commit together.
- Pass the connection/transaction through transaction-aware repository methods; avoid independent connections inside the unit of work.
- Use status predicates, row version, locks, unique idempotency keys, or procedure guards appropriate to the risk.
- Prevent double posting from retries, double-clicks, worker restarts, and concurrent users.
- Do not hold SQL transactions open across SAP/HTTP calls.

## External effects

For SAP, BEAS, branch sync, or other external systems:

- name the source of truth;
- persist durable intent/outbox before remote processing when required;
- map stable external identifiers;
- distinguish transient, permanent, and business rejection failures;
- bound retry/backoff and record attempts;
- mark success only after confirmed remote success;
- provide reconciliation/manual recovery for terminal failures;
- make handlers idempotent.

## Cancellation, reversal, and delete

Operational records are not generic-deleted. Define cancel/reverse/void semantics, authorization, compensating stock/money/documents, audit, external correction, and irreversible states explicitly.

## Representative references

- Purchase-order workflow: `Application/Features/Purchasing/PurchaseOrders`.
- Tenant transaction implementation: `Persistence/Transactions/SqlTransactionRunner.cs`.
- Sync/outbox contracts and tests: `Application/Features/Sync`, `Persistence/Repositories/Sync`, and `tests/.../Features/Sync`.

## Antipatterns

- CRUD update used to post/cancel a document.
- Transaction per repository while the use case spans writes.
- Remote call inside an open SQL transaction.
- Success recorded before external confirmation.
- Infinite/unobservable retry.
- Catching a failure and returning fabricated success.
- Cancel implemented as logical delete without business reversal.

## Completion checklist

- [ ] State machine, transaction, concurrency, idempotency, and audit are explicit.
- [ ] Sensitive values are authoritative and recalculated.
- [ ] Duplicate submission and failure recovery are tested.
- [ ] External effects are durable, tenant-aware, and observable.
- [ ] Cancellation/reversal semantics are complete.
- [ ] Authorization and capabilities are backend-enforced.
- [ ] Build/tests and review gates are reported truthfully.
