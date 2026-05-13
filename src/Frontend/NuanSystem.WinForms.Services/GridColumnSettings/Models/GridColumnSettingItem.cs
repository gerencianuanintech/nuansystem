namespace NuanSystem.WinForms.Services.GridColumnSettings.Models;

public sealed record GridColumnSettingItem(
    string FieldName,
    string DefaultCaption,
    string Caption,
    bool IsVisible,
    int VisibleIndex,
    int Width);
