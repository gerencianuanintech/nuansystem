namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record RoleAccessItem(
    IReadOnlyCollection<RoleAccessMenuItem> Menus,
    IReadOnlyCollection<RoleAccessOperationItem> Operations);
