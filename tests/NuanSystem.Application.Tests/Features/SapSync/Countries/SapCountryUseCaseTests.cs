using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Countries.Commands;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Application.Features.SapSync.Countries.Queries;

namespace NuanSystem.Application.Tests.Features.SapSync.Countries;

public sealed class SapCountryUseCaseTests
{
    [Fact]
    public async Task PreviewWithoutActiveCompany_ReturnsStableCompanyRequiredError()
    {
        var context = Substitute.For<ICompanyContext>();
        var service = Substitute.For<ISapCountryImportService>();
        context.HasActiveCompany.Returns(false);
        var handler = new PreviewCountriesFromSapQueryHandler(context, service);

        var result = await handler.Handle(new PreviewCountriesFromSapQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "COMPANY_REQUIRED");
        await service.DidNotReceiveWithAnyArgs().PreviewAsync(default, default);
    }

    [Fact]
    public async Task ImportWithoutActiveCompany_ReturnsStableCompanyRequiredError()
    {
        var context = Substitute.For<ICompanyContext>();
        var service = Substitute.For<ISapCountryImportService>();
        context.HasActiveCompany.Returns(false);
        var handler = new ImportCountriesFromSapCommandHandler(context, service);

        var result = await handler.Handle(new ImportCountriesFromSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "COMPANY_REQUIRED");
        await service.DidNotReceiveWithAnyArgs().ImportAsync(default, default, default, default);
    }
}
