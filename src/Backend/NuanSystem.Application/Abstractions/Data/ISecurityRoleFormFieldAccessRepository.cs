using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityRoleFormFieldAccessRepository
{
    Task<IReadOnlyCollection<SecurityFormFieldAccessDto>> GetFieldsAsync(
        int roleId,
        int formId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task SaveFieldsAsync(
        int roleId,
        int formId,
        IReadOnlyCollection<SaveSecurityFormFieldAccessData> fields,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityFormFieldAccessDto>> GetDocumentSeriesFieldsAsync(
        int roleId,
        string companyCode,
        int formId,
        string documentType,
        int securityDocumentSeriesId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityFormFieldAccessDto>> GetEffectiveDocumentSeriesFieldsForUserAsync(
        int userId,
        string companyCode,
        string formKey,
        string documentType,
        int securityDocumentSeriesId,
        CancellationToken cancellationToken = default);

    Task SaveDocumentSeriesFieldsAsync(
        int roleId,
        string companyCode,
        int formId,
        string documentType,
        int securityDocumentSeriesId,
        IReadOnlyCollection<SaveSecurityFormFieldAccessData> fields,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default);
}
