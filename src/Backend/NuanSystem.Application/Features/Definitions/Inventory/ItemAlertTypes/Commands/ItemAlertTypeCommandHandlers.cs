using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Commands;

public sealed class CreateItemAlertTypeCommandHandler(IItemAlertTypeRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<CreateItemAlertTypeCommand, ItemAlertTypeDto>
{
    public Task<Result<ItemAlertTypeDto>> Handle(CreateItemAlertTypeCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var code = request.Code.Trim();
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Result<ItemAlertTypeDto>.Failure("El código ya existe.", [new ApiError("ItemAlertTypeCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var data = new CreateItemAlertTypeData(Guid.NewGuid(), code, request.Name.Trim(), NormalizeOptional(request.Description), request.SortOrder, request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName));
            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Result<ItemAlertTypeDto>.Failure("El código ya existe.", [new ApiError("ItemAlertTypeCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var item = id > 0 ? await repository.GetByIdAsync(id, connection, transaction, token) : null;
            return item is null ? Result<ItemAlertTypeDto>.Failure("No se pudo crear Tipos de alerta de artículos.") : Result<ItemAlertTypeDto>.Success(item);
        }, ct);

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class UpdateItemAlertTypeCommandHandler(IItemAlertTypeRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<UpdateItemAlertTypeCommand, ItemAlertTypeDto>
{
    public Task<Result<ItemAlertTypeDto>> Handle(UpdateItemAlertTypeCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Result<ItemAlertTypeDto>.Failure("Registro no encontrado.", [new ApiError("ItemAlertTypeNotFound", "Registro no encontrado.", nameof(request.Id))]);
            var code = request.Code.Trim();
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Result<ItemAlertTypeDto>.Failure("El código ya existe.", [new ApiError("ItemAlertTypeCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var data = new UpdateItemAlertTypeData(request.Id, code, request.Name.Trim(), NormalizeOptional(request.Description), request.SortOrder, request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName));
            var changed = await repository.UpdateAsync(data, connection, transaction, token);
            if (changed == -1)
                return Result<ItemAlertTypeDto>.Failure("El código ya existe.", [new ApiError("ItemAlertTypeCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var item = changed > 0 ? await repository.GetByIdAsync(request.Id, connection, transaction, token) : null;
            return item is null ? Result<ItemAlertTypeDto>.Failure("No se pudo actualizar Tipos de alerta de artículos.") : Result<ItemAlertTypeDto>.Success(item);
        }, ct);

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class DeleteItemAlertTypeCommandHandler(IItemAlertTypeRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<DeleteItemAlertTypeCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteItemAlertTypeCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Result<bool>.Failure("Registro no encontrado.", [new ApiError("ItemAlertTypeNotFound", "Registro no encontrado.", nameof(request.Id))]);
            var changed = await repository.DeleteAsync(request.Id, request.AuditUserId, string.IsNullOrWhiteSpace(request.AuditUserName) ? null : request.AuditUserName.Trim(), connection, transaction, token);
            return changed > 0 ? Result<bool>.Success(true) : Result<bool>.Failure("No se pudo eliminar Tipos de alerta de artículos.", [new ApiError("ItemAlertTypeDeleteFailed", "No se pudo eliminar Tipos de alerta de artículos.", nameof(request.Id))]);
        }, ct);
}

