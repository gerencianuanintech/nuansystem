using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SapPaymentTermImportRepository(ITenantConnectionFactory connectionFactory)
    : ISapPaymentTermImportRepository
{
    public async Task<SapPaymentTermUpsertResult> UpsertAsync(
        SapPaymentTermUpsertData data,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SapPaymentTermUpsertResult>(new CommandDefinition(
            "SP_NA_POST_BUSINESSPARTNERPAYMENTTERMS_IMPORTARSAP",
            data,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }
}
