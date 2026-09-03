using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.SapCodes;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSapCodePolicyUseCaseTests
{
    private readonly ICompanyContext companyContext = Substitute.For<ICompanyContext>();
    private readonly IBusinessPartnerSapCodePolicyRepository repository =
        Substitute.For<IBusinessPartnerSapCodePolicyRepository>();

    [Fact]
    public async Task Get_MapsMissingPolicyToDisabledCentralDefault()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany(10));
        repository.GetByCompanyIdAsync(10, Arg.Any<CancellationToken>())
            .Returns((BusinessPartnerSapCodePolicyRecord?)null);
        var handler = new GetBusinessPartnerSapCodePolicyQueryHandler(companyContext, repository);

        var result = await handler.Handle(new GetBusinessPartnerSapCodePolicyQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(new BusinessPartnerSapCodePolicyDto(
            10,
            false,
            "NationalForeign",
            "PASSPORT",
            "CN0999999999001",
            "CEAB123",
            "PL0999999999001",
            "PEAB123",
            string.Empty));
    }

    [Fact]
    public async Task Get_UsesCurrentMasterCompanyAndCalculatesRoleOnlyExamples()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany(27));
        repository.GetByCompanyIdAsync(27, Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapCodePolicyRecord(
                27, true, "RoleOnly", "PASS", [1, 2, 3, 4, 5, 6, 7, 8]));
        var handler = new GetBusinessPartnerSapCodePolicyQueryHandler(companyContext, repository);

        var result = await handler.Handle(new GetBusinessPartnerSapCodePolicyQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(new BusinessPartnerSapCodePolicyDto(
            27,
            true,
            "RoleOnly",
            "PASS",
            "C0999999999001",
            "CAB123",
            "P0999999999001",
            "PAB123",
            "AQIDBAUGBwg="));
    }

    [Fact]
    public async Task Get_RejectsBranchCompanyContext()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(BranchCompany(parentCompanyId: 10));
        var handler = new GetBusinessPartnerSapCodePolicyQueryHandler(companyContext, repository);

        var result = await handler.Handle(
            new GetBusinessPartnerSapCodePolicyQuery(),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error =>
            error.Code == "BP_SAP_CODE_POLICY_MASTER_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().GetByCompanyIdAsync(default, default);
    }

    [Fact]
    public async Task Update_RejectsBranchCompanyContext()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(BranchCompany(parentCompanyId: 10));
        var handler = new UpdateBusinessPartnerSapCodePolicyCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            new UpdateBusinessPartnerSapCodePolicyCommand(
                true,
                "RoleOnly",
                "PASSPORT",
                Convert.ToBase64String([1, 2, 3]),
                null,
                null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(error => error.Code == "BP_SAP_CODE_POLICY_MASTER_REQUIRED");
        await repository.DidNotReceiveWithAnyArgs().SaveAsync(default!, default);
    }

    [Fact]
    public async Task Update_CreatesOnlyWithNullExpectedVersionAndTrustedCompanyAudit()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany(10));
        repository.GetByCompanyIdAsync(10, Arg.Any<CancellationToken>())
            .Returns((BusinessPartnerSapCodePolicyRecord?)null);
        repository.SaveAsync(Arg.Any<SaveBusinessPartnerSapCodePolicyData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapCodePolicyWriteResult(
                BusinessPartnerSapCodePolicyWriteOutcome.Saved,
                new BusinessPartnerSapCodePolicyRecord(
                    10, true, "NationalForeign", "PASS", [8, 7, 6, 5, 4, 3, 2, 1])));
        var handler = new UpdateBusinessPartnerSapCodePolicyCommandHandler(companyContext, repository);
        using var cancellation = new CancellationTokenSource();

        var result = await handler.Handle(
            new UpdateBusinessPartnerSapCodePolicyCommand(
                true, "NationalForeign", "  PASS  ", null, 81, "  tester  "),
            cancellation.Token);

        result.IsSuccess.Should().BeTrue();
        result.Value!.RowVersion.Should().Be("CAcGBQQDAgE=");
        await repository.Received(1).SaveAsync(
            Arg.Is<SaveBusinessPartnerSapCodePolicyData>(data =>
                data.CompanyId == 10
                && data.IsEnabled
                && data.PrefixMode == "NationalForeign"
                && data.PassportIdentificationTypeCode == "PASS"
                && data.ExpectedRowVersion == null
                && data.AuditUserId == 81
                && data.AuditUserName == "tester"),
            cancellation.Token);
    }

    [Fact]
    public async Task Update_DecodesExpectedVersionForExistingPolicy()
    {
        var rowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany(10));
        repository.GetByCompanyIdAsync(10, Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapCodePolicyRecord(
                10, false, "NationalForeign", "PASSPORT", rowVersion));
        repository.SaveAsync(Arg.Any<SaveBusinessPartnerSapCodePolicyData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapCodePolicyWriteResult(
                BusinessPartnerSapCodePolicyWriteOutcome.Saved,
                new BusinessPartnerSapCodePolicyRecord(
                    10, true, "RoleOnly", "PASSPORT", [2, 3, 4, 5, 6, 7, 8, 9])));
        var handler = new UpdateBusinessPartnerSapCodePolicyCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            new UpdateBusinessPartnerSapCodePolicyCommand(
                true, "RoleOnly", "PASSPORT", "AQIDBAUGBwg=", 81, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await repository.Received(1).SaveAsync(
            Arg.Is<SaveBusinessPartnerSapCodePolicyData>(data =>
                data.ExpectedRowVersion != null
                && data.ExpectedRowVersion.SequenceEqual(rowVersion)),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false, "AQID")]
    [InlineData(true, null)]
    public async Task Update_ReturnsStableConflictWhenCreateUpdateIntentDoesNotMatchCurrentRow(
        bool rowExists,
        string? expectedRowVersion)
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany(10));
        repository.GetByCompanyIdAsync(10, Arg.Any<CancellationToken>())
            .Returns(rowExists
                ? new BusinessPartnerSapCodePolicyRecord(
                    10, false, "NationalForeign", "PASSPORT", [1, 2, 3, 4, 5, 6, 7, 8])
                : null);
        var handler = new UpdateBusinessPartnerSapCodePolicyCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            new UpdateBusinessPartnerSapCodePolicyCommand(
                true, "RoleOnly", "PASSPORT", expectedRowVersion, 81, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error =>
            error.Code == "BP_SAP_CODE_POLICY_CONCURRENCY_CONFLICT"
            && error.Field == "ExpectedRowVersion");
        await repository.DidNotReceiveWithAnyArgs().SaveAsync(default!, default);
    }

    [Fact]
    public async Task Update_MapsConcurrentInsertOrUpdateReportedByPersistenceToStableConflict()
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany(10));
        repository.GetByCompanyIdAsync(10, Arg.Any<CancellationToken>())
            .Returns((BusinessPartnerSapCodePolicyRecord?)null);
        repository.SaveAsync(Arg.Any<SaveBusinessPartnerSapCodePolicyData>(), Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapCodePolicyWriteResult(
                BusinessPartnerSapCodePolicyWriteOutcome.ConcurrencyConflict,
                null));
        var handler = new UpdateBusinessPartnerSapCodePolicyCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            new UpdateBusinessPartnerSapCodePolicyCommand(
                true, "RoleOnly", "PASSPORT", null, 81, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error =>
            error.Code == "BP_SAP_CODE_POLICY_CONCURRENCY_CONFLICT");
    }

    [Theory]
    [InlineData("Unknown", "PASSPORT", null, "BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID")]
    [InlineData("0", "PASSPORT", null, "BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID")]
    [InlineData("1", "PASSPORT", null, "BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID")]
    [InlineData(" 0 ", "PASSPORT", null, "BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID")]
    [InlineData(" 1 ", "PASSPORT", null, "BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID")]
    [InlineData("99", "PASSPORT", null, "BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID")]
    [InlineData("RoleOnly", "", null, "BP_SAP_CODE_POLICY_PASSPORT_CODE_REQUIRED")]
    [InlineData("RoleOnly", "1234567890123456789012345678901", null, "BP_SAP_CODE_POLICY_PASSPORT_CODE_MAX_LENGTH")]
    [InlineData("RoleOnly", "PASSPORT", "not-base64", "BP_SAP_CODE_POLICY_ROW_VERSION_INVALID")]
    [InlineData("RoleOnly", "PASSPORT", "", "BP_SAP_CODE_POLICY_ROW_VERSION_INVALID")]
    public void Validator_RejectsInvalidClosedFields(
        string prefixMode,
        string passportCode,
        string? expectedRowVersion,
        string expectedErrorCode)
    {
        var validator = new UpdateBusinessPartnerSapCodePolicyCommandValidator();

        var result = validator.Validate(new UpdateBusinessPartnerSapCodePolicyCommand(
            true, prefixMode, passportCode, expectedRowVersion, null, null));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorCode == expectedErrorCode);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData(" 0 ")]
    [InlineData(" 1 ")]
    [InlineData("99")]
    public async Task Update_RejectsNumericPrefixModesWithStableBusinessCode(string prefixMode)
    {
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(MasterCompany(10));
        var handler = new UpdateBusinessPartnerSapCodePolicyCommandHandler(companyContext, repository);

        var result = await handler.Handle(
            new UpdateBusinessPartnerSapCodePolicyCommand(
                true, prefixMode, "PASSPORT", null, 81, "tester"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error =>
            error.Code == "BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID"
            && error.Field == "PrefixMode");
        await repository.DidNotReceiveWithAnyArgs().GetByCompanyIdAsync(default, default);
    }

    [Theory]
    [InlineData("NationalForeign", null)]
    [InlineData("RoleOnly", "AQID")]
    public void Validator_AcceptsSupportedModeAndOptionalBase64Version(
        string prefixMode,
        string? expectedRowVersion)
    {
        var validator = new UpdateBusinessPartnerSapCodePolicyCommandValidator();

        var result = validator.Validate(new UpdateBusinessPartnerSapCodePolicyCommand(
            true, prefixMode, "PASSPORT", expectedRowVersion, null, null));

        result.IsValid.Should().BeTrue();
    }

    private static CompanyConnectionInfo MasterCompany(int id) => new(
        id,
        $"MASTER-{id}",
        "Empresa central",
        DatabaseEngine.SqlServer,
        "protected-connection",
        SapIntegrationMode.None,
        IsMaster: true);

    private static CompanyConnectionInfo BranchCompany(int parentCompanyId) => new(
        20,
        "BRANCH-20",
        "Sucursal",
        DatabaseEngine.SqlServer,
        "protected-connection",
        SapIntegrationMode.None,
        IsMaster: false,
        ParentCompanyId: parentCompanyId,
        BranchCode: "B20",
        SyncEnabled: true);
}
