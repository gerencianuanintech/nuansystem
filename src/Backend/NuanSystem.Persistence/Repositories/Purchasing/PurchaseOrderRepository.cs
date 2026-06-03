using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Persistence.Repositories.Purchasing;

public sealed class PurchaseOrderRepository(ITenantConnectionFactory connectionFactory) : IPurchaseOrderRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_PURCHASEORDERS_LISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_PURCHASEORDERS_BUSCARPORID";
    private const string LookupsProcedure = "dbo.SP_NA_GET_PURCHASEORDERS_LOOKUPS";
    private const string CreateProcedure = "dbo.SP_NA_POST_PURCHASEORDERS_CREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_PURCHASEORDERS_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_PURCHASEORDERS_ELIMINAR";
    private const string UpdateStatusProcedure = "dbo.SP_NA_PATCH_PURCHASEORDERS_ESTADO";
    private const string AddSapLogProcedure = "dbo.SP_NA_POST_PURCHASEORDERS_SAPLOG";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<PurchaseOrderSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var orders = await connection.QueryAsync<PurchaseOrderSummaryDto>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return orders.AsList();
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        var order = await grid.ReadSingleOrDefaultAsync<PurchaseOrderDto>();
        if (order is null)
        {
            return null;
        }

        order.Lines = (await grid.ReadAsync<PurchaseOrderLineDto>()).AsList();
        order.Addresses = (await grid.ReadAsync<PurchaseOrderAddressDto>()).AsList();
        order.Approvals = (await grid.ReadAsync<PurchaseOrderApprovalDto>()).AsList();
        order.RelatedDocuments = (await grid.ReadAsync<PurchaseOrderRelatedDocumentDto>()).AsList();
        order.Attachments = (await grid.ReadAsync<PurchaseOrderAttachmentDto>()).AsList();
        order.SapLogs = (await grid.ReadAsync<PurchaseOrderSapSyncLogDto>()).AsList();
        return order;
    }

    public async Task<PurchaseOrderLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(LookupsProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return new PurchaseOrderLookupsDto(
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderWarehouseLookupDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderTaxLookupDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList(),
            (await grid.ReadAsync<PurchaseOrderLookupOptionDto>()).AsList());
    }

    public async Task<int> CreateAsync(PurchaseOrderPersistData order, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, ToParameters(order), cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateAsync(PurchaseOrderPersistData order, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, ToParameters(order), cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> UpdateIfEditableAsync(
        PurchaseOrderPersistData order,
        IReadOnlyCollection<string> expectedCurrentStatuses,
        CancellationToken cancellationToken = default)
    {
        if (expectedCurrentStatuses is null || expectedCurrentStatuses.Count == 0)
        {
            throw new ArgumentException("At least one expected current status is required.", nameof(expectedCurrentStatuses));
        }

        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                UpdateProcedure,
                ToParameters(order, JsonSerializer.Serialize(expectedCurrentStatuses, JsonOptions)),
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteIfCurrentAsync(
        int id,
        IReadOnlyCollection<string> expectedCurrentStatuses,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default)
    {
        if (expectedCurrentStatuses is null || expectedCurrentStatuses.Count == 0)
        {
            throw new ArgumentException("At least one expected current status is required.", nameof(expectedCurrentStatuses));
        }

        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new
                {
                    Id = id,
                    ExpectedStatusesJson = JsonSerializer.Serialize(expectedCurrentStatuses, JsonOptions),
                    DeletedByUserId = deletedByUserId,
                    DeletedByUserName = deletedByUserName
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status, int? userId, string? userName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                UpdateStatusProcedure,
                new { Id = id, Status = status, UpdatedByUserId = userId, UpdatedByUserName = userName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> UpdateStatusIfCurrentAsync(
        int id,
        string nextStatus,
        IReadOnlyCollection<string> expectedCurrentStatuses,
        int? userId,
        string? userName,
        CancellationToken cancellationToken = default)
    {
        if (expectedCurrentStatuses is null || expectedCurrentStatuses.Count == 0)
        {
            throw new ArgumentException("At least one expected current status is required.", nameof(expectedCurrentStatuses));
        }

        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                UpdateStatusProcedure,
                new
                {
                    Id = id,
                    Status = nextStatus,
                    ExpectedStatusesJson = JsonSerializer.Serialize(expectedCurrentStatuses, JsonOptions),
                    UpdatedByUserId = userId,
                    UpdatedByUserName = userName
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<PurchaseOrderSapSyncLogDto> AddSapLogAsync(int id, string process, string status, string? message, int? userId, string? userName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<PurchaseOrderSapSyncLogDto>(
            new CommandDefinition(
                AddSapLogProcedure,
                new { PurchaseOrderId = id, Process = process, Status = status, Message = message, UserId = userId, UserName = userName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    private static object ToParameters(PurchaseOrderPersistData order, string? expectedStatusesJson = null)
    {
        return new
        {
            order.Id,
            order.BranchId,
            order.DocumentSeriesId,
            order.SeriesCode,
            order.DocumentNumber,
            order.SupplierId,
            order.SupplierCode,
            order.SupplierName,
            order.SupplierTaxId,
            order.ContactName,
            order.Phone,
            order.Email,
            order.DocumentDate,
            order.DeliveryDate,
            order.CurrencyCode,
            order.ExchangeRate,
            order.PaymentTermId,
            order.PriceListId,
            order.BuyerId,
            order.MainWarehouseId,
            order.ProjectId,
            order.CostCenterId,
            order.PurchaseTypeId,
            order.Comments,
            order.Status,
            order.Subtotal,
            order.DiscountPercent,
            order.DiscountAmount,
            order.TaxAmount,
            order.TotalAmount,
            order.TotalItems,
            order.TotalQuantity,
            order.TotalWeight,
            order.SapObjectType,
            order.SapStatus,
            LinesJson = JsonSerializer.Serialize(order.Lines, JsonOptions),
            AddressesJson = JsonSerializer.Serialize(order.Addresses, JsonOptions),
            RelatedDocumentsJson = JsonSerializer.Serialize(order.RelatedDocuments, JsonOptions),
            AttachmentsJson = JsonSerializer.Serialize(order.Attachments, JsonOptions),
            ExpectedStatusesJson = expectedStatusesJson,
            order.AuditUserId,
            order.AuditUserName
        };
    }
}
