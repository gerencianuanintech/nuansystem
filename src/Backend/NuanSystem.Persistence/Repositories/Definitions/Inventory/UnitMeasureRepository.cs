using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory;

public sealed class UnitMeasureRepository(ITenantConnectionFactory connectionFactory) : IUnitMeasureRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_UNIT_OF_MEASURES_LISTAR";
    private const string DetailProcedure = "dbo.SP_NA_GET_UNIT_OF_MEASURES_BUSCARPORID";
    private const string LookupProcedure = "dbo.SP_NA_GET_UNIT_OF_MEASURES_LOOKUP";
    private const string ExistsProcedure = "dbo.SP_NA_GET_UNIT_OF_MEASURES_BUSCARPORCODIGO";
    private const string HistoryProcedure = "dbo.SP_NA_GET_UNIT_OF_MEASURES_HISTORIAL";
    private const string CreateProcedure = "dbo.SP_NA_POST_UNIT_OF_MEASURES_CREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_UNIT_OF_MEASURES_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_UNIT_OF_MEASURES_ELIMINAR";

    public async Task<IReadOnlyCollection<UnitMeasureDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<UnitMeasureDto>(Command(ListProcedure, null, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<UnitMeasureLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<UnitMeasureLookupDto>(Command(LookupProcedure, null, null, cancellationToken))).AsList();
    }

    public async Task<UnitMeasureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<UnitMeasureDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<UnitMeasureDto>(Command(DetailProcedure, new { Id = id }, transaction, cancellationToken));

    public async Task<IReadOnlyCollection<UnitMeasureAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<UnitMeasureAuditChangeDto>(Command(HistoryProcedure, new { Id = id }, null, cancellationToken))).AsList();
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(ExistsProcedure, new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    public Task<int> CreateAsync(CreateUnitMeasureData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(CreateProcedure, data, transaction, cancellationToken));

    public Task<int> UpdateAsync(UpdateUnitMeasureData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(UpdateProcedure, data, transaction, cancellationToken));

    public Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(DeleteProcedure,
            new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, cancellationToken));

    private static CommandDefinition Command(string procedure, object? parameters,
        IDbTransaction? transaction, CancellationToken cancellationToken) =>
        new(procedure, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
