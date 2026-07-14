using NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Security.Access;

public sealed class SecurityDocumentSeriesAccessRow
{
    private bool isSelected;

    public SecurityDocumentSeriesAccessRow(SecurityDocumentSeriesAccessItem item)
    {
        Id = item.Id;
        DocumentType = item.DocumentType;
        Code = item.Code;
        Name = item.Name;
        Prefix = item.Prefix;
        Establishment = item.Establishment;
        EmissionPoint = item.EmissionPoint;
        IsActive = item.IsActive;
        isSelected = item.IsSelected;
        UpdatedByUserName = item.UpdatedByUserName;
        UpdatedAt = item.UpdatedAt;
        CreatedByUserName = item.CreatedByUserName;
        CreatedAt = item.CreatedAt;
    }

    public int Id { get; }
    public string DocumentType { get; }
    public string DocumentTypeName => SecurityDocumentSeriesCatalogs.GetDocumentTypeName(DocumentType);
    public string Code { get; }
    public string Name { get; }
    public string Prefix { get; }
    public string Establishment { get; }
    public string EmissionPoint { get; }
    public bool IsActive { get; }
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected == value)
            {
                return;
            }

            isSelected = value;
            IsSelectionDirty = true;
        }
    }

    public bool IsSelectionDirty { get; private set; }
    public string? UpdatedByUserName { get; }
    public DateTime? UpdatedAt { get; }
    public string? CreatedByUserName { get; }
    public DateTime? CreatedAt { get; }
    public string DisplayName => $"{Code} - {Name}";

    public void AcceptChanges()
    {
        IsSelectionDirty = false;
    }
}
