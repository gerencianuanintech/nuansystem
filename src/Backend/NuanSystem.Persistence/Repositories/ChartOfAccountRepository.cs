using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class ChartOfAccountRepository(ITenantConnectionFactory connectionFactory) : IChartOfAccountRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_CHART_OF_ACCOUNTS_LISTAR";
    private const string LookupProcedure = "dbo.SP_NA_GET_CHART_OF_ACCOUNTS_LOOKUP";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_CHART_OF_ACCOUNTS_BUSCARPORID";
    private const string CreateProcedure = "dbo.SP_NA_POST_CHART_OF_ACCOUNTS_CREAR";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_CHART_OF_ACCOUNTSBUSCARPORCODIGO";
    private const string HasChildrenProcedure = "dbo.SP_NA_GET_CHART_OF_ACCOUNTS_TIENEHIJAS";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_CHART_OF_ACCOUNTS_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_CHART_OF_ACCOUNTS_ELIMINAR";

    public async Task<IReadOnlyCollection<ChartOfAccountDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var accounts = await connection.QueryAsync<ChartOfAccountDto>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return accounts.AsList();
    }

    public async Task<IReadOnlyCollection<ChartOfAccountLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var accounts = await connection.QueryAsync<ChartOfAccountLookupDto>(
            new CommandDefinition(LookupProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return accounts.AsList();
    }

    public async Task<ChartOfAccountDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ChartOfAccountDto>(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(CreateChartOfAccountData account, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, account, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> ExistsByCodeAsync(int companyId, string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { CompanyId = companyId, Code = code, ExcluirId = (int?)null }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> ExistsByCodeAsync(int companyId, string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { CompanyId = companyId, Code = code, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> HasChildrenAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(HasChildrenProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> UpdateAsync(UpdateChartOfAccountData account, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, account, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(DeleteProcedure, new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }
}
