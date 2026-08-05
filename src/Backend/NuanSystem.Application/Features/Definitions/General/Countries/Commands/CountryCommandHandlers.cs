using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.General.Countries.Commands;

public sealed class CreateCountryCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, ICountryLocalOutboxWriter localOutboxWriter) : ICommandHandler<CreateCountryCommand, CountryDto>
{
    public async Task<Result<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            if (await repository.CountryCodeExistsAsync(code, null, connection, transaction, token))
                return Result<CountryDto>.Failure("Ya existe un pais con el codigo indicado.", [new ApiError("GEOGRAPHY_COUNTRY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
            var id = await repository.CreateCountryAsync(new SaveCountryData(null, Guid.NewGuid(), code, request.Name.Trim(), NormalizeOptional(request.Iso2), NormalizeOptional(request.Iso3), NormalizeOptional(request.PhonePrefix), request.IsActive, request.AuditUserId, NormalizeOptional(request.AuditUserName), NormalizeOptional(request.ExternalSystem), NormalizeOptional(request.ExternalCode)), connection, transaction, token);
            var created = await repository.GetCountryByIdAsync(id, connection, transaction, token) ?? throw new InvalidOperationException("El pais fue creado pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(created, SyncOperation.Created, connection, transaction, token);
            return Result<CountryDto>.Success(created, "Pais creado correctamente.");
        }, cancellationToken);
    }

    internal static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class UpdateCountryCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, ICountryLocalOutboxWriter localOutboxWriter) : ICommandHandler<UpdateCountryCommand, CountryDto>
{
    public async Task<Result<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var code = CreateCountryCommandHandler.NormalizeCode(request.Code);
        return await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
        {
            var existing = await repository.GetCountryByIdAsync(request.Id, connection, transaction, token);
            if (existing is null) return Result<CountryDto>.Failure("No se encontro el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe.", nameof(request.Id))]);
            if (await repository.CountryCodeExistsAsync(code, request.Id, connection, transaction, token)) return Result<CountryDto>.Failure("Ya existe un pais con el codigo indicado.", [new ApiError("GEOGRAPHY_COUNTRY_DUPLICATED_CODE", "El codigo ya existe.", nameof(request.Code))]);
            var updated = await repository.UpdateCountryAsync(new SaveCountryData(request.Id, existing.GlobalId, code, request.Name.Trim(), CreateCountryCommandHandler.NormalizeOptional(request.Iso2), CreateCountryCommandHandler.NormalizeOptional(request.Iso3), CreateCountryCommandHandler.NormalizeOptional(request.PhonePrefix), request.IsActive, request.AuditUserId, CreateCountryCommandHandler.NormalizeOptional(request.AuditUserName), CreateCountryCommandHandler.NormalizeOptional(request.ExternalSystem) ?? existing.ExternalSystem, CreateCountryCommandHandler.NormalizeOptional(request.ExternalCode) ?? existing.ExternalCode), connection, transaction, token);
            if (!updated) return Result<CountryDto>.Failure("No se pudo actualizar el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe o fue eliminado.", nameof(request.Id))]);
            var country = await repository.GetCountryByIdAsync(request.Id, connection, transaction, token) ?? throw new InvalidOperationException("El pais fue actualizado pero no pudo consultarse.");
            await localOutboxWriter.EnqueueAsync(country, country.IsActive ? SyncOperation.Updated : SyncOperation.Disabled, connection, transaction, token);
            return Result<CountryDto>.Success(country, "Pais actualizado correctamente.");
        }, cancellationToken);
    }
}

public sealed class DeleteCountryCommandHandler(IGeographyRepository repository, ITransactionRunner transactionRunner, ICountryLocalOutboxWriter localOutboxWriter) : ICommandHandler<DeleteCountryCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken) => await transactionRunner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
    {
        var existing = await repository.GetCountryByIdAsync(request.Id, connection, transaction, token);
        if (existing is null) return Result<bool>.Failure("No se pudo eliminar el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe o fue eliminado.", nameof(request.Id))]);
        var deleted = await repository.DeleteCountryAsync(request.Id, request.AuditUserId, request.AuditUserName, connection, transaction, token);
        if (!deleted) return Result<bool>.Failure("No se pudo eliminar el pais.", [new ApiError("GEOGRAPHY_COUNTRY_NOT_FOUND", "El pais no existe o fue eliminado.", nameof(request.Id))]);
        await localOutboxWriter.EnqueueAsync(existing, SyncOperation.Deleted, connection, transaction, token);
        return Result<bool>.Success(true, "Pais eliminado correctamente.");
    }, cancellationToken);
}
