using Dapper;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Abstractions.Data;
using System.Data;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncSettingsRepository(IMasterConnectionFactory connectionFactory) : ISapSyncSettingsRepository
{
    internal const string EnabledSettingsProcedure =
        "dbo.SP_NA_GET_SAPSYNCENTITYSETTINGSHABILITADOS";

    public async Task<IReadOnlyCollection<SapSyncEntitySettingsDto>> GetEnabledEntitiesAsync(int companyId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapSyncEntitySettingsDto>(new CommandDefinition(
            EnabledSettingsProcedure,
            new { CompanyId = companyId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
