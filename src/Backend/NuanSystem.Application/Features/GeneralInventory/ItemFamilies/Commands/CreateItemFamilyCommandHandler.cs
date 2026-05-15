using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

public sealed class CreateItemFamilyCommandHandler(IItemFamilyRepository itemFamilyRepository, IItemGroupRepository itemGroupRepository)
    : ICommandHandler<CreateItemFamilyCommand, ItemFamilyDto>
{
    public async Task<Result<ItemFamilyDto>> Handle(CreateItemFamilyCommand request, CancellationToken cancellationToken)
    {
        if (await itemGroupRepository.GetByIdAsync(request.ItemGroupId, cancellationToken) is null)
        {
            return Result<ItemFamilyDto>.Failure(
                "Grupo de articulos no encontrado.",
                [new ApiError("ItemGroupNotFound", "No existe el grupo de articulos indicado.", nameof(request.ItemGroupId))]);
        }

        var code = NormalizeCode(request.Code);
        if (await itemFamilyRepository.ExistsByCodeAsync(request.ItemGroupId, code, cancellationToken))
        {
            return Result<ItemFamilyDto>.Failure(
                "Ya existe una linea/familia con el codigo indicado dentro del grupo.",
                [new ApiError("ItemFamilyCodeAlreadyExists", "El codigo de linea/familia ya existe para el grupo.", nameof(request.Code))]);
        }

        var id = await itemFamilyRepository.CreateAsync(new CreateItemFamilyData(
            request.ItemGroupId,
            code,
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            request.IsActive,
            NormalizeOptional(request.SapFamilyCode),
            NormalizeOptional(request.SapCode),
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName)), cancellationToken);

        var itemFamily = await itemFamilyRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("La linea/familia fue creada pero no pudo consultarse.");

        return Result<ItemFamilyDto>.Success(itemFamily, "Linea/familia creada correctamente.");
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
