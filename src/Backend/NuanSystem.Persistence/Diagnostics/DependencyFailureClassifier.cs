using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Diagnostics;
using NuanSystem.Application.Common.Exceptions;

namespace NuanSystem.Persistence.Diagnostics;

public sealed class DependencyFailureClassifier : IDependencyFailureClassifier
{
    public DependencyFailureException? Classify(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is DependencyFailureException dependencyFailure) return dependencyFailure;
            if (current is SqlException sqlException) return ClassifySql(sqlException);
            if (current is TimeoutException or TaskCanceledException)
                return new(DependencyKind.Timeout, "DEPENDENCY_TIMEOUT", "La dependencia no respondio a tiempo.", true);
            if (current is HttpRequestException)
                return new(DependencyKind.Network, "DEPENDENCY_NETWORK_UNAVAILABLE", "No fue posible conectar con el servicio externo.", true);
            if (current is InvalidOperationException invalidOperation
                && invalidOperation.Message.Contains("ConnectionStrings:", StringComparison.OrdinalIgnoreCase))
                return new(DependencyKind.Database, "DATABASE_CONFIGURATION_MISSING", "La conexion a la base de datos no esta configurada.", false);
        }

        return null;
    }

    private static DependencyFailureException ClassifySql(SqlException exception)
    {
        var numbers = exception.Errors.Cast<SqlError>().Select(error => error.Number).ToHashSet();
        var diagnostic = $"SQL error numbers: {string.Join(',', numbers.Order())}";
        if (numbers.Overlaps([-2, 2, 53, 64, 233, 10053, 10054, 10060, 10061, 11001]))
            return new(DependencyKind.Database, "DATABASE_UNAVAILABLE", "No se pudo conectar con la base de datos.", true, diagnostic);
        if (numbers.Contains(18456))
            return new(DependencyKind.Database, "DATABASE_LOGIN_FAILED", "La base de datos rechazo las credenciales configuradas.", false, diagnostic);
        if (numbers.Contains(4060))
            return new(DependencyKind.Database, "DATABASE_NOT_AVAILABLE", "La base de datos configurada no esta disponible.", true, diagnostic);
        return new(DependencyKind.Database, "DATABASE_ERROR", "Ocurrio un error de base de datos.", false, diagnostic);
    }
}
