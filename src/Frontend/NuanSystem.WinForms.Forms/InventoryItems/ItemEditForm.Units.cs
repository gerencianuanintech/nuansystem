using System.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemEditForm
{
    private void ConfigureUnitsAndCodesTab()
    {
        ConfigurePresentationAction(
            btnAddItemPresentation,
            "agregar_16.svg",
            BrandResources.Primary);
        ConfigurePresentationAction(
            btnUpdateItemPresentation,
            "editar_16.svg",
            BrandResources.CustomerAccent);
        ConfigurePresentationAction(
            btnRemoveItemPresentation,
            "quitar_16.svg",
            BrandResources.ErrorText);
        ConfigurePresentationAction(
            btnSetMainItemPresentation,
            "aprobar_16.svg",
            BrandResources.SuccessText);

        UpdateUnitsPresentationSummary();
    }

    private static void ConfigurePresentationAction(
        SimpleButton button,
        string iconName,
        Color color)
    {
        button.Appearance.ForeColor = color;
        button.Appearance.Options.UseForeColor = true;
        button.AppearanceHovered.BackColor = BrandResources.PrimarySoft;
        button.AppearanceHovered.ForeColor = color;
        button.AppearanceHovered.Options.UseBackColor = true;
        button.AppearanceHovered.Options.UseForeColor = true;
        button.ButtonStyle = BorderStyles.NoBorder;
        button.ImageOptions.ImageToTextAlignment = ImageAlignToText.LeftCenter;
        button.ImageOptions.SvgImage = OperationButtonIcons.LoadOperationIcon(iconName, color);
        button.ImageOptions.SvgImageSize = new Size(16, 16);
    }

    private void UpdateUnitsPresentationSummary()
    {
        var rows = itemPresentationsTable.Rows
            .Cast<DataRow>()
            .Where(row => row.RowState != DataRowState.Deleted)
            .ToArray();
        var activeCount = rows.Count(row => ToBool(row["Activa"], true));
        var mainRow = rows.FirstOrDefault(row => ToBool(row["Principal"]));
        var mainPresentation = mainRow is null
            ? "-"
            : Convert.ToString(mainRow["Presentacion"]);

        var presentationCountText = rows.Length == 1
            ? "1 presentación"
            : $"{rows.Length} presentaciones";
        var activeCountText = activeCount == 1
            ? "1 activa"
            : $"{activeCount} activas";

        lblPresentationSummary.Text =
            $"{presentationCountText}   •   {activeCountText}   •   Principal: {mainPresentation ?? "-"}";
    }
}
