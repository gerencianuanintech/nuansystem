using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class CreateItemGroupCommandHandler(IItemGroupRepository itemGroupRepository)
    : ICommandHandler<CreateItemGroupCommand, ItemGroupDto>
{
    public async Task<Result<ItemGroupDto>> Handle(CreateItemGroupCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        if (await itemGroupRepository.ExistsByCodeAsync(code, cancellationToken))
        {
            return Result<ItemGroupDto>.Failure(
                "Ya existe un grupo de artículos con el código indicado.",
                [new ApiError("ItemGroupCodeAlreadyExists", "El código de grupo ya existe.", nameof(request.Code))]);
        }

        var id = await itemGroupRepository.CreateAsync(new CreateItemGroupData(
            code,
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            request.IsActive,
            NormalizeOptional(request.InventoryAccountCode),
            NormalizeOptional(request.CostOfSalesAccountCode),
            NormalizeOptional(request.SalesAccountCode),
            NormalizeOptional(request.PurchaseAccountCode),
            NormalizeOptional(request.SapGroupCode),
            NormalizeOptional(request.SapCode),
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName)), cancellationToken);

        var itemGroup = await itemGroupRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El grupo de artículos fue creado pero no pudo consultarse.");

        return Result<ItemGroupDto>.Success(itemGroup, "Grupo de artículos creado correctamente.");
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
