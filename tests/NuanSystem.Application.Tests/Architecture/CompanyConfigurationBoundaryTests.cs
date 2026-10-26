using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NuanSystem.Api.Middleware;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Companies.Dtos;
using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Queries;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Options;
using NuanSystem.Persistence.Tenancy;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class CompanyConfigurationBoundaryTests
{
    private const string Cipher = "test-encrypted-value";
    private const string Password = "test-password;with=characters";

    [Fact]
    public void Business_company_serializes_only_business_fields()
    {
        var company = new Company { Id = 7, Code = "BRANCH", CommercialName = "Test", IsActive = true,
            OperationMode = CompanyOperationMode.Standalone, IsMaster = false, ParentCompanyId = 1, BranchCode = "01", SyncEnabled = true };
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(company));
        Assert.Equal(new[] { "BranchCode", "Code", "CommercialName", "Id", "IsActive", "IsMaster", "OperationMode", "ParentCompanyId", "SyncEnabled" },
            json.RootElement.EnumerateObject().Select(property => property.Name).OrderBy(name => name, StringComparer.Ordinal));
    }

    [Fact]
    public void Runtime_connection_remains_available_but_is_omitted_from_JSON_and_ToString()
    {
        var company = Info(1) with { ConnectionString = $"Server=private-server;Password={Password};" };
        var json = JsonSerializer.Serialize(company);
        Assert.DoesNotContain("ConnectionString", json);
        Assert.DoesNotContain(Password, json);
        Assert.DoesNotContain("private-server", json);
        Assert.DoesNotContain(Password, company.ToString());
        Assert.DoesNotContain("private-server", company.ToString());
        Assert.Contains(Password, company.ConnectionString);
        Assert.Contains("CompanyId", json);
    }

    public static TheoryData<Type> PublicResponseTypes => new()
    {
        typeof(AuthCompanyDto), typeof(CompanyDto), typeof(ConfigurationCompanyDto), typeof(SapServiceLayerSettingsDto)
    };

    [Theory]
    [MemberData(nameof(PublicResponseTypes))]
    public void Published_company_responses_do_not_declare_secret_properties(Type responseType)
    {
        Assert.DoesNotContain(responseType.GetProperties(), property =>
            property.Name.Contains("Encrypted", StringComparison.OrdinalIgnoreCase)
            || property.Name.Contains("ConnectionString", StringComparison.OrdinalIgnoreCase)
            || property.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) && property.PropertyType != typeof(bool));
    }

    [Fact]
    public void Authentication_response_shape_stays_compatible()
    {
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(new AuthCompanyDto(1, "TEST", "Test", null, null, null)));
        Assert.Equal(new[] { "Code", "CommercialName", "Id", "LogoImage", "LogoImageContentType", "LogoImageFileName" },
            json.RootElement.EnumerateObject().Select(property => property.Name).OrderBy(name => name, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData(null, true, false)]
    [InlineData(1433, true, false)]
    [InlineData(1444, true, true)]
    [InlineData(null, false, false)]
    public void Internal_configuration_preserves_decryption_port_and_TLS_policy(int? port, bool encrypt, bool trust)
    {
        var protector = Substitute.For<ISecretProtector>();
        protector.Unprotect(Cipher).Returns(Password);
        var configuration = new CompanyDatabaseConfiguration(DatabaseEngine.SqlServer, "test-server", port, "TEST-DB", "test-user", Cipher);
        var connection = new SqlConnectionStringBuilder(configuration.BuildConnectionString(protector,
            new SqlConnectionPolicyOptions { Encrypt = encrypt, TrustServerCertificate = trust }));
        Assert.Equal(port is null ? "test-server" : $"test-server,{port}", connection.DataSource);
        Assert.Equal("TEST-DB", connection.InitialCatalog);
        Assert.Equal("test-user", connection.UserID);
        Assert.Equal(Password, connection.Password);
        Assert.Equal(encrypt ? SqlConnectionEncryptOption.Mandatory : SqlConnectionEncryptOption.Optional, connection.Encrypt);
        Assert.Equal(trust, connection.TrustServerCertificate);
        Assert.False(connection.MultipleActiveResultSets);
        protector.Received(1).Unprotect(Cipher);
        Assert.False(typeof(CompanyDatabaseConfiguration).IsVisible);
        Assert.DoesNotContain(Cipher, configuration.ToString());
    }

    [Fact]
    public void Invalid_cipher_is_not_silently_replaced_and_unsupported_provider_remains_unsupported()
    {
        var protector = Substitute.For<ISecretProtector>();
        var original = new InvalidOperationException("Controlled decryption failure");
        protector.Unprotect(Cipher).Returns(_ => throw original);
        var configuration = new CompanyDatabaseConfiguration(DatabaseEngine.SqlServer, "server", null, "db", "user", Cipher);
        Assert.Same(original, Assert.Throws<InvalidOperationException>(() => configuration.BuildConnectionString(protector, new())));
        protector = Substitute.For<ISecretProtector>();
        protector.Unprotect(Cipher).Returns(Password);
        Assert.Throws<NotSupportedException>(() => (configuration with { Engine = DatabaseEngine.MySql }).BuildConnectionString(protector, new()));
    }

    [Fact]
    public void Standalone_company_and_connection_do_not_require_SAP()
    {
        var context = new CompanyContext();
        context.SetCurrentCompany(Info(1));
        Assert.Equal(SapIntegrationMode.None, context.CurrentCompany!.SapIntegrationMode);
        using var connection = new NuanSystem.Persistence.Connections.TenantConnectionFactory(context).CreateConnection();
        Assert.Equal(System.Data.ConnectionState.Closed, connection.State);
        Assert.Equal("DB-1", new SqlConnectionStringBuilder(connection.ConnectionString).InitialCatalog);
    }

    [Fact]
    public async Task Companies_with_same_external_database_keep_separate_settings_and_hide_encrypted_passwords()
    {
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        foreach (var id in new[] { 1, 2 })
            repository.GetByCompanyIdAsync(id, Arg.Any<CancellationToken>()).Returns(new SapCompanySettingsDto
            { CompanyId = id, SapCompanyDb = "SAME-EXTERNAL-DB", SapUser = $"user-{id}", SapPasswordEncrypted = $"{Cipher}-{id}" });
        foreach (var id in new[] { 1, 2 })
        {
            var context = new CompanyContext();
            context.SetCurrentCompany(Info(id));
            var handler = new GetSapServiceLayerSettingsQueryHandler(context, repository);
            var result = await handler.Handle(new GetSapServiceLayerSettingsQuery(), CancellationToken.None);
            Assert.Equal(id, result.Value!.CompanyId);
            Assert.Equal($"user-{id}", result.Value.SapUser);
            Assert.Equal("SAME-EXTERNAL-DB", result.Value.SapCompanyDb);
            Assert.True(result.Value.HasPassword);
            Assert.DoesNotContain(Cipher, JsonSerializer.Serialize(result.Value));
            await repository.Received(1).GetByCompanyIdAsync(id, Arg.Any<CancellationToken>());
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Middleware_preserves_user_scoped_resolution_and_denies_unauthorized_company(bool allowed)
    {
        var http = new DefaultHttpContext();
        http.Request.Path = "/api/items";
        http.Request.Headers[CompanyContextMiddleware.CompanyCodeHeaderName] = " TEST-1 ";
        http.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "42")], "Test"));
        http.Response.Body = new MemoryStream();
        var resolver = Substitute.For<ICompanyResolver>();
        resolver.ResolveByCodeForUserAsync("TEST-1", 42, Arg.Any<CancellationToken>()).Returns(allowed ? Info(1) : null);
        var context = new CompanyContext();
        var called = false;
        var middleware = new CompanyContextMiddleware(_ => { called = true; return Task.CompletedTask; }, NullLogger<CompanyContextMiddleware>.Instance);
        await middleware.InvokeAsync(http, resolver, context);
        Assert.Equal(allowed, called);
        Assert.Equal(allowed, context.HasActiveCompany);
        Assert.Equal(allowed ? 200 : 403, http.Response.StatusCode);
        await resolver.Received(1).ResolveByCodeForUserAsync("TEST-1", 42, Arg.Any<CancellationToken>());
        await resolver.DidNotReceive().ResolveByCodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        if (allowed) Assert.Equal(1, context.CurrentCompany!.CompanyId);
    }

    private static CompanyConnectionInfo Info(int id) => new(id, $"TEST-{id}", "Test", DatabaseEngine.SqlServer,
        $"Server=local-test;Initial Catalog=DB-{id};Integrated Security=true;", SapIntegrationMode.None);
}
