using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Carriers.Commands;

public sealed class CreateCarrierCommandHandler(
    ICarrierRepository repository,
    ITransactionRunner transactionRunner,
    ICarrierLocalOutboxWriter localOutboxWriter) : ICommandHandler<CreateCarrierCommand, CarrierDetailDto>
{
    public async Task<Result<CarrierDetailDto>> Handle(CreateCarrierCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        var data = new CreateCarrierData(
            Guid.NewGuid(),
            code,
            request.Name.Trim(),
            request.IdentificationTypeCode.Trim(),
            request.IdentificationNumber.Trim(),
            NormalizeOptional(request.Description),
            request.IsActive,
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                {
                    return DuplicateCode(request.Code);
                }

                var createResult = await repository.CreateAsync(data, connection, transaction, token);
                if (createResult.DuplicateCode)
                {
                    return DuplicateCode(request.Code);
                }

                var id = createResult.Id
                    ?? throw new InvalidOperationException("El transportista fue creado sin devolver un identificador valido.");

                var created = await repository.GetByIdAsync(id, connection, transaction, token)
                    ?? throw new InvalidOperationException("El transportista fue creado pero no pudo consultarse.");

                await localOutboxWriter.EnqueueAsync(created, SyncOperation.Created, connection, transaction, token);
                return Result<CarrierDetailDto>.Success(created, "Transportista creado correctamente.");
            },
            cancellationToken);
    }

    internal static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    internal static Result<CarrierDetailDto> DuplicateCode(string code) =>
        Result<CarrierDetailDto>.Failure(
            "Ya existe un transportista con el codigo indicado.",
            [new ApiError("CARRIER_DUPLICATED_CODE", $"El codigo '{code.Trim()}' ya existe.", "Code")]);
}

public sealed class UpdateCarrierCommandHandler(
    ICarrierRepository repository,
    ITransactionRunner transactionRunner,
    ICarrierLocalOutboxWriter localOutboxWriter) : ICommandHandler<UpdateCarrierCommand, CarrierDetailDto>
{
    public async Task<Result<CarrierDetailDto>> Handle(UpdateCarrierCommand request, CancellationToken cancellationToken)
    {
        var code = CreateCarrierCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateCarrierData(
            request.Id,
            code,
            request.Name.Trim(),
            request.IdentificationTypeCode.Trim(),
            request.IdentificationNumber.Trim(),
            CreateCarrierCommandHandler.NormalizeOptional(request.Description),
            request.IsActive,
            request.AuditUserId,
            CreateCarrierCommandHandler.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                {
                    return NotFound(request.Id);
                }

                if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                {
                    return CreateCarrierCommandHandler.DuplicateCode(request.Code);
                }

                var updateResult = await repository.UpdateAsync(data, connection, transaction, token);
                if (updateResult.DuplicateCode)
                {
                    return CreateCarrierCommandHandler.DuplicateCode(request.Code);
                }

                if (!updateResult.Updated)
                {
                    return NotFound(request.Id);
                }

                var carrier = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                    ?? throw new InvalidOperationException("El transportista fue actualizado pero no pudo consultarse.");

                var operation = carrier.IsActive ? SyncOperation.Updated : SyncOperation.Disabled;
                await localOutboxWriter.EnqueueAsync(carrier, operation, connection, transaction, token);
                return Result<CarrierDetailDto>.Success(carrier, "Transportista actualizado correctamente.");
            },
            cancellationToken);
    }

    internal static Result<CarrierDetailDto> NotFound(int id) =>
        Result<CarrierDetailDto>.Failure(
            "No se encontro el transportista.",
            [new ApiError("CARRIER_NOT_FOUND", $"El transportista {id} no existe o fue eliminado.", "Id")]);
}

public sealed class DeleteCarrierCommandHandler(
    ICarrierRepository repository,
    ITransactionRunner transactionRunner,
    ICarrierLocalOutboxWriter localOutboxWriter) : ICommandHandler<DeleteCarrierCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteCarrierCommand request, CancellationToken cancellationToken)
    {
        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var carrier = await repository.GetByIdAsync(request.Id, connection, transaction, token);
                if (carrier is null)
                {
                    return Result<bool>.Failure("No se pudo eliminar el transportista.", [new ApiError("CARRIER_NOT_FOUND", "El transportista no existe o fue eliminado.", "Id")]);
                }

                var deleted = await repository.DeleteAsync(
                    new DeleteCarrierData(request.Id, request.AuditUserId, CreateCarrierCommandHandler.NormalizeOptional(request.AuditUserName)),
                    connection,
                    transaction,
                    token);

                if (!deleted)
                {
                    return Result<bool>.Failure("No se pudo eliminar el transportista.", [new ApiError("CARRIER_NOT_FOUND", "El transportista no existe o fue eliminado.", "Id")]);
                }

                await localOutboxWriter.EnqueueAsync(carrier, SyncOperation.Deleted, connection, transaction, token);
                return Result<bool>.Success(true, "Transportista eliminado correctamente.");
            },
            cancellationToken);
    }
}
