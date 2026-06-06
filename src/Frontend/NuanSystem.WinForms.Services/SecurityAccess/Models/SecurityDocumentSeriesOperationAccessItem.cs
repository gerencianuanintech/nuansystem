namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed class SecurityDocumentSeriesOperationAccessItem
{
    public int? SecurityRoleDocumentSeriesId { get; set; }
    public int OperationId { get; set; }
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public string? OperationDescription { get; set; }
    public string? ActionKey { get; set; }
    public string? IconLarge { get; set; }
    public string? IconSmall { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsAllowed { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime? CreatedAt { get; set; }
}
