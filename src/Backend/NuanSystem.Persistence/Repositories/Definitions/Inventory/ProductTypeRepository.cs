using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory;

public sealed class ProductTypeRepository(ITenantConnectionFactory connectionFactory) : IProductTypeRepository
{
    private const string Prefix = "dbo.SP_NA_GET_GENERAL_INVENTORY_PRODUCTTYPES_";
    private const string ListProcedure = Prefix + "LISTAR";
    private const string DetailProcedure = Prefix + "BUSCARPORID";
    private const string LookupProcedure = Prefix + "LOOKUP";
    private const string ExistsProcedure = Prefix + "BUSCARPORCODIGO";
    private const string HistoryProcedure = Prefix + "HISTORIAL";
    private const string CreateProcedure = "dbo.SP_NA_POST_GENERAL_INVENTORY_PRODUCTTYPES_CREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_GENERAL_INVENTORY_PRODUCTTYPES_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_GENERAL_INVENTORY_PRODUCTTYPES_ELIMINAR";

    public async Task<IReadOnlyCollection<ProductTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ProductTypeDto>(Command(ListProcedure, null, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<ProductTypeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ProductTypeLookupDto>(Command(LookupProcedure, null, null, cancellationToken))).AsList();
    }

    public async Task<ProductTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<ProductTypeDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<ProductTypeDto>(Command(DetailProcedure, new { Id = id }, transaction, cancellationToken));

    public async Task<IReadOnlyCollection<ProductTypeAuditChangeDto>> GetHistoryAsync(int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ProductTypeAuditChangeDto>(
            Command(HistoryProcedure, new { Id = id }, null, cancellationToken))).AsList();
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection,
        IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(ExistsProcedure,
            new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    public Task<int> CreateAsync(CreateProductTypeData data, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(CreateProcedure, data, transaction, cancellationToken));

    public Task<int> UpdateAsync(UpdateProductTypeData data, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(UpdateProcedure, data, transaction, cancellationToken));

    public Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection,
        IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(DeleteProcedure,
            new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName },
            transaction, cancellationToken));

    private static CommandDefinition Command(string procedure, object? parameters,
        IDbTransaction? transaction, CancellationToken cancellationToken) =>
        new(procedure, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
