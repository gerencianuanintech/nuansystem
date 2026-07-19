namespace NuanSystem.WinForms.Services.Carriers.Models;

public sealed class CarrierItem
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IdentificationTypeCode { get; set; } = string.Empty;
    public string IdentificationTypeDisplay => CarrierIdentificationTypes.GetDisplay(IdentificationTypeCode);
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

public sealed class CarrierDetail
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IdentificationTypeCode { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public sealed class CarrierLookupItem
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class CarrierAuditChange
{
    public string Action { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed record SaveCarrierRequest(string Code, string Name, string IdentificationTypeCode, string IdentificationNumber, string? Description, bool IsActive);
public sealed record CarrierIdentificationTypeItem(string Code, string Name)
{
    public string DisplayText => $"{Code} - {Name}";
}

public static class CarrierIdentificationTypes
{
    public static IReadOnlyCollection<CarrierIdentificationTypeItem> All { get; } =
    [
        new("05", "Cédula"),
        new("04", "RUC"),
        new("06", "Pasaporte")
    ];

    public static string GetDisplay(string? code) =>
        All.FirstOrDefault(item => string.Equals(item.Code, code, StringComparison.Ordinal))?.DisplayText ?? code ?? string.Empty;
}
