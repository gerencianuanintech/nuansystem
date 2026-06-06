using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Queries;

public sealed class GetSecurityDocumentSeriesAccessQueryHandler(
    ISecurityDocumentSeriesRepository seriesRepository,
    ISecurityDocumentSeriesAccessRepository accessRepository)
    : IQueryHandler<GetSecurityDocumentSeriesAccessQuery, IReadOnlyCollection<SecurityDocumentSeriesAccessDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityDocumentSeriesAccessDto>>> Handle(
        GetSecurityDocumentSeriesAccessQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RoleId <= 0 || string.IsNullOrWhiteSpace(request.CompanyCode) || string.IsNullOrWhiteSpace(request.FormKey))
        {
            return Result<IReadOnlyCollection<SecurityDocumentSeriesAccessDto>>.Failure("Debe seleccionar rol, empresa y formulario.");
        }

        var series = await seriesRepository.GetAllAsync(
            new SecurityDocumentSeriesFilterData(request.Search, request.DocumentType, request.IsActive),
            cancellationToken);
        var selectedIds = await accessRepository.GetSelectedSeriesIdsAsync(
            request.RoleId,
            request.CompanyCode.Trim(),
            request.FormKey.Trim(),
            request.DocumentType,
            cancellationToken);

        var result = series
            .Select(item => new SecurityDocumentSeriesAccessDto
            {
                Id = item.Id,
                DocumentType = item.DocumentType,
                Code = item.Code,
                Name = item.Name,
                Description = item.Description,
                Prefix = item.Prefix,
                Establishment = item.Establishment,
                EmissionPoint = item.EmissionPoint,
                InitialNumber = item.InitialNumber,
                CurrentNumber = item.CurrentNumber,
                NextNumber = item.NextNumber,
                NumberLength = item.NumberLength,
                NextNumberFormatted = item.NextNumberFormatted,
                SapObjectType = item.SapObjectType,
                SapSeriesId = item.SapSeriesId,
                SapSeriesName = item.SapSeriesName,
                IsDefault = item.IsDefault,
                IsActive = item.IsActive,
                IsSapIntegrationActive = item.IsSapIntegrationActive,
                CreatedByUserId = item.CreatedByUserId,
                CreatedByUserName = item.CreatedByUserName,
                CreatedAt = item.CreatedAt,
                UpdatedByUserId = item.UpdatedByUserId,
                UpdatedByUserName = item.UpdatedByUserName,
                UpdatedAt = item.UpdatedAt,
                IsSelected = selectedIds.Contains(item.Id)
            })
            .ToArray();

        return Result<IReadOnlyCollection<SecurityDocumentSeriesAccessDto>>.Success(result);
    }
}
