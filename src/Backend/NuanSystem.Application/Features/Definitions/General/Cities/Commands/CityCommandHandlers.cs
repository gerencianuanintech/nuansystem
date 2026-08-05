using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Commands;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.General.Cities.Commands;

public sealed class CreateCityCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, ICityLocalOutboxWriter localOutboxWriter) : ICommandHandler<CreateCityCommand, CityDto>
{
    public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.CityCodeExistsAsync(request.ProvinceId, code, null, connection, transaction, token)) return Result<CityDto>.Failure("Ya existe una ciudad con el codigo indicado para la provincia.", [new ApiError("GEOGRAPHY_CITY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
            var id = await repository.CreateCityAsync(new SaveCityData(null, Guid.NewGuid(), request.CountryId, request.ProvinceId, code, request.Name.Trim(), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName), CreateCountryCommandHandler.NormalizeOptional(request.ExternalSystem), CreateCountryCommandHandler.NormalizeOptional(request.ExternalCode)), connection, transaction, token);
            var city = await repository.GetCityByIdAsync(id, connection, transaction, token) ?? throw new InvalidOperationException("La ciudad fue creada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(city, SyncOperation.Created, connection, transaction, token);
            return Result<CityDto>.Success(city, "Ciudad creada correctamente.");
        }, cancellationToken);
    }
}

public sealed class UpdateCityCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, ICityLocalOutboxWriter localOutboxWriter) : ICommandHandler<UpdateCityCommand, CityDto>
{
    public async Task<Result<CityDto>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var existing = await repository.GetCityByIdAsync(request.Id, connection, transaction, token);
            if (existing is null) return Result<CityDto>.Failure("No se encontro la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe.", nameof(request.Id))]);
            if (existing.CountryId != request.CountryId || existing.ProvinceId != request.ProvinceId) return Result<CityDto>.Failure("No se puede reasignar la ciudad a otro pais o provincia.", [new ApiError("GEOGRAPHY_CITY_PARENT_CHANGE_NOT_ALLOWED", "El pais y la provincia de una ciudad no pueden modificarse.", nameof(request.ProvinceId))]);
            if (await repository.CityCodeExistsAsync(request.ProvinceId, code, request.Id, connection, transaction, token)) return Result<CityDto>.Failure("Ya existe una ciudad con el codigo indicado para la provincia.", [new ApiError("GEOGRAPHY_CITY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
            var updated = await repository.UpdateCityAsync(new SaveCityData(request.Id, existing.GlobalId, request.CountryId, request.ProvinceId, code, request.Name.Trim(), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName), existing.ExternalSystem, existing.ExternalCode), connection, transaction, token);
            if (!updated) return Result<CityDto>.Failure("No se pudo actualizar la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe o fue eliminada.", nameof(request.Id))]);
            var city = await repository.GetCityByIdAsync(request.Id, connection, transaction, token) ?? throw new InvalidOperationException("La ciudad fue actualizada pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(city, city.IsActive ? SyncOperation.Updated : SyncOperation.Disabled, connection, transaction, token);
            return Result<CityDto>.Success(city, "Ciudad actualizada correctamente.");
        }, cancellationToken);
    }
}

public sealed class DeleteCityCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, ICityLocalOutboxWriter localOutboxWriter) : ICommandHandler<DeleteCityCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteCityCommand request, CancellationToken cancellationToken) => await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
    {
        var existing = await repository.GetCityByIdAsync(request.Id, connection, transaction, token);
        if (existing is null) return Result<bool>.Failure("No se pudo eliminar la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe o fue eliminada.", nameof(request.Id))]);
        var deleted = await repository.DeleteCityAsync(request.Id, request.AuditUserId, request.AuditUserName, connection, transaction, token);
        if (!deleted) return Result<bool>.Failure("No se pudo eliminar la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe o fue eliminada.", nameof(request.Id))]);
        await localOutboxWriter.EnqueueAsync(existing, SyncOperation.Deleted, connection, transaction, token);
        return Result<bool>.Success(true, "Ciudad eliminada correctamente.");
    }, cancellationToken);
}
