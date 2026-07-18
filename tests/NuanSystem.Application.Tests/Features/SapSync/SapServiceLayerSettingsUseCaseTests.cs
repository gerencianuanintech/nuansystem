using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Commands;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Queries;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapServiceLayerSettingsUseCaseTests
{
    [Fact]
    public async Task Query_ReturnsOnlyPasswordPresence()
    {
        var context = ActiveCompanyContext();
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        repository.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>()).Returns(Settings("protected-value"));
        var handler = new GetSapServiceLayerSettingsQueryHandler(context, repository);

        var result = await handler.Handle(new GetSapServiceLayerSettingsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.HasPassword.Should().BeTrue();
        typeof(SapServiceLayerSettingsDto).GetProperties()
            .Select(property => property.Name)
            .Should().NotContain(name => name.Contains("Encrypted", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Update_RequiresPassword_WhenEnablingFirstConfiguration()
    {
        var context = ActiveCompanyContext();
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        var protector = Substitute.For<ISecretProtector>();
        repository.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>()).Returns((SapCompanySettingsDto?)null);
        var handler = new UpdateSapServiceLayerSettingsCommandHandler(context, repository, protector);

        var result = await handler.Handle(Command(password: null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "SAP_PASSWORD_REQUIRED");
        await repository.DidNotReceive().UpsertServiceLayerAsync(
            Arg.Any<UpdateSapServiceLayerSettingsData>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ProtectsPasswordBeforePersistence()
    {
        var context = ActiveCompanyContext();
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        var protector = Substitute.For<ISecretProtector>();
        var updated = Settings("protected-value");
        repository.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>())
            .Returns((SapCompanySettingsDto?)null, updated);
        protector.Protect("temporary-secret").Returns("protected-value");
        repository.UpsertServiceLayerAsync(
            Arg.Any<UpdateSapServiceLayerSettingsData>(),
            Arg.Any<CancellationToken>()).Returns(10);
        var handler = new UpdateSapServiceLayerSettingsCommandHandler(context, repository, protector);

        var result = await handler.Handle(Command("temporary-secret"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await repository.Received(1).UpsertServiceLayerAsync(
            Arg.Is<UpdateSapServiceLayerSettingsData>(data =>
                data.CompanyId == 1
                && data.ServiceLayerUrl == "https://sap.test:50000/b1s/v1/"
                && data.SapPasswordEncrypted == "protected-value"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_KeepsExistingPassword_WhenPasswordIsOmitted()
    {
        var context = ActiveCompanyContext();
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        var protector = Substitute.For<ISecretProtector>();
        var current = Settings("protected-value");
        repository.GetByCompanyIdAsync(1, Arg.Any<CancellationToken>()).Returns(current);
        repository.UpsertServiceLayerAsync(
            Arg.Any<UpdateSapServiceLayerSettingsData>(),
            Arg.Any<CancellationToken>()).Returns(10);
        var handler = new UpdateSapServiceLayerSettingsCommandHandler(context, repository, protector);

        var result = await handler.Handle(Command(password: null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        protector.DidNotReceive().Protect(Arg.Any<string>());
        await repository.Received(1).UpsertServiceLayerAsync(
            Arg.Is<UpdateSapServiceLayerSettingsData>(data => data.SapPasswordEncrypted == null),
            Arg.Any<CancellationToken>());
    }

    private static UpdateSapServiceLayerSettingsCommand Command(string? password)
        => new(
            true,
            "https://sap.test:50000/b1s/v1",
            "SBO_TEST",
            "technical-user",
            password,
            3,
            1,
            "tester");

    private static ICompanyContext ActiveCompanyContext()
    {
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(true);
        context.CurrentCompany.Returns(new CompanyConnectionInfo(
            1,
            "DEMO",
            "Empresa Demo",
            DatabaseEngine.SqlServer,
            "protected-connection",
            SapIntegrationMode.None));
        return context;
    }

    private static SapCompanySettingsDto Settings(string encryptedPassword)
        => new()
        {
            Id = 10,
            CompanyId = 1,
            CompanyCode = "DEMO",
            IsEnabled = true,
            IntegrationMode = SapIntegrationMode.ServiceLayer,
            ServiceLayerUrl = "https://sap.test:50000/b1s/v1/",
            SapCompanyDb = "SBO_TEST",
            SapUser = "technical-user",
            SapPasswordEncrypted = encryptedPassword,
            MaxRetryCount = 3
        };
}
