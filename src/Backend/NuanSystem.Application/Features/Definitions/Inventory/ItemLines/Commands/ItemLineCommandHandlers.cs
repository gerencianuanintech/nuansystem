using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Commands;

public sealed class CreateItemLineCommandHandler(
    IItemLineRepository repository, ITransactionRunner transactionRunner,
    IItemLineLocalOutboxWriter localOutboxWriter) : ICommandHandler<CreateItemLineCommand, ItemLineDto>
{
    public async Task<Result<ItemLineDto>> Handle(CreateItemLineCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        var data = new CreateItemLineData(Guid.NewGuid(), code, request.Name.Trim(),
            NormalizeOptional(request.Description), request.SortOrder, request.IsActive,
            request.AuditUserId, NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Failure("ItemLineCodeAlreadyExists", "El codigo de linea de articulos ya existe.", nameof(request.Code));

            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Failure("ItemLineCodeAlreadyExists", "El codigo de linea de articulos ya existe.", nameof(request.Code));
            if (id <= 0) return Result<ItemLineDto>.Failure("No se pudo crear la linea de articulos.");

            var item = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("La linea fue creada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, SyncOperation.Created, connection, transaction, token);
            return Result<ItemLineDto>.Success(item, "Linea de articulos creada correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    internal static Result<ItemLineDto> Failure(string code, string message, string field) =>
        Result<ItemLineDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class UpdateItemLineCommandHandler(
    IItemLineRepository repository, ITransactionRunner transactionRunner,
    IItemLineLocalOutboxWriter localOutboxWriter) : ICommandHandler<UpdateItemLineCommand, ItemLineDto>
{
    public async Task<Result<ItemLineDto>> Handle(UpdateItemLineCommand request, CancellationToken cancellationToken)
    {
        var code = CreateItemLineCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateItemLineData(request.Id, code, request.Name.Trim(),
            CreateItemLineCommandHandler.NormalizeOptional(request.Description), request.SortOrder, request.IsActive,
            request.AuditUserId, CreateItemLineCommandHandler.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Failure("ItemLineNotFound", "No existe la linea de articulos indicada.", nameof(request.Id));
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Failure("ItemLineCodeAlreadyExists", "El codigo de linea de articulos ya existe.", nameof(request.Code));

            var result = await repository.UpdateAsync(data, connection, transaction, token);
            if (result == -1)
                return Failure("ItemLineCodeAlreadyExists", "El codigo de linea de articulos ya existe.", nameof(request.Code));
            if (result <= 0) return Result<ItemLineDto>.Failure("No se pudo actualizar la linea de articulos.");

            var item = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("La linea fue actualizada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                connection, transaction, token);
            return Result<ItemLineDto>.Success(item, "Linea de articulos actualizada correctamente.");
        }, cancellationToken);
    }

    private static Result<ItemLineDto> Failure(string code, string message, string field) =>
        CreateItemLineCommandHandler.Failure(code, message, field);
}

public sealed class DeleteItemLineCommandHandler(
    IItemLineRepository repository, ITransactionRunner transactionRunner,
    IItemLineLocalOutboxWriter localOutboxWriter) : ICommandHandler<DeleteItemLineCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteItemLineCommand request, CancellationToken cancellationToken) =>
        await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null)
                return Failure("ItemLineNotFound", "No existe la linea de articulos indicada.");

            var result = await repository.DeleteAsync(request.Id, request.AuditUserId,
                CreateItemLineCommandHandler.NormalizeOptional(request.AuditUserName), connection, transaction, token);
            if (result == -3)
                return Failure("ItemLineInUse", "La linea de articulos esta asociada a otros registros.");
            if (result <= 0) return Result<bool>.Failure("No se pudo eliminar la linea de articulos.");

            await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Linea de articulos eliminada correctamente.");
        }, cancellationToken);

    private static Result<bool> Failure(string code, string message) =>
        Result<bool>.Failure(message, [new ApiError(code, message, nameof(DeleteItemLineCommand.Id))]);
}
