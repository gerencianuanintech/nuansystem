namespace NuanSystem.Persistence.Options;

public sealed class MasterDatabaseOptions
{
    public const string SectionName = "MasterDatabase";

    public string DatabaseName { get; set; } = "NuanSystem_Master";
}
