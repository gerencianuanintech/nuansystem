using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Queries;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class UpdateSapServiceLayerSettingsCommandHandler(
    ICompanyContext companyContext,
    ISapCompanySettingsRepository settingsRepository,
    ISecretProtector secretProtector)
    : ICommandHandler<UpdateSapServiceLayerSettingsCommand, SapServiceLayerSettingsDto>
{
    public async Task<Result<SapServiceLayerSettingsDto>> Handle(
        UpdateSapServiceLayerSettingsCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapServiceLayerSettingsDto>.Failure(
                "No hay empresa activa para configurar SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de configurar SAP.", "X-Company-Code")]);
        }

        var company = companyContext.CurrentCompany!;
        var current = await settingsRepository.GetByCompanyIdAsync(company.CompanyId, cancellationToken);
        if (request.IsEnabled
            && string.IsNullOrEmpty(request.SapPassword)
            && string.IsNullOrWhiteSpace(current?.SapPasswordEncrypted))
        {
            return Result<SapServiceLayerSettingsDto>.Failure(
                "Debe indicar la credencial SAP al activar Service Layer.",
                [new ApiError("SAP_PASSWORD_REQUIRED", "Ingrese la credencial del usuario tecnico SAP.", nameof(request.SapPassword))]);
        }

        var encryptedPassword = string.IsNullOrEmpty(request.SapPassword)
            ? null
            : secretProtector.Protect(request.SapPassword);

        await settingsRepository.UpsertServiceLayerAsync(new UpdateSapServiceLayerSettingsData(
            company.CompanyId,
            request.IsEnabled,
            NormalizeUrl(request.ServiceLayerUrl),
            request.SapCompanyDb.Trim(),
            request.SapUser.Trim(),
            encryptedPassword,
            request.MaxRetryCount,
            request.AuditUserId,
            Clean(request.AuditUserName)), cancellationToken);

        var updated = await settingsRepository.GetByCompanyIdAsync(company.CompanyId, cancellationToken)
            ?? throw new InvalidOperationException("La configuracion SAP fue guardada pero no pudo consultarse.");

        return Result<SapServiceLayerSettingsDto>.Success(
            GetSapServiceLayerSettingsQueryHandler.Map(company.CompanyId, company.CompanyCode, updated),
            "Configuracion SAP Service Layer actualizada correctamente.");
    }

    private static string NormalizeUrl(string value) => value.Trim().TrimEnd('/') + "/";
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
