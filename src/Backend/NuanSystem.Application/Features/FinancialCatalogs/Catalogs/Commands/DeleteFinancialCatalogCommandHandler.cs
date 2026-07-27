using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

public sealed class DeleteFinancialCatalogCommandHandler(
    IFinancialCatalogRepository catalogRepository,
    ITransactionRunner transactionRunner,
    ICurrencyLocalOutboxWriter currencyLocalOutboxWriter)
    : ICommandHandler<DeleteFinancialCatalogCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteFinancialCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = CreateFinancialCatalogCommandHandler.NormalizeKey(request.CatalogKey);
        if (CreateFinancialCatalogCommandHandler.IsCurrency(catalogKey))
        {
            return await DeleteCurrencyAsync(catalogKey, request, cancellationToken);
        }

        var existing = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<bool>.Failure(
                "No se encontro el catalogo financiero.",
                [new ApiError("FINANCIAL_CATALOG_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        var deleted = await catalogRepository.DeleteAsync(
            catalogKey,
            request.Id,
            request.AuditUserId,
            CreateFinancialCatalogCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        if (!deleted)
        {
            return Result<bool>.Failure(
                "No se encontro el catalogo financiero.",
                [new ApiError("FINANCIAL_CATALOG_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        return Result<bool>.Success(true, "Catalogo financiero eliminado correctamente.");
    }

    private Task<Result<bool>> DeleteCurrencyAsync(
        string catalogKey,
        DeleteFinancialCatalogCommand request,
        CancellationToken cancellationToken)
    {
        return transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var existing = await catalogRepository.GetByIdAsync(
                    catalogKey, request.Id, connection, transaction, token);
                if (existing is null)
                {
                    return NotFound(request.Id);
                }

                var deleted = await catalogRepository.DeleteAsync(
                    catalogKey,
                    request.Id,
                    request.AuditUserId,
                    CreateFinancialCatalogCommandHandler.NormalizeOptional(request.AuditUserName),
                    connection,
                    transaction,
                    token);
                if (!deleted)
                {
                    return NotFound(request.Id);
                }

                await currencyLocalOutboxWriter.EnqueueAsync(
                    existing, SyncOperation.Deleted, connection, transaction, token);
                return Result<bool>.Success(true, "Catalogo financiero eliminado correctamente.");
            },
            cancellationToken);
    }

    private static Result<bool> NotFound(int id) =>
        Result<bool>.Failure(
            "No se encontro el catalogo financiero.",
            [new ApiError(
                "FINANCIAL_CATALOG_NOT_FOUND",
                "El registro no existe o fue eliminado.",
                nameof(id))]);
}
