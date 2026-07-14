namespace NuanSystem.Persistence.Options;

public sealed class SqlConnectionPolicyOptions
{
    public const string SectionName = "SqlConnectionPolicy";

    public bool Encrypt { get; set; } = true;

    public bool TrustServerCertificate { get; set; }
}
