using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.Services.Security.Access;

public interface ISecurityDocumentSeriesAccessClient
{
    Task<IReadOnlyCollection<SecurityDocumentSeriesAccessItem>> GetSeriesAsync(
        int roleId,
        string formKey,
        string? search,
        string? documentType,
        bool? isActive,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityDocumentSeriesOperationAccessItem>> GetOperationsAsync(
        int roleId,
        int seriesId,
        string formKey,
        string documentType,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task<bool> SaveAsync(
        int roleId,
        int seriesId,
        string formKey,
        string documentType,
        SaveSecurityDocumentSeriesAccessRequest request,
        CancellationToken cancellationToken = default);
}
