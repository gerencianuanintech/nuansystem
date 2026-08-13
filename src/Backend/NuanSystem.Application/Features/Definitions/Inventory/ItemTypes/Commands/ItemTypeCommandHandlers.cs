using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Commands;

public sealed class CreateItemTypeCommandHandler(IItemTypeRepository repository)
    : ICommandHandler<CreateItemTypeCommand, ItemTypeDto>
{
    public async Task<Result<ItemTypeDto>> Handle(CreateItemTypeCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        if (await repository.ExistsByCodeAsync(code, cancellationToken: cancellationToken))
        {
            return DuplicateCode(request.Code);
        }

        var result = await repository.CreateAsync(
            new CreateItemTypeData(
                Guid.NewGuid(),
                code,
                request.Name.Trim(),
                NormalizeOptional(request.Description),
                ItemTypeBehaviorCodes.Normalize(request.BehaviorCode),
                request.DefaultIsPurchaseItem,
                request.DefaultIsSalesItem,
                request.DefaultIsInventoryItem,
                request.SortOrder,
                request.IsActive,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (result.DuplicateCode)
        {
            return DuplicateCode(request.Code);
        }

        var id = result.Id
            ?? throw new InvalidOperationException("El tipo de item fue creado sin devolver un identificador valido.");
        var created = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El tipo de item fue creado pero no pudo consultarse.");

        return Result<ItemTypeDto>.Success(created, "Tipo de item creado correctamente.");
    }

    internal static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    internal static Result<ItemTypeDto> DuplicateCode(string code) =>
        Result<ItemTypeDto>.Failure(
            "Ya existe un tipo de item con el codigo indicado.",
            [new ApiError("ITEM_TYPE_DUPLICATED_CODE", $"El codigo '{code.Trim()}' ya existe.", "Code")]);
}

public sealed class UpdateItemTypeCommandHandler(IItemTypeRepository repository)
    : ICommandHandler<UpdateItemTypeCommand, ItemTypeDto>
{
    public async Task<Result<ItemTypeDto>> Handle(UpdateItemTypeCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return NotFound(request.Id);
        }

        var code = CreateItemTypeCommandHandler.NormalizeCode(request.Code);
        var behaviorCode = ItemTypeBehaviorCodes.Normalize(request.BehaviorCode);
        if (existing.IsSystem &&
            (!string.Equals(existing.Code, code, StringComparison.Ordinal) ||
             !string.Equals(existing.BehaviorCode, behaviorCode, StringComparison.Ordinal)))
        {
            return SystemProtected();
        }

        if (await repository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return CreateItemTypeCommandHandler.DuplicateCode(request.Code);
        }

        var result = await repository.UpdateAsync(
            new UpdateItemTypeData(
                request.Id,
                code,
                request.Name.Trim(),
                CreateItemTypeCommandHandler.NormalizeOptional(request.Description),
                behaviorCode,
                request.DefaultIsPurchaseItem,
                request.DefaultIsSalesItem,
                request.DefaultIsInventoryItem,
                request.SortOrder,
                request.IsActive,
                request.AuditUserId,
                CreateItemTypeCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (result.DuplicateCode)
        {
            return CreateItemTypeCommandHandler.DuplicateCode(request.Code);
        }
        if (result.SystemProtected)
        {
            return SystemProtected();
        }
        if (!result.Updated)
        {
            return NotFound(request.Id);
        }

        var updated = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El tipo de item fue actualizado pero no pudo consultarse.");
        return Result<ItemTypeDto>.Success(updated, "Tipo de item actualizado correctamente.");
    }

    internal static Result<ItemTypeDto> NotFound(int id) =>
        Result<ItemTypeDto>.Failure(
            "No se encontro el tipo de item.",
            [new ApiError("ITEM_TYPE_NOT_FOUND", $"El tipo de item {id} no existe o fue eliminado.", "Id")]);

    internal static Result<ItemTypeDto> SystemProtected() =>
        Result<ItemTypeDto>.Failure(
            "No se puede cambiar el codigo ni el comportamiento de un tipo de sistema.",
            [new ApiError("ITEM_TYPE_SYSTEM_PROTECTED", "El tipo de item pertenece al sistema.", "Id")]);
}

public sealed class DeleteItemTypeCommandHandler(IItemTypeRepository repository)
    : ICommandHandler<DeleteItemTypeCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemTypeCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }
        if (existing.IsSystem)
        {
            return SystemProtected();
        }

        var result = await repository.DeleteAsync(
            new DeleteItemTypeData(
                request.Id,
                request.AuditUserId,
                CreateItemTypeCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (result.SystemProtected)
        {
            return SystemProtected();
        }
        if (result.InUse)
        {
            return Result<bool>.Failure(
                "No se puede eliminar el tipo de item porque esta siendo utilizado.",
                [new ApiError("ITEM_TYPE_IN_USE", "Existen articulos asociados al tipo de item.", "Id")]);
        }
        return result.Deleted
            ? Result<bool>.Success(true, "Tipo de item eliminado correctamente.")
            : NotFound();
    }

    private static Result<bool> NotFound() =>
        Result<bool>.Failure(
            "No se encontro el tipo de item.",
            [new ApiError("ITEM_TYPE_NOT_FOUND", "El tipo de item no existe o fue eliminado.", "Id")]);

    private static Result<bool> SystemProtected() =>
        Result<bool>.Failure(
            "No se puede eliminar un tipo de item del sistema.",
            [new ApiError("ITEM_TYPE_SYSTEM_PROTECTED", "El tipo de item pertenece al sistema.", "Id")]);
}
