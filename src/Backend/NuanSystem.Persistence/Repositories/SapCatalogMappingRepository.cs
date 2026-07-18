using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SapCatalogMappingRepository(IMasterConnectionFactory connectionFactory) : ISapCatalogMappingRepository
{
    public async Task<IReadOnlyCollection<SapCatalogMappingDto>> GetByCompanyIdAsync(int companyId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapCatalogMappingDto>(new CommandDefinition(
            "dbo.SP_NA_GET_SAPCATALOGMAPPINGS_LISTARPOREMPRESA", new { CompanyId = companyId },
            commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task ReplaceAsync(ReplaceSapCatalogMappingsData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.SP_NA_PUT_SAPCATALOGMAPPINGS_REEMPLAZAR",
            new { data.CompanyId, MappingsJson = JsonSerializer.Serialize(data.Mappings, new JsonSerializerOptions(JsonSerializerDefaults.Web)), data.AuditUserId, data.AuditUserName },
            commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
    }
}
