using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Cities.Configuration;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapCityQuerySettingsUseCasesTests
{
    private const string Query = """
        SELECT
            'EC' AS "CountryCode",
            LEFT(TRIM("Code"), 2) AS "ProvinceCode",
            TRIM("Code") AS "CityCode",
            TRIM("Name") AS "CityName"
        FROM "@MUNI_CANTO"
        """;

    [Fact]
    public async Task Get_WithoutActiveCompany_ReturnsCompanyRequiredWithoutRepositoryAccess()
    {
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(false);
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        var handler = new GetSapCityQuerySettingsQueryHandler(context, repository);

        var result = await handler.Handle(new GetSapCityQuerySettingsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "COMPANY_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().GetByCompanyIdAsync(default);
    }

    [Fact]
    public async Task Get_ReturnsConfigurationForActiveCompanyOnly()
    {
        var context = ActiveCompanyContext();
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        var updatedAt = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc);
        repository.GetByCompanyIdAsync(7, Arg.Any<CancellationToken>())
            .Returns(Settings(Query, updatedAt));
        var handler = new GetSapCityQuerySettingsQueryHandler(context, repository);

        var result = await handler.Handle(new GetSapCityQuerySettingsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new SapCityQuerySettingsDto(
            7, "DEMO", Query, true, updatedAt));
        await repository.Received(1).GetByCompanyIdAsync(7, Arg.Any<CancellationToken>());
        await repository.DidNotReceive().GetByCompanyCodeAsync(
            Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Put_NormalizesQueryAndAuditIdentityThenReloadsSavedConfiguration()
    {
        var context = ActiveCompanyContext();
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        repository.UpdateCitiesSelectQueryAsync(
            Arg.Any<UpdateSapCityQuerySettingsData>(), Arg.Any<CancellationToken>()).Returns(10);
        repository.GetByCompanyIdAsync(7, Arg.Any<CancellationToken>())
            .Returns(Settings(Query.Trim(), DateTime.UtcNow));
        var handler = new UpdateSapCityQuerySettingsCommandHandler(context, repository);

        var result = await handler.Handle(
            new UpdateSapCityQuerySettingsCommand($"  {Query}\r\n ", 42, "  tester  "),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsConfigured.Should().BeTrue();
        await repository.Received(1).UpdateCitiesSelectQueryAsync(
            Arg.Is<UpdateSapCityQuerySettingsData>(data =>
                data.CompanyId == 7
                && data.CitiesSelectQuery == Query.Trim()
                && data.UpdatedByUserId == 42
                && data.UpdatedByUserName == "tester"),
            Arg.Any<CancellationToken>());
        await repository.Received(1).GetByCompanyIdAsync(7, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Put_BlankQueryPersistsNullAndReturnsDisabledConfiguration()
    {
        var context = ActiveCompanyContext();
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        repository.UpdateCitiesSelectQueryAsync(
            Arg.Any<UpdateSapCityQuerySettingsData>(), Arg.Any<CancellationToken>()).Returns(10);
        repository.GetByCompanyIdAsync(7, Arg.Any<CancellationToken>())
            .Returns(Settings(null, DateTime.UtcNow));
        var handler = new UpdateSapCityQuerySettingsCommandHandler(context, repository);

        var result = await handler.Handle(
            new UpdateSapCityQuerySettingsCommand(" \r\n ", 42, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsConfigured.Should().BeFalse();
        result.Value.CitiesSelectQuery.Should().BeNull();
        await repository.Received(1).UpdateCitiesSelectQueryAsync(
            Arg.Is<UpdateSapCityQuerySettingsData>(data => data.CitiesSelectQuery == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Put_WithoutActiveCompany_ReturnsCompanyRequiredWithoutWrite()
    {
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(false);
        var repository = Substitute.For<ISapCompanySettingsRepository>();
        var handler = new UpdateSapCityQuerySettingsCommandHandler(context, repository);

        var result = await handler.Handle(
            new UpdateSapCityQuerySettingsCommand(Query), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "COMPANY_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().UpdateCitiesSelectQueryAsync(default!);
    }

    [Fact]
    public void Validator_AcceptsQuotedHanaAliasesAndRejectsMissingAlias()
    {
        var validator = new UpdateSapCityQuerySettingsCommandValidator();

        var valid = validator.Validate(new UpdateSapCityQuerySettingsCommand(Query));
        var invalid = validator.Validate(new UpdateSapCityQuerySettingsCommand(
            "SELECT 'EC' AS CountryCode, '01' AS ProvinceCode, '0101' AS CityCode FROM DUMMY"));

        valid.IsValid.Should().BeTrue();
        invalid.IsValid.Should().BeFalse();
        invalid.Errors.Should().ContainSingle(error => error.ErrorMessage.Contains("CityName"));
    }

    private static ICompanyContext ActiveCompanyContext()
    {
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(true);
        context.CurrentCompany.Returns(new CompanyConnectionInfo(
            7,
            "DEMO",
            "Empresa Demo",
            DatabaseEngine.SqlServer,
            "protected-connection",
            SapIntegrationMode.None));
        return context;
    }

    private static SapCompanySettingsDto Settings(string? query, DateTime? updatedAt) => new()
    {
        Id = 10,
        CompanyId = 7,
        CompanyCode = "DEMO",
        CitiesSelectQuery = query,
        UpdatedAt = updatedAt
    };
}
