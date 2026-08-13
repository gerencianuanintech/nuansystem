using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Commands;

public sealed class CreateUnitMeasureCommandHandler(
    IUnitMeasureRepository repository, ITransactionRunner transactionRunner,
    IUnitMeasureLocalOutboxWriter localOutboxWriter) : ICommandHandler<CreateUnitMeasureCommand, UnitMeasureDto>
{
    public async Task<Result<UnitMeasureDto>> Handle(CreateUnitMeasureCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        var data = new CreateUnitMeasureData(Guid.NewGuid(), code, request.Name.Trim(),
            NormalizeOptional(request.Description), NormalizeOptional(request.Symbol),
            UnitMeasureMagnitudeCodes.Normalize(request.MagnitudeCode), request.SortOrder, request.IsActive,
            NormalizeOptional(request.ExternalSystem), NormalizeOptional(request.ExternalCode),
            request.AuditUserId, NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Failure("UnitMeasureCodeAlreadyExists", "El codigo de unidad de medida ya existe.", nameof(request.Code));

            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Failure("UnitMeasureCodeAlreadyExists", "El codigo de unidad de medida ya existe.", nameof(request.Code));
            if (id <= 0)
                return Result<UnitMeasureDto>.Failure("No se pudo crear la unidad de medida.");

            var item = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("La unidad de medida fue creada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, SyncOperation.Created, connection, transaction, token);
            return Result<UnitMeasureDto>.Success(item, "Unidad de medida creada correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static Result<UnitMeasureDto> Failure(string code, string message, string field) =>
        Result<UnitMeasureDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class UpdateUnitMeasureCommandHandler(
    IUnitMeasureRepository repository, ITransactionRunner transactionRunner,
    IUnitMeasureLocalOutboxWriter localOutboxWriter) : ICommandHandler<UpdateUnitMeasureCommand, UnitMeasureDto>
{
    public async Task<Result<UnitMeasureDto>> Handle(UpdateUnitMeasureCommand request, CancellationToken cancellationToken)
    {
        var code = CreateUnitMeasureCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateUnitMeasureData(request.Id, code, request.Name.Trim(),
            CreateUnitMeasureCommandHandler.NormalizeOptional(request.Description),
            CreateUnitMeasureCommandHandler.NormalizeOptional(request.Symbol),
            UnitMeasureMagnitudeCodes.Normalize(request.MagnitudeCode), request.SortOrder, request.IsActive,
            CreateUnitMeasureCommandHandler.NormalizeOptional(request.ExternalSystem),
            CreateUnitMeasureCommandHandler.NormalizeOptional(request.ExternalCode), request.AuditUserId,
            CreateUnitMeasureCommandHandler.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Failure("UnitMeasureNotFound", "No existe la unidad de medida indicada.", nameof(request.Id));
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Failure("UnitMeasureCodeAlreadyExists", "El codigo de unidad de medida ya existe.", nameof(request.Code));

            var result = await repository.UpdateAsync(data, connection, transaction, token);
            if (result == -1)
                return Failure("UnitMeasureCodeAlreadyExists", "El codigo de unidad de medida ya existe.", nameof(request.Code));
            if (result <= 0)
                return Result<UnitMeasureDto>.Failure("No se pudo actualizar la unidad de medida.");

            var item = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("La unidad de medida fue actualizada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                connection, transaction, token);
            return Result<UnitMeasureDto>.Success(item, "Unidad de medida actualizada correctamente.");
        }, cancellationToken);
    }

    private static Result<UnitMeasureDto> Failure(string code, string message, string field) =>
        Result<UnitMeasureDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class DeleteUnitMeasureCommandHandler(
    IUnitMeasureRepository repository, ITransactionRunner transactionRunner,
    IUnitMeasureLocalOutboxWriter localOutboxWriter) : ICommandHandler<DeleteUnitMeasureCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteUnitMeasureCommand request, CancellationToken cancellationToken) =>
        await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null)
                return Result<bool>.Failure("Unidad de medida no encontrada.",
                    [new ApiError("UnitMeasureNotFound", "No existe la unidad de medida indicada.", nameof(request.Id))]);

            var result = await repository.DeleteAsync(request.Id, request.AuditUserId,
                CreateUnitMeasureCommandHandler.NormalizeOptional(request.AuditUserName), connection, transaction, token);
            if (result == -2)
                return Result<bool>.Failure("No se puede eliminar la unidad de medida porque esta en uso.",
                    [new ApiError("UnitMeasureInUse", "La unidad esta asociada a articulos, codigos o documentos.", nameof(request.Id))]);
            if (result <= 0)
                return Result<bool>.Failure("No se pudo eliminar la unidad de medida.");

            await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Unidad de medida eliminada correctamente.");
        }, cancellationToken);
}
