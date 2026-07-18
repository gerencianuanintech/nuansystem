using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

public sealed class DeleteFinancialCatalogCommandHandler(
    IFinancialCatalogRepository catalogRepository,
    ISyncEventPublisher syncEventPublisher,
    ICompanyContext companyContext)
    : ICommandHandler<DeleteFinancialCatalogCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteFinancialCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = CreateFinancialCatalogCommandHandler.NormalizeKey(request.CatalogKey);
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

        var syncResult = await CurrencySyncPublisher.PublishAsync(
            syncEventPublisher,
            companyContext,
            catalogKey,
            existing,
            SyncOperation.Deleted,
            cancellationToken);

        return syncResult is { IsSuccess: false }
            ? Result<bool>.Failure(syncResult.Message, syncResult.Errors)
            : Result<bool>.Success(true, "Catalogo financiero eliminado correctamente.");
    }
}
