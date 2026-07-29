using System.Text.Json;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TenantConfiguration;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SriDocuments.Services;

public sealed class SriDocumentQueuePolicy(ITenantFeatureService featureService, ITenantIntegrationService integrationService) : ISriDocumentQueuePolicy
{
    public async Task<Result<bool>> ValidateEnqueueAsync(string environment, CancellationToken cancellationToken = default)
    {
        var features = await featureService.GetActiveCompanyFeaturesAsync(cancellationToken);
        if (!features.IsSuccess) return Result<bool>.Failure(features.Message, features.Errors);
        if (features.Value!.All(item => !item.FeatureCode.Equals(TenantFeatureCodes.SriDocuments, StringComparison.OrdinalIgnoreCase) || !item.IsEnabled))
            return Failure("SRI_FEATURE_DISABLED", "La capacidad de documentos SRI no esta habilitada para la empresa activa.");

        var integrations = await integrationService.GetActiveCompanyIntegrationsAsync(cancellationToken);
        if (!integrations.IsSuccess) return Result<bool>.Failure(integrations.Message, integrations.Errors);
        var integration = integrations.Value!.FirstOrDefault(item => item.IntegrationCode.Equals(TenantIntegrationCodes.Sri, StringComparison.OrdinalIgnoreCase));
        if (integration is null || !integration.IsEnabled)
            return Failure("SRI_INTEGRATION_DISABLED", "La integracion SRI no esta habilitada para la empresa activa.");

        var configuredEnvironment = ReadEnvironment(integration.ConfigurationJson);
        if (configuredEnvironment is null)
            return Failure("SRI_ENVIRONMENT_NOT_CONFIGURED", "La integracion SRI no tiene un ambiente valido configurado.");
        if (!configuredEnvironment.Equals(SriEnvironmentCodes.Normalize(environment), StringComparison.Ordinal))
            return Failure("SRI_ENVIRONMENT_MISMATCH", "El ambiente solicitado no coincide con la configuracion SRI de la empresa activa.");
        return Result<bool>.Success(true);
    }

    private static string? ReadEnvironment(string? configurationJson)
    {
        if (string.IsNullOrWhiteSpace(configurationJson)) return null;
        try
        {
            using var document = JsonDocument.Parse(configurationJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object) return null;
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (property.Name.Equals("environment", StringComparison.OrdinalIgnoreCase) && property.Value.ValueKind == JsonValueKind.String && SriEnvironmentCodes.IsValid(property.Value.GetString()))
                    return SriEnvironmentCodes.Normalize(property.Value.GetString()!);
            }
        }
        catch (JsonException) { return null; }
        return null;
    }

    private static Result<bool> Failure(string code, string message) => Result<bool>.Failure(message, [new ApiError(code, message)]);
}
