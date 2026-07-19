---
name: nuansystem-security-auth
description: Implement or review NuanSystem authentication and credential lifecycle: login, JWT creation/validation, permission claims, security stamp revocation, password hashing/change requirements, user state, company access bootstrap, secret protection, HTTPS, and production configuration. Use when changing AuthEndpoints, JwtTokenService, SqlServerAuthService, password flows, authentication middleware, claims, signing keys, or credential storage.
---

# NuanSystem Security Authentication

## Scope

This skill owns authentication and credential lifecycle. Use `$nuansystem-backend-multitenancy-security` for tenant isolation, endpoint permissions, form operations, audit identity, and Master/tenant ownership.

Inspect the real flow:

- `Api/Endpoints/AuthEndpoints.cs`
- `Persistence/Security/SqlServerAuthService.cs`
- `Infrastructure/Authentication/JwtTokenService.cs`
- `Infrastructure/Authentication/Pbkdf2PasswordHasher.cs`
- `Api/Extensions/ServiceCollectionExtensions.cs`
- `Api/Middleware/RequiredPasswordChangeMiddleware.cs`
- `Application/Features/Auth/Commands/ChangePasswordCommandHandler.cs`
- `Persistence/Security/UserSecurityStateService.cs`

## Authentication flow

```text
credentials -> SqlServerAuthService verifies active user/password
  -> roles/permissions/companies loaded from Master
  -> JwtTokenService issues signed access token
  -> JwtBearer validates issuer/audience/signature/lifetime
  -> security stamp/current user state revalidated
  -> RequiredPasswordChangeMiddleware restricts forced-change sessions
```

Do not invent refresh-token behavior as active unless the executable flow proves it. A table or initializer alone is not evidence that issuance, rotation, revocation, and client use are complete.

## Rules

- Hash passwords only through `IPasswordHasher`; never store or log plaintext.
- Load JWT signing material from approved environment/local secret configuration; production validation must reject missing/weak configuration.
- Keep tokens, password hashes, connection credentials, SAP credentials, and encryption keys out of repository files, responses, screenshots, and logs.
- Include only claims used by the implemented authorization/session flow.
- Treat permission claims as a snapshot: changes require a renewed token unless the server-side authorization path rechecks current storage.
- Preserve security-stamp invalidation and inactive/deleted user checks.
- Keep forced password change restricted to the approved endpoint until a new token is issued.
- Enforce HTTPS and production-safe Swagger/configuration behavior.

## Company and authorization boundary

Authentication identifies the user and allowed bootstrap data. `X-Company-Code` remains untrusted until `CompanyContextMiddleware` validates the user-company relationship. Endpoint access still requires backend permission/form-operation enforcement.

Never place connection strings, mutable configuration, or full secrets in JWT claims.

## Change-impact tree

```text
Claims/token lifetime/signing changed?
  -> inspect token service, JWT validation, ApiSession/frontend renewal, security stamp, tests
Password policy/hash changed?
  -> inspect create/update user, change-password flow, existing hash compatibility, tests
Permissions changed?
  -> inspect Master Permissions + RolePermissions, JWT issuance, endpoint policy, renewed-token runtime test
```

## Antipatterns

- Hard-coded signing/encryption key.
- Frontend-decided authorization.
- Permission added only to form operations but absent from `Permissions`/`RolePermissions`.
- Security change tested with a stale token and reported as failed configuration.
- New refresh-token API documented without complete persistence, rotation, revocation, and client support.
- Raw password/token in structured log properties.

## Completion gate

- [ ] Login, token validation, user state, security stamp, and forced-change behavior remain coherent.
- [ ] Secrets and passwords follow approved protection boundaries.
- [ ] Company context and endpoint authorization remain separate and enforced.
- [ ] Permission changes are tested with a newly issued token.
- [ ] Authentication and negative-path tests plus affected build are reported.
