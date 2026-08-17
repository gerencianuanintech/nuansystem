using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Commands;

public sealed class CreateItemOriginCommandHandler(
    IItemOriginRepository repository, ITransactionRunner transactionRunner,
    IItemOriginLocalOutboxWriter localOutboxWriter) : ICommandHandler<CreateItemOriginCommand, ItemOriginDto>
{
    public Task<Result<ItemOriginDto>> Handle(CreateItemOriginCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        var data = new CreateItemOriginData(Guid.NewGuid(), code, request.Name.Trim(),
            NormalizeOptional(request.Description), request.SortOrder, request.IsActive,
            request.AuditUserId, NormalizeOptional(request.AuditUserName));

        return transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Failure("ItemOriginCodeAlreadyExists", "El código de origen de artículo ya existe.", nameof(request.Code));

            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1) return Failure("ItemOriginCodeAlreadyExists", "El código de origen de artículo ya existe.", nameof(request.Code));
            if (id <= 0) return Result<ItemOriginDto>.Failure("No se pudo crear el origen de artículo.");

            var item = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("El origen fue creado pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, SyncOperation.Created, connection, transaction, token);
            return Result<ItemOriginDto>.Success(item, "Origen de artículo creado correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string value) => value.Trim();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    internal static Result<ItemOriginDto> Failure(string code, string message, string field) =>
        Result<ItemOriginDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class UpdateItemOriginCommandHandler(
    IItemOriginRepository repository, ITransactionRunner transactionRunner,
    IItemOriginLocalOutboxWriter localOutboxWriter) : ICommandHandler<UpdateItemOriginCommand, ItemOriginDto>
{
    public Task<Result<ItemOriginDto>> Handle(UpdateItemOriginCommand request, CancellationToken cancellationToken)
    {
        var code = CreateItemOriginCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateItemOriginData(request.Id, code, request.Name.Trim(),
            CreateItemOriginCommandHandler.NormalizeOptional(request.Description), request.SortOrder, request.IsActive,
            request.AuditUserId, CreateItemOriginCommandHandler.NormalizeOptional(request.AuditUserName));

        return transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null) return Failure("ItemOriginNotFound", "No existe el origen de artículo indicado.", nameof(request.Id));
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Failure("ItemOriginCodeAlreadyExists", "El código de origen de artículo ya existe.", nameof(request.Code));

            var affected = await repository.UpdateAsync(data, connection, transaction, token);
            if (affected == -1) return Failure("ItemOriginCodeAlreadyExists", "El código de origen de artículo ya existe.", nameof(request.Code));
            if (affected <= 0) return Result<ItemOriginDto>.Failure("No se pudo actualizar el origen de artículo.");

            var item = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("El origen fue actualizado pero no pudo consultarse.");
            var operation = item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled;
            await localOutboxWriter.EnqueueAsync(item, operation, connection, transaction, token);
            return Result<ItemOriginDto>.Success(item, "Origen de artículo actualizado correctamente.");
        }, cancellationToken);
    }

    private static Result<ItemOriginDto> Failure(string code, string message, string field) =>
        CreateItemOriginCommandHandler.Failure(code, message, field);
}

public sealed class DeleteItemOriginCommandHandler(
    IItemOriginRepository repository, ITransactionRunner transactionRunner,
    IItemOriginLocalOutboxWriter localOutboxWriter) : ICommandHandler<DeleteItemOriginCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteItemOriginCommand request, CancellationToken cancellationToken) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null) return Failure("ItemOriginNotFound", "No existe el origen de artículo indicado.");

            var affected = await repository.DeleteAsync(request.Id, request.AuditUserId,
                CreateItemOriginCommandHandler.NormalizeOptional(request.AuditUserName), connection, transaction, token);
            if (affected == -3) return Failure("ItemOriginInUse", "El origen de artículo está asociado a otros registros.");
            if (affected <= 0) return Result<bool>.Failure("No se pudo eliminar el origen de artículo.");

            await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Origen de artículo eliminado correctamente.");
        }, cancellationToken);

    private static Result<bool> Failure(string code, string message) =>
        Result<bool>.Failure(message, [new ApiError(code, message, nameof(DeleteItemOriginCommand.Id))]);
}
