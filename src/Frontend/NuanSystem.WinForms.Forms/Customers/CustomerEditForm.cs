using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Customers.Models;

namespace NuanSystem.WinForms.Forms.Customers;

public sealed partial class CustomerEditForm : BaseEditForm
{
    public CustomerEditForm(CustomerItem? customer = null, bool copyMode = false)
    {
        CustomerId = customer?.Id;
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();

        if (customer is not null)
        {
            Text = copyMode ? "Copiar cliente" : "Editar cliente";
            txtCodigo.Text = copyMode ? string.Empty : customer.Code;
            txtNombre.Text = customer.Name;
            txtIdentificacion.Text = customer.TaxIdentification;
            txtCorreo.Text = customer.Email;
            txtTelefono.Text = customer.Phone;
            memDireccion.Text = customer.AddressLine;
            chkActivo.Checked = customer.IsActive;
        }
        else
        {
            chkActivo.Checked = true;
        }
    }

    public int? CustomerId { get; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveCustomerRequest Request { get; private set; } = new(string.Empty, string.Empty, null, null, null, null);

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtCodigo, "Codigo es requerido.")
            & Validator.RequireText(txtNombre, "Nombre es requerido.")
            & Validator.EmailIfPresent(txtCorreo, "Correo no tiene un formato valido.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveCustomerRequest(
            txtCodigo.Text.Trim(),
            txtNombre.Text.Trim(),
            NullIfEmpty(txtIdentificacion.Text),
            NullIfEmpty(txtCorreo.Text),
            NullIfEmpty(txtTelefono.Text),
            NullIfEmpty(memDireccion.Text),
            chkActivo.Checked);
    }

    private static string? NullIfEmpty(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
