using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.OperationalCatalogs.Dtos;

namespace NuanSystem.Persistence.Repositories.OperationalCatalogs;

public sealed class OperationalCatalogRepository(ITenantConnectionFactory connectionFactory) : IOperationalCatalogRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_OPERATIONALCATALOG_LISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_OPERATIONALCATALOG_BUSCARPORID";
    private const string LookupProcedure = "dbo.SP_NA_GET_OPERATIONALCATALOG_LOOKUP";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_OPERATIONALCATALOG_BUSCARPORCODIGO";
    private const string CreateProcedure = "dbo.SP_NA_POST_OPERATIONALCATALOG_CREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_OPERATIONALCATALOG_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_OPERATIONALCATALOG_ELIMINAR";

    public async Task<IReadOnlyCollection<OperationalCatalogDto>> GetAllAsync(OperationalCatalogFilterData filter, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<OperationalCatalogDto>(
            new CommandDefinition(ListProcedure, filter, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return items.AsList();
    }

    public async Task<OperationalCatalogDto?> GetByIdAsync(string catalogKey, int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<OperationalCatalogDto>(
            new CommandDefinition(GetByIdProcedure, new { CatalogKey = catalogKey, Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<IReadOnlyCollection<OperationalCatalogLookupDto>> GetLookupAsync(string catalogKey, string? parentCatalogKey, string? parentCode, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<OperationalCatalogLookupDto>(
            new CommandDefinition(LookupProcedure, new { CatalogKey = catalogKey, ParentCatalogKey = parentCatalogKey, ParentCode = parentCode, ActiveOnly = activeOnly }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return items.AsList();
    }

    public async Task<bool> ExistsByCodeAsync(string catalogKey, string code, int? excludedId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var exists = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { CatalogKey = catalogKey, Code = code, ExcludedId = excludedId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return exists == 1;
    }

    public async Task<int> CreateAsync(CreateOperationalCatalogData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, data, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateAsync(UpdateOperationalCatalogData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, data, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(string catalogKey, int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(DeleteProcedure, new { CatalogKey = catalogKey, Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }
}
