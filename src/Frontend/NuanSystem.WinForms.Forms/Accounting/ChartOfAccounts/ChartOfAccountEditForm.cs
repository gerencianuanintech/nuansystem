using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;

namespace NuanSystem.WinForms.Forms.Accounting.ChartOfAccounts;

public sealed partial class ChartOfAccountEditForm : BaseEditForm
{
    private readonly int companyId;
    private IReadOnlyCollection<ChartOfAccountLookupItem> parentAccounts;
    private EditorButton? createParentButton;
    private bool loadingParentAccounts;
    private int? currentAccountId;

    public ChartOfAccountEditForm(
        int companyId,
        IReadOnlyCollection<ChartOfAccountLookupItem> parentAccounts,
        bool canCreateParent,
        ChartOfAccountItem? account = null,
        bool copyMode = false)
    {
        this.companyId = companyId;
        this.parentAccounts = parentAccounts;
        InitializeComponent();
        ConfigureForm(canCreateParent);
        lueTipoCuenta.EditValue = account?.AccountType ?? "ASSET";
        LoadParentAccounts(parentAccounts, account?.Id);

        if (account is not null)
        {
            LoadAccount(account, copyMode);
        }
        else
        {
            lueClaseCuenta.EditValue = "OTHER";
            txtSaldo.Text = 0m.ToString("N2", CultureInfo.CurrentCulture);
            chkPermiteMovimiento.Checked = true;
            chkActivo.Checked = true;
            ApplyTitleMovementRule();
        }
    }

    public event Func<ChartOfAccountEditForm, Task<ChartOfAccountLookupItem?>>? CreateParentRequested;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveChartOfAccountRequest Request { get; private set; } = new(
        0,
        string.Empty,
        string.Empty,
        null,
        null,
        "ASSET",
        "OTHER",
        null,
        false,
        true,
        true,
        null,
        0m,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? CreatedOrUpdatedId { get; set; }

    public void RefreshParentAccounts(IReadOnlyCollection<ChartOfAccountLookupItem> accounts, int? selectedId)
    {
        parentAccounts = accounts;
        LoadParentAccounts(accounts, null);
        lueCuentaPadre.EditValue = selectedId;
    }

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCodigo, "Ingrese el codigo de la cuenta.");
        isValid &= Validator.RequireText(txtNombre, "Ingrese el nombre de la cuenta.");
        isValid &= Validator.RequireText(lueTipoCuenta, "Seleccione el tipo de cuenta.");

        if (!string.IsNullOrWhiteSpace(txtMoneda.Text) && txtMoneda.Text.Trim().Length != 3)
        {
            Validator.SetError(txtMoneda, "La moneda debe tener 3 caracteres.");
            isValid = false;
        }

