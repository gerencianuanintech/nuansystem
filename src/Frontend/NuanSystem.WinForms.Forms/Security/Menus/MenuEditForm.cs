using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Security.Forms.Models;
using NuanSystem.WinForms.Services.Security.Menus.Models;
using SecurityMenuItem = NuanSystem.WinForms.Services.Security.Menus.Models.MenuItem;

namespace NuanSystem.WinForms.Forms.Security.Menus;

public sealed partial class MenuEditForm : BaseEditForm
{
    public MenuEditForm(
        IReadOnlyCollection<SecurityMenuItem> menus,
        IReadOnlyCollection<FormItem> forms,
        SecurityMenuItem? menu = null,
        bool copyMode = false)
    {
        InitializeComponent();
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();
        LoadParents(menus, menu?.Id);
        LoadForms(forms);

        if (menu is not null)
        {
            Text = copyMode ? "Copiar menu" : "Editar menu";
            parentLookUpEdit.EditValue = menu.ParentId;
            codeTextEdit.Text = copyMode ? string.Empty : menu.Code;
            nameTextEdit.Text = menu.Name;
            descriptionMemoEdit.Text = menu.Description;
            menuTypeComboBoxEdit.SelectedIndex = Math.Max(0, menu.MenuType - 1);
            formKeyLookUpEdit.EditValue = menu.FormKey;
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
    public SaveMenuRequest Request { get; private set; } = new(
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
        Request = new SaveMenuRequest(
            parentLookUpEdit.EditValue is int parentId ? parentId : null,
            codeTextEdit.Text.Trim(),
            nameTextEdit.Text.Trim(),
            string.IsNullOrWhiteSpace(descriptionMemoEdit.Text) ? null : descriptionMemoEdit.Text.Trim(),
            menuTypeComboBoxEdit.SelectedIndex + 1,
            formKeyLookUpEdit.EditValue is string formKey && !string.IsNullOrWhiteSpace(formKey) ? formKey.Trim() : null,
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

    private void LoadForms(IReadOnlyCollection<FormItem> forms)
    {
        var formOptions = forms
            .Where(form => form.IsActive)
            .OrderBy(form => form.Name)
            .Select(form => new FormKeyOption(form.FormKey, form.Code, form.Name, form.Description))
            .ToList();

        formKeyLookUpEdit.Properties.DataSource = formOptions;
        formKeyLookUpEdit.Properties.DisplayMember = nameof(FormKeyOption.DisplayText);
        formKeyLookUpEdit.Properties.ValueMember = nameof(FormKeyOption.FormKey);
        formKeyLookUpEdit.Properties.NullText = "";
        formKeyLookUpEdit.Properties.Columns.Clear();
        formKeyLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(FormKeyOption.FormKey), "FormKey", 140));
        formKeyLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(FormKeyOption.Code), "Codigo", 180));
        formKeyLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo(nameof(FormKeyOption.Name), "Formulario", 220));
    }

    private sealed record ParentMenuOption(int Id, string Code, string Name, string? Description)
    {
        public string DisplayText => $"{Code} - {Name}";
    }

    private sealed record FormKeyOption(string FormKey, string Code, string Name, string? Description)
    {
        public string DisplayText => $"{FormKey} - {Name}";
    }
}
