using Microsoft.Extensions.Diagnostics.HealthChecks;
using NuanSystem.Application.Abstractions.Data;

namespace NuanSystem.Api.Health;

public sealed class MasterDatabaseHealthCheck(IMasterConnectionFactory connectionFactory) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1;";
            _ = command.ExecuteScalar();
            return Task.FromResult(HealthCheckResult.Healthy("Master database available."));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Master database unavailable.", exception));
        }
    }
}
