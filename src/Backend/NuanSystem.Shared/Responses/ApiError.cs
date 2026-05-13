namespace NuanSystem.Shared.Responses;

public sealed record ApiError(string Code, string Message, string? Field = null);
