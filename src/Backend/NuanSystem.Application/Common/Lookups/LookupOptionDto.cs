namespace NuanSystem.Application.Common.Lookups;

public sealed record LookupOptionDto(
    int Id,
    string Code,
    string Name,
    bool IsActive = true);

public sealed record LookupOptionDto<TValue>(
    TValue Id,
    string Code,
    string Name,
    bool IsActive = true);
