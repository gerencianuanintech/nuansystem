namespace NuanSystem.Application.Features.Carriers.Dtos;

public sealed class CarrierListItemDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IdentificationTypeCode { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CarrierDetailDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IdentificationTypeCode { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CarrierLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class CarrierAuditChangeDto
{
    public string RecordId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed record CreateCarrierData(string Code, string Name, string IdentificationTypeCode, string IdentificationNumber, string? Description, bool IsActive, int? AuditUserId, string? AuditUserName);
public sealed record UpdateCarrierData(int Id, string Code, string Name, string IdentificationTypeCode, string IdentificationNumber, string? Description, bool IsActive, int? AuditUserId, string? AuditUserName);
public sealed record DeleteCarrierData(int Id, int? AuditUserId, string? AuditUserName);
