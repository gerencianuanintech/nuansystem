using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Services.GeneralSupplier.Catalogs;

namespace NuanSystem.WinForms.ViewModels.GeneralSupplier.Catalogs;

public static class GeneralSupplierCatalogDescriptors
{
    public static GeneralSupplierCatalogDescriptor SupplierGroups { get; } = new(
        GeneralSupplierCatalogRoutes.SupplierGroups,
        "supplier-groups",
        "Grupos de proveedor",
        "grupo de proveedor",
        "Codigo del grupo",
        "Nombre del grupo",
        PermissionCodes.GeneralSupplierSupplierGroupsRead,
        PermissionCodes.GeneralSupplierSupplierGroupsManage,
        PermissionCodes.GeneralSupplierSupplierGroupsManage,
        PermissionCodes.GeneralSupplierSupplierGroupsManage);

    public static GeneralSupplierCatalogDescriptor SupplierClasses { get; } = new(
        GeneralSupplierCatalogRoutes.SupplierClasses,
        "supplier-classes",
        "Clases de proveedor",
        "clase de proveedor",
        "Codigo de la clase",
        "Nombre de la clase",
        PermissionCodes.GeneralSupplierSupplierClassesRead,
        PermissionCodes.GeneralSupplierSupplierClassesManage,
        PermissionCodes.GeneralSupplierSupplierClassesManage,
        PermissionCodes.GeneralSupplierSupplierClassesManage);

    public static GeneralSupplierCatalogDescriptor EconomicActivities { get; } = new(
        GeneralSupplierCatalogRoutes.EconomicActivities,
        "economic-activities",
        "Actividades economicas",
        "actividad economica",
        "Codigo de actividad",
        "Nombre de actividad",
        PermissionCodes.GeneralSupplierEconomicActivitiesRead,
        PermissionCodes.GeneralSupplierEconomicActivitiesManage,
        PermissionCodes.GeneralSupplierEconomicActivitiesManage,
        PermissionCodes.GeneralSupplierEconomicActivitiesManage);

    public static GeneralSupplierCatalogDescriptor Zones { get; } = new(
        GeneralSupplierCatalogRoutes.Zones,
        "supplier-zones",
        "Zonas de proveedor",
        "zona de proveedor",
        "Codigo de zona",
        "Nombre de zona",
        PermissionCodes.GeneralSupplierZonesRead,
        PermissionCodes.GeneralSupplierZonesManage,
        PermissionCodes.GeneralSupplierZonesManage,
        PermissionCodes.GeneralSupplierZonesManage);

    public static GeneralSupplierCatalogDescriptor SupplyMethods { get; } = new(
        GeneralSupplierCatalogRoutes.SupplyMethods,
        "supply-methods",
        "Formas de abastecimiento",
        "forma de abastecimiento",
        "Codigo de forma",
        "Nombre de forma",
        PermissionCodes.GeneralSupplierSupplyMethodsRead,
        PermissionCodes.GeneralSupplierSupplyMethodsManage,
        PermissionCodes.GeneralSupplierSupplyMethodsManage,
        PermissionCodes.GeneralSupplierSupplyMethodsManage);

    public static GeneralSupplierCatalogDescriptor ContactTypes { get; } = new(
        GeneralSupplierCatalogRoutes.ContactTypes,
        "supplier-contact-types",
        "Tipos de contacto proveedor",
        "tipo de contacto",
        "Codigo del tipo",
        "Nombre del tipo",
        PermissionCodes.GeneralSupplierContactTypesRead,
        PermissionCodes.GeneralSupplierContactTypesManage,
        PermissionCodes.GeneralSupplierContactTypesManage,
        PermissionCodes.GeneralSupplierContactTypesManage);

    public static GeneralSupplierCatalogDescriptor ContactChannels { get; } = new(
        GeneralSupplierCatalogRoutes.ContactChannels,
        "supplier-contact-channels",
        "Canales de contacto proveedor",
        "canal de contacto",
        "Codigo del canal",
        "Nombre del canal",
        PermissionCodes.GeneralSupplierContactChannelsRead,
        PermissionCodes.GeneralSupplierContactChannelsManage,
        PermissionCodes.GeneralSupplierContactChannelsManage,
        PermissionCodes.GeneralSupplierContactChannelsManage);
}
