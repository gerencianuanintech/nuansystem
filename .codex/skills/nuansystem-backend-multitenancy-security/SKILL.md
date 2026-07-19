---
name: nuansystem-backend-multitenancy-security
description: Preserve NuanSystem backend tenant/company isolation, authenticated company context, endpoint authorization, form operations, permission constants, audit identity, master-versus-tenant ownership, and sensitive-data boundaries. Use for any company-scoped endpoint, repository, permission, menu/form operation, audit field, or cross-company change.
---

# NuanSystem Backend Multitenancy and Security

## Non-negotiable boundary

`X-Company-Code` is user input until backend middleware validates the authenticated user's access and establishes the trusted company context. Application/Persistence consume trusted context; they do not trust arbitrary company values from request DTOs.

## Scope classification

Before implementation classify data as:

- **Master/global** — identity, company registry, cross-company security/configuration explicitly owned by Master.
- **Tenant/company** — operational and company-specific catalog data accessed through `ITenantConnectionFactory`.
- **Replicated/synchronized** — explicit source of truth plus distribution contracts; never inferred merely because multiple companies need the data.

Record the reason. Do not place data in Master for convenience.

## Request flow

```text
JWT authentication
  -> CompanyContextMiddleware validates X-Company-Code and user access
  -> trusted ICompanyContext
  -> endpoint authorization
  -> Application use case
  -> ITenantConnectionFactory / approved master repository
```

Inspect `Api/Middleware/CompanyContextMiddleware.cs`, `Application/Abstractions/Tenancy`, `Persistence/Connections/TenantConnectionFactory.cs`, and the same-domain endpoint/repository.

## Authorization rules

- Enforce authorization in backend; frontend permissions are UX only.
- Use `RequirePermission` for established coarse permissions.
- Use `RequireFormOperation` for navigable form actions such as refresh, create, update, delete, consult, history, and specialized operations.
- Keep `FormKey`, action keys, permission constants, Master security scripts, menu registration, frontend navigation, and endpoint checks identical.
- Define specialized operational permissions rather than hiding a generic manage action.
- Test allowed and denied paths; authentication alone is insufficient.

## Audit rules

- Derive user id/name from authenticated claims using the approved helper.
- Never accept `CreatedBy`, `UpdatedBy`, `DeletedBy`, company id/code, or permission grants from WinForms request bodies.
- Persist create/update/delete audit fields consistently and query the correct audit source for history.
- Audit meaningful state-changing operations and permission changes; do not expose secrets or raw technical failures.

## Isolation rules

- Every tenant read/write resolves the active company through trusted infrastructure.
- Do not cache tenant data across company changes without a company-scoped key and invalidation.
- Do not reuse ids from one tenant in another.
- Do not query a tenant table from a Master connection or vice versa.
- Synchronization/distribution requires explicit source-of-truth, identity, idempotency, and conflict rules.

## Representative references

- Company validation middleware: `Api/Middleware/CompanyContextMiddleware.cs`.
- Endpoint form operations: `Api/Extensions/EndpointAuthorizationExtensions.cs`.
- Tenant connection: `Persistence/Connections/TenantConnectionFactory.cs`.
- Security access repository: `Persistence/Repositories/SecurityAccessRepository.cs`.
- Permission constants: `Shared/Constants/PermissionCodes.cs`.

## Antipatterns

- Company code in a command treated as authoritative.
- Only hiding a menu/button without endpoint authorization.
- One permission granting unrelated destructive actions.
- Hard-coded admin bypass outside the approved policy.
- Audit user supplied by frontend.
- Tenant connection string or SAP credentials in logs/responses.
- Adding a feature to sync/outbox without an approved distribution requirement.

## Completion checklist

- [ ] Master/tenant/replicated ownership is explicit.
- [ ] Company context is validated and trusted.
- [ ] Each endpoint action has matching backend authorization.
- [ ] Form/menu/operation/permission keys align end to end.
- [ ] Audit identity and history source are correct.
- [ ] Cross-company allowed/denied tests exist.
- [ ] Secrets and technical details remain protected.
