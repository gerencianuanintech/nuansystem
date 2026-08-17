using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Commands;

public sealed class CreateItemSubgroupCommandHandler(
    IItemSubgroupRepository repository,
    IItemFamilyRepository itemFamilyRepository,
    ITransactionRunner transactionRunner,
    IItemSubgroupLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<CreateItemSubgroupCommand, ItemSubgroupDto>
{
    public async Task<Result<ItemSubgroupDto>> Handle(CreateItemSubgroupCommand request, CancellationToken cancellationToken)
    {
        var family = await itemFamilyRepository.GetByIdAsync(request.ItemFamilyId, cancellationToken);
        if (family is null)
            return Failure("ItemFamilyNotFound", "No existe la familia de artículos indicada.", nameof(request.ItemFamilyId));
        if (!family.IsActive)
            return Failure("ItemFamilyInactive", "La familia de artículos indicada está inactiva.", nameof(request.ItemFamilyId));

        var code = NormalizeCode(request.Code);
        var data = new CreateItemSubgroupData(
            Guid.NewGuid(), request.ItemFamilyId, code, request.Name.Trim(), NormalizeOptional(request.Description),
            request.SortOrder, request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ExistsByCodeAsync(request.ItemFamilyId, code, null, connection, transaction, token))
                return Failure("ItemSubgroupCodeAlreadyExists", "El código de subgrupo ya existe dentro de la familia.", nameof(request.Code));

            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Failure("ItemSubgroupCodeAlreadyExists", "El código de subgrupo ya existe dentro de la familia.", nameof(request.Code));
            if (id == -2)
                return Failure("ItemFamilyInactive", "La familia de artículos no existe o está inactiva.", nameof(request.ItemFamilyId));
            if (id <= 0)
                return Result<ItemSubgroupDto>.Failure("No se pudo crear el subgrupo de artículos.");

            var item = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("El subgrupo fue creado pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, SyncOperation.Created, connection, transaction, token);
            return Result<ItemSubgroupDto>.Success(item, "Subgrupo de artículos creado correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static Result<ItemSubgroupDto> Failure(string code, string message, string field) =>
        Result<ItemSubgroupDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class UpdateItemSubgroupCommandHandler(
    IItemSubgroupRepository repository,
    IItemFamilyRepository itemFamilyRepository,
    ITransactionRunner transactionRunner,
    IItemSubgroupLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<UpdateItemSubgroupCommand, ItemSubgroupDto>
{
    public async Task<Result<ItemSubgroupDto>> Handle(UpdateItemSubgroupCommand request, CancellationToken cancellationToken)
    {
        var family = await itemFamilyRepository.GetByIdAsync(request.ItemFamilyId, cancellationToken);
        if (family is null)
            return Failure("ItemFamilyNotFound", "No existe la familia de artículos indicada.", nameof(request.ItemFamilyId));
        if (!family.IsActive)
            return Failure("ItemFamilyInactive", "La familia de artículos indicada está inactiva.", nameof(request.ItemFamilyId));

        var code = CreateItemSubgroupCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateItemSubgroupData(
            request.Id, request.ItemFamilyId, code, request.Name.Trim(),
            CreateItemSubgroupCommandHandler.NormalizeOptional(request.Description), request.SortOrder, request.IsActive,
            request.AuditUserId, CreateItemSubgroupCommandHandler.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Failure("ItemSubgroupNotFound", "No existe el subgrupo de artículos indicado.", nameof(request.Id));
            if (await repository.ExistsByCodeAsync(request.ItemFamilyId, code, request.Id, connection, transaction, token))
                return Failure("ItemSubgroupCodeAlreadyExists", "El código de subgrupo ya existe dentro de la familia.", nameof(request.Code));

            var result = await repository.UpdateWithResultAsync(data, connection, transaction, token);
            if (result == -1)
                return Failure("ItemSubgroupCodeAlreadyExists", "El código de subgrupo ya existe dentro de la familia.", nameof(request.Code));
            if (result == -2)
                return Failure("ItemFamilyInactive", "La familia de artículos no existe o está inactiva.", nameof(request.ItemFamilyId));
            if (result == -3)
                return Failure("ItemSubgroupFamilyMismatch", "No se puede cambiar la familia porque existen artículos asociados al subgrupo.", nameof(request.ItemFamilyId));
            if (result <= 0)
                return Result<ItemSubgroupDto>.Failure("No se pudo actualizar el subgrupo de artículos.");

            var item = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("El subgrupo fue actualizado pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                connection, transaction, token);
            return Result<ItemSubgroupDto>.Success(item, "Subgrupo de artículos actualizado correctamente.");
        }, cancellationToken);
    }

    private static Result<ItemSubgroupDto> Failure(string code, string message, string field) =>
        Result<ItemSubgroupDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class DeleteItemSubgroupCommandHandler(
    IItemSubgroupRepository repository,
    ITransactionRunner transactionRunner,
    IItemSubgroupLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteItemSubgroupCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemSubgroupCommand request, CancellationToken cancellationToken) =>
        await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null)
                return Result<bool>.Failure("Subgrupo de artículos no encontrado.",
                    [new ApiError("ItemSubgroupNotFound", "No existe el subgrupo de artículos indicado.", nameof(request.Id))]);

            var result = await repository.DeleteWithResultAsync(request.Id, request.AuditUserId,
                CreateItemSubgroupCommandHandler.NormalizeOptional(request.AuditUserName), connection, transaction, token);
            if (result == -4)
                return Result<bool>.Failure("No se puede eliminar el subgrupo porque está en uso.",
                    [new ApiError("ItemSubgroupInUse", "Existen artículos asociados al subgrupo.", nameof(request.Id))]);
            if (result <= 0)
                return Result<bool>.Failure("No se pudo eliminar el subgrupo de artículos.");

            await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Subgrupo de artículos eliminado correctamente.");
        }, cancellationToken);
}
