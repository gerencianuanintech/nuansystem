using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityDocumentSeriesAccessRepository
{
    Task<IReadOnlySet<int>> GetSelectedSeriesIdsAsync(
        int roleId,
        string companyCode,
        string formKey,
        string? documentType,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityDocumentSeriesOperationAccessDto>> GetOperationsAsync(
        int roleId,
        string companyCode,
        string formKey,
        string documentType,
        int securityDocumentSeriesId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        int roleId,
        string companyCode,
        string formKey,
        string documentType,
        int securityDocumentSeriesId,
        bool isSelected,
        IReadOnlyCollection<SaveSecurityDocumentSeriesOperationAccessData> operations,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default);

    Task<IReadOnlySet<int>> GetAuthorizedSeriesIdsForUserAsync(
        int userId,
        string companyCode,
        string formKey,
        string documentType,
        string actionKey,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateUserOperationAsync(
        int userId,
        string companyCode,
        string formKey,
        string documentType,
        int securityDocumentSeriesId,
        string actionKey,
        CancellationToken cancellationToken = default);
}
