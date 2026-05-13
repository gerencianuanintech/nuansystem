namespace NuanSystem.Application.Features.GridColumnSettings.Dtos;

public sealed record GridColumnSettingDto(
    string FieldName,
    string DefaultCaption,
    string Caption,
    bool IsVisible,
    int VisibleIndex,
    int Width);
