using MediatR;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Warehouses.Contracts;

namespace NuanSystem.Application.Features.SapSync.Warehouses.Services;

public sealed class SapWarehouseRecordProcessor(IWarehouseRepository warehouseRepository, ISender sender)
{
    private const string SapExternalSystem = "SAP_B1";

    public async Task<SapWarehouseRecordProcessResult> ProcessAsync(
        SapWarehouseSnapshot snapshot,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var code = Normalize(snapshot.WarehouseCode);
        var name = Normalize(snapshot.WarehouseName);
        if (code.Length == 0 || name.Length == 0)
        {
            return Result(SapSyncExecutionDetailActions.Skip, SapSyncExecutionDetailStatuses.Skipped,
                null, SapWarehouseResultCodes.Invalid, "La bodega SAP no tiene codigo o nombre.");
        }

        var localWarehouses = await warehouseRepository.GetAllAsync(cancellationToken);
        var sapMatches = localWarehouses.Where(item => EqualsCode(item.SapCode, code)).ToArray();
        if (sapMatches.Length > 1)
        {
            return Result(SapSyncExecutionDetailActions.Conflict, SapSyncExecutionDetailStatuses.Conflict,
                null, SapWarehouseResultCodes.IdentityConflict,
                "Existe mas de una bodega local con el mismo codigo SAP.");
        }

        var local = sapMatches.SingleOrDefault();
        if (local is null)
        {
            var codeMatches = localWarehouses.Where(item => EqualsCode(item.Code, code)).ToArray();
            if (codeMatches.Length > 0)
            {
                return Result(SapSyncExecutionDetailActions.Approval,
                    SapSyncExecutionDetailStatuses.ApprovalRequired, codeMatches[0],
                    SapWarehouseResultCodes.CodeCollisionApprovalRequired,
                    "Existe una bodega con el mismo codigo, pero su relacion SAP requiere aprobacion.");
            }

            if (!snapshot.IsActive)
            {
                return Result(SapSyncExecutionDetailActions.Skip, SapSyncExecutionDetailStatuses.Skipped,
                    null, SapWarehouseResultCodes.Inactive,
                    "La bodega SAP nueva esta inactiva y no se crea automaticamente.");
            }

            var created = await sender.Send(new CreateWarehouseCommand(
                GlobalId: null, Code: code, Name: name,
                Description: "Importada desde SAP Business One.", BranchCode: null,
                Address: NormalizeOptional(snapshot.Street), City: NormalizeOptional(snapshot.City),
                Province: NormalizeOptional(snapshot.Province), Country: NormalizeOptional(snapshot.Country),
                Phone: null, Email: null, ManagerName: null,
                AllowsSales: true, AllowsPurchases: true, AllowsTransfers: true,
                AllowsProduction: false, IsDefault: false,
                ExternalSystem: SapExternalSystem, ExternalCode: code, SapCode: code,
                IsActive: true, AuditUserId: auditUserId, AuditUserName: auditUserName), cancellationToken);

            return created.IsSuccess && created.Value is not null
                ? Result(SapSyncExecutionDetailActions.Create, SapSyncExecutionDetailStatuses.Created,
                    created.Value, SapWarehouseResultCodes.Created, "Bodega creada desde SAP.")
                : Result(SapSyncExecutionDetailActions.Create, SapSyncExecutionDetailStatuses.Failed,
                    null, SapWarehouseResultCodes.SaveFailed, SafeMessage(created.Message));
        }

        if (!snapshot.IsActive && local.IsActive)
        {
            return Result(SapSyncExecutionDetailActions.Approval,
                SapSyncExecutionDetailStatuses.ApprovalRequired, local,
                SapWarehouseResultCodes.ApprovalRequired,
                "SAP reporta la bodega inactiva; la bodega local permanece activa hasta aprobacion.");
        }

        if (!HasRelevantChanges(snapshot, local))
        {
            return Result(SapSyncExecutionDetailActions.NoChange, SapSyncExecutionDetailStatuses.Unchanged,
                local, SapWarehouseResultCodes.Unchanged, "La bodega local ya esta actualizada.");
        }

        var updated = await sender.Send(new UpdateWarehouseCommand(
            local.Id, local.GlobalId, local.Code, name, local.Description, local.BranchCode,
            NormalizeOptional(snapshot.Street), NormalizeOptional(snapshot.City),
            NormalizeOptional(snapshot.Province), NormalizeOptional(snapshot.Country),
            local.Phone, local.Email, local.ManagerName,
            local.AllowsSales, local.AllowsPurchases, local.AllowsTransfers, local.AllowsProduction,
            local.IsDefault, SapExternalSystem, code, code, local.IsActive,
            auditUserId, auditUserName), cancellationToken);

        return updated.IsSuccess && updated.Value is not null
            ? Result(SapSyncExecutionDetailActions.Update, SapSyncExecutionDetailStatuses.Updated,
                updated.Value, SapWarehouseResultCodes.Updated, "Bodega actualizada desde SAP.")
            : Result(SapSyncExecutionDetailActions.Update, SapSyncExecutionDetailStatuses.Failed,
                local, SapWarehouseResultCodes.SaveFailed, SafeMessage(updated.Message));
    }

    private static bool HasRelevantChanges(SapWarehouseSnapshot snapshot, WarehouseDto local) =>
        !EqualsText(snapshot.WarehouseName, local.Name)
        || !EqualsText(snapshot.Street, local.Address)
        || !EqualsText(snapshot.City, local.City)
        || !EqualsText(snapshot.Province, local.Province)
        || !EqualsText(snapshot.Country, local.Country)
        || !EqualsCode(local.ExternalSystem, SapExternalSystem)
        || !EqualsCode(local.ExternalCode, snapshot.WarehouseCode)
        || !EqualsCode(local.SapCode, snapshot.WarehouseCode);

    private static SapWarehouseRecordProcessResult Result(
        string action, string status, WarehouseDto? local, string resultCode, string safeMessage) =>
        new(action, status, local?.Id, local?.GlobalId, resultCode, safeMessage);

    private static bool EqualsCode(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
    private static bool EqualsText(string? left, string? right) =>
        string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
    private static string SafeMessage(string? message) =>
        string.IsNullOrWhiteSpace(message) ? "No fue posible guardar la bodega." : message.Trim();
    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;
    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
