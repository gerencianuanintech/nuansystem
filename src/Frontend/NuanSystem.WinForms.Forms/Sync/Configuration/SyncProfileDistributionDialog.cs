using System.ComponentModel;
using System.Text.Json;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncProfileDistributionDialog : XtraForm
{
    private readonly SyncProfileDistributionDialogData data;
    private readonly ISyncConfigurationClient? client;
    private readonly HashSet<Guid> selectedIds = [];
    private readonly BindingList<SyncDistributionCandidate> candidates = [];
    private readonly BindingList<SyncRuleConditionRow> ruleConditions = [];
    private CancellationTokenSource? searchCancellation;
    private bool busy;

    public SyncProfileDistributionDialog()
        : this(
            new SyncProfileDistributionDialogData(
                1, "Warehouse", "Bodegas", 1002, "DEMO-REMIGIO", "REMIGIO", "Sucursal Remigio",
                true, null, 500, null, 500),
            null,
            new SyncDistributionPolicy(
                1, 1, "DEMO-PREVIEW", 1, "DEMO", "Warehouse", 1002, "DEMO-REMIGIO", "Sucursal Remigio",
                "Selected", "KeepInMaster", null, 1, Array.Empty<SyncDistributionSelection>()),
            new SyncDistributionPolicyCatalog(
                ["None", "All", "Selected", "Rule"],
                ["KeepInMaster"],
                ["Equals", "NotEquals", "In", "IsTrue", "IsFalse"],
                ["code", "branchCode", "sapCode", "isActive", "allowsSales", "allowsPurchases"]))
    {
    }

    public SyncProfileDistributionDialog(
        SyncProfileDistributionDialogData data,
        ISyncConfigurationClient? client,
        SyncDistributionPolicy policy,
        SyncDistributionPolicyCatalog catalog)
    {
        this.data = data;
        this.client = client;
        Policy = policy;
        Catalog = catalog;
        InitializeComponent();
        AppTypography.ApplyToForm(this);
        ConfigureEditors();
        LoadData();
        WireEvents();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SyncProfileDistributionDialogResult? Result { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SyncDistributionPolicy Policy { get; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SyncDistributionPolicyCatalog Catalog { get; }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (string.Equals(cboDistributionMode.Text, "Selected", StringComparison.OrdinalIgnoreCase))
        {
            _ = SearchCandidatesAsync();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        searchCancellation?.Cancel();
        searchCancellation?.Dispose();
        base.OnFormClosed(e);
    }

    private void ConfigureEditors()
    {
        cboDistributionMode.Properties.Items.Clear();
        cboDistributionMode.Properties.Items.AddRange(Catalog.Modes.Cast<object>().ToArray());
        cboDistributionMode.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

        cboRuleMatch.Properties.Items.Clear();
        cboRuleMatch.Properties.Items.AddRange(new object[] { "All", "Any" });
        cboRuleMatch.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

        repRuleField.Items.Clear();
        repRuleField.Items.AddRange(Catalog.Fields.Cast<object>().ToArray());
        repRuleOperator.Items.Clear();
        repRuleOperator.Items.AddRange(Catalog.Operators.Cast<object>().ToArray());

        grcCandidates.DataSource = candidates;
        grcRuleConditions.DataSource = ruleConditions;
    }

    private void LoadData()
    {
        txtEntityCode.Text = data.EntityCode;
        txtEntityName.Text = data.EntityName;
        txtBranchCompanyCode.Text = data.BranchCompanyCode;
        txtBranchName.Text = data.BranchName;
        swIsEnabled.EditValue = data.IsEnabled;
        sedBatchSize.EditValue = data.BatchSize;
        cboDistributionMode.EditValue = Policy.DistributionMode;
        cboRuleMatch.EditValue = "All";

        foreach (var selection in Policy.Selections)
        {
            selectedIds.Add(selection.EntityGlobalId);
        }

        LoadRule(Policy.RuleExpressionJson);
        UpdateEffectiveBatch();
        UpdateModeState();
    }

    private void WireEvents()
    {
        sedBatchSize.EditValueChanged += (_, _) => UpdateEffectiveBatch();
        sedBatchSize.Properties.ButtonClick += BatchSizeButtonClick;
        cboDistributionMode.SelectedIndexChanged += (_, _) => UpdateModeState();
        txtCandidateSearch.ButtonClick += async (_, _) => await SearchCandidatesAsync();
        txtCandidateSearch.KeyDown += async (_, args) =>
        {
            if (args.KeyCode == Keys.Enter)
            {
                args.SuppressKeyPress = true;
                await SearchCandidatesAsync();
            }
        };
        grvCandidates.CellValueChanged += CandidatesCellValueChanged;
        btnAddRule.Click += (_, _) => AddRuleCondition();
        btnRemoveRule.Click += (_, _) => RemoveSelectedRuleCondition();
        grvRuleConditions.CellValueChanged += RuleConditionChanged;
        btnSave.Click += async (_, _) => await SaveAsync();
    }

    private void BatchSizeButtonClick(object sender, ButtonPressedEventArgs e)
    {
        if (e.Button.Kind == ButtonPredefines.Delete)
        {
            sedBatchSize.EditValue = null;
        }
    }

    private void UpdateEffectiveBatch()
    {
        txtEffectiveBatch.Text = EffectiveBatchSize().ToString("N0");
    }

    private int EffectiveBatchSize()
    {
        return SpecificBatchSize()
               ?? data.EntityBatchSize
               ?? data.BranchBatchSize
               ?? data.ProfileBatchSize;
    }

    private int? SpecificBatchSize()
    {
        return sedBatchSize.EditValue is null or DBNull
            ? null
            : Convert.ToInt32(sedBatchSize.Value);
    }

    private void UpdateModeState()
    {
        var selectedMode = string.Equals(cboDistributionMode.Text, "Selected", StringComparison.OrdinalIgnoreCase);
        var ruleMode = string.Equals(cboDistributionMode.Text, "Rule", StringComparison.OrdinalIgnoreCase);
        pageSelected.PageEnabled = selectedMode;
        pageRule.PageEnabled = ruleMode;
        if (selectedMode)
        {
            tabPolicy.SelectedTabPage = pageSelected;
        }
        else if (ruleMode)
        {
            tabPolicy.SelectedTabPage = pageRule;
        }
    }

    private async Task SearchCandidatesAsync()
    {
        if (client is null || busy)
        {
            return;
        }

        searchCancellation?.Cancel();
        searchCancellation?.Dispose();
        searchCancellation = new CancellationTokenSource();
        var cancellationToken = searchCancellation.Token;

        try
        {
            SetBusy(true, "Buscando registros...");
            var result = await client.SearchDistributionCandidatesAsync(
                data.MatrixId,
                txtCandidateSearch.Text,
                200,
                cancellationToken);

            candidates.RaiseListChangedEvents = false;
            candidates.Clear();
            foreach (var candidate in result)
            {
                candidate.IsSelected = selectedIds.Contains(candidate.EntityGlobalId);
                candidates.Add(candidate);
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, Text, exception);
        }
        finally
        {
            candidates.RaiseListChangedEvents = true;
            candidates.ResetBindings();
            SetBusy(false, $"{selectedIds.Count:N0} registros seleccionados");
        }
    }

    private void CandidatesCellValueChanged(object? sender, CellValueChangedEventArgs e)
    {
        if (e.Column.FieldName != nameof(SyncDistributionCandidate.IsSelected)
            || grvCandidates.GetRow(e.RowHandle) is not SyncDistributionCandidate candidate)
        {
            return;
        }

        if (candidate.IsSelected)
        {
            selectedIds.Add(candidate.EntityGlobalId);
        }
        else
        {
            selectedIds.Remove(candidate.EntityGlobalId);
        }

        lblSelectionStatus.Text = $"{selectedIds.Count:N0} registros seleccionados";
    }

    private void LoadRule(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            cboRuleMatch.EditValue = root.TryGetProperty("match", out var match) ? match.GetString() ?? "All" : "All";
            if (!root.TryGetProperty("conditions", out var conditions) || conditions.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var condition in conditions.EnumerateArray())
            {
                var operation = condition.TryGetProperty("operator", out var operatorElement)
                    ? operatorElement.GetString() ?? "Equals"
                    : "Equals";
                var value = condition.TryGetProperty("values", out var valuesElement)
                    ? string.Join(", ", valuesElement.EnumerateArray().Select(ReadJsonValue))
                    : condition.TryGetProperty("value", out var valueElement)
                        ? ReadJsonValue(valueElement)
                        : string.Empty;

                ruleConditions.Add(new SyncRuleConditionRow
                {
                    Field = condition.TryGetProperty("field", out var fieldElement) ? fieldElement.GetString() ?? string.Empty : string.Empty,
                    Operator = operation,
                    Value = value
                });
            }
        }
        catch (JsonException)
        {
            ruleConditions.Clear();
        }
    }

    private static string ReadJsonValue(JsonElement value) => value.ValueKind switch
    {
        JsonValueKind.String => value.GetString() ?? string.Empty,
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        JsonValueKind.Number => value.GetRawText(),
        _ => value.GetRawText()
    };

    private void AddRuleCondition()
    {
        ruleConditions.Add(new SyncRuleConditionRow
        {
            Field = Catalog.Fields.FirstOrDefault() ?? "code",
            Operator = Catalog.Operators.FirstOrDefault() ?? "Equals"
        });
        grvRuleConditions.FocusedRowHandle = ruleConditions.Count - 1;
    }

    private void RemoveSelectedRuleCondition()
    {
        if (grvRuleConditions.GetFocusedRow() is SyncRuleConditionRow row)
        {
            ruleConditions.Remove(row);
        }
    }

    private void RuleConditionChanged(object? sender, CellValueChangedEventArgs e)
    {
        if (e.Column.FieldName == nameof(SyncRuleConditionRow.Operator))
        {
            grvRuleConditions.RefreshRow(e.RowHandle);
        }
    }

    private async Task SaveAsync()
    {
        if (busy || !ValidatePolicy())
        {
            return;
        }

        var request = BuildPolicyRequest();
        try
        {
            SetBusy(true, "Guardando política...");
            if (client is not null)
            {
                await client.UpdateDistributionPolicyAsync(data.MatrixId, request);
            }

            Result = new SyncProfileDistributionDialogResult(
                Convert.ToBoolean(swIsEnabled.EditValue),
                SpecificBatchSize(),
                request.DistributionMode,
                selectedIds.Count,
                request.RuleExpressionJson);
            DialogResult = DialogResult.OK;
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, Text, exception);
        }
        finally
        {
            SetBusy(false, $"{selectedIds.Count:N0} registros seleccionados");
        }
    }

    private bool ValidatePolicy()
    {
        var batchSize = SpecificBatchSize();
        if (batchSize is < 1 or > 10000)
        {
            ShowWarning("El batch específico debe estar entre 1 y 10000, o quedar vacío para heredar la configuración.");
            return false;
        }

        var mode = cboDistributionMode.Text;
        if (!Catalog.Modes.Contains(mode, StringComparer.OrdinalIgnoreCase))
        {
            ShowWarning("Seleccione una modalidad de distribución válida.");
            return false;
        }

        if (string.Equals(mode, "Selected", StringComparison.OrdinalIgnoreCase) && selectedIds.Count == 0)
        {
            tabPolicy.SelectedTabPage = pageSelected;
            ShowWarning("Seleccione al menos un registro para la modalidad Selected.");
            return false;
        }

        if (string.Equals(mode, "Rule", StringComparison.OrdinalIgnoreCase))
        {
            grvRuleConditions.CloseEditor();
            grvRuleConditions.UpdateCurrentRow();
            if (ruleConditions.Count == 0)
            {
                tabPolicy.SelectedTabPage = pageRule;
                ShowWarning("Agregue al menos una condición para la modalidad Rule.");
                return false;
            }

            var invalid = ruleConditions.Any(condition =>
                !Catalog.Fields.Contains(condition.Field, StringComparer.OrdinalIgnoreCase)
                || !Catalog.Operators.Contains(condition.Operator, StringComparer.OrdinalIgnoreCase)
                || (condition.Operator is not ("IsTrue" or "IsFalse") && string.IsNullOrWhiteSpace(condition.Value)));
            if (invalid)
            {
                tabPolicy.SelectedTabPage = pageRule;
                ShowWarning("Complete los campos, operadores y valores de todas las condiciones.");
                return false;
            }
        }

        return true;
    }

    private SaveSyncDistributionPolicyRequest BuildPolicyRequest()
    {
        var mode = cboDistributionMode.Text;
        return new SaveSyncDistributionPolicyRequest
        {
            DistributionMode = mode,
            OnNoMatch = "KeepInMaster",
            Selections = string.Equals(mode, "Selected", StringComparison.OrdinalIgnoreCase)
                ? selectedIds.Select(id => new SyncDistributionSelection(id, FindCandidateCode(id))).ToArray()
                : Array.Empty<SyncDistributionSelection>(),
            RuleExpressionJson = string.Equals(mode, "Rule", StringComparison.OrdinalIgnoreCase)
                ? BuildRuleJson()
                : null
        };
    }

    private string? FindCandidateCode(Guid id)
    {
        return candidates.FirstOrDefault(candidate => candidate.EntityGlobalId == id)?.EntityCode
               ?? Policy.Selections.FirstOrDefault(selection => selection.EntityGlobalId == id)?.EntityCode;
    }

    private string BuildRuleJson()
    {
        var conditions = ruleConditions.Select(condition =>
        {
            var values = condition.Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var item = new Dictionary<string, object?>
            {
                ["field"] = condition.Field,
                ["operator"] = condition.Operator
            };
            if (string.Equals(condition.Operator, "In", StringComparison.OrdinalIgnoreCase))
            {
                item["values"] = values;
            }
            else if (condition.Operator is not ("IsTrue" or "IsFalse"))
            {
                item["value"] = condition.Value.Trim();
            }

            return item;
        }).ToArray();

        return JsonSerializer.Serialize(new
        {
            match = string.IsNullOrWhiteSpace(cboRuleMatch.Text) ? "All" : cboRuleMatch.Text,
            conditions
        });
    }

    private void SetBusy(bool value, string status)
    {
        busy = value;
        btnSave.Enabled = !value;
        btnCancel.Enabled = !value;
        txtCandidateSearch.Enabled = !value;
        lblSelectionStatus.Text = status;
    }

    private void ShowWarning(string message)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}

public sealed record SyncProfileDistributionDialogData(
    int MatrixId,
    string EntityCode,
    string EntityName,
    int BranchCompanyId,
    string BranchCompanyCode,
    string? BranchCode,
    string BranchName,
    bool IsEnabled,
    int? BatchSize,
    int? EntityBatchSize,
    int? BranchBatchSize,
    int ProfileBatchSize);

public sealed record SyncProfileDistributionDialogResult(
    bool IsEnabled,
    int? BatchSize,
    string DistributionMode,
    int SelectedCount,
    string? RuleExpressionJson);

public sealed class SyncRuleConditionRow
{
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = "Equals";
    public string Value { get; set; } = string.Empty;
}
