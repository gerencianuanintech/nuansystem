using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class CitySyncApplyRepository(ICompanyResolver companyResolver) : ICitySyncApplyRepository
{
    public Task<CitySyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CitySyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default) =>
        ApplyAsync(branchCompanyId, context, payload, operation, false, cancellationToken);

    public Task<CitySyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CitySyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default) =>
        ApplyAsync(
            branchCompanyId,
            context,
            payload,
            markDeleted ? SyncOperation.Deleted : SyncOperation.Disabled,
            markDeleted,
            cancellationToken);

    private async Task<CitySyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CitySyncPayload payload,
        SyncOperation operation,
        bool markDeleted,
        CancellationToken cancellationToken)
    {
        var company = await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro la sucursal destino {branchCompanyId}.");
        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
        {
            throw new NotSupportedException($"El motor {company.DatabaseEngine} no esta implementado para Sync Cities.");
        }

        var isDeleted = markDeleted || operation == SyncOperation.Deleted || payload.IsDeleted;
        await using var connection = new SqlConnection(company.ConnectionString);
        var result = await connection.QuerySingleAsync<ApplyResult>(
            new CommandDefinition(
                "dbo.SP_NA_POST_CITY_SYNC_APPLY_EVENT",
                new
                {
                    context.EventId,
                    context.SourceCompanyId,
                    context.EntityName,
                    context.EntityGlobalId,
                    Operation = operation.ToString(),
                    context.PayloadJson,
                    payload.GlobalId,
                    payload.CountryGlobalId,
                    payload.ProvinceGlobalId,
                    Code = Required(payload.Code, nameof(payload.Code), 20),
                    Name = Required(payload.Name, nameof(payload.Name), 120),
                    IsActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive,
                    IsDeleted = isDeleted,
                    ExternalSystem = Optional(payload.ExternalSystem, nameof(payload.ExternalSystem), 50),
                    ExternalCode = Optional(payload.ExternalCode, nameof(payload.ExternalCode), 100),
                    CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                    UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return result.ResultCode switch
        {
            2 => new(true, true, false, result.CityId, "Evento ya aplicado en SyncInbox."),
            -2 => new(false, false, true, null, "El codigo de City pertenece a otro GlobalId dentro de la provincia.", "SYNC_CITY_CODE_CONFLICT"),
            -3 => new(false, false, true, null, "La referencia externa de City pertenece a otro GlobalId.", "SYNC_CITY_EXTERNAL_CONFLICT"),
            -4 => new(false, false, true, null, "ProvinceGlobalId no pertenece al CountryGlobalId del payload.", "SYNC_CITY_HIERARCHY_CONFLICT"),
            -5 => new(false, false, true, null, "City ya pertenece a otro CountryGlobalId o ProvinceGlobalId.", "SYNC_CITY_PARENT_CONFLICT"),
            -6 => new(false, false, true, null, "El evento City ya se encuentra en DeadLetter en la sucursal.", "SYNC_CITY_TERMINAL_CONFLICT"),
            > 0 => new(true, false, false, result.CityId, $"Ciudad sincronizada por GlobalId {payload.GlobalId}."),
            _ => new(false, false, false, null, "No se pudo aplicar City.", "SYNC_CITY_APPLY_FAILED")
        };
    }

    private static string Required(string value, string field, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{field} es requerido.");
        }

        var result = value.Trim();
        if (result.Length > max)
        {
            throw new InvalidOperationException($"{field} excede la longitud permitida.");
        }

        return result;
    }

    private static string? Optional(string? value, string field, int max)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var result = value.Trim();
        if (result.Length > max)
        {
            throw new InvalidOperationException($"{field} excede la longitud permitida.");
        }

        return result;
    }

    private sealed class ApplyResult
    {
        public int ResultCode { get; set; }

        public int? CityId { get; set; }
    }
}
