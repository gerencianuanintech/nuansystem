using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSyncPublishingTests
{
    private readonly IBusinessPartnerRepository _repository = Substitute.For<IBusinessPartnerRepository>();
    private readonly ISyncEventPublisher _syncEventPublisher = Substitute.For<ISyncEventPublisher>();
    private readonly ICompanyContext _companyContext = Substitute.For<ICompanyContext>();

    [Fact]
    public async Task Create_PublishesSyncEvent_WhenCompanyContextIsActive()
    {
        SyncPublishRequest? captured = null;
        var partner = CreatePartner();
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.ExistsByCodeAsync("CLI-001", cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync(1, "0999999999001", cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), Arg.Any<CancellationToken>()).Returns(partner.Id);
        _repository.GetByIdAsync(partner.Id, Arg.Any<CancellationToken>()).Returns(partner);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.CompanyId.Should().Be(10);
        captured.EntityName.Should().Be("BusinessPartner");
        captured.EntityGlobalId.Should().Be(partner.GlobalId);
        captured.EntityGlobalId.Should().NotBe(Guid.Empty);
        captured.EntityCode.Should().Be(partner.Code);
        captured.Operation.Should().Be(SyncOperation.Created);
        captured.Payload.Should().BeOfType<BusinessPartnerSyncPayload>();
        captured.Payload.Should().NotBeAssignableTo<BusinessPartnerDto>();
    }

    [Fact]
    public async Task Update_PublishesSyncEvent_WithGlobalIdAndCode()
    {
        SyncPublishRequest? captured = null;
        var partner = CreatePartner(name: "Cliente Actualizado");
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.GetByIdAsync(partner.Id, Arg.Any<CancellationToken>()).Returns(partner);
        _repository.ExistsByCodeAsync("CLI-001", partner.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync(1, "0999999999001", partner.Id, Arg.Any<CancellationToken>()).Returns(false);
        _repository.UpdateAsync(Arg.Any<UpdateBusinessPartnerData>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateUpdateHandler();

        var result = await handler.Handle(UpdateCommand(partner.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityGlobalId.Should().Be(partner.GlobalId);
        captured.EntityCode.Should().Be(partner.Code);
        captured.Operation.Should().Be(SyncOperation.Updated);
    }

    [Fact]
    public async Task Delete_PublishesDeletedSyncEvent_AfterLogicalDelete()
    {
        SyncPublishRequest? captured = null;
        var partner = CreatePartner();
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.GetByIdAsync(partner.Id, Arg.Any<CancellationToken>()).Returns(partner);
        _repository.DeleteAsync(partner.Id, 7, "admin", Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateDeleteHandler();

        var result = await handler.Handle(new DeleteBusinessPartnerCommand(partner.Id, 7, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.EntityGlobalId.Should().Be(partner.GlobalId);
        captured.EntityCode.Should().Be(partner.Code);
        captured.Operation.Should().Be(SyncOperation.Deleted);
    }

    [Fact]
    public async Task Create_DoesNotPublish_WhenNoActiveCompanyContext()
    {
        var partner = CreatePartner();
        ConfigureNoActiveCompany();
        _repository.ExistsByCodeAsync("CLI-001", cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync(1, "0999999999001", cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), Arg.Any<CancellationToken>()).Returns(partner.Id);
        _repository.GetByIdAsync(partner.Id, Arg.Any<CancellationToken>()).Returns(partner);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _syncEventPublisher.DidNotReceiveWithAnyArgs().PublishAsync(default!, default);
    }

    [Fact]
    public async Task Create_KeepsStandaloneCrudWorking_WhenPublisherSkipsForDisabledSync()
    {
        var partner = CreatePartner();
        ConfigureActiveCompany(syncEnabled: false);
        _syncEventPublisher.PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(false, null, "La empresa no tiene sincronizacion habilitada.")));
        _repository.ExistsByCodeAsync("CLI-001", cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync(1, "0999999999001", cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), Arg.Any<CancellationToken>()).Returns(partner.Id);
        _repository.GetByIdAsync(partner.Id, Arg.Any<CancellationToken>()).Returns(partner);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _syncEventPublisher.Received(1).PublishAsync(Arg.Any<SyncPublishRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_DoesNotIncludeSapCardCode_InSyncPayload()
    {
        SyncPublishRequest? captured = null;
        var partner = CreatePartner(sapCardCode: "S0001");
        ConfigureActiveCompany(syncEnabled: true);
        ConfigureSyncPublisher(request => captured = request);
        _repository.ExistsByCodeAsync("CLI-001", cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync(1, "0999999999001", cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), Arg.Any<CancellationToken>()).Returns(partner.Id);
        _repository.GetByIdAsync(partner.Id, Arg.Any<CancellationToken>()).Returns(partner);
        var handler = CreateCreateHandler();

        var result = await handler.Handle(CreateCommand(sapCardCode: "S0001"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        var payload = captured!.Payload.Should().BeOfType<BusinessPartnerSyncPayload>().Subject;
        payload.GlobalId.Should().Be(partner.GlobalId);
        payload.Code.Should().Be(partner.Code);
        payload.GetType().GetProperty(nameof(BusinessPartnerDto.SapCardCode)).Should().BeNull();
    }

    private CreateBusinessPartnerCommandHandler CreateCreateHandler()
    {
        return new CreateBusinessPartnerCommandHandler(_repository, _syncEventPublisher, _companyContext);
    }

    private UpdateBusinessPartnerCommandHandler CreateUpdateHandler()
    {
        return new UpdateBusinessPartnerCommandHandler(_repository, _syncEventPublisher, _companyContext);
    }

    private DeleteBusinessPartnerCommandHandler CreateDeleteHandler()
    {
        return new DeleteBusinessPartnerCommandHandler(_repository, _syncEventPublisher, _companyContext);
    }

    private void ConfigureSyncPublisher(Action<SyncPublishRequest> capture)
    {
        _syncEventPublisher.PublishAsync(Arg.Do(capture), Arg.Any<CancellationToken>())
            .Returns(Result<SyncPublishResult>.Success(new SyncPublishResult(true, 45, "Evento publicado.")));
    }

    private void ConfigureActiveCompany(bool syncEnabled)
    {
        _companyContext.HasActiveCompany.Returns(true);
        _companyContext.CurrentCompany.Returns(new CompanyConnectionInfo(
            CompanyId: 10,
            CompanyCode: "MASTER",
            CommercialName: "Empresa Master",
            DatabaseEngine: DatabaseEngine.SqlServer,
            ConnectionString: "Server=(local);Database=NuanSystem_Tenant;",
            SapIntegrationMode: SapIntegrationMode.None,
            OperationMode: CompanyOperationMode.Standalone,
            IsMaster: true,
            SyncEnabled: syncEnabled));
    }

    private void ConfigureNoActiveCompany()
    {
        _companyContext.HasActiveCompany.Returns(false);
        _companyContext.CurrentCompany.Returns((CompanyConnectionInfo?)null);
    }

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
            Email = "cliente@nuansystem.local",
            Phone = "0999999999",
            IsActive = true,
            ExternalSystem = "ExternalApi",
            ExternalCode = "EXT-001",
            SapCardCode = sapCardCode
        };
    }

    private static CreateBusinessPartnerCommand CreateCommand(string? sapCardCode = null)
    {
        return new CreateBusinessPartnerCommand(
            Code: "CLI-001",
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
            SapCardCode: sapCardCode,
            SapCardType: null,
            SapSyncStatus: null,
            SapLastSyncAt: null,
            SapLastError: null,
            SapEnabled: false,
            SapMode: null,
            SapCompanyCode: null,
            SapRetryCount: 0,
            SyncAsSupplier: false,
            AllowManualSapRetry: false,
            RequiresApprovalBeforeSapSync: false,
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
            create.Code,
            create.Name,
            create.CommercialName,
            create.PartnerType,
            create.IdentificationTypeId,
            create.IdentificationNumber,
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
            create.SapCardCode,
            create.SapCardType,
            create.SapSyncStatus,
            create.SapLastSyncAt,
            create.SapLastError,
            create.SapEnabled,
            create.SapMode,
            create.SapCompanyCode,
            create.SapRetryCount,
            create.SyncAsSupplier,
            create.AllowManualSapRetry,
            create.RequiresApprovalBeforeSapSync,
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
}
