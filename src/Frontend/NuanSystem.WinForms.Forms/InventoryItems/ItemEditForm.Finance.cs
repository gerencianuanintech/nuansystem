namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private void ConfigureFinanceTab()
    {
        tabCosts.Appearance.PageClient.BackColor = Color.White;
        tabCosts.Appearance.PageClient.Options.UseBackColor = true;
        tabAccounting.Appearance.PageClient.BackColor = Color.White;
        tabAccounting.Appearance.PageClient.Options.UseBackColor = true;
        tabTaxes.Appearance.PageClient.BackColor = Color.White;
        tabTaxes.Appearance.PageClient.Options.UseBackColor = true;

        spnSuggestedPrice.EditValueChanged += FinancePresentationChanged;

        UpdateFinancePresentation();
    }

    private void FinancePresentationChanged(object? sender, EventArgs e)
    {
        UpdateFinancePresentation();
    }

    private void UpdateFinancePresentation()
    {
        kpiFinanceSuggestedPrice.ValueText = spnSuggestedPrice.Value.ToString("N2");

        SetKpiAccessibility(kpiFinanceGrossMargin);
        SetKpiAccessibility(kpiFinanceGrossMarginPercent);
        SetKpiAccessibility(kpiFinanceProfitability);
        SetKpiAccessibility(kpiFinanceSuggestedPrice);
    }
}
