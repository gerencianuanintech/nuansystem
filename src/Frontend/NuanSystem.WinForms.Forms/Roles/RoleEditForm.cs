using System.ComponentModel;
using NuanSystem.WinForms.Services.Roles.Models;

namespace NuanSystem.WinForms.Forms.Roles;

public sealed class RoleEditForm : Form
{
    private readonly TextBox codeTextBox = new();
    private readonly TextBox nameTextBox = new();
    private readonly TextBox descriptionTextBox = new();
    private readonly CheckBox activeCheckBox = new();

    public RoleEditForm()
    {
        BuildLayout();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public CreateRoleRequest Request { get; private set; } = new(string.Empty, string.Empty, null, true);

    private void BuildLayout()
    {
        Common.FormStyler.ApplyBase(this);
        Text = "Nuevo rol";
        ClientSize = new Size(540, 300);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            ColumnCount = 2,
            RowCount = 5
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        activeCheckBox.Text = "Activo";
        activeCheckBox.Checked = true;

        AddRow(layout, 0, "Codigo", codeTextBox);
        AddRow(layout, 1, "Nombre", nameTextBox);
        AddRow(layout, 2, "Descripcion", descriptionTextBox);
        layout.Controls.Add(new Label(), 0, 3);
        layout.Controls.Add(activeCheckBox, 1, 3);

        var buttonsPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
        var saveButton = new Button { Text = "Guardar", Width = 100, Height = 32 };
        var cancelButton = new Button { Text = "Cancelar", Width = 100, Height = 32, DialogResult = DialogResult.Cancel };
        saveButton.Click += SaveButton_Click;
        buttonsPanel.Controls.AddRange([saveButton, cancelButton]);
        layout.Controls.Add(buttonsPanel, 1, 4);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
        Controls.Add(layout);
    }

    private static void AddRow(TableLayoutPanel layout, int row, string label, TextBox textBox)
    {
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        textBox.Dock = DockStyle.Fill;
        layout.Controls.Add(textBox, 1, row);
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(codeTextBox.Text) || string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show(this, "Codigo y nombre son requeridos.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Request = new CreateRoleRequest(
            codeTextBox.Text.Trim().ToUpperInvariant(),
            nameTextBox.Text.Trim(),
            string.IsNullOrWhiteSpace(descriptionTextBox.Text) ? null : descriptionTextBox.Text.Trim(),
            activeCheckBox.Checked);

        DialogResult = DialogResult.OK;
        Close();
    }
}

