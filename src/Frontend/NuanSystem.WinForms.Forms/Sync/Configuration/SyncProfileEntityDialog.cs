using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncProfileEntityDialog : XtraForm
{
    private readonly List<SyncEntityCatalogItem> entityCatalog;
    private readonly IReadOnlyCollection<string> executionModes;
    private readonly bool canCreateEntity;
    private bool loading;

    public SyncProfileEntityDialog()
        : this(Array.Empty<SyncEntityCatalogItem>(), new[] { "Full", "Incremental", "Manual" })
    {
    }

    public SyncProfileEntityDialog(
        IReadOnlyCollection<SyncEntityCatalogItem> entityCatalog,
        IReadOnlyCollection<string> executionModes,
        SyncProfileEntityEditorRow? currentEntity = null,
        int? suggestedExecutionOrder = null,
        bool canCreateEntity = false)
    {
        this.entityCatalog = entityCatalog.ToList();
        this.executionModes = executionModes;
        this.canCreateEntity = canCreateEntity && currentEntity is null;
        InitializeComponent();
        AppTypography.ApplyToForm(this);
        ConfigureEntityLookup();
        ConfigureExecutionModes();
        WireEvents();

        if (currentEntity is null)
        {
            sedExecutionOrder.EditValue = suggestedExecutionOrder ?? NextExecutionOrder();
            sedBatchSize.EditValue = null;
            return;
        }

        Text = "Editar entidad del perfil";
        LoadEntity(currentEntity);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SyncProfileEntityDialogResult? Result { get; private set; }

    public event Func<SyncProfileEntityDialog, Task<SyncEntityCatalogItem?>>? CreateEntityRequested;

    private void ConfigureEntityLookup()
    {
        lueEntity.RefreshButtons();
        lueEntity.Properties.DataSource = entityCatalog;
        lueEntity.Properties.DisplayMember = nameof(SyncEntityCatalogItem.Code);
        lueEntity.Properties.ValueMember = nameof(SyncEntityCatalogItem.Code);
        lueEntity.Properties.NullText = string.Empty;
        lueEntity.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueEntity.Properties.SearchMode = SearchMode.AutoSearch;
        lueEntity.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lueEntity.Properties.Columns.Clear();
        lueEntity.Properties.Columns.Add(new LookUpColumnInfo(nameof(SyncEntityCatalogItem.Code), "Código", 180));
        lueEntity.Properties.Columns.Add(new LookUpColumnInfo(nameof(SyncEntityCatalogItem.Name), "Entidad", 220));
        lueEntity.Properties.Columns.Add(new LookUpColumnInfo(nameof(SyncEntityCatalogItem.IsOperative), "Operativa", 80));
        lueEntity.Properties.Columns.Add(new LookUpColumnInfo(nameof(SyncEntityCatalogItem.Description), "Descripción", 280));
        lueEntity.CreateButtonEnabled = canCreateEntity;
        lueEntity.ClearButtonEnabled = true;
        lueEntity.RefreshButtons();
    }

    private void ConfigureExecutionModes()
    {
        cboSyncMode.Properties.Items.Clear();
        cboSyncMode.Properties.Items.AddRange(executionModes.Cast<object>().ToArray());
        cboSyncMode.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
    }

    private void WireEvents()
    {
        lueEntity.EditValueChanged += (_, _) => FillCatalogDefaults();
        lueEntity.ClearButtonClick += (_, _) => ClearEntityFields();
        lueEntity.CreateButtonClick += EntityLookupCreateButtonClick;
        cboSyncMode.SelectedIndexChanged += (_, _) => EnforceSelectedEntityCapabilities();
        btnSave.Click += BtnSave_Click;
    }

    private async void EntityLookupCreateButtonClick(object? sender, EventArgs e)
    {
        if (!canCreateEntity || CreateEntityRequested is null)
        {
            return;
        }

        lueEntity.CreateButtonEnabled = false;
        try
        {
            var created = await CreateEntityRequested(this);
            if (created is null)
            {
                return;
            }

            entityCatalog.RemoveAll(item => string.Equals(item.Code, created.Code, StringComparison.OrdinalIgnoreCase));
            entityCatalog.Add(created);
            lueEntity.Properties.DataSource = null;
            lueEntity.Properties.DataSource = entityCatalog
                .OrderBy(item => item.DefaultExecutionOrder)
                .ThenBy(item => item.Code)
                .ToList();
            lueEntity.EditValue = created.Code;
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, Text, exception);
        }
        finally
        {
            if (!IsDisposed && !Disposing)
            {
                lueEntity.CreateButtonEnabled = canCreateEntity;
            }
        }
    }

    private void FillCatalogDefaults()
    {
        if (loading || SelectedCatalogItem() is not { } item)
        {
            return;
        }

        txtEntityName.Text = item.Name;
        txtDependencies.Text = DependencyText(item);
        if (sedExecutionOrder.Value <= 0)
        {
            sedExecutionOrder.EditValue = item.DefaultExecutionOrder;
        }
        cboSyncMode.EditValue = item.SupportsIncremental ? "Incremental" : "Full";
        txtKeyField.Text = item.DefaultKeyField ?? string.Empty;
        txtModifiedAtField.Text = item.DefaultModifiedAtField ?? string.Empty;
        txtVersionField.Text = string.Empty;
        txtActiveField.Text = string.Empty;
        swAllowInsert.EditValue = item.SupportsInsert;
        swAllowUpdate.EditValue = item.SupportsUpdate;
        swAllowDeactivate.EditValue = item.SupportsDeactivate;
        swContinueOnError.EditValue = false;
        swIsActive.EditValue = true;
        EnforceSelectedEntityCapabilities();
    }

    private void LoadEntity(SyncProfileEntityEditorRow entity)
    {
        loading = true;
        try
        {
            lueEntity.EditValue = entity.EntityCode;
            lueEntity.Properties.ReadOnly = true;
            lueEntity.ClearButtonEnabled = false;
            txtEntityName.Text = entity.EntityName;
            txtDependencies.Text = SelectedCatalogItem() is { } catalogItem
                ? DependencyText(catalogItem)
                : "No registradas";
            sedExecutionOrder.EditValue = entity.ExecutionOrder;
            cboSyncMode.EditValue = entity.SyncMode;
            txtKeyField.Text = entity.KeyField ?? string.Empty;
            txtModifiedAtField.Text = entity.ModifiedAtField ?? string.Empty;
            txtVersionField.Text = entity.VersionField ?? string.Empty;
            txtActiveField.Text = entity.ActiveField ?? string.Empty;
            swAllowInsert.EditValue = entity.AllowInsert;
            swAllowUpdate.EditValue = entity.AllowUpdate;
            swAllowDeactivate.EditValue = entity.AllowDeactivate;
            swContinueOnError.EditValue = entity.ContinueOnError;
            sedBatchSize.EditValue = entity.BatchSize;
            swIsActive.EditValue = entity.IsActive;
            EnforceSelectedEntityCapabilities();
        }
        finally
        {
            loading = false;
        }
    }

    private void ClearEntityFields()
    {
        txtEntityName.Text = string.Empty;
        txtDependencies.Text = string.Empty;
        txtKeyField.Text = string.Empty;
        txtModifiedAtField.Text = string.Empty;
        txtVersionField.Text = string.Empty;
        txtActiveField.Text = string.Empty;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (SelectedCatalogItem() is not { } entity)
        {
            DialogResult = DialogResult.None;
            XtraMessageBox.Show(this, "Seleccione una entidad.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (sedExecutionOrder.Value <= 0)
        {
            DialogResult = DialogResult.None;
            XtraMessageBox.Show(this, "El orden de ejecución debe ser mayor que cero.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(cboSyncMode.Text))
        {
            DialogResult = DialogResult.None;
            XtraMessageBox.Show(this, "Seleccione el modo de sincronización.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtEntityName.Text) || txtEntityName.Text.Trim().Length > 120)
        {
            DialogResult = DialogResult.None;
            XtraMessageBox.Show(this, "El nombre de la entidad es obligatorio y admite hasta 120 caracteres.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.Equals(cboSyncMode.Text, "Incremental", StringComparison.OrdinalIgnoreCase)
            && !entity.SupportsIncremental)
        {
            DialogResult = DialogResult.None;
            XtraMessageBox.Show(this, "La entidad seleccionada no soporta sincronización incremental.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!ValidateTechnicalField(txtKeyField.Text)
            || !ValidateTechnicalField(txtModifiedAtField.Text)
            || !ValidateTechnicalField(txtVersionField.Text)
            || !ValidateTechnicalField(txtActiveField.Text))
        {
            DialogResult = DialogResult.None;
            XtraMessageBox.Show(this,
                "Los campos técnicos admiten hasta 100 caracteres y solo letras, números o guion bajo.",
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (sedBatchSize.EditValue is not null
            && (sedBatchSize.Value < 1 || sedBatchSize.Value > 10000))
        {
            DialogResult = DialogResult.None;
            XtraMessageBox.Show(this, "El batch específico debe estar entre 1 y 10000.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new SyncProfileEntityDialogResult(
            entity.Code,
            txtEntityName.Text.Trim(),
            Convert.ToInt32(sedExecutionOrder.Value),
            cboSyncMode.Text,
            NullIfWhiteSpace(txtKeyField.Text),
            NullIfWhiteSpace(txtModifiedAtField.Text),
            NullIfWhiteSpace(txtVersionField.Text),
            NullIfWhiteSpace(txtActiveField.Text),
            Convert.ToBoolean(swAllowInsert.EditValue),
            Convert.ToBoolean(swAllowUpdate.EditValue),
            Convert.ToBoolean(swAllowDeactivate.EditValue),
            Convert.ToBoolean(swContinueOnError.EditValue),
            sedBatchSize.EditValue is null ? null : Convert.ToInt32(sedBatchSize.Value),
            Convert.ToBoolean(swIsActive.EditValue));
    }

    private SyncEntityCatalogItem? SelectedCatalogItem()
    {
        var code = Convert.ToString(lueEntity.EditValue);
        return string.IsNullOrWhiteSpace(code)
            ? null
            : entityCatalog.FirstOrDefault(item => string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase));
    }

    private int NextExecutionOrder()
    {
        return entityCatalog.Count == 0
            ? 1
            : Math.Max(1, entityCatalog.Min(entity => entity.DefaultExecutionOrder));
    }

    private void EnforceSelectedEntityCapabilities()
    {
        if (SelectedCatalogItem() is not { } item)
        {
            return;
        }

        ApplyCapability(swAllowInsert, item.SupportsInsert);
        ApplyCapability(swAllowUpdate, item.SupportsUpdate);
        ApplyCapability(swAllowDeactivate, item.SupportsDeactivate);

        if (!item.SupportsIncremental
            && string.Equals(cboSyncMode.Text, "Incremental", StringComparison.OrdinalIgnoreCase))
        {
            cboSyncMode.EditValue = executionModes.FirstOrDefault(mode =>
                string.Equals(mode, "Full", StringComparison.OrdinalIgnoreCase)) ?? executionModes.FirstOrDefault();
        }
    }

    private static void ApplyCapability(ToggleSwitch control, bool isSupported)
    {
        control.Enabled = isSupported;
        if (!isSupported)
        {
            control.EditValue = false;
        }
    }

    private static bool ValidateTechnicalField(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= 100
               && trimmed.All(character => char.IsLetterOrDigit(character) || character == '_');
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string DependencyText(SyncEntityCatalogItem item)
    {
        return item.Dependencies.Count == 0
            ? "Sin dependencias"
            : string.Join(" → ", item.Dependencies.Append(item.Code));
    }
}

public sealed record SyncProfileEntityDialogResult(
    string EntityCode,
    string EntityName,
    int ExecutionOrder,
    string SyncMode,
    string? KeyField,
    string? ModifiedAtField,
    string? VersionField,
    string? ActiveField,
    bool AllowInsert,
    bool AllowUpdate,
    bool AllowDeactivate,
    bool ContinueOnError,
    int? BatchSize,
    bool IsActive);
