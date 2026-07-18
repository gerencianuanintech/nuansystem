using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed class GetSapServiceLayerSettingsQueryHandler(
    ICompanyContext companyContext,
    ISapCompanySettingsRepository settingsRepository)
    : IQueryHandler<GetSapServiceLayerSettingsQuery, SapServiceLayerSettingsDto>
{
    public async Task<Result<SapServiceLayerSettingsDto>> Handle(
        GetSapServiceLayerSettingsQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapServiceLayerSettingsDto>.Failure(
                "No hay empresa activa para consultar la configuracion SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de configurar SAP.", "X-Company-Code")]);
        }

        var company = companyContext.CurrentCompany!;
        var settings = await settingsRepository.GetByCompanyIdAsync(company.CompanyId, cancellationToken);

        return Result<SapServiceLayerSettingsDto>.Success(Map(company.CompanyId, company.CompanyCode, settings));
    }

    internal static SapServiceLayerSettingsDto Map(
        int companyId,
        string companyCode,
        SapCompanySettingsDto? settings)
        => new(
            companyId,
            companyCode,
            settings?.IsEnabled ?? false,
            settings?.ServiceLayerUrl,
            settings?.SapCompanyDb,
            settings?.SapUser,
            !string.IsNullOrWhiteSpace(settings?.SapPasswordEncrypted),
            settings?.MaxRetryCount ?? 3,
            settings?.UpdatedAt);
}
