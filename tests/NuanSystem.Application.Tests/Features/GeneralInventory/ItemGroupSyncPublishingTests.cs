using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.GeneralInventory;

public sealed class ItemGroupSyncPublishingTests
{
    [Fact]
    public async Task CreateItemGroup_PublishesStableGlobalIdAndSapReference()
    {
        var repository = Substitute.For<IItemGroupRepository>();
        var chartRepository = Substitute.For<IChartOfAccountRepository>();
        var publisher = Substitute.For<ISyncEventPublisher>();
        var companyContext = Substitute.For<ICompanyContext>();
        var itemGroup = CreateItemGroup();
        SyncPublishRequest? captured = null;

        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            1,
            "DEMO",
            "Empresa Demo",
            DatabaseEngine.SqlServer,
            "Server=(local);Database=NuanSystem_DEMO;",
            SapIntegrationMode.ServiceLayer,
            CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: true));
        repository.ExistsByCodeAsync("INV-PAP", Arg.Any<CancellationToken>()).Returns(false);
        repository.CreateAsync(Arg.Any<CreateItemGroupData>(), Arg.Any<CancellationToken>()).Returns(itemGroup.Id);
        repository.GetByIdAsync(itemGroup.Id, Arg.Any<CancellationToken>()).Returns(itemGroup);
        publisher.PublishAsync(Arg.Do<SyncPublishRequest>(request => captured = request), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 1, "Publicado.")));
        var handler = new CreateItemGroupCommandHandler(repository, chartRepository, publisher, companyContext);

        var result = await handler.Handle(
            new CreateItemGroupCommand(
                "inv-pap",
                "Papeleria",
                null,
                true,
                null,
                null,
                null,
                null,
                "114",
                "114",
                1,
                "admin"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityName.Should().Be("ItemGroups");
        captured.EntityGlobalId.Should().Be(itemGroup.GlobalId!.Value);
        captured.Operation.Should().Be(SyncOperation.Created);
        var payload = captured.Payload.Should().BeOfType<ItemGroupSyncPayload>().Subject;
        payload.Code.Should().Be("INV-PAP");
        payload.SapGroupCode.Should().Be("114");
    }

    private static ItemGroupDto CreateItemGroup()
    {
        return new ItemGroupDto
        {
            Id = 4,
            GlobalId = Guid.NewGuid(),
            Code = "INV-PAP",
            Name = "Papeleria",
            SapGroupCode = "114",
            SapCode = "114",
            IsActive = true,
            CreatedAt = new DateTime(2026, 7, 17, 10, 0, 0, DateTimeKind.Utc)
        };
    }
}
