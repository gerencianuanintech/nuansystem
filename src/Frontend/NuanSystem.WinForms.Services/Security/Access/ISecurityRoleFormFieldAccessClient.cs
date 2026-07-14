using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.Services.Security.Access;

public interface ISecurityRoleFormFieldAccessClient
{
    Task<IReadOnlyCollection<SecurityFormFieldAccessItem>> GetFieldsAsync(
        int roleId,
        int formId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityFormFieldAccessItem>> GetDocumentSeriesFieldsAsync(
        int roleId,
        int formId,
        int seriesId,
        string documentType,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task<bool> SaveAsync(
        int roleId,
        int formId,
        SaveSecurityFormFieldAccessRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> SaveDocumentSeriesAsync(
        int roleId,
        int formId,
        int seriesId,
        string documentType,
        SaveSecurityFormFieldAccessRequest request,
        CancellationToken cancellationToken = default);
}
