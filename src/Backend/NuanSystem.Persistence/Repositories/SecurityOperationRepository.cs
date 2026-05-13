using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SecurityOperations.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SecurityOperationRepository(IMasterConnectionFactory connectionFactory) : ISecurityOperationRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_OPERACIONSEGURIDADLISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORID";
    private const string CreateProcedure = "dbo.SP_NA_POST_OPERACIONSEGURIDADCREAR";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORCODIGO";
    private const string ExistsByNameProcedure = "dbo.SP_NA_GET_OPERACIONSEGURIDADBUSCARPORNOMBRE";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_OPERACIONSEGURIDADACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_OPERACIONSEGURIDADELIMINAR";

    public async Task<IReadOnlyCollection<SecurityOperationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SecurityOperationDto>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<SecurityOperationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SecurityOperationDto>(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(CreateSecurityOperationData operation, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, operation, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
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

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByNameProcedure, new { Name = name, ExcluirId = (int?)null }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> ExistsByNameAsync(string name, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByNameProcedure, new { Name = name, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> UpdateAsync(UpdateSecurityOperationData operation, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, operation, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default)
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
