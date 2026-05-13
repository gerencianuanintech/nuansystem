namespace NuanSystem.WinForms.Services.Configuration;

public sealed record ApiClientOptions
{
    public string BaseUrl { get; init; } = "http://localhost:5081";
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
}
