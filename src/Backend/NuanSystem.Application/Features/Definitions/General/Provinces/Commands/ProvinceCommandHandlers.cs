using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Countries.Commands;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.General.Provinces.Commands;

public sealed class CreateProvinceCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, IProvinceLocalOutboxWriter localOutboxWriter) : ICommandHandler<CreateProvinceCommand, ProvinceDto>
{
    public async Task<Result<ProvinceDto>> Handle(CreateProvinceCommand request, CancellationToken cancellationToken)
    {
        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.ProvinceCodeExistsAsync(request.CountryId, code, null, connection, transaction, token)) return Result<ProvinceDto>.Failure("Ya existe una provincia con el codigo indicado para el pais.", [new ApiError("GEOGRAPHY_PROVINCE_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
            var id = await repository.CreateProvinceAsync(new SaveProvinceData(null, Guid.NewGuid(), request.CountryId, code, request.Name.Trim(), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName), CreateCountryCommandHandler.NormalizeOptional(request.ExternalSystem), CreateCountryCommandHandler.NormalizeOptional(request.ExternalCode)), connection, transaction, token);
            var province = await repository.GetProvinceByIdAsync(id, connection, transaction, token) ?? throw new InvalidOperationException("La provincia fue creada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(province, SyncOperation.Created, connection, transaction, token);
            return Result<ProvinceDto>.Success(province, "Provincia creada correctamente.");
        }, cancellationToken);
    }
}

public sealed class UpdateProvinceCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, IProvinceLocalOutboxWriter localOutboxWriter) : ICommandHandler<UpdateProvinceCommand, ProvinceDto>
{
    public async Task<Result<ProvinceDto>> Handle(UpdateProvinceCommand request, CancellationToken cancellationToken)
    {
        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var existing = await repository.GetProvinceByIdAsync(request.Id, connection, transaction, token);
            if (existing is null) return Result<ProvinceDto>.Failure("No se encontro la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe.", nameof(request.Id))]);
            if (await repository.ProvinceCodeExistsAsync(request.CountryId, code, request.Id, connection, transaction, token)) return Result<ProvinceDto>.Failure("Ya existe una provincia con el codigo indicado para el pais.", [new ApiError("GEOGRAPHY_PROVINCE_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
            var updated = await repository.UpdateProvinceAsync(new SaveProvinceData(request.Id, existing.GlobalId, request.CountryId, code, request.Name.Trim(), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName), CreateCountryCommandHandler.NormalizeOptional(request.ExternalSystem) ?? existing.ExternalSystem, CreateCountryCommandHandler.NormalizeOptional(request.ExternalCode) ?? existing.ExternalCode), connection, transaction, token);
            if (!updated) return Result<ProvinceDto>.Failure("No se pudo actualizar la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe o fue eliminada.", nameof(request.Id))]);
            var province = await repository.GetProvinceByIdAsync(request.Id, connection, transaction, token) ?? throw new InvalidOperationException("La provincia fue actualizada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(province, province.IsActive ? SyncOperation.Updated : SyncOperation.Disabled, connection, transaction, token);
            return Result<ProvinceDto>.Success(province, "Provincia actualizada correctamente.");
        }, cancellationToken);
    }
}

public sealed class DeleteProvinceCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, IProvinceLocalOutboxWriter localOutboxWriter) : ICommandHandler<DeleteProvinceCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteProvinceCommand request, CancellationToken cancellationToken) => await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
    {
        var existing = await repository.GetProvinceByIdAsync(request.Id, connection, transaction, token);
        if (existing is null) return Result<bool>.Failure("No se pudo eliminar la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe o fue eliminada.", nameof(request.Id))]);
        var deleted = await repository.DeleteProvinceAsync(request.Id, request.AuditUserId, request.AuditUserName, connection, transaction, token);
        if (!deleted) return Result<bool>.Failure("No se pudo eliminar la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe o fue eliminada.", nameof(request.Id))]);
        await localOutboxWriter.EnqueueAsync(existing, SyncOperation.Deleted, connection, transaction, token);
        return Result<bool>.Success(true, "Provincia eliminada correctamente.");
    }, cancellationToken);
}
