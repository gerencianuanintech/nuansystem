namespace NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;

public sealed class TaxDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Rate { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed record TaxLookupDto(int Id, string Code, string Name, decimal Rate, bool IsActive);

public sealed record TaxAuditChangeDto(
    string RecordId,
    string Action,
    string FieldName,
    string? OldValue,
    string? NewValue,
    int? UserId,
    string? UserName,
    DateTime CreatedAt);

public sealed record CreateTaxData(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    decimal Rate,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record UpdateTaxData(
    int Id,
    string Code,
    string Name,
    string? Description,
    decimal Rate,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);

public sealed record TaxSyncPayloadV1(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    decimal Rate,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
