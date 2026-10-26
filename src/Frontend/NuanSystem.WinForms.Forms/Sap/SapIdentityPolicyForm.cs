using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.SapSync.IdentityPolicy.Models;
using NuanSystem.WinForms.ViewModels.SapSync;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapIdentityPolicyForm : BaseEditForm
{
    private readonly SapIdentityPolicyViewModel? viewModel;

    public SapIdentityPolicyForm()
    {
        InitializeComponent();
    }

    public SapIdentityPolicyForm(SapIdentityPolicyViewModel viewModel) : this()
    {
        this.viewModel = viewModel;
        Shown += async (_, _) => await LoadPolicyAsync();
    }

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtCustomerNational, "Prefijo cliente nacional es requerido.")
            & Validator.RequireText(txtCustomerForeign, "Prefijo cliente extranjero es requerido.")
            & Validator.RequireText(txtSupplierNational, "Prefijo proveedor nacional es requerido.")
            & Validator.RequireText(txtSupplierForeign, "Prefijo proveedor extranjero es requerido.")
            & Validator.RequireText(txtPassportTypes, "Tipos tributarios de pasaporte son requeridos.")
            & Validator.RequireText(txtPassportIdentification, "Tipo de identificación de pasaporte es requerido.");
    }

    protected override async Task<bool> PersistAsync()
    {
        if (viewModel is null) return false;
        await viewModel.SaveAsync(new SaveSapIdentityPolicyRequest(txtCustomerNational.Text.Trim(), txtCustomerForeign.Text.Trim(), txtSupplierNational.Text.Trim(), txtSupplierForeign.Text.Trim(), txtPassportTypes.Text.Trim(), txtPassportIdentification.Text.Trim()));
        ShowSuccess("La política de identidad SAP se guardó correctamente.");
        return true;
    }

    private async Task LoadPolicyAsync()
    {
        if (viewModel is null) return;
        await viewModel.LoadAsync();
        if (viewModel.Policy is not { } policy) return;
        txtCustomerNational.Text = policy.CustomerNationalPrefix;
        txtCustomerForeign.Text = policy.CustomerForeignPrefix;
        txtSupplierNational.Text = policy.SupplierNationalPrefix;
        txtSupplierForeign.Text = policy.SupplierForeignPrefix;
        txtPassportTypes.Text = policy.PassportTaxpayerTypeCodes;
        txtPassportIdentification.Text = policy.PassportIdentificationTypeCode;
    }
}