        if (!TryParseBalance(out _))
        {
            Validator.SetError(txtSaldo, "Ingrese un saldo valido.");
            isValid = false;
        }

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveChartOfAccountRequest(
            companyId,
            txtCodigo.Text.Trim(),
            txtNombre.Text.Trim(),
            NormalizeText(memDescripcion.Text),
            NormalizeText(txtCodigoExterno.Text),
            Convert.ToString(lueTipoCuenta.EditValue) ?? "ASSET",
            NormalizeUpper(Convert.ToString(lueClaseCuenta.EditValue)),
            lueCuentaPadre.EditValue is int parentId ? parentId : null,
            chkTitulo.Checked,
            chkPermiteMovimiento.Checked,
            chkActivo.Checked,
            NormalizeUpper(txtMoneda.Text),
            ParseBalance(),
            chkConfidencial.Checked,
            chkCuentaMonetaria.Checked,
            chkCuentaAsociada.Checked,
            chkRevaluaIndice.Checked,
            chkBloquearManual.Checked,
            chkFlujoCaja.Checked,
            chkCentroCosto.Checked,
            chkTercero.Checked,
            chkProyecto.Checked);
    }

    private void ConfigureForm(bool canCreateParent)
    {
        OperationButtonIcons.ApplySaveCancel(btnGuardar, btnCancelar);
        btnGuardar.Click += (_, _) => Save();
        ApplyAccountTypeButtonIcons();

        if (canCreateParent)
        {
            createParentButton = new EditorButton(ButtonPredefines.Plus)
            {
                ToolTip = "Crear cuenta padre"
            };
            lueCuentaPadre.Properties.Buttons.Add(createParentButton);
            lueCuentaPadre.Properties.ButtonClick += CuentaPadreButtonClick;
        }

        lueTipoCuenta.Properties.DataSource = AccountTypes.All;
        lueTipoCuenta.Properties.DisplayMember = nameof(AccountTypeOption.DisplayText);
        lueTipoCuenta.Properties.ValueMember = nameof(AccountTypeOption.Code);
        lueTipoCuenta.Properties.Columns.Clear();
        lueTipoCuenta.Properties.Columns.Add(new LookUpColumnInfo(nameof(AccountTypeOption.Code), "Codigo", 100));
        lueTipoCuenta.Properties.Columns.Add(new LookUpColumnInfo(nameof(AccountTypeOption.Name), "Nombre", 160));
        lueTipoCuenta.Properties.NullText = "";

        lueClaseCuenta.Properties.DataSource = AccountClasses.All;
        lueClaseCuenta.Properties.DisplayMember = nameof(AccountClassOption.DisplayText);
        lueClaseCuenta.Properties.ValueMember = nameof(AccountClassOption.Code);
        lueClaseCuenta.Properties.Columns.Clear();
        lueClaseCuenta.Properties.Columns.Add(new LookUpColumnInfo(nameof(AccountClassOption.Code), "Codigo", 100));
        lueClaseCuenta.Properties.Columns.Add(new LookUpColumnInfo(nameof(AccountClassOption.Name), "Nombre", 160));
        lueClaseCuenta.Properties.NullText = "";

        chkTitulo.CheckedChanged += (_, _) => ApplyTitleMovementRule();
        lueTipoCuenta.EditValueChanged += (_, _) => RefreshParentAccountOptions();
        lueCuentaPadre.EditValueChanged += (_, _) => RefreshLevelPreview();
        trlAccounts.FocusedNodeChanged += AccountTreeFocusedNodeChanged;

        btnTipoActivo.Click += (_, _) => SetAccountType("ASSET");
        btnTipoPasivo.Click += (_, _) => SetAccountType("LIABILITY");
        btnTipoPatrimonio.Click += (_, _) => SetAccountType("EQUITY");
        btnTipoIngreso.Click += (_, _) => SetAccountType("INCOME");
        btnTipoCosto.Click += (_, _) => SetAccountType("COST");
        btnTipoGasto.Click += (_, _) => SetAccountType("EXPENSE");
        btnTipoOrden.Click += (_, _) => SetAccountType("ORDER");
    }

    private void ApplyAccountTypeButtonIcons()
    {
        ApplyAccountingIcon(btnTipoActivo, "account_asset_32.svg");
        ApplyAccountingIcon(btnTipoPasivo, "account_liability_32.svg");
        ApplyAccountingIcon(btnTipoPatrimonio, "account_equity_32.svg");
        ApplyAccountingIcon(btnTipoIngreso, "account_income_32.svg");
        ApplyAccountingIcon(btnTipoCosto, "account_cost_32.svg");
        ApplyAccountingIcon(btnTipoGasto, "account_expense_32.svg");
        ApplyAccountingIcon(btnTipoOrden, "account_order_32.svg");
    }

    private static void ApplyAccountingIcon(SimpleButton button, string fileName)
    {
        var iconPath = ResolveAccountingIconPath(fileName);
        if (!File.Exists(iconPath))
        {
            return;
        }

        button.ImageOptions.SvgImage = SvgImage.FromFile(iconPath);
        button.ImageOptions.SvgImageSize = new Size(24, 24);
    }

    private static string ResolveAccountingIconPath(string fileName)
    {
        var relativePath = Path.Combine("Assets", "Icons", "Accounting", fileName);
        var outputPath = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (File.Exists(outputPath))
        {
            return outputPath;
        }

        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var projectPath = Path.Combine(
                directory.FullName,
                "src",
                "Frontend",
                "NuanSystem.WinForms.Forms",
                relativePath);
            if (File.Exists(projectPath))
            {
                return projectPath;
            }

            var localPath = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(localPath))
            {
                return localPath;
            }

            directory = directory.Parent;
        }

        return outputPath;
    }

    private async void CuentaPadreButtonClick(object sender, ButtonPressedEventArgs e)
    {
        if (!ReferenceEquals(e.Button, createParentButton))
        {
            return;
        }

        await CreateParentAsync();
    }

    private async Task CreateParentAsync()
    {
        if (CreateParentRequested is null)
        {
            return;
        }

        var created = await CreateParentRequested(this);
        if (created is not null)
        {
            lueCuentaPadre.EditValue = created.Id;
        }
    }

    private void LoadParentAccounts(IReadOnlyCollection<ChartOfAccountLookupItem> accounts, int? currentId)
    {
        parentAccounts = accounts;
        currentAccountId = currentId;

        loadingParentAccounts = true;
        lueCuentaPadre.Properties.DisplayMember = nameof(ChartOfAccountLookupItem.DisplayText);
        lueCuentaPadre.Properties.ValueMember = nameof(ChartOfAccountLookupItem.Id);
        lueCuentaPadre.Properties.NullText = "";
        lueCuentaPadre.Properties.Columns.Clear();
        lueCuentaPadre.Properties.Columns.Add(new LookUpColumnInfo(nameof(ChartOfAccountLookupItem.Code), "Codigo", 120));
        lueCuentaPadre.Properties.Columns.Add(new LookUpColumnInfo(nameof(ChartOfAccountLookupItem.Name), "Nombre", 240));
        loadingParentAccounts = false;

        RefreshParentAccountOptions();
    }

    private void RefreshParentAccountOptions()
    {
        var selectedType = Convert.ToString(lueTipoCuenta.EditValue);
        var selectedParentId = lueCuentaPadre.EditValue is int parentId ? parentId : (int?)null;
        var options = parentAccounts
            .Where(account => !currentAccountId.HasValue || account.Id != currentAccountId.Value)
            .Where(account => string.IsNullOrWhiteSpace(selectedType) || account.AccountType == selectedType)
            .OrderBy(account => account.Code)
            .ToList();

        loadingParentAccounts = true;
        lueCuentaPadre.Properties.DataSource = options;
        trlAccounts.DataSource = BuildTreeOptions(options);
        trlAccounts.ExpandAll();

        if (selectedParentId.HasValue && options.All(account => account.Id != selectedParentId.Value))
        {
            lueCuentaPadre.EditValue = null;
        }

        loadingParentAccounts = false;
        RefreshLevelPreview();
    }

    private static IReadOnlyCollection<ChartOfAccountTreeItem> BuildTreeOptions(
        IReadOnlyCollection<ChartOfAccountLookupItem> accounts)
    {
        var availableIds = accounts.Select(account => account.Id).ToHashSet();
        return accounts
            .Select(account => new ChartOfAccountTreeItem(
                account.Id,
                account.Code,
                account.Name,
                account.AccountType,
                account.ParentAccountId.HasValue && availableIds.Contains(account.ParentAccountId.Value)
                    ? account.ParentAccountId
                    : null,
                account.Level,
                account.IsActive))
            .ToList();
    }

    private void AccountTreeFocusedNodeChanged(object? sender, FocusedNodeChangedEventArgs e)
    {
        if (loadingParentAccounts)
        {
            return;
        }

        if (e.Node?.GetValue(nameof(ChartOfAccountLookupItem.Id)) is int accountId)
        {
            lueCuentaPadre.EditValue = accountId;
        }
    }

    private void SetAccountType(string accountType)
    {
        lueTipoCuenta.EditValue = accountType;
    }

    private void RefreshLevelPreview()
    {
        var parentId = lueCuentaPadre.EditValue is int id ? id : (int?)null;
        var level = 1;

        if (parentId.HasValue)
        {
            var parent = parentAccounts.FirstOrDefault(account => account.Id == parentId.Value);
            level = (parent?.Level ?? 0) + 1;
        }

        txtNivel.Text = level.ToString();
    }

    private void LoadAccount(ChartOfAccountItem account, bool copyMode)
    {
        Text = copyMode ? "Copiar cuenta contable" : "Editar cuenta contable";
        CreatedOrUpdatedId = copyMode ? null : account.Id;
        txtCodigo.Text = copyMode ? string.Empty : account.Code;
        txtNombre.Text = account.Name;
        memDescripcion.Text = account.Description;
        txtCodigoExterno.Text = account.ExternalCode;
        lueTipoCuenta.EditValue = account.AccountType;
        lueClaseCuenta.EditValue = account.AccountClass ?? "OTHER";
        lueCuentaPadre.EditValue = account.ParentAccountId;
        chkTitulo.Checked = account.IsTitle;
        chkPermiteMovimiento.Checked = account.AllowsMovement;
        chkActivo.Checked = account.IsActive;
        txtMoneda.Text = account.CurrencyCode;
        txtSaldo.Text = account.Balance.ToString("N2", CultureInfo.CurrentCulture);
        chkConfidencial.Checked = account.IsConfidential;
        chkCuentaMonetaria.Checked = account.IsMonetaryAccount;
        chkCuentaAsociada.Checked = account.IsAssociatedAccount;
        chkRevaluaIndice.Checked = account.RevalueByIndex;
        chkBloquearManual.Checked = account.BlockManualPosting;
        chkFlujoCaja.Checked = account.RelevantForCashFlow;
        chkCentroCosto.Checked = account.RequiresCostCenter;
        chkTercero.Checked = account.RequiresThirdParty;
        chkProyecto.Checked = account.RequiresProject;
        ApplyTitleMovementRule();
    }

    private void ApplyTitleMovementRule()
    {
        if (chkTitulo.Checked)
        {
            chkPermiteMovimiento.Checked = false;
        }

        chkPermiteMovimiento.Enabled = !chkTitulo.Checked;
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? NormalizeUpper(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }

    private decimal ParseBalance()
    {
        return TryParseBalance(out var balance) ? balance : 0m;
    }

    private bool TryParseBalance(out decimal balance)
    {
        return decimal.TryParse(txtSaldo.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out balance);
    }

    private sealed record AccountTypeOption(string Code, string Name)
    {
        public string DisplayText => $"{Code} - {Name}";
    }

    private sealed record ChartOfAccountTreeItem(
        int Id,
        string Code,
        string Name,
        string AccountType,
        int? ParentAccountId,
        int Level,
        bool IsActive)
    {
        public string DisplayText => $"{Code} - {Name}";
    }

    private sealed record AccountClassOption(string Code, string Name)
    {
        public string DisplayText => $"{Code} - {Name}";
    }

    private static class AccountTypes
    {
        public static IReadOnlyCollection<AccountTypeOption> All { get; } =
        [
            new("ASSET", "Activo"),
            new("LIABILITY", "Pasivo"),
            new("EQUITY", "Patrimonio"),
            new("INCOME", "Ingreso"),
            new("EXPENSE", "Gasto"),
            new("COST", "Costo"),
            new("ORDER", "Orden")
        ];
    }

    private static class AccountClasses
    {
        public static IReadOnlyCollection<AccountClassOption> All { get; } =
        [
            new("OTHER", "Otros"),
            new("MONETARY", "Cuenta monetaria"),
            new("ASSOCIATED", "Cuenta asociada"),
            new("CASH_FLOW", "Flujo de caja")
        ];
    }
}
