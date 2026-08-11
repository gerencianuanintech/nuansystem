using DevExpress.XtraTab;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private readonly Dictionary<XtraTabPage, ItemTabIconPair> integrationTabIcons = new();

    private void ConfigureIntegrationTab()
    {
        lueSapSyncStatus.Properties.ReadOnly = true;
        lueSapCompany.Properties.ReadOnly = true;
        lueSapMode.Properties.ReadOnly = true;
        lueSapSyncAsSupplier.Properties.ReadOnly = true;
        lueSapManualRetry.Properties.ReadOnly = true;
        lueSapRequiresApproval.Properties.ReadOnly = true;
        txtSapMapSystemField.Properties.ReadOnly = true;
        txtSapMapSapField.Properties.ReadOnly = true;
        txtSapMapDescription.Properties.ReadOnly = true;
        lueSapMapRequired.Properties.ReadOnly = true;
        lueSapMapEnabled.Properties.ReadOnly = true;
    }

    private void tabSapSections_HandleCreated(object? sender, EventArgs e)
    {
        if (integrationTabIcons.Count == 0)
        {
            integrationTabIcons[tabSapStatusPage] = new ItemTabIconPair(
                LoadItemTabIcon("integracion_estado_20.svg"),
                LoadItemTabIcon("integracion_estado_active_20.svg"));
            integrationTabIcons[tabSapHistoryPage] = new ItemTabIconPair(
                LoadItemTabIcon("integracion_historial_20.svg"),
                LoadItemTabIcon("integracion_historial_active_20.svg"));
        }

        ApplyIntegrationTabVisualState();
    }

    private void tabSapSections_SelectedPageChanged(object? sender, TabPageChangedEventArgs e)
    {
        ApplyIntegrationTabVisualState();
    }

    private void tabSapSections_CustomDrawTabHeader(object? sender, TabHeaderCustomDrawEventArgs e)
    {
        e.DefaultDraw();

        if (ReferenceEquals(e.TabHeaderInfo.Page, tabSapSections.SelectedTabPage))
        {
            Rectangle indicatorBounds = new(
                e.Bounds.Left + 8,
                e.Bounds.Bottom - 3,
                Math.Max(1, e.Bounds.Width - 16),
                3);
            using SolidBrush indicatorBrush = new(BrandResources.Primary);
            e.Graphics.FillRectangle(indicatorBrush, indicatorBounds);
        }

        e.Handled = true;
    }

    private void ApplyIntegrationTabVisualState()
    {
        foreach (KeyValuePair<XtraTabPage, ItemTabIconPair> entry in integrationTabIcons)
        {
            entry.Key.ImageOptions.SvgImage = ReferenceEquals(entry.Key, tabSapSections.SelectedTabPage)
                ? entry.Value.Active
                : entry.Value.Inactive;
            entry.Key.ImageOptions.SvgImageSize = new Size(20, 20);
        }

        tabSapSections.Invalidate();
    }
}
