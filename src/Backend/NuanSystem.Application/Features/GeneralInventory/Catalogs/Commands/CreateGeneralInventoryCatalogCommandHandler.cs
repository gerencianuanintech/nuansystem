using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Commands;

public sealed class CreateGeneralInventoryCatalogCommandHandler(
    IGeneralInventoryCatalogRepository catalogRepository)
    : ICommandHandler<CreateGeneralInventoryCatalogCommand, GeneralInventoryCatalogDto>
{
    public async Task<Result<GeneralInventoryCatalogDto>> Handle(
        CreateGeneralInventoryCatalogCommand request,
        CancellationToken cancellationToken)
    {
        var catalogKey = NormalizeKey(request.CatalogKey);
        var code = NormalizeCode(request.Code);

        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, cancellationToken))
        {
            return Result<GeneralInventoryCatalogDto>.Failure(
                "Ya existe un maestro de inventario con el codigo indicado.",
                [new ApiError("GENERAL_INVENTORY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var id = await catalogRepository.CreateAsync(
            catalogKey,
            new CreateGeneralInventoryCatalogData(
                code,
                request.Name.Trim(),
                NormalizeOptional(request.Description),
                request.IsActive,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        var created = await catalogRepository.GetByIdAsync(catalogKey, id, cancellationToken)
            ?? throw new InvalidOperationException("El maestro de inventario fue creado pero no pudo consultarse.");

        return Result<GeneralInventoryCatalogDto>.Success(created, "Maestro de inventario creado correctamente.");
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
