namespace NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;

public sealed class UnitMeasureDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Symbol { get; set; }
    public string MagnitudeCode { get; set; } = UnitMeasureMagnitudeCodes.Quantity;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public sealed class UnitMeasureLookupDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Symbol { get; set; }
    public string MagnitudeCode { get; set; } = UnitMeasureMagnitudeCodes.Quantity;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public sealed class UnitMeasureAuditChangeDto
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

public static class UnitMeasureMagnitudeCodes
{
    public const string Quantity = "Quantity";
    public const string Packaging = "Packaging";
    public const string Mass = "Mass";
    public const string Volume = "Volume";
    public const string Length = "Length";
    public const string Area = "Area";
    public const string Time = "Time";
    public const string Other = "Other";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    { Quantity, Packaging, Mass, Volume, Length, Area, Time, Other };

    public static string Normalize(string value) => All.First(code => code.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
}

public sealed record CreateUnitMeasureData(
    Guid GlobalId, string Code, string Name, string? Description, string? Symbol,
    string MagnitudeCode, int SortOrder, bool IsActive, string? ExternalSystem,
    string? ExternalCode, int? CreatedByUserId, string? CreatedByUserName);

public sealed record UpdateUnitMeasureData(
    int Id, string Code, string Name, string? Description, string? Symbol,
    string MagnitudeCode, int SortOrder, bool IsActive, string? ExternalSystem,
    string? ExternalCode, int? UpdatedByUserId, string? UpdatedByUserName);

public sealed record UnitMeasureSyncPayload(
    Guid GlobalId, string Code, string Name, string? Description, string? Symbol,
    string MagnitudeCode, int SortOrder, bool IsActive, bool IsDeleted, DateTime UpdatedAt);
