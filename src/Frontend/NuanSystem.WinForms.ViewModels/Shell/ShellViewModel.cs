using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;
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
        Modules = securityAccessClient is null
            ? BuildAllowedModules()
            : Array.Empty<ShellModuleItem>();
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
    public string? NavigationLoadError { get; private set; }

    public bool HasModules => Modules.Count > 0;

    public void Logout()
    {
        session.Clear();
    }

    public async Task LoadNavigationAsync(CancellationToken cancellationToken = default)
    {
        if (securityAccessClient is null)
        {
            NavigationLoadError = null;
            Modules = BuildAllowedModules();
            return;
        }

        try
        {
            NavigationLoadError = null;
            var navigation = await securityAccessClient.GetNavigationAsync(cancellationToken);
            var modules = BuildModulesFromNavigation(navigation);
            NavigationMenus = navigation;
            Modules = modules;
            if (modules.Count > 0)
            {
                return;
            }

            NavigationLoadError = navigation.Count == 0
                ? "La API no devolvio menus visibles para el usuario."
                : "La API devolvio menus, pero no hay formularios navegables configurados.";
        }
        catch (Exception exception)
        {
            NavigationLoadError = exception.Message;
            NavigationMenus = Array.Empty<NavigationMenuItem>();
            Modules = Array.Empty<ShellModuleItem>();
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
            new("Seguridad", "Accesos", "Accesos a formularios de mantenimiento", "security-access", [PermissionCodes.SecurityFormAccessMaintenanceManage, PermissionCodes.RolesManage]),
            new("Seguridad", "Accesos transaccionales", "Accesos a formularios transaccionales", "security-form-access-transactional", [PermissionCodes.SecurityFormAccessTransactionalManage, PermissionCodes.RolesManage]),
            new("Seguridad", "Campos mantenimiento", "Accesos a campos de formularios de mantenimiento", "security-field-access-maintenance", [PermissionCodes.SecurityFieldAccessMaintenanceManage, PermissionCodes.RolesManage]),
            new("Seguridad", "Campos transaccionales", "Accesos a campos de formularios transaccionales", "security-field-access-transactional", [PermissionCodes.SecurityFieldAccessTransactionalManage, PermissionCodes.RolesManage]),
            new("Seguridad", "Auditoria", "Trazabilidad del sistema", "audit", [PermissionCodes.AuditRead]),
            new("Contabilidad", "Plan de cuentas", "Plan de cuentas contable", "chart-of-accounts", [PermissionCodes.AccountingRead, PermissionCodes.AccountingManage]),
            new("Catalogos", "Clientes", "Mantenimiento empresarial de clientes", "customers", [PermissionCodes.BusinessPartnersRead, PermissionCodes.BusinessPartnersManage]),
            new("Catalogos", "Proveedores", "Mantenimiento empresarial de proveedores", "suppliers", [PermissionCodes.BusinessPartnersRead, PermissionCodes.BusinessPartnersManage]),
            new("Catalogos", "Transportistas", "Mantenimiento independiente de transportistas", "carriers", [PermissionCodes.CarriersRead, PermissionCodes.CarriersManage]),
            new("Modulo de configuracion / Definiciones / General", "Países", "Mantenimiento de países", "countries", [PermissionCodes.GeographyCountriesRead, PermissionCodes.GeographyCountriesManage]),
            new("Modulo de configuracion / Definiciones / General", "Provincias", "Mantenimiento de provincias", "provinces", [PermissionCodes.GeographyProvincesRead, PermissionCodes.GeographyProvincesManage]),
            new("Modulo de configuracion / Definiciones / General", "Ciudades", "Mantenimiento de ciudades", "cities", [PermissionCodes.GeographyCitiesRead, PermissionCodes.GeographyCitiesManage]),
            new("General Proveedores", "Grupos de proveedor", "Clasificacion principal de proveedores", "supplier-groups", [PermissionCodes.GeneralSupplierSupplierGroupsRead, PermissionCodes.GeneralSupplierSupplierGroupsManage]),
            new("General Proveedores", "Clases de proveedor", "Clasificacion operativa de proveedores", "supplier-classes", [PermissionCodes.GeneralSupplierSupplierClassesRead, PermissionCodes.GeneralSupplierSupplierClassesManage]),
            new("General Proveedores", "Actividades economicas", "Actividades economicas de proveedores", "economic-activities", [PermissionCodes.GeneralSupplierEconomicActivitiesRead, PermissionCodes.GeneralSupplierEconomicActivitiesManage]),
            new("General Proveedores", "Zonas de proveedor", "Zonas comerciales de proveedores", "supplier-zones", [PermissionCodes.GeneralSupplierZonesRead, PermissionCodes.GeneralSupplierZonesManage]),
            new("General Proveedores", "Formas de abastecimiento", "Metodos de abastecimiento de proveedores", "supply-methods", [PermissionCodes.GeneralSupplierSupplyMethodsRead, PermissionCodes.GeneralSupplierSupplyMethodsManage]),
            new("General Proveedores", "Tipos de contacto", "Tipos de contacto para proveedores", "supplier-contact-types", [PermissionCodes.GeneralSupplierContactTypesRead, PermissionCodes.GeneralSupplierContactTypesManage]),
            new("General Proveedores", "Canales de contacto", "Canales de contacto para proveedores", "supplier-contact-channels", [PermissionCodes.GeneralSupplierContactChannelsRead, PermissionCodes.GeneralSupplierContactChannelsManage]),
            new("Inventario General", "Grupos de Artículos", "Maestro de grupos de artículos", "item-groups", [PermissionCodes.ItemsRead, PermissionCodes.ItemsManage]),
            new("Inventario General", "Lineas/Familias", "Maestro de lineas y familias", "item-families", [PermissionCodes.ItemsRead, PermissionCodes.ItemsManage]),
            new("Catalogos", "Articulos", "Catalogo de articulos", "items", [PermissionCodes.ItemsRead, PermissionCodes.ItemsManage]),
            new("Modulo de configuracion", "Documentos", "Mantenimiento de series de documentos", "security-document-series", [PermissionCodes.DocumentsSeriesRead, PermissionCodes.DocumentsSeriesManage]),
            new("Modulo de configuracion", "Catalogos operativos", "Mantenimiento de catalogos operativos", "operational-catalogs", [PermissionCodes.OperationalCatalogsRead, PermissionCodes.OperationalCatalogsManage]),
            new("Compras", "Ordenes de compra", "Gestion de ordenes de compra", "purchase-orders", [PermissionCodes.PurchaseOrdersRead, PermissionCodes.PurchaseOrdersManage, PermissionCodes.PurchaseOrdersApprove, PermissionCodes.PurchaseOrdersSyncSap]),
            new("Administracion", "Monitor Sync", "Monitoreo Sync Master/Sucursal", "sync-monitor", [PermissionCodes.SyncOutboxView]),
            new("Integraciones", "Perfiles de sincronizacion", "Configuracion Maestro - Sucursales", "sync-profiles", [PermissionCodes.SyncConfigurationView]),
            new("Integraciones", "Entidades de sincronizacion", "Catalogo tecnico de entidades sincronizables", "sync-entities", [PermissionCodes.SyncEntitiesView]),
            new("Integraciones", "Ejecuciones de sincronizacion", "Monitoreo administrativo de ejecuciones", "sync-executions", [PermissionCodes.SyncConfigurationViewExecutions]),
            new("Catalogos Financieros", "Metodos de pago contable", "Mantenimiento de metodos de pago contable", "accounting-payment-methods", [PermissionCodes.FinancialCatalogsAccountingPaymentMethodsRead, PermissionCodes.FinancialCatalogsAccountingPaymentMethodsManage]),
            new("Catalogos Financieros", "Prioridades de pago", "Mantenimiento de prioridades de pago", "payment-priorities", [PermissionCodes.FinancialCatalogsPaymentPrioritiesRead, PermissionCodes.FinancialCatalogsPaymentPrioritiesManage]),
            new("Catalogos Financieros", "Flujos de aprobacion", "Mantenimiento de flujos de aprobacion", "approval-flows", [PermissionCodes.FinancialCatalogsApprovalFlowsRead, PermissionCodes.FinancialCatalogsApprovalFlowsManage]),
            new("Catalogos Financieros", "Tipos de documento de pago", "Mantenimiento de tipos de documento de pago", "payment-document-types", [PermissionCodes.FinancialCatalogsPaymentDocumentTypesRead, PermissionCodes.FinancialCatalogsPaymentDocumentTypesManage]),
            new("Catalogos Financieros", "Sucursales", "Mantenimiento de sucursales", "branches", [PermissionCodes.FinancialCatalogsBranchesRead, PermissionCodes.FinancialCatalogsBranchesManage]),
            new("Catalogos Financieros", "Departamentos", "Mantenimiento de departamentos", "departments", [PermissionCodes.FinancialCatalogsDepartmentsRead, PermissionCodes.FinancialCatalogsDepartmentsManage]),
            new("Catalogos Financieros", "Lineas de negocio", "Mantenimiento de lineas de negocio", "business-lines", [PermissionCodes.FinancialCatalogsBusinessLinesRead, PermissionCodes.FinancialCatalogsBusinessLinesManage]),
            new("Catalogos Financieros", "Centros de costo", "Mantenimiento de centros de costo", "cost-centers", [PermissionCodes.FinancialCatalogsCostCentersRead, PermissionCodes.FinancialCatalogsCostCentersManage]),
            new("Catalogos Financieros", "Proyectos", "Mantenimiento de proyectos", "projects", [PermissionCodes.FinancialCatalogsProjectsRead, PermissionCodes.FinancialCatalogsProjectsManage]),
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
