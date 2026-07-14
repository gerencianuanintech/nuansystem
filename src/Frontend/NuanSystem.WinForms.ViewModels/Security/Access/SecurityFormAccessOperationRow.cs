using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Security.Access;

public sealed class SecurityFormAccessOperationRow
{
    public SecurityFormAccessOperationRow(SecurityFormAccessOperationItem source)
    {
        FormId = source.FormId;
        FormCode = source.FormCode;
        FormName = source.FormName;
        FormKey = source.FormKey;
        OperationId = source.OperationId;
        OperationCode = source.OperationCode;
        OperationName = source.OperationName;
        OperationDescription = source.OperationDescription;
        ActionKey = source.ActionKey;
        RibbonPageName = source.RibbonPageName;
        RibbonGroupName = source.RibbonGroupName;
        IconLarge = source.IconLarge;
        IconSmall = source.IconSmall;
        DisplayOrder = source.DisplayOrder;
        IsAllowed = source.IsAllowed;
        UpdatedByUserId = source.UpdatedByUserId;
        UpdatedByUserName = source.UpdatedByUserName;
        UpdatedAt = source.UpdatedAt;
        CreatedByUserId = source.CreatedByUserId;
        CreatedByUserName = source.CreatedByUserName;
        CreatedAt = source.CreatedAt;
    }

    public int FormId { get; }
    public string FormCode { get; }
    public string FormName { get; }
    public string FormKey { get; }
    public int OperationId { get; }
    public string OperationCode { get; }
    public string OperationName { get; }
    public string? OperationDescription { get; }
    public string? ActionKey { get; }
    public string? RibbonPageName { get; }
    public string? RibbonGroupName { get; }
    public string? IconLarge { get; }
    public string? IconSmall { get; }
    public int DisplayOrder { get; }
    public bool IsAllowed { get; set; }
    public int? UpdatedByUserId { get; }
    public string? UpdatedByUserName { get; }
    public DateTime? UpdatedAt { get; }
    public int? CreatedByUserId { get; }
    public string? CreatedByUserName { get; }
    public DateTime? CreatedAt { get; }
}
