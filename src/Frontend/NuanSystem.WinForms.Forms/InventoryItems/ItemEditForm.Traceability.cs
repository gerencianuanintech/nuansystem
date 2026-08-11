using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private bool updatingExpiredLotControls;

    private void ConfigureTraceabilityTab()
    {
        tglGeneralPerishable.EditValueChanged += GeneralTraceabilitySettingChanged;
        tglGeneralExpirationManaged.EditValueChanged += GeneralTraceabilitySettingChanged;
        tglAllowExpiredBatchSale.EditValueChanged += AllowExpiredBatchSaleChanged;
        tglBlockExpiredBatch.EditValueChanged += BlockExpiredBatchChanged;

        tglBlockExpiredBatch.IsOn = !tglAllowExpiredBatchSale.IsOn;
        UpdateTraceabilityInheritedSummary();
    }

    private void GeneralTraceabilitySettingChanged(object? sender, EventArgs e)
    {
        UpdateTraceabilityInheritedSummary();
    }

    private void AllowExpiredBatchSaleChanged(object? sender, EventArgs e)
    {
        if (updatingExpiredLotControls)
        {
            return;
        }

        updatingExpiredLotControls = true;
        tglBlockExpiredBatch.IsOn = !tglAllowExpiredBatchSale.IsOn;
        updatingExpiredLotControls = false;
    }

    private void BlockExpiredBatchChanged(object? sender, EventArgs e)
    {
        if (updatingExpiredLotControls)
        {
            return;
        }

        updatingExpiredLotControls = true;
        tglAllowExpiredBatchSale.IsOn = !tglBlockExpiredBatch.IsOn;
        updatingExpiredLotControls = false;
    }

    private void UpdateTraceabilityInheritedSummary()
    {
        if (lblInheritedBatchStatus is null)
        {
            return;
        }

        lblInheritedBatchStatus.Text = BuildInheritedStatus("Control por lote", IsBatchManaged);
        lblInheritedSerialStatus.Text = BuildInheritedStatus("Control por serie", IsSerialManaged);
        lblInheritedPerishableStatus.Text = BuildInheritedStatus("Perecible", tglGeneralPerishable.IsOn);
        lblInheritedExpirationStatus.Text = BuildInheritedStatus("Maneja vencimiento", tglGeneralExpirationManaged.IsOn);

        lblLotTransferRule.ForeColor = IsBatchManaged ? BrandResources.Text : BrandResources.MutedText;
        lblSerialDispatchRule.ForeColor = IsSerialManaged ? BrandResources.Text : BrandResources.MutedText;
    }

    private static string BuildInheritedStatus(string caption, bool enabled)
    {
        var status = enabled ? "✓ Sí" : "× No";
        var statusColor = enabled ? "#00A884" : "#94A3B8";
        return $"<color=#1F2A44>{caption}</color>   <color={statusColor}>{status}</color>";
    }
}
