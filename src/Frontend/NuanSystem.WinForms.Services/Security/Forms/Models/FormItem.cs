namespace NuanSystem.WinForms.Services.Security.Forms.Models;

public sealed record FormItem(
    int Id,
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    string FormTypeName,
    bool HasListView,
    bool HasEditView,
    bool IsVisible,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName,
    DateTime CreatedAt,
    int? UpdatedByUserId,
    string? UpdatedByUserName,
    DateTime? UpdatedAt,
    int? DeletedByUserId,
    string? DeletedByUserName,
    DateTime? DeletedAt);
