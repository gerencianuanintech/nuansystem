using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Services.SecurityAccess;
using NuanSystem.WinForms.Services.SecurityAccess.Models;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.WinForms.ViewModels.Shell;

public sealed class ShellViewModel
{
    private readonly ApiSession session;
    private readonly ISecurityAccessClient? securityAccessClient;

    public ShellViewModel(ApiSession session, ISecurityAccessClient? securityAccessClient = null)
    {
        this.session = session;
        this.securityAccessClient = securityAccessClient;
        Modules = BuildAllowedModules();
    }

    public string UserDisplayName => session.CurrentUser?.DisplayName ?? "Usuario";
    public string UserName => session.CurrentUser?.UserName ?? "usuario";
    public string CompanyName => session.CurrentCompany?.CommercialName ?? "Sin empresa";
    public string CompanyCode => session.CurrentCompany?.Code ?? string.Empty;
    public byte[]? CompanyLogoImage => session.CurrentCompany?.LogoImage;
    public string? CompanyLogoImageContentType => session.CurrentCompany?.LogoImageContentType;
    public string? CompanyLogoImageFileName => session.CurrentCompany?.LogoImageFileName;
    public IReadOnlyCollection<string> Roles => session.CurrentUser?.Roles ?? Array.Empty<string>();
    public IReadOnlyCollection<ShellModuleItem> Modules { get; private set; }
    public IReadOnlyCollection<NavigationMenuItem> NavigationMenus { get; private set; } = Array.Empty<NavigationMenuItem>();

    public bool HasModules => Modules.Count > 0;

    public void Logout()
    {
        session.Clear();
    }

    public async Task LoadNavigationAsync(CancellationToken cancellationToken = default)
    {
        if (securityAccessClient is null)
        {
            return;
        }

        try
        {
            var navigation = await securityAccessClient.GetNavigationAsync(cancellationToken);
            var modules = BuildModulesFromNavigation(navigation);
            if (modules.Count > 0)
            {
                NavigationMenus = navigation;
                Modules = modules;
            }
        }
        catch
        {
            Modules = BuildAllowedModules();
        }
    }

    public async Task<IReadOnlyCollection<string>> GetAllowedOperationsAsync(string formKey, CancellationToken cancellationToken = default)
    {
        var operations = await GetFormOperationsAsync(formKey, cancellationToken);
        return operations
            .Where(operation => operation.IsAllowed)
            .SelectMany(operation => new[] { operation.ActionKey, operation.Code, operation.Name })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<FormOperationAccessItem>> GetFormOperationsAsync(string formKey, CancellationToken cancellationToken = default)
    {
        if (securityAccessClient is null || string.IsNullOrWhiteSpace(formKey))
        {
            return Array.Empty<FormOperationAccessItem>();
        }

        try
        {
            return await securityAccessClient.GetFormOperationsAsync(formKey, cancellationToken);
        }
        catch
        {
            return Array.Empty<FormOperationAccessItem>();
        }
    }

    private IReadOnlyCollection<ShellModuleItem> BuildAllowedModules()
    {
        var definitions = GetModuleDefinitions();
        var permissions = new HashSet<string>(
            session.CurrentUser?.Permissions ?? Array.Empty<string>(),
            StringComparer.OrdinalIgnoreCase);

        var modules = definitions
            .Where(module => module.RequiredPermissions.Count == 0 || module.RequiredPermissions.Any(permissions.Contains))
            .ToArray();

        NavigationMenus = Array.Empty<NavigationMenuItem>();
        return modules;
    }

    private static IReadOnlyCollection<ShellModuleItem> GetModuleDefinitions()
    {
        return
        [
            new("Administracion", "Empresas", "Administracion multiempresa", "companies-admin", [PermissionCodes.CompaniesManage]),
            new("Seguridad", "Usuarios", "Administracion de usuarios", "users", [PermissionCodes.UsersManage]),
            new("Seguridad", "Roles", "Roles y permisos", "roles", [PermissionCodes.RolesManage]),
            new("Seguridad", "Operaciones", "Operaciones de seguridad", "security-operations", [PermissionCodes.RolesManage]),
            new("Seguridad", "Menus", "Estructura de navegacion", "security-menus", [PermissionCodes.RolesManage]),
            new("Seguridad", "Formularios", "Registro de pantallas", "security-forms", [PermissionCodes.RolesManage]),
            new("Seguridad", "Campos", "Campos por formulario", "security-fields", [PermissionCodes.RolesManage]),
            new("Seguridad", "Accesos", "Accesos por rol", "security-access", [PermissionCodes.RolesManage]),
            new("Seguridad", "Auditoria", "Trazabilidad del sistema", "audit", [PermissionCodes.AuditRead]),
            new("Contabilidad", "Plan de cuentas", "Plan de cuentas contable", "chart-of-accounts", [PermissionCodes.AccountingRead, PermissionCodes.AccountingManage]),
            new("Catalogos", "Clientes", "Catalogo de clientes", "customers", [PermissionCodes.CustomersRead, PermissionCodes.CustomersManage]),
            new("Inventario General", "Grupos de Artículos", "Maestro de grupos de artículos", "item-groups", [PermissionCodes.ItemsRead, PermissionCodes.ItemsManage]),
            new("Inventario General", "Lineas/Familias", "Maestro de lineas y familias", "item-families", [PermissionCodes.ItemsRead, PermissionCodes.ItemsManage]),
            new("Catalogos", "Articulos", "Catalogo de articulos", "items", [PermissionCodes.ItemsRead, PermissionCodes.ItemsManage]),
            new("Ventas", "Documentos", "Documentos comerciales", "documents", [PermissionCodes.DocumentsRead, PermissionCodes.DocumentsManage]),
            new("SAP", "Integracion SAP", "Envio y bitacora SAP", "sap", [PermissionCodes.SapRead, PermissionCodes.SapManage]),
            new("Sistema", "Configuracion", "Parametros del sistema", "settings", [PermissionCodes.SettingsManage])
        ];
    }

    private static IReadOnlyCollection<ShellModuleItem> BuildModulesFromNavigation(IReadOnlyCollection<NavigationMenuItem> navigation)
    {
        if (navigation.Count == 0)
        {
            return Array.Empty<ShellModuleItem>();
        }

        var byId = navigation.ToDictionary(menu => menu.Id);
        var modules = new List<ShellModuleItem>();

        foreach (var menu in navigation.Where(menu => !string.IsNullOrWhiteSpace(menu.FormKey)).OrderBy(menu => menu.DisplayOrder).ThenBy(menu => menu.Name))
        {
            var category = ResolveCategory(menu, byId);
            modules.Add(new ShellModuleItem(
                category,
                menu.Name,
                menu.Description ?? menu.Name,
                menu.FormKey!,
                Array.Empty<string>()));
        }

        return modules;
    }

    private static string ResolveCategory(NavigationMenuItem menu, IReadOnlyDictionary<int, NavigationMenuItem> byId)
    {
        var current = menu;
        var category = menu.Name;

        while (current.ParentId.HasValue && byId.TryGetValue(current.ParentId.Value, out var parent))
        {
            category = parent.Name;
            current = parent;
        }

        return category;
    }
}

public sealed record ShellModuleItem(
    string Category,
    string Title,
    string Description,
    string Key,
    IReadOnlyCollection<string> RequiredPermissions);
