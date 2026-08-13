using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Commands;

public sealed class CreateItemFamilyCommandHandler(
    IItemFamilyRepository repository,
    IItemGroupRepository itemGroupRepository,
    ITransactionRunner transactionRunner,
    IItemFamilyLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<CreateItemFamilyCommand, ItemFamilyDto>
{
    public async Task<Result<ItemFamilyDto>> Handle(CreateItemFamilyCommand request, CancellationToken cancellationToken)
    {
        var group = await itemGroupRepository.GetByIdAsync(request.ItemGroupId, cancellationToken);
        if (group is null)
            return Failure("ItemGroupNotFound", "No existe el grupo de articulos indicado.", nameof(request.ItemGroupId));
        if (!group.IsActive)
            return Failure("ItemGroupInactive", "El grupo de articulos indicado esta inactivo.", nameof(request.ItemGroupId));

        var code = NormalizeCode(request.Code);
        var data = new CreateItemFamilyData(
            Guid.NewGuid(), request.ItemGroupId, code, request.Name.Trim(),
            NormalizeOptional(request.Description), request.SortOrder, request.IsActive,
            NormalizeOptional(request.ExternalSystem), NormalizeOptional(request.ExternalCode),
            NormalizeOptional(request.SapFamilyCode), NormalizeOptional(request.SapCode),
            request.AuditUserId, NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ExistsByCodeAsync(request.ItemGroupId, code, null, connection, transaction, token))
                return Failure("ItemFamilyCodeAlreadyExists", "El codigo de familia ya existe dentro del grupo.", nameof(request.Code));

            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Failure("ItemFamilyCodeAlreadyExists", "El codigo de familia ya existe dentro del grupo.", nameof(request.Code));
            if (id == -2)
                return Failure("ItemGroupInactive", "El grupo de articulos no existe o esta inactivo.", nameof(request.ItemGroupId));
            if (id <= 0)
                return Result<ItemFamilyDto>.Failure("No se pudo crear la familia de articulos.");

            var item = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("La familia fue creada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, SyncOperation.Created, connection, transaction, token);
            return Result<ItemFamilyDto>.Success(item, "Familia de articulos creada correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static Result<ItemFamilyDto> Failure(string code, string message, string field) =>
        Result<ItemFamilyDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class UpdateItemFamilyCommandHandler(
    IItemFamilyRepository repository,
    IItemGroupRepository itemGroupRepository,
    ITransactionRunner transactionRunner,
    IItemFamilyLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<UpdateItemFamilyCommand, ItemFamilyDto>
{
    public async Task<Result<ItemFamilyDto>> Handle(UpdateItemFamilyCommand request, CancellationToken cancellationToken)
    {
        var group = await itemGroupRepository.GetByIdAsync(request.ItemGroupId, cancellationToken);
        if (group is null)
            return Failure("ItemGroupNotFound", "No existe el grupo de articulos indicado.", nameof(request.ItemGroupId));
        if (!group.IsActive)
            return Failure("ItemGroupInactive", "El grupo de articulos indicado esta inactivo.", nameof(request.ItemGroupId));

        var code = CreateItemFamilyCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateItemFamilyData(
            request.Id, request.ItemGroupId, code, request.Name.Trim(),
            CreateItemFamilyCommandHandler.NormalizeOptional(request.Description), request.SortOrder, request.IsActive,
            CreateItemFamilyCommandHandler.NormalizeOptional(request.ExternalSystem),
            CreateItemFamilyCommandHandler.NormalizeOptional(request.ExternalCode),
            CreateItemFamilyCommandHandler.NormalizeOptional(request.SapFamilyCode),
            CreateItemFamilyCommandHandler.NormalizeOptional(request.SapCode),
            request.AuditUserId, CreateItemFamilyCommandHandler.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Failure("ItemFamilyNotFound", "No existe la familia de articulos indicada.", nameof(request.Id));
            if (await repository.ExistsByCodeAsync(request.ItemGroupId, code, request.Id, connection, transaction, token))
                return Failure("ItemFamilyCodeAlreadyExists", "El codigo de familia ya existe dentro del grupo.", nameof(request.Code));

            var result = await repository.UpdateWithResultAsync(data, connection, transaction, token);
            if (result == -1)
                return Failure("ItemFamilyCodeAlreadyExists", "El codigo de familia ya existe dentro del grupo.", nameof(request.Code));
            if (result == -2)
                return Failure("ItemGroupInactive", "El grupo de articulos no existe o esta inactivo.", nameof(request.ItemGroupId));
            if (result == -3)
                return Failure("ItemFamilyGroupMismatch", "No se puede cambiar el grupo porque existen articulos asociados a la familia.", nameof(request.ItemGroupId));
            if (result <= 0)
                return Result<ItemFamilyDto>.Failure("No se pudo actualizar la familia de articulos.");

            var item = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("La familia fue actualizada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                connection, transaction, token);
            return Result<ItemFamilyDto>.Success(item, "Familia de articulos actualizada correctamente.");
        }, cancellationToken);
    }

    private static Result<ItemFamilyDto> Failure(string code, string message, string field) =>
        Result<ItemFamilyDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class DeleteItemFamilyCommandHandler(
    IItemFamilyRepository repository,
    ITransactionRunner transactionRunner,
    IItemFamilyLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteItemFamilyCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemFamilyCommand request, CancellationToken cancellationToken) =>
        await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null)
                return Result<bool>.Failure("Familia de articulos no encontrada.",
                    [new ApiError("ItemFamilyNotFound", "No existe la familia de articulos indicada.", nameof(request.Id))]);

            var result = await repository.DeleteWithResultAsync(request.Id, request.AuditUserId,
                CreateItemFamilyCommandHandler.NormalizeOptional(request.AuditUserName), connection, transaction, token);
            if (result == -4)
                return Result<bool>.Failure("No se puede eliminar la familia porque esta en uso.",
                    [new ApiError("ItemFamilyInUse", "Existen articulos asociados a la familia.", nameof(request.Id))]);
            if (result <= 0)
                return Result<bool>.Failure("No se pudo eliminar la familia de articulos.");

            await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Familia de articulos eliminada correctamente.");
        }, cancellationToken);
}
