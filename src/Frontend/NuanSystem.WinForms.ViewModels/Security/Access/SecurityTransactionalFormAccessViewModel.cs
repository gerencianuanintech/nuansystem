using NuanSystem.WinForms.Services.Security.Roles;
using NuanSystem.WinForms.Services.Security.Roles.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Security.Access;

public sealed class SecurityTransactionalFormAccessViewModel(
    IRoleClient roleClient,
    ISecurityTransactionalFormAccessClient formAccessClient,
    ISecurityDocumentSeriesAccessClient documentSeriesAccessClient)
{
    public IReadOnlyCollection<RoleItem> Roles { get; private set; } = Array.Empty<RoleItem>();
    public IReadOnlyCollection<SecurityFormAccessFormItem> Forms { get; private set; } = Array.Empty<SecurityFormAccessFormItem>();
    public IReadOnlyCollection<SecurityDocumentSeriesAccessRow> Series { get; private set; } = Array.Empty<SecurityDocumentSeriesAccessRow>();
    public IReadOnlyCollection<SecurityDocumentSeriesOperationAccessRow> Operations { get; private set; } = Array.Empty<SecurityDocumentSeriesOperationAccessRow>();
    public IReadOnlyCollection<DocumentTypeFilterItem> DocumentTypes { get; private set; } = Array.Empty<DocumentTypeFilterItem>();

    public RoleItem? SelectedRole { get; private set; }
    public SecurityFormAccessFormItem? SelectedForm { get; private set; }
    public SecurityDocumentSeriesAccessRow? SelectedSeries { get; private set; }
    public string? SelectedDocumentType { get; private set; }

    public async Task LoadAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default)
    {
        Roles = await roleClient.GetAsync(cancellationToken);
        Forms = await formAccessClient.GetFormsAsync(onlyActive, null, cancellationToken);
        SelectedRole = Roles.FirstOrDefault(role => role.IsActive) ?? Roles.FirstOrDefault();
        SelectedForm = Forms.FirstOrDefault();
        await LoadSeriesAsync(null, search, onlyActive, cancellationToken);
    }

    public async Task LoadFormsAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default)
    {
        Forms = await formAccessClient.GetFormsAsync(onlyActive, search, cancellationToken);
        if (SelectedForm is null || Forms.All(form => form.Id != SelectedForm.Id))
        {
            SelectedForm = Forms.FirstOrDefault();
        }

        await LoadSeriesAsync(SelectedDocumentType, null, onlyActive, cancellationToken);
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
            Operations = Array.Empty<SecurityDocumentSeriesOperationAccessRow>();
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

        await LoadOperationsAsync(null, onlyActive, cancellationToken);
    }

    public async Task SelectSeriesAsync(SecurityDocumentSeriesAccessRow? series, string? operationSearch, bool onlyActive, CancellationToken cancellationToken = default)
    {
        SelectedSeries = series;
        await LoadOperationsAsync(operationSearch, onlyActive, cancellationToken);
    }

    public async Task LoadOperationsAsync(string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        if (SelectedRole is null || SelectedForm is null || SelectedSeries is null)
        {
            Operations = Array.Empty<SecurityDocumentSeriesOperationAccessRow>();
            return;
        }

        var operations = await documentSeriesAccessClient.GetOperationsAsync(
            SelectedRole.Id,
            SelectedSeries.Id,
            SelectedForm.FormKey,
            SelectedSeries.DocumentType,
            onlyActive,
            search,
            cancellationToken);

        Operations = operations.Select(operation => new SecurityDocumentSeriesOperationAccessRow(operation)).ToArray();
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedRole is null || SelectedForm is null || SelectedSeries is null)
        {
            return;
        }

        foreach (var dirtySeries in Series.Where(series => series.IsSelectionDirty && series.Id != SelectedSeries.Id))
        {
            var selectionRequest = new SaveSecurityDocumentSeriesAccessRequest(
                dirtySeries.IsSelected,
                Array.Empty<SaveSecurityDocumentSeriesOperationAccessRequest>());

            await documentSeriesAccessClient.SaveAsync(
                SelectedRole.Id,
                dirtySeries.Id,
                SelectedForm.FormKey,
                dirtySeries.DocumentType,
                selectionRequest,
                cancellationToken);

            dirtySeries.AcceptChanges();
        }

        var selectedSeriesRequest = new SaveSecurityDocumentSeriesAccessRequest(
            SelectedSeries.IsSelected,
            Operations
                .Select(operation => new SaveSecurityDocumentSeriesOperationAccessRequest(
                    operation.OperationId,
                    operation.ActionKey,
                    operation.IsAllowed))
                .ToArray());

        await documentSeriesAccessClient.SaveAsync(
            SelectedRole.Id,
            SelectedSeries.Id,
            SelectedForm.FormKey,
            SelectedSeries.DocumentType,
            selectedSeriesRequest,
            cancellationToken);

        SelectedSeries.AcceptChanges();
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

public sealed record DocumentTypeFilterItem(string Code, string Name);
