using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.ConfigurationCompanies.Models;

namespace NuanSystem.WinForms.Forms.ConfigurationCompanies;

public sealed partial class ConfigurationCompanyEditForm : BaseEditForm
{
    private readonly bool isNewRecord;
    private byte[]? logoImage;
    private string? logoImageContentType;
    private string? logoImageFileName;

    public ConfigurationCompanyEditForm(ConfigurationCompanyItem? company = null, bool copyMode = false)
    {
        isNewRecord = company is null || copyMode;

        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();

        cmbMotor.Properties.Items.AddRange(["SQL Server"]);
        cmbSap.Properties.Items.AddRange(["Sin SAP", "Service Layer", "DI API"]);
        btnCargarLogo.Click += (_, _) => LoadLogoImage();
        btnQuitarLogo.Click += (_, _) => ClearLogoImage();

        if (company is not null)
        {
            Text = copyMode ? "Copiar compania" : "Editar compania";
            txtCodigo.Text = copyMode ? string.Empty : company.Code;
            txtNombreComercial.Text = company.CommercialName;
            txtRazonSocial.Text = company.LegalName;
            txtIdentificacion.Text = company.TaxIdentification;
            memDireccion.Text = company.Address;
            txtTelefono.Text = company.Phone;
            txtCorreo.Text = company.Email;
            logoImage = company.LogoImage;
            logoImageContentType = company.LogoImageContentType;
            logoImageFileName = company.LogoImageFileName;
            SetLogoImagePreview(logoImage);
            cmbMotor.SelectedIndex = Math.Max(0, company.DatabaseEngine - 1);
            txtServidor.Text = company.Server;
            sedPuerto.Value = company.Port ?? 0;
            txtBaseDatos.Text = company.DatabaseName;
            txtUsuarioDb.Text = company.DatabaseUser;
            chkActivo.Checked = company.IsActive;
            cmbSap.SelectedIndex = company.SapIntegrationMode;
            sedOrden.Value = company.DisplayOrder;
            chkPredeterminada.Checked = company.IsDefault;
            txtZonaHoraria.Text = company.TimeZoneId;
            txtCultura.Text = company.CultureCode;
            txtMoneda.Text = company.CurrencyCode;
            chkValidarConexion.Checked = false;
        }
        else
        {
            cmbMotor.SelectedIndex = 0;
            cmbSap.SelectedIndex = 0;
            txtServidor.Text = "localhost";
            sedPuerto.Value = 1433;
            txtUsuarioDb.Text = "sa";
            chkActivo.Checked = true;
            chkValidarConexion.Checked = true;
            txtZonaHoraria.Text = "America/Guayaquil";
            txtCultura.Text = "es-EC";
            txtMoneda.Text = "USD";
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveConfigurationCompanyRequest Request { get; private set; } = new(
        string.Empty,
        string.Empty,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        1,
        string.Empty,
        null,
        string.Empty,
        string.Empty,
        null,
        false,
        true,
        0,
        0,
        false,
        "America/Guayaquil",
        "es-EC",
        "USD");

    protected override bool ValidateForm()
    {
        var isValid = Validator.RequireText(txtCodigo, "Codigo es requerido.")
            & Validator.RequireText(txtNombreComercial, "Nombre comercial es requerido.")
            & Validator.RequireText(txtServidor, "Servidor es requerido.")
            & Validator.RequireText(txtBaseDatos, "Base de datos es requerida.")
            & Validator.RequireText(txtUsuarioDb, "Usuario DB es requerido.")
            & Validator.RequireText(txtZonaHoraria, "Zona horaria es requerida.")
            & Validator.RequireText(txtCultura, "Cultura es requerida.")
            & Validator.RequireText(txtMoneda, "Moneda es requerida.");

        if (isNewRecord)
        {
            isValid &= Validator.RequireText(txtClaveDb, "Clave DB es requerida.");
        }

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveConfigurationCompanyRequest(
            txtCodigo.Text.Trim().ToUpperInvariant(),
            txtNombreComercial.Text.Trim(),
            NullIfEmpty(txtRazonSocial.Text),
            NullIfEmpty(txtIdentificacion.Text),
            NullIfEmpty(memDireccion.Text),
            NullIfEmpty(txtTelefono.Text),
            NullIfEmpty(txtCorreo.Text),
            logoImage,
            logoImageContentType,
            logoImageFileName,
            cmbMotor.SelectedIndex + 1,
            txtServidor.Text.Trim(),
            sedPuerto.Value <= 0 ? null : Convert.ToInt32(sedPuerto.Value),
            txtBaseDatos.Text.Trim(),
            txtUsuarioDb.Text.Trim(),
            NullIfEmpty(txtClaveDb.Text),
            chkValidarConexion.Checked,
            chkActivo.Checked,
            cmbSap.SelectedIndex,
            Convert.ToInt32(sedOrden.Value),
            chkPredeterminada.Checked,
            txtZonaHoraria.Text.Trim(),
            txtCultura.Text.Trim(),
            txtMoneda.Text.Trim().ToUpperInvariant());
    }

    private void LoadLogoImage()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Imagenes|*.jpg;*.jpeg;*.png;*.bmp|Todos los archivos|*.*",
            Title = "Seleccionar logo de compania"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var file = new FileInfo(dialog.FileName);
        if (file.Length > 2 * 1024 * 1024)
        {
            XtraMessageBox.Show(this, "El logo no debe superar 2 MB.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var imageBytes = File.ReadAllBytes(dialog.FileName);
        if (!TryCreateImage(imageBytes, out var preview))
        {
            XtraMessageBox.Show(this, "El archivo seleccionado no es una imagen valida.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        logoImage = imageBytes;
        logoImageContentType = ResolveContentType(file.Extension);
        logoImageFileName = file.Name;
        picLogo.Image = preview;
    }

    private void ClearLogoImage()
    {
        logoImage = null;
        logoImageContentType = null;
        logoImageFileName = null;
        picLogo.Image = null;
    }

    private void SetLogoImagePreview(byte[]? imageBytes)
    {
        picLogo.Image = null;
        if (imageBytes is null || imageBytes.Length == 0)
        {
            return;
        }

        try
        {
            picLogo.Image = TryCreateImage(imageBytes, out var preview)
                ? preview
                : null;
        }
        catch (ArgumentException)
        {
            logoImage = null;
            logoImageContentType = null;
            logoImageFileName = null;
            picLogo.Image = null;
        }
    }

    private static string ResolveContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }

    private static bool TryCreateImage(byte[] imageBytes, out Bitmap? image)
    {
        try
        {
            using var stream = new MemoryStream(imageBytes);
            using var source = Image.FromStream(stream);
            image = new Bitmap(source);
            return true;
        }
        catch (ArgumentException)
        {
            image = null;
            return false;
        }
    }

    private static string? NullIfEmpty(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
