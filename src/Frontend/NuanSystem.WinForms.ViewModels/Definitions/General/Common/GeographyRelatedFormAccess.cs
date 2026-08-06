using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Definitions.General.Common;

internal readonly record struct GeographyRelatedFormAccess(bool CanCreate, bool CanUpdate)
{
    public static async Task<GeographyRelatedFormAccess> LoadAsync(
        ISecurityAccessClient securityAccessClient,
        string formKey,
        CancellationToken cancellationToken = default)
    {
        var operations = await securityAccessClient.GetFormOperationsAsync(formKey, cancellationToken);
        return new GeographyRelatedFormAccess(
            Allows(operations, "create", "new", "nuevo", "crear"),
            Allows(operations, "update", "edit", "editar", "modificar"));
    }

    private static bool Allows(
        IEnumerable<FormOperationAccessItem> operations,
        params string[] aliases)
    {
        return operations.Any(operation =>
            operation.IsAllowed
            && (Matches(operation.ActionKey, aliases)
                || Matches(operation.Code, aliases)
                || Matches(operation.Name, aliases)));
    }

    private static bool Matches(string? value, IReadOnlyCollection<string> aliases)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = Normalize(value);
        return aliases.Any(alias => string.Equals(normalized, Normalize(alias), StringComparison.OrdinalIgnoreCase));
    }

    private static string Normalize(string value)
    {
        return value.Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(".", string.Empty, StringComparison.Ordinal);
    }
}
