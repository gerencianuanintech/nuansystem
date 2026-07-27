using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

public sealed class UpdateFinancialCatalogCommandHandler(
    IFinancialCatalogRepository catalogRepository,
    ITransactionRunner transactionRunner,
    ICurrencyLocalOutboxWriter currencyLocalOutboxWriter)
    : ICommandHandler<UpdateFinancialCatalogCommand, FinancialCatalogDto>
{
    public async Task<Result<FinancialCatalogDto>> Handle(UpdateFinancialCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = CreateFinancialCatalogCommandHandler.NormalizeKey(request.CatalogKey);
        if (CreateFinancialCatalogCommandHandler.IsCurrency(catalogKey))
        {
            return await UpdateCurrencyAsync(catalogKey, request, cancellationToken);
        }

        var existing = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<FinancialCatalogDto>.Failure(
                "No se encontro el catalogo financiero.",
                [new ApiError("FINANCIAL_CATALOG_NOT_FOUND", "El registro no existe.", nameof(request.Id))]);
        }

        var code = CreateFinancialCatalogCommandHandler.NormalizeCode(request.Code);
        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, request.Id, cancellationToken))
        {
            return Result<FinancialCatalogDto>.Failure(
                "Ya existe un catalogo financiero con el codigo indicado.",
                [new ApiError("FINANCIAL_CATALOG_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var updated = await catalogRepository.UpdateAsync(
            catalogKey,
            new UpdateFinancialCatalogData(
                request.Id,
                code,
                request.Name.Trim(),
                CreateFinancialCatalogCommandHandler.NormalizeOptional(request.Description),
                request.IsActive,
                request.AuditUserId,
                CreateFinancialCatalogCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (!updated)
        {
            return Result<FinancialCatalogDto>.Failure(
                "No se pudo actualizar el catalogo financiero.",
                [new ApiError("FINANCIAL_CATALOG_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        var catalog = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El catalogo financiero fue actualizado pero no pudo consultarse.");

        return Result<FinancialCatalogDto>.Success(catalog, "Catalogo financiero actualizado correctamente.");
    }

    private Task<Result<FinancialCatalogDto>> UpdateCurrencyAsync(
        string catalogKey,
        UpdateFinancialCatalogCommand request,
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

                var code = CreateFinancialCatalogCommandHandler.NormalizeCode(request.Code);
                if (await catalogRepository.ExistsByCodeAsync(
                        catalogKey, code, request.Id, connection, transaction, token))
                {
                    return Result<FinancialCatalogDto>.Failure(
                        "Ya existe un catalogo financiero con el codigo indicado.",
                        [new ApiError(
                            "FINANCIAL_CATALOG_DUPLICATED_CODE",
                            "El codigo ya existe.",
                            nameof(request.Code))]);
                }

                var updated = await catalogRepository.UpdateAsync(
                    catalogKey,
                    CreateData(request, code),
                    connection,
                    transaction,
                    token);
                if (!updated)
                {
                    return NotFound(request.Id);
                }

                var catalog = await catalogRepository.GetByIdAsync(
                    catalogKey, request.Id, connection, transaction, token)
                    ?? throw new InvalidOperationException(
                        "El catalogo financiero fue actualizado pero no pudo consultarse.");

                await currencyLocalOutboxWriter.EnqueueAsync(
                    catalog,
                    catalog.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                    connection,
                    transaction,
                    token);
                return Result<FinancialCatalogDto>.Success(
                    catalog, "Catalogo financiero actualizado correctamente.");
            },
            cancellationToken);
    }

    private static UpdateFinancialCatalogData CreateData(
        UpdateFinancialCatalogCommand request,
        string code) =>
        new(
            request.Id,
            code,
            request.Name.Trim(),
            CreateFinancialCatalogCommandHandler.NormalizeOptional(request.Description),
            request.IsActive,
            request.AuditUserId,
            CreateFinancialCatalogCommandHandler.NormalizeOptional(request.AuditUserName));

    private static Result<FinancialCatalogDto> NotFound(int id) =>
        Result<FinancialCatalogDto>.Failure(
            "No se encontro el catalogo financiero.",
            [new ApiError("FINANCIAL_CATALOG_NOT_FOUND", "El registro no existe.", nameof(id))]);
}
