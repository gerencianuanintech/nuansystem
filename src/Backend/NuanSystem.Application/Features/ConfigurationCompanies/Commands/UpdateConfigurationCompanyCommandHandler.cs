using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Companies.Dtos;
using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed class UpdateConfigurationCompanyCommandHandler(
    IConfigurationCompanyRepository companyRepository,
    ICompanyConnectionTester connectionTester,
    ISecretProtector secretProtector)
    : ICommandHandler<UpdateConfigurationCompanyCommand, ConfigurationCompanyDto>
{
    public async Task<Result<ConfigurationCompanyDto>> Handle(
        UpdateConfigurationCompanyCommand request,
        CancellationToken cancellationToken)
    {
        if (await companyRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<ConfigurationCompanyDto>.Failure(
                "Compania no encontrada.",
                [new ApiError("ConfigurationCompanyNotFound", "La compania no existe.", nameof(request.Id))]);
        }

        var code = request.Code.Trim().ToUpperInvariant();
        if (await companyRepository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return Result<ConfigurationCompanyDto>.Failure(
                "Ya existe una compania con el codigo indicado.",
                [new ApiError("ConfigurationCompanyCodeAlreadyExists", "El codigo de compania ya existe.", nameof(request.Code))]);
        }

        if (request.ValidateConnection && !string.IsNullOrWhiteSpace(request.DatabasePassword))
        {
            var test = await connectionTester.TestAsync(new CompanyConnectionTestData(
                request.DatabaseEngine,
                request.Server.Trim(),
                request.Port,
                request.DatabaseName.Trim(),
                request.DatabaseUser.Trim(),
                request.DatabasePassword), cancellationToken);

            if (!test.Success)
            {
                return Result<ConfigurationCompanyDto>.Failure(
                    "No se pudo validar la conexion de la compania.",
                    [new ApiError("ConfigurationCompanyConnectionFailed", test.Message)]);
            }
        }

        await companyRepository.UpdateAsync(new UpdateConfigurationCompanyData(
            request.Id,
            code,
            request.CommercialName.Trim(),
            Clean(request.LegalName),
            Clean(request.TaxIdentification),
            Clean(request.Address),
            Clean(request.Phone),
            Clean(request.Email),
            request.LogoImage,
            Clean(request.LogoImageContentType),
            Clean(request.LogoImageFileName),
            request.DatabaseEngine,
            request.Server.Trim(),
            request.Port,
            request.DatabaseName.Trim(),
            request.DatabaseUser.Trim(),
            string.IsNullOrWhiteSpace(request.DatabasePassword) ? null : secretProtector.Protect(request.DatabasePassword),
            request.IsActive,
            request.SapIntegrationMode,
            request.DisplayOrder,
            request.IsDefault,
            request.TimeZoneId.Trim(),
            request.CultureCode.Trim(),
            request.CurrencyCode.Trim().ToUpperInvariant(),
            request.AuditUserId,
            Clean(request.AuditUserName)), cancellationToken);

        var company = await companyRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("La compania fue actualizada pero no pudo consultarse.");

        return Result<ConfigurationCompanyDto>.Success(company, "Compania actualizada correctamente.");
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
