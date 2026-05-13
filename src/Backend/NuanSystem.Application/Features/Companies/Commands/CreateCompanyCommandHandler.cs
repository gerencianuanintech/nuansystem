using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Companies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed class CreateCompanyCommandHandler(
    ICompanyAdminRepository companyRepository,
    ICompanyConnectionTester connectionTester,
    ISecretProtector secretProtector)
    : ICommandHandler<CreateCompanyCommand, CompanyDto>
{
    public async Task<Result<CompanyDto>> Handle(
        CreateCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        if (await companyRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<CompanyDto>.Failure(
                "Ya existe una empresa con el codigo indicado.",
                new[] { new ApiError("CompanyCodeAlreadyExists", "El codigo de empresa ya existe.", nameof(request.Code)) });
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
                return Result<CompanyDto>.Failure(
                    "No se pudo validar la conexion de la empresa.",
                    new[] { new ApiError("CompanyConnectionFailed", test.Message) });
            }
        }

        await companyRepository.CreateAsync(new CreateCompanyData(
            code,
            request.CommercialName.Trim(),
            request.LegalName?.Trim(),
            request.TaxIdentification?.Trim(),
            request.DatabaseEngine,
            request.Server.Trim(),
            request.Port,
            request.DatabaseName.Trim(),
            request.DatabaseUser.Trim(),
            secretProtector.Protect(request.DatabasePassword),
            request.IsActive,
            request.SapIntegrationMode), cancellationToken);

        var company = await companyRepository.GetByCodeAsync(code, cancellationToken)
            ?? throw new InvalidOperationException("La empresa fue creada pero no pudo consultarse.");

        return Result<CompanyDto>.Success(company, "Empresa creada correctamente.");
    }
}
