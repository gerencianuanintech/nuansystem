namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record RoleAccessItem(
    IReadOnlyCollection<RoleAccessMenuItem> Menus,
    IReadOnlyCollection<RoleAccessOperationItem> Operations);
