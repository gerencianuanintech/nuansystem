using System.Text.Json.Serialization;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSupplierImportResultDto(
    int TotalRead,
    int Created,
    int Updated,
    int Unchanged,
    int Skipped,
    int Failed,
    IReadOnlyCollection<SapSupplierImportItemResultDto> Items);

public sealed record SapSupplierImportItemResultDto(
    string SapCardCode,
    string SapCardName,
    string Status,
    string Message,
    int? LocalBusinessPartnerId)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CorrelationId { get; init; }
}
