using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory;

public sealed class ItemTypeRepository(ITenantConnectionFactory connectionFactory) : IItemTypeRepository
{
    public async Task<IReadOnlyCollection<ItemTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<ItemTypeDto>(Command(
            "dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_LISTAR", cancellationToken));
        return items.AsList();
    }

    public async Task<IReadOnlyCollection<ItemTypeLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<ItemTypeLookupDto>(Command(
            "dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_LOOKUP", cancellationToken));
        return items.AsList();
    }

    public async Task<ItemTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ItemTypeDto>(Command(
            "dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_BUSCARPORID",
            new { Id = id },
            cancellationToken));
    }

    public async Task<IReadOnlyCollection<ItemTypeAuditChangeDto>> GetHistoryAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<ItemTypeAuditChangeDto>(Command(
            "dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_HISTORIAL",
            new { Id = id },
            cancellationToken));
        return items.AsList();
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        int? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_GET_GENERAL_INVENTORY_ITEMTYPES_BUSCARPORCODIGO",
            new { Code = code, ExcluirId = excludingId },
            cancellationToken));
        return count > 0;
    }

    public async Task<CreateItemTypeResult> CreateAsync(
        CreateItemTypeData data,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_POST_GENERAL_INVENTORY_ITEMTYPES_CREAR", data, cancellationToken));
        return result < 0
            ? new CreateItemTypeResult(null, DuplicateCode: true)
            : new CreateItemTypeResult(result, DuplicateCode: false);
    }

    public async Task<UpdateItemTypeResult> UpdateAsync(
        UpdateItemTypeData data,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_PUT_GENERAL_INVENTORY_ITEMTYPES_ACTUALIZAR", data, cancellationToken));
        return result switch
        {
            -1 => new UpdateItemTypeResult(Updated: false, DuplicateCode: true, SystemProtected: false),
            -2 => new UpdateItemTypeResult(Updated: false, DuplicateCode: false, SystemProtected: true),
            _ => new UpdateItemTypeResult(Updated: result > 0, DuplicateCode: false, SystemProtected: false)
        };
    }

    public async Task<DeleteItemTypeResult> DeleteAsync(
        DeleteItemTypeData data,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_DELETE_GENERAL_INVENTORY_ITEMTYPES_ELIMINAR", data, cancellationToken));
        return result switch
        {
            -2 => new DeleteItemTypeResult(Deleted: false, SystemProtected: true, InUse: false),
            -3 => new DeleteItemTypeResult(Deleted: false, SystemProtected: false, InUse: true),
            _ => new DeleteItemTypeResult(Deleted: result > 0, SystemProtected: false, InUse: false)
        };
    }

    private static CommandDefinition Command(string name, CancellationToken cancellationToken) =>
        new(name, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);

    private static CommandDefinition Command(string name, object parameters, CancellationToken cancellationToken) =>
        new(name, parameters, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
