using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Commands;

public sealed class CreateSalesChannelCommandHandler(ISalesChannelRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<CreateSalesChannelCommand, SalesChannelDto>
{
    public Task<Result<SalesChannelDto>> Handle(CreateSalesChannelCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var code = request.Code.Trim();
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Result<SalesChannelDto>.Failure("El código ya existe.", [new ApiError("SalesChannelCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var data = new CreateSalesChannelData(Guid.NewGuid(), code, request.Name.Trim(), NormalizeOptional(request.Description), request.SortOrder, request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName));
            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Result<SalesChannelDto>.Failure("El código ya existe.", [new ApiError("SalesChannelCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var item = id > 0 ? await repository.GetByIdAsync(id, connection, transaction, token) : null;
            return item is null ? Result<SalesChannelDto>.Failure("No se pudo crear Canales de venta.") : Result<SalesChannelDto>.Success(item);
        }, ct);

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class UpdateSalesChannelCommandHandler(ISalesChannelRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<UpdateSalesChannelCommand, SalesChannelDto>
{
    public Task<Result<SalesChannelDto>> Handle(UpdateSalesChannelCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Result<SalesChannelDto>.Failure("Registro no encontrado.", [new ApiError("SalesChannelNotFound", "Registro no encontrado.", nameof(request.Id))]);
            var code = request.Code.Trim();
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Result<SalesChannelDto>.Failure("El código ya existe.", [new ApiError("SalesChannelCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var data = new UpdateSalesChannelData(request.Id, code, request.Name.Trim(), NormalizeOptional(request.Description), request.SortOrder, request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName));
            var changed = await repository.UpdateAsync(data, connection, transaction, token);
            if (changed == -1)
                return Result<SalesChannelDto>.Failure("El código ya existe.", [new ApiError("SalesChannelCodeAlreadyExists", "El código ya existe.", nameof(request.Code))]);
            var item = changed > 0 ? await repository.GetByIdAsync(request.Id, connection, transaction, token) : null;
            return item is null ? Result<SalesChannelDto>.Failure("No se pudo actualizar Canales de venta.") : Result<SalesChannelDto>.Success(item);
        }, ct);

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class DeleteSalesChannelCommandHandler(ISalesChannelRepository repository, ITransactionRunner transactionRunner)
    : ICommandHandler<DeleteSalesChannelCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteSalesChannelCommand request, CancellationToken ct) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Result<bool>.Failure("Registro no encontrado.", [new ApiError("SalesChannelNotFound", "Registro no encontrado.", nameof(request.Id))]);
            var changed = await repository.DeleteAsync(request.Id, request.AuditUserId, string.IsNullOrWhiteSpace(request.AuditUserName) ? null : request.AuditUserName.Trim(), connection, transaction, token);
            return changed > 0 ? Result<bool>.Success(true) : Result<bool>.Failure("No se pudo eliminar Canales de venta.", [new ApiError("SalesChannelDeleteFailed", "No se pudo eliminar Canales de venta.", nameof(request.Id))]);
        }, ct);
}


