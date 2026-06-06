using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;

namespace NuanSystem.WinForms.Forms.Documents.SecurityDocumentSeries;

public sealed partial class SecurityDocumentSeriesEditForm : BaseEditForm
{
    private readonly SecurityDocumentSeriesLookupSet lookups;

    public SecurityDocumentSeriesEditForm()
        : this(SecurityDocumentSeriesCatalogs.Defaults())
    {
    }

    public SecurityDocumentSeriesEditForm(SecurityDocumentSeriesLookupSet lookups)
    {
        this.lookups = lookups;
        InitializeComponent();
        ConfigureForm();
    }

    public SecurityDocumentSeriesEditForm(SecurityDocumentSeriesItem item, bool copyMode = false)
        : this(SecurityDocumentSeriesCatalogs.Defaults(), item, copyMode)
    {
    }

    public SecurityDocumentSeriesEditForm(
        SecurityDocumentSeriesLookupSet lookups,
        SecurityDocumentSeriesItem item,
        bool copyMode = false)
        : this(lookups)
    {
        LoadItem(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveSecurityDocumentSeriesRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        isValid &= Validator.RequireText(txtPrefix, "Ingrese el prefijo.");
        isValid &= RequireLookup(lueDocumentType, "Seleccione el tipo de documento.");
        isValid &= RequireLookup(lueEstablishment, "Seleccione el establecimiento.");
        isValid &= RequireLookup(lueEmissionPoint, "Seleccione el punto de emision.");

        if (ToInt(sedNextNumber.Value) <= ToInt(sedCurrentNumber.Value))
        {
            isValid = false;
            Validator.SetError(sedNextNumber, "El siguiente numero debe ser mayor al numero actual.");
        }

        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveSecurityDocumentSeriesRequest(
            GetLookupValue(lueDocumentType),
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            NormalizeText(memDescription.Text),
            txtPrefix.Text.Trim(),
            GetLookupValue(lueEstablishment),
            GetLookupValue(lueEmissionPoint),
            ToInt(sedInitialNumber.Value),
            ToInt(sedCurrentNumber.Value),
            ToInt(sedNextNumber.Value),
            ToInt(sedNumberLength.Value),
            NormalizeLookupValue(lueSapObjectType.EditValue),
            ToNullableInt(sedSapSeriesId.Value),
            NormalizeText(txtSapSeriesName.Text),
            chkIsDefault.Checked,
            chkIsActive.Checked,
            chkIsSapIntegrationActive.Checked);
    }

    private void ConfigureForm()
    {
        OperationButtonIcons.ApplySaveCancel(btnSave, btnCancel);
        btnSave.Click += (_, _) => Save();
        chkIsActive.Checked = true;
        chkIsSapIntegrationActive.Checked = true;
        sedInitialNumber.Value = 1;
        sedCurrentNumber.Value = 0;
        sedNextNumber.Value = 1;
        sedNumberLength.Value = 8;

        ConfigureLookup(lueDocumentType, lookups.DocumentTypes);
        ConfigureLookup(lueEstablishment, lookups.Establishments);
        ConfigureEmissionPointLookup(null);
        ConfigureLookup(lueSapObjectType, lookups.SapObjectTypes, allowClear: true);

        lueEstablishment.EditValueChanged += (_, _) => ConfigureEmissionPointLookup(GetLookupValue(lueEstablishment));
    }

    private void LoadItem(SecurityDocumentSeriesItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar serie de documento" : "Editar serie de documento";

        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        lueDocumentType.EditValue = item.DocumentType;
        memDescription.Text = item.Description;
        txtPrefix.Text = item.Prefix;
        lueEstablishment.EditValue = item.Establishment;
        ConfigureEmissionPointLookup(item.Establishment);
        lueEmissionPoint.EditValue = item.EmissionPoint;
        sedInitialNumber.Value = item.InitialNumber;
        sedCurrentNumber.Value = copyMode ? 0 : item.CurrentNumber;
        sedNextNumber.Value = copyMode ? Math.Max(1, item.InitialNumber) : item.NextNumber;
        sedNumberLength.Value = item.NumberLength;
        chkIsDefault.Checked = item.IsDefault;
        chkIsActive.Checked = item.IsActive;
        chkIsSapIntegrationActive.Checked = item.IsSapIntegrationActive;
        lueSapObjectType.EditValue = item.SapObjectType;
        sedSapSeriesId.EditValue = item.SapSeriesId;
        txtSapSeriesName.Text = item.SapSeriesName;
    }

    private static void ConfigureLookup(LookUpEdit lookup, IReadOnlyCollection<LookupOption> options, bool allowClear = false)
    {
        if (allowClear && lookup.Properties.Buttons.Count == 1)
        {
            lookup.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
        }

        lookup.Properties.DataSource = options;
        lookup.Properties.DisplayMember = nameof(LookupOption.Text);
        lookup.Properties.ValueMember = nameof(LookupOption.Value);
        lookup.Properties.NullText = string.Empty;
        lookup.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(LookupOption.Text), "Nombre", 180));

        lookup.ButtonClick += (_, e) =>
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                lookup.EditValue = null;
            }
        };
    }

    private void ConfigureEmissionPointLookup(string? establishment)
    {
        var selected = Convert.ToString(lueEmissionPoint.EditValue);
        var filtered = FilterEmissionPoints(establishment);
        ConfigureLookup(lueEmissionPoint, filtered);

        if (!string.IsNullOrWhiteSpace(selected)
            && filtered.Any(item => string.Equals(item.Value, selected, StringComparison.OrdinalIgnoreCase)))
        {
            lueEmissionPoint.EditValue = selected;
        }
        else
        {
            lueEmissionPoint.EditValue = filtered.FirstOrDefault(item => item.Value == "001")?.Value
                ?? filtered.FirstOrDefault()?.Value;
        }
    }

    private IReadOnlyCollection<LookupOption> FilterEmissionPoints(string? establishment)
    {
        if (string.IsNullOrWhiteSpace(establishment))
        {
            return lookups.EmissionPoints;
        }

        var filtered = lookups.EmissionPoints
            .Where(item => string.IsNullOrWhiteSpace(item.ParentCode)
                || string.Equals(item.ParentCode, establishment, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return filtered.Length == 0 ? lookups.EmissionPoints : filtered;
    }

    private static string GetLookupValue(LookUpEdit lookup)
    {
        return Convert.ToString(lookup.EditValue)?.Trim() ?? string.Empty;
    }

    private bool RequireLookup(LookUpEdit lookup, string message)
    {
        if (lookup.EditValue is null || string.IsNullOrWhiteSpace(Convert.ToString(lookup.EditValue)))
        {
            Validator.SetError(lookup, message);
            return false;
        }

        return true;
    }

    private static string? NormalizeLookupValue(object? value)
    {
        return value is null ? null : NormalizeText(value.ToString());
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static int ToInt(decimal value)
    {
        return Convert.ToInt32(value);
    }

    private static int? ToNullableInt(decimal value)
    {
        var integer = ToInt(value);
        return integer <= 0 ? null : integer;
    }

    private static SaveSecurityDocumentSeriesRequest EmptyRequest()
    {
        return new SaveSecurityDocumentSeriesRequest(
            string.Empty,
            string.Empty,
            string.Empty,
            null,
            string.Empty,
            string.Empty,
            string.Empty,
            1,
            0,
            1,
            8,
            null,
            null,
            null,
            false,
            true,
            true);
    }
}
