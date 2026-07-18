using System.Text.RegularExpressions;
using DevExpress.Utils;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.ViewModels.Sync.EntityDefinitions;

namespace NuanSystem.WinForms.Forms.Sync.EntityDefinitions;

public sealed partial class SyncEntityEditForm : BaseEditForm
{
    private SyncEntityDefinitionEditViewModel? viewModel;

    public SyncEntityEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public SyncEntityEditForm(SyncEntityDefinitionEditViewModel viewModel)
        : this()
    {
        this.viewModel = viewModel;
        LoadState();
    }

    private SyncEntityDefinitionEditViewModel ViewModel =>
        viewModel ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo de la entidad.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre de la entidad.");

        if (!string.IsNullOrWhiteSpace(txtCode.Text)
            && !Regex.IsMatch(txtCode.Text.Trim(), "^[A-Za-z][A-Za-z0-9._-]*$", RegexOptions.CultureInvariant))
        {
            Validator.SetError(txtCode, "El codigo debe iniciar con una letra y solo puede contener letras, numeros, punto, guion o guion bajo.");
            isValid = false;
        }

        isValid &= ValidateTechnicalField(txtKeyField, "El campo clave solo puede contener letras, numeros y guion bajo.");
        isValid &= ValidateTechnicalField(txtModifiedAtField, "El campo de modificacion solo puede contener letras, numeros y guion bajo.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        grdDependencies.InnerGridView.CloseEditor();
        grdDependencies.InnerGridView.UpdateCurrentRow();

        var state = ViewModel.State;
        state.Code = txtCode.Text.Trim();
        state.Name = txtName.Text.Trim();
        state.Description = Normalize(memDescription.Text);
        state.DefaultExecutionOrder = Convert.ToInt32(spnExecutionOrder.Value);
        state.DefaultKeyField = Normalize(txtKeyField.Text);
        state.DefaultModifiedAtField = Normalize(txtModifiedAtField.Text);
        state.SupportsIncremental = chkIncremental.Checked;
        state.SupportsInsert = chkInsert.Checked;
        state.SupportsUpdate = chkUpdate.Checked;
        state.SupportsDeactivate = chkDeactivate.Checked;
        state.IsActive = chkActive.Checked;
    }

    protected override async Task<bool> PersistAsync()
    {
        await ViewModel.SaveAsync();
        return true;
    }

    private void ConfigureForm()
    {
        FormStyler.ApplyBase(this);
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        ConfigureDependencyGrid();
    }

    private void ConfigureDependencyGrid()
    {
        grdDependencies.ConfigureColumns(
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncEntityDefinitionDependencyOption.IsSelected),
                Caption = "Seleccionar",
                VisibleIndex = 0,
                Width = 85,
                Format = NuanGridColumnFormat.Boolean,
                Alignment = HorzAlignment.Center,
                AllowFilter = false,
                AllowSort = false
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncEntityDefinitionDependencyOption.Code),
                Caption = "Codigo",
                VisibleIndex = 1,
                Width = 170
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncEntityDefinitionDependencyOption.Name),
                Caption = "Entidad requerida",
                VisibleIndex = 2,
                Width = 330
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncEntityDefinitionDependencyOption.IsAvailable),
                Caption = "Disponible",
                VisibleIndex = 3,
                Width = 85,
                Format = NuanGridColumnFormat.Boolean,
                Alignment = HorzAlignment.Center
            });
    }

    private void LoadState()
    {
        var state = ViewModel.State;
        Text = state.Id > 0 ? "Editar entidad de sincronizacion" : "Nueva entidad de sincronizacion";
        txtCode.Text = state.Code;
        txtCode.Properties.ReadOnly = state.IsCodeReadOnly;
        txtName.Text = state.Name;
        memDescription.Text = state.Description;
        spnExecutionOrder.Value = state.DefaultExecutionOrder;
        txtKeyField.Text = state.DefaultKeyField;
        txtModifiedAtField.Text = state.DefaultModifiedAtField;
        chkIncremental.Checked = state.SupportsIncremental;
        chkInsert.Checked = state.SupportsInsert;
        chkUpdate.Checked = state.SupportsUpdate;
        chkDeactivate.Checked = state.SupportsDeactivate;
        chkActive.Checked = state.IsActive;
        chkSystem.Checked = state.IsSystem;
        chkProducer.Checked = state.HasProducer;
        chkApplier.Checked = state.HasApplier;
        chkOperative.Checked = state.IsOperative;

        grdDependencies.SetData(state.Dependencies);
        ConfigureDependencyEditing();
    }

    private void ConfigureDependencyEditing()
    {
        var gridView = grdDependencies.InnerGridView;
        gridView.OptionsBehavior.Editable = true;
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in gridView.Columns)
        {
            var editable = column.FieldName == nameof(SyncEntityDefinitionDependencyOption.IsSelected);
            column.OptionsColumn.AllowEdit = editable;
            column.OptionsColumn.ReadOnly = !editable;
        }
    }

    private bool ValidateTechnicalField(DevExpress.XtraEditors.TextEdit control, string message)
    {
        var value = control.Text.Trim();
        if (value.Length == 0 || value.All(character => char.IsLetterOrDigit(character) || character == '_'))
        {
            return true;
        }

        Validator.SetError(control, message);
        return false;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
