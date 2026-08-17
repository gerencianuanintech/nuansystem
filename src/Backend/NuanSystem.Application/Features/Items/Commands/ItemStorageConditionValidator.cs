using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Responses;
namespace NuanSystem.Application.Features.Items.Commands;
internal static class ItemStorageConditionValidator
{
    public static async Task<ApiError?> ValidateAssignmentAsync(string? requested,string? current,IStorageConditionRepository repository,IDbConnection connection,IDbTransaction transaction,CancellationToken ct)
    {
        if(string.Equals(requested,current,StringComparison.Ordinal))return null;
        var code=string.IsNullOrWhiteSpace(requested)?null:requested.Trim();
        if(code is null)return null;
        var condition=await repository.GetByCodeAsync(code,connection,transaction,ct);
        if(condition is null || !string.Equals(condition.Code,code,StringComparison.Ordinal))
            return new ApiError("StorageConditionNotFound","No existe la condición de almacenamiento indicada.","MasterData.Inventory.Condition");
        return condition.IsActive?null:new ApiError("StorageConditionInactive","La condición de almacenamiento indicada está inactiva.","MasterData.Inventory.Condition");
    }
}
