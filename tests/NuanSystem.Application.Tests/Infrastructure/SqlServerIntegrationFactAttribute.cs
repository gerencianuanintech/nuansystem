namespace NuanSystem.Application.Tests.Infrastructure;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SqlServerIntegrationFactAttribute : FactAttribute
{
    public const string EnabledEnvironmentVariable = "NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS";

    public SqlServerIntegrationFactAttribute()
    {
        var configuredValue = Environment.GetEnvironmentVariable(EnabledEnvironmentVariable);
        if (!IsEnabled(configuredValue))
        {
            Skip = $"Requiere SQL Server de integracion. Establezca {EnabledEnvironmentVariable}=1 para ejecutar.";
        }
    }

    private static bool IsEnabled(string? value)
    {
        return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
