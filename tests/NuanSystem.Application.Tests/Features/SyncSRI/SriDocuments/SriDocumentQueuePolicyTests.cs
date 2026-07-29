using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriDocuments.Services;
using NuanSystem.Application.Features.TenantConfiguration;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;

namespace NuanSystem.Application.Tests.Features.SriDocuments;

public sealed class SriDocumentQueuePolicyTests
{
    private readonly ITenantFeatureService _features = Substitute.For<ITenantFeatureService>();
    private readonly ITenantIntegrationService _integrations = Substitute.For<ITenantIntegrationService>();

    [Fact]
    public async Task Validate_AllowsEnabledFeatureIntegrationAndMatchingEnvironment()
    {
        _features.GetActiveCompanyFeaturesAsync(Arg.Any<CancellationToken>()).Returns(Result<IReadOnlyCollection<TenantFeatureDto>>.Success(
            [new TenantFeatureDto(TenantFeatureCodes.SriDocuments, true, null, null)]));
        _integrations.GetActiveCompanyIntegrationsAsync(Arg.Any<CancellationToken>()).Returns(Result<IReadOnlyCollection<TenantIntegrationDto>>.Success(
            [new TenantIntegrationDto(TenantIntegrationCodes.Sri, true, "{\"environment\":\"Production\"}", null, null)]));

        var result = await new SriDocumentQueuePolicy(_features, _integrations).ValidateEnqueueAsync("Production");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_RejectsConfiguredEnvironmentMismatch()
    {
        _features.GetActiveCompanyFeaturesAsync(Arg.Any<CancellationToken>()).Returns(Result<IReadOnlyCollection<TenantFeatureDto>>.Success(
            [new TenantFeatureDto(TenantFeatureCodes.SriDocuments, true, null, null)]));
        _integrations.GetActiveCompanyIntegrationsAsync(Arg.Any<CancellationToken>()).Returns(Result<IReadOnlyCollection<TenantIntegrationDto>>.Success(
            [new TenantIntegrationDto(TenantIntegrationCodes.Sri, true, "{\"environment\":\"Test\"}", null, null)]));

        var result = await new SriDocumentQueuePolicy(_features, _integrations).ValidateEnqueueAsync("Production");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "SRI_ENVIRONMENT_MISMATCH");
    }
}
