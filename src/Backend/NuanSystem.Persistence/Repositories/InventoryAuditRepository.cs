using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class InventoryAuditRepository(ITenantConnectionFactory connectionFactory) : IInventoryAuditRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_INVENTORYCHANGESLISTAR";

    public async Task<IReadOnlyCollection<SecurityChangeDto>> GetChangesAsync(
        string entityName,
        string recordId,
        int take,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var changes = await connection.QueryAsync<SecurityChangeDto>(
            new CommandDefinition(
                ListProcedure,
                new { EntityName = entityName, RecordId = recordId, Take = take },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return changes.AsList();
    }
}
