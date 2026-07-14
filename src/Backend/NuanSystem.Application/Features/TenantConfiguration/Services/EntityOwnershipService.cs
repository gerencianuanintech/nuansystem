using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Features.TenantConfiguration.Services;

public sealed class EntityOwnershipService(
    IEntityOwnershipRepository repository,
    ICompanyContext companyContext) : IEntityOwnershipService
{
    public async Task<Result<IReadOnlyCollection<EntityOwnershipConfigurationDto>>> GetActiveCompanyOwnershipAsync(
        CancellationToken cancellationToken = default)
    {
        if (companyContext.CurrentCompany is null)
        {
            return Result<IReadOnlyCollection<EntityOwnershipConfigurationDto>>.Failure("Debe seleccionar una empresa.");
        }

        var ownership = await repository.GetByCompanyIdAsync(
            companyContext.CurrentCompany.CompanyId,
            cancellationToken);

        return Result<IReadOnlyCollection<EntityOwnershipConfigurationDto>>.Success(ownership);
    }

    public async Task<Result<EntityOwnershipConfigurationDto>> GetActiveCompanyOwnershipAsync(
        string entityName,
        CancellationToken cancellationToken = default)
    {
        if (companyContext.CurrentCompany is null)
        {
            return Result<EntityOwnershipConfigurationDto>.Failure("Debe seleccionar una empresa.");
        }

        if (string.IsNullOrWhiteSpace(entityName))
        {
            return Result<EntityOwnershipConfigurationDto>.Failure("Debe indicar la entidad.");
        }

        var ownership = await repository.GetByCompanyIdAndEntityAsync(
            companyContext.CurrentCompany.CompanyId,
            entityName.Trim(),
            cancellationToken);

        return ownership is null
            ? Result<EntityOwnershipConfigurationDto>.Failure("No existe configuracion de ownership para la entidad indicada.")
            : Result<EntityOwnershipConfigurationDto>.Success(ownership);
    }
}

