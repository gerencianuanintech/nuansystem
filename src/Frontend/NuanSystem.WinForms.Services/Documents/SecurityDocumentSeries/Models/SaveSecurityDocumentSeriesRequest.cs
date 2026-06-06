namespace NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;

public sealed record SaveSecurityDocumentSeriesRequest(
    string DocumentType,
    string Code,
    string Name,
    string? Description,
    string Prefix,
    string Establishment,
    string EmissionPoint,
    int InitialNumber,
    int CurrentNumber,
    int NextNumber,
    int NumberLength,
    string? SapObjectType,
    int? SapSeriesId,
    string? SapSeriesName,
    bool IsDefault,
    bool IsActive,
    bool IsSapIntegrationActive);
