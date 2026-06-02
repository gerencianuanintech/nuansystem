---
name: nuansystem-security-auth
description: Define and review NuanSystem security, authentication, authorization, JWT, refresh tokens, roles, permissions, module/company permissions, X-Company-Code validation, password hashing, secret protection, and multi-company access isolation. Use when touching login, users, roles, permissions, claims, company access, security middleware, API authorization, WinForms session security, or production secret handling.
---

# NuanSystem Security Auth

## Core Rules

- Authenticate users with username/password only through the backend API.
- Issue a JWT access token that identifies the user; include only claims needed for authorization and correlation.
- Use refresh tokens only from the backend flow and store them revocably with expiration, device/session metadata, and audit fields.
- Require `Authorization: Bearer {token}` from WinForms for authenticated endpoints.
- Require `X-Company-Code` when an endpoint uses tenant/company data.
- Validate `X-Company-Code` against the master database on every company-scoped request.
- Validate that the authenticated user is assigned to the requested company.
- Block inactive users, inactive companies, inactive roles, and revoked refresh tokens.
- Authorize operations in backend using role/module/company permissions; never trust permissions sent by WinForms.
- Keep business data isolated by company and prevent cross-company access at middleware, Application, and Persistence boundaries.
- Never use `CompanyCode` sent by the frontend without validating it against master company configuration.
- Never store passwords in plain text. Use ASP.NET Core Identity password hashing or a strong PBKDF2/Argon2/bcrypt implementation.
- Encrypt or protect sensitive credentials: tenant connection strings, SAP credentials, refresh tokens if persisted, and integration secrets.
- Do not include passwords, tokens, connection strings, SAP credentials, or raw secret values in logs.
- Do not return sensitive details in authentication, authorization, or validation errors.
- Force HTTPS in production and reject token usage over insecure channels.

## Recommended Tables

- `Users`: identity, username, email, password hash, active/locked state, audit fields.
- `Roles`: role code/name, active state.
- `Permissions`: stable permission code, module, operation, description.
- `UserRoles`: user-role assignment.
- `Companies`: company code, status, database provider, connection reference, SAP settings reference.
- `UserCompanies`: user-company assignment and default company flag.
- `RolePermissions`: permission grants per role.
- `RefreshTokens`: token hash, user, company optional, expires/revoked dates, device metadata, created IP.

## JWT Claims

Recommended claims:

```csharp
public static class NuanClaimTypes
{
    public const string UserId = "sub";
    public const string UserName = "preferred_username";
    public const string DisplayName = "name";
    public const string SessionId = "sid";
    public const string CompanyCodes = "nuan:companies";
    public const string SecurityStamp = "nuan:security_stamp";
}
```

Do not put tenant connection strings, SAP credentials, full permission lists, or mutable company configuration in the JWT. Permissions can change before the token expires; the backend must re-check critical permissions against the current store or a safe server-side cache.

## Permission Constants

Use stable permission codes and reuse them across endpoints, menu registration, and seed data:

```csharp
public static class PermissionCodes
{
    public const string CustomersRead = "Customers.Read";
    public const string CustomersCreate = "Customers.Create";
    public const string CustomersUpdate = "Customers.Update";
    public const string CustomersDelete = "Customers.Delete";
    public const string SapSyncRetry = "Integrations.Sap.SyncRetry";
    public const string SecurityUsersManage = "Security.Users.Manage";
}
```

## Company Access Validation

Create a backend service that resolves the active company from `X-Company-Code`, validates user access, and exposes a trusted company context:

```csharp
public sealed record CompanyAccessContext(
    int CompanyId,
    string CompanyCode,
    string DatabaseName,
    DatabaseProvider DatabaseProvider,
    bool SapEnabled);

public interface ICompanyAccessService
{
    Task<Result<CompanyAccessContext>> ValidateAccessAsync(
        int userId,
        string companyCode,
        CancellationToken cancellationToken);
}
```

Example validation rules:

```csharp
public async Task<Result<CompanyAccessContext>> ValidateAccessAsync(
    int userId,
    string companyCode,
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(companyCode))
        return Result.Failure<CompanyAccessContext>("COMPANY_REQUIRED", "Debe seleccionar una empresa.");

    var company = await _masterRepository.GetCompanyByCodeAsync(companyCode, cancellationToken);
    if (company is null || !company.IsActive)
        return Result.Failure<CompanyAccessContext>("COMPANY_NOT_AVAILABLE", "La empresa no esta disponible.");

    var hasAccess = await _masterRepository.UserHasCompanyAccessAsync(userId, company.Id, cancellationToken);
    if (!hasAccess)
        return Result.Failure<CompanyAccessContext>("COMPANY_ACCESS_DENIED", "No tiene acceso a la empresa seleccionada.");

    return Result.Success(new CompanyAccessContext(
        company.Id,
        company.Code,
        company.DatabaseName,
        company.DatabaseProvider,
        company.SapEnabled));
}
```

## Authorization Helper

Use endpoint filters, policies, or helpers that evaluate backend permissions:

```csharp
public interface IPermissionAuthorizationService
{
    Task<bool> HasPermissionAsync(
        int userId,
        string permissionCode,
        int? companyId,
        CancellationToken cancellationToken);
}

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequireNuanPermission(
        this RouteHandlerBuilder builder,
        string permissionCode)
    {
        return builder.RequireAuthorization(policy =>
            policy.Requirements.Add(new PermissionRequirement(permissionCode)));
    }
}
```

Permission checks for company-scoped endpoints must include the active company id. Global administrative endpoints must explicitly document when no company is required.

## Frontend Responsibilities

- Send `Authorization: Bearer {token}` automatically through `NuanApiClient`.
- Send `X-Company-Code` automatically after company selection.
- Store access tokens only in memory unless a specific secure persistence requirement is approved.
- Clear token, refresh token, selected company, cached permissions, and menu state on logout.
- Hide UI actions based on server-provided permissions for usability only; backend authorization remains mandatory.

## Production Security Checklist

- HTTPS enabled and HTTP redirected or rejected.
- JWT signing keys loaded from environment variables or a secret store.
- Connection strings protected outside repository files.
- SAP credentials stored encrypted or in a secret provider.
- Swagger protected or disabled in production.
- Serilog filters prevent secret leakage.
- Password reset, lockout, and refresh-token revocation events are audited.
