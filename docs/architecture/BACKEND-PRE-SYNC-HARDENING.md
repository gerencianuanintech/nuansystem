# Backend pre-sync hardening

## Decision

NuanSystem will complete the reliability of its standalone ERP core before implementing Master/Branch synchronization or ERP-to-SAP export. Existing import and synchronization components are not extended by this increment, and an unavailable export capability must fail without mutating business state.

## Dependency boundary

```text
Api -> Application -> Domain
Persistence -> Application contracts
Infrastructure/SapIntegration -> Application contracts
```

- Domain remains independent from SQL Server, Dapper, HTTP, SAP and workers.
- API binds transport, trusted identity and authorization only.
- Application owns use cases, workflow decisions and stable failure codes.
- Adapters expose executable capabilities through Application contracts.

## Included increments

1. Guard unavailable ERP-to-SAP operations without creating false pending state.
2. Complete the current manual purchase-order approval workflow atomically.
3. Persist and rotate a lightweight user security stamp.
4. Sanitize and decouple dependency failures, then split only the components touched by these changes.

## Explicit exclusions

- Master/Branch profiles, routing, Outbox, Inbox, relay and workers.
- LocalOutbox migration of additional aggregates.
- ERP-to-SAP exporters and `SapSyncJobRunner.RunOutboxAsync` implementation.
- SAP import expansion, watermarks, locks, retry orchestration and reconciliation.
- Refactoring `ISyncOutboxRepository` or `SyncOutboxRepository`.
- MySQL synchronization support.
- Activation, deployment or execution against production databases.

## Discovery Record

Outcome: Prevent an unavailable purchase-order exporter from recording `Pending` or `SapPending`.

Work type: Operational safety boundary for an external integration.

Domain: Purchasing owns the order workflow. SAP export remains optional and deferred.

Affected layers: Application, tests and documentation. Domain, Persistence, API routes, frontend and database are verified unchanged for this increment.

Risk: Medium. The current command can create a durable-looking state without an executable exporter.

Evidence inspected:

- `Features/Purchasing/PurchaseOrders/Commands/PurchaseOrderCommandHandlers.cs` — current command writes a pending log and changes status.
- `Abstractions/SapSync/ISapSyncProcessorRegistry.cs` — existing executable-capability lookup.
- `Features/SapSync/Services/SapSyncProcessorRegistry.cs` — exporters are registered by entity code; an empty exporter collection is valid.
- `DependencyInjection/ApplicationServiceRegistration.cs` — registry exists and no production `ISapSyncExporter` is registered.
- `Features/Purchasing/PurchaseOrders/PurchaseOrderWorkflowHandlerTests.cs` — established handler test pattern.

Selected pattern: Reuse `ISapSyncProcessorRegistry.FindExporter(entityCode)` as the executable capability boundary already used by the SAP orchestrator.

Permitted reuse boundary: Reuse only capability lookup and stable `Result<T>` errors. Do not invoke or implement export, Outbox, SAP transport or worker behavior.

Alternatives rejected:

- New availability service — duplicates the executable registry without a second consumer requirement.
- Marking `SapPending` for a future worker — presents a capability that cannot currently complete.
- Removing the HTTP route — breaks clients unnecessarily; a stable unavailable result is backward safer.

Confidence: High.

Validation required: Handler red/green regression, composition/capability contract test, targeted purchase tests, architecture tests and solution build.

## Runtime rule

When no exporter exists for `SapSyncEntityCode.PurchaseOrders`, the command returns `FailureKind.Unavailable` with code `SAP_EXPORT_NOT_AVAILABLE`. It must not add a SAP pending log, change the order status or claim that synchronization was scheduled.
