using System.Data;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private void ConfigureInventoryTab()
    {
        ConfigurePresentationAction(
            btnAddWarehouseStock,
            "agregar_16.svg",
            BrandResources.Primary);
        ConfigurePresentationAction(
            btnUpdateWarehouseStock,
            "editar_16.svg",
            BrandResources.CustomerAccent);
        ConfigurePresentationAction(
            btnRemoveWarehouseStock,
            "quitar_16.svg",
            BrandResources.ErrorText);
        ConfigurePresentationAction(
            btnSetMainWarehouseStock,
            "aprobar_16.svg",
            BrandResources.SuccessText);

        UpdateInventoryWarehouseSummary();
    }

    private void UpdateInventoryWarehouseSummary()
    {
        var rows = warehouseStockTable.Rows
            .Cast<DataRow>()
            .Where(row => row.RowState != DataRowState.Deleted)
            .ToArray();
        var availableStock = rows.Sum(row => ToDecimal(row["Disponible"]));
        var mainRow = rows.FirstOrDefault(row => ToBool(row["Principal"]));
        var inventoryUnitId = GetLookupInt(lueInventoryUnit) ?? GetLookupInt(lueBaseUnit);
        var inventoryUnit = ResolveUnitCode(inventoryUnitId) ?? "UND";
        var principalWarehouse = mainRow is null
            ? "-"
            : $"{Convert.ToString(mainRow["Bodega"])} - {Convert.ToString(mainRow["NombreBodega"])}";
        var warehouseCount = rows.Length == 1 ? "1 bodega" : $"{rows.Length} bodegas";

        lblWarehouseSummary.Text =
            $"{warehouseCount}   •   Disponible: {availableStock:N2} {inventoryUnit}   •   Principal: {principalWarehouse}";
    }
}
