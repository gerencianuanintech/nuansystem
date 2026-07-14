using NuanSystem.WinForms.Services.Security.Roles;
using NuanSystem.WinForms.Services.Security.Roles.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Security.Access;

public sealed class SecurityTransactionalFieldAccessViewModel(
    IRoleClient roleClient,
    ISecurityRoleFormAccessClient formAccessClient,
    ISecurityDocumentSeriesAccessClient documentSeriesAccessClient,
    ISecurityRoleFormFieldAccessClient fieldAccessClient)
{
    public IReadOnlyCollection<RoleItem> Roles { get; private set; } = Array.Empty<RoleItem>();
    public IReadOnlyCollection<SecurityFormAccessFormItem> Forms { get; private set; } = Array.Empty<SecurityFormAccessFormItem>();
    public IReadOnlyCollection<SecurityDocumentSeriesAccessRow> Series { get; private set; } = Array.Empty<SecurityDocumentSeriesAccessRow>();
    public IReadOnlyCollection<SecurityFormFieldAccessRow> Fields { get; private set; } = Array.Empty<SecurityFormFieldAccessRow>();
    public IReadOnlyCollection<DocumentTypeFilterItem> DocumentTypes { get; private set; } = Array.Empty<DocumentTypeFilterItem>();

    public RoleItem? SelectedRole { get; private set; }
    public SecurityFormAccessFormItem? SelectedForm { get; private set; }
    public SecurityDocumentSeriesAccessRow? SelectedSeries { get; private set; }
    public string? SelectedDocumentType { get; private set; }

    public async Task LoadAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default)
    {
        Roles = await roleClient.GetAsync(cancellationToken);
        Forms = await formAccessClient.GetFormsAsync(2, onlyActive, null, cancellationToken);
        SelectedRole = Roles.FirstOrDefault(role => role.IsActive) ?? Roles.FirstOrDefault();
        SelectedForm = Forms.FirstOrDefault();
        await LoadSeriesAsync(null, search, onlyActive, cancellationToken);
    }

    public async Task SelectRoleAsync(RoleItem? role, string? documentType, string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        SelectedRole = role;
        await LoadSeriesAsync(documentType, search, onlyActive, cancellationToken);
    }

    public async Task SelectFormAsync(SecurityFormAccessFormItem? form, string? documentType, string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        SelectedForm = form;
        await LoadSeriesAsync(documentType, search, onlyActive, cancellationToken);
    }

    public async Task LoadSeriesAsync(string? documentType, string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        SelectedDocumentType = string.IsNullOrWhiteSpace(documentType) ? null : documentType.Trim();

        if (SelectedRole is null || SelectedForm is null)
        {
            Series = Array.Empty<SecurityDocumentSeriesAccessRow>();
            Fields = Array.Empty<SecurityFormFieldAccessRow>();
            SelectedSeries = null;
            DocumentTypes = Array.Empty<DocumentTypeFilterItem>();
            return;
        }

        var items = await documentSeriesAccessClient.GetSeriesAsync(
            SelectedRole.Id,
            SelectedForm.FormKey,
            search,
            SelectedDocumentType,
            onlyActive,
            cancellationToken);

        Series = items.Select(item => new SecurityDocumentSeriesAccessRow(item)).ToArray();
        DocumentTypes = BuildDocumentTypeFilters(Series);

        if (SelectedSeries is null || Series.All(series => series.Id != SelectedSeries.Id))
        {
            SelectedSeries = Series.FirstOrDefault();
        }
        else
        {
            SelectedSeries = Series.First(series => series.Id == SelectedSeries.Id);
        }

        await LoadFieldsAsync(null, onlyActive, cancellationToken);
    }

    public async Task SelectSeriesAsync(SecurityDocumentSeriesAccessRow? series, string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        SelectedSeries = series;
        await LoadFieldsAsync(search, onlyActive, cancellationToken);
    }

    public async Task LoadFieldsAsync(string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        if (SelectedRole is null || SelectedForm is null || SelectedSeries is null)
        {
            Fields = Array.Empty<SecurityFormFieldAccessRow>();
            return;
        }

        var fields = await fieldAccessClient.GetDocumentSeriesFieldsAsync(
            SelectedRole.Id,
            SelectedForm.Id,
            SelectedSeries.Id,
            SelectedSeries.DocumentType,
            onlyActive,
            search,
            cancellationToken);

        Fields = fields.Select(field => new SecurityFormFieldAccessRow(field)).ToArray();
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedRole is null || SelectedForm is null || SelectedSeries is null)
        {
            return Task.CompletedTask;
        }

        var request = new SaveSecurityFormFieldAccessRequest(
            Fields
                .Select(field => new SaveSecurityFormFieldAccessItemRequest(
                    field.FieldId,
                    field.IsVisible,
                    field.IsEditable,
                    field.IsRequired,
                    field.IsReadOnly,
                    field.IsActive))
                .ToArray());

        return fieldAccessClient.SaveDocumentSeriesAsync(
            SelectedRole.Id,
            SelectedForm.Id,
            SelectedSeries.Id,
            SelectedSeries.DocumentType,
            request,
            cancellationToken);
    }

    private static IReadOnlyCollection<DocumentTypeFilterItem> BuildDocumentTypeFilters(IEnumerable<SecurityDocumentSeriesAccessRow> series)
    {
        return series
            .GroupBy(item => item.DocumentType)
            .OrderBy(group => group.First().DocumentTypeName)
            .Select(group => new DocumentTypeFilterItem(group.Key, group.First().DocumentTypeName))
            .ToArray();
    }
}
