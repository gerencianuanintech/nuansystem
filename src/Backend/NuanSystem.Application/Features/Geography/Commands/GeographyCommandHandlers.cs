using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Geography.Commands;

public sealed class CreateCountryCommandHandler(IGeographyRepository repository)
    : ICommandHandler<CreateCountryCommand, CountryDto>
{
    public async Task<Result<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        if (await repository.CountryCodeExistsAsync(code, cancellationToken: cancellationToken))
        {
            return Result<CountryDto>.Failure("Ya existe un pais con el codigo indicado.", [new ApiError("GEOGRAPHY_COUNTRY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var id = await repository.CreateCountryAsync(
            new SaveCountryData(null, code, request.Name.Trim(), NormalizeOptional(request.Iso2), NormalizeOptional(request.Iso3), NormalizeOptional(request.PhonePrefix), request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        var created = await repository.GetCountryByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("El pais fue creado pero no pudo consultarse.");

        return Result<CountryDto>.Success(created, "Pais creado correctamente.");
    }

    internal static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class UpdateCountryCommandHandler(IGeographyRepository repository)
    : ICommandHandler<UpdateCountryCommand, CountryDto>
{
    public async Task<Result<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetCountryByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<CountryDto>.Failure("No se encontro el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe.", nameof(request.Id))]);
        }

        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        if (await repository.CountryCodeExistsAsync(code, request.Id, cancellationToken))
        {
            return Result<CountryDto>.Failure("Ya existe un pais con el codigo indicado.", [new ApiError("GEOGRAPHY_COUNTRY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var updated = await repository.UpdateCountryAsync(
            new SaveCountryData(request.Id, code, request.Name.Trim(), CreateCountryCommandHandler.NormalizeOptional(request.Iso2), CreateCountryCommandHandler.NormalizeOptional(request.Iso3), CreateCountryCommandHandler.NormalizeOptional(request.PhonePrefix), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (!updated)
        {
            return Result<CountryDto>.Failure("No se pudo actualizar el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe o fue eliminado.", nameof(request.Id))]);
        }

        return Result<CountryDto>.Success((await repository.GetCountryByIdAsync(request.Id, cancellationToken))!, "Pais actualizado correctamente.");
    }
}

public sealed class DeleteCountryCommandHandler(IGeographyRepository repository)
    : ICommandHandler<DeleteCountryCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteCountryAsync(request.Id, request.AuditUserId, request.AuditUserName, cancellationToken);
        return deleted
            ? Result<bool>.Success(true, "Pais eliminado correctamente.")
            : Result<bool>.Failure("No se pudo eliminar el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe o fue eliminado.", nameof(request.Id))]);
    }
}

public sealed class CreateProvinceCommandHandler(IGeographyRepository repository)
    : ICommandHandler<CreateProvinceCommand, ProvinceDto>
{
    public async Task<Result<ProvinceDto>> Handle(CreateProvinceCommand request, CancellationToken cancellationToken)
    {
        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        if (await repository.ProvinceCodeExistsAsync(request.CountryId, code, cancellationToken: cancellationToken))
        {
            return Result<ProvinceDto>.Failure("Ya existe una provincia con el codigo indicado para el pais.", [new ApiError("GEOGRAPHY_PROVINCE_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var id = await repository.CreateProvinceAsync(new SaveProvinceData(null, request.CountryId, code, request.Name.Trim(), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);
        return Result<ProvinceDto>.Success((await repository.GetProvinceByIdAsync(id, cancellationToken))!, "Provincia creada correctamente.");
    }
}

public sealed class UpdateProvinceCommandHandler(IGeographyRepository repository)
    : ICommandHandler<UpdateProvinceCommand, ProvinceDto>
{
    public async Task<Result<ProvinceDto>> Handle(UpdateProvinceCommand request, CancellationToken cancellationToken)
    {
        if (await repository.GetProvinceByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<ProvinceDto>.Failure("No se encontro la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe.", nameof(request.Id))]);
        }

        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        if (await repository.ProvinceCodeExistsAsync(request.CountryId, code, request.Id, cancellationToken))
        {
            return Result<ProvinceDto>.Failure("Ya existe una provincia con el codigo indicado para el pais.", [new ApiError("GEOGRAPHY_PROVINCE_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var updated = await repository.UpdateProvinceAsync(new SaveProvinceData(request.Id, request.CountryId, code, request.Name.Trim(), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);
        return updated
            ? Result<ProvinceDto>.Success((await repository.GetProvinceByIdAsync(request.Id, cancellationToken))!, "Provincia actualizada correctamente.")
            : Result<ProvinceDto>.Failure("No se pudo actualizar la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe o fue eliminada.", nameof(request.Id))]);
    }
}

public sealed class DeleteProvinceCommandHandler(IGeographyRepository repository)
    : ICommandHandler<DeleteProvinceCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteProvinceCommand request, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteProvinceAsync(request.Id, request.AuditUserId, request.AuditUserName, cancellationToken);
        return deleted
            ? Result<bool>.Success(true, "Provincia eliminada correctamente.")
            : Result<bool>.Failure("No se pudo eliminar la provincia.", [new ApiError("GEOGRAPHY_PROVINCE_NOT_FOUND", "La provincia no existe o fue eliminada.", nameof(request.Id))]);
    }
}

public sealed class CreateCityCommandHandler(IGeographyRepository repository)
    : ICommandHandler<CreateCityCommand, CityDto>
{
    public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        if (await repository.CityCodeExistsAsync(request.ProvinceId, code, cancellationToken: cancellationToken))
        {
            return Result<CityDto>.Failure("Ya existe una ciudad con el codigo indicado para la provincia.", [new ApiError("GEOGRAPHY_CITY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var id = await repository.CreateCityAsync(new SaveCityData(null, request.CountryId, request.ProvinceId, code, request.Name.Trim(), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);
        return Result<CityDto>.Success((await repository.GetCityByIdAsync(id, cancellationToken))!, "Ciudad creada correctamente.");
    }
}

public sealed class UpdateCityCommandHandler(IGeographyRepository repository)
    : ICommandHandler<UpdateCityCommand, CityDto>
{
    public async Task<Result<CityDto>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        if (await repository.GetCityByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<CityDto>.Failure("No se encontro la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe.", nameof(request.Id))]);
        }

        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        if (await repository.CityCodeExistsAsync(request.ProvinceId, code, request.Id, cancellationToken))
        {
            return Result<CityDto>.Failure("Ya existe una ciudad con el codigo indicado para la provincia.", [new ApiError("GEOGRAPHY_CITY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
        }

        var updated = await repository.UpdateCityAsync(new SaveCityData(request.Id, request.CountryId, request.ProvinceId, code, request.Name.Trim(), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);
        return updated
            ? Result<CityDto>.Success((await repository.GetCityByIdAsync(request.Id, cancellationToken))!, "Ciudad actualizada correctamente.")
            : Result<CityDto>.Failure("No se pudo actualizar la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe o fue eliminada.", nameof(request.Id))]);
    }
}

public sealed class DeleteCityCommandHandler(IGeographyRepository repository)
    : ICommandHandler<DeleteCityCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteCityAsync(request.Id, request.AuditUserId, request.AuditUserName, cancellationToken);
        return deleted
            ? Result<bool>.Success(true, "Ciudad eliminada correctamente.")
            : Result<bool>.Failure("No se pudo eliminar la ciudad.", [new ApiError("GEOGRAPHY_CITY_NOT_FOUND", "La ciudad no existe o fue eliminada.", nameof(request.Id))]);
    }
}
