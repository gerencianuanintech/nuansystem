namespace NuanSystem.Api.Options;

public static class ProductionConfigurationValidator
{
    public static void Validate(IConfiguration configuration, IHostEnvironment environment)
    {
        if (!environment.IsProduction())
        {
            return;
        }

        Require(configuration.GetConnectionString("SqlServerAdmin"), "ConnectionStrings:SqlServerAdmin");
        Require(configuration["Security:EncryptionKey"], "Security:EncryptionKey");
        Require(configuration["Jwt:SigningKey"], "Jwt:SigningKey");

        var allowedHosts = configuration["AllowedHosts"];
        if (string.IsNullOrWhiteSpace(allowedHosts) || allowedHosts.Trim() == "*")
        {
            throw new InvalidOperationException("AllowedHosts debe declarar hosts explicitos en Production.");
        }

        if (configuration.GetValue<bool>("DatabaseInitialization:InitializeMasterOnStartup"))
        {
            throw new InvalidOperationException("InitializeMasterOnStartup debe permanecer false en Production.");
        }
    }

    private static void Require(string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"La configuracion segura {key} es obligatoria en Production.");
        }
    }
}
