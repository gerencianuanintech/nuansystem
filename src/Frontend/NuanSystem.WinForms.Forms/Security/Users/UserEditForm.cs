using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Security.Users.Models;

namespace NuanSystem.WinForms.Forms.Security.Users;

public sealed partial class UserEditForm : BaseEditForm
{
    private readonly bool isEditing;
    private readonly bool canCreateRole;
    private byte[]? profileImage;
    private string? profileImageContentType;
    private string? profileImageFileName;

    public UserEditForm(
        IReadOnlyCollection<RoleItem> roles,
        UserAdminItem? user = null,
        bool copyMode = false,
        bool canCreateRole = false)
    {
        isEditing = user is not null && !copyMode;
        this.canCreateRole = canCreateRole;

        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        ConfigureRoleLookup(roles);
        btnCargarFoto.Click += (_, _) => LoadProfileImage();
        btnQuitarFoto.Click += (_, _) => ClearProfileImage();

        if (user is not null)
        {
            Text = copyMode ? "Copiar usuario" : "Editar usuario";
            txtUsuario.Text = copyMode ? string.Empty : user.UserName;
            txtCorreo.Text = user.Email;
            txtTelefono.Text = user.PhoneNumber;
            txtNombre.Text = user.DisplayName;
            txtNombres.Text = user.FirstName;
            txtApellidos.Text = user.LastName;
            txtClave.Text = string.Empty;
            lueRol.EditValue = user.RoleId;
            chkActivo.Checked = user.IsActive;
            chkBloqueado.Checked = user.IsLocked;
            chkPuedeWeb.Checked = user.CanUseWeb;
            chkPuedeMovil.Checked = user.CanUseMobile;
            chkCorreoConfirmado.Checked = user.EmailConfirmed;
            chkTelefonoConfirmado.Checked = user.PhoneNumberConfirmed;
            chkCambiarClave.Checked = user.MustChangePassword;
            chkDobleFactor.Checked = user.TwoFactorEnabled;
            deBloqueo.EditValue = user.LockoutEndAt;
            profileImage = user.ProfileImage;
            profileImageContentType = user.ProfileImageContentType;
            profileImageFileName = user.ProfileImageFileName;
            SetProfileImagePreview(profileImage);
        }
        else
        {
            chkActivo.Checked = true;
            chkPuedeWeb.Checked = true;
            chkPuedeMovil.Checked = true;
        }
    }

    public event Func<UserEditForm, Task<RoleItem?>>? CreateRoleRequested;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CreateUserRequest Request { get; private set; } = new(
        string.Empty,
        null,
        null,
        false,
        false,
        null,
        null,
        string.Empty,
        null,
        null,
        true,
        false,
        true,
        true,
        false,
        null,
        false,
        null,
        null,
        null,
        null);

    public void RefreshRoleLookup(IReadOnlyCollection<RoleItem> roles, int? selectedRoleId = null)
    {
        ConfigureRoleLookup(roles);

        if (selectedRoleId.HasValue)
        {
            lueRol.EditValue = selectedRoleId.Value;
        }
    }

    protected override bool ValidateForm()
    {
        var isValid = Validator.RequireText(txtUsuario, "Usuario es requerido.")
            & Validator.RequireText(txtNombre, "Nombre es requerido.")
            & Validator.EmailIfPresent(txtCorreo, "Correo no tiene un formato valido.");

        if (!isEditing)
        {
            isValid &= Validator.RequireText(txtClave, "Clave es requerida.");
        }

        if (!string.IsNullOrWhiteSpace(txtClave.Text) && txtClave.Text.Length < 8)
        {
            Validator.SetError(txtClave, "Clave debe tener al menos 8 caracteres.");
            isValid = false;
        }

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new CreateUserRequest(
            txtUsuario.Text.Trim(),
            string.IsNullOrWhiteSpace(txtCorreo.Text) ? null : txtCorreo.Text.Trim(),
            string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
            chkCorreoConfirmado.Checked,
            chkTelefonoConfirmado.Checked,
            string.IsNullOrWhiteSpace(txtNombres.Text) ? null : txtNombres.Text.Trim(),
            string.IsNullOrWhiteSpace(txtApellidos.Text) ? null : txtApellidos.Text.Trim(),
            txtNombre.Text.Trim(),
            string.IsNullOrWhiteSpace(txtClave.Text) ? null : txtClave.Text,
            lueRol.EditValue is int roleId ? roleId : null,
            chkActivo.Checked,
            chkBloqueado.Checked,
            chkPuedeWeb.Checked,
            chkPuedeMovil.Checked,
            chkCambiarClave.Checked,
            deBloqueo.EditValue is DateTime lockoutEndAt ? lockoutEndAt : null,
            chkDobleFactor.Checked,
            null,
            profileImage,
            profileImageContentType,
            profileImageFileName);
    }

    private void ConfigureRoleLookup(IReadOnlyCollection<RoleItem> roles)
    {
        lueRol.RefreshButtons();
        lueRol.Properties.DataSource = roles.ToList();
        lueRol.Properties.DisplayMember = nameof(RoleItem.Name);
        lueRol.Properties.ValueMember = nameof(RoleItem.Id);
        lueRol.Properties.NullText = string.Empty;
        lueRol.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueRol.Properties.SearchMode = SearchMode.AutoSearch;
        lueRol.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lueRol.Properties.Columns.Clear();
        lueRol.Properties.Columns.Add(new LookUpColumnInfo(nameof(RoleItem.Code), "Codigo", 90));
        lueRol.Properties.Columns.Add(new LookUpColumnInfo(nameof(RoleItem.Name), "Nombre", 160));
        lueRol.Properties.Columns.Add(new LookUpColumnInfo(nameof(RoleItem.Description), "Descripcion", 260));
        lueRol.CreateButtonEnabled = canCreateRole;
        lueRol.RefreshButtons();
        lueRol.CreateButtonClick -= RoleLookupCreateButtonClick;
        lueRol.CreateButtonClick += RoleLookupCreateButtonClick;
    }

    private async void RoleLookupCreateButtonClick(object? sender, EventArgs e)
    {
        if (!canCreateRole || CreateRoleRequested is null)
        {
            return;
        }

        var created = await CreateRoleRequested(this);
        if (created is not null)
        {
            lueRol.EditValue = created.Id;
        }
    }

    private void LoadProfileImage()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Imagenes|*.jpg;*.jpeg;*.png;*.bmp|Todos los archivos|*.*",
            Title = "Seleccionar foto de usuario"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var file = new FileInfo(dialog.FileName);
        if (file.Length > 2 * 1024 * 1024)
        {
            XtraMessageBox.Show(this, "La foto no debe superar 2 MB.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        profileImage = File.ReadAllBytes(dialog.FileName);
        profileImageContentType = ResolveContentType(file.Extension);
        profileImageFileName = file.Name;
        SetProfileImagePreview(profileImage);
    }

    private void ClearProfileImage()
    {
        profileImage = null;
        profileImageContentType = null;
        profileImageFileName = null;
        picFoto.Image = null;
    }

    private void SetProfileImagePreview(byte[]? imageBytes)
    {
        picFoto.Image = null;
        if (imageBytes is null || imageBytes.Length == 0)
        {
            return;
        }

        using var stream = new MemoryStream(imageBytes);
        using var image = Image.FromStream(stream);
        picFoto.Image = new Bitmap(image);
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
}
