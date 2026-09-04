using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NuanSystem.Api.Endpoints;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.SyncConflicts;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Persistence.DependencyInjection;
using NuanSystem.Persistence.Repositories.Sync;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSyncConflictApiContractTests
{
    private const string ListRoute = "/api/sync/business-partner-conflicts";
    private const string ResolveRoute = "/api/sync/business-partner-conflicts/{id:long}/resolve";

    [Fact]
    public void Endpoints_ExposeGetAndPostWithSeparatePermissions()
    {
        var endpoints = BuildEndpoints(Substitute.For<ISender>());

        Permission(endpoints, ListRoute, "GET").Should().Be(
            PermissionCodes.BusinessPartnerSyncConflictsView);
        Permission(endpoints, ResolveRoute, "POST").Should().Be(
            PermissionCodes.BusinessPartnerSyncConflictsResolve);
    }

    [Fact]
    public async Task Resolve_UsesRouteIdAndClaimsAuditInsteadOfBodyValues()
    {
        var sender = Substitute.For<ISender>();
        ResolveBusinessPartnerSyncConflictCommand? captured = null;
        sender.Send(
                Arg.Do<ResolveBusinessPartnerSyncConflictCommand>(command => captured = command),
                Arg.Any<CancellationToken>())
            .Returns(Result<BusinessPartnerSyncConflictDto>.Failure("expected test result"));
        var endpoint = Endpoint(BuildEndpoints(sender), ResolveRoute, "POST");
        var services = new ServiceCollection().AddLogging().AddSingleton(sender).BuildServiceProvider();
        var context = new DefaultHttpContext
        {
            RequestServices = services,
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "7"), new Claim(ClaimTypes.Name, "claims-admin")],
                "test"))
        };
        context.Request.Method = "POST";
        context.Request.RouteValues["id"] = "81";
        context.Request.ContentType = "application/json";
        var bytes = Encoding.UTF8.GetBytes(
            """
            {"conflictId":999,"resolution":"KeepCentral","reason":"approved","expectedRowVersion":"AQIDBAUGBwg=","auditUserId":999,"auditUserName":"body-user"}
            """);
        context.Request.ContentLength = bytes.Length;
        context.Request.Body = new MemoryStream(bytes);
        context.Features.Set<IHttpRequestBodyDetectionFeature>(new RequestBodyDetectionFeature());
        context.Response.Body = new MemoryStream();

        await endpoint.RequestDelegate!(context);

        captured.Should().NotBeNull();
        captured!.ConflictId.Should().Be(81);
        captured.AuditUserId.Should().Be(7);
        captured.AuditUserName.Should().Be("claims-admin");
    }

    [Fact]
    public void PublicDto_DoesNotExposeTechnicalSnapshotsOrPayloads()
    {
        typeof(BusinessPartnerSyncConflictDto).GetProperties().Select(property => property.Name)
            .Should().NotContain(name => name.Contains("Snapshot", StringComparison.OrdinalIgnoreCase)
                || name.Contains("Payload", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("Json", StringComparison.OrdinalIgnoreCase));
        typeof(BusinessPartnerSyncConflictDto).GetProperty("Differences").Should().NotBeNull();
        typeof(ResolveBusinessPartnerSyncConflictCommand).GetProperty("CompanyId").Should().BeNull();
    }

    [Fact]
    public void PersistenceRegistration_PreservesExistingRepositoriesAndAddsConflictRepository()
    {
        var services = new ServiceCollection();
        services.AddPersistenceServices(new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build());

        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IBusinessPartnerSyncConflictRepository)
            && descriptor.ImplementationType == typeof(BusinessPartnerSyncConflictRepository));
        services.Should().Contain(descriptor =>
            descriptor.ServiceType == typeof(IBusinessPartnerProposalApplyRepository));
        BusinessPartnerSyncConflictRepository.ListProcedure.Should().Be(
            "dbo.SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICTS_LISTAR");
        BusinessPartnerSyncConflictRepository.GetByIdProcedure.Should().Be(
            "dbo.SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICT_BUSCARPORID");
        BusinessPartnerSyncConflictRepository.ResolveProcedure.Should().Be(
            "dbo.SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER");
        BusinessPartnerSyncConflictRepository.StableReferencesProcedure.Should().Be(
            "dbo.SP_NA_GET_BUSINESSPARTNER_STABLE_REFERENCES_RESOLVE");
    }

    [Fact]
    public void PersistenceProjection_DeserializesSnapshotsPathsAndOpaqueRowVersion()
    {
        var snapshot = new BusinessPartnerCanonicalSnapshot(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "BP-AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
            "Central",
            null,
            "Customer",
            "RUC",
            "0999999999001",
            "0999999999001",
            null,
            null,
            "CN0999999999001",
            true,
            [],
            []);
        var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var row = new BusinessPartnerSyncConflictRepository.ConflictRow
        {
            Id = 81,
            ProposalEventId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            BusinessPartnerId = 9,
            BusinessPartnerGlobalId = snapshot.GlobalId,
            OriginCompanyId = 20,
            BaseCanonicalVersion = 4,
            CurrentCanonicalVersion = 5,
            BaseSnapshotJson = json,
            ProposedSnapshotJson = json,
            CanonicalSnapshotJson = json,
            ConflictFieldsJson = "[\"Name\"]",
            Status = "Open",
            CreatedAt = new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc),
            RowVersion = [1, 2, 3, 4, 5, 6, 7, 8],
            Code = snapshot.Code,
            Name = snapshot.Name
        };

        var record = BusinessPartnerSyncConflictRepository.ToRecord(row);

        record.Base.Should().BeEquivalentTo(snapshot);
        record.Proposed.Should().BeEquivalentTo(snapshot);
        record.Canonical.Should().BeEquivalentTo(snapshot);
        record.ConflictFields.Should().Equal("Name");
        Convert.ToBase64String(record.RowVersion).Should().Be("AQIDBAUGBwg=");
    }

    private static IReadOnlyCollection<RouteEndpoint> BuildEndpoints(ISender sender)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(sender);
        var app = builder.Build();
        app.MapSyncEndpoints();
        return ((IEndpointRouteBuilder)app).DataSources.SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>().ToArray();
    }

    private static RouteEndpoint Endpoint(
        IEnumerable<RouteEndpoint> endpoints,
        string route,
        string method) => endpoints.Single(endpoint =>
        endpoint.RoutePattern.RawText == route
        && endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()!.HttpMethods.Single() == method);

    private static string? Permission(
        IEnumerable<RouteEndpoint> endpoints,
        string route,
        string method) => Endpoint(endpoints, route, method).Metadata
        .GetOrderedMetadata<Microsoft.AspNetCore.Authorization.IAuthorizeData>()
        .Single().Policy;

    private sealed class RequestBodyDetectionFeature : IHttpRequestBodyDetectionFeature
    {
        public bool CanHaveBody => true;
    }
}
