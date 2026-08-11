using System.Runtime.CompilerServices;
using DevExpress.Utils.Svg;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private readonly Dictionary<XtraTabPage, ItemTabIconPair> itemTabIcons = new();
    private readonly Dictionary<XtraTabPage, ItemTabIconPair> commercialTabIcons = new();
    private readonly Dictionary<XtraTabPage, ItemTabIconPair> financeTabIcons = new();

    private void tabMain_HandleCreated(object? sender, EventArgs e)
    {
        if (itemTabIcons.Count == 0)
        {
            RegisterItemTabIcons(tabGeneral, "general_24.svg", "general_active_24.svg");
            RegisterItemTabIcons(tabUnits, "unidades_codigos_24.svg", "unidades_codigos_active_24.svg");
            RegisterItemTabIcons(tabInventory, "inventario_24.svg", "inventario_active_24.svg");
            RegisterItemTabIcons(tabCommercial, "comercial_24.svg", "comercial_active_24.svg");
            RegisterItemTabIcons(tabFinance, "finanzas_24.svg", "finanzas_active_24.svg");
            RegisterItemTabIcons(tabLots, "trazabilidad_24.svg", "trazabilidad_active_24.svg");
            RegisterItemTabIcons(tabSap, "integracion_24.svg", "integracion_active_24.svg");
            RegisterItemTabIcons(tabDocuments, "documentos_24.svg", "documentos_active_24.svg");
        }

        ApplyItemTabVisualState();
    }

    private void tabMain_SelectedPageChanged(object? sender, TabPageChangedEventArgs e)
    {
        ApplyItemTabVisualState();
    }

    private void tabMain_CustomDrawTabHeader(object? sender, TabHeaderCustomDrawEventArgs e)
    {
        e.DefaultDraw();

        if (ReferenceEquals(e.TabHeaderInfo.Page, tabMain.SelectedTabPage))
        {
            var indicatorBounds = new Rectangle(
                e.Bounds.Left,
                e.Bounds.Top + 2,
                4,
                Math.Max(1, e.Bounds.Height - 4));
            using var indicatorBrush = new SolidBrush(BrandResources.Primary);
            e.Graphics.FillRectangle(indicatorBrush, indicatorBounds);
        }

        e.Handled = true;
    }

    private void tabCommercialSections_HandleCreated(object? sender, EventArgs e)
    {
        if (commercialTabIcons.Count == 0)
        {
            RegisterCommercialTabIcons(tabPurchases, "compras_20.svg", "compras_active_20.svg");
            RegisterCommercialTabIcons(tabSales, "ventas_20.svg", "ventas_active_20.svg");
        }

        ApplyCommercialTabVisualState();
    }

    private void tabCommercialSections_SelectedPageChanged(object? sender, TabPageChangedEventArgs e)
    {
        ApplyCommercialTabVisualState();
    }

    private void tabCommercialSections_CustomDrawTabHeader(object? sender, TabHeaderCustomDrawEventArgs e)
    {
        e.DefaultDraw();

        if (ReferenceEquals(e.TabHeaderInfo.Page, tabCommercialSections.SelectedTabPage))
        {
            var indicatorBounds = new Rectangle(
                e.Bounds.Left + 8,
                e.Bounds.Bottom - 3,
                Math.Max(1, e.Bounds.Width - 16),
                3);
            using var indicatorBrush = new SolidBrush(BrandResources.Primary);
            e.Graphics.FillRectangle(indicatorBrush, indicatorBounds);
        }

        e.Handled = true;
    }

    private void tabFinanceSections_HandleCreated(object? sender, EventArgs e)
    {
        if (financeTabIcons.Count == 0)
        {
            RegisterFinanceTabIcons(tabCosts, "costos_precios_20.svg", "costos_precios_active_20.svg");
            RegisterFinanceTabIcons(tabAccounting, "contabilidad_20.svg", "contabilidad_active_20.svg");
            RegisterFinanceTabIcons(tabTaxes, "impuestos_20.svg", "impuestos_active_20.svg");
        }

        ApplyFinanceTabVisualState();
    }

    private void tabFinanceSections_SelectedPageChanged(object? sender, TabPageChangedEventArgs e)
    {
        ApplyFinanceTabVisualState();
    }

    private void tabFinanceSections_CustomDrawTabHeader(object? sender, TabHeaderCustomDrawEventArgs e)
    {
        e.DefaultDraw();

        if (ReferenceEquals(e.TabHeaderInfo.Page, tabFinanceSections.SelectedTabPage))
        {
            var indicatorBounds = new Rectangle(
                e.Bounds.Left + 8,
                e.Bounds.Bottom - 3,
                Math.Max(1, e.Bounds.Width - 16),
                3);
            using var indicatorBrush = new SolidBrush(BrandResources.Primary);
            e.Graphics.FillRectangle(indicatorBrush, indicatorBounds);
        }

        e.Handled = true;
    }

    private void RegisterItemTabIcons(XtraTabPage page, string inactiveFileName, string activeFileName)
    {
        itemTabIcons[page] = new ItemTabIconPair(
            LoadItemTabIcon(inactiveFileName),
            LoadItemTabIcon(activeFileName));
    }

    private void RegisterCommercialTabIcons(XtraTabPage page, string inactiveFileName, string activeFileName)
    {
        commercialTabIcons[page] = new ItemTabIconPair(
            LoadItemTabIcon(inactiveFileName),
            LoadItemTabIcon(activeFileName));
    }

    private void RegisterFinanceTabIcons(XtraTabPage page, string inactiveFileName, string activeFileName)
    {
        financeTabIcons[page] = new ItemTabIconPair(
            LoadItemTabIcon(inactiveFileName),
            LoadItemTabIcon(activeFileName));
    }

    private void ApplyItemTabVisualState()
    {
        foreach (var entry in itemTabIcons)
        {
            entry.Key.ImageOptions.SvgImage = ReferenceEquals(entry.Key, tabMain.SelectedTabPage)
                ? entry.Value.Active
                : entry.Value.Inactive;
        }

        tabMain.Invalidate();
    }

    private void ApplyCommercialTabVisualState()
    {
        foreach (var entry in commercialTabIcons)
        {
            entry.Key.ImageOptions.SvgImage = ReferenceEquals(entry.Key, tabCommercialSections.SelectedTabPage)
                ? entry.Value.Active
                : entry.Value.Inactive;
        }

        tabCommercialSections.Invalidate();
    }

    private void ApplyFinanceTabVisualState()
    {
        foreach (var entry in financeTabIcons)
        {
            entry.Key.ImageOptions.SvgImage = ReferenceEquals(entry.Key, tabFinanceSections.SelectedTabPage)
                ? entry.Value.Active
                : entry.Value.Inactive;
        }

        tabFinanceSections.Invalidate();
    }

    private static SvgImage? LoadItemTabIcon(
        string fileName,
        [CallerFilePath] string callerFilePath = "")
    {
        var iconPath = ResolveItemTabIconPath(fileName, callerFilePath);
        return File.Exists(iconPath) ? SvgImage.FromFile(iconPath) : null;
    }

    private static string ResolveItemTabIconPath(string fileName, string callerFilePath)
    {
        var relativePath = Path.Combine("Assets", "Tabs", fileName);
        var outputPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        var sourceDirectory = Path.GetDirectoryName(callerFilePath);
        if (!string.IsNullOrWhiteSpace(sourceDirectory))
        {
            var projectPath = Path.GetFullPath(Path.Combine(sourceDirectory, "..", relativePath));
            if (File.Exists(projectPath))
            {
                return projectPath;
            }
        }

        return outputPath;
    }

    private sealed record ItemTabIconPair(SvgImage? Inactive, SvgImage? Active);
}
