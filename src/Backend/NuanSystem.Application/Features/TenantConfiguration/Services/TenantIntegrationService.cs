using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NuanSystem.Application.Features.TenantConfiguration.Services;

public sealed class TenantIntegrationService(
    ITenantIntegrationRepository repository,
    ICompanyContext companyContext) : ITenantIntegrationService
{
    public async Task<Result<IReadOnlyCollection<TenantIntegrationDto>>> GetActiveCompanyIntegrationsAsync(
        CancellationToken cancellationToken = default)
    {
        if (companyContext.CurrentCompany is null)
        {
            return Result<IReadOnlyCollection<TenantIntegrationDto>>.Failure("Debe seleccionar una empresa.");
        }

        var configuredIntegrations = await repository.GetByCompanyIdAsync(
            companyContext.CurrentCompany.CompanyId,
            cancellationToken);
        var configuredByCode = configuredIntegrations.ToDictionary(x => x.IntegrationCode, StringComparer.OrdinalIgnoreCase);

        var integrations = TenantIntegrationCodes.All
            .Select(code => configuredByCode.TryGetValue(code, out var integration)
                ? integration with { ConfigurationJson = RedactConfigurationJson(integration.ConfigurationJson) }
                : new TenantIntegrationDto(code, false, null, null, null))
            .ToArray();

        return Result<IReadOnlyCollection<TenantIntegrationDto>>.Success(integrations);
    }

    private static string? RedactConfigurationJson(string? configurationJson)
    {
        if (string.IsNullOrWhiteSpace(configurationJson))
        {
            return configurationJson;
        }

        var node = JsonNode.Parse(configurationJson);
        if (node is null)
        {
            return null;
        }

        RedactNode(node);
        return node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
    }

    private static void RedactNode(JsonNode node)
    {
        if (node is JsonObject jsonObject)
        {
            foreach (var property in jsonObject.ToArray())
            {
                if (IsSensitiveKey(property.Key))
                {
                    jsonObject[property.Key] = "********";
                    continue;
                }

                if (property.Value is not null)
                {
                    RedactNode(property.Value);
                }
            }

            return;
        }

        if (node is JsonArray jsonArray)
        {
            foreach (var item in jsonArray)
            {
                if (item is not null)
                {
                    RedactNode(item);
                }
            }
        }
    }

    private static bool IsSensitiveKey(string key)
    {
        return key.Contains("password", StringComparison.OrdinalIgnoreCase)
            || key.Contains("secret", StringComparison.OrdinalIgnoreCase)
            || key.Contains("token", StringComparison.OrdinalIgnoreCase)
            || key.Contains("credential", StringComparison.OrdinalIgnoreCase)
            || key.EndsWith("key", StringComparison.OrdinalIgnoreCase);
    }
}
