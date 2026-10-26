namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal readonly record struct Phase7SqlGates(bool StandardEnabled, bool IsolatedEnabled)
{
    public const string IsolatedEnvironmentVariable = "NUANSYSTEM_RUN_PHASE7_SQL_TESTS";

    public static Phase7SqlGates FromValues(string? standard, string? isolated) => new(
        SqlServerIntegrationFactAttribute.IsEnabled(standard),
        SqlServerIntegrationFactAttribute.IsEnabled(isolated));

    // Reads only the two enablement flags. It does not read credentials or product settings.
    public static Phase7SqlGates ReadEnvironment() => FromValues(
        Environment.GetEnvironmentVariable(SqlServerIntegrationFactAttribute.EnabledEnvironmentVariable),
        Environment.GetEnvironmentVariable(IsolatedEnvironmentVariable));
}
