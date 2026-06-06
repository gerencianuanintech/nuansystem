using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;

namespace NuanSystem.Persistence.Repositories.Documents;

public sealed class SecurityDocumentNumberingService(ITenantConnectionFactory connectionFactory)
    : ISecurityDocumentNumberingService
{
    private const string ReserveNumberProcedure = "dbo.SP_NA_POST_SECURITYDOCUMENTSERIES_RESERVARNUMERO";

    public async Task<ReserveSecurityDocumentNumberResult> ReserveNumberAsync(
        int securityDocumentSeriesId,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<ReserveSecurityDocumentNumberResult>(
            new CommandDefinition(
                ReserveNumberProcedure,
                new
                {
                    Id = securityDocumentSeriesId,
                    UpdatedByUserId = auditUserId,
                    UpdatedByUserName = auditUserName
                },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }
}
