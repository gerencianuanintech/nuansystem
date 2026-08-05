using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Provinces.Commands;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;
using NuanSystem.Application.Features.SapSync.Provinces.Queries;

namespace NuanSystem.Application.Tests.Features.SapSync.Provinces;

public sealed class SapProvinceUseCaseTests
{
    [Fact]
    public async Task PreviewWithoutActiveCompany_ReturnsStableCompanyRequiredError()
    {
        var context = Substitute.For<ICompanyContext>();
        var service = Substitute.For<ISapProvinceImportService>();
        context.HasActiveCompany.Returns(false);
        var handler = new PreviewProvincesFromSapQueryHandler(context, service);

        var result = await handler.Handle(new PreviewProvincesFromSapQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "COMPANY_REQUIRED");
        await service.DidNotReceiveWithAnyArgs().PreviewAsync(default, default);
    }

    [Fact]
    public async Task ImportWithoutActiveCompany_ReturnsStableCompanyRequiredError()
    {
        var context = Substitute.For<ICompanyContext>();
        var service = Substitute.For<ISapProvinceImportService>();
        context.HasActiveCompany.Returns(false);
        var handler = new ImportProvincesFromSapCommandHandler(context, service);

        var result = await handler.Handle(new ImportProvincesFromSapCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "COMPANY_REQUIRED");
        await service.DidNotReceiveWithAnyArgs().ImportAsync(default, default, default, default);
    }
}
