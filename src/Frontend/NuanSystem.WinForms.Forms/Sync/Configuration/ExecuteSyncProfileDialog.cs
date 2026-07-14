using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class ExecuteSyncProfileDialog : DevExpress.XtraEditors.XtraForm
{
    public ExecuteSyncProfileDialog()
    {
        InitializeComponent();
        ConfigureRuntimeBehavior();
    }

    public ExecuteSyncProfileDialog(string profileName)
        : this()
    {
        Text = $"Ejecutar perfil - {profileName}";
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ExecuteSyncProfileRequest Request { get; private set; } = new();

    private void ConfigureRuntimeBehavior()
    {
        FormStyler.ApplyBase(this);
        btnExecute.Click += (_, _) => BuildRequest();
    }

    private void BuildRequest()
    {
        var entityCodes = txtEntityCodes.Text
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        Request = new ExecuteSyncProfileRequest
        {
            EntityCodes = entityCodes.Length == 0 ? null : entityCodes,
            FromKey = string.IsNullOrWhiteSpace(txtFromKey.Text) ? null : txtFromKey.Text.Trim(),
            MaxRecords = sedMaxRecords.Value > 0 ? Convert.ToInt32(sedMaxRecords.Value) : null
        };
    }

    private bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }
}
