using FluentAssertions;
using NSubstitute;
using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerSyncPublishingTests
{
    private readonly IBusinessPartnerRepository _repository = Substitute.For<IBusinessPartnerRepository>();
    private readonly IBusinessPartnerLocalOutboxWriter _writer = Substitute.For<IBusinessPartnerLocalOutboxWriter>();
    private readonly ImmediateTransactionRunner _transactionRunner = new();

    [Fact]
    public async Task Create_WritesLocalOutboxInsideTheSameTransaction()
    {
        var partner = CreatePartner();
        _repository.ExistsByCodeAsync("CLI-001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync(1, "0999999999001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
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
        _repository.ExistsByCodeAsync("CLI-001", partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync(1, "0999999999001", partner.Id, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
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
        _repository.DeleteAsync(partner.Id, 7, "admin", _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(true);
        var handler = CreateDeleteHandler();

        var result = await handler.Handle(new DeleteBusinessPartnerCommand(partner.Id, 7, "admin"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _writer.Received(1).EnqueueAsync(
            partner, SyncOperation.Deleted, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_RollsBackWhenLocalOutboxFails()
    {
        var partner = CreatePartner();
        _repository.ExistsByCodeAsync("CLI-001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsByIdentificationAsync(1, "0999999999001", null, _transactionRunner.Connection, _transactionRunner.Transaction, Arg.Any<CancellationToken>()).Returns(false);
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

    private CreateBusinessPartnerCommandHandler CreateCreateHandler()
    {
        return new CreateBusinessPartnerCommandHandler(_repository, _transactionRunner, _writer);
    }

    private UpdateBusinessPartnerCommandHandler CreateUpdateHandler()
    {
        return new UpdateBusinessPartnerCommandHandler(_repository, _transactionRunner, _writer);
    }

    private DeleteBusinessPartnerCommandHandler CreateDeleteHandler()
    {
        return new DeleteBusinessPartnerCommandHandler(_repository, _transactionRunner, _writer);
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
