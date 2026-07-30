using System.Security.Claims;
using System.Text.Json;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using NSubstitute;
using NuanSystem.Api.Endpoints;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Profiles.Commands;
using NuanSystem.Application.Features.SapSync.Profiles.Queries;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncProfileApiContractTests
{
    [Fact]
    public void Endpoints_ExposeOnlyApprovedRoutesWithIndependentPermissions()
    {
        var endpoints = BuildEndpoints();
        var expected = new Dictionary<(string Method, string Route), string>
        {
            [("GET", "/api/sap/sync-profiles")] = PermissionCodes.SapSyncProfilesView,
            [("GET", "/api/sap/sync-profiles/{id:long}")] = PermissionCodes.SapSyncProfilesView,
            [("GET", "/api/sap/sync-profiles/catalog")] = PermissionCodes.SapSyncProfilesView,
            [("POST", "/api/sap/sync-profiles")] = PermissionCodes.SapSyncProfilesCreate,
            [("PUT", "/api/sap/sync-profiles/{id:long}")] = PermissionCodes.SapSyncProfilesEdit,
            [("DELETE", "/api/sap/sync-profiles/{id:long}")] = PermissionCodes.SapSyncProfilesDelete,
            [("POST", "/api/sap/sync-profiles/{id:long}/validate")] = PermissionCodes.SapSyncProfilesValidate,
            [("POST", "/api/sap/sync-profiles/{id:long}/activate")] = PermissionCodes.SapSyncProfilesActivate,
            [("POST", "/api/sap/sync-profiles/{id:long}/deactivate")] = PermissionCodes.SapSyncProfilesActivate
        };

        endpoints.Should().HaveCount(expected.Count);
        foreach (var endpoint in endpoints)
        {
            var route = endpoint.RoutePattern.RawText!;
            var method = endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()!.HttpMethods.Single();
            expected.Should().ContainKey((method, route));
            endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>()
                .Should().ContainSingle()
                .Which.Policy.Should().Be(expected[(method, route)]);
        }

        endpoints.Should().OnlyContain(endpoint =>
            endpoint.Metadata.GetMetadata<ITagsMetadata>()!.Tags
                .Contains("SAP Business One - Sync Profiles"));
        endpoints.Should().NotContain(endpoint =>
            endpoint.RoutePattern.RawText!.Contains("execute", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task PermissionPolicy_ChallengesAnonymousForbidsLegacyAndMatrixAndAllowsExactClaim()
    {
        var permission = PermissionCodes.SapSyncProfilesView;
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthentication();
        services.AddAuthorization(options =>
            options.AddPolicy(permission, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(AuthClaimNames.Permission, permission)));
        await using var provider = services.BuildServiceProvider();
        var evaluator = provider.GetRequiredService<IPolicyEvaluator>();
        var policy = await provider.GetRequiredService<IAuthorizationPolicyProvider>()
            .GetPolicyAsync(permission);
        var context = new DefaultHttpContext { RequestServices = provider };
        context.User = new ClaimsPrincipal(new ClaimsIdentity());

        var anonymous = await evaluator.AuthorizeAsync(
            policy!,
            AuthenticateResult.NoResult(),
            context,
            resource: null);
        anonymous.Challenged.Should().BeTrue("una solicitud no autenticada debe producir 401");

        foreach (var substitutePermission in new[]
                 {
                     PermissionCodes.SapRead,
                     PermissionCodes.SapManage,
                     PermissionCodes.SyncConfigurationView,
                     PermissionCodes.SyncConfigurationEdit
                 })
        {
            var principal = Principal(substitutePermission);
            context.User = principal;
            var authentication = AuthenticateResult.Success(
                new AuthenticationTicket(principal, "test"));
            var denied = await evaluator.AuthorizeAsync(policy!, authentication, context, null);
            denied.Forbidden.Should().BeTrue(
                $"el permiso {substitutePermission} no debe autorizar perfiles SAP");
        }

        var allowedPrincipal = Principal(permission);
        context.User = allowedPrincipal;
        var allowed = await evaluator.AuthorizeAsync(
            policy!,
            AuthenticateResult.Success(new AuthenticationTicket(allowedPrincipal, "test")),
            context,
            null);
        allowed.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Contracts_UseBase64RowVersionAndExposeNoSensitiveConfiguration()
    {
        var dto = new SapSyncProfileDto(
            1,
            1,
            "DEMO",
            "Demo",
            "SAP-DEMO",
            "SAP Demo",
            null,
            false,
            1,
            "tester",
            DateTime.UtcNow,
            null,
            null,
            null,
            [1, 2, 3, 4, 5, 6, 7, 8],
            [
                new SapSyncProfileEntityDto(
                    1,
                    "Suppliers",
                    SapSyncDirection.SapToErp.ToString(),
                    SapSyncModes.Full,
                    100,
                    3,
                    1,
                    true,
                    30,
                    false,
                    new SapSyncScheduleDto(
                        1,
                        SapSyncScheduleTypes.Manual,
                        null,
                        null,
                        "America/Guayaquil",
                        true,
                        false,
                        null,
                        null,
                        null,
                        null,
                        [1, 2, 3, 4, 5, 6, 7, 8]),
                    [1, 2, 3, 4, 5, 6, 7, 8])
            ]);

        var json = JsonSerializer.Serialize(dto);

        json.Should().Contain("\"RowVersion\":\"AQIDBAUGBwg=\"");
        foreach (var forbidden in new[]
                 {
                     "Password", "ConnectionString", "ServiceLayerUrl", "SapUser",
                     "Cookie", "B1SESSION", "ROUTEID", "Authorization", "Session"
                 })
        {
            json.Contains(forbidden, StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        }
    }

    [Theory]
    [InlineData(SapSyncProfileErrorCodes.CompanyAccessDenied, StatusCodes.Status403Forbidden)]
    [InlineData(SapSyncProfileErrorCodes.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(SapSyncProfileErrorCodes.DuplicateCode, StatusCodes.Status409Conflict)]
    [InlineData(SapSyncProfileErrorCodes.ConcurrencyConflict, StatusCodes.Status409Conflict)]
    [InlineData(SapSyncProfileErrorCodes.CompanyImmutable, StatusCodes.Status409Conflict)]
    [InlineData(SapSyncProfileErrorCodes.UnsupportedCapability, StatusCodes.Status400BadRequest)]
    public void HttpMapping_ReturnsStableStatusCodes(string errorCode, int expectedStatus)
    {
        var mapper = typeof(SapSyncProfileEndpoints)
            .GetMethod(
                "ToSapSyncProfileHttpResult",
                BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(bool));
        var failure = Result<bool>.Failure(
            "Rejected",
            [new ApiError(errorCode, "Safe message")]);

        var response = (IResult)mapper.Invoke(null, [failure])!;

        response.Should().BeAssignableTo<IStatusCodeHttpResult>()
            .Which.StatusCode.Should().Be(expectedStatus);
    }

    [Fact]
    public void ApiAndApplication_DoNotExposeExecutionOrReuseMatrixBranchContracts()
    {
        var endpoints = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "SapSyncProfileEndpoints.cs");
        var handlers = Read(
            "src", "Backend", "NuanSystem.Application", "Features", "SapSync", "Profiles",
            "Commands", "SapSyncProfileCommandHandlers.cs");
        var validation = Read(
            "src", "Backend", "NuanSystem.Application", "Features", "SapSync", "Profiles",
            "Services", "SapSyncProfileValidationService.cs");
        var program = Read("src", "Backend", "NuanSystem.Api", "Program.cs");

        endpoints.Should().NotContain("/execute")
            .And.NotContain("SapSyncProfilesExecute")
            .And.NotContain("PermissionCodes.SapRead")
            .And.NotContain("PermissionCodes.SapManage")
            .And.NotContain("PermissionCodes.SyncConfiguration");
        handlers.Should().NotContain("ISyncProfileRepository")
            .And.NotContain("SyncProfileExecution")
            .And.NotContain("ISapClient")
            .And.NotContain("ServiceLayer");
        validation.Should().NotContain("ISapClient")
            .And.NotContain("ISapCompanySettingsRepository")
            .And.NotContain("ServiceLayer")
            .And.NotContain("SRI")
            .And.NotContain("Worker");
        program.Should().Contain("app.MapSapSyncProfileEndpoints()");
    }

    [Fact]
    public void Validators_RejectMalformedRowVersionAndOutOfRangeLimits()
    {
        var request = new SaveSapSyncProfileRequest(
            1,
            "SAP",
            "SAP",
            null,
            [
                new SaveSapSyncProfileEntityRequest(
                    null,
                    "Suppliers",
                    "SapToErp",
                    "Full",
                    0,
                    0,
                    -1,
                    true,
                    0,
                    true,
                    new SaveSapSyncScheduleRequest(
                        null,
                        "Manual",
                        null,
                        null,
                        null,
                        true,
                        false),
                    [1])
            ]);
        var validator = new CreateSapSyncProfileCommandValidator();

        var result = validator.Validate(
            new CreateSapSyncProfileCommand(request, 1, 1, "tester"));

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.ErrorCode).Should().Contain(
            "SAP_SYNC_PROFILE_BATCH_SIZE_RANGE",
            "SAP_SYNC_PROFILE_MAX_ATTEMPTS_RANGE",
            "SAP_SYNC_PROFILE_EXECUTION_ORDER_RANGE",
            "SAP_SYNC_PROFILE_TIMEOUT_RANGE",
            "SAP_SYNC_PROFILE_ROW_VERSION_INVALID");
    }

    private static IReadOnlyCollection<RouteEndpoint> BuildEndpoints()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Substitute.For<ISender>());
        var app = builder.Build();
        app.MapSapSyncProfileEndpoints();
        return ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .ToArray();
    }

    private static ClaimsPrincipal Principal(string permission) =>
        new(new ClaimsIdentity(
            [new Claim(AuthClaimNames.Permission, permission)],
            authenticationType: "test"));

    private static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine([Root(), .. parts]));

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
        {
            directory = directory.Parent;
        }
        return directory?.FullName
               ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
