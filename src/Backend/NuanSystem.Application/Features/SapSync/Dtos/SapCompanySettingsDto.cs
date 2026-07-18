using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed class SapCompanySettingsDto
{
    public int Id { get; init; }
    public int CompanyId { get; init; }
    public string CompanyCode { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
    public SapIntegrationMode IntegrationMode { get; init; }
    public string? ServiceLayerUrl { get; init; }
    public string? SapCompanyDb { get; init; }
    public string? SapUser { get; init; }
    // Credentials stay encrypted at this boundary; technical clients decrypt them only when opening a SAP connection.
    public string? SapPasswordEncrypted { get; init; }
    public string? DiApiServer { get; init; }
    public string? LicenseServer { get; init; }
    public string? Language { get; init; }
    public string? HanaServer { get; init; }
    public int? HanaPort { get; init; }
    public string? HanaSchema { get; init; }
    public string? HanaUser { get; init; }
    // HANA access is read-only for imports and previews; writes to SAP must go through Service Layer.
    public string? HanaPasswordEncrypted { get; init; }
    public int MaxRetryCount { get; init; } = 3;
    public DateTime? UpdatedAt { get; init; }
}

public sealed record SapServiceLayerSettingsDto(
    int CompanyId,
    string CompanyCode,
    bool IsEnabled,
    string? ServiceLayerUrl,
    string? SapCompanyDb,
    string? SapUser,
    bool HasPassword,
    int MaxRetryCount,
    DateTime? UpdatedAt);

public sealed record UpdateSapServiceLayerSettingsData(
    int CompanyId,
    bool IsEnabled,
    string ServiceLayerUrl,
    string SapCompanyDb,
    string SapUser,
    string? SapPasswordEncrypted,
    int MaxRetryCount,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
