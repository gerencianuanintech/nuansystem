using System.Data;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private void ConfigureCommercialTab()
    {
        EnsureCommercialPresentationColumns();
        ConfigureCommercialLookups();

        btnRefreshPurchases.Click += (_, _) => UpdateCommercialPresentation();
        btnRefreshSales.Click += (_, _) => UpdateCommercialPresentation();
        btnViewPurchaseDocument.Click += (_, _) => grdPurchaseHistory.Focus();
        btnViewSalesHistory.Click += (_, _) => grdSalesPriceLists.Focus();

        salesPriceListsTable.RowChanged += CommercialTableChanged;
        salesPriceListsTable.RowDeleted += CommercialTableChanged;
        purchaseHistoryTable.RowChanged += CommercialTableChanged;
        purchaseHistoryTable.RowDeleted += CommercialTableChanged;

        UpdateCommercialPresentation();
    }

    private void ConfigureCommercialLookups()
    {
        var currencies = new[]
        {
            new LookupOption("USD", "USD"),
            new LookupOption("EUR", "EUR"),
            new LookupOption("COP", "COP"),
            new LookupOption("PEN", "PEN")
        };
        BindFixedLookup(lueSalesCurrency, currencies);
        BindFixedLookup(luePreferredPurchaseCurrency, currencies);
        lueSalesCurrency.EditValue = "USD";
        luePreferredPurchaseCurrency.EditValue = "USD";

        var priceLists = new[]
        {
            new LookupOption("PVP", "PVP - Precio público"),
            new LookupOption("MAY", "MAY - Mayorista"),
            new LookupOption("DIS", "DIS - Distribuidor")
        };
        BindFixedLookup(lueMainPriceList, priceLists);
        BindFixedLookup(lueSalesMinimumPriceList, priceLists);
        lueMainPriceList.EditValue = "PVP";
        lueSalesMinimumPriceList.EditValue = "PVP";
        dtSalesValidFrom.EditValue = DateTime.Today;

        luePreferredPurchasePresentation.Properties.DataSource = itemPresentationsTable;
        luePreferredPurchasePresentation.Properties.DisplayMember = "Presentacion";
        luePreferredPurchasePresentation.Properties.ValueMember = "Presentacion";
        luePreferredPurchasePresentation.Properties.NullText = string.Empty;
    }

    private void EnsureCommercialPresentationColumns()
    {
        AddColumnIfMissing(salesPriceListsTable, "ListaPrecio", typeof(string));
        AddColumnIfMissing(salesPriceListsTable, "Moneda", typeof(string));
        AddColumnIfMissing(salesPriceListsTable, "Precio", typeof(decimal));
        AddColumnIfMissing(salesPriceListsTable, "Margen", typeof(decimal));
        AddColumnIfMissing(salesPriceListsTable, "Vigencia", typeof(DateTime));
        AddColumnIfMissing(salesPriceListsTable, "Activa", typeof(bool));

        AddColumnIfMissing(purchaseHistoryTable, "Fecha", typeof(DateTime));
        AddColumnIfMissing(purchaseHistoryTable, "Documento", typeof(string));
        AddColumnIfMissing(purchaseHistoryTable, "Proveedor", typeof(string));
        AddColumnIfMissing(purchaseHistoryTable, "Presentacion", typeof(string));
        AddColumnIfMissing(purchaseHistoryTable, "Cantidad", typeof(decimal));
        AddColumnIfMissing(purchaseHistoryTable, "Unidad", typeof(string));
        AddColumnIfMissing(purchaseHistoryTable, "CantidadInventario", typeof(decimal));
        AddColumnIfMissing(purchaseHistoryTable, "CostoUnitario", typeof(decimal));
        AddColumnIfMissing(purchaseHistoryTable, "Moneda", typeof(string));
        AddColumnIfMissing(purchaseHistoryTable, "Estado", typeof(string));
    }

    private static void AddColumnIfMissing(DataTable table, string name, Type type)
    {
        if (!table.Columns.Contains(name))
        {
            table.Columns.Add(name, type);
        }
    }

    private void CommercialTableChanged(object? sender, DataRowChangeEventArgs e)
    {
        UpdateCommercialPresentation();
    }

    private void UpdateCommercialPresentation()
    {
        var purchaseCost = spnLastCost.Value > 0 ? spnLastCost.Value : spnAverageCost.Value;
        var averageCost = spnAverageCost.Value;
        var salesPrice = spnBaseSalesPrice.Value > 0 ? spnBaseSalesPrice.Value : spnAnalysisBasePrice.Value;
        var salesCurrency = ResolveLookupDisplayText(lueSalesCurrency, "USD");
        var purchaseCurrency = ResolvePurchaseCurrency();

        kpiPurchaseLast.ValueText = purchaseCost.ToString("N2");
        kpiPurchaseLast.UnitText = purchaseCurrency;
        kpiPurchaseAverage.ValueText = averageCost.ToString("N2");
        kpiPurchaseAverage.UnitText = purchaseCurrency;
        kpiSalesLastPrice.ValueText = salesPrice.ToString("N2");
        kpiSalesLastPrice.UnitText = salesCurrency;
        lblSalesMinimumCurrency.Text = salesCurrency;
        kpiPurchaseLeadTime.ValueText = spnPurchaseDeliveryDays.Value.ToString("N0");

        SetKpiAccessibility(kpiPurchaseLast);
        SetKpiAccessibility(kpiPurchaseAverage);
        SetKpiAccessibility(kpiPurchaseLeadTime);
        SetKpiAccessibility(kpiPurchaseCompliance);
        SetKpiAccessibility(kpiSales30d);
        SetKpiAccessibility(kpiSales12m);
        SetKpiAccessibility(kpiSalesLastPrice);
        SetKpiAccessibility(kpiSalesCustomers);
    }

    private string ResolvePurchaseCurrency()
    {
        var latestCurrency = purchaseHistoryTable.Rows
            .Cast<DataRow>()
            .Where(row => row.RowState != DataRowState.Deleted)
            .OrderByDescending(row => ToDateTime(row["Fecha"]))
            .Select(row => Convert.ToString(row["Moneda"]))
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

        return string.IsNullOrWhiteSpace(latestCurrency)
            ? ResolveLookupDisplayText(luePreferredPurchaseCurrency, "USD")
            : latestCurrency;
    }

    private static string ResolveLookupDisplayText(DevExpress.XtraEditors.LookUpEdit lookup, string fallback)
    {
        return string.IsNullOrWhiteSpace(lookup.Text) || lookup.Text == lookup.Properties.NullText
            ? fallback
            : lookup.Text.Trim();
    }

    private static bool ToBoolean(object value)
    {
        return value != DBNull.Value && Convert.ToBoolean(value);
    }

    private static DateTime? ToDateTime(object value)
    {
        return value == DBNull.Value ? null : Convert.ToDateTime(value);
    }
}
