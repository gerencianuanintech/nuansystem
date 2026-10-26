namespace NuanSystem.WinForms.Services.Sap.Models;

public sealed record SapServiceLayerSettingsItem(
    int CompanyId,
    string CompanyCode,
    bool IsEnabled,
    string? ServiceLayerUrl,
    string? SapCompanyDb,
    string? SapUser,
    bool HasPassword,
    int MaxRetryCount,
    DateTime? UpdatedAt);

public sealed record SaveSapServiceLayerSettingsRequest(
    bool IsEnabled,
    string ServiceLayerUrl,
    string SapCompanyDb,
    string SapUser,
    string? SapPassword,
    int MaxRetryCount = 3);

public sealed record SapConnectionTestResult(bool IsSuccessful, string Message);
