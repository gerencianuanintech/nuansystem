using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;

public sealed class CreateGeneralSupplierCatalogCommandHandler(
    IGeneralSupplierCatalogRepository catalogRepository)
    : ICommandHandler<CreateGeneralSupplierCatalogCommand, GeneralSupplierCatalogDto>
{
    public async Task<Result<GeneralSupplierCatalogDto>> Handle(
        CreateGeneralSupplierCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalogKey = NormalizeKey(request.CatalogKey);
        var code = NormalizeCode(request.Code);

        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, cancellationToken))
        {
            return Result<GeneralSupplierCatalogDto>.Failure(
                "Ya existe un catalogo de proveedor con el codigo indicado.",
                [new ApiError("GENERAL_SUPPLIER_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var id = await catalogRepository.CreateAsync(
            catalogKey,
            new CreateGeneralSupplierCatalogData(
                code,
                request.Name.Trim(),
                NormalizeOptional(request.Description),
                request.IsActive,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        var created = await catalogRepository.GetByIdAsync(catalogKey, id, cancellationToken)
            ?? throw new InvalidOperationException("El catalogo de proveedor fue creado pero no pudo consultarse.");

        return Result<GeneralSupplierCatalogDto>.Success(created, "Catalogo de proveedor creado correctamente.");
    }

    internal static string NormalizeKey(string catalogKey)
    {
        return catalogKey.Trim();
    }

    internal static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }

    internal static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

