using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

internal static class ItemReplenishmentMethodValidator
{
    public static async Task<ApiError?> ValidateAssignmentAsync(
        string? requestedMethod,
        string? currentMethod,
        IReplenishmentMethodRepository repository,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        if (string.Equals(requestedMethod, currentMethod, StringComparison.Ordinal)) return null;

        var requested = Normalize(requestedMethod);
        if (requested is null) return null;

        var method = await repository.GetByCodeAsync(requested, connection, transaction, cancellationToken);
        if (method is null)
            return new ApiError("ReplenishmentMethodNotFound", "No existe el método de reposición indicado.", "MasterData.Inventory.ReplenishmentMethod");

        return method.IsActive
            ? null
            : new ApiError("ReplenishmentMethodInactive", "El método de reposición indicado está inactivo.", "MasterData.Inventory.ReplenishmentMethod");
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
