using System.ComponentModel;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.ViewModels.Sap;

public sealed class SapSupplierMappingViewModel(
    long profileId,
    long profileEntityId,
    ISapMasterDataMappingClient client)
{
    public BindingList<SapSupplierMappingRow> Rows { get; } = [];

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var mappings = await client.GetFieldMappingsAsync(profileId, profileEntityId, cancellationToken: cancellationToken);
        Rows.Clear();
        foreach (var mapping in mappings.OrderBy(item => item.ExecutionOrder))
        {
            Rows.Add(SapSupplierMappingRow.From(mapping));
        }

        if (Rows.Count == 0)
        {
            AddDefaults();
        }
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
        => client.ReplaceFieldMappingsAsync(profileId, profileEntityId, Rows.Select(row => row.ToRequest()).ToArray(), cancellationToken);

    private void AddDefaults()
    {
        var defaults = new[]
        {
            ("CardCode", "SapCardCode", true, true, "Trim"),
            ("Code", "Code", true, false, "GenerateInternalCode"),
            ("CardName", "Name", true, false, "Trim"),
            ("LicTradNum", "IdentificationNumber", true, false, "TaxIdNormalize"),
            ("TaxIdType", "TaxpayerType", false, false, "Trim"),
            ("Phone1", "Phone", false, false, "Trim"),
            ("E_Mail", "Email", false, false, "LowerInvariant"),
            ("ValidFor", "IsActive", true, false, "SapBooleanYN")
        };
        var order = 1;
        foreach (var item in defaults)
        {
            Rows.Add(new SapSupplierMappingRow
            {
                SapField = item.Item1,
                NuanField = item.Item2,
                Direction = item.Item2.Equals("Code", StringComparison.OrdinalIgnoreCase) ? "Generated" : "SapToErp",
                IsRequired = item.Item3,
                IsReadOnly = item.Item4,
                TransformationKey = item.Item5,
                ExecutionOrder = order++,
                IsActive = true
            });
        }
    }
}

public sealed class SapSupplierMappingRow
{
    public string SapField { get; set; } = string.Empty;
    public string NuanField { get; set; } = string.Empty;
    public string Direction { get; set; } = "SapToErp";
    public string DataType { get; set; } = "Text";
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public string TransformationKey { get; set; } = "Trim";
    public string? DefaultValue { get; set; }
    public string? ValidationRuleKey { get; set; }
    public int ExecutionOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public static SapSupplierMappingRow From(SapFieldMappingItem item) => new()
    {
        SapField = item.SapField,
        NuanField = item.NuanField,
        Direction = item.Direction,
        DataType = item.DataType,
        IsRequired = item.IsRequired,
        IsReadOnly = item.IsReadOnly,
        TransformationKey = item.TransformationKey,
        DefaultValue = item.DefaultValue,
        ValidationRuleKey = item.ValidationRuleKey,
        ExecutionOrder = item.ExecutionOrder,
        IsActive = item.IsActive
    };

    public SaveSapFieldMappingRequest ToRequest() => new(
        SapField.Trim(), NuanField.Trim(), Direction, DataType, IsRequired, IsReadOnly,
        TransformationKey.Trim(), string.IsNullOrWhiteSpace(DefaultValue) ? null : DefaultValue.Trim(),
        string.IsNullOrWhiteSpace(ValidationRuleKey) ? null : ValidationRuleKey.Trim(),
        ExecutionOrder, IsActive);
}
