using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Commands;

public sealed class CreateProductTypeCommandHandler(
    IProductTypeRepository repository, ITransactionRunner transactionRunner,
    IProductTypeLocalOutboxWriter localOutboxWriter) : ICommandHandler<CreateProductTypeCommand, ProductTypeDto>
{
    public async Task<Result<ProductTypeDto>> Handle(CreateProductTypeCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        var data = new CreateProductTypeData(code, request.Name.Trim(), NormalizeOptional(request.Description),
            ProductTypeNatureCodes.Normalize(request.NatureCode), request.SortOrder, request.IsActive, Guid.NewGuid(),
            request.AuditUserId, NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                return Failure("ProductTypeCodeAlreadyExists", "El codigo de tipo de producto ya existe.", nameof(request.Code));

            var id = await repository.CreateAsync(data, connection, transaction, token);
            if (id == -1)
                return Failure("ProductTypeCodeAlreadyExists", "El codigo de tipo de producto ya existe.", nameof(request.Code));
            if (id <= 0) return Result<ProductTypeDto>.Failure("No se pudo crear el tipo de producto.");

            var item = await repository.GetByIdAsync(id, connection, transaction, token)
                ?? throw new InvalidOperationException("El tipo de producto fue creado pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, SyncOperation.Created, connection, transaction, token);
            return Result<ProductTypeDto>.Success(item, "Tipo de producto creado correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string value) => value.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    internal static Result<ProductTypeDto> Failure(string code, string message, string field) =>
        Result<ProductTypeDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class UpdateProductTypeCommandHandler(
    IProductTypeRepository repository, ITransactionRunner transactionRunner,
    IProductTypeLocalOutboxWriter localOutboxWriter) : ICommandHandler<UpdateProductTypeCommand, ProductTypeDto>
{
    public async Task<Result<ProductTypeDto>> Handle(UpdateProductTypeCommand request, CancellationToken cancellationToken)
    {
        var code = CreateProductTypeCommandHandler.NormalizeCode(request.Code);
        var data = new UpdateProductTypeData(request.Id, code, request.Name.Trim(),
            CreateProductTypeCommandHandler.NormalizeOptional(request.Description),
            ProductTypeNatureCodes.Normalize(request.NatureCode), request.SortOrder, request.IsActive,
            request.AuditUserId, CreateProductTypeCommandHandler.NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.GetByIdAsync(request.Id, connection, transaction, token) is null)
                return Failure("ProductTypeNotFound", "No existe el tipo de producto indicado.", nameof(request.Id));
            if (await repository.ExistsByCodeAsync(code, request.Id, connection, transaction, token))
                return Failure("ProductTypeCodeAlreadyExists", "El codigo de tipo de producto ya existe.", nameof(request.Code));

            var result = await repository.UpdateAsync(data, connection, transaction, token);
            if (result == -1)
                return Failure("ProductTypeCodeAlreadyExists", "El codigo de tipo de producto ya existe.", nameof(request.Code));
            if (result == -2)
                return Failure("ProductTypeSystemProtected", "No se puede cambiar el codigo ni la naturaleza de un tipo de producto del sistema.", nameof(request.Id));
            if (result <= 0) return Result<ProductTypeDto>.Failure("No se pudo actualizar el tipo de producto.");

            var item = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                ?? throw new InvalidOperationException("El tipo de producto fue actualizado pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(item, item.IsActive ? SyncOperation.Updated : SyncOperation.Disabled,
                connection, transaction, token);
            return Result<ProductTypeDto>.Success(item, "Tipo de producto actualizado correctamente.");
        }, cancellationToken);
    }

    private static Result<ProductTypeDto> Failure(string code, string message, string field) =>
        CreateProductTypeCommandHandler.Failure(code, message, field);
}

public sealed class DeleteProductTypeCommandHandler(
    IProductTypeRepository repository, ITransactionRunner transactionRunner,
    IProductTypeLocalOutboxWriter localOutboxWriter) : ICommandHandler<DeleteProductTypeCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteProductTypeCommand request, CancellationToken cancellationToken) =>
        await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
            if (current is null)
                return Failure("ProductTypeNotFound", "No existe el tipo de producto indicado.");

            var result = await repository.DeleteAsync(request.Id, request.AuditUserId,
                CreateProductTypeCommandHandler.NormalizeOptional(request.AuditUserName), connection, transaction, token);
            if (result == -2)
                return Failure("ProductTypeSystemProtected", "No se puede eliminar un tipo de producto del sistema.");
            if (result == -3)
                return Failure("ProductTypeInUse", "El tipo de producto esta asociado a otros registros.");
            if (result <= 0) return Result<bool>.Failure("No se pudo eliminar el tipo de producto.");

            await localOutboxWriter.EnqueueAsync(current, SyncOperation.Deleted, connection, transaction, token);
            return Result<bool>.Success(true, "Tipo de producto eliminado correctamente.");
        }, cancellationToken);

    private static Result<bool> Failure(string code, string message) =>
        Result<bool>.Failure(message, [new ApiError(code, message, nameof(DeleteProductTypeCommand.Id))]);
}
