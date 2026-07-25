using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class BusinessPartnerRepository(ITenantConnectionFactory connectionFactory) : IBusinessPartnerRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_BUSINESSPARTNERS_LISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORID";
    private const string LookupsProcedure = "dbo.SP_NA_GET_BUSINESSPARTNERS_LOOKUPS";
    private const string CreateProcedure = "dbo.SP_NA_POST_BUSINESSPARTNERS_CREAR";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORCODIGO";
    private const string ExistsByIdentificationProcedure = "dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR";
    private const string ImportSupplierFromSapProcedure = "dbo.SP_NA_POST_BUSINESSPARTNERS_IMPORTARSAP";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_BUSINESSPARTNERS_ELIMINAR";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<BusinessPartnerDto>> GetAllAsync(string? partnerType, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var partners = await connection.QueryAsync<BusinessPartnerDto>(
            new CommandDefinition(ListProcedure, new { PartnerType = partnerType }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return partners.AsList();
    }

    public async Task<BusinessPartnerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdCoreAsync(id, connection, transaction: null, cancellationToken);
    }

    public Task<BusinessPartnerDto?> GetByIdAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        GetByIdCoreAsync(id, connection, transaction, cancellationToken);

    private static async Task<BusinessPartnerDto?> GetByIdCoreAsync(
        int id,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        var partner = await grid.ReadSingleOrDefaultAsync<BusinessPartnerDto>();
        if (partner is null)
        {
            return null;
        }

        partner.Addresses = (await grid.ReadAsync<BusinessPartnerAddressDto>()).AsList();
        partner.Contacts = (await grid.ReadAsync<BusinessPartnerContactDto>()).AsList();
        partner.BankAccounts = (await grid.ReadAsync<BusinessPartnerBankAccountDto>()).AsList();
        partner.RetentionSettings = (await grid.ReadAsync<BusinessPartnerRetentionSettingDto>()).AsList();
        partner.Notes = await grid.ReadSingleOrDefaultAsync<BusinessPartnerNotesDto>();
        partner.SapFieldMappings = (await grid.ReadAsync<BusinessPartnerSapFieldMappingDto>()).AsList();
        partner.Attachments = (await grid.ReadAsync<BusinessPartnerAttachmentDto>()).AsList();
        return partner;
    }

    public async Task<BusinessPartnerLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(LookupsProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return new BusinessPartnerLookupsDto(
            (await grid.ReadAsync<BusinessPartnerIdentificationTypeLookupDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerPaymentTermLookupDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerCodeNameLookupDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerCodeNameLookupDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerCodeNameLookupDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerGeoLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerGeoLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerRetentionConceptLookupDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<BusinessPartnerLookupOptionDto>()).AsList());
    }

    public async Task<int> CreateAsync(CreateBusinessPartnerData partner, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateCoreAsync(partner, connection, transaction: null, cancellationToken);
    }

    public Task<int> CreateAsync(
        CreateBusinessPartnerData partner,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        CreateCoreAsync(partner, connection, transaction, cancellationToken);

    private static Task<int> CreateCoreAsync(
        CreateBusinessPartnerData partner,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken) =>
        connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, ToParameters(partner), transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByCodeCoreAsync(code, excludingId, connection, transaction: null, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        ExistsByCodeCoreAsync(code, excludingId, connection, transaction, cancellationToken);

    private static async Task<bool> ExistsByCodeCoreAsync(
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return count > 0;
    }

    public async Task<bool> ExistsByIdentificationAsync(int identificationTypeId, string identificationNumber, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByIdentificationCoreAsync(
            identificationTypeId, identificationNumber, excludingId, connection, transaction: null, cancellationToken);
    }

    public Task<bool> ExistsByIdentificationAsync(
        int identificationTypeId,
        string identificationNumber,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        ExistsByIdentificationCoreAsync(
            identificationTypeId, identificationNumber, excludingId, connection, transaction, cancellationToken);

    private static async Task<bool> ExistsByIdentificationCoreAsync(
        int identificationTypeId,
        string identificationNumber,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByIdentificationProcedure,
                new { IdentificationTypeId = identificationTypeId, IdentificationNumber = identificationNumber, ExcluirId = excludingId },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> UpdateAsync(UpdateBusinessPartnerData partner, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateCoreAsync(partner, connection, transaction: null, cancellationToken);
    }

    public Task<bool> UpdateAsync(
        UpdateBusinessPartnerData partner,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        UpdateCoreAsync(partner, connection, transaction, cancellationToken);

    private static async Task<bool> UpdateCoreAsync(
        UpdateBusinessPartnerData partner,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, ToParameters(partner), transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<BusinessPartnerSapImportResultData> ImportSupplierFromSapAsync(
        BusinessPartnerSapImportData supplier,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<BusinessPartnerSapImportResultData>(
            new CommandDefinition(
                ImportSupplierFromSapProcedure,
                supplier,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteCoreAsync(id, deletedByUserId, deletedByUserName, connection, transaction: null, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        DeleteCoreAsync(id, deletedByUserId, deletedByUserName, connection, transaction, cancellationToken);

    private static async Task<bool> DeleteCoreAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    private static object ToParameters(CreateBusinessPartnerData partner)
    {
        return new
        {
            partner.Code,
            partner.Name,
            partner.CommercialName,
            partner.PartnerType,
            partner.IdentificationTypeId,
            partner.IdentificationNumber,
            partner.SupplierGroupId,
            partner.SupplierClassId,
            partner.EconomicActivityId,
            partner.ZoneId,
            partner.SupplyMethodId,
            partner.Email,
            partner.Phone,
            partner.Website,
            partner.Remarks,
            partner.IsActive,
            partner.TaxpayerTypeId,
            partner.TaxRegimeId,
            partner.FiscalCountryId,
            partner.TaxpayerType,
            partner.IsAccountingRequired,
            partner.AppliesRetention,
            partner.FiscalRegime,
            partner.CountryCode,
            partner.Province,
            partner.City,
            partner.CustomerAccountId,
            partner.SupplierAccountId,
            partner.CustomerAdvanceAccountId,
            partner.SupplierAdvanceAccountId,
            partner.RetentionAccountId,
            partner.BranchId,
            partner.DepartmentId,
            partner.BusinessLineId,
            partner.CostCenterId,
            partner.ProjectId,
            partner.CostCenterCode,
            partner.DefaultExpenseAccountId,
            partner.DifferenceAccountId,
            partner.RoundingAccountId,
            partner.ClearingAccountId,
            partner.DiscountAccountId,
            partner.AccountingBySupplier,
            partner.RequiresProvision,
            partner.AllowsAdvance,
            partner.AllowsCompensation,
            partner.AllowsPartialPayments,
            partner.IsPaymentBlocked,
            partner.UsesWithholdingBase,
            partner.ConciliationRequired,
            partner.AccountingPaymentMethodId,
            partner.PaymentPriorityId,
            partner.ApprovalFlowId,
            partner.PaymentDocumentTypeId,
            partner.AccountingPaymentMethod,
            partner.PaymentPriority,
            partner.RequiredPaymentDay,
            partner.ApprovalFlow,
            partner.PaymentDocumentType,
            partner.AveragePaymentDays,
            partner.PaymentTolerancePercent,
            partner.PaymentTermId,
            partner.CreditDays,
            partner.CreditLimit,
            partner.DeliveryDays,
            partner.MinimumOrderAmount,
            partner.AllowsBackorder,
            partner.PreferredCurrencyCode,
            partner.PriceListCode,
            partner.AssignedSellerCode,
            partner.AssignedBuyerCode,
            partner.Incoterm,
            partner.CommercialDiscountPercent,
            partner.PurchaseCurrencyCode,
            partner.PreferredWarehouseId,
            partner.PurchaseSupplierType,
            partner.PreferredWarehouseCode,
            partner.MinimumOrderQuantity,
            partner.ActiveForImport,
            partner.SubjectToEvaluation,
            partner.AllowsUrgentPurchases,
            partner.AverageDeliveryDays,
            partner.LeadTimeDays,
            partner.DeliveryToleranceDays,
            partner.RequiresPurchaseOrder,
            partner.CreditStatus,
            partner.SapCardCode,
            partner.SapCardType,
            partner.SapSyncStatus,
            partner.SapLastSyncAt,
            partner.SapLastError,
            partner.SapEnabled,
            partner.SapMode,
            partner.SapCompanyCode,
            partner.SapRetryCount,
            partner.SyncAsSupplier,
            partner.AllowManualSapRetry,
            partner.RequiresApprovalBeforeSapSync,
            AddressesJson = JsonSerializer.Serialize(partner.Addresses, JsonOptions),
            ContactsJson = JsonSerializer.Serialize(partner.Contacts, JsonOptions),
            BankAccountsJson = JsonSerializer.Serialize(partner.BankAccounts, JsonOptions),
            RetentionSettingsJson = JsonSerializer.Serialize(partner.RetentionSettings, JsonOptions),
            NotesJson = JsonSerializer.Serialize(partner.Notes, JsonOptions),
            SapFieldMappingsJson = JsonSerializer.Serialize(partner.SapFieldMappings, JsonOptions),
            AttachmentsJson = partner.Attachments is null ? null : JsonSerializer.Serialize(partner.Attachments, JsonOptions),
            partner.CreatedByUserId,
            partner.CreatedByUserName
        };
    }

    private static object ToParameters(UpdateBusinessPartnerData partner)
    {
        return new
        {
            partner.Id,
            partner.Code,
            partner.Name,
            partner.CommercialName,
            partner.PartnerType,
            partner.IdentificationTypeId,
            partner.IdentificationNumber,
            partner.SupplierGroupId,
            partner.SupplierClassId,
            partner.EconomicActivityId,
            partner.ZoneId,
            partner.SupplyMethodId,
            partner.Email,
            partner.Phone,
            partner.Website,
            partner.Remarks,
            partner.IsActive,
            partner.TaxpayerTypeId,
            partner.TaxRegimeId,
            partner.FiscalCountryId,
            partner.TaxpayerType,
            partner.IsAccountingRequired,
            partner.AppliesRetention,
            partner.FiscalRegime,
            partner.CountryCode,
            partner.Province,
            partner.City,
            partner.CustomerAccountId,
            partner.SupplierAccountId,
            partner.CustomerAdvanceAccountId,
            partner.SupplierAdvanceAccountId,
            partner.RetentionAccountId,
            partner.BranchId,
            partner.DepartmentId,
            partner.BusinessLineId,
            partner.CostCenterId,
            partner.ProjectId,
            partner.CostCenterCode,
            partner.DefaultExpenseAccountId,
            partner.DifferenceAccountId,
            partner.RoundingAccountId,
            partner.ClearingAccountId,
            partner.DiscountAccountId,
            partner.AccountingBySupplier,
            partner.RequiresProvision,
            partner.AllowsAdvance,
            partner.AllowsCompensation,
            partner.AllowsPartialPayments,
            partner.IsPaymentBlocked,
            partner.UsesWithholdingBase,
            partner.ConciliationRequired,
            partner.AccountingPaymentMethodId,
            partner.PaymentPriorityId,
            partner.ApprovalFlowId,
            partner.PaymentDocumentTypeId,
            partner.AccountingPaymentMethod,
            partner.PaymentPriority,
            partner.RequiredPaymentDay,
            partner.ApprovalFlow,
            partner.PaymentDocumentType,
            partner.AveragePaymentDays,
            partner.PaymentTolerancePercent,
            partner.PaymentTermId,
            partner.CreditDays,
            partner.CreditLimit,
            partner.DeliveryDays,
            partner.MinimumOrderAmount,
            partner.AllowsBackorder,
            partner.PreferredCurrencyCode,
            partner.PriceListCode,
            partner.AssignedSellerCode,
            partner.AssignedBuyerCode,
            partner.Incoterm,
            partner.CommercialDiscountPercent,
            partner.PurchaseCurrencyCode,
            partner.PreferredWarehouseId,
            partner.PurchaseSupplierType,
            partner.PreferredWarehouseCode,
            partner.MinimumOrderQuantity,
            partner.ActiveForImport,
            partner.SubjectToEvaluation,
            partner.AllowsUrgentPurchases,
            partner.AverageDeliveryDays,
            partner.LeadTimeDays,
            partner.DeliveryToleranceDays,
            partner.RequiresPurchaseOrder,
            partner.CreditStatus,
            partner.SapCardCode,
            partner.SapCardType,
            partner.SapSyncStatus,
            partner.SapLastSyncAt,
            partner.SapLastError,
            partner.SapEnabled,
            partner.SapMode,
            partner.SapCompanyCode,
            partner.SapRetryCount,
            partner.SyncAsSupplier,
            partner.AllowManualSapRetry,
            partner.RequiresApprovalBeforeSapSync,
            AddressesJson = JsonSerializer.Serialize(partner.Addresses, JsonOptions),
            ContactsJson = JsonSerializer.Serialize(partner.Contacts, JsonOptions),
            BankAccountsJson = JsonSerializer.Serialize(partner.BankAccounts, JsonOptions),
            RetentionSettingsJson = JsonSerializer.Serialize(partner.RetentionSettings, JsonOptions),
            NotesJson = JsonSerializer.Serialize(partner.Notes, JsonOptions),
            SapFieldMappingsJson = JsonSerializer.Serialize(partner.SapFieldMappings, JsonOptions),
            AttachmentsJson = partner.Attachments is null ? null : JsonSerializer.Serialize(partner.Attachments, JsonOptions),
            partner.UpdatedByUserId,
            partner.UpdatedByUserName
        };
    }
}
