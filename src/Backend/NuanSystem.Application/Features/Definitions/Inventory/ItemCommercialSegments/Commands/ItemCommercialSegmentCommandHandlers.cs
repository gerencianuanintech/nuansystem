using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Commands;

public sealed class CreateItemCommercialSegmentCommandHandler(IItemCommercialSegmentRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<CreateItemCommercialSegmentCommand, ItemCommercialSegmentDto>
{
    public Task<Result<ItemCommercialSegmentDto>> Handle(CreateItemCommercialSegmentCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var code = request.Code.Trim();
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Result<ItemCommercialSegmentDto>.Failure("El código ya existe.", [new ApiError("ItemCommercialSegmentCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var data = new CreateItemCommercialSegmentData(Guid.NewGuid(), code, request.Name.Trim(), NormalizeOptional(request.Description), request.SortOrder, request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName));
            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Result<ItemCommercialSegmentDto>.Failure("El código ya existe.", [new ApiError("ItemCommercialSegmentCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var item = id > 0 ? await repository.GetByIdAsync(id, connection, transaction, token) : null;
            return item is null ? Result<ItemCommercialSegmentDto>.Failure("No se pudo crear Segmentos comerciales de artículos.") : Result<ItemCommercialSegmentDto>.Success(item);
        }, ct);

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class UpdateItemCommercialSegmentCommandHandler(IItemCommercialSegmentRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<UpdateItemCommercialSegmentCommand, ItemCommercialSegmentDto>
{
    public Task<Result<ItemCommercialSegmentDto>> Handle(UpdateItemCommercialSegmentCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Result<ItemCommercialSegmentDto>.Failure("Registro no encontrado.", [new ApiError("ItemCommercialSegmentNotFound", "Registro no encontrado.", nameof(request.Id))]);
            var code = request.Code.Trim();
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Result<ItemCommercialSegmentDto>.Failure("El código ya existe.", [new ApiError("ItemCommercialSegmentCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var data = new UpdateItemCommercialSegmentData(request.Id, code, request.Name.Trim(), NormalizeOptional(request.Description), request.SortOrder, request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName));
            var changed = await repository.UpdateAsync(data, connection, transaction, token);
            if (changed == -1)
                return Result<ItemCommercialSegmentDto>.Failure("El código ya existe.", [new ApiError("ItemCommercialSegmentCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var item = changed > 0 ? await repository.GetByIdAsync(request.Id, connection, transaction, token) : null;
            return item is null ? Result<ItemCommercialSegmentDto>.Failure("No se pudo actualizar Segmentos comerciales de artículos.") : Result<ItemCommercialSegmentDto>.Success(item);
        }, ct);

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class DeleteItemCommercialSegmentCommandHandler(IItemCommercialSegmentRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<DeleteItemCommercialSegmentCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteItemCommercialSegmentCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Result<bool>.Failure("Registro no encontrado.", [new ApiError("ItemCommercialSegmentNotFound", "Registro no encontrado.", nameof(request.Id))]);
            var changed = await repository.DeleteAsync(request.Id, request.AuditUserId, string.IsNullOrWhiteSpace(request.AuditUserName) ? null : request.AuditUserName.Trim(), connection, transaction, token);
            return changed > 0 ? Result<bool>.Success(true) : Result<bool>.Failure("No se pudo eliminar Segmentos comerciales de artículos.", [new ApiError("ItemCommercialSegmentDeleteFailed", "No se pudo eliminar Segmentos comerciales de artículos.", nameof(request.Id))]);
        }, ct);
}
