using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Companies.Dtos;
using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed class CreateConfigurationCompanyCommandHandler(
    IConfigurationCompanyRepository companyRepository,
    ICompanyConnectionTester connectionTester,
    ISecretProtector secretProtector)
    : ICommandHandler<CreateConfigurationCompanyCommand, ConfigurationCompanyDto>
{
    public async Task<Result<ConfigurationCompanyDto>> Handle(
        CreateConfigurationCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await companyRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<ConfigurationCompanyDto>.Failure(
                "Ya existe una compania con el codigo indicado.",
                [new ApiError("ConfigurationCompanyCodeAlreadyExists", "El codigo de compania ya existe.", nameof(request.Code))]);
        }

        var hierarchyError = await ValidateHierarchyAsync(request.IsMaster, request.ParentCompanyId, cancellationToken);
        if (hierarchyError is not null)
        {
            return Result<ConfigurationCompanyDto>.Failure("La jerarquia de la compania no es valida.", [hierarchyError]);
        }

        if (request.ValidateConnection)
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

        var id = await companyRepository.CreateAsync(new CreateConfigurationCompanyData(
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
            secretProtector.Protect(request.DatabasePassword),
            request.IsActive,
            request.SapIntegrationMode,
            request.DisplayOrder,
            request.IsDefault,
            request.TimeZoneId.Trim(),
            request.CultureCode.Trim(),
            request.CurrencyCode.Trim().ToUpperInvariant(),
            request.IsMaster,
            request.IsMaster ? null : request.ParentCompanyId,
            request.IsMaster ? null : Clean(request.BranchCode)?.ToUpperInvariant(),
            request.SyncEnabled,
            request.AuditUserId,
            Clean(request.AuditUserName)), cancellationToken);

        var company = await companyRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("La compania fue creada pero no pudo consultarse.");

        return Result<ConfigurationCompanyDto>.Success(company, "Compania creada correctamente.");
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private async Task<ApiError?> ValidateHierarchyAsync(
        bool isMaster,
        int? parentCompanyId,
        CancellationToken cancellationToken)
    {
        if (isMaster)
        {
            return null;
        }

        var parent = parentCompanyId.HasValue
            ? await companyRepository.GetByIdAsync(parentCompanyId.Value, cancellationToken)
            : null;

        return parent is { IsMaster: true, IsActive: true }
            ? null
            : new ApiError(
                "ConfigurationCompanyInvalidParent",
                "La empresa padre debe existir, estar activa y ser una empresa maestra.",
                nameof(CreateConfigurationCompanyCommand.ParentCompanyId));
    }
}
