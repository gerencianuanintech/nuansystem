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
        return await connection.QuerySingleOrDefaultAsync<CarrierDetailDto>(Command("dbo.SP_NA_GET_CARRIERS_BUSCARPORID", new { Id = id }, cancellationToken));
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
        var count = await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_GET_CARRIERSBUSCARPORCODIGO", new { Code = code, ExcluirId = excludingId }, cancellationToken));
        return count > 0;
    }

    public async Task<CreateCarrierResult> CreateAsync(CreateCarrierData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_CARRIERS_CREAR", data, cancellationToken));
        return result < 0
            ? new CreateCarrierResult(null, DuplicateCode: true)
            : new CreateCarrierResult(result, DuplicateCode: false);
    }

    public async Task<UpdateCarrierResult> UpdateAsync(UpdateCarrierData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var result = await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_PUT_CARRIERS_ACTUALIZAR", data, cancellationToken));
        return result < 0
            ? new UpdateCarrierResult(Updated: false, DuplicateCode: true)
            : new UpdateCarrierResult(Updated: result > 0, DuplicateCode: false);
    }

    public async Task<bool> DeleteAsync(DeleteCarrierData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_DELETE_CARRIERS_ELIMINAR", data, cancellationToken)) > 0;
    }

    private static CommandDefinition Command(string name, CancellationToken cancellationToken) =>
        new(name, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);

    private static CommandDefinition Command(string name, object parameters, CancellationToken cancellationToken) =>
        new(name, parameters, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
