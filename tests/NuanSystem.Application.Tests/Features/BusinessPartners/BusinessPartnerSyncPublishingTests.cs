using FluentAssertions;
using NSubstitute;
using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSyncPublishingTests
{
    private readonly IBusinessPartnerRepository _repository = Substitute.For<IBusinessPartnerRepository>();
    private readonly IBusinessPartnerLocalOutboxWriter _writer = Substitute.For<IBusinessPartnerLocalOutboxWriter>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();
    private readonly IBusinessPartnerSapCodePolicyRepository _sapPolicyRepository = Substitute.For<IBusinessPartnerSapCodePolicyRepository>();
    private readonly ImmediateTransactionRunner _transactionRunner = new();

    [Fact]
    public async Task Create_WritesLocalOutboxInsideTheSameTransaction()
    {
        var partner = CreatePartner();
        _repository.ExistsByCodeAsync(Arg.Is<string>(value => value.StartsWith("BP-")), null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync("Customer", 1, "0999999999001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner.Id);
        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            partner, SyncOperation.Created, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
        _transactionRunner.Committed.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WritesLocalOutboxInsideTheSameTransaction()
    {
        var partner = CreatePartner(name: "Cliente Actualizado");
        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);
        _repository.UpdateAsync(Arg.Any<UpdateBusinessPartnerData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(UpdateCommand(partner.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            partner, SyncOperation.Updated, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Delete_WritesLocalOutboxInsideTheSameTransaction()
    {
        var partner = CreatePartner();
        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);
        _repository.DeleteAsync(Arg.Any<DeleteBusinessPartnerData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateDeleteHandler();

        var result = await handler.Handle(new DeleteBusinessPartnerCommand(partner.Id, partner.RowVersion, 7, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            partner, SyncOperation.Deleted, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenLocalOutboxFails()
    {
        var partner = CreatePartner();
        _repository.ExistsByCodeAsync(Arg.Is<string>(value => value.StartsWith("BP-")), null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync("Customer", 1, "0999999999001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner.Id);
        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);
        _writer.EnqueueAsync(Arg.Any<BusinessPartnerDto>(), Arg.Any<SyncOperation>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>())
            .Returns<Task<Guid?>>(_ => throw new InvalidOperationException("Controlled outbox failure"));
        var handler = CreateCreateHandler();

        var action = () => handler.Handle(CreateCommand(), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Controlled outbox failure");
        _transactionRunner.RolledBack.Should().BeTrue();
        _transactionRunner.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Create_GeneratesInternalCodeNormalizesIdentityAndAssignsChildGlobalIds()
    {
        CreateBusinessPartnerData? saved = null;
        var command = CreateCommand() with
        {
            IdentificationNumber = " 09.999-999 99001 ",
            Addresses = [new SaveBusinessPartnerAddressData(null, null, null, null, "Main", " Calle 1 ", null, null, null, null, null, null, null, true, true)],
            Contacts = [new SaveBusinessPartnerContactData(null, null, null, " Contacto ", null, null, null, null, null, null, null, false, true, true, null)]
        };
        _repository.ExistsByIdentificationAsync("Customer", 1, "0999999999001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Do<CreateBusinessPartnerData>(data => saved = data), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(25);
        _repository.GetByIdAsync(25, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(CreatePartner());

        var result = await CreateCreateHandler().Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        saved!.GlobalId.Should().NotBeEmpty();
        saved.Code.Should().Be(BusinessPartnerIdentityPolicy.CreateInternalCode(saved.GlobalId));
        saved.NormalizedIdentificationNumber.Should().Be("0999999999001");
        saved.Addresses.Should().OnlyContain(item => item.GlobalId.HasValue && item.GlobalId != Guid.Empty);
        saved.Contacts.Should().OnlyContain(item => item.GlobalId.HasValue && item.GlobalId != Guid.Empty);
    }

    [Fact]
    public async Task Create_RejectsBothWithStableCodeBeforePersistence()
    {
        var result = await CreateCreateHandler().Handle(CreateCommand() with { PartnerType = "Both" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_ROLE_INVALID");
        await _repository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Create_RejectsIdentificationThatNormalizesToEmptyBeforePersistence()
    {
        var result = await CreateCreateHandler().Handle(
            CreateCommand() with { IdentificationNumber = " -- .. " }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_IDENTIFICATION_INVALID");
        await _repository.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Create_UsesRoleAwareNormalizedUniqueness()
    {
        CreateBusinessPartnerData? saved = null;
        _repository.ExistsByIdentificationAsync("Supplier", 1, "0999999999001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Do<CreateBusinessPartnerData>(data => saved = data), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(25);
        _repository.GetByIdAsync(25, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(WithPartnerType(CreatePartner(), "Supplier"));

        var result = await CreateCreateHandler().Handle(CreateCommand() with { PartnerType = "Supplier" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        saved!.PartnerType.Should().Be("Supplier");
        await _repository.Received(1).ExistsByIdentificationAsync("Supplier", 1, "0999999999001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_CentralEnabledPolicyCalculatesSapCodeButBranchNeverDoes()
    {
        var central = Company(syncEnabled: true);
        _companyContext.HasActiveCompany.Returns(true);
        _companyContext.CurrentCompany.Returns(central);
        _sapPolicyRepository.GetByCompanyIdAsync(central.CompanyId, Arg.Any<CancellationToken>())
            .Returns(new BusinessPartnerSapCodePolicyRecord(central.CompanyId, true, "RoleOnly", "PASS", [1, 2, 3, 4, 5, 6, 7, 8]));
        _repository.GetIdentificationTypeCodeAsync(1, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns("NATIONAL");
        CreateBusinessPartnerData? centralSaved = null;
        _repository.CreateAsync(Arg.Do<CreateBusinessPartnerData>(data => centralSaved = data), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(25);
        _repository.GetByIdAsync(25, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(CreatePartner(sapCardCode: "C0999999999001"));

        var centralResult = await new CreateBusinessPartnerCommandHandler(_repository, _transactionRunner, _writer, _companyContext, _sapPolicyRepository)
            .Handle(CreateCommand(), CancellationToken.None);

        centralResult.IsSuccess.Should().BeTrue();
        centralSaved!.SapCardCode.Should().Be("C0999999999001");
        centralSaved.CanonicalVersion.Should().Be(1);
        centralSaved.MasterSyncStatus.Should().Be("Accepted");

        _companyContext.CurrentCompany.Returns(BranchCompany());
        CreateBusinessPartnerData? branchSaved = null;
        _repository.CreateAsync(Arg.Do<CreateBusinessPartnerData>(data => branchSaved = data), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(26);
        _repository.GetByIdAsync(26, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(CreatePartner());

        var branchResult = await new CreateBusinessPartnerCommandHandler(_repository, _transactionRunner, _writer, _companyContext, _sapPolicyRepository)
            .Handle(CreateCommand(), CancellationToken.None);

        branchResult.IsSuccess.Should().BeTrue();
        branchSaved!.SapCardCode.Should().BeNull();
        branchSaved.CanonicalVersion.Should().Be(0);
        branchSaved.MasterSyncStatus.Should().Be("PendingMaster");
        await _sapPolicyRepository.Received(1).GetByCompanyIdAsync(central.CompanyId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_MapsZeroRowsToConcurrencyConflictAndDecodesExpectedVersion()
    {
        var partner = CreatePartner();
        UpdateBusinessPartnerData? saved = null;
        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);
        _repository.UpdateAsync(Arg.Do<UpdateBusinessPartnerData>(data => saved = data), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);

        var result = await CreateUpdateHandler().Handle(UpdateCommand(partner.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == "BP_CONCURRENCY_CONFLICT");
        saved!.ExpectedRowVersion.Should().Equal([1, 2, 3, 4, 5, 6, 7, 8]);
        saved.Code.Should().Be(partner.Code);
        saved.PartnerType.Should().Be(partner.PartnerType);
        saved.IdentificationNumber.Should().Be(partner.IdentificationNumber);
    }

    [Theory]
    [InlineData("PendingMaster", "BP_MASTER_PROPOSAL_IN_FLIGHT")]
    [InlineData("Conflict", "BP_MASTER_PROPOSAL_IN_FLIGHT")]
    [InlineData("LegacyReview", "BP_LEGACY_REVIEW_REQUIRED")]
    public async Task Update_BranchBlocksInFlightAndLegacyReview(string status, string code)
    {
        var partner = CreatePartner();
        partner.MasterSyncStatus = status;
        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);

        var result = await CreateUpdateHandler(BranchCompany()).Handle(UpdateCommand(partner.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.Code == code);
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Update_BranchAllowsOnlyEditableFieldsAndPreservesBaseVersion()
    {
        var partner = CreatePartner();
        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);
        UpdateBusinessPartnerData? saved = null;
        _repository.UpdateAsync(Arg.Do<UpdateBusinessPartnerData>(data => saved = data), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);

        var allowed = await CreateUpdateHandler(BranchCompany()).Handle(UpdateCommand(partner.Id) with { Name = "Cliente Editado" }, CancellationToken.None);

        allowed.IsSuccess.Should().BeTrue();
        saved!.CanonicalVersion.Should().Be(partner.CanonicalVersion);
        saved.MasterSyncStatus.Should().Be("PendingMaster");

        var protectedResult = await CreateUpdateHandler(BranchCompany()).Handle(UpdateCommand(partner.Id) with { Website = "https://branch.invalid" }, CancellationToken.None);
        protectedResult.IsSuccess.Should().BeFalse();
        protectedResult.Errors.Should().Contain(error => error.Code == "BP_PROTECTED_FIELD" && error.Field == "Website");
    }

    [Fact]
    public async Task Update_StandaloneAllowsManagedFieldsWithoutDistributionVersionIncrement()
    {
        var partner = CreatePartner();
        partner.CanonicalVersion = 7;
        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);
        UpdateBusinessPartnerData? saved = null;
        _repository.UpdateAsync(Arg.Do<UpdateBusinessPartnerData>(data => saved = data), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);

        var result = await CreateUpdateHandler().Handle(UpdateCommand(partner.Id) with { Website = "https://standalone.example" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        saved!.CanonicalVersion.Should().Be(7);
        saved.MasterSyncStatus.Should().Be("Accepted");
        saved.SapCardCode.Should().Be(partner.SapCardCode);
    }

    [Fact]
    public async Task Delete_BranchIsRejectedAndZeroRowsMapsToConcurrencyConflict()
    {
        var partner = CreatePartner();
        var branch = await CreateDeleteHandler(BranchCompany()).Handle(new DeleteBusinessPartnerCommand(partner.Id, partner.RowVersion), CancellationToken.None);
        branch.Errors.Should().ContainSingle(error => error.Code == "BP_SYNC_DELETE_NOT_SUPPORTED");

        _repository.GetByIdAsync(partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(partner);
        _repository.DeleteAsync(Arg.Any<DeleteBusinessPartnerData>(), _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        var stale = await CreateDeleteHandler().Handle(new DeleteBusinessPartnerCommand(partner.Id, partner.RowVersion), CancellationToken.None);
        stale.Errors.Should().ContainSingle(error => error.Code == "BP_CONCURRENCY_CONFLICT");
    }

    [Fact]
    public void EditPolicy_IsDerivedFromTrustedCompanyContextShape()
    {
        BusinessPartnerWritePolicy.GetEditPolicy(BranchCompany()).Should().BeEquivalentTo(
            new BusinessPartnerEditPolicyDto(true, false, ["Name", "CommercialName", "Phone", "Email", "Addresses", "Contacts"]));
        BusinessPartnerWritePolicy.GetEditPolicy(Company(syncEnabled: false)).Should().BeEquivalentTo(
            new BusinessPartnerEditPolicyDto(false, true, []));
    }

    [Fact]
    public async Task Writer_SkipsStandaloneOrDisabledCompany()
    {
        var partner = CreatePartner();
        var companyContext = Substitute.For<ICompanyContext>();
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(Company(syncEnabled: false));
        var localOutbox = Substitute.For<ILocalSyncOutboxRepository>();
        var writer = new BusinessPartnerLocalOutboxWriter(companyContext, new SyncEventPayloadFactory(), localOutbox);

        var eventId = await writer.EnqueueAsync(
            partner, SyncOperation.Created, _transactionRunner.Connection, _transactionRunner.Transaction);

        eventId.Should().BeNull();
        await localOutbox.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!, default!, default);
    }

    [Fact]
    public async Task Writer_CreatesSanitizedPayloadAndStableEventIdentity()
    {
        var partner = CreatePartner(sapCardCode: "S0001");
        var companyContext = Substitute.For<ICompanyContext>();
        companyContext.HasActiveCompany.Returns(true);
        companyContext.CurrentCompany.Returns(Company(syncEnabled: true));
        var localOutbox = Substitute.For<ILocalSyncOutboxRepository>();
        CreateLocalSyncOutboxData? captured = null;
        localOutbox.CreateAsync(
                Arg.Do<CreateLocalSyncOutboxData>(value => captured = value),
                _transactionRunner.Connection,
                _transactionRunner.Transaction,
                Arg.Any<CancellationToken>())
            .Returns(10);
        var writer = new BusinessPartnerLocalOutboxWriter(companyContext, new SyncEventPayloadFactory(), localOutbox);

        var eventId = await writer.EnqueueAsync(
            partner, SyncOperation.Created, _transactionRunner.Connection, _transactionRunner.Transaction);

        eventId.Should().NotBeNull().And.NotBe(Guid.Empty);
        captured.Should().NotBeNull();
        captured!.EventId.Should().Be(eventId!.Value);
        captured.EntityGlobalId.Should().Be(partner.GlobalId);
        captured.EntityName.Should().Be("BusinessPartner");
        captured.PayloadJson.Should().Contain("\"operation\":\"Created\"")
            .And.NotContain("SapCardCode")
            .And.NotContain("S0001");
    }

    private CreateBusinessPartnerCommandHandler CreateCreateHandler(CompanyConnectionInfo? company = null)
    {
        _companyContext.CurrentCompany.Returns(company ?? Company(syncEnabled: false));
        _companyContext.HasActiveCompany.Returns(true);
        return new CreateBusinessPartnerCommandHandler(_repository, _transactionRunner, _writer, _companyContext, _sapPolicyRepository);
    }

    private UpdateBusinessPartnerCommandHandler CreateUpdateHandler(CompanyConnectionInfo? company = null)
    {
        _companyContext.CurrentCompany.Returns(company ?? Company(syncEnabled: false));
        _companyContext.HasActiveCompany.Returns(true);
        return new UpdateBusinessPartnerCommandHandler(_repository, _transactionRunner, _writer, _companyContext);
    }

    private DeleteBusinessPartnerCommandHandler CreateDeleteHandler(CompanyConnectionInfo? company = null)
    {
        _companyContext.CurrentCompany.Returns(company ?? Company(syncEnabled: false));
        _companyContext.HasActiveCompany.Returns(true);
        return new DeleteBusinessPartnerCommandHandler(_repository, _transactionRunner, _writer, _companyContext);
    }

    private static CompanyConnectionInfo Company(bool syncEnabled) =>
        new(
            CompanyId: 10,
            CompanyCode: "MASTER",
            CommercialName: "Empresa Master",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode: SapIntegrationMode.None,
            OperationMode: CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: syncEnabled);

    private static CompanyConnectionInfo BranchCompany() =>
        new(
            CompanyId: 20,
            CompanyCode: "BRANCH",
            CommercialName: "Sucursal",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Branch;",
            SapIntegrationMode: SapIntegrationMode.None,
            OperationMode: CompanyOperationMode.Standalone,
            IsMaster: false,
            ParentCompanyId: 10,
            BranchCode: "B20",
            SyncEnabled: true);

    private static BusinessPartnerDto CreatePartner(string name = "Cliente Uno", string? sapCardCode = null)
    {
        return new BusinessPartnerDto
        {
            Id = 25,
            GlobalId = Guid.NewGuid(),
            Code = "CLI-001",
            Name = name,
            CommercialName = "Cliente Comercial",
            PartnerType = "Customer",
            IdentificationTypeId = 1,
            IdentificationNumber = "0999999999001",
            NormalizedIdentificationNumber = "0999999999001",
            CanonicalVersion = 0,
            RowVersion = "AQIDBAUGBwg=",
            MasterSyncStatus = "Accepted",
            Email = "cliente@nuansystem.local",
            Phone = "0999999999",
            CountryCode = "EC",
            IsActive = true,
            AllowsPartialPayments = true,
            ExternalSystem = "ExternalApi",
            ExternalCode = "EXT-001",
            SapCardCode = sapCardCode
        };
    }

    private static BusinessPartnerDto WithPartnerType(BusinessPartnerDto partner, string partnerType)
    {
        partner.PartnerType = partnerType;
        return partner;
    }

    private static CreateBusinessPartnerCommand CreateCommand(string? sapCardCode = null)
    {
        return new CreateBusinessPartnerCommand(
            Name: "Cliente Uno",
            CommercialName: "Cliente Comercial",
            PartnerType: "Customer",
            IdentificationTypeId: 1,
            IdentificationNumber: "0999999999001",
            SupplierGroupId: null,
            SupplierClassId: null,
            EconomicActivityId: null,
            ZoneId: null,
            SupplyMethodId: null,
            Email: "cliente@nuansystem.local",
            Phone: "0999999999",
            Website: null,
            Remarks: null,
            IsActive: true,
            TaxpayerTypeId: null,
            TaxRegimeId: null,
            FiscalCountryId: null,
            TaxpayerType: null,
            IsAccountingRequired: false,
            AppliesRetention: false,
            FiscalRegime: null,
            CountryCode: "EC",
            Province: null,
            City: null,
            CustomerAccountId: null,
            SupplierAccountId: null,
            CustomerAdvanceAccountId: null,
            SupplierAdvanceAccountId: null,
            RetentionAccountId: null,
            BranchId: null,
            DepartmentId: null,
            BusinessLineId: null,
            CostCenterId: null,
            ProjectId: null,
            CostCenterCode: null,
            DefaultExpenseAccountId: null,
            DifferenceAccountId: null,
            RoundingAccountId: null,
            ClearingAccountId: null,
            DiscountAccountId: null,
            AccountingBySupplier: false,
            RequiresProvision: false,
            AllowsAdvance: false,
            AllowsCompensation: false,
            AllowsPartialPayments: true,
            IsPaymentBlocked: false,
            UsesWithholdingBase: false,
            ConciliationRequired: false,
            AccountingPaymentMethodId: null,
            PaymentPriorityId: null,
            ApprovalFlowId: null,
            PaymentDocumentTypeId: null,
            AccountingPaymentMethod: null,
            PaymentPriority: null,
            RequiredPaymentDay: null,
            ApprovalFlow: null,
            PaymentDocumentType: null,
            AveragePaymentDays: 0,
            PaymentTolerancePercent: 0,
            PaymentTermId: null,
            CreditDays: 0,
            CreditLimit: 0,
            DeliveryDays: 0,
            MinimumOrderAmount: 0,
            AllowsBackorder: false,
            PreferredCurrencyCode: null,
            PriceListCode: null,
            AssignedSellerCode: null,
            AssignedBuyerCode: null,
            Incoterm: null,
            CommercialDiscountPercent: 0,
            PurchaseCurrencyCode: null,
            PreferredWarehouseId: null,
            PurchaseSupplierType: null,
            PreferredWarehouseCode: null,
            MinimumOrderQuantity: 0,
            ActiveForImport: false,
            SubjectToEvaluation: false,
            AllowsUrgentPurchases: false,
            AverageDeliveryDays: 0,
            LeadTimeDays: 0,
            DeliveryToleranceDays: 0,
            RequiresPurchaseOrder: false,
            CreditStatus: "Normal",
            Addresses: null,
            Contacts: null,
            BankAccounts: null,
            RetentionSettings: null,
            Notes: null,
            SapFieldMappings: null,
            Attachments: null,
            AuditUserId: 7,
            AuditUserName: "admin");
    }

    private static UpdateBusinessPartnerCommand UpdateCommand(int id)
    {
        var create = CreateCommand();
        return new UpdateBusinessPartnerCommand(
            id,
            "AQIDBAUGBwg=",
            create.Name,
            create.CommercialName,
            create.SupplierGroupId,
            create.SupplierClassId,
            create.EconomicActivityId,
            create.ZoneId,
            create.SupplyMethodId,
            create.Email,
            create.Phone,
            create.Website,
            create.Remarks,
            create.IsActive,
            create.TaxpayerTypeId,
            create.TaxRegimeId,
            create.FiscalCountryId,
            create.TaxpayerType,
            create.IsAccountingRequired,
            create.AppliesRetention,
            create.FiscalRegime,
            create.CountryCode,
            create.Province,
            create.City,
            create.CustomerAccountId,
            create.SupplierAccountId,
            create.CustomerAdvanceAccountId,
            create.SupplierAdvanceAccountId,
            create.RetentionAccountId,
            create.BranchId,
            create.DepartmentId,
            create.BusinessLineId,
            create.CostCenterId,
            create.ProjectId,
            create.CostCenterCode,
            create.DefaultExpenseAccountId,
            create.DifferenceAccountId,
            create.RoundingAccountId,
            create.ClearingAccountId,
            create.DiscountAccountId,
            create.AccountingBySupplier,
            create.RequiresProvision,
            create.AllowsAdvance,
            create.AllowsCompensation,
            create.AllowsPartialPayments,
            create.IsPaymentBlocked,
            create.UsesWithholdingBase,
            create.ConciliationRequired,
            create.AccountingPaymentMethodId,
            create.PaymentPriorityId,
            create.ApprovalFlowId,
            create.PaymentDocumentTypeId,
            create.AccountingPaymentMethod,
            create.PaymentPriority,
            create.RequiredPaymentDay,
            create.ApprovalFlow,
            create.PaymentDocumentType,
            create.AveragePaymentDays,
            create.PaymentTolerancePercent,
            create.PaymentTermId,
            create.CreditDays,
            create.CreditLimit,
            create.DeliveryDays,
            create.MinimumOrderAmount,
            create.AllowsBackorder,
            create.PreferredCurrencyCode,
            create.PriceListCode,
            create.AssignedSellerCode,
            create.AssignedBuyerCode,
            create.Incoterm,
            create.CommercialDiscountPercent,
            create.PurchaseCurrencyCode,
            create.PreferredWarehouseId,
            create.PurchaseSupplierType,
            create.PreferredWarehouseCode,
            create.MinimumOrderQuantity,
            create.ActiveForImport,
            create.SubjectToEvaluation,
            create.AllowsUrgentPurchases,
            create.AverageDeliveryDays,
            create.LeadTimeDays,
            create.DeliveryToleranceDays,
            create.RequiresPurchaseOrder,
            create.CreditStatus,
            create.Addresses,
            create.Contacts,
            create.BankAccounts,
            create.RetentionSettings,
            create.Notes,
            create.SapFieldMappings,
            create.Attachments,
            create.AuditUserId,
            create.AuditUserName);
    }

    private sealed class ImmediateTransactionRunner : ITransactionRunner
    {
        public IDbConnection Connection { get; } = Substitute.For<IDbConnection>();
        public IDbTransaction Transaction { get; } = Substitute.For<IDbTransaction>();
        public bool Committed { get; private set; }
        public bool RolledBack { get; private set; }

        public async Task ExecuteInTenantTransactionAsync(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
        {
            await ExecuteInTenantTransactionAsync<object?>(
                async (connection, transaction, token) =>
                {
                    await operation(connection, transaction, token);
                    return null;
                },
                cancellationToken);
        }

        public async Task<T> ExecuteInTenantTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await operation(Connection, Transaction, cancellationToken);
                Committed = true;
                return result;
            }
            catch
            {
                RolledBack = true;
                throw;
            }
        }
    }
}
