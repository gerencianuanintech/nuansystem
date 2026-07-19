---
name: nuansystem-backend-endpoints
description: Create, modify, or review NuanSystem ASP.NET Core Minimal API endpoints, route groups, request binding, MediatR dispatch, Result-to-HTTP mapping, claims-based audit identity, cancellation, permission policies, and form-operation authorization. Use whenever src/Backend/NuanSystem.Api/Endpoints or an API contract changes.
---

# NuanSystem Backend Endpoints

## Authority and discovery

Follow `$nuansystem-backend-architecture` and inspect a same-domain endpoint group, `Api/Extensions/EndpointAuthorizationExtensions.cs`, the current `ToHttpResult` mapping, `Endpoints/EndpointContextHelper.cs` when applicable, and every frontend consumer of the route.

## Endpoint shape

Keep endpoints transport-thin:

```text
bind route/query/body
  -> obtain trusted audit user when mutating
  -> construct command/query
  -> sender.Send(..., cancellationToken)
  -> result.ToHttpResult()
  -> backend authorization filter/policy
```

Use `MapGroup` and return `IEndpointRouteBuilder` from `Map...Endpoints()`. Register groups through existing API composition.

## Contract rules

- Represent use cases, not WinForms controls or form internals.
- Use route constraints such as `{id:int}` when established.
- Keep request records local only when they are transport-only and not shared.
- Reuse Application DTOs for responses when they are the public contract.
- Propagate `CancellationToken` to `ISender`.
- Use `ClaimsPrincipal.GetAuditUser()` or the current trusted helper for create/update/delete audit identity.
- Never accept audit user, tenant connection, company identity, permission grants, totals, or authoritative status from the body.
- Update every typed client and contract test when a route or payload changes.

## Authorization tree

```text
Stable coarse permission owns the action?
  -> RequirePermission(PermissionCodes.X)
Navigable form operation owns the action?
  -> RequireFormOperation(formKey, actionKey)
Public/authentication bootstrap endpoint?
  -> follow its explicit security contract
No proven policy?
  -> stop and define authorization; do not ship an unprotected endpoint
```

UI visibility is not authorization. Read, create, update, delete, history, retry, post, approve, and cancel operations require matching backend enforcement.

## Result and status handling

- Use the repository's `Result<T>` and `ToHttpResult` behavior.
- The current shared `ToHttpResult()` maps every failed `Result<T>` to HTTP 400. Do not claim 404/409 semantics unless the shared mapper is deliberately evolved with all consumers/tests.
- Return stable business/validation error codes from Application.
- Let global exception handling own unexpected exceptions.
- Do not return raw exceptions, SQL messages, stack traces, or integration credentials.
- Do not add endpoint-local `try/catch` solely to translate every failure.

## Representative references

- CRUD and lookup routes: `Api/Endpoints/GeographyEndpoints.cs`.
- Form-operation authorization: `Api/Endpoints/BusinessPartnerEndpoints.cs`.
- Operational routes: `Api/Endpoints/PurchaseOrderEndpoints.cs`.
- Authorization implementation: `Api/Extensions/EndpointAuthorizationExtensions.cs`.

Copy lifecycle, not domain contracts.

## Antipatterns

- Business decisions or repository calls directly in endpoints.
- Manual tenant lookup when middleware/context already owns it.
- Hard-coded user identity or company code.
- One route silently serving unrelated domain owners.
- Returning `Results.Ok` for a failed `Result<T>`.
- Adding a route without registering or testing it.

## Completion checklist

- [ ] Route, verb, request, response, and consumer contracts align.
- [ ] Authorization matches the real action.
- [ ] Audit identity and company context are trusted.
- [ ] Cancellation and result mapping are preserved.
- [ ] Failure/not-found/conflict/forbidden paths are tested or inspected.
- [ ] Endpoint group is registered exactly once.
