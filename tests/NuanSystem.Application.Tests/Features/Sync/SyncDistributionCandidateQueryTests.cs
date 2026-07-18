using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Distribution;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncDistributionCandidateQueryTests
{
    [Fact]
    public async Task Handler_ShouldFilterFullSourceByCodeOrNameAndRespectTake()
    {
        var matrixId = 41;
        var companyId = 7;
        var repository = Substitute.For<ISyncDistributionPolicyRepository>();
        repository.GetByMatrixIdAsync(matrixId, Arg.Any<CancellationToken>())
            .Returns(Policy(matrixId, companyId, "Item"));
        var profileRepository = Substitute.For<ISyncProfileRepository>();
        profileRepository.GetCompanyLookupsAsync(19, Arg.Any<CancellationToken>())
            .Returns([new SyncCompanyLookupRecord(companyId, "MST", "Matriz", true, true, null, true)]);
        var source = Substitute.For<ISyncFullEntitySource>();
        source.EntityCode.Returns("Item");
        source.ReadPageAsync(Arg.Any<SyncSourceReadContext>(), Arg.Any<CancellationToken>())
            .Returns(new SyncSourcePage(
            [
                Record("INV-001", "Papel bond"),
                Record("INV-002", "Tinta negra"),
                Record("PAP-003", "Resma premium")
            ],
            "PAP-003",
            false));
        var handler = new GetSyncDistributionCandidatesQueryHandler(repository, profileRepository, [source]);

        var result = await handler.Handle(
            new GetSyncDistributionCandidatesQuery(matrixId, "pap", 2, 19),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull().And.HaveCount(2);
        result.Value!.Select(item => item.EntityCode).Should().Equal("INV-001", "PAP-003");
        result.Value!.First().EntityName.Should().Be("Papel bond");
        await source.Received(1).ReadPageAsync(
            Arg.Is<SyncSourceReadContext>(context => context.CompanyId == companyId && context.PageSize == 200),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handler_ShouldRejectACompanyOutsideUserScope()
    {
        var repository = Substitute.For<ISyncDistributionPolicyRepository>();
        repository.GetByMatrixIdAsync(41, Arg.Any<CancellationToken>())
            .Returns(Policy(41, 7, "Item"));
        var profileRepository = Substitute.For<ISyncProfileRepository>();
        profileRepository.GetCompanyLookupsAsync(19, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<SyncCompanyLookupRecord>());
        var handler = new GetSyncDistributionCandidatesQueryHandler(
            repository,
            profileRepository,
            Array.Empty<ISyncFullEntitySource>());

        var result = await handler.Handle(
            new GetSyncDistributionCandidatesQuery(41, null, 50, 19),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == "SyncDistributionPolicyAccessDenied");
    }

    private static SyncSourceRecord Record(string code, string name) =>
        new(Guid.NewGuid(), code, true, new { code, name, isActive = true });

    private static SyncDistributionPolicyDto Policy(int matrixId, int companyId, string entityCode) =>
        new(
            matrixId,
            5,
            "DEMO-ITEMS-PILOT",
            companyId,
            "MST",
            entityCode,
            8,
            "BR1",
            "Sucursal 1",
            "Selected",
            "KeepInMaster",
            null,
            1,
            []);
}
