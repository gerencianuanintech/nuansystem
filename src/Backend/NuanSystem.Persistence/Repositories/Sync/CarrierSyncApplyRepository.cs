using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class CarrierSyncApplyRepository(ICompanyResolver companyResolver) : ICarrierSyncApplyRepository
{
    private const string ApplyProcedure = "dbo.SP_NA_POST_CARRIER_SYNC_APPLY_EVENT";

    public async Task<CarrierSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CarrierSyncPayloadV1 payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        ValidatePayload(payload);

        var company = await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro la sucursal destino {branchCompanyId}.");

        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
        {
            throw new NotSupportedException(
                $"El motor {company.DatabaseEngine} no esta implementado para Sync Transportistas.");
        }

        var isDeleted = operation == SyncOperation.Deleted || payload.IsDeleted;
        var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;

        await using var connection = new SqlConnection(company.ConnectionString);
        var result = await connection.QuerySingleAsync<ApplyResult>(new CommandDefinition(
            ApplyProcedure,
            new
            {
                context.EventId,
                context.SourceCompanyId,
                context.EntityName,
                context.EntityGlobalId,
                Operation = operation.ToString(),
                context.PayloadJson,
                payload.GlobalId,
                Code = Required(payload.Code, nameof(payload.Code), 50),
                Name = Required(payload.Name, nameof(payload.Name), 150),
                IdentificationTypeCode = Required(
                    payload.IdentificationTypeCode,
                    nameof(payload.IdentificationTypeCode),
                    2),
                IdentificationNumber = Required(
                    payload.IdentificationNumber,
                    nameof(payload.IdentificationNumber),
                    30),
                Description = Optional(payload.Description, nameof(payload.Description), 500),
                IsActive = isActive,
                IsDeleted = isDeleted,
                CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
            },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));

        return result.ResultCode switch
        {
            2 => new(true, true, false, result.CarrierId, "Evento ya aplicado en SyncInbox."),
            -2 => new(false, false, true, null,
                "El codigo de Transportista pertenece a otro GlobalId; no se realizo adopcion automatica.",
                "SYNC_CARRIER_CODE_CONFLICT"),
            > 0 => new(true, false, false, result.CarrierId,
                $"Transportista sincronizado por GlobalId {payload.GlobalId}."),
            _ => new(false, false, false, null,
                "No se pudo aplicar Transportista.",
                "SYNC_CARRIER_APPLY_FAILED")
        };
    }

    private static void ValidatePayload(CarrierSyncPayloadV1 payload)
    {
        if (payload.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("GlobalId es requerido para sincronizar Transportistas.");
        }

        if (payload.IdentificationTypeCode is not ("04" or "05" or "06"))
        {
            throw new InvalidOperationException(
                "IdentificationTypeCode no pertenece al catalogo cerrado de Transportistas.");
        }
    }

    private static string Required(string value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{field} es requerido para sincronizar Transportistas.");
        }

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
        {
            throw new InvalidOperationException($"{field} excede la longitud permitida.");
        }

        return normalized;
    }

    private static string? Optional(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
        {
            throw new InvalidOperationException($"{field} excede la longitud permitida.");
        }

        return normalized;
    }

    private sealed class ApplyResult
    {
        public ApplyResult()
        {
        }

        public int ResultCode { get; set; }

        public int? CarrierId { get; set; }
    }
}
