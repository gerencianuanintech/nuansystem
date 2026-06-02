using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

public sealed class CreateFinancialCatalogCommandHandler(IFinancialCatalogRepository catalogRepository)
    : ICommandHandler<CreateFinancialCatalogCommand, FinancialCatalogDto>
{
    public async Task<Result<FinancialCatalogDto>> Handle(CreateFinancialCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = NormalizeKey(request.CatalogKey);
        var code = NormalizeCode(request.Code);

        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, cancellationToken))
        {
            return Result<FinancialCatalogDto>.Failure(
                "Ya existe un catalogo financiero con el codigo indicado.",
                [new ApiError("FINANCIAL_CATALOG_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var id = await catalogRepository.CreateAsync(
            catalogKey,
            new CreateFinancialCatalogData(
                code,
                request.Name.Trim(),
                NormalizeOptional(request.Description),
                request.IsActive,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        var created = await catalogRepository.GetByIdAsync(catalogKey, id, cancellationToken)
            ?? throw new InvalidOperationException("El catalogo financiero fue creado pero no pudo consultarse.");

        return Result<FinancialCatalogDto>.Success(created, "Catalogo financiero creado correctamente.");
    }

    internal static string NormalizeKey(string catalogKey) => catalogKey.Trim();

    internal static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
