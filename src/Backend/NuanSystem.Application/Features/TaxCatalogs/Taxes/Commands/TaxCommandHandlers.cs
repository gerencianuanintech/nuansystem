using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.TaxCatalogs.Taxes.Commands;

public sealed class CreateTaxCommandHandler(
    ITaxRepository repository,
    ITransactionRunner transactionRunner,
    ITaxLocalOutboxWriter outboxWriter) : ICommandHandler<CreateTaxCommand, TaxDto>
{
    public Task<Result<TaxDto>> Handle(CreateTaxCommand request, CancellationToken cancellationToken) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var values = TaxCommandRules.Normalize(request.Code, request.Name, request.Description, request.Rate, request.IsActive);
            var validation = await TaxCommandRules.ValidateAsync(repository, values, null, connection, transaction, token);
            if (validation is not null) return validation;

            var id = await repository.CreateAsync(new CreateTaxData(
                Guid.NewGuid(), values.Code, values.Name, values.Description, values.Rate, values.IsActive,
                request.AuditUserId, TaxCommandRules.Optional(request.AuditUserName)), connection, transaction, token);
            var created = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("El impuesto fue creado pero no pudo consultarse.");
            await outboxWriter.EnqueueAsync(created, SyncOperation.Created, connection, transaction, token);
            return Result<TaxDto>.Success(created, "Impuesto creado correctamente.");
        }, cancellationToken);
}

public sealed class UpdateTaxCommandHandler(
    ITaxRepository repository,
    ITransactionRunner transactionRunner,
    ITaxLocalOutboxWriter outboxWriter) : ICommandHandler<UpdateTaxCommand, TaxDto>
{
    public Task<Result<TaxDto>> Handle(UpdateTaxCommand request, CancellationToken cancellationToken) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return TaxCommandRules.NotFound<TaxDto>(nameof(request.Id));

            var values = TaxCommandRules.Normalize(request.Code, request.Name, request.Description, request.Rate, request.IsActive);
            var validation = await TaxCommandRules.ValidateAsync(repository, values, request.Id, connection, transaction, token);
            if (validation is not null) return validation;

            if (!await repository.UpdateAsync(new UpdateTaxData(
                    request.Id, values.Code, values.Name, values.Description, values.Rate, values.IsActive,
                    request.AuditUserId, TaxCommandRules.Optional(request.AuditUserName)),
                connection, transaction, token))
                return TaxCommandRules.NotFound<TaxDto>(nameof(request.Id));

            var updated = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("El impuesto fue actualizado pero no pudo consultarse.");
            await outboxWriter.EnqueueAsync(updated,
                updated.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                connection, transaction, token);
            return Result<TaxDto>.Success(updated, "Impuesto actualizado correctamente.");
        }, cancellationToken);
}

public sealed class DeleteTaxCommandHandler(
    ITaxRepository repository,
    ITransactionRunner transactionRunner,
    ITaxLocalOutboxWriter outboxWriter) : ICommandHandler<DeleteTaxCommand, bool>
{
    public Task<Result<bool>> Handle(DeleteTaxCommand request, CancellationToken cancellationToken) =>
        transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null) return TaxCommandRules.NotFound<bool>(nameof(request.Id));
            if (await repository.HasActiveItemReferencesAsync(current.Id, connection, transaction, token))
                return Result<bool>.Failure("El impuesto tiene artículos activos asociados.",
                    [new ApiError("TAX_ACTIVE_ITEM_REFERENCES", "Desactive o reasigne los artículos antes de eliminar.", nameof(request.Id))]);

            if (!await repository.DeleteAsync(request.Id, request.AuditUserId,
                    TaxCommandRules.Optional(request.AuditUserName), connection, transaction, token))
                return TaxCommandRules.NotFound<bool>(nameof(request.Id));

            await outboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Impuesto eliminado correctamente.");
        }, cancellationToken);
}

internal static class TaxCommandRules
{
    public static TaxValues Normalize(string code, string name, string? description, decimal rate, bool isActive) =>
        new(code.Trim().ToUpperInvariant(), name.Trim(), Optional(description), rate, isActive);

    public static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static async Task<Result<TaxDto>?> ValidateAsync(
        ITaxRepository repository, TaxValues values, int? excludingId,
        System.Data.IDbConnection connection, System.Data.IDbTransaction transaction, CancellationToken token)
    {
        if (values.Rate is < 0m or > 1m)
            return Result<TaxDto>.Failure("La tasa debe estar entre 0% y 100%.",
                [new ApiError("TAX_RATE_OUT_OF_RANGE", "La API recibe la tasa decimal entre 0 y 1.", nameof(values.Rate))]);
        if (await repository.ExistsByCodeAsync(values.Code, excludingId, connection, transaction, token))
            return Result<TaxDto>.Failure("El código ya está reservado por otro impuesto.",
                [new ApiError("TAX_CODE_CONFLICT", "El código existe, incluso como registro eliminado.", nameof(values.Code))]);
        return null;
    }

    public static Result<T> NotFound<T>(string field) =>
        Result<T>.Failure("Impuesto no encontrado.",
            [new ApiError("TAX_NOT_FOUND", "El registro no existe o fue eliminado.", field)]);
}

internal sealed record TaxValues(string Code, string Name, string? Description, decimal Rate, bool IsActive);
