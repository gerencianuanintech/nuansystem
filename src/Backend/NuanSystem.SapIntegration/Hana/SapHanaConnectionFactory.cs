using System.Data.Common;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.SapIntegration.Hana;

public sealed class SapHanaConnectionFactory(
    ISapCompanySettingsRepository settingsRepository,
    ISecretProtector secretProtector) : ISapHanaConnectionFactory
{
    private const string HanaProviderInvariantName = "Sap.Data.Hana";

    public async Task<DbConnection> CreateOpenConnectionAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var settings = await settingsRepository.GetByCompanyIdAsync(companyId, cancellationToken);
        if (settings is null || !settings.IsEnabled || settings.IntegrationMode == SapIntegrationMode.None)
        {
            throw new InvalidOperationException("La empresa no tiene integracion SAP activa.");
        }

        if (string.IsNullOrWhiteSpace(settings.HanaServer)
            || string.IsNullOrWhiteSpace(settings.HanaSchema)
            || string.IsNullOrWhiteSpace(settings.HanaUser)
            || string.IsNullOrWhiteSpace(settings.HanaPasswordEncrypted))
        {
            throw new InvalidOperationException("La configuracion HANA de la empresa esta incompleta.");
        }

        var factory = CreateProviderFactory();
        var connection = factory.CreateConnection()
            ?? throw new InvalidOperationException("No se pudo crear la conexion HANA.");

        // The password is decrypted only while opening the technical HANA connection.
        connection.ConnectionString = BuildConnectionString(
            settings.HanaServer,
            settings.HanaPort,
            settings.HanaSchema,
            settings.HanaUser,
            secretProtector.Unprotect(settings.HanaPasswordEncrypted));

        await connection.OpenAsync(cancellationToken);

        return connection;
    }

    private static DbProviderFactory CreateProviderFactory()
    {
        try
        {
            return DbProviderFactories.GetFactory(HanaProviderInvariantName);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(
                "El proveedor ADO.NET de SAP HANA no esta registrado. Instale y registre Sap.Data.Hana en el servidor de la API.",
                exception);
        }
    }

    private static string BuildConnectionString(
        string server,
        int? port,
        string schema,
        string user,
        string password)
    {
        var builder = new DbConnectionStringBuilder
        {
            ["Server"] = port.HasValue ? $"{server}:{port.Value}" : server,
            ["UserID"] = user,
            ["Password"] = password,
            ["Current Schema"] = schema
        };

        return builder.ConnectionString;
    }
}
