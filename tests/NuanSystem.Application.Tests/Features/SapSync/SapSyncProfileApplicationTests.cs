using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Profiles.Commands;
using NuanSystem.Application.Features.SapSync.Profiles.Queries;
using NuanSystem.Application.Features.SapSync.Profiles.Services;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncProfileApplicationTests
{
    private const int UserId = 17;

    [Fact]
    public async Task ValidateCompany_DistinguishesMissingInactiveSapDisabledAndUnauthorized()
    {
        var repository = Substitute.For<ISapSyncProfileRepository>();
        var service = new SapSyncProfileValidationService(repository);

        (await service.ValidateCompanyAsync(1, UserId, true)).Errors
            .Should().ContainSingle(error => error.Code == SapSyncProfileErrorCodes.CompanyNotFound);

        repository.GetCompanyAccessAsync(UserId, 1, Arg.Any<CancellationToken>())
            .Returns([Company(isActive: false)]);
        (await service.ValidateCompanyAsync(1, UserId, true)).Errors
            .Should().ContainSingle(error => error.Code == SapSyncProfileErrorCodes.CompanyInactive);

        repository.GetCompanyAccessAsync(UserId, 1, Arg.Any<CancellationToken>())
            .Returns([Company(sapEnabled: false)]);
        (await service.ValidateCompanyAsync(1, UserId, true)).Errors
            .Should().ContainSingle(error => error.Code == SapSyncProfileErrorCodes.CompanySapDisabled);

        repository.GetCompanyAccessAsync(UserId, 1, Arg.Any<CancellationToken>())
            .Returns([Company(isAuthorized: false)]);
        (await service.ValidateCompanyAsync(1, UserId, true)).Errors
            .Should().ContainSingle(error => error.Code == SapSyncProfileErrorCodes.CompanyAccessDenied);
    }

    [Theory]
    [InlineData("Manual", null, null)]
    [InlineData("Interval", 15, null)]
    [InlineData("Daily", null, "08:30:00")]
    public async Task Validate_AcceptsSupportedManualIntervalAndDailySchedules(
        string scheduleType,
        int? intervalMinutes,
        string? executionTime)
    {
        var (repository, service) = ValidService();
        var request = Profile(Entity(
            schedule: Schedule(
                scheduleType,
                intervalMinutes,
                executionTime is null ? null : TimeSpan.Parse(executionTime))));

        var result = await service.ValidateAsync(request, UserId, requireActiveEntity: true);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        await repository.Received(1).GetHandlerCapabilitiesAsync(false, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Validate_RejectsUnknownUnimplementedPurchaseOrdersBothAndDuplicatePairs()
    {
        var repository = Substitute.For<ISapSyncProfileRepository>();
        repository.GetCompanyAccessAsync(UserId, 1, Arg.Any<CancellationToken>())
            .Returns([Company()]);
        repository.GetHandlerCapabilitiesAsync(false, Arg.Any<CancellationToken>())
            .Returns([
                Capability("Suppliers", implemented: true, incremental: true),
                Capability("PurchaseOrders", implemented: false)
            ]);
        var service = new SapSyncProfileValidationService(repository);
        var request = Profile(
            Entity(direction: "Both"),
            Entity(direction: "Both"),
            Entity(code: "PurchaseOrders"),
            Entity(code: "DoesNotExist"));

        var result = await service.ValidateAsync(request, UserId, requireActiveEntity: true);

        result.Errors.Select(error => error.Code).Should().Contain(
            SapSyncProfileErrorCodes.DuplicateEntityDirection,
            SapSyncProfileErrorCodes.DirectionBothUnsupported,
            SapSyncProfileErrorCodes.PurchaseOrdersUnsupported,
            SapSyncProfileErrorCodes.EntityUnknown,
            SapSyncProfileErrorCodes.NoActiveSupportedEntities);
    }

    [Fact]
    public async Task Validate_AllowsInactiveFutureCapabilityWhileKeepingActiveUseBlocked()
    {
        var repository = Substitute.For<ISapSyncProfileRepository>();
        repository.GetCompanyAccessAsync(UserId, 1, Arg.Any<CancellationToken>())
            .Returns([Company()]);
        repository.GetHandlerCapabilitiesAsync(false, Arg.Any<CancellationToken>())
            .Returns([
                Capability("Suppliers", implemented: true),
                Capability("PurchaseOrders", implemented: false)
            ]);
        var service = new SapSyncProfileValidationService(repository);

        var editable = await service.ValidateAsync(
            Profile(
                Entity(code: "Suppliers", isActive: true),
                Entity(code: "PurchaseOrders", direction: "Both", isActive: false)),
            UserId,
            requireActiveEntity: true);
        var executable = await service.ValidateAsync(
            Profile(Entity(code: "PurchaseOrders", isActive: true)),
            UserId,
            requireActiveEntity: false);

        editable.IsValid.Should().BeTrue();
        editable.Errors.Should().BeEmpty();
        executable.IsValid.Should().BeFalse();
        executable.Errors.Select(error => error.Code).Should().Contain(
            SapSyncProfileErrorCodes.PurchaseOrdersUnsupported);
    }

    [Fact]
    public async Task Validate_RejectsUnsupportedDirectionModesTimezoneConcurrencyAndLimits()
    {
        var (_, service) = ValidService();
        var invalid = Entity(
            direction: "ErpToSap",
            syncMode: "Incremental",
            batchSize: 10001,
            maxAttempts: 21,
            executionOrder: -1,
            timeout: 1441,
            schedule: Schedule(
                "Daily",
                intervalMinutes: 5,
                executionTime: null,
                timeZoneId: "Invalid/Zone",
                preventConcurrent: false));

        var result = await service.ValidateAsync(Profile(invalid), UserId, requireActiveEntity: false);

        result.Errors.Select(error => error.Code).Should().Contain(
            SapSyncProfileErrorCodes.DirectionUnsupported,
            SapSyncProfileErrorCodes.SyncModeUnsupported,
            SapSyncProfileErrorCodes.UnsupportedCapability,
            SapSyncProfileErrorCodes.ScheduleInvalid,
            SapSyncProfileErrorCodes.TimeZoneInvalid,
            SapSyncProfileErrorCodes.ConcurrentExecutionRequired);
    }

    [Fact]
    public async Task Create_AlwaysPersistsProfileEntitiesAndSchedulesInactive()
    {
        var (repository, service) = ValidService();
        SapSyncProfileAggregate? captured = null;
        repository.CreateAsync(
                Arg.Do<SapSyncProfileAggregate>(profile => captured = profile),
                Arg.Any<CancellationToken>())
            .Returns(new SapSyncProfileWriteResult(42, SapSyncProfilePersistenceCodes.Created, Version(2)));
        var handler = new CreateSapSyncProfileCommandHandler(repository, service);
        var request = Profile(Entity(isActive: true, schedule: Schedule("Manual", isActive: true)));

        var result = await handler.Handle(
            new CreateSapSyncProfileCommand(request, UserId, UserId, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(42);
        result.Value.IsActive.Should().BeFalse();
        result.Value.RowVersion.Should().Equal(Version(2));
        captured.Should().NotBeNull();
        captured!.IsActive.Should().BeFalse();
        captured.Entities.Should().OnlyContain(entity => !entity.IsActive && !entity.Schedule.IsActive);
    }

    [Fact]
    public async Task BuildAggregate_DefaultsTimezoneToAmericaGuayaquil()
    {
        var (_, service) = ValidService();
        var request = Profile(Entity(schedule: Schedule("Manual", timeZoneId: null)));

        var result = await service.BuildAggregateAsync(
            null,
            request,
            UserId,
            UserId,
            "tester",
            profileIsActive: false,
            forceChildrenInactive: true,
            rowVersion: null);

        result.Value!.Entities.Single().Schedule.TimeZoneId
            .Should().Be("America/Guayaquil");
    }

    [Fact]
    public async Task Create_MapsDuplicateCodeToStableFunctionalError()
    {
        var (repository, service) = ValidService();
        repository.CreateAsync(Arg.Any<SapSyncProfileAggregate>(), Arg.Any<CancellationToken>())
            .Returns(new SapSyncProfileWriteResult(
                null,
                SapSyncProfilePersistenceCodes.DuplicateCode,
                null));
        var handler = new CreateSapSyncProfileCommandHandler(repository, service);

        var result = await handler.Handle(
            new CreateSapSyncProfileCommand(Profile(Entity()), UserId, UserId, "tester"),
            CancellationToken.None);

        result.Errors.Should().ContainSingle(error =>
            error.Code == SapSyncProfileErrorCodes.DuplicateCode);
    }

    [Fact]
    public async Task Update_PreservesProfileActiveStateAndUsesExpectedRowVersion()
    {
        var (repository, service) = ValidService();
        repository.GetByIdAsync(9, Arg.Any<CancellationToken>()).Returns(Detail(isActive: true));
        SapSyncProfileAggregate? captured = null;
        repository.UpdateAsync(
                Arg.Do<SapSyncProfileAggregate>(profile => captured = profile),
                Arg.Any<CancellationToken>())
            .Returns(new SapSyncProfileWriteResult(9, SapSyncProfilePersistenceCodes.Updated, Version(3)));
        var handler = new UpdateSapSyncProfileCommandHandler(repository, service);

        var result = await handler.Handle(
            new UpdateSapSyncProfileCommand(
                9,
                new UpdateSapSyncProfileRequest(Profile(Entity()), Version(1)),
                UserId,
                UserId,
                "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured!.IsActive.Should().BeTrue("la actualizacion general no cambia el estado del perfil");
        captured.CompanyId.Should().Be(1);
        captured.RowVersion.Should().Equal(Version(1));
        await repository.Received(1)
            .UpdateAsync(Arg.Any<SapSyncProfileAggregate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_RejectsCompanyChangeBeforeBuildingOrWritingAggregate()
    {
        var (repository, service) = ValidService();
        repository.GetByIdAsync(9, Arg.Any<CancellationToken>()).Returns(Detail());
        var handler = new UpdateSapSyncProfileCommandHandler(repository, service);
        var request = Profile(Entity()) with { CompanyId = 2 };

        var result = await handler.Handle(
            new UpdateSapSyncProfileCommand(
                9,
                new UpdateSapSyncProfileRequest(request, Version(1)),
                UserId,
                UserId,
                "tester"),
            CancellationToken.None);

        result.Errors.Should().ContainSingle(error =>
            error.Code == SapSyncProfileErrorCodes.CompanyImmutable
            && error.Field == "CompanyId");
        await repository.DidNotReceive()
            .GetHandlerCapabilitiesAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
        await repository.DidNotReceive()
            .UpdateAsync(Arg.Any<SapSyncProfileAggregate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_MapsPersistenceCompanyImmutableToStableFunctionalError()
    {
        var (repository, service) = ValidService();
        repository.GetByIdAsync(9, Arg.Any<CancellationToken>()).Returns(Detail());
        repository.UpdateAsync(
                Arg.Any<SapSyncProfileAggregate>(),
                Arg.Any<CancellationToken>())
            .Returns(new SapSyncProfileWriteResult(
                9,
                SapSyncProfilePersistenceCodes.CompanyImmutable,
                null));
        var handler = new UpdateSapSyncProfileCommandHandler(repository, service);

        var result = await handler.Handle(
            new UpdateSapSyncProfileCommand(
                9,
                new UpdateSapSyncProfileRequest(Profile(Entity()), Version(1)),
                UserId,
                UserId,
                "tester"),
            CancellationToken.None);

        result.Errors.Should().ContainSingle(error =>
            error.Code == SapSyncProfileErrorCodes.CompanyImmutable);
    }

    [Fact]
    public async Task Activate_RequiresActiveSupportedEntityAndMapsConcurrency()
    {
        var (repository, service) = ValidService();
        repository.GetByIdAsync(9, Arg.Any<CancellationToken>())
            .Returns(Detail(isActive: false, entityIsActive: false));
        var handler = new ActivateSapSyncProfileCommandHandler(repository, service);

        var invalid = await handler.Handle(
            new ActivateSapSyncProfileCommand(9, Version(1), UserId, UserId, "tester"),
            CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Errors.Should().Contain(error =>
            error.Code == SapSyncProfileErrorCodes.NoActiveSupportedEntities);
        await repository.DidNotReceive().SetActiveAsync(
            Arg.Any<long>(),
            Arg.Any<bool>(),
            Arg.Any<byte[]>(),
            Arg.Any<int?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());

        repository.GetByIdAsync(9, Arg.Any<CancellationToken>())
            .Returns(Detail(isActive: false, entityIsActive: true));
        repository.SetActiveAsync(
                9,
                true,
                Arg.Any<byte[]>(),
                Arg.Any<int?>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(new SapSyncProfileWriteResult(
                9,
                SapSyncProfilePersistenceCodes.ConcurrencyConflict,
                null));

        var conflict = await handler.Handle(
            new ActivateSapSyncProfileCommand(9, Version(1), UserId, UserId, "tester"),
            CancellationToken.None);
        conflict.Errors.Should().ContainSingle(error =>
            error.Code == SapSyncProfileErrorCodes.ConcurrencyConflict);
    }

    [Fact]
    public async Task Deactivate_OnlyChangesProfileFutureTriggerState()
    {
        var (repository, service) = ValidService();
        repository.GetByIdAsync(9, Arg.Any<CancellationToken>()).Returns(Detail(isActive: true));
        repository.SetActiveAsync(
                9,
                false,
                Arg.Is<byte[]>(value => value.SequenceEqual(Version(1))),
                UserId,
                "tester",
                Arg.Any<CancellationToken>())
            .Returns(new SapSyncProfileWriteResult(9, SapSyncProfilePersistenceCodes.Deactivated, Version(2)));
        var handler = new DeactivateSapSyncProfileCommandHandler(repository, service);

        var result = await handler.Handle(
            new DeactivateSapSyncProfileCommand(9, Version(1), UserId, UserId, "tester"),
            CancellationToken.None);

        result.Value!.IsActive.Should().BeFalse();
        await repository.Received(1).SetActiveAsync(
            9,
            false,
            Arg.Is<byte[]>(value => value.SequenceEqual(Version(1))),
            UserId,
            "tester",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_UsesRowVersionAndMapsLogicalDeleteResult()
    {
        var (repository, service) = ValidService();
        repository.GetByIdAsync(9, Arg.Any<CancellationToken>()).Returns(Detail());
        repository.DeleteAsync(
                9,
                Arg.Is<byte[]>(value => value.SequenceEqual(Version(1))),
                UserId,
                "tester",
                Arg.Any<CancellationToken>())
            .Returns(new SapSyncProfileWriteResult(9, SapSyncProfilePersistenceCodes.Deleted, Version(2)));
        var handler = new DeleteSapSyncProfileCommandHandler(repository, service);

        var result = await handler.Handle(
            new DeleteSapSyncProfileCommand(9, Version(1), UserId, UserId, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Catalog_ReturnsOnlyAuthorizedSapReadyAndImplementedCapabilities()
    {
        var repository = Substitute.For<ISapSyncProfileRepository>();
        repository.GetCompanyAccessAsync(UserId, null, Arg.Any<CancellationToken>())
            .Returns([
                Company(),
                Company() with { CompanyId = 2, CompanyCode = "OUT", IsUserAuthorized = false },
                Company() with { CompanyId = 3, CompanyCode = "OFF", IsSapEnabled = false }
            ]);
        repository.GetHandlerCapabilitiesAsync(true, Arg.Any<CancellationToken>())
            .Returns([
                Capability("Suppliers", implemented: true),
                Capability("PurchaseOrders", implemented: false)
            ]);
        var handler = new GetSapSyncProfileCatalogQueryHandler(repository);

        var result = await handler.Handle(
            new GetSapSyncProfileCatalogQuery(UserId),
            CancellationToken.None);

        result.Value!.Companies.Should().ContainSingle(company => company.Code == "DEMO");
        result.Value.Entities.Should().ContainSingle(entity => entity.EntityCode == "Suppliers");
        result.Value.Directions.Should().ContainSingle(direction => direction.Code == "SapToErp");
        result.Value.DefaultTimeZoneId.Should().Be("America/Guayaquil");
    }

    [Fact]
    public async Task List_DeniesOutOfScopeCompanyBeforeSearchingProfiles()
    {
        var repository = Substitute.For<ISapSyncProfileRepository>();
        repository.GetCompanyAccessAsync(UserId, 1, Arg.Any<CancellationToken>())
            .Returns([Company(isAuthorized: false)]);
        var service = new SapSyncProfileValidationService(repository);
        var handler = new GetSapSyncProfilesQueryHandler(repository, service);

        var result = await handler.Handle(
            new GetSapSyncProfilesQuery(
                new SapSyncProfileListRequest(1, null, null, null, 1, 50),
                UserId),
            CancellationToken.None);

        result.Errors.Should().ContainSingle(error =>
            error.Code == SapSyncProfileErrorCodes.CompanyAccessDenied);
        await repository.DidNotReceive()
            .SearchAsync(Arg.Any<SapSyncProfileFilter>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task StaticValidation_UsesOnlyPersistedProfileMetadataAndCapabilities()
    {
        var (repository, service) = ValidService();
        repository.GetByIdAsync(9, Arg.Any<CancellationToken>())
            .Returns(Detail(entityIsActive: true));
        var handler = new ValidateSapSyncProfileCommandHandler(repository, service);

        var result = await handler.Handle(
            new ValidateSapSyncProfileCommand(9, UserId),
            CancellationToken.None);

        result.Value!.IsValid.Should().BeTrue();
        await repository.Received(1).GetByIdAsync(9, Arg.Any<CancellationToken>());
        await repository.Received(2).GetCompanyAccessAsync(
            UserId,
            1,
            Arg.Any<CancellationToken>());
        await repository.Received(1).GetHandlerCapabilitiesAsync(
            false,
            Arg.Any<CancellationToken>());
    }

    private static (ISapSyncProfileRepository Repository, SapSyncProfileValidationService Service) ValidService()
    {
        var repository = Substitute.For<ISapSyncProfileRepository>();
        repository.GetCompanyAccessAsync(UserId, 1, Arg.Any<CancellationToken>())
            .Returns([Company()]);
        repository.GetHandlerCapabilitiesAsync(false, Arg.Any<CancellationToken>())
            .Returns([Capability("Suppliers", implemented: true, incremental: false)]);
        return (repository, new SapSyncProfileValidationService(repository));
    }

    private static SapSyncProfileCompanyAccessDto Company(
        bool isActive = true,
        bool sapEnabled = true,
        bool isAuthorized = true) =>
        new(1, "DEMO", "Demo", isActive, 1, true, sapEnabled, 1, isAuthorized);

    private static SapSyncHandlerCapabilityDto Capability(
        string code,
        bool implemented,
        bool incremental = false) =>
        new(code, code, true, false, true, incremental, implemented, true);

    private static SaveSapSyncProfileRequest Profile(params SaveSapSyncProfileEntityRequest[] entities) =>
        new(1, "SAP-DEMO", "SAP Demo", "Safe description", entities);

    private static SaveSapSyncProfileEntityRequest Entity(
        string code = "Suppliers",
        string direction = "SapToErp",
        string syncMode = "Full",
        int batchSize = 100,
        int maxAttempts = 3,
        int executionOrder = 10,
        int timeout = 30,
        bool isActive = true,
        SaveSapSyncScheduleRequest? schedule = null) =>
        new(
            11,
            code,
            direction,
            syncMode,
            batchSize,
            maxAttempts,
            executionOrder,
            true,
            timeout,
            isActive,
            schedule ?? Schedule("Manual"),
            Version(1));

    private static SaveSapSyncScheduleRequest Schedule(
        string type,
        int? intervalMinutes = null,
        TimeSpan? executionTime = null,
        string? timeZoneId = "America/Guayaquil",
        bool preventConcurrent = true,
        bool isActive = false) =>
        new(21, type, intervalMinutes, executionTime, timeZoneId, preventConcurrent, isActive, Version(1));

    private static SapSyncProfileDetailDto Detail(
        bool isActive = false,
        bool entityIsActive = true) =>
        new(
            9,
            1,
            "DEMO",
            "Demo",
            "SAP-DEMO",
            "SAP Demo",
            null,
            isActive,
            UserId,
            "tester",
            DateTime.UtcNow,
            null,
            null,
            null,
            Version(1),
            [
                new SapSyncProfileEntityData(
                    11,
                    "Suppliers",
                    SapSyncDirection.SapToErp,
                    SapSyncModes.Full,
                    100,
                    3,
                    10,
                    true,
                    30,
                    entityIsActive,
                    new SapSyncScheduleData(
                        21,
                        SapSyncScheduleTypes.Manual,
                        null,
                        null,
                        "America/Guayaquil",
                        true,
                        null,
                        null,
                        null,
                        null,
                        false,
                        Version(1)),
                    Version(1))
            ]);

    private static byte[] Version(byte value) => [value, 0, 0, 0, 0, 0, 0, 0];
}
