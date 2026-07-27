using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Commands;

public sealed class CreatePriceListCommandHandler(
    IPriceListRepository repository,
    ITransactionRunner transactionRunner,
    IPriceListLocalOutboxWriter outboxWriter)
    : ICommandHandler<CreatePriceListCommand, PriceListDto>
{
    public Task<Result<PriceListDto>> Handle(CreatePriceListCommand request, CancellationToken cancellationToken) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var values = PriceListCommandHandler.Normalize(request.Code, request.Name, request.Description,
                request.CurrencyCode, request.AppliesTo, request.IsDefault, request.IsActive);

            var validation = await PriceListCommandHandler.ValidateAsync(
                repository, values, null, connection, transaction, token);
            if (validation is not null)
            {
                return validation;
            }

            var id = await repository.CreateAsync(new CreatePriceListData(
                Guid.NewGuid(), values.Code, values.Name, values.Description, values.CurrencyCode,
                values.AppliesTo, values.IsDefault, values.IsActive, request.AuditUserId,
                PriceListCommandHandler.NormalizeOptional(request.AuditUserName)), connection, transaction, token);

            var created = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("La lista fue creada pero no pudo consultarse.");
            await outboxWriter.EnqueueAsync(created, SyncOperation.Created, connection, transaction, token);
            return Result<PriceListDto>.Success(created, "Lista de precios creada correctamente.");
        }, cancellationToken);
}

public sealed class UpdatePriceListCommandHandler(
    IPriceListRepository repository,
    ITransactionRunner transactionRunner,
    IPriceListLocalOutboxWriter outboxWriter)
    : ICommandHandler<UpdatePriceListCommand, PriceListDto>
{
    public Task<Result<PriceListDto>> Handle(UpdatePriceListCommand request, CancellationToken cancellationToken) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
            {
                return PriceListCommandHandler.NotFound<PriceListDto>(nameof(request.Id));
            }

            var values = PriceListCommandHandler.Normalize(request.Code, request.Name, request.Description,
                request.CurrencyCode, request.AppliesTo, request.IsDefault, request.IsActive);
            var validation = await PriceListCommandHandler.ValidateAsync(
                repository, values, request.Id, connection, transaction, token);
            if (validation is not null)
            {
                return validation;
            }

            if (!await repository.UpdateAsync(new UpdatePriceListData(
                    request.Id, values.Code, values.Name, values.Description, values.CurrencyCode,
                    values.AppliesTo, values.IsDefault, values.IsActive, request.AuditUserId,
                    PriceListCommandHandler.NormalizeOptional(request.AuditUserName)),
                connection, transaction, token))
            {
                return PriceListCommandHandler.NotFound<PriceListDto>(nameof(request.Id));
            }

            var updated = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("La lista fue actualizada pero no pudo consultarse.");
            await outboxWriter.EnqueueAsync(updated,
                updated.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                connection, transaction, token);
            return Result<PriceListDto>.Success(updated, "Lista de precios actualizada correctamente.");
        }, cancellationToken);
}

public sealed class DeletePriceListCommandHandler(
    IPriceListRepository repository,
    ITransactionRunner transactionRunner,
    IPriceListLocalOutboxWriter outboxWriter)
    : ICommandHandler<DeletePriceListCommand, bool>
{
    public Task<Result<bool>> Handle(DeletePriceListCommand request, CancellationToken cancellationToken) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null)
            {
                return PriceListCommandHandler.NotFound<bool>(nameof(request.Id));
            }

            if (await repository.HasActiveReferencesAsync(current.Id, current.Code, connection, transaction, token))
            {
                return Result<bool>.Failure("La lista de precios tiene referencias operativas activas.",
                    [new ApiError("PRICE_LIST_ACTIVE_REFERENCES", "No se puede eliminar una lista en uso.", nameof(request.Id))]);
            }

            if (!await repository.DeleteAsync(request.Id, request.AuditUserId,
                    PriceListCommandHandler.NormalizeOptional(request.AuditUserName),
                    connection, transaction, token))
            {
                return PriceListCommandHandler.NotFound<bool>(nameof(request.Id));
            }

            await outboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Lista de precios eliminada correctamente.");
        }, cancellationToken);
}

internal static class PriceListCommandHandler
{
    public static PriceListValues Normalize(string code, string name, string? description,
        string currencyCode, string appliesTo, bool isDefault, bool isActive) =>
        new(code.Trim().ToUpperInvariant(), name.Trim(), NormalizeOptional(description),
            currencyCode.Trim().ToUpperInvariant(), appliesTo.Trim(), isDefault, isActive);

    public static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static async Task<Result<PriceListDto>?> ValidateAsync(
        IPriceListRepository repository, PriceListValues values, int? excludingId,
        System.Data.IDbConnection connection, System.Data.IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        if (await repository.ExistsByCodeAsync(values.Code, excludingId, connection, transaction, cancellationToken))
        {
            return Result<PriceListDto>.Failure("El código ya está reservado por otra lista.",
                [new ApiError("PRICE_LIST_CODE_CONFLICT", "El código ya existe, incluso como registro eliminado.", nameof(values.Code))]);
        }

        if (await repository.GetCurrencyAsync(values.CurrencyCode, connection, transaction, cancellationToken) is null)
        {
            return Result<PriceListDto>.Failure("La moneda no existe o no está activa.",
                [new ApiError("PRICE_LIST_CURRENCY_NOT_FOUND", "Seleccione una moneda activa.", nameof(values.CurrencyCode))]);
        }

        if (values.IsDefault && await repository.HasDefaultConflictAsync(
                values.AppliesTo, excludingId, connection, transaction, cancellationToken))
        {
            return Result<PriceListDto>.Failure("Ya existe una lista predeterminada para el ámbito indicado.",
                [new ApiError("PRICE_LIST_DEFAULT_CONFLICT", "Sales y Purchasing admiten una sola lista predeterminada efectiva.", nameof(values.IsDefault))]);
        }

        return null;
    }

    public static Result<T> NotFound<T>(string field) =>
        Result<T>.Failure("Lista de precios no encontrada.",
            [new ApiError("PRICE_LIST_NOT_FOUND", "El registro no existe o fue eliminado.", field)]);
}

internal sealed record PriceListValues(
    string Code, string Name, string? Description, string CurrencyCode,
    string AppliesTo, bool IsDefault, bool IsActive);
