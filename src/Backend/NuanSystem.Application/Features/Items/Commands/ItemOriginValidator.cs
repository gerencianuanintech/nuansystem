using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

internal static class ItemOriginValidator
{
    public static async Task<ApiError?> ValidateAssignmentAsync(
        string? requestedOrigin, string? currentOrigin, IItemOriginRepository repository,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var requested = Normalize(requestedOrigin);
        var current = Normalize(currentOrigin);
        if (string.Equals(requested, current, StringComparison.Ordinal)) return null;
        if (requested is null) return null;

        var origin = await repository.GetByCodeAsync(requested, connection, transaction, cancellationToken);
        if (origin is null)
            return new ApiError("ItemOriginNotFound", "No existe el origen de artículo indicado.", "MasterData.General.Origin");
        return origin.IsActive
            ? null
            : new ApiError("ItemOriginInactive", "El origen de artículo indicado está inactivo.", "MasterData.General.Origin");
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
