using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

public sealed class CreateFinancialCatalogCommandHandler(
    IFinancialCatalogRepository catalogRepository,
    ITransactionRunner transactionRunner,
    ICurrencyLocalOutboxWriter currencyLocalOutboxWriter)
    : ICommandHandler<CreateFinancialCatalogCommand, FinancialCatalogDto>
{
    public async Task<Result<FinancialCatalogDto>> Handle(CreateFinancialCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = NormalizeKey(request.CatalogKey);
        var code = NormalizeCode(request.Code);
        var data = new CreateFinancialCatalogData(
            code,
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            request.IsActive,
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName));

        if (IsCurrency(catalogKey))
        {
            return await transactionRunner.ExecuteInTenantTransactionAsync(
                async (connection, transaction, token) =>
                {
                    if (await catalogRepository.ExistsByCodeAsync(
                            catalogKey, code, null, connection, transaction, token))
                    {
                        return DuplicatedCode(request.Code);
                    }

                    var id = await catalogRepository.CreateAsync(catalogKey, data, connection, transaction, token);
                    var created = await catalogRepository.GetByIdAsync(
                        catalogKey, id, connection, transaction, token)
                        ?? throw new InvalidOperationException(
                            "El catalogo financiero fue creado pero no pudo consultarse.");

                    await currencyLocalOutboxWriter.EnqueueAsync(
                        created, SyncOperation.Created, connection, transaction, token);
                    return Result<FinancialCatalogDto>.Success(
                        created, "Catalogo financiero creado correctamente.");
                },
                cancellationToken);
        }

        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, cancellationToken))
        {
            return DuplicatedCode(request.Code);
        }

        var id = await catalogRepository.CreateAsync(catalogKey, data, cancellationToken);

        var created = await catalogRepository.GetByIdAsync(catalogKey, id, cancellationToken)
            ?? throw new InvalidOperationException("El catalogo financiero fue creado pero no pudo consultarse.");

        return Result<FinancialCatalogDto>.Success(created, "Catalogo financiero creado correctamente.");
    }

    internal static bool IsCurrency(string catalogKey) =>
        string.Equals(catalogKey, CurrencySyncEventFactory.CatalogKey, StringComparison.OrdinalIgnoreCase);

    private static Result<FinancialCatalogDto> DuplicatedCode(string fieldName) =>
        Result<FinancialCatalogDto>.Failure(
            "Ya existe un catalogo financiero con el codigo indicado.",
            [new ApiError("FINANCIAL_CATALOG_DUPLICATED_CODE", "El codigo ya existe.", fieldName)]);

    internal static string NormalizeKey(string catalogKey) => catalogKey.Trim();

    internal static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
