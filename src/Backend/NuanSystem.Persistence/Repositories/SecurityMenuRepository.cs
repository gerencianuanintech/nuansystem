using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SecurityMenus.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SecurityMenuRepository(IMasterConnectionFactory connectionFactory) : ISecurityMenuRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_MENUSEGURIDADLISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_MENUSEGURIDADBUSCARPORID";
    private const string CreateProcedure = "dbo.SP_NA_POST_MENUSEGURIDADCREAR";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_MENUSEGURIDADBUSCARPORCODIGO";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_MENUSEGURIDADACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_MENUSEGURIDADELIMINAR";

    public async Task<IReadOnlyCollection<SecurityMenuDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SecurityMenuDto>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<SecurityMenuDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SecurityMenuDto>(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(CreateSecurityMenuData menu, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, menu, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { Code = code, ExcluirId = (int?)null }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { Code = code, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> UpdateAsync(UpdateSecurityMenuData menu, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, menu, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

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
}
