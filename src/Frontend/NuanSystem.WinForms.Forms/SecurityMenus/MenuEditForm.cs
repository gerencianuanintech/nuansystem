using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.SecurityMenus.Models;

namespace NuanSystem.WinForms.Forms.SecurityMenus;

public sealed partial class MenuEditForm : BaseEditForm
{
    public MenuEditForm(IReadOnlyCollection<SecurityMenuItem> menus, SecurityMenuItem? menu = null, bool copyMode = false)
    {
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();
        LoadParents(menus, menu?.Id);

        if (menu is not null)
        {
            Text = copyMode ? "Copiar menu" : "Editar menu";
            parentLookUpEdit.EditValue = menu.ParentId;
            codeTextEdit.Text = copyMode ? string.Empty : menu.Code;
            nameTextEdit.Text = menu.Name;
            descriptionMemoEdit.Text = menu.Description;
            menuTypeComboBoxEdit.SelectedIndex = Math.Max(0, menu.MenuType - 1);
            formKeyTextEdit.Text = menu.FormKey;
            iconLargeTextEdit.Text = menu.IconLarge;
            iconSmallTextEdit.Text = menu.IconSmall;
            displayOrderSpinEdit.Value = menu.DisplayOrder;
            visibleCheckEdit.Checked = menu.IsVisible;
            activeCheckEdit.Checked = menu.IsActive;
        }
        else
        {
            menuTypeComboBoxEdit.SelectedIndex = 0;
            visibleCheckEdit.Checked = true;
            activeCheckEdit.Checked = true;
        }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveSecurityMenuRequest Request { get; private set; } = new(
        null,
        string.Empty,
        string.Empty,
        null,
        1,
        null,
        null,
        null,
        0,
        true,
        true);

    protected override bool ValidateForm()
    {
        return Validator.RequireText(codeTextEdit, "Codigo es requerido.")
            & Validator.RequireText(nameTextEdit, "Nombre es requerido.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveSecurityMenuRequest(
            parentLookUpEdit.EditValue is int parentId ? parentId : null,
            codeTextEdit.Text.Trim(),
            nameTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(descriptionMemoEdit.Text) ? null : descriptionMemoEdit.Text.Trim(),
            menuTypeComboBoxEdit.SelectedIndex + 1,
            string.IsNullOrWhiteSpace(formKeyTextEdit.Text) ? null : formKeyTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(iconLargeTextEdit.Text) ? null : iconLargeTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(iconSmallTextEdit.Text) ? null : iconSmallTextEdit.Text.Trim(),
            Convert.ToInt32(displayOrderSpinEdit.Value),
            visibleCheckEdit.Checked,
            activeCheckEdit.Checked);
    }

    private void LoadParents(IReadOnlyCollection<SecurityMenuItem> menus, int? currentId)
    {
        var parentOptions = menus
            .Where(menu => !currentId.HasValue || menu.Id != currentId.Value)
            .OrderBy(menu => menu.Name)
            .Select(menu => new ParentMenuOption(menu.Id, menu.Code, menu.Name, menu.Description))
            .ToList();

        parentLookUpEdit.Properties.DataSource = parentOptions;
        parentLookUpEdit.Properties.DisplayMember = nameof(ParentMenuOption.DisplayText);
        parentLookUpEdit.Properties.ValueMember = nameof(ParentMenuOption.Id);
        parentLookUpEdit.Properties.NullText = "";
        parentLookUpEdit.Properties.Columns.Clear();
        parentLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(ParentMenuOption.Code), "Codigo", 160));
        parentLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(ParentMenuOption.Name), "Nombre", 180));
        parentLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(ParentMenuOption.Description), "Descripcion", 260));
    }

    private sealed record ParentMenuOption(int Id, string Code, string Name, string? Description)
    {
        public string DisplayText => $"{Code} - {Name}";
    }
}
