---
name: nuansystem-deployment-production
description: "Prepare or review NuanSystem production deployment. Use for IIS/Windows Service hosting, HTTPS, environment settings, secrets, logging, backups, health checks, Swagger, SQL least privilege, firewall, migrations, WinForms publishing, ApiBaseUrl, monitoring, and release validation."
---

# NuanSystem Deployment Production

## SRI production readiness authority

Before installing or enabling `NuanSystem.SriWorker`, read:

1. `docs/architecture/SRI-ITERATION-7-PRODUCTION-READINESS-BLUEPRINT.md`;
2. `docs/operations/SRI-WORKER-PRODUCTION-READINESS.md`;
3. `docs/operations/SRI-ITERATION-7-DECISION-REGISTER.md`;
4. `docs/operations/DOTNET-10-RELEASE-ARTIFACTS.md`.

D7-01 through D7-10 are hard gates. A blocked host, gMSA, vault, alerting,
support, restore, retention or canary decision prohibits SCM installation,
worker startup and SRI processing. `.NET 10` and release artifacts alone are
not production approval.

## Production Principles

- Deploy the API as IIS-hosted ASP.NET Core or as a Windows Service when the customer environment requires it.
- Use HTTPS in production and configure certificates before exposing the API.
- Keep `appsettings.Development.json` and `appsettings.Production.json` separate.
- Store secrets in environment variables, Windows certificate store, Azure Key Vault, DPAPI-protected files, or another approved secret provider.
- Do not commit production secrets, connection strings, JWT signing keys, SAP passwords, or customer credentials.
- Protect or disable Swagger in production.
- Use SQL users with least privilege per database role.
- Validate firewall ports for API, SQL Server, and SAP Service Layer/DI API dependencies.
- Maintain repeatable, versioned database scripts and controlled migration procedures.

## Backend Checklist

- Build Release.
- Configure `appsettings.Production.json`.
- Configure environment variables for secrets.
- Configure HTTPS certificate and binding.
- Configure Serilog file path and retention.
- Validate connection to `NuanSystem_Master`.
- Validate tenant company connections.
- Validate JWT issuer, audience, signing key, and expiration.
- Validate `X-Company-Code` behavior.
- Protect or disable Swagger.
- Validate CORS only if a browser client exists.
- Enable health checks for API, master DB, tenant DB, and optional SAP dependencies.
- Monitor HTTP 500 responses and authentication failures.

## Frontend Checklist

- Configure `ApiBaseUrl` for the production API.
- Compile WinForms in Release.
- Validate login.
- Validate company selection.
- Validate dynamic menu and permissions.
- Validate CRUD list/create/update/delete.
- Validate API error messages.
- Validate logout clears token and selected company.
- Package installer or publish output with the correct runtime requirements.

## Database Checklist

- Execute versioned master scripts.
- Execute versioned tenant scripts for every company database.
- Validate `SchemaVersions` or the repository's equivalent version marker.
- Validate backups for `NuanSystem_Master`.
- Validate backups for each company database.
- Validate SQL user permissions.
- Validate indexes for high-traffic list/search procedures.
- Validate maintenance jobs for backups and index/statistics upkeep.

## SAP Checklist

- Validate whether SAP integration is enabled per company.
- Validate Service Layer URL or DI API installation.
- Validate SAP credentials securely.
- Validate company DB mapping.
- Validate `SapSyncLog`.
- Validate retry limits.
- Validate logs for successful and failed sync attempts.
- Validate network connectivity from backend host to SAP.

## Configuration Example

Use safe non-secret production configuration in files:

```json
{
  "Api": {
    "RequireHttps": true
  },
  "Swagger": {
    "Enabled": false
  },
  "Serilog": {
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "C:/NuanSystem/Logs/api-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 60
        }
      }
    ]
  }
}
```

Use environment variables for secrets:

```text
NUANSYSTEM_MASTER_CONNECTIONSTRING=...
NUANSYSTEM_JWT_SIGNINGKEY=...
NUANSYSTEM_SAP_PASSWORD__COMPANY01=...
```

## Health Checks

Expose a protected health endpoint for operators:

```csharp
builder.Services
    .AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("Master")!);

app.MapHealthChecks("/health").RequireAuthorization("OperationsOnly");
```

Do not expose sensitive connection details in health responses.

## Release Control

- Tag production releases.
- Keep a deployment note with API version, WinForms version, database script version, and SAP integration changes.
- Back up master and tenant databases before schema changes.
- Run database scripts in a staging copy before production.
- Define rollback steps for API binaries, WinForms installer, and database changes.
