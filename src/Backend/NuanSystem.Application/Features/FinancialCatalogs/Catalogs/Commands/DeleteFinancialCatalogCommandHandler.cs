using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

public sealed class DeleteFinancialCatalogCommandHandler(IFinancialCatalogRepository catalogRepository)
    : ICommandHandler<DeleteFinancialCatalogCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteFinancialCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = CreateFinancialCatalogCommandHandler.NormalizeKey(request.CatalogKey);
        var deleted = await catalogRepository.DeleteAsync(
            catalogKey,
            request.Id,
            request.AuditUserId,
            CreateFinancialCatalogCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Catalogo financiero eliminado correctamente.")
            : Result<bool>.Failure(
                "No se encontro el catalogo financiero.",
                [new ApiError("FINANCIAL_CATALOG_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
    }
}
