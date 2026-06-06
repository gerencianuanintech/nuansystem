using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class SupplierAddressEditDialog : XtraForm
{
    public SupplierAddressEditDialog(string code)
        : this(null, code)
    {
    }

    internal SupplierAddressEditDialog(SupplierAddressViewModel address)
        : this(address, address.Code)
    {
    }

    private SupplierAddressEditDialog(SupplierAddressViewModel? address, string code)
    {
        InitializeComponent();
        BindLookups();

        Address = address?.Clone() ?? new SupplierAddressViewModel
        {
            Code = code,
            AddressType = "Entrega",
            AddressName = "Almacén Norte",
            MainStreet = "Av. De las Américas",
            SecondaryStreet = "Los Pinos",
            AddressNumber = "450",
            Reference = "Junto al centro logístico",
            Neighborhood = "Parque Industrial",
            Province = "Lima",
            City = "San Martín de Porres",
            Country = "Perú",
            PostalCode = "15108",
            Latitude = -12.0464m,
            Longitude = -77.0428m,
            IsDefaultDelivery = true,
            IsActive = true,
            Notes = "Dirección principal de entrega para recepción de mercadería."
        };

        Text = address is null ? "Nueva Dirección" : "Editar Dirección";
        LoadAddress();
        WireEvents();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal SupplierAddressViewModel Address { get; private set; }

    private void WireEvents()
    {
        btnSaveAddress.Click += (_, _) => SaveAddress();
        btnCancelAddress.Click += (_, _) => Close();
    }

    private void BindLookups()
    {
        BindLookup(lueAddressType, "Fiscal", "Entrega", "Facturación", "Otro");
        BindLookup(lueAddressCountry, "Perú", "Ecuador", "Colombia");
    }

    private static void BindLookup(DevExpress.XtraEditors.LookUpEdit lookup, params string[] values)
    {
        lookup.Properties.DataSource = values.Select(value => new SupplierTextOptionViewModel(value, value)).ToList();
        lookup.Properties.DisplayMember = nameof(SupplierTextOptionViewModel.Name);
        lookup.Properties.ValueMember = nameof(SupplierTextOptionViewModel.Code);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(SupplierTextOptionViewModel.Name), "Nombre", 180));
    }

    private void LoadAddress()
    {
        lueAddressType.EditValue = Address.AddressType;
        txtAddressName.Text = Address.AddressName;
        txtMainStreet.Text = Address.MainStreet;
        txtSecondaryStreet.Text = Address.SecondaryStreet;
        txtAddressNumber.Text = Address.AddressNumber;
        txtAddressReference.Text = Address.Reference;
        txtNeighborhood.Text = Address.Neighborhood;
        txtAddressProvince.Text = Address.Province;
        txtAddressCity.Text = Address.City;
        lueAddressCountry.EditValue = string.IsNullOrWhiteSpace(Address.Country) ? "Perú" : Address.Country;
        txtPostalCode.Text = Address.PostalCode;
        spnLatitude.EditValue = Address.Latitude;
        spnLongitude.EditValue = Address.Longitude;
        tglDefaultBillingAddress.IsOn = Address.IsDefaultBilling;
        tglDefaultDeliveryAddress.IsOn = Address.IsDefaultDelivery;
        tglAddressActive.IsOn = Address.IsActive;
        memAddressNotes.Text = Address.Notes;
    }

    private void SaveAddress()
    {
        if (!ValidateAddress())
        {
            return;
        }

        Address.AddressType = Convert.ToString(lueAddressType.EditValue) ?? string.Empty;
        Address.AddressName = txtAddressName.Text.Trim();
        Address.MainStreet = txtMainStreet.Text.Trim();
        Address.SecondaryStreet = txtSecondaryStreet.Text.Trim();
        Address.AddressNumber = txtAddressNumber.Text.Trim();
        Address.Reference = txtAddressReference.Text.Trim();
        Address.Neighborhood = txtNeighborhood.Text.Trim();
        Address.Province = txtAddressProvince.Text.Trim();
        Address.City = txtAddressCity.Text.Trim();
        Address.Country = Convert.ToString(lueAddressCountry.EditValue) ?? string.Empty;
        Address.PostalCode = txtPostalCode.Text.Trim();
        Address.Latitude = spnLatitude.EditValue is null ? null : spnLatitude.Value;
        Address.Longitude = spnLongitude.EditValue is null ? null : spnLongitude.Value;
        Address.IsDefaultBilling = tglDefaultBillingAddress.IsOn;
        Address.IsDefaultDelivery = tglDefaultDeliveryAddress.IsOn;
        Address.IsPrimary = Address.IsDefaultBilling || Address.IsDefaultDelivery || Address.IsPrimary;
        Address.IsActive = tglAddressActive.IsOn;
        Address.Notes = memAddressNotes.Text.Trim();

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateAddress()
    {
        if (lueAddressType.EditValue is null)
        {
            return ShowValidation("Tipo de Dirección es requerido.", lueAddressType);
        }

        if (string.IsNullOrWhiteSpace(txtAddressName.Text))
        {
            return ShowValidation("Nombre de la Dirección es requerido.", txtAddressName);
        }

        if (string.IsNullOrWhiteSpace(txtMainStreet.Text))
        {
            return ShowValidation("Calle Principal es requerida.", txtMainStreet);
        }

        if (lueAddressCountry.EditValue is null)
        {
            return ShowValidation("País es requerido.", lueAddressCountry);
        }

        if (string.IsNullOrWhiteSpace(txtAddressProvince.Text))
        {
            return ShowValidation("Provincia es requerida.", txtAddressProvince);
        }

        if (string.IsNullOrWhiteSpace(txtAddressCity.Text))
        {
            return ShowValidation("Ciudad / Distrito es requerida.", txtAddressCity);
        }

        return true;
    }

    private bool ShowValidation(string message, Control control)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
        return false;
    }
}
