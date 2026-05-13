using NuanSystem.Shared.Constants;

namespace NuanSystem.WinForms.Forms.Common;

public sealed record CrudOperationPermissions(
    string Read,
    string Create,
    string Update,
    string Delete)
{
    public static CrudOperationPermissions Customers { get; } = new(
        PermissionCodes.CustomersRead,
        PermissionCodes.CustomersManage,
        PermissionCodes.CustomersManage,
        PermissionCodes.CustomersManage);

    public static CrudOperationPermissions Items { get; } = new(
        PermissionCodes.ItemsRead,
        PermissionCodes.ItemsManage,
        PermissionCodes.ItemsManage,
        PermissionCodes.ItemsManage);
}
