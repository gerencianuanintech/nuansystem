using DevExpress.XtraEditors;
using System.ComponentModel;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class SupplierContactEditDialog : XtraForm
{
    public SupplierContactEditDialog()
        : this(null)
    {
    }

    internal SupplierContactEditDialog(SupplierContactViewModel? contact)
    {
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
            new TextOption("Sr.", "Sr."),
            new TextOption("Sra.", "Sra."),
            new TextOption("Ing.", "Ing."),
            new TextOption("Lic.", "Lic.")
        };
        lueContactTreatment.Properties.DisplayMember = nameof(TextOption.Name);
        lueContactTreatment.Properties.ValueMember = nameof(TextOption.Code);
        lueContactTreatment.Properties.Columns.Clear();
        lueContactTreatment.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(TextOption.Name), "Tratamiento", 120));
    }

    private void LoadContact()
    {
        lueContactTreatment.EditValue = string.IsNullOrWhiteSpace(Contact.Treatment) ? "Sr." : Contact.Treatment;
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

        return true;
    }

    private void ShowValidation(string message)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private sealed record TextOption(string Code, string Name);
}
