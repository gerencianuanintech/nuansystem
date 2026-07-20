---
name: nuansystem-mediatr-cqrs
description: Implement or review NuanSystem Application commands, queries, handlers, FluentValidation pipeline behavior, standard Result outcomes, feature folders, cancellation, and MediatR dispatch. Use when changing src/Backend/NuanSystem.Application features, messaging abstractions, handlers, validators, pipeline behaviors, or API-to-Application flow.
---

# NuanSystem MediatR CQRS

## Authority

Follow `$nuansystem-backend-architecture`. Also load `$nuansystem-backend-validation`, `$nuansystem-backend-persistence`, `$nuansystem-backend-testing`, and `$nuansystem-operational-usecase` when their boundaries are affected.

Use the repository contracts, not generic MediatR examples:

- `Application/Abstractions/Messaging/ICommand.cs`
- `Application/Abstractions/Messaging/IQuery.cs`
- `Application/Abstractions/Messaging/ICommandHandler.cs`
- `Application/Abstractions/Messaging/IQueryHandler.cs`
- `Application/Common/Models/Result.cs`
- `Application/Common/Behaviors/ValidationBehavior.cs`
- `Application/Common/Behaviors/LoggingBehavior.cs`

## Command/query tree

```text
Reads without state change?
  -> IQuery<TResponse> + IQueryHandler<TQuery, TResponse>
Changes state or records durable intent?
  -> ICommand<TResponse> + ICommandHandler<TCommand, TResponse>
Changes stock, money, document/workflow, sync, or external state?
  -> command plus $nuansystem-operational-usecase
```

Both messaging contracts already wrap the response as `Result<TResponse>` through MediatR. Do not declare `IRequest<Result<T>>` directly for ordinary features and do not create another result abstraction.

## Handler contract

- Represent one named use case.
- Depend on Application abstractions, Domain policies, clocks, or focused services; never on Minimal API, Dapper, SQL connections, WinForms, or concrete SAP clients.
- Normalize input before authoritative uniqueness checks and persistence.
- Return `Result<T>.Failure(message, ApiError[])` for expected business, conflict, and not-found outcomes.
- Let `ApplicationValidationException` and `GlobalExceptionMiddleware` handle pipeline validation and unexpected exceptions.
- Pass `CancellationToken` through every repository/service call.
- Reload authoritative state before update, delete, transition, calculation, or post.
- Return the authoritative persisted DTO when the established vertical requires it.

## Validation pipeline

FluentValidation owns request shape. `ValidationBehavior` runs validators asynchronously, maps each failure to `ApiError(ErrorCode, ErrorMessage, PropertyName)`, and throws `ApplicationValidationException`.

Do not duplicate this behavior in endpoints or manually invoke validators from normal handlers. Business invariants that require persisted state remain in the handler/domain policy and SQL constraints provide defense in depth.

## Feature organization

Follow the closest same-domain folder convention under `Application/Features/{Owner}`. Separate `Commands`, `Queries`, and `Dtos` when the vertical already does so. File granularity may follow the nearby feature: Carriers groups small records/handlers; SecurityRoles uses one type per file. Preserve consistency instead of imposing a new universal layout.

## Repository-backed references

- Independent CRUD: `Application/Features/Carriers`.
- Rich aggregate: `Application/Features/BusinessPartners`.
- Operational workflow: `Application/Features/Purchasing/PurchaseOrders`.
- Authentication command: `Application/Features/Auth/Commands/ChangePasswordCommandHandler.cs`.

Use the reference for lifecycle/technique only. Do not inherit its domain ownership, table, synchronization, or permissions.

## Antipatterns

- Generic tutorial Customer code copied into the repository.
- Direct `IRequest<Result<T>>` bypassing the project messaging abstractions.
- Query that writes or command named as generic Save for a business transition.
- Handler opening connections, naming procedures, reading HTTP claims, or returning `IResult`.
- New validation/logging/transaction pipeline without inspecting the current registrations and all consumers.
- Exception used for a normal duplicate, not-found, or invalid-state outcome.

## Completion gate

- [ ] Command/query classification and domain owner are explicit.
- [ ] Messaging abstractions and `Result<T>` are reused.
- [ ] Shape and authoritative validation are separated.
- [ ] Cancellation and stable `ApiError` fields/codes propagate.
- [ ] Persistence/transaction/integration boundaries are correct.
- [ ] Targeted handler/validator tests and affected build are reported.
