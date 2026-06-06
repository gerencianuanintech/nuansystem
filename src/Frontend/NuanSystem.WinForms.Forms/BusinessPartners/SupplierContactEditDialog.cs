using DevExpress.XtraEditors;
using System.ComponentModel;
using NuanSystem.WinForms.Services.BusinessPartners.Models;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class SupplierContactEditDialog : XtraForm
{
    private readonly IReadOnlyCollection<BusinessPartnerLookupOption> contactTypes;
    private readonly IReadOnlyCollection<BusinessPartnerLookupOption> contactChannels;

    public SupplierContactEditDialog()
        : this(null, Array.Empty<BusinessPartnerLookupOption>(), Array.Empty<BusinessPartnerLookupOption>())
    {
    }

    internal SupplierContactEditDialog(
        IReadOnlyCollection<BusinessPartnerLookupOption> contactTypes,
        IReadOnlyCollection<BusinessPartnerLookupOption> contactChannels)
        : this(null, contactTypes, contactChannels)
    {
    }

    internal SupplierContactEditDialog(SupplierContactViewModel? contact)
        : this(contact, Array.Empty<BusinessPartnerLookupOption>(), Array.Empty<BusinessPartnerLookupOption>())
    {
    }

    internal SupplierContactEditDialog(
        SupplierContactViewModel? contact,
        IReadOnlyCollection<BusinessPartnerLookupOption> contactTypes,
        IReadOnlyCollection<BusinessPartnerLookupOption> contactChannels)
    {
        this.contactTypes = contactTypes ?? Array.Empty<BusinessPartnerLookupOption>();
        this.contactChannels = contactChannels ?? Array.Empty<BusinessPartnerLookupOption>();

        InitializeComponent();
        BindLookups();

        Contact = contact?.Clone() ?? new SupplierContactViewModel
        {
            Treatment = "Sr.",
            IsActive = true
        };

        Text = contact is null ? "Nuevo Contacto" : "Editar Contacto";
        LoadContact();
        WireEvents();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal SupplierContactViewModel Contact { get; private set; }

    private void WireEvents()
    {
        btnSaveContact.Click += (_, _) => SaveContact();
        btnCancelContact.Click += (_, _) => Close();
    }

    private void BindLookups()
    {
        lueContactTreatment.Properties.DataSource = new[]
        {
            new SupplierTextOptionViewModel("Sr.", "Sr."),
            new SupplierTextOptionViewModel("Sra.", "Sra."),
            new SupplierTextOptionViewModel("Ing.", "Ing."),
            new SupplierTextOptionViewModel("Lic.", "Lic.")
        };
        lueContactTreatment.Properties.DisplayMember = nameof(SupplierTextOptionViewModel.Name);
        lueContactTreatment.Properties.ValueMember = nameof(SupplierTextOptionViewModel.Code);
        lueContactTreatment.Properties.Columns.Clear();
        lueContactTreatment.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(SupplierTextOptionViewModel.Name), "Tratamiento", 120));

        BindLookupOptions(lueContactType, contactTypes);
        BindLookupOptions(lueContactChannel, contactChannels);
    }

    private void LoadContact()
    {
        lueContactTreatment.EditValue = string.IsNullOrWhiteSpace(Contact.Treatment) ? "Sr." : Contact.Treatment;
        lueContactType.EditValue = Contact.ContactTypeId;
        lueContactChannel.EditValue = Contact.ContactChannelId;
        txtContactFirstName.Text = Contact.FirstName;
        txtContactLastName.Text = Contact.LastName;
        txtContactPosition.Text = Contact.Position;
        txtContactDepartment.Text = Contact.Department;
        txtContactPhone.Text = Contact.Phone;
        txtContactExtension.Text = Contact.Extension;
        txtContactMobile.Text = Contact.Mobile;
        txtContactEmail.Text = Contact.Email;
        dteContactBirthday.EditValue = Contact.Birthday;
        tglContactPrimary.IsOn = Contact.IsPrimary;
        tglContactActive.IsOn = Contact.IsActive;
        memContactNotes.Text = Contact.Notes;
    }

    private void SaveContact()
    {
        if (!ValidateContact())
        {
            return;
        }

        Contact.Treatment = Convert.ToString(lueContactTreatment.EditValue) ?? string.Empty;
        Contact.ContactTypeId = ToNullableInt(lueContactType.EditValue);
        Contact.ContactChannelId = ToNullableInt(lueContactChannel.EditValue);
        ApplyLookupSelection(Contact);
        Contact.FirstName = txtContactFirstName.Text.Trim();
        Contact.LastName = txtContactLastName.Text.Trim();
        Contact.Position = txtContactPosition.Text.Trim();
        Contact.Department = txtContactDepartment.Text.Trim();
        Contact.Phone = txtContactPhone.Text.Trim();
        Contact.Extension = txtContactExtension.Text.Trim();
        Contact.Mobile = txtContactMobile.Text.Trim();
        Contact.Email = txtContactEmail.Text.Trim();
        Contact.Birthday = dteContactBirthday.EditValue is DateTime birthday ? birthday : null;
        Contact.IsPrimary = tglContactPrimary.IsOn;
        Contact.IsActive = tglContactActive.IsOn;
        Contact.Notes = memContactNotes.Text.Trim();

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateContact()
    {
        if (string.IsNullOrWhiteSpace(txtContactFirstName.Text))
        {
            ShowValidation("Nombres es requerido.");
            txtContactFirstName.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtContactLastName.Text))
        {
            ShowValidation("Apellidos es requerido.");
            txtContactLastName.Focus();
            return false;
        }

        var email = txtContactEmail.Text.Trim();
        if (email.Length > 0 && (!email.Contains('@') || !email.Contains('.')))
        {
            ShowValidation("Ingrese un correo electrónico válido.");
            txtContactEmail.Focus();
            return false;
        }

        if (!IsValidLookupSelection(lueContactType, contactTypes))
        {
            ShowValidation("Seleccione un tipo de contacto válido.");
            lueContactType.Focus();
            return false;
        }

        if (!IsValidLookupSelection(lueContactChannel, contactChannels))
        {
            ShowValidation("Seleccione un canal de contacto válido.");
            lueContactChannel.Focus();
            return false;
        }

        return true;
    }

    private static void BindLookupOptions(
        DevExpress.XtraEditors.LookUpEdit lookup,
        IReadOnlyCollection<BusinessPartnerLookupOption> options)
    {
        lookup.Properties.DataSource = options;
        lookup.Properties.DisplayMember = nameof(BusinessPartnerLookupOption.Name);
        lookup.Properties.ValueMember = nameof(BusinessPartnerLookupOption.Id);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(BusinessPartnerLookupOption.Code), "Código", 70));
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(BusinessPartnerLookupOption.Name), "Nombre", 160));
    }

    private static int? ToNullableInt(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        if (value is string text)
        {
            return string.IsNullOrWhiteSpace(text) ? null : Convert.ToInt32(text);
        }

        return Convert.ToInt32(value);
    }

    private static bool IsValidLookupSelection(
        DevExpress.XtraEditors.LookUpEdit lookup,
        IReadOnlyCollection<BusinessPartnerLookupOption> options)
    {
        var selectedId = ToNullableInt(lookup.EditValue);
        return selectedId is null || options.Any(option => option.Id == selectedId.Value);
    }

    private void ApplyLookupSelection(SupplierContactViewModel contact)
    {
        var contactType = contact.ContactTypeId is null
            ? null
            : contactTypes.FirstOrDefault(option => option.Id == contact.ContactTypeId.Value);
        contact.ContactTypeCode = contactType?.Code ?? string.Empty;
        contact.ContactTypeName = contactType?.Name ?? string.Empty;

        var contactChannel = contact.ContactChannelId is null
            ? null
            : contactChannels.FirstOrDefault(option => option.Id == contact.ContactChannelId.Value);
        contact.ContactChannelCode = contactChannel?.Code ?? string.Empty;
        contact.ContactChannelName = contactChannel?.Name ?? string.Empty;
    }

    private void ShowValidation(string message)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
