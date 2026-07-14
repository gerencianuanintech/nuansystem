using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Security.Access;

public sealed class SecurityDocumentSeriesOperationAccessRow
{
    public SecurityDocumentSeriesOperationAccessRow(SecurityDocumentSeriesOperationAccessItem item)
    {
        SecurityRoleDocumentSeriesId = item.SecurityRoleDocumentSeriesId;
        OperationId = item.OperationId;
        OperationCode = item.OperationCode;
        OperationName = item.OperationName;
        ActionKey = item.ActionKey ?? item.OperationCode;
        IsAllowed = item.IsAllowed;
        DisplayOrder = item.DisplayOrder;
        UpdatedByUserName = item.UpdatedByUserName;
        UpdatedAt = item.UpdatedAt;
        CreatedByUserName = item.CreatedByUserName;
        CreatedAt = item.CreatedAt;
    }

    public int? SecurityRoleDocumentSeriesId { get; }
    public int OperationId { get; }
    public string OperationCode { get; }
    public string OperationName { get; }
    public string ActionKey { get; }
    public int DisplayOrder { get; }
    public bool IsAllowed { get; set; }
    public string? UpdatedByUserName { get; }
    public DateTime? UpdatedAt { get; }
    public string? CreatedByUserName { get; }
    public DateTime? CreatedAt { get; }
}
