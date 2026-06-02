---
name: nuansystem-sap-business-one
description: Design, implement, or review NuanSystem SAP Business One integration through backend-only Service Layer or DI API clients, per-company SAP configuration, ISapIntegrationService, ISapClientFactory, SAP document mapping, SapSyncLog, retries, error handling, and optional company capabilities. Use when touching SAP sync, SAP credentials, documents sent to SAP, Service Layer, DI API, or integration logs.
---

# NuanSystem SAP Business One

## Core Rules

- SAP integration is optional per company; NuanSystem must work with or without SAP.
- WinForms must never connect directly to SAP Business One.
- All SAP operations must run from the backend API or backend worker process.
- The integration mode is selected from company configuration/capabilities.
- Prefer Service Layer when possible.
- Use DI API only for scenarios that Service Layer cannot support or where the deployment requires it.
- Do not let SAP concepts contaminate Domain. Keep SAP DTOs, mappings, clients, and response parsing isolated.
- Validate local document state before sending to SAP.
- Never repeat successful syncs unless a forced retry is explicitly allowed, audited, and validated.
- Register all SAP errors in `SapSyncLog`.
- Store `RequestJson` and `ResponseJson` when applicable, with secrets redacted.
- Control retries with max retry count, status checks, and idempotency guards.

## Recommended Structure

```text
NuanSystem.SapIntegration
├── Abstractions
├── ServiceLayer
├── DiApi
├── Mapping
├── Sync
└── Options
```

Keep `NuanSystem.Application` dependent only on abstractions:

```csharp
public interface ISapIntegrationService
{
    Task<Result<SapSyncResult>> SendSalesInvoiceAsync(
        int companyId,
        int documentId,
        CancellationToken cancellationToken);
}

public interface ISapClient
{
    Task<SapClientResult> PostAsync(
        string resource,
        object payload,
        CancellationToken cancellationToken);
}

public interface ISapClientFactory
{
    Task<ISapClient> CreateAsync(int companyId, CancellationToken cancellationToken);
}
```

## Configuration

Use per-company SAP settings:

```csharp
public sealed class SapConfiguration
{
    public required int CompanyId { get; init; }
    public bool Enabled { get; init; }
    public required SapIntegrationMode Mode { get; init; }
    public string? ServiceLayerBaseUrl { get; init; }
    public string? CompanyDb { get; init; }
    public string? UserName { get; init; }
    public string? EncryptedPassword { get; init; }
    public int MaxRetryCount { get; init; } = 3;
}

public enum SapIntegrationMode
{
    None = 0,
    ServiceLayer = 1,
    DiApi = 2
}
```

Credentials must be protected and never logged.

## SapSyncLog

SQL Server example:

```sql
CREATE TABLE dbo.SapSyncLog
(
    SapSyncLogId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SapSyncLog PRIMARY KEY,
    CompanyId INT NOT NULL,
    LocalEntityName NVARCHAR(100) NOT NULL,
    LocalEntityId INT NOT NULL,
    Operation NVARCHAR(80) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    RequestJson NVARCHAR(MAX) NULL,
    ResponseJson NVARCHAR(MAX) NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    SapDocEntry INT NULL,
    SapDocNum INT NULL,
    SyncedAt DATETIME2 NULL,
    RetryCount INT NOT NULL CONSTRAINT DF_SapSyncLog_RetryCount DEFAULT 0,
    TraceId NVARCHAR(100) NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_SapSyncLog_CreatedAt DEFAULT SYSUTCDATETIME(),
    CreatedByUserId INT NULL
);

CREATE INDEX IX_SapSyncLog_LocalEntity
ON dbo.SapSyncLog (CompanyId, LocalEntityName, LocalEntityId, Operation);
```

Recommended statuses: `Pending`, `InProgress`, `Succeeded`, `Failed`, `RetryScheduled`, `Skipped`, `Cancelled`.

## SapClientFactory

```csharp
public sealed class SapClientFactory : ISapClientFactory
{
    private readonly ISapConfigurationRepository _configurationRepository;
    private readonly IServiceProvider _services;

    public SapClientFactory(
        ISapConfigurationRepository configurationRepository,
        IServiceProvider services)
    {
        _configurationRepository = configurationRepository;
        _services = services;
    }

    public async Task<ISapClient> CreateAsync(int companyId, CancellationToken cancellationToken)
    {
        var configuration = await _configurationRepository.GetAsync(companyId, cancellationToken);

        if (configuration is null || !configuration.Enabled || configuration.Mode == SapIntegrationMode.None)
            throw new SapIntegrationDisabledException(companyId);

        return configuration.Mode switch
        {
            SapIntegrationMode.ServiceLayer => ActivatorUtilities.CreateInstance<SapServiceLayerClient>(_services, configuration),
            SapIntegrationMode.DiApi => ActivatorUtilities.CreateInstance<SapDiApiClient>(_services, configuration),
            _ => throw new InvalidOperationException("Unsupported SAP integration mode.")
        };
    }
}
```

## Conceptual Document Send

```csharp
public async Task<Result<SapSyncResult>> SendSalesInvoiceAsync(
    int companyId,
    int documentId,
    CancellationToken cancellationToken)
{
    var document = await _documents.GetSalesInvoiceForSapAsync(documentId, cancellationToken);
    if (document is null)
        return Result<SapSyncResult>.Failure("DOCUMENT_NOT_FOUND", "No se encontro el documento.");

    if (document.SapSyncStatus == SapSyncStatus.Succeeded)
        return Result<SapSyncResult>.Failure("SAP_ALREADY_SYNCED", "El documento ya fue sincronizado con SAP.");

    if (document.Status != DocumentStatus.Posted)
        return Result<SapSyncResult>.Failure("DOCUMENT_NOT_POSTED", "Solo se pueden sincronizar documentos contabilizados.");

    var payload = _mapper.MapSalesInvoice(document);
    var logId = await _syncLog.CreatePendingAsync(companyId, documentId, payload, cancellationToken);

    try
    {
        var client = await _clientFactory.CreateAsync(companyId, cancellationToken);
        var response = await client.PostAsync("Invoices", payload, cancellationToken);

        await _syncLog.MarkSucceededAsync(logId, response.DocEntry, response.DocNum, response.RawJson, cancellationToken);
        return Result<SapSyncResult>.Success(new SapSyncResult(response.DocEntry, response.DocNum));
    }
    catch (Exception ex)
    {
        await _syncLog.MarkFailedAsync(logId, ex.Message, cancellationToken);
        _logger.LogError(ex, "SAP sync failed for company {CompanyId}, document {DocumentId}", companyId, documentId);
        return Result<SapSyncResult>.Failure("SAP_SYNC_FAILED", "No se pudo sincronizar el documento con SAP.");
    }
}
```

## Mapping Rules

- Keep SAP field names in `Mapping` or integration DTOs only.
- Map NuanSystem documents to SAP payloads after all local validations pass.
- Redact passwords, session ids, tokens, and license/server details from request/response logs.
- Store SAP identifiers (`SapDocEntry`, `SapDocNum`) back on local documents only after successful confirmation.
