using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NuanSystem.Api.Endpoints;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Application.Features.BusinessPartners.SapCodes;
using NuanSystem.Application.Common.Models;
using NuanSystem.Persistence.DependencyInjection;
using NuanSystem.Persistence.Repositories;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSapCodePolicyApiContractTests
{
    private const string Route = "/api/sap/settings/business-partner-codes";

    [Fact]
    public void Endpoints_ExposeGetAndPutWithSeparateSapPermissions()
    {
        var endpoints = BuildEndpoints(Substitute.For<ISender>());
        var policyEndpoints = endpoints
            .Where(endpoint => endpoint.RoutePattern.RawText == Route)
            .ToArray();

        policyEndpoints.Should().HaveCount(2);
        Permission(policyEndpoints, "GET").Should().Be(PermissionCodes.SapRead);
        Permission(policyEndpoints, "PUT").Should().Be(PermissionCodes.SapManage);
    }

    [Fact]
    public async Task Put_UsesClaimsForAuditIgnoresBodyAuditAndPropagatesCancellation()
    {
        var sender = Substitute.For<ISender>();
        UpdateBusinessPartnerSapCodePolicyCommand? captured = null;
        CancellationToken capturedToken = default;
        sender.Send(
                Arg.Do<UpdateBusinessPartnerSapCodePolicyCommand>(command => captured = command),
                Arg.Do<CancellationToken>(token => capturedToken = token))
            .Returns(Result<BusinessPartnerSapCodePolicyDto>.Success(PolicyDto()));
        var endpoint = Endpoint(BuildEndpoints(sender), "PUT");
        using var cancellation = new CancellationTokenSource();
        var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton(sender)
            .BuildServiceProvider();
        var context = new DefaultHttpContext
        {
            RequestServices = services,
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "81"),
                    new Claim(ClaimTypes.Name, "claims-user")
                ],
                "test"))
        };
        context.Request.Method = "PUT";
        context.Request.ContentType = "application/json";
        context.RequestAborted = cancellation.Token;
        var requestBody = Encoding.UTF8.GetBytes(
            """
            {
              "isEnabled": true,
              "prefixMode": "RoleOnly",
              "passportIdentificationTypeCode": "PASSPORT",
              "expectedRowVersion": null,
              "auditUserId": 999,
              "auditUserName": "body-user",
              "companyId": 999,
              "password": "must-not-bind"
            }
            """);
        context.Request.ContentLength = requestBody.Length;
        context.Request.Body = new MemoryStream(requestBody);
        context.Features.Set<IHttpRequestBodyDetectionFeature>(new RequestBodyDetectionFeature());
        context.Response.Body = new MemoryStream();

        await endpoint.RequestDelegate!(context);

        context.Response.Body.Position = 0;
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        captured.Should().NotBeNull(
            $"the endpoint returned HTTP {context.Response.StatusCode}: {responseBody}");
        captured!.AuditUserId.Should().Be(81);
        captured.AuditUserName.Should().Be("claims-user");
        captured.GetType().GetProperty("CompanyId").Should().BeNull();
        capturedToken.Should().Be(cancellation.Token);
    }

    [Fact]
    public void PublicContracts_HaveExactSafeShapeAndBase64RowVersion()
    {
        typeof(BusinessPartnerSapCodePolicyDto).GetProperties()
            .Select(property => property.Name)
            .Should().Equal(
                "CompanyId",
                "IsEnabled",
                "PrefixMode",
                "PassportIdentificationTypeCode",
                "CustomerNationalExample",
                "CustomerForeignExample",
                "SupplierNationalExample",
                "SupplierForeignExample",
                "RowVersion");

        var commandJson = JsonSerializer.Serialize(new UpdateBusinessPartnerSapCodePolicyCommand(
            true, "RoleOnly", "PASSPORT", "AQID", 81, "tester"));
        using var document = JsonDocument.Parse(commandJson);
        document.RootElement.EnumerateObject().Select(property => property.Name)
            .Should().BeEquivalentTo(
                "IsEnabled",
                "PrefixMode",
                "PassportIdentificationTypeCode",
                "ExpectedRowVersion");
        commandJson.Contains("password", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        commandJson.Contains("AuditUser", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        commandJson.Contains("CompanyId", StringComparison.OrdinalIgnoreCase).Should().BeFalse();

        var dtoJson = JsonSerializer.Serialize(PolicyDto());
        dtoJson.Should().Contain("\"RowVersion\":\"AQIDBAUGBwg=\"");
        dtoJson.Contains("password", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public void DependencyInjection_RegistersApplicationAndMasterRepositoryWithoutReplacingExistingServices()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices();
        services.AddPersistenceServices(new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build());

        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IBusinessPartnerSapCodePolicyRepository)
            && descriptor.ImplementationType == typeof(BusinessPartnerSapCodePolicyRepository));
        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(NuanSystem.Application.Abstractions.Data.IBusinessPartnerRepository));
        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(IRequestHandler<
            GetBusinessPartnerSapCodePolicyQuery,
            Result<BusinessPartnerSapCodePolicyDto>>));
    }

    [Fact]
    public void Persistence_UsesOnlyMasterFactoryAndProceduresFromMigration229()
    {
        BusinessPartnerSapCodePolicyRepository.GetByCompanyIdProcedure.Should().Be(
            "dbo.SP_NA_GET_BUSINESSPARTNERSAPCODEPOLICY_BUSCARPOREMPRESAID");
        BusinessPartnerSapCodePolicyRepository.SaveProcedure.Should().Be(
            "dbo.SP_NA_PUT_BUSINESSPARTNERSAPCODEPOLICY_GUARDAR");
        typeof(BusinessPartnerSapCodePolicyRepository).GetConstructors()
            .Should().ContainSingle()
            .Which.GetParameters().Select(parameter => parameter.ParameterType)
            .Should().Equal(typeof(NuanSystem.Application.Abstractions.Data.IMasterConnectionFactory));
    }

    private static IReadOnlyCollection<RouteEndpoint> BuildEndpoints(ISender sender)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(sender);
        var app = builder.Build();
        app.MapSapEndpoints();
        return ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .ToArray();
    }

    private static RouteEndpoint Endpoint(IEnumerable<RouteEndpoint> endpoints, string method) =>
        endpoints.Single(endpoint =>
            endpoint.RoutePattern.RawText == Route
            && endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()!.HttpMethods.Single() == method);

    private static string? Permission(IEnumerable<RouteEndpoint> endpoints, string method) =>
        Endpoint(endpoints, method).Metadata
            .GetOrderedMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>()
            .Single().Policy;

    private static BusinessPartnerSapCodePolicyDto PolicyDto() => new(
        10,
        true,
        "RoleOnly",
        "PASSPORT",
        "C0999999999001",
        "CAB123",
        "P0999999999001",
        "PAB123",
        "AQIDBAUGBwg=");

    private sealed class RequestBodyDetectionFeature : IHttpRequestBodyDetectionFeature
    {
        public bool CanHaveBody => true;
    }
}
