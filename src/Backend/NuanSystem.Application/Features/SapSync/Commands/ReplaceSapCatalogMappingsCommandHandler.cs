using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class ReplaceSapCatalogMappingsCommandHandler(ICompanyContext companyContext, ISapCatalogMappingRepository repository)
    : ICommandHandler<ReplaceSapCatalogMappingsCommand, IReadOnlyCollection<SapCatalogMappingDto>>
{
    public async Task<Result<IReadOnlyCollection<SapCatalogMappingDto>>> Handle(ReplaceSapCatalogMappingsCommand request, CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
            return Result<IReadOnlyCollection<SapCatalogMappingDto>>.Failure("No hay empresa activa.", [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa.", "X-Company-Code")]);

        var companyId = companyContext.CurrentCompany!.CompanyId;
        var normalized = request.Mappings.Select(row => row with
        {
            MappingType = SapCatalogMappingTypes.All.First(type => type.Equals(row.MappingType.Trim(), StringComparison.OrdinalIgnoreCase)),
            SapCode = row.SapCode.Trim(),
            NuanCode = row.NuanCode.Trim()
        }).ToArray();
        await repository.ReplaceAsync(new(companyId, normalized, request.AuditUserId, Clean(request.AuditUserName)), cancellationToken);
        var rows = await repository.GetByCompanyIdAsync(companyId, cancellationToken);
        return Result<IReadOnlyCollection<SapCatalogMappingDto>>.Success(rows, "Matriz de equivalencias SAP actualizada correctamente.");
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
