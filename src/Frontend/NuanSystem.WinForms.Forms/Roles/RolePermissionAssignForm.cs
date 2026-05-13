using System.ComponentModel;
using NuanSystem.WinForms.Services.Roles.Models;

namespace NuanSystem.WinForms.Forms.Roles;

public sealed class RolePermissionAssignForm : Form
{
    private readonly ComboBox permissionComboBox = new();

    public RolePermissionAssignForm(RoleAdminItem role, IReadOnlyCollection<PermissionItem> permissions)
    {
        Common.FormStyler.ApplyBase(this);
        Text = $"Asignar permiso - {role.Code}";
        ClientSize = new Size(560, 180);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var label = new Label { Text = "Permiso", AutoSize = true, Location = new Point(24, 28) };
        permissionComboBox.Location = new Point(24, 54);
        permissionComboBox.Width = 500;
        permissionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        permissionComboBox.DisplayMember = nameof(PermissionDisplay.Text);
        permissionComboBox.ValueMember = nameof(PermissionDisplay.Id);
        permissionComboBox.DataSource = permissions
            .Select(permission => new PermissionDisplay(permission.Id, $"{permission.ModuleCode} | {permission.Code} - {permission.Name}"))
            .ToList();

        var okButton = new Button { Text = "Asignar", Width = 100, Height = 32, Location = new Point(318, 106) };
        var cancelButton = new Button { Text = "Cancelar", Width = 100, Height = 32, Location = new Point(424, 106), DialogResult = DialogResult.Cancel };
        okButton.Click += (_, _) =>
        {
            if (permissionComboBox.SelectedItem is not PermissionDisplay permission)
            {
                MessageBox.Show(this, "Seleccione un permiso.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PermissionId = permission.Id;
            DialogResult = DialogResult.OK;
            Close();
        };

        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.AddRange([label, permissionComboBox, okButton, cancelButton]);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int PermissionId { get; private set; }

    private sealed record PermissionDisplay(int Id, string Text);
}

