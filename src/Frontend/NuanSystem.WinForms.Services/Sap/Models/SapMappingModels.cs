namespace NuanSystem.WinForms.Services.Sap.Models;

public sealed record SapFieldMappingItem
{
    public long Id { get; init; }
    public long SapSyncProfileEntityId { get; init; }
    public string SapField { get; init; } = string.Empty;
    public string NuanField { get; init; } = string.Empty;
    public string Direction { get; init; } = "SapToErp";
    public string DataType { get; init; } = "Text";
    public bool IsRequired { get; init; }
    public bool IsReadOnly { get; init; }
    public string TransformationKey { get; init; } = "Trim";
    public string? DefaultValue { get; init; }
    public string? ValidationRuleKey { get; init; }
    public int ExecutionOrder { get; init; }
    public bool IsActive { get; init; }
}

public sealed record SaveSapFieldMappingRequest(
    string SapField,
    string NuanField,
    string Direction,
    string DataType,
    bool IsRequired,
    bool IsReadOnly,
    string TransformationKey,
    string? DefaultValue,
    string? ValidationRuleKey,
    int ExecutionOrder,
    bool IsActive);

public sealed record SapMappingWriteResult(long? Id, string ResultCode, byte[]? RowVersion);
