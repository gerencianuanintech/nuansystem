using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TaxCatalogs.Catalogs.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.TaxCatalogs.Catalogs.Commands;

public sealed class CreateTaxCatalogCommandHandler(ITaxCatalogRepository catalogRepository)
    : ICommandHandler<CreateTaxCatalogCommand, TaxCatalogDto>
{
    public async Task<Result<TaxCatalogDto>> Handle(CreateTaxCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = NormalizeKey(request.CatalogKey);
        var code = NormalizeCode(request.Code);
        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, cancellationToken))
        {
            return Result<TaxCatalogDto>.Failure(
                "Ya existe un catalogo tributario con el codigo indicado.",
                [new ApiError("TAX_CATALOG_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var id = await catalogRepository.CreateAsync(
            catalogKey,
            new CreateTaxCatalogData(code, request.Name.Trim(), NormalizeOptional(request.Description), request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        var created = await catalogRepository.GetByIdAsync(catalogKey, id, cancellationToken)
            ?? throw new InvalidOperationException("El catalogo tributario fue creado pero no pudo consultarse.");

        return Result<TaxCatalogDto>.Success(created, "Catalogo tributario creado correctamente.");
    }

    internal static string NormalizeKey(string catalogKey) => catalogKey.Trim();

    internal static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class UpdateTaxCatalogCommandHandler(ITaxCatalogRepository catalogRepository)
    : ICommandHandler<UpdateTaxCatalogCommand, TaxCatalogDto>
{
    public async Task<Result<TaxCatalogDto>> Handle(UpdateTaxCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = CreateTaxCatalogCommandHandler.NormalizeKey(request.CatalogKey);
        var existing = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<TaxCatalogDto>.Failure("No se encontro el catalogo tributario.", [new ApiError("TAX_CATALOG_NOT_FOUND", "El registro no existe.", nameof(request.Id))]);
        }

        var code = CreateTaxCatalogCommandHandler.NormalizeCode(request.Code);
        if (await catalogRepository.ExistsByCodeAsync(catalogKey, code, request.Id, cancellationToken))
        {
            return Result<TaxCatalogDto>.Failure("Ya existe un catalogo tributario con el codigo indicado.", [new ApiError("TAX_CATALOG_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var updated = await catalogRepository.UpdateAsync(
            catalogKey,
            new UpdateTaxCatalogData(request.Id, code, request.Name.Trim(), CreateTaxCatalogCommandHandler.NormalizeOptional(request.Description), request.IsActive, request.AuditUserId, CreateTaxCatalogCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (!updated)
        {
            return Result<TaxCatalogDto>.Failure("No se pudo actualizar el catalogo tributario.", [new ApiError("TAX_CATALOG_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        var catalog = await catalogRepository.GetByIdAsync(catalogKey, request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El catalogo tributario fue actualizado pero no pudo consultarse.");

        return Result<TaxCatalogDto>.Success(catalog, "Catalogo tributario actualizado correctamente.");
    }
}

public sealed class DeleteTaxCatalogCommandHandler(ITaxCatalogRepository catalogRepository)
    : ICommandHandler<DeleteTaxCatalogCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteTaxCatalogCommand request, CancellationToken cancellationToken)
    {
        var deleted = await catalogRepository.DeleteAsync(
            CreateTaxCatalogCommandHandler.NormalizeKey(request.CatalogKey),
            request.Id,
            request.AuditUserId,
            CreateTaxCatalogCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Catalogo tributario eliminado correctamente.")
            : Result<bool>.Failure("No se encontro el catalogo tributario.", [new ApiError("TAX_CATALOG_NOT_FOUND", "El registro no existe o ya fue eliminado.", nameof(request.Id))]);
    }
}

public sealed class CreateRetentionConceptCommandHandler(ITaxCatalogRepository catalogRepository)
    : ICommandHandler<CreateRetentionConceptCommand, RetentionConceptDto>
{
    public async Task<Result<RetentionConceptDto>> Handle(CreateRetentionConceptCommand request, CancellationToken cancellationToken)
    {
        var code = CreateTaxCatalogCommandHandler.NormalizeCode(request.Code);
        if (await catalogRepository.ExistsByCodeAsync("retention-concepts", code, cancellationToken))
        {
            return Result<RetentionConceptDto>.Failure("Ya existe un concepto de retencion con el codigo indicado.", [new ApiError("RETENTION_CONCEPT_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var id = await catalogRepository.CreateRetentionConceptAsync(ToData(request, null, code), cancellationToken);
        var concept = await catalogRepository.GetRetentionConceptByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El concepto de retencion fue creado pero no pudo consultarse.");

        return Result<RetentionConceptDto>.Success(concept, "Concepto de retencion creado correctamente.");
    }

    internal static SaveRetentionConceptData ToData(CreateRetentionConceptCommand request, int? id, string code)
        => new(
            id,
            code,
            request.Name.Trim(),
            CreateTaxCatalogCommandHandler.NormalizeOptional(request.Description),
            request.RetentionTypeId,
            CreateTaxCatalogCommandHandler.NormalizeOptional(request.SriCode),
            request.Percent,
            request.AppliesIva,
            request.AppliesIncome,
            request.IsActive,
            request.AuditUserId,
            CreateTaxCatalogCommandHandler.NormalizeOptional(request.AuditUserName));
}

public sealed class UpdateRetentionConceptCommandHandler(ITaxCatalogRepository catalogRepository)
    : ICommandHandler<UpdateRetentionConceptCommand, RetentionConceptDto>
{
    public async Task<Result<RetentionConceptDto>> Handle(UpdateRetentionConceptCommand request, CancellationToken cancellationToken)
    {
        var existing = await catalogRepository.GetRetentionConceptByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<RetentionConceptDto>.Failure("No se encontro el concepto de retencion.", [new ApiError("RETENTION_CONCEPT_NOT_FOUND", "El registro no existe.", nameof(request.Id))]);
        }

        var code = CreateTaxCatalogCommandHandler.NormalizeCode(request.Code);
        if (await catalogRepository.ExistsByCodeAsync("retention-concepts", code, request.Id, cancellationToken))
        {
            return Result<RetentionConceptDto>.Failure("Ya existe un concepto de retencion con el codigo indicado.", [new ApiError("RETENTION_CONCEPT_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var updated = await catalogRepository.UpdateRetentionConceptAsync(
            new SaveRetentionConceptData(
                request.Id,
                code,
                request.Name.Trim(),
                CreateTaxCatalogCommandHandler.NormalizeOptional(request.Description),
                request.RetentionTypeId,
                CreateTaxCatalogCommandHandler.NormalizeOptional(request.SriCode),
                request.Percent,
                request.AppliesIva,
                request.AppliesIncome,
                request.IsActive,
                request.AuditUserId,
                CreateTaxCatalogCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (!updated)
        {
            return Result<RetentionConceptDto>.Failure("No se pudo actualizar el concepto de retencion.", [new ApiError("RETENTION_CONCEPT_NOT_FOUND", "El registro no existe o fue eliminado.", nameof(request.Id))]);
        }

        var concept = await catalogRepository.GetRetentionConceptByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El concepto de retencion fue actualizado pero no pudo consultarse.");

        return Result<RetentionConceptDto>.Success(concept, "Concepto de retencion actualizado correctamente.");
    }
}
