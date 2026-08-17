using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Commands;

public sealed class CreateReplenishmentMethodCommandHandler(
    IReplenishmentMethodRepository repository,
    ITransactionRunner transactionRunner,
    IReplenishmentMethodLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<CreateReplenishmentMethodCommand, ReplenishmentMethodDto>
{
    public Task<Result<ReplenishmentMethodDto>> Handle(CreateReplenishmentMethodCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        var data = new CreateReplenishmentMethodData(Guid.NewGuid(), code, request.Name.Trim(),
            NormalizeOptional(request.Description), request.SortOrder, request.IsActive,
            request.AuditUserId, NormalizeOptional(request.AuditUserName));

        return transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Failure("ReplenishmentMethodCodeAlreadyExists", "El código del método de reposición ya existe.", nameof(request.Code));

            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1) return Failure("ReplenishmentMethodCodeAlreadyExists", "El código del método de reposición ya existe.", nameof(request.Code));
            if (id <= 0) return Result<ReplenishmentMethodDto>.Failure("No se pudo crear el método de reposición.");

            var item = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("El método de reposición fue creado pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, SyncOperation.Created, connection, transaction, token);
            return Result<ReplenishmentMethodDto>.Success(item, "Método de reposición creado correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string value) => value.Trim();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    internal static Result<ReplenishmentMethodDto> Failure(string code, string message, string field) =>
        Result<ReplenishmentMethodDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class UpdateReplenishmentMethodCommandHandler(
    IReplenishmentMethodRepository repository,
    ITransactionRunner transactionRunner,
    IReplenishmentMethodLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<UpdateReplenishmentMethodCommand, ReplenishmentMethodDto>
{
    public Task<Result<ReplenishmentMethodDto>> Handle(UpdateReplenishmentMethodCommand request, CancellationToken cancellationToken)
    {
        var code = CreateReplenishmentMethodCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateReplenishmentMethodData(request.Id, code, request.Name.Trim(),
            CreateReplenishmentMethodCommandHandler.NormalizeOptional(request.Description), request.SortOrder,
            request.IsActive, request.AuditUserId,
            CreateReplenishmentMethodCommandHandler.NormalizeOptional(request.AuditUserName));

        return transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null) return Failure("ReplenishmentMethodNotFound", "No existe el método de reposición indicado.", nameof(request.Id));
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Failure("ReplenishmentMethodCodeAlreadyExists", "El código del método de reposición ya existe.", nameof(request.Code));

            var affected = await repository.UpdateAsync(data, connection, transaction, token);
            if (affected == -1) return Failure("ReplenishmentMethodCodeAlreadyExists", "El código del método de reposición ya existe.", nameof(request.Code));
            if (affected <= 0) return Result<ReplenishmentMethodDto>.Failure("No se pudo actualizar el método de reposición.");

            var item = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("El método de reposición fue actualizado pero no pudo consultarse.");
            var operation = item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled;
            await localOutboxWriter.EnqueueAsync(item, operation, connection, transaction, token);
            return Result<ReplenishmentMethodDto>.Success(item, "Método de reposición actualizado correctamente.");
        }, cancellationToken);
    }

    private static Result<ReplenishmentMethodDto> Failure(string code, string message, string field) =>
        CreateReplenishmentMethodCommandHandler.Failure(code, message, field);
}

public sealed class DeleteReplenishmentMethodCommandHandler(
    IReplenishmentMethodRepository repository,
    ITransactionRunner transactionRunner,
    IReplenishmentMethodLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<DeleteReplenishmentMethodCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteReplenishmentMethodCommand request, CancellationToken cancellationToken) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null) return Failure("ReplenishmentMethodNotFound", "No existe el método de reposición indicado.");

            var affected = await repository.DeleteAsync(request.Id, request.AuditUserId,
                CreateReplenishmentMethodCommandHandler.NormalizeOptional(request.AuditUserName), connection, transaction, token);
            if (affected == -3) return Failure("ReplenishmentMethodInUse", "El método de reposición está asociado a otros registros.");
            if (affected <= 0) return Result<bool>.Failure("No se pudo eliminar el método de reposición.");

            await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Método de reposición eliminado correctamente.");
        }, cancellationToken);

    private static Result<bool> Failure(string code, string message) =>
        Result<bool>.Failure(message, [new ApiError(code, message, nameof(DeleteReplenishmentMethodCommand.Id))]);
}
