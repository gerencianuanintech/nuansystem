using NuanSystem.WinForms.Services.SecurityAccess.Models;

namespace NuanSystem.WinForms.ViewModels.SecurityAccess;

public sealed class SecurityFormFieldAccessRow
{
    public SecurityFormFieldAccessRow(SecurityFormFieldAccessItem item)
    {
        FieldId = item.FieldId;
        FormId = item.FormId;
        FormCode = item.FormCode;
        FormName = item.FormName;
        FormKey = item.FormKey;
        FieldCode = item.FieldCode;
        FieldName = item.FieldName;
        FieldKey = item.FieldKey;
        Description = item.Description;
        ControlType = item.ControlType;
        DataType = item.DataType;
        DefaultVisible = item.DefaultVisible;
        DefaultEditable = item.DefaultEditable;
        DefaultRequired = item.DefaultRequired;
        DefaultReadOnly = item.DefaultReadOnly;
        DisplayOrder = item.DisplayOrder;
        IsVisible = item.IsVisible;
        IsEditable = item.IsEditable;
        IsRequired = item.IsRequired;
        IsReadOnly = item.IsReadOnly;
        IsActive = item.IsActive;
        UpdatedByUserName = item.UpdatedByUserName;
        UpdatedAt = item.UpdatedAt;
        CreatedByUserName = item.CreatedByUserName;
        CreatedAt = item.CreatedAt;
    }

    public int FieldId { get; }
    public int FormId { get; }
    public string FormCode { get; }
    public string FormName { get; }
    public string FormKey { get; }
    public string FieldCode { get; }
    public string FieldName { get; }
    public string FieldKey { get; }
    public string? Description { get; }
    public string ControlType { get; }
    public string DataType { get; }
    public bool DefaultVisible { get; }
    public bool DefaultEditable { get; }
    public bool DefaultRequired { get; }
    public bool DefaultReadOnly { get; }
    public int DisplayOrder { get; }
    public bool IsVisible { get; set; }
    public bool IsEditable { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsActive { get; set; }
    public string? UpdatedByUserName { get; }
    public DateTime? UpdatedAt { get; }
    public string? CreatedByUserName { get; }
    public DateTime? CreatedAt { get; }
}
