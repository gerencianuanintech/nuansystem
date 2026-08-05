using System.Text.Json.Serialization;
using FluentValidation;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Cities.Configuration;

public sealed record GetSapCityQuerySettingsQuery : IQuery<SapCityQuerySettingsDto>;

public sealed record UpdateSapCityQuerySettingsCommand(
    string? CitiesSelectQuery,
    [property: JsonIgnore] int? AuditUserId = null,
    [property: JsonIgnore] string? AuditUserName = null) : ICommand<SapCityQuerySettingsDto>;

public sealed class UpdateSapCityQuerySettingsCommandValidator
    : AbstractValidator<UpdateSapCityQuerySettingsCommand>
{
    public UpdateSapCityQuerySettingsCommandValidator()
    {
        RuleFor(command => command.CitiesSelectQuery)
            .Must(query => string.IsNullOrWhiteSpace(query)
                || SapCitySelectQueryPolicy.TryValidate(query, out _))
            .WithMessage(command => SapCitySelectQueryPolicy.TryValidate(
                command.CitiesSelectQuery, out var error) ? string.Empty : error);
    }
}

public sealed class GetSapCityQuerySettingsQueryHandler(
    ICompanyContext companyContext,
    ISapCompanySettingsRepository settingsRepository)
    : IQueryHandler<GetSapCityQuerySettingsQuery, SapCityQuerySettingsDto>
{
    public async Task<Result<SapCityQuerySettingsDto>> Handle(
        GetSapCityQuerySettingsQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return CompanyRequired();
        }

        var company = companyContext.CurrentCompany!;
        var settings = await settingsRepository.GetByCompanyIdAsync(company.CompanyId, cancellationToken);
        return Result<SapCityQuerySettingsDto>.Success(Map(company.CompanyId, company.CompanyCode, settings));
    }

    internal static SapCityQuerySettingsDto Map(
        int companyId,
        string companyCode,
        SapCompanySettingsDto? settings) =>
        new(
            companyId,
            companyCode,
            settings?.CitiesSelectQuery,
            !string.IsNullOrWhiteSpace(settings?.CitiesSelectQuery),
            settings?.UpdatedAt);

    internal static Result<SapCityQuerySettingsDto> CompanyRequired() =>
        Result<SapCityQuerySettingsDto>.Failure(
            "No hay empresa activa para configurar la consulta SAP de ciudades.",
            [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de configurar SAP.", "X-Company-Code")]);
}

public sealed class UpdateSapCityQuerySettingsCommandHandler(
    ICompanyContext companyContext,
    ISapCompanySettingsRepository settingsRepository)
    : ICommandHandler<UpdateSapCityQuerySettingsCommand, SapCityQuerySettingsDto>
{
    public async Task<Result<SapCityQuerySettingsDto>> Handle(
        UpdateSapCityQuerySettingsCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return GetSapCityQuerySettingsQueryHandler.CompanyRequired();
        }

        var company = companyContext.CurrentCompany!;
        var query = string.IsNullOrWhiteSpace(request.CitiesSelectQuery)
            ? null
            : SapCitySelectQueryPolicy.Normalize(request.CitiesSelectQuery);

        await settingsRepository.UpdateCitiesSelectQueryAsync(new(
            company.CompanyId,
            query,
            request.AuditUserId,
            Clean(request.AuditUserName)), cancellationToken);

        var updated = await settingsRepository.GetByCompanyIdAsync(company.CompanyId, cancellationToken)
            ?? throw new InvalidOperationException("La consulta SAP de ciudades fue guardada pero no pudo consultarse.");

        return Result<SapCityQuerySettingsDto>.Success(
            GetSapCityQuerySettingsQueryHandler.Map(company.CompanyId, company.CompanyCode, updated),
            query is null
                ? "Consulta SAP de ciudades deshabilitada correctamente."
                : "Consulta SAP de ciudades actualizada correctamente.");
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
