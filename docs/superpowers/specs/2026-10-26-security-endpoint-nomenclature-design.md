# Security Endpoint Nomenclature Design

## Intent

Consolidate every security endpoint class under `NuanSystem.Api/Endpoints/Seguridad`, align the namespace with that physical organization, and make `/api/security` the only public route prefix for authentication, users, roles, and access administration.

## Goals

- Move the twelve security endpoint classes into `Endpoints/Seguridad`.
- Rename `AuthEndpoints`, `UserEndpoints`, and `RoleEndpoints` to `SecurityAuthEndpoints`, `SecurityUserEndpoints`, and `SecurityRoleEndpoints`, including their `Map...Endpoints()` registration methods.
- Replace `/api/auth/*` with `/api/security/auth/*`.
- Remove the legacy `/api/users` and `/api/users/roles` routes immediately.
- Update every runtime consumer, middleware exception, Swagger reference, test, and maintained document that uses the removed routes.

## Non-goals

- No compatibility aliases, redirects, or deprecation period.
- No changes to authentication rules, DTOs, handlers, permissions, database objects, or business logic.
- No reorganization outside API security endpoints.

## Selected approach

Use one `Seguridad` directory and the namespace `NuanSystem.Api.Endpoints.Seguridad`. Keep all existing security endpoint classes together without adding deeper subdirectories. This matches the repository convention that nested endpoint folders use matching namespaces while avoiding unnecessary hierarchy for twelve files.

## Public contract

Canonical authentication routes become:

- `POST /api/security/auth/login`
- `POST /api/security/auth/change-password`

Canonical user and role routes remain under:

- `/api/security/users`
- `/api/security/users/{id}`
- `/api/security/users/roles`
- `/api/security/roles`
- `/api/security/roles/{id}`

The following routes must no longer be registered:

- `/api/auth/login`
- `/api/auth/change-password`
- `/api/users`
- `/api/users/roles`

## Constraints

- Login remains anonymous and protected by the `auth-login` rate limit.
- Change-password remains authenticated and remains available to a user whose token requires a password change.
- Authentication routes must not require `X-Company-Code`.
- Existing form-operation and permission authorization for all non-authentication security endpoints remains unchanged.
- WinForms must use only the new authentication routes.

## Validation

- Route-contract tests prove the new routes exist and the removed routes do not.
- Middleware tests prove the new authentication routes bypass company resolution and allow the required-password-change flow.
- Authentication-client tests prove WinForms calls the new routes.
- Repository search finds no executable-code references to the removed routes.
- The solution builds and the complete automated test suite passes.
