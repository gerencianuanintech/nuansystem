namespace NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;

public sealed class UnitMeasureAuditChange
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
