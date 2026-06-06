using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.OperationalCatalogs.Dtos;
using NuanSystem.Shared.Responses;
using static NuanSystem.Application.Features.OperationalCatalogs.OperationalCatalogNormalizer;

namespace NuanSystem.Application.Features.OperationalCatalogs.Commands;

public sealed class CreateOperationalCatalogCommandHandler(IOperationalCatalogRepository repository)
    : ICommandHandler<CreateOperationalCatalogCommand, OperationalCatalogDto>
{
    public async Task<Result<OperationalCatalogDto>> Handle(CreateOperationalCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = NormalizeKey(request.CatalogKey);
        var code = NormalizeCode(request.Code);

        if (await repository.ExistsByCodeAsync(catalogKey, code, null, cancellationToken))
        {
            return DuplicateFailure();
        }

        var id = await repository.CreateAsync(new CreateOperationalCatalogData(
            catalogKey,
            code,
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            NormalizeKeyOptional(request.ParentCatalogKey),
            NormalizeCodeOptional(request.ParentCode),
            request.DisplayOrder,
            request.IsDefault,
            request.IsActive,
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName)), cancellationToken);

        var item = await repository.GetByIdAsync(catalogKey, id, cancellationToken)
            ?? throw new InvalidOperationException("El valor del catalogo fue creado pero no pudo consultarse.");

        return Result<OperationalCatalogDto>.Success(item, "Valor de catalogo creado correctamente.");
    }

    private static Result<OperationalCatalogDto> DuplicateFailure()
    {
        return Result<OperationalCatalogDto>.Failure(
            "Ya existe un valor con el mismo codigo en este catalogo.",
            new[] { new ApiError("OperationalCatalogCodeAlreadyExists", "El codigo ya existe en este catalogo.", "Code") });
    }
}

public sealed class UpdateOperationalCatalogCommandHandler(IOperationalCatalogRepository repository)
    : ICommandHandler<UpdateOperationalCatalogCommand, OperationalCatalogDto>
{
    public async Task<Result<OperationalCatalogDto>> Handle(UpdateOperationalCatalogCommand request, CancellationToken cancellationToken)
    {
        var catalogKey = NormalizeKey(request.CatalogKey);
        var code = NormalizeCode(request.Code);

        if (await repository.GetByIdAsync(catalogKey, request.Id, cancellationToken) is null)
        {
            return Result<OperationalCatalogDto>.Failure("El valor del catalogo operativo no existe.");
        }

        if (await repository.ExistsByCodeAsync(catalogKey, code, request.Id, cancellationToken))
        {
            return Result<OperationalCatalogDto>.Failure(
                "Ya existe un valor con el mismo codigo en este catalogo.",
                new[] { new ApiError("OperationalCatalogCodeAlreadyExists", "El codigo ya existe en este catalogo.", "Code") });
        }

        var updated = await repository.UpdateAsync(new UpdateOperationalCatalogData(
            request.Id,
            catalogKey,
            code,
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            NormalizeKeyOptional(request.ParentCatalogKey),
            NormalizeCodeOptional(request.ParentCode),
            request.DisplayOrder,
            request.IsDefault,
            request.IsActive,
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName)), cancellationToken);

        if (!updated)
        {
            return Result<OperationalCatalogDto>.Failure("El valor del catalogo operativo no existe o fue eliminado.");
        }

        var item = await repository.GetByIdAsync(catalogKey, request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El valor del catalogo fue actualizado pero no pudo consultarse.");

        return Result<OperationalCatalogDto>.Success(item, "Valor de catalogo actualizado correctamente.");
    }
}

public sealed class DeleteOperationalCatalogCommandHandler(IOperationalCatalogRepository repository)
    : ICommandHandler<DeleteOperationalCatalogCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteOperationalCatalogCommand request, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(
            NormalizeKey(request.CatalogKey),
            request.Id,
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Valor de catalogo eliminado correctamente.")
            : Result<bool>.Failure("El valor del catalogo operativo no existe o ya fue eliminado.");
    }
}
