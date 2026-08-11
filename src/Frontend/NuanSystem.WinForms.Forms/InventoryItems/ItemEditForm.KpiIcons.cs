using System.Runtime.CompilerServices;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Kpi;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private void InitializeOperationalKpiIcons()
    {
        ApplyOperationalKpiIcon(kpiStockAvailable, "stock_disponible_24.svg");
        ApplyOperationalKpiIcon(kpiOnOrder, "en_pedido_24.svg");
        ApplyOperationalKpiIcon(kpiCommitted, "comprometido_24.svg");
        ApplyOperationalKpiIcon(kpiPurchases, "compras_24.svg");
        ApplyOperationalKpiIcon(kpiSales, "ventas_24.svg");
        ApplyOperationalKpiIcon(kpiAverageCost, "costo_promedio_24.svg");
        ApplyOperationalKpiIcon(kpiPurchaseCost, "costo_compra_24.svg");
        ApplyOperationalKpiIcon(kpiSalesPrice, "precio_venta_24.svg");
        ApplyOperationalKpiIcon(kpiMargin, "margen_24.svg");
        ApplyOperationalKpiIcon(kpiSapStatus, "estado_sap_24.svg");
        ApplyOperationalKpiIcon(kpiPurchaseLast, "costo_compra_24.svg");
        ApplyOperationalKpiIcon(kpiPurchaseAverage, "costo_promedio_24.svg");
        ApplyOperationalKpiIcon(kpiPurchaseLeadTime, "en_pedido_24.svg");
        ApplyOperationalKpiIcon(kpiPurchaseCompliance, "margen_24.svg");
        ApplyOperationalKpiIcon(kpiSales30d, "ventas_24.svg");
        ApplyOperationalKpiIcon(kpiSales12m, "compras_24.svg");
        ApplyOperationalKpiIcon(kpiSalesLastPrice, "precio_venta_24.svg");
        ApplyOperationalKpiIcon(kpiSalesCustomers, "comprometido_24.svg");
        ApplyOperationalKpiIcon(kpiFinanceGrossMargin, "margen_24.svg");
        ApplyOperationalKpiIcon(kpiFinanceGrossMarginPercent, "margen_24.svg");
        ApplyOperationalKpiIcon(kpiFinanceProfitability, "ventas_24.svg");
        ApplyOperationalKpiIcon(kpiFinanceSuggestedPrice, "precio_venta_24.svg");
        ApplyCommercialActionIcon(btnViewPurchaseDocument, "document_view_20.svg");
        ApplyCommercialActionIcon(btnViewSalesHistory, "document_view_20.svg");
        ApplyCommercialActionIcon(btnRefreshPurchases, "refresh_20.svg");
        ApplyCommercialActionIcon(btnRefreshSales, "refresh_20.svg");
    }

    private static void ApplyCommercialActionIcon(
        SimpleButton button,
        string fileName,
        [CallerFilePath] string callerFilePath = "")
    {
        var relativePath = Path.Combine("Assets", "Icons", "Commercial", fileName);
        var outputPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        var sourceDirectory = Path.GetDirectoryName(callerFilePath);
        var sourcePath = string.IsNullOrWhiteSpace(sourceDirectory)
            ? outputPath
            : Path.GetFullPath(Path.Combine(sourceDirectory, "..", relativePath));
        var iconPath = File.Exists(outputPath) ? outputPath : sourcePath;
        if (!File.Exists(iconPath))
        {
            return;
        }

        button.ImageOptions.SvgImage = SvgImage.FromFile(iconPath);
        button.ImageOptions.SvgImageSize = new Size(18, 18);
        button.ImageOptions.ImageToTextIndent = 6;
    }

    private static void ApplyOperationalKpiIcon(
        NuanOperationalKpiCardControl card,
        string fileName,
        [CallerFilePath] string callerFilePath = "")
    {
        var iconPath = ResolveOperationalKpiIconPath(fileName, callerFilePath);
        if (!File.Exists(iconPath))
        {
            return;
        }

        card.SvgIcon = SvgImage.FromFile(iconPath);
        card.UseSvgIcon = true;
    }

    private static string ResolveOperationalKpiIconPath(string fileName, string callerFilePath)
    {
        var relativePath = Path.Combine("Assets", "KPI", fileName);
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
}
