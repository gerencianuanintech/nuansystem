using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.ViewModels.Sap;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSupplierMappingForm : BaseEditForm
{
    private readonly SapSupplierMappingViewModel? viewModel;

    public SapSupplierMappingForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SapSupplierMappingForm(SapSupplierMappingViewModel viewModel) : this()
    {
        this.viewModel = viewModel;
        Shown += async (_, _) => await LoadMappingsAsync();
    }

    protected override bool ValidateForm()
    {
        var valid = true;
        foreach (var row in viewModel?.Rows ?? [])
        {
            if (string.IsNullOrWhiteSpace(row.SapField) && !string.Equals(row.Direction, "Generated", StringComparison.OrdinalIgnoreCase))
            {
                valid = false;
                break;
            }

            if (string.IsNullOrWhiteSpace(row.NuanField) || string.IsNullOrWhiteSpace(row.TransformationKey))
            {
                valid = false;
                break;
            }
        }

        if (!valid)
        {
            ShowWarning("Cada mapeo debe tener campo NuanSystem y transformación. Revise los campos resaltados.");
        }

        return valid;
    }

    protected override async Task<bool> PersistAsync()
    {
        if (viewModel is null) return false;
        entitiesView.CloseEditor();
        entitiesView.UpdateCurrentRow();
        await viewModel.SaveAsync();
        ShowSuccess("El mapeo de proveedores SAP se guardó correctamente.");
        return true;
    }

    private async Task LoadMappingsAsync()
    {
        if (viewModel is null) return;
        await viewModel.LoadAsync();
        entitiesGrid.DataSource = viewModel.Rows;
        statusLabel.Text = $"{viewModel.Rows.Count} campos configurados";
    }
}
