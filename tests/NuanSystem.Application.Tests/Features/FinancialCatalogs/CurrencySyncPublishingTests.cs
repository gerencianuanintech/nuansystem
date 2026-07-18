using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.FinancialCatalogs;

public sealed class CurrencySyncPublishingTests
{
    private readonly IFinancialCatalogRepository _repository = Substitute.For<IFinancialCatalogRepository>();
    private readonly ISyncEventPublisher _publisher = Substitute.For<ISyncEventPublisher>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();

    [Fact]
    public async Task CreateCurrency_PublishesStableGlobalIdAndCompletePayload()
    {
        SyncPublishRequest? captured = null;
        var currency = CreateCurrency();
        ConfigureCompany();
        _repository.ExistsByCodeAsync("currencies", "USD", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync("currencies", Arg.Any<CreateFinancialCatalogData>(), Arg.Any<CancellationToken>()).Returns(currency.Id);
        _repository.GetByIdAsync("currencies", currency.Id, Arg.Any<CancellationToken>()).Returns(currency);
        _publisher.PublishAsync(Arg.Do<SyncPublishRequest>(request => captured = request), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 1, "Publicado.")));
        var handler = new CreateFinancialCatalogCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new CreateFinancialCatalogCommand("currencies", "usd", "Dolar", "Moneda base", true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityName.Should().Be("Currencies");
        captured.EntityGlobalId.Should().Be(currency.GlobalId!.Value);
        captured.Operation.Should().Be(SyncOperation.Created);
        var payload = captured.Payload.Should().BeOfType<CurrencySyncPayload>().Subject;
        payload.Symbol.Should().Be("$");
        payload.IsBaseCurrency.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateInactiveCurrency_PublishesDisabledOperation()
    {
        SyncPublishRequest? captured = null;
        var current = CreateCurrency();
        var inactive = CreateCurrency(isActive: false, globalId: current.GlobalId);
        ConfigureCompany();
        _repository.GetByIdAsync("currencies", current.Id, Arg.Any<CancellationToken>()).Returns(current, inactive);
        _repository.ExistsByCodeAsync("currencies", "USD", current.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync("currencies", Arg.Any<UpdateFinancialCatalogData>(), Arg.Any<CancellationToken>()).Returns(true);
        _publisher.PublishAsync(Arg.Do<SyncPublishRequest>(request => captured = request), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 1, "Publicado.")));
        var handler = new UpdateFinancialCatalogCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new UpdateFinancialCatalogCommand("currencies", current.Id, "USD", "Dolar", "Moneda base", false, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured!.EntityGlobalId.Should().Be(current.GlobalId!.Value);
        captured.Operation.Should().Be(SyncOperation.Disabled);
    }

    [Fact]
    public async Task CreateOtherFinancialCatalog_DoesNotPublishCurrencyEvent()
    {
        var bank = new FinancialCatalogDto
        {
            Id = 1,
            Code = "BANK",
            Name = "Banco",
            IsActive = true,
            CreatedAt = new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc)
        };
        ConfigureCompany();
        _repository.ExistsByCodeAsync("banks", "BANK", Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync("banks", Arg.Any<CreateFinancialCatalogData>(), Arg.Any<CancellationToken>()).Returns(bank.Id);
        _repository.GetByIdAsync("banks", bank.Id, Arg.Any<CancellationToken>()).Returns(bank);
        var handler = new CreateFinancialCatalogCommandHandler(_repository, _publisher, _companyContext);

        var result = await handler.Handle(
            new CreateFinancialCatalogCommand("banks", "bank", "Banco", null, true, 7, "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _publisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);
    }

    private void ConfigureCompany()
    {
        _companyContext.HasActiveCompany.Returns(true);
        _companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            10,
            "MASTER",
            "Empresa Master",
            DatabaseEngine.SqlServer,
            "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode.None,
            CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: true));
    }

    private static FinancialCatalogDto CreateCurrency(bool isActive = true, Guid? globalId = null)
    {
        return new FinancialCatalogDto
        {
            Id = 1,
            GlobalId = globalId ?? Guid.NewGuid(),
            Code = "USD",
            Name = "Dolar",
            Symbol = "$",
            Description = "Moneda base",
            IsBaseCurrency = true,
            IsActive = isActive,
            CreatedAt = new DateTime(2026, 7, 16, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = isActive ? null : new DateTime(2026, 7, 16, 11, 0, 0, DateTimeKind.Utc)
        };
    }
}
