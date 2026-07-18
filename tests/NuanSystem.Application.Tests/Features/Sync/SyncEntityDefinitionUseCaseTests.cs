using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Configuration.Queries;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Commands;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Queries;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Services;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class SyncEntityDefinitionUseCaseTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateCustomDefinitionAsNonOperative()
    {
        var repository = Substitute.For<ISyncEntityDefinitionRepository>();
        repository.GetByCodeAsync("CustomCatalog", Arg.Any<CancellationToken>()).Returns((SyncEntityDefinitionDetailRecord?)null);
        repository.GetLookupAsync(null, true, Arg.Any<CancellationToken>()).Returns([]);
        repository.CreateAsync(Arg.Any<CreateSyncEntityDefinitionData>(), Arg.Any<CancellationToken>())
            .Returns(SyncEntityDefinitionMutationResult.Success(20));
        repository.GetByIdAsync(20, Arg.Any<CancellationToken>()).Returns(Detail(20, "CustomCatalog", false));
        var handler = new CreateSyncEntityDefinitionCommandHandler(repository);

        var result = await handler.Handle(CreateCommand("  CustomCatalog  "), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Code.Should().Be("CustomCatalog");
        result.Value.HasProducer.Should().BeFalse();
        result.Value.HasApplier.Should().BeFalse();
        result.Value.IsOperative.Should().BeFalse();
        await repository.Received(1).CreateAsync(
            Arg.Is<CreateSyncEntityDefinitionData>(data =>
                data.Code == "CustomCatalog"
                && data.Name == "Catalogo personalizado"
                && data.AuditUserName == "admin"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFunctionalErrorForDependencyCycle()
    {
        var repository = Substitute.For<ISyncEntityDefinitionRepository>();
        repository.GetByCodeAsync("CustomCatalog", Arg.Any<CancellationToken>()).Returns((SyncEntityDefinitionDetailRecord?)null);
        repository.GetLookupAsync(null, true, Arg.Any<CancellationToken>()).Returns([]);
        repository.CreateAsync(Arg.Any<CreateSyncEntityDefinitionData>(), Arg.Any<CancellationToken>())
            .Returns(SyncEntityDefinitionMutationResult.Failure(SyncEntityDefinitionMutationError.DependencyCycle));
        var handler = new CreateSyncEntityDefinitionCommandHandler(repository);

        var result = await handler.Handle(CreateCommand("CustomCatalog"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "SyncEntityDefinitionDependencyCycle");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRejectSystemDefinitionBeforePersistenceMutation()
    {
        var repository = Substitute.For<ISyncEntityDefinitionRepository>();
        repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(Detail(1, "Warehouse", true));
        var handler = new DeleteSyncEntityDefinitionCommandHandler(repository);

        var result = await handler.Handle(new DeleteSyncEntityDefinitionCommand(1, 5, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "SyncEntityDefinitionSystemProtected");
        await repository.DidNotReceive().DeleteAsync(
            Arg.Any<int>(),
            Arg.Any<int?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_ShouldCombineMasterMetadataWithTechnicalCapabilities()
    {
        var repository = Substitute.For<ISyncEntityDefinitionRepository>();
        repository.SearchAsync(Arg.Any<SyncEntityDefinitionListFilter>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResultDto<SyncEntityDefinitionRecord>(
                [Record(1, "Warehouse", true), Record(20, "CustomCatalog", false)],
                2,
                1,
                50));
        var handler = new GetSyncEntityDefinitionsQueryHandler(repository);

        var result = await handler.Handle(new GetSyncEntityDefinitionsQuery(null, null, 1, 50), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Single(item => item.Code == "Warehouse").IsOperative.Should().BeTrue();
        result.Value.Items.Single(item => item.Code == "CustomCatalog").IsOperative.Should().BeFalse();
    }

    [Fact]
    public async Task ConfigurationCatalog_ShouldUseMasterEntityDefinitions()
    {
        var profileRepository = Substitute.For<ISyncProfileRepository>();
        profileRepository.GetCompanyLookupsAsync(null, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<SyncCompanyLookupRecord>());
        var catalogService = Substitute.For<ISyncEntityCatalogService>();
        catalogService.GetAsync(false, null, Arg.Any<CancellationToken>())
            .Returns([Lookup(20, "CustomCatalog", false)]);
        var handler = new GetSyncConfigurationCatalogQueryHandler(profileRepository, catalogService);

        var result = await handler.Handle(new GetSyncConfigurationCatalogQuery(null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Entities.Should().ContainSingle(entity =>
            entity.Id == 20
            && entity.Code == "CustomCatalog"
            && !entity.HasProducer
            && !entity.HasApplier);
    }

    [Fact]
    public void CreateValidator_ShouldRejectExecutableCodeAndDuplicateDependencies()
    {
        var validator = new CreateSyncEntityDefinitionCommandValidator();
        var command = CreateCommand("Entity;DROP") with { DependencyDefinitionIds = [1, 1] };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(command.Code));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(command.DependencyDefinitionIds));
    }

    private static CreateSyncEntityDefinitionCommand CreateCommand(string code)
    {
        return new CreateSyncEntityDefinitionCommand(
            code,
            "Catalogo personalizado",
            "Prueba",
            300,
            true,
            true,
            true,
            false,
            "Code",
            "UpdatedAt",
            false,
            [],
            5,
            " admin ");
    }

    private static SyncEntityDefinitionDetailRecord Detail(int id, string code, bool isSystem)
    {
        return new SyncEntityDefinitionDetailRecord(Record(id, code, isSystem), []);
    }

    private static SyncEntityDefinitionRecord Record(int id, string code, bool isSystem)
    {
        return new SyncEntityDefinitionRecord
        {
            Id = id,
            Code = code,
            Name = code,
            DefaultExecutionOrder = 100,
            SupportsIncremental = true,
            SupportsInsert = true,
            SupportsUpdate = true,
            SupportsDeactivate = true,
            IsSystem = isSystem,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static SyncEntityDefinitionLookupDto Lookup(int id, string code, bool isSystem)
    {
        return new SyncEntityDefinitionLookupDto(
            id,
            code,
            code,
            null,
            100,
            true,
            true,
            true,
            false,
            "Code",
            "UpdatedAt",
            isSystem,
            true,
            false,
            false,
            []);
    }
}
