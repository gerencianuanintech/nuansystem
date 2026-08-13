using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Commands;

public sealed class CreateItemBrandCommandHandler(
    IItemBrandRepository repository,
    ITransactionRunner transactionRunner,
    IItemBrandLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<CreateItemBrandCommand, ItemBrandDto>
{
    public async Task<Result<ItemBrandDto>> Handle(CreateItemBrandCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        var data = new CreateItemBrandData(
            Guid.NewGuid(), code, request.Name.Trim(), NormalizeOptional(request.Description),
            request.SortOrder, request.IsActive, NormalizeOptional(request.ExternalSystem),
            NormalizeOptional(request.ExternalCode), NormalizeOptional(request.SapManufacturerCode),
            NormalizeOptional(request.SapCode), request.AuditUserId, NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Failure("ItemBrandCodeAlreadyExists", "El codigo de marca ya existe.", nameof(request.Code));

            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Failure("ItemBrandCodeAlreadyExists", "El codigo de marca ya existe.", nameof(request.Code));
            if (id <= 0)
                return Result<ItemBrandDto>.Failure("No se pudo crear la marca de articulos.");

            var item = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("La marca fue creada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, SyncOperation.Created, connection, transaction, token);
            return Result<ItemBrandDto>.Success(item, "Marca de articulos creada correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static Result<ItemBrandDto> Failure(string code, string message, string field) =>
        Result<ItemBrandDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class UpdateItemBrandCommandHandler(
    IItemBrandRepository repository,
    ITransactionRunner transactionRunner,
    IItemBrandLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<UpdateItemBrandCommand, ItemBrandDto>
{
    public async Task<Result<ItemBrandDto>> Handle(UpdateItemBrandCommand request, CancellationToken cancellationToken)
    {
        var code = CreateItemBrandCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateItemBrandData(
            request.Id, code, request.Name.Trim(), CreateItemBrandCommandHandler.NormalizeOptional(request.Description),
            request.SortOrder, request.IsActive, CreateItemBrandCommandHandler.NormalizeOptional(request.ExternalSystem),
            CreateItemBrandCommandHandler.NormalizeOptional(request.ExternalCode),
            CreateItemBrandCommandHandler.NormalizeOptional(request.SapManufacturerCode),
            CreateItemBrandCommandHandler.NormalizeOptional(request.SapCode),
            request.AuditUserId, CreateItemBrandCommandHandler.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Failure("ItemBrandNotFound", "No existe la marca de articulos indicada.", nameof(request.Id));
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Failure("ItemBrandCodeAlreadyExists", "El codigo de marca ya existe.", nameof(request.Code));

            var result = await repository.UpdateAsync(data, connection, transaction, token);
            if (result == -1)
                return Failure("ItemBrandCodeAlreadyExists", "El codigo de marca ya existe.", nameof(request.Code));
            if (result <= 0)
                return Result<ItemBrandDto>.Failure("No se pudo actualizar la marca de articulos.");

            var item = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("La marca fue actualizada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                connection, transaction, token);
            return Result<ItemBrandDto>.Success(item, "Marca de articulos actualizada correctamente.");
        }, cancellationToken);
    }

    private static Result<ItemBrandDto> Failure(string code, string message, string field) =>
        Result<ItemBrandDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class DeleteItemBrandCommandHandler(
    IItemBrandRepository repository,
    ITransactionRunner transactionRunner,
    IItemBrandLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteItemBrandCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemBrandCommand request, CancellationToken cancellationToken) =>
        await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null)
                return Result<bool>.Failure("Marca de articulos no encontrada.",
                    [new ApiError("ItemBrandNotFound", "No existe la marca de articulos indicada.", nameof(request.Id))]);

            var result = await repository.DeleteAsync(request.Id, request.AuditUserId,
                CreateItemBrandCommandHandler.NormalizeOptional(request.AuditUserName), connection, transaction, token);
            if (result == -2)
                return Result<bool>.Failure("No se puede eliminar la marca porque esta en uso.",
                    [new ApiError("ItemBrandInUse", "Existen articulos asociados a la marca.", nameof(request.Id))]);
            if (result <= 0)
                return Result<bool>.Failure("No se pudo eliminar la marca de articulos.");

            await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Marca de articulos eliminada correctamente.");
        }, cancellationToken);
}
