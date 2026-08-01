using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Carriers.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class CarrierRepository(ITenantConnectionFactory connectionFactory) : ICarrierRepository
{
    public async Task<IReadOnlyCollection<CarrierListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<CarrierListItemDto>(Command("dbo.SP_NA_GET_CARRIERS_LISTAR", cancellationToken));
        return items.AsList();
    }

    public async Task<IReadOnlyCollection<CarrierLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<CarrierLookupDto>(Command("dbo.SP_NA_GET_CARRIERS_LOOKUP", cancellationToken));
        return items.AsList();
    }

    public async Task<CarrierDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdCoreAsync(id, connection, transaction: null, cancellationToken);
    }

    public Task<CarrierDetailDto?> GetByIdAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return GetByIdCoreAsync(id, connection, transaction, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CarrierAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<CarrierAuditChangeDto>(Command("dbo.SP_NA_GET_CARRIERS_HISTORIAL", new { Id = id }, cancellationToken));
        return items.AsList();
    }

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
        CancellationToken cancellationToken = default)
    {
        return ExistsByCodeCoreAsync(code, excludingId, connection, transaction, cancellationToken);
    }

    public async Task<CreateCarrierResult> CreateAsync(CreateCarrierData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateCoreAsync(data, connection, transaction: null, cancellationToken);
    }

    public Task<CreateCarrierResult> CreateAsync(
        CreateCarrierData data,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return CreateCoreAsync(data, connection, transaction, cancellationToken);
    }

    private static async Task<CreateCarrierResult> CreateCoreAsync(
        CreateCarrierData data,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var result = await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_POST_CARRIERS_CREAR", data, transaction, cancellationToken));
        return result < 0
            ? new CreateCarrierResult(null, DuplicateCode: true)
            : new CreateCarrierResult(result, DuplicateCode: false);
    }

    public async Task<UpdateCarrierResult> UpdateAsync(UpdateCarrierData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateCoreAsync(data, connection, transaction: null, cancellationToken);
    }

    public Task<UpdateCarrierResult> UpdateAsync(
        UpdateCarrierData data,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return UpdateCoreAsync(data, connection, transaction, cancellationToken);
    }

    private static async Task<UpdateCarrierResult> UpdateCoreAsync(
        UpdateCarrierData data,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var result = await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_PUT_CARRIERS_ACTUALIZAR", data, transaction, cancellationToken));
        return result < 0
            ? new UpdateCarrierResult(Updated: false, DuplicateCode: true)
            : new UpdateCarrierResult(Updated: result > 0, DuplicateCode: false);
    }

    public async Task<bool> DeleteAsync(DeleteCarrierData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteCoreAsync(data, connection, transaction: null, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        DeleteCarrierData data,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return DeleteCoreAsync(data, connection, transaction, cancellationToken);
    }

    private static Task<CarrierDetailDto?> GetByIdCoreAsync(
        int id,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        return connection.QuerySingleOrDefaultAsync<CarrierDetailDto>(Command(
            "dbo.SP_NA_GET_CARRIERS_BUSCARPORID", new { Id = id }, transaction, cancellationToken));
    }

    private static async Task<bool> ExistsByCodeCoreAsync(
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var count = await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_GET_CARRIERSBUSCARPORCODIGO",
            new { Code = code, ExcluirId = excludingId },
            transaction,
            cancellationToken));
        return count > 0;
    }

    private static async Task<bool> DeleteCoreAsync(
        DeleteCarrierData data,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        return await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_DELETE_CARRIERS_ELIMINAR", data, transaction, cancellationToken)) > 0;
    }

    private static CommandDefinition Command(string name, CancellationToken cancellationToken) =>
        new(name, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);

    private static CommandDefinition Command(string name, object parameters, CancellationToken cancellationToken) =>
        new(name, parameters, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);

    private static CommandDefinition Command(
        string name,
        object parameters,
        IDbTransaction? transaction,
        CancellationToken cancellationToken) =>
        new(name, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
