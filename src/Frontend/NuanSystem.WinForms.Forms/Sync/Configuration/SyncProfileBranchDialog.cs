using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncProfileBranchDialog : XtraForm
{
    private IReadOnlyCollection<CompanyLookupItem> branchCompanies = Array.Empty<CompanyLookupItem>();
    private DateTime? lastSynchronizationAt;

    public SyncProfileBranchDialog()
        : this(Array.Empty<CompanyLookupItem>(), false)
    {
    }

    public SyncProfileBranchDialog(
        IReadOnlyCollection<CompanyLookupItem> branchCompanies,
        bool canCreateBranchCompany)
    {
        this.branchCompanies = branchCompanies;
        InitializeComponent();
        AppTypography.ApplyToForm(this);
        ConfigureBranchLookup(canCreateBranchCompany);
        WireEvents();
        ClearBranchDetails();
    }

    public SyncProfileBranchDialog(
        IReadOnlyCollection<CompanyLookupItem> branchCompanies,
        bool canCreateBranchCompany,
        SyncProfileBranchDialogResult initialValue)
        : this(branchCompanies, canCreateBranchCompany)
    {
        ArgumentNullException.ThrowIfNull(initialValue);
        LoadInitialValue(initialValue);
    }

    public event Func<SyncProfileBranchDialog, Task<CompanyLookupItem?>>? CreateBranchCompanyRequested;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SyncProfileBranchDialogResult? Result { get; private set; }

    public void RefreshBranchCompanies(
        IReadOnlyCollection<CompanyLookupItem> companies,
        int? selectedBranchCompanyId = null)
    {
        branchCompanies = companies;
        ConfigureBranchLookup(lueBranchCompany.CreateButtonEnabled);
        if (selectedBranchCompanyId.HasValue)
        {
            lueBranchCompany.EditValue = selectedBranchCompanyId.Value;
        }
    }

    private void ConfigureBranchLookup(bool canCreateBranchCompany)
    {
        lueBranchCompany.RefreshButtons();
        lueBranchCompany.Properties.DataSource = branchCompanies.ToList();
        lueBranchCompany.Properties.DisplayMember = nameof(CompanyLookupItem.DisplayName);
        lueBranchCompany.Properties.ValueMember = nameof(CompanyLookupItem.Id);
        lueBranchCompany.Properties.NullText = string.Empty;
        lueBranchCompany.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueBranchCompany.Properties.SearchMode = SearchMode.AutoSearch;
        lueBranchCompany.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lueBranchCompany.Properties.Columns.Clear();
        lueBranchCompany.Properties.Columns.Add(new LookUpColumnInfo(nameof(CompanyLookupItem.Code), "Codigo empresa", 140));
        lueBranchCompany.Properties.Columns.Add(new LookUpColumnInfo(nameof(CompanyLookupItem.Name), "Nombre", 220));
        lueBranchCompany.Properties.Columns.Add(new LookUpColumnInfo(nameof(CompanyLookupItem.BranchCode), "Codigo sucursal", 120));
        lueBranchCompany.Properties.Columns.Add(new LookUpColumnInfo(nameof(CompanyLookupItem.DatabaseName), "Base de datos", 170));
        lueBranchCompany.CreateButtonEnabled = canCreateBranchCompany;
        lueBranchCompany.RefreshButtons();
    }

    private void WireEvents()
    {
        lueBranchCompany.EditValueChanged += (_, _) => FillBranchDetails();
        lueBranchCompany.ClearButtonClick += (_, _) => ClearBranchDetails();
        lueBranchCompany.CreateButtonClick += BranchLookupCreateButtonClick;
        btnAdd.Click += BtnAdd_Click;
    }

    private async void BranchLookupCreateButtonClick(object? sender, EventArgs e)
    {
        if (!lueBranchCompany.CreateButtonEnabled || CreateBranchCompanyRequested is null)
        {
            return;
        }

        var created = await CreateBranchCompanyRequested(this);
        if (created is not null)
        {
            lueBranchCompany.EditValue = created.Id;
        }
    }

    private void FillBranchDetails()
    {
        if (SelectedBranch() is not { } branch)
        {
            ClearBranchDetails();
            return;
        }

        txtBranchCode.Text = branch.BranchCode ?? string.Empty;
        txtBranchName.Text = branch.Name;
        txtDatabaseName.Text = branch.DatabaseName ?? string.Empty;
    }

    private void ClearBranchDetails()
    {
        txtBranchCode.Text = string.Empty;
        txtBranchName.Text = string.Empty;
        txtDatabaseName.Text = string.Empty;
        txtLastSynchronization.Text = string.Empty;
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (SelectedBranch() is not { } branch)
        {
            DialogResult = DialogResult.None;
            XtraMessageBox.Show(this, "Seleccione una empresa tipo sucursal.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new SyncProfileBranchDialogResult(
            branch.Id,
            branch.Code,
            branch.Name,
            branch.BranchCode,
            branch.DatabaseName,
            Convert.ToInt32(sedBatchSize.Value),
            Convert.ToInt32(sedMaxRetries.Value),
            Convert.ToBoolean(swIsActive.EditValue),
            lastSynchronizationAt);
    }

    private void LoadInitialValue(SyncProfileBranchDialogResult initialValue)
    {
        lastSynchronizationAt = initialValue.LastSynchronizationAt;
        lueBranchCompany.EditValue = initialValue.BranchCompanyId;
        lueBranchCompany.Enabled = false;
        lueBranchCompany.CreateButtonEnabled = false;
        lueBranchCompany.RefreshButtons();
        sedBatchSize.Value = initialValue.BatchSize;
        sedMaxRetries.Value = initialValue.MaxRetries;
        swIsActive.EditValue = initialValue.IsActive;
        txtLastSynchronization.Text = initialValue.LastSynchronizationAt?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty;
        btnAdd.ButtonText = "Guardar";
        btnAdd.Text = "Guardar";
        btnAdd.IconNameOverride = "guardar_16.svg";
        Text = "Editar sucursal del perfil";
    }

    private CompanyLookupItem? SelectedBranch()
    {
        return lueBranchCompany.EditValue is int id
            ? branchCompanies.FirstOrDefault(company => company.Id == id)
            : null;
    }
}

public sealed record SyncProfileBranchDialogResult(
    int BranchCompanyId,
    string BranchCompanyCode,
    string BranchCompanyName,
    string? BranchCode,
    string? DatabaseName,
    int BatchSize,
    int MaxRetries,
    bool IsActive,
    DateTime? LastSynchronizationAt = null);
